using System;

using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatableBodyClip : BodyClip
{
    protected float scaleDiff;

    protected float scaleStep;

    protected float scaleProgress;

    protected float angleDiff;

    protected bool destroying;

    protected int rotationDirection;

    protected int scaleSign;

    public RotatableBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        scaleDiff = Maths.Random(0.1f, 0.25f);
        scaleStep = Maths.Random(0.02f, 0.05f);
        scaleProgress = Maths.Random(0f, (float)Math.PI * 2f);
        angleDiff = Maths.Random(4f, 8f);
        destroying = false;
        rotationDirection = 1;
        scaleSign = 1;
        clip.RotationDegrees = Maths.Random(360);
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (!destroying)
        {
            scaleProgress += scaleStep;
            clip.ScaleX = 1f + (Maths.Cos(scaleProgress) * scaleDiff);
            clip.ScaleX = scaleSign * clip.ScaleY;
        }
    }

    public void UpdateRotation(float time)
    {
        clip.RotationDegrees += rotationDirection * angleDiff;
    }
}
