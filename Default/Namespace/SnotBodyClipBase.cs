using System.Diagnostics.CodeAnalysis;

using ContreJour.Content;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SnotBodyClipBase : ContreJourBodyClip
{
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

    public SnotData Physics { get; }

    public virtual Vector2 StartPosition => Physics.GetWorldStartPoint();

    public virtual Body EyeBody => Physics.EyeBody;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public SnotBodyClipBase(LevelBuilderBase builder, SnotData body, Node clip, Hashtable config)
        : base(builder, body.EndBody, null, config)
    {
        Physics = body;
        game = (ContreJourGame)this.builder.Game;
        container = new Node();
        Physics.Snot = this;
        InitSizes();
        clipContent = CreateClip();
        baseClip = ClipTypesCache.CreateNewNode(BaseClipName());
        baseClip.Position = this.builder.ToIPadPoint(Physics.GetWorldStartPoint());
        baseEndClip = ClipTypesCache.CreateNewNode(BaseEndClipName());
        Physics.EndBody.ApplyLinearImpulse(new Vector2(Maths.Random(), Maths.Random()) * Physics.EndBody.Mass);
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
        return new MonsterEye((ContreJourGame)builder.Game, visible: false, Physics.EyeBody.Position);
    }

    public virtual SnotSprite CreateClip()
    {
        return new SnotSprite(this, startWidth, centerWidth, endWidth);
    }

    public virtual Vector2 EndPosition()
    {
        return Physics.EndBody.Position;
    }

    public override void Update(float time)
    {
        base.Update(time);
        baseClip.Position = builder.ToIPadPoint(Physics.GetWorldStartPoint());
        baseEndClip.Position = builder.ToIPadPoint(EndPosition());
        if (eye != null && eye.HasToUpdate)
        {
            eye.Position = builder.ToIPadPoint(EyeBody.Position);
            eye.UpdateNode(time);
        }
    }
}
