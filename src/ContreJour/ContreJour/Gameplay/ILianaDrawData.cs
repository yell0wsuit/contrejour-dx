using System.Numerics;

namespace ContreJour.Gameplay
{
    public interface ILianaDrawData
    {
        Vector2 PositionAt(int index);

        int PointsCount();
    }
}
