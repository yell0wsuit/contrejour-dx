using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class SunBackground : MoveBackground
    {
        private float currentOpacity;

        public SunBackground(Node node, Hashtable config, ContreJourDXGame game)
            : base(node, config, game)
        {
            if (Game.CanShowIntro)
            {
                currentOpacity = 255f;
                return;
            }
            currentOpacity = 0f;
            node.Tweener.Stop();
            Game.FlyOpacity = 0f;
        }

        public override void Update(float time)
        {
            if (currentOpacity >= 0f)
            {
                Game.FlyOpacity = currentOpacity;
                currentOpacity -= 0.4f;
            }
            else
            {
                Game.FlyOpacity = 0f;
            }
        }
    }
}
