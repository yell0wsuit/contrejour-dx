using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class RelsHint : FadeHint
{
    private readonly DragableBodyClip rels;

    private bool used;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public RelsHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        rels = (DragableBodyClip)FarseerUtil.Query(this.builder.World, this.builder.ToVec(this.clip.Position), 6.6666665f, typeof(DragableBodyClip));
        rels.DragStartEvent.AddListener(OnDragStart);
    }

    public override void Restart()
    {
        base.Restart();
        used = false;
    }

    private void OnDragStart()
    {
        if (!used)
        {
            used = true;
            hiding = true;
            Hide(0.5f * clip.OpacityByte / 255f);
        }
    }
}
