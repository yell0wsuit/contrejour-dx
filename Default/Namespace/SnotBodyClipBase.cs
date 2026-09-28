using ContreJour.Content;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SnotBodyClipBase : ContreJourBodyClip
{
    private readonly SnotData physics;

    protected MonsterEye eye;

    protected ContreJourGame game;

    protected SnotSprite clipContent;

    protected Node baseClip;

    protected Node baseEndClip;

    protected Node container;

    protected float endWidthPixels;

    protected float centerWidth;

    protected float endWidth;

    protected float startWidthPixels;

    protected float startWidth;

    public SnotData Physics => physics;

    public virtual Vector2 StartPosition => physics.GetWorldStartPoint();

    public virtual Body EyeBody => physics.EyeBody;

    public SnotBodyClipBase(LevelBuilderBase _builder, SnotData _body, Node _clip, Hashtable _config)
        : base(_builder, _body.EndBody, null, _config)
    {
        physics = _body;
        game = (ContreJourGame)builder.Game;
        container = new Node();
        physics.Snot = this;
        InitSizes();
        clipContent = CreateClip();
        baseClip = ClipTypesCache.CreateNewNode(BaseClipName());
        baseClip.Position = builder.ToIPadPoint(physics.GetWorldStartPoint());
        baseEndClip = ClipTypesCache.CreateNewNode(BaseEndClipName());
        physics.EndBody.ApplyLinearImpulse(new Vector2(Maths.Random(), Maths.Random()) * physics.EndBody.Mass);
        eye = CreateEye();
        AddClipsToStage();
    }

    public virtual void InitSizes()
    {
        endWidthPixels = 10f;
        centerWidth = 6f * builder.EngineConfig.SizeMultiplier;
        endWidth = endWidthPixels * builder.EngineConfig.SizeMultiplier;
        startWidthPixels = 28f;
        startWidth = startWidthPixels * builder.EngineConfig.SizeMultiplier;
    }

    public virtual int Layer()
    {
        return 0;
    }

    public virtual string BaseEndClipName()
    {
        return "McSnotEnd";
    }

    public virtual string BaseClipName()
    {
        return "McSnotStart";
    }

    public virtual void AddClipsToStage()
    {
        container.AddChild(clipContent);
        container.AddChild(baseEndClip);
        container.AddChild(baseClip);
        if (eye != null)
        {
            container.AddChild(eye);
        }
        builder.Add(container, Layer());
    }

    protected virtual MonsterEye CreateEye()
    {
        return new MonsterEye((ContreJourGame)builder.Game, _visible: false, physics.EyeBody.Position);
    }

    public virtual SnotSprite CreateClip()
    {
        return new SnotSprite(this, startWidth, centerWidth, endWidth);
    }

    public virtual Vector2 EndPosition()
    {
        return physics.EndBody.Position;
    }

    public override void Update(float time)
    {
        base.Update(time);
        baseClip.Position = builder.ToIPadPoint(physics.GetWorldStartPoint());
        baseEndClip.Position = builder.ToIPadPoint(EndPosition());
        if (eye != null && eye.HasToUpdate)
        {
            eye.Position = builder.ToIPadPoint(EyeBody.Position);
            eye.UpdateNode(time);
        }
    }
}
