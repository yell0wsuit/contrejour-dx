using System.Numerics;

namespace ContreJourDX.Gameplay
{
    public interface ILianaDrawData
    {
        Vector2 PositionAt(int index);

        int PointsCount();
    }
}
