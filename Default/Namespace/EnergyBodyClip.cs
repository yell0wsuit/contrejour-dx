using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Mokus2D.Events;
using Mokus2D.Sound;
using Mokus2D.Visual;

namespace Default.Namespace;

public class EnergyBodyClip : BodyClip, IRestartable
{
    private const int ENERGY_PARTS_COUNT = 5;

    protected ContreJourLevelBuilder contreJourBuilder;

    protected List<object> energyParts;

    protected bool collected;

    protected EventSender collectEvent;

    public EventSender CollectEvent => collectEvent;

    public EnergyBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        _clip.Visible = false;
        contreJourBuilder = (ContreJourLevelBuilder)builder;
        energyParts = new List<object>();
        collectEvent = new EventSender();
        CreateParts();
    }

    public void CreateParts()
    {
        for (int i = 0; i < 5; i++)
        {
            EnergyPart item = new EnergyPart((ContreJourGame)builder.Game, this, (float)Math.PI * 2f / 5f * (float)i, clip.Position);
            energyParts.Add(item);
        }
    }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
        if (!(body2.UserData is HeroBodyClip heroBodyClip) || collected || !heroBodyClip.CanDie())
        {
            return;
        }
        collected = true;
        collectEvent.SendEvent();
        SoundManager.PlayRandomSound(Sounds.BONUS, 0.5f);
        contreJourBuilder.ContreJour.CollectStar();
        foreach (EnergyPart energyPart in energyParts)
        {
            energyPart.Collect();
        }
    }

    public void Restart()
    {
        if (collected)
        {
            collected = false;
            energyParts.Clear();
            CreateParts();
        }
    }
}
