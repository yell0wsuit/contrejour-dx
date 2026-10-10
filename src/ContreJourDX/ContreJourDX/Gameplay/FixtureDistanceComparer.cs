using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Dynamics;

namespace ContreJourDX.Gameplay
{
    internal readonly struct FixtureDistanceComparer(Vector2 center) : IComparer<Fixture>
    {
        private readonly Vector2 center = center;

        public readonly int Compare(Fixture x, Fixture y)
        {
            float num = Vector2.Distance(x.Body.Position, center);
            float num2 = Vector2.Distance(y.Body.Position, center);
            return num < num2 ? -1 : num == num2 ? 0 : 2;
        }
    }
}
