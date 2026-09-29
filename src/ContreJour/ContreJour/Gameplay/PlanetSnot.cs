using System;
using System.Collections.Generic;

using ContreJour.Clips.common;
using ContreJour.Primitives;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace ContreJour.Gameplay
{
    public class PlanetSnot : LongNeckSprite, IDepthDependent
    {
        private readonly PlanetSnotEye _eye;

        public Sprite BaseSprite { get; }

        private Vector2 middle;

        private Vector2 end;

        private Vector2 targetEnd;
        private Vector2 endInit;

        private Vector2 middleInit;

        private static readonly Vector2 END = new(0f, 80f);

        private static readonly Vector2 MIDDLE = new(0f, 30f);

        public float Depth { get; set; }

        public PlanetSnot(PlanetSnotEye eye)
        {
            endInit = END;
            middleInit = MIDDLE;
            BaseSprite = new McFlowerHead();
            _eye = eye;
            _eye.Position = end;
            middle = middleInit;
            end = Vector2.Zero;
            BorderWidth = 4f;
        }

        public override void GetPairs(List<Pair<Vector2>> target)
        {
            target.Add(ContreDrawUtil.GetPointsPair(Vector2.Zero, Vector2.Zero, middle, 10f));
            target.Add(ContreDrawUtil.GetPointsPair(middle, Vector2.Zero, middle, 5f));
            target.Add(ContreDrawUtil.GetPointsPair(end, middle, end, 20f));
        }

        public override void Update(float time)
        {
            if (Maths.FuzzyEquals(Depth, 1f))
            {
                Vector2 vector = VectorUtil.ToVector(_eye.ViewDistance * 40f, _eye.ViewAngle);
                targetEnd = endInit + vector;
                float num = Vector2.Distance(end, targetEnd);
                middle = VectorUtil.StepTo(middle, middleInit, 1f);
                end = VectorUtil.StepTo(end, targetEnd, Math.Min(1f, num / 5f));
            }
            else if (Depth > 0.6f)
            {
                targetEnd = new Vector2(-10f, -10f);
                middle = VectorUtil.StepTo(middle, new Vector2(-5f, -5f), 10f);
                end = VectorUtil.StepTo(end, targetEnd, 10f);
            }
            else
            {
                middle = new Vector2(-5f, -5f);
                targetEnd = new Vector2(-10f, -10f);
                end = targetEnd;
            }
            _eye.Position = end;
            BaseSprite.Position = end;
            base.Update(time);
        }

        public override void Draw(VisualState state)
        {
            base.Draw(state);
        }
    }
}
