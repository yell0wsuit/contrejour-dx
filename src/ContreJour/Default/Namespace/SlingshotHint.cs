using System.Diagnostics.CodeAnalysis;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SlingshotHint : FadeHint
{
    private TrampolineBodyClip trampoline;

    private bool touched;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public SlingshotHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        HasToRun = false;
        Initialize();
    }

    private void Initialize()
    {
        trampoline = (TrampolineBodyClip)FarseerUtil.Query(Builder.World, Builder.ToVec(Clip.Position), 10f, typeof(TrampolineBodyClip));
        if (trampoline != null)
        {
            trampoline.DragEvent.AddListener(OnStartDrag);
            trampoline.HeroTouchEvent.AddListener(OnHeroTouch);
        }
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (trampoline == null)
        {
            Initialize();
        }
    }

    public override void Restart()
    {
        base.Restart();
        HasToRun = false;
        touched = false;
    }

    public override bool HasToHide()
    {
        return false;
    }

    private void OnHeroTouch()
    {
        if (!trampoline.Dragging && !touched)
        {
            touched = true;
            Show();
        }
    }

    private void OnStartDrag()
    {
        if (touched && !Hiding)
        {
            Hiding = true;
            Hide(1f);
        }
    }
}
