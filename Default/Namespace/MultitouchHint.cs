using System.Collections.Generic;
using System.Linq;

using Mokus2D.Visual;

namespace Default.Namespace;

public class MultitouchHint : FadeHint
{
    private const float QUERY_RADIUS = 6.6666665f;

    protected int joinCount;

    protected List<BodyClip> snots;

    public MultitouchHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        snots = FarseerUtil.QueryBodyClipsCenterRadiusType(builder.World, builder.ToIPhoneVec(clip.Position), 6.6666665f, typeof(StrongSnotBodyClip));
        hasToRun = false;
        foreach (StrongSnotBodyClip snot in snots.Cast<StrongSnotBodyClip>())
        {
            snot.LinkEvent.AddListener(OnSnotLink);
            snot.ReleaseEvent.AddListener(OnSnotRelease);
        }
    }

    public override void Restart()
    {
        base.Restart();
        hasToRun = false;
    }

    public override bool HasToHide()
    {
        return false;
    }

    private void OnSnotLink()
    {
        joinCount++;
        if (joinCount == 2)
        {
            Show();
        }
    }

    private void OnSnotRelease()
    {
        joinCount--;
        if (clip.Visible && !hiding)
        {
            Hide(0.5f * clip.OpacityByte / 255f);
        }
    }
}
