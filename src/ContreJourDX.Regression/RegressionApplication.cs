using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

using ContreJourDX.Gameplay;
using ContreJourDX.Saving;

using FarseerPhysics.Dynamics;

using Mokus2D.Game;
using Mokus2D.Visual;

namespace ContreJourDX.Regression
{
    public class RegressionApplication : ContreJourDXApplication
    {
        private const float TimeStep = 1f / 60f;

        private const int MaxStartupFrames = 60 * 60;

        private const int SettleFrames = 60 * 3;

        private const int RecordedFrames = 60 * 10;

        private const int CheckpointInterval = 60;

        private const int StepsPerUpdate = 100;

        private static readonly int MenuChapters = Constants.ChaptersCount;

        private const ulong FnvOffset = 14695981039346656037UL;

        private const ulong FnvPrime = 1099511628211UL;

        public static string OutputPath { get; set; } = "regression.txt";

        // Positions in the list of playable levels (see PlayableLevels), not level file numbers.
        public static int First { get; set; }

        public static int Count { get; set; } = int.MaxValue;

        public static bool Failed { get; set; }

        // Draws and reads back one frame. Set by Program to the host's capture.
        public static Func<(byte[] Pixels, int Width, int Height)> CaptureFrame { get; set; }

        // When set, every captured frame is also saved here as <label>-<settled|end>.png.
        public static string PixelsPath { get; set; }

        // When set, every recorded frame's body and node hashes (and each body's state) are written
        // here, to find the first frame where two runs diverge.
        public static readonly string TracePath = Environment.GetEnvironmentVariable("CJ_REGRESSION_TRACE");

        private enum Phase
        {
            Startup,
            Settle,
            Record,
            Done
        }

        // One level, or the main menu opened at one chapter with its level list shown.
        private sealed record Item(string Label, int Level, int MenuChapter);

        private readonly StringBuilder _checkpoints = new();

        private Phase _phase = Phase.Startup;

        private List<Item> _items;

        private int _position;

        private Item _item;

        private int _frame;

        private ulong _hash;

        private string _error;

        private StreamWriter _writer;

        private StreamWriter _trace;

        private string _settledPixels;

        // Windowed at a fixed size, so the run doesn't take over the screen and the aspect ratio
        // (which picks the 16x9 or 4x3 level set and the layout) is the same on every machine.
        protected override bool StartFullScreen => false;

        public override void Initialize(ApplicationController applicationController)
        {
            base.Initialize(applicationController);
            // The real cursor moves eyes and hover effects; keep it out of the recording.
            InputEnabled = false;
        }

        public override void Update(float time)
        {
            for (int i = 0; i < StepsPerUpdate && _phase != Phase.Done; i++)
            {
                try
                {
                    Step();
                }
                catch (Exception e) when (_phase != Phase.Startup)
                {
                    Console.Error.WriteLine(e);
                    _error = $"EXCEPTION {e.GetType().Name}: {e.Message.ReplaceLineEndings(" ")}";
                    FinishItem();
                }
            }
        }

        private void Step()
        {
            switch (_phase)
            {
                case Phase.Startup:
                    // A fresh save goes from the splash straight into the first level; wait for that
                    // so the splash can't interrupt the scenes we load ourselves.
                    base.Update(TimeStep);
                    if (Find<ContreJourDXGame>(Root) != null)
                    {
                        _writer = new StreamWriter(OutputPath);
                        if (TracePath != null)
                        {
                            _trace = new StreamWriter(TracePath);
                        }
                        if (PixelsPath != null)
                        {
                            _ = Directory.CreateDirectory(PixelsPath);
                        }
                        _writer.WriteLine($"# frames={RecordedFrames} settle={SettleFrames} step=1/60 checkpoint={CheckpointInterval} pixels=sha256/16");
                        _items = Items();
                        _position = 0;
                        StartItem();
                    }
                    else if (++_frame > MaxStartupFrames)
                    {
                        throw new TimeoutException("No level appeared after the splash.");
                    }
                    break;
                case Phase.Settle:
                    base.Update(TimeStep);
                    if (++_frame >= SettleFrames)
                    {
                        OnSettled();
                    }
                    break;
                case Phase.Record:
                    base.Update(TimeStep);
                    HashFrame();
                    if (++_frame % CheckpointInterval == 0)
                    {
                        _ = _checkpoints.Append(' ').Append((_hash & 0xFFFFFF).ToString("x6", CultureInfo.InvariantCulture));
                    }
                    if (_frame >= RecordedFrames)
                    {
                        FinishItem();
                    }
                    break;
                case Phase.Done:
                default:
                    break;
            }
        }

