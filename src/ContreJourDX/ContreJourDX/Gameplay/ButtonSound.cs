using Mokus2D.Visual.Interactive;

namespace ContreJourDX.Gameplay
{
    public class ButtonSound
    {
        public ButtonSound(TouchSprite sprite)
        {
            sprite.TouchEndEvent += OnClick;
        }

        private void OnClick(TouchArguments touchArguments)
        {
        }
    }
}
