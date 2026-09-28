using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using Default.Namespace;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Game;
using Mokus2D.Visual;

namespace ContreJour.Regression;

public class RegressionApplication : ContreJourApplication
{
    private const float TimeStep = 1f / 60f;

    private const int MaxStartupFrames = 60 * 60;

    private const int SettleFrames = 60 * 3;

    private const int RecordedFrames = 60 * 10;

    private const int CheckpointInterval = 60;

    private const int StepsPerUpdate = 100;

    private const int MenuChapters = 6;

    private const ulong FnvOffset = 14695981039346656037UL;

    private const ulong FnvPrime = 1099511628211UL;

    public static string OutputPath { get; set; } = "regression.txt";

    // Positions in the list of playable levels (see PlayableLevels), not level file numbers.
    public static int First { get; set; }

    public static int Count { get; set; } = int.MaxValue;

    public static bool Failed { get; set; }

    // When set, every recorded frame's body and node hashes (and each body's state) are written
    // here, to find the first frame where two runs diverge.
    public static readonly string TracePath = Environment.GetEnvironmentVariable("CJ_REGRESSION_TRACE");

    // A save file written by an earlier build; loading it checks that old saves still read the same.
    public static readonly string OldSavePath = Environment.GetEnvironmentVariable("CJ_REGRESSION_OLD_SAVE");

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

    // Windowed at a fixed size, so the run doesn't take over the screen and the aspect ratio
    // (which picks the 16x9 or 4x3 level set and the layout) is the same on every machine.
    protected override bool StartFullScreen => false;

    public override void Initialize(ApplicationController applicationController)
    {
        applicationController.PrefferedBackBufferSize = new Mokus2D.Util.Data.Point(1280, 720);
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
                if (Find<ContreJourGame>(Root) != null)
                {
                    _writer = new StreamWriter(OutputPath);
                    if (TracePath != null)
                    {
                        _trace = new StreamWriter(TracePath);
                    }
                    _writer.WriteLine($"# frames={RecordedFrames} settle={SettleFrames} step=1/60 checkpoint={CheckpointInterval}");
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
            ContreJourGame game = Find<ContreJourGame>(Root);
            if (game == null || game.LevelIndex != _item.Level)
            {
                _error = "NOT LOADED";
                FinishItem();
                return;
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
            menu.ShowLevels();
        }
        _phase = Phase.Record;
        _frame = 0;
    }

    private void FinishItem()
    {
        string result = _error ?? $"{_hash:x16}{_checkpoints}";
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
        ApplicationController.Application.Exit();
    }

    // The save file is UserData serialized with XmlSerializer, so its public members are a file
    // format. Hash a canonical form (member order ignored) of this run's save, and of an older
    // save loaded and written back, to catch changes to what gets saved or read.
    private void WriteSaveChecks()
    {
        XmlSerializer serializer = new(typeof(UserData));
        _writer.WriteLine($"save fresh {CanonicalHash(serializer, UserData.Instance)}");
        if (OldSavePath != null)
        {
            using XmlReader reader = XmlReader.Create(OldSavePath);
            UserData old = (UserData)serializer.Deserialize(reader);
            _writer.WriteLine($"save old {CanonicalHash(serializer, old)}");
        }
    }

    private string CanonicalHash(XmlSerializer serializer, UserData data)
    {
        using MemoryStream stream = new();
        serializer.Serialize(stream, data);
        stream.Position = 0;
        _hash = FnvOffset;
        Mix(Canonical(XElement.Load(stream)));
        return _hash.ToString("x16", CultureInfo.InvariantCulture);
    }

    // Members may be written in any order, but repeated elements (array items) keep theirs.
    private static string Canonical(XElement element)
    {
        StringBuilder builder = new();
        _ = builder.Append('<').Append(element.Name.LocalName);
        foreach (XAttribute attribute in element.Attributes().Where(a => !a.IsNamespaceDeclaration).OrderBy(a => a.Name.LocalName, StringComparer.Ordinal))
        {
            _ = builder.Append(' ').Append(attribute.Name.LocalName).Append('=').Append(attribute.Value);
        }
        _ = builder.Append('>');
        if (element.HasElements)
        {
            foreach (XElement child in element.Elements().OrderBy(e => e.Name.LocalName, StringComparer.Ordinal))
            {
                _ = builder.Append(Canonical(child));
            }
        }
        else
        {
            _ = builder.Append(element.Value);
        }
        return builder.Append("</>").ToString();
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

        ContreJourGame game = Find<ContreJourGame>(Root);
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