        // The selected range of playable levels, then every chapter's menu.
        private static List<Item> Items()
        {
            List<Item> items = [.. PlayableLevels().Skip(First).Take(Count).Select(level => new Item($"level {level:D3}", level, -1))];
            for (int chapter = 0; chapter < MenuChapters; chapter++)
            {
                items.Add(new Item($"menu {chapter}", -1, chapter));
            }
            return items;
        }

        // Every level the menus can reach, chapter by chapter, plus the ending. The level folders hold
        // more files than that, but the game can't load the unlisted ones.
        private static List<int> PlayableLevels()
        {
            List<int> levels = [];
            foreach (List<int> chapter in LevelsMenu.LevelsList)
            {
                levels.AddRange(chapter);
            }
            levels.Add(169);
            return levels;
        }

        private void StartItem()
        {
            _item = _items[_position];
            _frame = 0;
            _hash = FnvOffset;
            _error = null;
            _ = _checkpoints.Clear();
            _phase = Phase.Settle;
            if (_item.Level >= 0)
            {
                LoadLevel(_item.Level);
            }
            else
            {
                ShowMainMenu(_item.MenuChapter);
            }
        }

        private void OnSettled()
        {
            if (_item.Level >= 0)
            {
                ContreJourDXGame game = Find<ContreJourDXGame>(Root);
                if (game == null || game.LevelIndex != _item.Level)
                {
                    _error = "NOT LOADED";
                    FinishItem();
                    return;
                }
                if (game.BonusChapter)
                {
                    MangoInteractionRegression.VerifyAssets(game);
                }
                if (game.LevelIndex == 188 && Environment.GetEnvironmentVariable("CJ_REGRESSION_MANGO_INTERACTIONS") == "1")
                {
                    MangoInteractionRegression.Run(game, base.Update, label => { _ = CapturePixels(label); });
                }
                if (game.NewFriendChapter)
                {
                    NewFriendInteractionRegression.VerifyAssets(game);
                }
                if (game.LevelIndex == 300 && Environment.GetEnvironmentVariable("CJ_REGRESSION_NEW_FRIEND_INTERACTIONS") == "1")
                {
                    NewFriendInteractionRegression.Run(game, base.Update, label => { _ = CapturePixels(label); });
                }
                if (game.LevelIndex == 306 && Environment.GetEnvironmentVariable("CJ_REGRESSION_NEW_FRIEND_INTERACTIONS") == "1")
                {
                    NewFriendInteractionRegression.RunFlowers(game, base.Update);
                }
                if (Environment.GetEnvironmentVariable("CJ_REGRESSION_FINISH") == "1")
                {
                    game.Finish(game.Builder.ToPoint(game.Hero.Body.Position));
                    for (int frame = 0; frame < SettleFrames; frame++)
                    {
                        base.Update(TimeStep);
                    }
                    FinishView finishView = Find<FinishView>(game);
                    MenuPortal portal = finishView == null ? null : Find<MenuPortal>(finishView);
                    if (!game.Finished || portal == null || !portal.Visible || portal.SpeedValue != 40f)
                    {
                        throw new InvalidOperationException("The level finish screen did not show its animated menu portal.");
                    }
                    _ = CapturePixels("finished");
                    MangoInteractionRegression.VerifyResultEye(finishView, label => { _ = CapturePixels(label); });
                }
                if (game.LevelIndex == 199 && Environment.GetEnvironmentVariable("CJ_REGRESSION_MANGO_INTERACTIONS") == "1")
                {
                    NextLevel();
                    for (int frame = 0; frame < SettleFrames; frame++)
                    {
                        base.Update(TimeStep);
                    }
                    MainMenu completedMenu = Find<MainMenu>(Root);
                    if (completedMenu == null || Find<PlanetsSpinner>(completedMenu) is not PlanetsSpinner completedSpinner
                        || Math.Abs(completedSpinner.CurrentIndex - Constants.BonusChapter) > 0.01f)
                    {
                        throw new InvalidOperationException("Completing Mango did not return to its chapter menu.");
                    }
                    _ = CapturePixels("chapter-complete");
                }
                if (game.LevelIndex == 300 && Environment.GetEnvironmentVariable("CJ_REGRESSION_PAUSE_PREVIEW") == "1")
                {
                    for (int frame = 0; frame < 60; frame++)
                    {
                        base.Update(TimeStep);
                    }
                    game.OnMenuPressed();
                    for (int frame = 0; frame < SettleFrames; frame++)
                    {
                        base.Update(TimeStep);
                    }
                    _ = CapturePixels("paused");
                    game.OnMenuPressed();
                    for (int frame = 0; frame < SettleFrames; frame++)
                    {
                        base.Update(TimeStep);
                    }
                }
            }
            else
            {
                MainMenu menu = Find<MainMenu>(Root);
                if (menu == null)
                {
                    _error = "NOT LOADED";
                    FinishItem();
                    return;
                }
                menu.OnChapterSelect(_item.MenuChapter);
                for (int frame = 0; frame < SettleFrames; frame++)
                {
                    base.Update(TimeStep);
                }
                if (_item.MenuChapter == Constants.BonusChapter && Environment.GetEnvironmentVariable("CJ_REGRESSION_MANGO_LOCKS") == "1")
                {
                    MangoLockRegression.Run(Find<LevelsMenu>(menu));
                }
            }
            _phase = Phase.Record;
            _frame = 0;
            if (_item.MenuChapter == Constants.BonusChapter && Environment.GetEnvironmentVariable("CJ_REGRESSION_MANGO_INTERACTIONS") == "1")
            {
                MainMenu menu = Find<MainMenu>(Root);
                menu.HideLevels();
                for (int frame = 0; frame < SettleFrames; frame++)
                {
                    base.Update(TimeStep);
                }
                _ = CapturePixels("planet");
            }
            _settledPixels = CapturePixels("settled");
        }

