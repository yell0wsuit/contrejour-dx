using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class FadeHint : HintBase, IRemovable, IRestartable
{
    protected bool hasToRun;

    protected bool hiding;

    private readonly List<Action> callAfters = [];

    public override bool ShouldRemove => false;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public FadeHint(ContreJourLevelBuilder builder, object body, Sprite clip, Hashtable config)
        : base(builder, null, clip, config)
    {
        builder.ContreJour.AddUpdatable(this);
        builder.ContreJour.AddTextureToUnload(clip.Texture.Name);
        this.clip.OpacityByte = 0;
        this.clip.Visible = false;
        hasToRun = true;
    }

    public virtual void Restart()
    {
        clip.Tweener.Stop();
        _ = clip.FadeOutAndHide(0.2f);
        hasToRun = true;
        hiding = false;
        foreach (Action callAfter in callAfters)
        {
            UnSchedule(callAfter);
        }
        callAfters.Clear();
    }

    private void CallAfterDelay(Action action, float delay)
    {
        Schedule(action, delay);
        callAfters.Add(action);
    }

    public virtual bool HasToHide()
    {
        return true;
    }

    public override void Update(float time)
    {
        if (((ContreJourGame)builder.Game).TouchEnabled && hasToRun)
        {
            hasToRun = false;
            CallAfterDelay(Show, 2f);
        }
    }

    public void Show()
    {
        if (!hiding)
        {
            clip.Visible = true;
            clip.Tweener.Stop();
            _ = clip.FadeIn(2f);
            if (HasToHide())
            {
                CallAfterDelay(Hide, 5f);
            }
        }
    }

    public void Hide(float time)
    {
        clip.Tweener.Stop();
        _ = clip.FadeOutAndHide(time);
    }

    public void Hide()
    {
        if (!hiding)
        {
            hiding = true;
            Hide(clip.OpacityByte / 255f);
        }
    }
}
