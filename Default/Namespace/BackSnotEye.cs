using ContreJour.Clips.chapter1;

using ContreJourMono.ContreJour.Game.Eyes;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class BackSnotEye : MonsterEye
{
    public static readonly EyeAnimation[] BackSnotAnimations =
    [
        new("McBackSnotEyeBlink"),
        new("McBackSnotEyeBlinkOneTime")
    ];

    protected override EyeAnimation[] Animations => BackSnotAnimations;

    protected override float ViewRadius => 30f;

    public BackSnotEye(ContreJourGame game, bool visible, Vector2 position)
        : base(game, visible, position)
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
        background = new McBackSnotEye
        {
            Test = true
        };
        eyeBall = new McBackSnotEyeBall();
    }
}