        private void FinishItem()
        {
            string result = _error ?? $"{_hash:x16}{_checkpoints} px {_settledPixels} {CapturePixels("end")}";
            _writer.WriteLine($"{_item.Label} {result}");
            _writer.Flush();
            Failed |= _error != null;
            _position++;
            if (_position < _items.Count)
            {
                StartItem();
                return;
            }
            WriteSaveChecks();
            _writer.Dispose();
            _trace?.Dispose();
            _phase = Phase.Done;
            ApplicationController.Host.Quit();
        }

        // Hashes one rendered frame: the first 16 hex digits of the SHA-256 of its RGBA bytes.
        private string CapturePixels(string moment)
        {
            (byte[] pixels, int width, int height) = CaptureFrame();
            if (PixelsPath != null)
            {
                string name = $"{_item.Label.Replace(' ', '_')}-{moment}.png";
                PngWriter.Write(Path.Combine(PixelsPath, name), pixels, width, height);
            }
            return Convert.ToHexStringLower(SHA256.HashData(pixels))[..16];
        }

        // Settings and progress are saved by Preferences as two JSON files. Record some level results
        // (so level entries are written too), save through the game, and hash both files. Loading
        // them back and writing them again must give the same bytes.
        private void WriteSaveChecks()
        {
            UserData data = UserData.Instance;
            data.SetLevelData(new LevelData(4200, 3), 0);
            data.SetLevelData(new LevelData(2150, 2), 24);
            data.SetUnlockedLevelsChapter(5, 1);
            data.UnlockChapter(1);
            data.SpringShot++;
            Preferences.RequestSave();
            Preferences.Update(force: true);
            byte[] saved = ReadSaveFiles();

            Preferences.Load();
            Preferences.MarkAllDirty();
            Preferences.RequestSave();
            Preferences.Update(force: true);
            byte[] resaved = ReadSaveFiles();

            _hash = FnvOffset;
            foreach (byte b in saved)
            {
                _hash = (_hash ^ b) * FnvPrime;
            }
            string roundTrip = saved.AsSpan().SequenceEqual(resaved) ? "ok" : "CHANGED";
            _writer.WriteLine($"save {_hash:x16} round-trip {roundTrip}");
            Failed |= roundTrip != "ok";
        }

