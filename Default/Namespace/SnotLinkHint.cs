using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Mokus2D.Visual;

namespace Default.Namespace;

[method: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
public class SnotLinkHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config) : FadeHint(_builder, null, _clip, _config)
{
    protected SnotBodyClip snot;

    protected bool snotGot;

    public override bool HasToHide()
    {
        return false;
    }

    public override void Restart()
    {
        base.Restart();
        snotGot = false;
    }

    private void GetSnot()
    {
        List<BodyClip> list = FarseerUtil.QueryBodyClipsCenterRadiusType(builder.World, builder.ToIPhoneVec(clip.Position), 6.6666665f, typeof(SnotBodyClip));
        foreach (SnotBodyClip item in list.Cast<SnotBodyClip>())
        {
            item.LinkEvent.AddListener(OnSnotLink);
        }
        snot = (SnotBodyClip)list[0];
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (Maths.FuzzyNotEquals(time, 0f) && !snotGot)
        {
            GetSnot();
            snotGot = true;
        }
    }

    public virtual void CheckHeroDistance()
    {
    }

    public virtual void OnSnotLink()
    {
        hiding = true;
        Hide(0.5f * clip.OpacityByte / 255f);
    }
}
