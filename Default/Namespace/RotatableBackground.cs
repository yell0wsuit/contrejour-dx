using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatableBackground : MoveBackground
{
    protected float rotationStep;

    public RotatableBackground(Node node, Hashtable config, ContreJourGame game)
        : base(node, config, game)
    {
        rotationStep = this.config.GetFloat("speed") / 2f;
    }

    public override void Update(float time)
    {
        node.RotationDegrees -= rotationStep * time * 30f;
    }
}
