using System;
using System.Collections.Generic;
using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class FadeHint : HintBase, IRemovable, IRestartable
{
    protected bool hasToRun;

    protected bool hiding;

    protected List<Action> callAfters = new List<Action>();

    public override bool ShouldRemove => false;

    public FadeHint(ContreJourLevelBuilder _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, null, _clip, _config)
    {
        _builder.ContreJour.AddUpdatable(this);
        _builder.ContreJour.AddTextureToUnload(_clip.Texture.Name);
        clip.OpacityByte = 0;
        clip.Visible = false;
        hasToRun = true;
    }

    public virtual void Restart()
    {
        clip.Tweener.Stop();
        clip.FadeOutAndHide(0.2f);
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
        Show(2f);
    }

    public void Show(float time)
    {
        if (!hiding)
        {
            clip.Visible = true;
            clip.Tweener.Stop();
            clip.FadeIn(2f);
            if (HasToHide())
            {
                CallAfterDelay(Hide, 5f);
            }
        }
    }

    public void Hide(float time)
    {
        clip.Tweener.Stop();
        clip.FadeOutAndHide(time);
    }

    public void Hide()
    {
        if (!hiding)
        {
            hiding = true;
            Hide((float)clip.OpacityByte / 255f);
        }
    }
}
