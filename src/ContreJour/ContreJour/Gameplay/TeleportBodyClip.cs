using System;
using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Mokus2D.Events;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class TeleportBodyClip : BodyClip
    {
        private readonly Portal portal;
        private readonly bool limitSpeed;

        private readonly List<BodyClip> teleportables;

        public TeleportBodyClip Sibling { get; set; }

        public EventSender UseEvent { get; }

        public TeleportBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, null, config)
        {
            UseEvent = new EventSender();
            ContreJourGame contreJourGame = (ContreJourGame)builder.Game;
            limitSpeed = Config.GetBool("limitSpeed");
            string text = Config.GetString("color") ?? "0";
            portal = new Portal(textureName: (!(text != "0")) ? (contreJourGame.BlackSide ? "common/McTeleportPartBlue" : "common/McTeleportPart") : "common/McTeleportPartBlue", game: contreJourGame, position: clip.Position);
            if (contreJourGame.BonusChapter)
            {
                portal.Color = ContreJourConstants.GreenLightColor;
            }
            builder.RemoveChild(clip);
            builder.Add(portal, -2);
            portal.TargetScale = 1f;
            portal.SpeedValue = Maths.Random(20f, 35f);
            portal.ScaleStep = 0.2f;
            builder.Add(new TeleportPortal(portal), -2);
            Sibling = contreJourGame.GetTeleport(text);
            if (Sibling != null)
            {
                Sibling.Sibling = this;
            }
            else
            {
                contreJourGame.RegisterTeleportColor(this, text);
            }
            teleportables = [];
        }

        public void Use()
        {
            UseEvent.SendEvent();
            portal.TargetScale = 0.2f;
            Schedule(RestoreScale, 0.1f);
            Schedule(SetMaxScale, 1f / 30f);
        }

        private void SetMaxScale()
        {
            portal.TargetScale = 1.2f;
        }

        public override void OnCollisionStartPoint(Body body2, Contact point)
        {
            if (point.FixtureA.IsSensor && point.FixtureB.IsSensor)
            {
                return;
            }
            BodyClip bodyClip = (BodyClip)body2.UserData;
            if (!teleportables.Contains(bodyClip) && bodyClip is ITeleportable teleportable && teleportable.CanTeleport())
            {
                teleportables.Add(bodyClip);
                teleportable.Teleport(this);
                teleportable.SnotEnabled = false;
                float num = body2.LinearVelocity.Length() / Builder.EngineConfig.SizeMultiplier;
                float num2 = Math.Max((num > 200f) ? (20f / num) : 0.1f, 0.01f);
                teleportable.SetScaleTime(0f, num2);
                portal.TargetScale = 0.2f;
                Schedule(delegate
                {
                    MoveHero(bodyClip);
                }, num2);
                UseEvent.SendEvent();
            }
        }

        public override void OnCollisionEndPoint(Body body2, Contact point)
        {
            BodyClip bodyClip = (BodyClip)body2.UserData;
            if (teleportables.Contains(bodyClip))
            {
                _ = teleportables.Remove(bodyClip);
            }
        }

        private void TeleportFromSibling(BodyClip teleportable)
        {
            teleportables.Add(teleportable);
        }

        private void MoveHero(BodyClip bodyClip)
        {
            ITeleportable teleportable = bodyClip as ITeleportable;
            Sibling.TeleportFromSibling(bodyClip);
            bodyClip.Clip.Scale = 0f;
            bodyClip.Clip.Tweener.Stop();
            bodyClip.Body.SetTransform(Sibling.Body.Position, bodyClip.Body.Rotation);
            teleportable.AfterTeleport();
            teleportable.SnotEnabled = true;
            portal.TargetScale = 1.2f;
            Schedule(RestoreScale, 0.1f);
            teleportable.ForceClipPosition();
            ScaleHero(bodyClip);
            Sibling.Use();
            if (limitSpeed)
            {
                Vector2 vec = bodyClip.Body.LinearVelocity;
                _ = VectorUtil.ClampLength(ref vec, 23.333334f);
                bodyClip.Body.LinearVelocity = vec;
            }
        }

        public static void ScaleHero(BodyClip bodyClip)
        {
            (bodyClip as ITeleportable).SetScaleTime(1f, Math.Min(0.1f, 0.1f / bodyClip.Body.LinearVelocity.Length() * 10f));
        }

        private void RestoreScale()
        {
            portal.TargetScale = 1f;
        }
    }
}
