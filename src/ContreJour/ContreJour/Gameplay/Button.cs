using System;
using System.Numerics;

using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace ContreJour.Gameplay
{
    public class Button : TouchSprite
    {
        public bool StopEventPropagation { get; set; }

        public bool Enabled { get; set; }
        private readonly Sprite pressed;

        private float realScale;

        protected bool Touching { get; set; }

        // When set, a press fires only once held this many seconds, and a shorter one does nothing.
        public float HoldTime { get; set; }

        // The press counting toward HoldTime, if any.
        public Touch HoldingTouch { get; private set; }

        public float HoldProgress => HoldingTouch == null ? 0f : Math.Min(holdElapsed / holdDuration, 1f);

        public float HoldRemaining => HoldingTouch == null ? 0f : Math.Max(holdDuration - holdElapsed, 0f);

        // Raised just before a completed hold fires, with the touch that held it.
        public event Action<Touch> HoldCompleted;

        // How far, in the button's own units, a held finger may drift before the press counts as a drag.
        private const float HoldSlop = 15f;

        private float holdElapsed;

        private float holdDuration;

        // Set for a press that began in hold mode, so its release never fires.
        private bool holdPress;

        private readonly string backgroundName;

        private readonly string pressedName;

        private readonly string iconName;

        // The button again, drawn round as far as a hold has gone (see ShowHoldReveal).
        private Node reveal;

        private RadialSprite revealBackground;

        private RadialSprite revealPressed;

        private RadialSprite revealIcon;

        public float RealScale
        {
            get => realScale;
            set
            {
                realScale = value;
                Scale = value;
            }
        }

        public Sprite Icon { get; }

        public Button(string backgroundFile, string pressedName, string iconName)
            : base(backgroundFile)
        {
            backgroundName = backgroundFile;
            this.pressedName = pressedName;
            this.iconName = iconName;
            realScale = 1f;
            Enabled = true;
            if (pressedName != null)
            {
                pressed = new Sprite(pressedName);
                AddChild(pressed);
                pressed.Visible = false;
                pressed.OpacityByte = 0;
            }
            if (iconName != null)
            {
                Icon = new Sprite(iconName);
                AddChild(Icon);
            }
        }

        public Button(string backgroundFile, string iconName)
            : this(backgroundFile, "menu/McButtonPressed", iconName)
        {
        }

        public Button(string iconName)
            : this("menu/McButtonBackground", iconName)
        {
        }

        public static Button ButtonBigWithIcon(string iconName)
        {
            return new Button("menu/McButtonBackgroundBig", "menu/McButtonPressedBig", iconName);
        }

        public override bool TouchBegin(Touch touch)
        {
            if (!Enabled)
            {
                return false;
            }
            if (StopEventPropagation)
            {
                touch.StopPropagation();
            }
            _ = base.TouchBegin(touch);
            Touching = true;
            holdPress = HoldTime > 0f;
            if (holdPress)
            {
                HoldingTouch = touch;
                holdElapsed = 0f;
                holdDuration = HoldTime;
            }
            if (pressed != null)
            {
                pressed.Visible = true;
                pressed.Tweener.Stop();
                _ = pressed.FadeIn(0.1f);
            }
            _ = this.ScaleTo(0.1f, realScale * 1.1f);
            return true;
        }

        public override bool TouchOut(Touch touch)
        {
            CancelHold();
            HidePressed();
            return Enabled && base.TouchOut(touch);
        }

        public override void TouchEnd(Touch touch)
        {
            HidePressed();
            bool fire = Enabled && !holdPress;
            holdPress = false;
            CancelHold();
            if (fire)
            {
                Fire(touch);
            }
        }

        public void CancelHold()
        {
            HoldingTouch = null;
            holdElapsed = 0f;
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (HoldingTouch == null)
            {
                return;
            }
            if (Vector2.Distance(GlobalToLocal(HoldingTouch.Position), GlobalToLocal(HoldingTouch.InitialPosition)) > HoldSlop)
            {
                CancelHold();
                return;
            }
            holdElapsed += time;
            if (holdElapsed >= holdDuration)
            {
                Touch touch = HoldingTouch;
                CancelHold();
                HidePressed();
                if (Enabled)
                {
                    HoldCompleted?.Invoke(touch);
                    Fire(touch);
                }
            }
        }

        // Draws the button at this opacity within a wedge Progress of the way round from the top, over the button
        // whatever its parents' opacity, so a hold fills a faded button back in; zero hides it.
        public void ShowHoldReveal(float progress, float opacity)
        {
            if (progress <= 0f)
            {
                reveal?.Visible = false;
                return;
            }
            reveal ??= CreateReveal();
            reveal.Visible = true;
            reveal.OpacityFloat = OpacityFloat * opacity;
            revealBackground.Progress = progress;
            if (revealPressed != null)
            {
                revealPressed.Visible = pressed.Visible;
                revealPressed.OpacityFloat = pressed.OpacityFloat;
                revealPressed.Progress = progress;
            }
            if (revealIcon != null)
            {
                revealIcon.Position = Icon.Position;
                revealIcon.Scale = Icon.Scale;
                revealIcon.Progress = progress;
            }
        }

        private Node CreateReveal()
        {
            Node node = new()
            {
                IgnoreParentOpacity = true
            };
            revealBackground = new RadialSprite(backgroundName);
            node.AddChild(revealBackground);
            if (pressedName != null)
            {
                revealPressed = new RadialSprite(pressedName);
                node.AddChild(revealPressed);
            }
            if (iconName != null)
            {
                revealIcon = new RadialSprite(iconName);
                node.AddChild(revealIcon);
            }
            AddChild(node);
            return node;
        }

        private void Fire(Touch touch)
        {
            base.TouchEnd(touch);
            SoundManager.PlayRandomSound(Sounds.Tap, 0.7f);
        }

        public void HidePressed()
        {
            if (Touching)
            {
                Touching = false;
                _ = this.Schedule(0.2f, DoHidePressed);
            }
        }

        private void DoHidePressed()
        {
            if (!Touching)
            {
                _ = this.ScaleTo(0.1f, realScale);
                if (pressed != null)
                {
                    pressed.Tweener.Stop();
                    _ = pressed.FadeOutAndHide(0.3f);
                }
            }
        }

        protected override void OnAddedToStage()
        {
            base.OnAddedToStage();
            Scale = realScale;
        }
    }
}
