using System;
using System.Collections.Generic;
using System.Linq;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Mokus2D.Events;
using Mokus2D.Sound;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class EnergyBodyClip : BodyClip, IRestartable
    {
        private readonly ContreJourLevelBuilder contreJourBuilder;

        private readonly List<object> energyParts;

        private bool collected;

        public EventSender CollectEvent { get; }

        public EnergyBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            clip.Visible = false;
            contreJourBuilder = (ContreJourLevelBuilder)Builder;
            energyParts = [];
            CollectEvent = new EventSender();
            CreateParts();
        }

        public void CreateParts()
        {
            for (int i = 0; i < 5; i++)
            {
                EnergyPart item = new((ContreJourGame)Builder.Game, this, (float)Math.PI * 2f / 5f * i, Clip.Position);
                energyParts.Add(item);
            }
        }

        public override void OnCollisionStartPoint(Body body2, Contact point)
        {
            if (body2.UserData is not HeroBodyClip heroBodyClip || collected || !heroBodyClip.CanDie())
            {
                return;
            }
            collected = true;
            CollectEvent.SendEvent();
            SoundManager.PlayRandomSound(Sounds.BONUS, 0.5f);
            contreJourBuilder.ContreJour.CollectStar();
            foreach (EnergyPart energyPart in energyParts.Cast<EnergyPart>())
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
}
