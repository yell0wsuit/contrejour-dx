using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Mokus2D.Visual;

namespace Default.Namespace;

public class MultitouchHint : FadeHint
{
    private int joinCount;

    private readonly List<BodyClip> snots;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public MultitouchHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        snots = FarseerUtil.QueryBodyClipsCenterRadiusType(Builder.World, Builder.ToIPhoneVec(Clip.Position), 6.6666665f, typeof(StrongSnotBodyClip));
        HasToRun = false;
        foreach (StrongSnotBodyClip snot in snots.Cast<StrongSnotBodyClip>())
        {
            snot.LinkEvent.AddListener(OnSnotLink);
            snot.ReleaseEvent.AddListener(OnSnotRelease);
        }
    }

    public override void Restart()
    {
        base.Restart();
        HasToRun = false;
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
        if (Clip.Visible && !Hiding)
        {
            Hide(0.5f * Clip.OpacityByte / 255f);
        }
    }
}
