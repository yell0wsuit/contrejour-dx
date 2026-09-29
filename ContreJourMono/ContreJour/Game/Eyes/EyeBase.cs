using ContreJour.Clips.common;
using ContreJour.Content;

using Default.Namespace;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourMono.ContreJour.Game.Eyes;

public abstract class EyeBase : Node
{
    protected float eyeStep = 0.5f;

    protected Sprite Background { get; set; }

    protected Sprite eyeBall;

    protected IAnimatedNode EndDispatcher { get; set; }

    protected Node currentBackground;

    protected Node CurrentEyeBall { get; set; }

    private bool lockX;

    private bool lockY;

    private readonly bool useMask;
    private Vector2 targetPosition = Vector2.Zero;

    private bool dirty = true;
    private readonly AnimatedMaskedSprite mask;

    private readonly Node content = new();

    public virtual float EyeStep
    {
        get => eyeStep;
        set => eyeStep = value;
    }

    public Node CurrentBackground => currentBackground;

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

    protected ContreJourGame Game { get; }

    protected EyeBase(ContreJourGame game)
        : this(game, useMask: false, Vector2.Zero)
    {
    }

    protected EyeBase(ContreJourGame game, bool useMask, Vector2 maskSize)
    {
        Game = game;
        useMask = useMask && Mokus2DGame.Config.RenderTargetEnabled;
        this.useMask = useMask && Mokus2DGame.Config.RenderTargetEnabled;
        CreateDefaultView();
        if (useMask)
        {
            mask = new AnimatedMaskedSprite(maskSize)
            {
                UpdateMask = false
            };
            mask.AddChild(content);
            AddChild(mask);
        }
        else
        {
            AddChild(content);
        }
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
        return name == null ? null : ClipTypesCache.CreateNewNode(ProcessName(name));
    }

    protected virtual void CreateDefaultView()
    {
        Background = new McEye();
        eyeBall = new McEyeBall();
    }

    public void SetDefaultView()
    {
        SuspendLayout();
        if (currentBackground != null)
        {
            Background.Position = currentBackground.Position;
        }
        if (CurrentEyeBall != null)
        {
            eyeBall.Position = CurrentEyeBall.Position;
        }
        EndDispatcher = null;
        currentBackground = Background;
        CurrentEyeBall = eyeBall;
        lockX = lockY = false;
        RefreshLayout();
    }

    private void SuspendLayout()
    {
        if (CurrentEyeBall != null)
        {
            content.RemoveChild(CurrentEyeBall);
        }
        if (currentBackground != null)
        {
            content.RemoveChild(currentBackground);
        }
    }

    protected virtual void RefreshLayout()
    {
        if (currentBackground != null && currentBackground.Parent == null)
        {
            content.AddChild(currentBackground);
            if (useMask)
            {
                mask.Mask = (Sprite)currentBackground;
            }
        }
        if (CurrentEyeBall != null && CurrentEyeBall.Parent == null)
        {
            content.AddChild(CurrentEyeBall);
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
            node.Position = currentBackground.Position;
            EndDispatcher = (IAnimatedNode)node;
            currentBackground = node;
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
        Vector2 position = eyeBall.Position.StepTo(targetPosition, EyeStep);
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
