using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class RotatableBackground : MoveBackground
    {
        protected float RotationStep { get; set; }

        public RotatableBackground(Node node, Hashtable config, ContreJourDXGame game)
            : base(node, config, game)
        {
            RotationStep = Config.GetFloat("speed") / 2f;
        }

        public override void Update(float time)
        {
            Node.RotationDegrees -= RotationStep * time * 30f;
        }
    }
}
