using System;

using Mokus2D.Visual;

namespace Default.Namespace;

public class ContreJourLevelBuilder : LevelBuilderBase
{
    public ContreJourGame ContreJour => (ContreJourGame)game;

    public ContreJourLevelBuilder(GameBase _game)
        : base(_game)
    {
    }

    public override void AddProcessors()
    {
        base.AddProcessors();
        processors.Add(new PlasticineProcessor(this));
        processors.Add(new SnotProcessor(this));
        processors.Add(new BackSnotProcessor(this));
        processors.Add(new StrongSnotProcessor(this));
        processors.Add(new BridgeSnotProcessor(this));
        processors.Add(new VariableSnotProcessor(this));
        processors.Add(new TrampolineSnotProcessor(this));
        processors.Add(new LightPointProcessor(this));
        processors.Add(new ForegroundProcessor(this));
        processors.Add(new LianaProcessor(this));
        processors.Add(new FakeProcessor("hint", this));
    }

    public void AddAlphaBackground(Node child)
    {
        ContreJour.AlphaBackground.AddChild(child);
    }

    public void AddAlphaBackgroundZ(Node child, int z)
    {
        ContreJour.AlphaBackground.AddChild(child, z);
    }

    public override LevelBuilderBase GetBuilder()
    {
        return this;
    }

    public override string GetViewType(Hashtable config)
    {
        if (ContreJour.BlackSide && config.Exists("blackViewType"))
        {
            return config.GetString("blackViewType");
        }
        return base.GetViewType(config);
    }

    public override void Update(float time)
    {
        float num = Math.Min(time, maxWorldUpdateTime) * physicsSpeed / 2f;
        world.Step(num);
        world.Step(num);
        updater.Update(time);
    }
}
