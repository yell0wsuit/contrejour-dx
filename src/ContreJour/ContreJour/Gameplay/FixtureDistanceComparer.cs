using System.Collections.Generic;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;

namespace ContreJour.Gameplay
{
    internal readonly struct FixtureDistanceComparer(Vector2 center) : IComparer<Fixture>
    {
        private readonly Vector2 center = center;

        public readonly int Compare(Fixture x, Fixture y)
        {
            float num = x.Body.Position.DistanceTo(center);
            float num2 = y.Body.Position.DistanceTo(center);
            return num < num2 ? -1 : num == num2 ? 0 : 2;
        }
    }
}
