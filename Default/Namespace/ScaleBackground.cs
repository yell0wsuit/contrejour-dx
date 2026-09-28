using Microsoft.Xna.Framework;
using Mokus2D.Visual;

namespace Default.Namespace;

public class ScaleBackground : BackgroundBase
{
    private const float STEP = 0.02f;

    private const float SCALE_DIFF = 0.05f;

    protected Vector2 initialScale;

    protected float currentStep;

    private ScaleBackground(Node _node, Hashtable _config, ContreJourGame _game)
        : base(_node, _config, _game)
    {
        initialScale.X = node.ScaleX;
        initialScale.Y = node.ScaleY;
        currentStep = (config.Exists("angleOffset") ? (config.GetFloat("angleOffset") * 10f) : 0f);
    }

    public override void Update(float time)
    {
        base.Update(time);
        currentStep += 0.02f;
        node.ScaleX = initialScale.X + Maths.Cos(currentStep) * 0.05f + 0.05f;
        node.ScaleY = initialScale.Y + Maths.Cos(currentStep) * 0.05f + 0.05f;
    }
}
