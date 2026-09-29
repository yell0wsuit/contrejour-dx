using System.Collections.Generic;
using System.Numerics;

using ContreJour.Primitives;

using FarseerPhysics.Dynamics;

using Mokus2D.Util.Data;

namespace ContreJour.Gameplay
{
    public class SnotSprite : LongNeckSprite
    {
        public const int CircleSegments = 12;

        protected SnotBodyClipBase Snot { get; set; }

        private readonly SnotData data;

        private readonly float startWidth;

        private readonly float endWidth;

        private readonly float centerWidth;

        public SnotSprite(SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth)
        {
            Snot = snot;
            data = Snot.Physics;
            this.startWidth = startWidth;
            this.endWidth = endWidth;
            this.centerWidth = centerWidth;
        }

        public override void GetPairs(List<Pair<Vector2>> target)
        {
            Vector2 startPosition = Snot.StartPosition;
            target.Add(ContreDrawUtil.Ccp2Pair(ContreDrawUtil.GetPointsPairStartEndWidthResult(startPosition, startPosition, data.BodyAt(0).Position, startWidth)));
            Vector2 start = startPosition;
            Body val = null;
            for (int i = 0; i < data.BodiesSize() - 1; i++)
            {
                val = data.BodyAt(i);
                Body val2 = data.BodyAt(i + 1);
                target.Add(ContreDrawUtil.Ccp2Pair(ContreDrawUtil.GetPointsPairStartEndWidthResult(val.Position, start, val2.Position, centerWidth)));
                start = val2.Position;
                if (i < data.BodiesSize() - 2)
                {
                    target.Add(ContreDrawUtil.Ccp2Pair(ContreDrawUtil.GetPointsPairStartEndWidthResult((val.Position + val2.Position) * 0.5f, val.Position, val2.Position, centerWidth)));
                }
            }
            target.Add(ContreDrawUtil.Ccp2Pair(ContreDrawUtil.GetPointsPairStartEndWidthResult(Snot.EndPosition(), val.Position, Snot.EndPosition(), endWidth)));
        }
    }
}
