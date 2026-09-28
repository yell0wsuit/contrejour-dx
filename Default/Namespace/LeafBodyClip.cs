using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class LeafBodyClip : ForegroundBase
{
    protected CosChanger rotationChanger;

    protected float initialRotation;

    public LeafBodyClip(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        float num = _config.GetFloat("angleOffset", 0f);
        rotationChanger = new CosChanger(0f - num, num, _config.GetFloat("rotationSpeed") / 30f);
        initialRotation = clip.RotationDegrees;
    }

    public override void Update(float time)
    {
        base.Update(time);
        rotationChanger.Update(time);
        clip.RotationDegrees = initialRotation + rotationChanger.Value;
    }
}
