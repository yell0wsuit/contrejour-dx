using FarseerPhysics.Dynamics;

namespace ContreJourDX.Gameplay
{
    public class BodyTypeReq(BodyType type) : IReq
    {
        private readonly BodyType type = type;

        public bool Meet(object objectP)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            //IL_000c: Unknown result type (might be due to invalid IL or missing references)
            return ((Body)objectP).BodyType == type;
        }
    }
}
