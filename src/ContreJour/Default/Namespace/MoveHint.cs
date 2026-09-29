using Mokus2D.Visual;

namespace Default.Namespace;

public class MoveHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config) : FadeHint(builder, body, clip, config)
{
    private SnotPoint point;

    private bool used;

    private void GetPoint()
    {
        foreach (BodyClip item in FarseerUtil.QueryClips(Builder.World, Builder.ToVec(Clip.Position), 6.6666665f, typeof(SnotPoint)))
        {
            if (item is SnotPoint snotPoint && snotPoint.Used)
            {
                point = snotPoint;
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
            Hiding = true;
            Hide(0.5f * Clip.OpacityByte / 255f);
        }
    }
}
