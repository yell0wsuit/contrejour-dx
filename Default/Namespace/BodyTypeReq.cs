using FarseerPhysics.Dynamics;

namespace Default.Namespace;

public class BodyTypeReq : IReq
{
    protected BodyType type;

    public BodyTypeReq(BodyType type)
    {
        //IL_0007: Unknown result type (might be due to invalid IL or missing references)
        //IL_0008: Unknown result type (might be due to invalid IL or missing references)
        this.type = type;
    }

    public bool Meet(object objectP)
    {
        //IL_0001: Unknown result type (might be due to invalid IL or missing references)
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_000c: Unknown result type (might be due to invalid IL or missing references)
        return ((Body)objectP).BodyType == type;
    }
}
