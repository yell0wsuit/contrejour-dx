using System.Diagnostics.CodeAnalysis;

using ContreJour.Content;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Default.Namespace;

public class SnotBodyClipBase : ContreJourBodyClip
{
    protected MonsterEye Eye { get; set; }

    protected ContreJourGame game;

    protected SnotSprite ClipContent { get; set; }

    protected Node BaseClip { get; set; }

    protected Node BaseEndClip { get; set; }

    protected Node Container { get; set; }

    protected float EndWidthPixels { get; set; }

    protected float CenterWidth { get; set; }

    protected float EndWidth { get; set; }

    protected float StartWidthPixels { get; set; }

    protected float StartWidth { get; set; }

    public SnotData Physics { get; }

    public virtual Vector2 StartPosition => Physics.GetWorldStartPoint();

    public virtual Body EyeBody => Physics.EyeBody;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Level content creates this by reflection with this signature.")]
    public SnotBodyClipBase(LevelBuilderBase builder, SnotData body, Node clip, Hashtable config)
        : base(builder, body.EndBody, null, config)
    {
        Physics = body;
        game = (ContreJourGame)this.builder.Game;
        Container = new Node();
        Physics.Snot = this;
        InitSizes();
        ClipContent = CreateClip();
        BaseClip = ClipTypesCache.CreateNewNode(BaseClipName());
        BaseClip.Position = this.builder.ToIPadPoint(Physics.GetWorldStartPoint());
        BaseEndClip = ClipTypesCache.CreateNewNode(BaseEndClipName());
        Physics.EndBody.ApplyLinearImpulse(new Vector2(Maths.Random(), Maths.Random()) * Physics.EndBody.Mass);
        Eye = CreateEye();
        AddClipsToStage();
    }

    public virtual void InitSizes()
    {
        EndWidthPixels = 10f;
        CenterWidth = 6f * builder.EngineConfig.SizeMultiplier;
        EndWidth = EndWidthPixels * builder.EngineConfig.SizeMultiplier;
        StartWidthPixels = 28f;
        StartWidth = StartWidthPixels * builder.EngineConfig.SizeMultiplier;
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
        Container.AddChild(ClipContent);
        Container.AddChild(BaseEndClip);
        Container.AddChild(BaseClip);
        if (Eye != null)
        {
            Container.AddChild(Eye);
        }
        builder.Add(Container, Layer());
    }

    protected virtual MonsterEye CreateEye()
    {
        return new MonsterEye((ContreJourGame)builder.Game, visible: false, Physics.EyeBody.Position);
    }

    public virtual SnotSprite CreateClip()
    {
        return new SnotSprite(this, StartWidth, CenterWidth, EndWidth);
    }

    public virtual Vector2 EndPosition()
    {
        return Physics.EndBody.Position;
    }

    public override void Update(float time)
    {
        base.Update(time);
        BaseClip.Position = builder.ToIPadPoint(Physics.GetWorldStartPoint());
        BaseEndClip.Position = builder.ToIPadPoint(EndPosition());
        if (Eye != null && Eye.HasToUpdate)
        {
            Eye.Position = builder.ToIPadPoint(EyeBody.Position);
            Eye.UpdateNode(time);
        }
    }
}
