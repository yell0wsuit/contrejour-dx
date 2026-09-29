using System;

using Mokus2D.Effects.OnOff;
using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Mokus2D.UI.Controls.Buttons
{
    public class Button
    {
        private readonly AnchorNode Content;

        public IOnOff Effect { get; }
        private Touch _pressTouch;

        public bool MouseOver { get; private set; }

        public bool Pressed
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

        public bool HighliteOnMouseOver
        {
            get;
            set
            {
                field = value;
                Content.ProcessMouseOver = value;
                if (!value)
                {
                    MouseOver = false;
                }
                RefreshButton();
            }
        }

        public event Action<Button, Touch> ClickEvent;

        public event Action<Button, Touch> TouchBeginEvent;

        public Button(AnchorNode content, IOnOff effect)
        {
            Content = content;
            Effect = effect;
            RefreshButton();
            Content.Clickable = true;
            Content.TouchBeginEvent += OnTouchBegin;
            Content.TouchOutEvent += OnTouchOut;
            Content.TouchEndEvent += OnTouchEnd;
            Content.MouseOverEvent += OnMouseOver;
            Content.MouseOutEvent += OnMouseOut;
        }

        private void OnMouseOver()
        {
            if (HighliteOnMouseOver)
            {
                MouseOver = true;
                RefreshButton();
            }
        }

        private void OnMouseOut()
        {
            if (HighliteOnMouseOver)
            {
                MouseOver = false;
                RefreshButton();
            }
        }

        protected virtual void RefreshButton()
        {
            Effect.IsOn = Pressed || MouseOver;
        }

        public void ResetEffect()
        {
            Effect.SetOn(value: false);
        }

        public virtual void Clear()
        {
            Pressed = false;
            MouseOver = false;
        }

        protected virtual void OnTouchOut(TouchArguments obj)
        {
            if (obj.Touch == _pressTouch)
            {
                Pressed = false;
                _pressTouch = null;
            }
        }

        protected virtual void OnTouchBegin(TouchArguments touchArguments)
        {
            Pressed = true;
            _pressTouch = touchArguments.Touch;
            TouchBeginEvent.Dispatch(this, touchArguments.Touch);
        }

        protected virtual void OnTouchEnd(TouchArguments obj)
        {
            if (obj.Touch == _pressTouch)
            {
                Pressed = false;
                _pressTouch = null;
                ClickEvent.Dispatch(this, obj.Touch);
            }
        }
    }
}
