using Mokus2D.Visual;

namespace Default.Namespace;

public class MoveHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config) : FadeHint(builder, body, clip, config)
{
    private SnotPoint point;

    private bool used;

    private void GetPoint()
    {
        foreach (BodyClip item in FarseerUtil.QueryClips(builder.World, builder.ToVec(clip.Position), 6.6666665f, typeof(SnotPoint)))
        {
            if (item is SnotPoint && ((SnotPoint)item).Used)
            {
                point = (SnotPoint)item;
                point.UnuseEvent.AddListener(OnUnuse);
                break;
            }
        }
    }

    public override bool HasToHide()
    {
        return false;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (point == null)
        {
            GetPoint();
        }
    }

    public override void Restart()
    {
        base.Restart();
        used = false;
    }

    private void OnUnuse()
    {
        if (!used)
        {
            used = true;
            hiding = true;
            Hide(0.5f * clip.OpacityByte / 255f);
        }
    }
}
