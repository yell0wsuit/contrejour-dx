using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Content;
using Mokus2D.Events;
using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class GameBase : Node, IUpdatable
{
    public LevelBuilderBase Builder { get; private set; }

    protected List<IRemovable> Updatables { get; set; } = [];

    public Node GameRoot { get; } = new();

    public float TotalTime { get; protected set; }

    private LevelsCache _levelsCache;

    private readonly List<object> _toRemove = [];

    public EventSender LevelLoadedEvent { get; }

    public virtual bool Paused { get; set; }

    public Dictionary<string, Level> CachedLevels => _levelsCache.CachedLevels;

    public virtual Vector2 LevelSize => Builder.LevelSize;

    public virtual Vector2 PhysicsLevelSize => Builder.PhysicsLevelSize;

    public GameBase()
    {
        AddChild(GameRoot);
        LevelLoadedEvent = new EventSender();
        TotalTime = 0f;
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
        LevelLoadedEvent.SendEvent();
    }

    public virtual void OnLoadLevelLevel(string levelName, Level level)
    {
    }

    public void AddUpdatable(IRemovable updatable)
    {
        Updatables.Add(updatable);
    }

    public virtual void ProcessLevel(Level level)
    {
        LevelBuilderBase levelBuilderBase = Builder;
        Builder = CreateLevelBuilder();
        Builder.ProcessLevel(level);
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
        if (Paused)
        {
            return;
        }
        TotalTime += time;
        UpdateGame(time);
        _toRemove.Clear();
        foreach (IRemovable updatable in Updatables)
        {
            ((IUpdatable)updatable).Update(time);
            if (updatable.ShouldRemove)
            {
                _toRemove.Add(updatable);
            }
        }
        Updatables.RemoveList(_toRemove);
        _toRemove.Clear();
    }
}
