using Mokus2D.Visual.Interactive;

namespace ContreJour.Gameplay;

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
