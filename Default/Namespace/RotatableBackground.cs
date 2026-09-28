using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatableBackground : MoveBackground
{
    protected float rotationStep;

    public RotatableBackground(Node _node, Hashtable _config, ContreJourGame _game)
        : base(_node, _config, _game)
    {
        rotationStep = config.GetFloat("speed") / 2f;
    }

    public override void Update(float time)
    {
        node.RotationDegrees -= rotationStep * time * 30f;
    }
}
