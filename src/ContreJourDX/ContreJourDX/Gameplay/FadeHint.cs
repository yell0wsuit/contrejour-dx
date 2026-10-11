using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class FadeHint : HintBase, IRemovable, IRestartable
    {
        protected bool HasToRun { get; set; }

        protected bool Hiding { get; set; }

        protected virtual float FadeInTime => 2f;

        private readonly List<Action> callAfters = [];

        public override bool ShouldRemove => false;

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
        public FadeHint(ContreJourDXLevelBuilder builder, object body, Sprite clip, Hashtable config)
            : base(builder, null, clip, config)
        {
            builder.ContreJourDX.AddUpdatable(this);
            builder.ContreJourDX.AddTextureToUnload(clip.Texture.Name);
            Clip.OpacityByte = 0;
            Clip.Visible = false;
            HasToRun = true;
        }

        public virtual void Restart()
        {
            Clip.Tweener.Stop();
            _ = Clip.FadeOutAndHide(0.2f);
            HasToRun = true;
            Hiding = false;
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
            if (((ContreJourDXGame)Builder.Game).TouchEnabled && HasToRun)
            {
                HasToRun = false;
                CallAfterDelay(Show, 2f);
            }
        }

        public void Show()
        {
            if (!Hiding)
            {
                Clip.Visible = true;
                Clip.Tweener.Stop();
                _ = Clip.FadeIn(FadeInTime);
                if (HasToHide())
                {
                    CallAfterDelay(Hide, 5f);
                }
            }
        }

        public void Hide(float time)
        {
            Clip.Tweener.Stop();
            _ = Clip.FadeOutAndHide(time);
        }

        public void Hide()
        {
            if (!Hiding)
            {
                Hiding = true;
                Hide(Clip.OpacityByte / 255f);
            }
        }
    }
}
