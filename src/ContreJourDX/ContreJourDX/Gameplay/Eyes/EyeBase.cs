using System.Numerics;

using ContreJourDX.Clips;
using ContreJourDX.Content;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay.Eyes
{
    public abstract class EyeBase : Node
    {
        public virtual float EyeStep { get; set; } = 0.5f;

        protected Sprite Background { get; set; }

        protected Sprite EyeBallSprite { get; set; }

        protected IAnimatedNode EndDispatcher { get; set; }

        public Node CurrentBackground { get; private set; }

        protected Node CurrentEyeBall { get; set; }

        private bool lockX;

        private bool lockY;

        private Vector2 targetPosition = Vector2.Zero;

        private bool dirty = true;

        private readonly Node content = new();
        private readonly Node eyeBallContent;

        protected virtual bool MaskEyeBall => false;

        public float ViewDistance
        {
            get;
            set
            {
                if (value != field)
                {
                    field = value.Clamp(0f, 1f);
                    dirty = true;
                }
            }
        }

        public float ViewAngle
        {
            get; set
            {
                if (value != field)
                {
                    field = value;
                    dirty = true;
                }
            }
        }

        protected virtual float ViewRadius => 7f;

        protected ContreJourDXGame Game { get; }

        protected EyeBase(ContreJourDXGame game)
        {
            Game = game;
            eyeBallContent = MaskEyeBall ? new AlphaMaskedNode() : content;
            CreateDefaultView();
            AddChild(content);
            SetDefaultView();
        }

        protected virtual string ProcessName(string name)
        {
            return name;
        }

        protected Node CreateEyeBall(EyeAnimation animation)
        {
            return Create(animation.EyeBall);
        }

        protected Node CreateBackground(EyeAnimation animation)
        {
            return Create(animation.Background);
        }

        private Node Create(string name)
        {
            return name == null ? null : ClipCatalog.Create(ProcessName(name));
        }

        protected virtual void CreateDefaultView()
        {
            Background = new Sprite(ClipIds.Common.McEye);
            EyeBallSprite = new Sprite(ClipIds.Common.McEyeBall);
        }

        public void SetDefaultView()
        {
            SuspendLayout();
            if (CurrentBackground != null)
            {
                Background.Position = CurrentBackground.Position;
            }
            if (CurrentEyeBall != null)
            {
                EyeBallSprite.Position = CurrentEyeBall.Position;
            }
            EndDispatcher = null;
            CurrentBackground = Background;
            CurrentEyeBall = EyeBallSprite;
            lockX = lockY = false;
            RefreshLayout();
        }

        private void SuspendLayout()
        {
            if (CurrentEyeBall != null)
            {
                eyeBallContent.RemoveChild(CurrentEyeBall);
            }
            if (CurrentBackground != null)
            {
                content.RemoveChild(CurrentBackground);
            }
        }

        protected virtual void RefreshLayout()
        {
            if (CurrentBackground != null && CurrentBackground.Parent == null)
            {
                content.AddChild(CurrentBackground);
            }
            if (CurrentEyeBall != null && CurrentEyeBall.Parent == null)
            {
                if (eyeBallContent is AlphaMaskedNode masked)
                {
                    masked.Mask = (Sprite)CurrentBackground;
                    // Reinsert after the background each time the animation changes.
                    if (masked.Parent != null)
                    {
                        content.RemoveChild(masked);
                    }
                    content.AddChild(masked);
                }
                eyeBallContent.AddChild(CurrentEyeBall);
            }
        }

        public void SetEyeContent(EyeAnimation animation)
        {
            SuspendLayout();
            lockX = animation.LockX;
            lockY = animation.LockY;
            EndDispatcher = null;
            if (animation.ReplaceBackground)
            {
                Node node = CreateBackground(animation);
                node.Position = CurrentBackground.Position;
                EndDispatcher = (IAnimatedNode)node;
                CurrentBackground = node;
            }
            if (animation.ReplaceEye)
            {
                Node node2 = CreateEyeBall(animation);
                if (!animation.ReplaceBackground)
                {
                    EndDispatcher = (IAnimatedNode)node2;
                }
                CurrentEyeBall = node2;
            }
            RefreshLayout();
        }

        public override void Update(float time)
        {
            if (dirty)
            {
                Refresh();
                dirty = false;
            }
            Vector2 position = EyeBallSprite.Position.StepTo(targetPosition, EyeStep);
            if (lockX)
            {
                position.X = 0f;
            }
            if (lockY)
            {
                position.Y = 0f;
            }
            CurrentEyeBall.Position = position;
        }

        private void Refresh()
        {
            targetPosition = VectorUtil.ToVector(ViewRadius * ViewDistance, ViewAngle);
        }
    }
}
