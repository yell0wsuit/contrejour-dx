using ContreJour.Clips.chapter1;
using ContreJourMono.ContreJour.Game.Eyes;
using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class BackSnotEye : MonsterEye
{
    public static readonly EyeAnimation[] BACK_SNOT_ANIMATIONS = new EyeAnimation[2]
    {
        new EyeAnimation("McBackSnotEyeBlink"),
        new EyeAnimation("McBackSnotEyeBlinkOneTime")
    };

    protected override EyeAnimation[] Animations => BACK_SNOT_ANIMATIONS;

    protected override float ViewRadius => 30f;

    public BackSnotEye(ContreJourGame _game, bool _visible, Vector2 position)
        : base(_game, _visible, position)
    {
        eyeStep = 1.5f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        currentBackground.Position = currentEyeBall.Position * 0.4f;
    }

    protected override void CreateDefaultView()
    {
        background = new McBackSnotEye();
        background.Test = true;
        eyeBall = new McBackSnotEyeBall();
    }
}
