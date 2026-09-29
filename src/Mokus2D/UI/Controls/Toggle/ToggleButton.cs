using System;

using Mokus2D.Effects.OnOff;
using Mokus2D.UI.Controls.Buttons;
using Mokus2D.Util;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Mokus2D.UI.Controls.Toggle
{
    public class ToggleButton(AnchorNode content, IOnOff effect) : Button(content, effect)
    {
        public bool ToggleOnTouchBegin { get; set; }

        private readonly bool HighliteOnPressed = true;

        public bool Toggle
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    RefreshButton();
                }
            }
        }

        public event Action<ToggleButton> ToggleChangeEvent;

        public void ForceEffect()
        {
            Effect.SetOn(Toggle);
        }

        protected override void RefreshButton()
        {
            Effect.IsOn = (Pressed && HighliteOnPressed) || Toggle || MouseOver;
        }

        protected override void OnTouchBegin(TouchArguments touchArguments)
        {
            base.OnTouchBegin(touchArguments);
            if (ToggleOnTouchBegin)
            {
                ChangeToggle();
            }
        }

        protected override void OnTouchEnd(TouchArguments obj)
        {
            if (Pressed)
            {
                if (!ToggleOnTouchBegin)
                {
                    ChangeToggle();
                }
                Pressed = false;
            }
            base.OnTouchEnd(obj);
        }

        private void ChangeToggle()
        {
            Toggle = !Toggle;
            ToggleChangeEvent.Dispatch(this);
        }
    }
}
