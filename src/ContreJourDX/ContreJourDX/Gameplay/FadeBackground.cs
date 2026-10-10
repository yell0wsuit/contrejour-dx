using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class FadeBackground : BackgroundBase
    {
        private float opacity;

        private readonly Sprite sprite;

        public FadeBackground(Node node, Hashtable config, ContreJourDXGame game)
            : base(node, config, game)
        {
            sprite = (Sprite)node;
            if (!Game.CanShowIntro)
            {
                sprite.OpacityByte = 0;
                sprite.Visible = false;
            }
            else
            {
                opacity = 1f;
            }
        }

        public override void Update(float time)
        {
            base.Update(time);
            Game.LightPower = 1f - sprite.OpacityFloat;
            if (sprite.Visible)
            {
                opacity -= time * 2f / 60f;
                sprite.OpacityFloat = opacity;
                if (sprite.OpacityFloat <= 0f)
                {
                    sprite.Visible = false;
                }
            }
        }
    }
}