        private static byte[] ReadSaveFiles()
        {
            return [.. File.ReadAllBytes(Path.Combine(Preferences.SaveDirectory, Preferences.Settings.FileName)),
                .. File.ReadAllBytes(Path.Combine(Preferences.SaveDirectory, Preferences.GameSave.FileName))];
        }

        private static T Find<T>(Node node) where T : Node
        {
            if (node is T found)
            {
                return found;
            }
            foreach (Node child in node.Children)
            {
                T result = Find<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        private void HashFrame()
        {
            if (_item.Level < 0)
            {
                MainMenu menu = Find<MainMenu>(Root);
                if (menu == null)
                {
                    Mix(-1);
                    return;
                }
                HashNode(menu);
                if (_trace != null)
                {
                    _trace.WriteLine($"F{_frame} {_item.Label}");
                    TraceNode(menu, "menu");
                }
                return;
            }

            ContreJourDXGame game = Find<ContreJourDXGame>(Root);
            if (game == null)
            {
                Mix(-1);
                _trace?.WriteLine($"F{_frame} no game");
                return;
            }
            Mix(game.LevelIndex);
            ulong levelHash = _hash;

            _hash = FnvOffset;
            World world = game.Builder?.World;
            if (world != null)
            {
                Mix(world.BodyList.Count);
                for (int i = 0; i < world.BodyList.Count; i++)
                {
                    Body body = world.BodyList[i];
                    Mix(body.Position);
                    Mix(body.Rotation);
                    Mix(body.LinearVelocity);
                    Mix(body.AngularVelocity);
                    Mix(body.Awake ? 1 : 0);
                }
            }
            ulong bodiesHash = _hash;

            _hash = FnvOffset;
            HashNode(game.GameRoot);
            ulong nodesHash = _hash;

            _hash = levelHash;
            Mix((int)bodiesHash);
            Mix((int)(bodiesHash >> 32));
            Mix((int)nodesHash);
            Mix((int)(nodesHash >> 32));

            if (_trace != null)
            {
                _trace.WriteLine($"F{_frame} B{bodiesHash:x16} N{nodesHash:x16}");
                if (world != null)
                {
                    for (int i = 0; i < world.BodyList.Count; i++)
                    {
                        Body body = world.BodyList[i];
                        _trace.WriteLine($"  b{i} {body.UserData?.GetType().Name} p={body.Position.X:R},{body.Position.Y:R} a={body.Rotation:R} v={body.LinearVelocity.X:R},{body.LinearVelocity.Y:R} w={body.AngularVelocity:R} awake={body.Awake}");
                    }
                }
                TraceNode(game.GameRoot, "root");
            }
        }

        private void TraceNode(Node node, string path)
        {
            _trace.WriteLine($"  {path} p={node.Position.X:R},{node.Position.Y:R} r={node.RotationRadians:R} s={node.ScaleVec.X:R},{node.ScaleVec.Y:R} o={node.OpacityFloat:R} v={node.Visible} c={node.Children.Count}");
            for (int i = 0; i < node.Children.Count; i++)
            {
                TraceNode(node.Children[i], $"{path}/{i}:{node.Children[i].GetType().Name}");
            }
        }

        private void HashNode(Node node)
        {
            Mix(node.GetType().FullName);
            Mix(node.Position);
            Mix(node.RotationRadians);
            Mix(node.ScaleVec);
            Mix(node.OpacityFloat);
            Mix(node.Visible ? 1 : 0);
            Mix(node.Children.Count);
            foreach (Node child in node.Children)
            {
                HashNode(child);
            }
        }

        private void Mix(int value)
        {
            for (int i = 0; i < 4; i++)
            {
                _hash = (_hash ^ (byte)(value >> (i * 8))) * FnvPrime;
            }
        }

        private void Mix(float value)
        {
            Mix(BitConverter.SingleToInt32Bits(value));
        }

        private void Mix(Vector2 value)
        {
            Mix(value.X);
            Mix(value.Y);
        }

        private void Mix(string value)
        {
            foreach (char c in value)
            {
                Mix(c);
            }
        }
    }
}
