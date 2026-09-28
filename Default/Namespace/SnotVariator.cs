using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class SnotVariator : ContreJourBodyClip, IRemovable
{
    protected SnotBodyClip snot;

    protected CosChanger changer;

    public bool ShouldRemove => false;

    public SnotVariator(SnotBodyClip _snot)
        : base(_snot.Builder, null, null, null)
    {
        snot = _snot;
        Game.AddUpdatable(this);
        changer = new CosChanger(0f, 150f * builder.SizeMult, 0.1f);
    }

    public override void Update(float time)
    {
        changer.Update(time);
        snot.SetLengthDiff(0f - changer.Value);
    }
}
