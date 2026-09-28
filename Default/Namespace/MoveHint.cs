using Mokus2D.Visual;

namespace Default.Namespace;

public class MoveHint : FadeHint
{
    private const float QUERY_RADIUS = 6.6666665f;

    private SnotPoint point;

    private bool used;

    public MoveHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
    }

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
            Hide(0.5f * (float)clip.OpacityByte / 255f);
        }
    }
}
