using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class LeafBodyClip : ForegroundBase
{
    protected CosChanger rotationChanger;

    protected float initialRotation;

    public LeafBodyClip(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        float num = config.GetFloat("angleOffset", 0f);
        rotationChanger = new CosChanger(0f - num, num, config.GetFloat("rotationSpeed") / 30f);
        initialRotation = this.clip.RotationDegrees;
    }

    public override void Update(float time)
    {
        base.Update(time);
        rotationChanger.Update(time);
        clip.RotationDegrees = initialRotation + rotationChanger.Value;
    }
}
