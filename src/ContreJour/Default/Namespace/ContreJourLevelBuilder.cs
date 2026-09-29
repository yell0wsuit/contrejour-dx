using System;

using Mokus2D.Visual;

namespace Default.Namespace;

public class ContreJourLevelBuilder(GameBase game) : LevelBuilderBase(game)
{
    public ContreJourGame ContreJour => (ContreJourGame)Game;

    public override void AddProcessors()
    {
        base.AddProcessors();
        Processors.Add(new PlasticineProcessor(this));
        Processors.Add(new SnotProcessor(this));
        Processors.Add(new BackSnotProcessor(this));
        Processors.Add(new StrongSnotProcessor(this));
        Processors.Add(new BridgeSnotProcessor(this));
        Processors.Add(new VariableSnotProcessor(this));
        Processors.Add(new TrampolineSnotProcessor(this));
        Processors.Add(new LightPointProcessor(this));
        Processors.Add(new ForegroundProcessor(this));
        Processors.Add(new LianaProcessor(this));
        Processors.Add(new FakeProcessor("hint", this));
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
        return ContreJour.BlackSide && config.Exists("blackViewType") ? config.GetString("blackViewType") : base.GetViewType(config);
    }

    public override void Update(float time)
    {
        float num = Math.Min(time, MaxWorldUpdateTime) * PhysicsSpeed / 2f;
        World.Step(num);
        World.Step(num);
        Updater.Update(time);
    }
}
