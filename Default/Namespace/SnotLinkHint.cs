using System.Collections.Generic;
using System.Linq;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SnotLinkHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config) : FadeHint(builder, body, clip, config)
{
    protected SnotBodyClip snot;

    private bool snotGot;

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
