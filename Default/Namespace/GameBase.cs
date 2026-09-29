using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Content;
using Mokus2D.Events;
using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Default.Namespace;

public class GameBase : Node, IUpdatable
{
    protected LevelBuilderBase builder;

    protected List<IRemovable> updatables = [];

    protected readonly Node gameRoot = new();

    private readonly EventSender levelLoadedEvent;

    protected bool paused;

    protected float totalTime;

    private LevelsCache _levelsCache;

    protected Vector2 levelSize = Mokus2DGame.Instance.ScreenSize;

    private readonly List<object> _toRemove = [];

    public LevelBuilderBase Builder => builder;

    public Node GameRoot => gameRoot;

    public EventSender LevelLoadedEvent => levelLoadedEvent;

    public virtual bool Paused
    {
        get => paused;
        set => paused = value;
    }

    public float TotalTime => totalTime;

    public Dictionary<string, Level> CachedLevels => _levelsCache.CachedLevels;

    public virtual Vector2 LevelSize => Builder.LevelSize;

    public virtual Vector2 PhysicsLevelSize => Builder.PhysicsLevelSize;

    public GameBase()
    {
        AddChild(gameRoot);
        levelLoadedEvent = new EventSender();
        totalTime = 0f;
    }

    public virtual void LoadLevel(string levelName)
    {
        DoLoadLevel(levelName);
    }

    public void DoLoadLevel(string levelName)
    {
        _levelsCache ??= new LevelsCache();
        Level level = _levelsCache.Load(levelName);
        ProcessLevel(level);
        OnLoadLevelLevel(levelName, level);
        levelLoadedEvent.SendEvent();
    }

    public virtual void OnLoadLevelLevel(string levelName, Level level)
    {
    }

    public void AddUpdatable(IRemovable updatable)
    {
        updatables.Add(updatable);
    }

    public virtual void ProcessLevel(Level level)
    {
        LevelBuilderBase levelBuilderBase = builder;
        builder = CreateLevelBuilder();
        builder.ProcessLevel(level);
        levelBuilderBase?.Dispose();
    }

    public virtual LevelBuilderBase CreateLevelBuilder()
    {
        return new LevelBuilderBase(this);
    }

    public virtual void UpdateGame(float time)
    {
        Builder.Update(time);
    }

    public virtual bool ShouldRemove()
    {
        return false;
    }

    public virtual void DecreaseZoomOut()
    {
    }

    public override void Update(float time)
    {
        if (paused)
        {
            return;
        }
        totalTime += time;
        UpdateGame(time);
        _toRemove.Clear();
        foreach (IRemovable updatable in updatables)
        {
            ((IUpdatable)updatable).Update(time);
            if (updatable.ShouldRemove)
            {
                _toRemove.Add(updatable);
            }
        }
        updatables.RemoveList(_toRemove);
        _toRemove.Clear();
    }
}
