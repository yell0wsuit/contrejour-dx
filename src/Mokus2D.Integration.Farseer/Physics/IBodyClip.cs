using FarseerPhysics.Dynamics;

using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Physics
{
    public interface IBodyClip
    {
        Node Clip { get; }

        Body Body { get; }

        void UpdatePosition();
    }
}
