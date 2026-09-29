using System;

using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatableBodyClip : BodyClip
{
    private readonly float scaleDiff;

    private readonly float scaleStep;

    private float scaleProgress;

    private readonly bool destroying;

    private readonly int scaleSign;

    public RotatableBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        scaleDiff = Maths.Random(0.1f, 0.25f);
        scaleStep = Maths.Random(0.02f, 0.05f);
        scaleProgress = Maths.Random(0f, (float)Math.PI * 2f);
        // The value is unused, but the draw keeps the shared random sequence unchanged.
        _ = Maths.Random(4f, 8f);
        destroying = false;
        scaleSign = 1;
        this.Clip.RotationDegrees = Maths.Random(360);
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (!destroying)
        {
            scaleProgress += scaleStep;
            Clip.ScaleX = 1f + (Maths.Cos(scaleProgress) * scaleDiff);
            Clip.ScaleX = scaleSign * Clip.ScaleY;
        }
    }

}
