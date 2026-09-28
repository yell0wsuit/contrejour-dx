using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

    private const ulong FnvOffset = 14695981039346656037UL;

    private const ulong FnvPrime = 1099511628211UL;

    public static string OutputPath = "regression.txt";

    // Positions in the list of playable levels (see PlayableLevels), not level file numbers.
    public static int First;

    public static int Count = int.MaxValue;

    public static bool Failed;

    // When set, every recorded frame's body and node hashes (and each body's state) are written
    // here, to find the first frame where two runs diverge.
    public static string TracePath = Environment.GetEnvironmentVariable("CJ_REGRESSION_TRACE");

    private StreamWriter _trace;

    private enum Phase
    {
        Startup,
        Settle,
        Record,
        Done
    }

    private readonly StringBuilder _checkpoints = new StringBuilder();

    private Phase _phase = Phase.Startup;

    private List<int> _levels;

    private int _position;

    private int _level;

    private int _frame;

    private ulong _hash;

    private string _error;

    private StreamWriter _writer;

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
                FinishLevel();
            }
        }
    }

    private void Step()
    {
        switch (_phase)
        {
            case Phase.Startup:
                // A fresh save goes from the splash straight into the first level; wait for that
                // so the splash can't interrupt the levels we load ourselves.
                base.Update(TimeStep);
                if (FindGame() != null)
                {
                    _writer = new StreamWriter(OutputPath);
                    if (TracePath != null)
                    {
                        _trace = new StreamWriter(TracePath);
                    }
                    _writer.WriteLine($"# frames={RecordedFrames} settle={SettleFrames} step=1/60 checkpoint={CheckpointInterval}");
                    _levels = PlayableLevels();
                    _position = Math.Min(First, _levels.Count - 1);
                    StartLevel();
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
                    ContreJourGame game = FindGame();
                    if (game == null || game.LevelIndex != _level)
                    {
                        _error = "NOT LOADED";
                        FinishLevel();
                        break;
                    }
                    _phase = Phase.Record;
                    _frame = 0;
                }
                break;
            case Phase.Record:
                base.Update(TimeStep);
                HashFrame();
                if (++_frame % CheckpointInterval == 0)
                {
                    _checkpoints.Append(' ').Append((_hash & 0xFFFFFF).ToString("x6"));
                }
                if (_frame >= RecordedFrames)
                {
                    FinishLevel();
                }
                break;
        }
    }

    // Every level the menus can reach, chapter by chapter, plus the ending. The level folders hold
    // more files than that, but the game can't load the unlisted ones.
    private static List<int> PlayableLevels()
    {
        List<int> levels = new List<int>();
        foreach (List<int> chapter in LevelsMenu.LevelsList)
        {
            levels.AddRange(chapter);
        }
        levels.Add(169);
        return levels;
    }

    private void StartLevel()
    {
        int level = _levels[_position];
        _level = level;
        _frame = 0;
        _hash = FnvOffset;
        _error = null;
        _checkpoints.Clear();
        _phase = Phase.Settle;
        LoadLevel(level);
    }

    private void FinishLevel()
    {
        string result = _error ?? $"{_hash:x16}{_checkpoints}";
        _writer.WriteLine($"level {_level:D3} {result}");
        _writer.Flush();
        Failed |= _error != null;
        _position++;
        if (_position < _levels.Count && _position < First + Count)
        {
            StartLevel();
            return;
        }
        _writer.Dispose();
        _trace?.Dispose();
        _phase = Phase.Done;
        ApplicationController.Application.Exit();
    }

    private ContreJourGame FindGame()
    {
        return FindGame(Root);
    }

    private static ContreJourGame FindGame(Node node)
    {
        if (node is ContreJourGame game)
        {
            return game;
        }
        foreach (Node child in node.Children)
        {
            ContreJourGame found = FindGame(child);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    private void HashFrame()
    {
        ContreJourGame game = FindGame();
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
