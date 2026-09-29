using System;

using ContreJour.Clips.chapter5;
using ContreJour.Clips.common;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Integration.Farseer.Util;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class SuckerBodyClip : ContreJourBodyClip, IClickable, IVectorPositionProvider, IRestartable
{
    private static readonly Vector2 MinBorderOffset = new(30f);

    public readonly EventSender FinishDragEvent = new();

    public readonly EventSender RemoveEvent = new();

    protected readonly Node ghostSprite;

    protected float bounceAngle;

    private Vector2 bouncePosition;

    private readonly Bouncer bouncer;

    protected Vector2 createPosition;

    private bool creating;

    protected SuckerEndBodyClip end;

    private Body endBody;

    private readonly MonsterEye eye;

    private readonly SuckerNeckSprite ghostNeck;

    private readonly Node ghostPimpa;

    private readonly Sprite limit;

    protected float maxDistance;

    protected Fixture middleFixture;

    protected SuckerNeckSprite neck;

    private readonly Node pimpa;

    private readonly Sprite pimpaHighlite;

    private Vector2 pimpaPosition;

    private bool pulled;

    protected Touch touch;

    protected virtual float BounceVolume => 0.3f;

    protected virtual string BounceSound => "landing1";

    public bool Dragging => touch != null;

    public Vector2 TargetPosition
    {
        get
        {
            Vector2 position = builder.TouchRootVec(touch);
            Vector2 vector = Builder.ToVec(MinBorderOffset);
            RectangleFloat levelScreenPhysicsBounds = Game.LevelScreenPhysicsBounds;
            levelScreenPhysicsBounds.Extend(-vector / Game.GameRoot.Scale);
            return levelScreenPhysicsBounds.ClampToBounds(position).ClampDistance(Body.Position, maxDistance);
        }
    }

    public bool DisableHeroFocus => true;

    public SuckerBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        //IL_0043: Unknown result type (might be due to invalid IL or missing references)
        //IL_006c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0076: Expected O, but got Unknown
        clip = new Node();
        this.clip = clip;
        builder.Add(clip, 1);
        Body val = builder.World.CreateCircle(0.1f, ((Body)body).Position);
        val.SetSensor(value: true);
        builder.World.RemoveBody((Body)body);
        Body = val;
        bouncer = new Bouncer(4f, 7f, 3f);
        this.config["noShadow"] = "true";
        float num = this.config.GetFloat("Width");
        maxDistance = num / 2f * this.builder.SizeMult;
        ghostSprite = new Node
        {
            OpacityFloat = 0.5f
        };
        this.clip.AddChild(ghostSprite);
        ghostNeck = CreateNeck();
        ghostNeck.Color = new Color(10f / 51f, 10f / 51f, 10f / 51f, 1f);
        ghostSprite.AddChild(ghostNeck);
        ghostPimpa = CreatePimpa();
        ghostSprite.AddChild(ghostPimpa);
        limit = new McRoundDragFrameView();
        this.builder.Add(limit, -1);
        limit.Position = this.builder.ToPoint(Body.Position);
        limit.Scale = num / 200f;
        neck = CreateNeck();
        this.clip.AddChild(neck);
        Node node = new McSuckerHighlite();
        this.clip.AddChild(node);
        Sprite node2 = new McSuckerStart();
        this.clip.AddChild(node2);
        eye = new MonsterEye(Game, visible: false, Body.Position);
        this.clip.AddChild(eye);
        eye.Visible = false;
        pimpa = new Node();
        this.clip.AddChild(pimpa);
        pimpaHighlite = new McSuckerHighlite();
        pimpa.AddChild(pimpaHighlite);
        Node node3 = CreatePimpa();
        pimpa.AddChild(node3);
    }

    public override float TouchDistance(Vector2 touchPosition)
    {
        return endBody != null
            ? Math.Min(touchPosition.DistanceTo(Body.Position), touchPosition.DistanceTo(endBody.Position))
            : touchPosition.DistanceTo(Body.Position);
    }

    public int Priority(Vector2 touchPosition)
    {
        return 0;
    }

    public bool AcceptFreeTouches()
    {
        return false;
    }

    public bool UseForZoom()
    {
        return false;
    }

    public bool TouchBegan(Touch touch)
    {
        if (creating)
        {
            return false;
        }
        if (this.touch == null)
        {
            Vector2 target = builder.TouchRootVec(touch);
            if (Body.Position.DistanceTo(target) <= 2f || endBody.Position.DistanceTo(target) <= 2f)
            {
                StartDrag(touch);
                return true;
            }
        }
        return false;
    }

    public void TouchEnd(Touch touch)
    {
        FinishDrag();
    }

    public bool TouchMove(Touch touch)
    {
        return true;
    }

    public void TouchOut(Touch touch)
    {
    }

    public virtual void Restart()
    {
        if (end != null && touch == null)
        {
            eye.Open = false;
            eye.Visible = false;
            DestroyBodies();
            ghostSprite.Visible = false;
        }
    }

    public virtual Node CreatePimpa()
    {
        return new McSuckerBodyStrong();
    }

    protected virtual SuckerNeckSprite CreateNeck()
    {
        return new TexturedSuckerNeck("McSuckerStrongSnotTexture");
    }

    private static void RedrawGhost()
    {
    }

    public override void Update(float time)
    {
        base.Update(time);
        bouncer.Update(time);
        bouncePosition = pimpaPosition + VectorUtil.ToVector(bouncer.Value, bounceAngle);
        if (pimpa.Position != bouncePosition)
        {
            pimpa.Position = VectorUtil.StepTo(pimpa.Position, bouncePosition, 1000f * time);
            neck.Length = pimpa.Position.Length();
            neck.RotationDegrees = MathHelper.ToDegrees(Maths.Atan2(pimpa.Position.Y, pimpa.Position.X));
        }
        limit.OpacityByte = (int)Maths.StepTo(limit.OpacityByte, (touch != null) ? 200 : 80, time * 200f);
        pimpaHighlite.OpacityByte = (int)Maths.StepTo(pimpaHighlite.OpacityByte, (end != null) ? 255 : 0, time * 600f);
        if (touch != null)
        {
            RefreshGhostPosition(time, TargetPosition);
        }
        eye.UpdateNode(time);
        neck.Update(time);
    }

    public void RefreshGhostPosition(float time, Vector2 position)
    {
        Vector2 vector = builder.ToPoint(position - Body.Position);
        ghostPimpa.Position = new Vector2(vector.Length(), 0f);
        ghostNeck.Length = ghostPimpa.Position.X;
        ghostNeck.UpdateNode(time);
        RedrawGhost();
        ghostSprite.RotationDegrees = MathHelper.ToDegrees(Maths.Atan2(vector.Y, vector.X));
    }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
        if (body2.UserData is BodyClip bodyClip && bodyClip is HeroBodyClip && FarseerUtil.GetWorldPoint(point).DistanceTo(Body.Position) > 1.1666666f && pulled)
        {
            neck.Bounce();
            PlayBounceSound();
        }
    }

    protected void PlayBounceSound()
    {
        SoundManager.PlaySound(BounceSound, BounceVolume);
    }

    public virtual void StartDrag(Touch touch)
    {
        if (!creating)
        {
            if (this.touch != null)
            {
                throw new InvalidOperationException("already dragging");
            }
            SoundManager.PlaySound("leapOn1");
            ghostSprite.Tweener.Stop();
            ghostSprite.Visible = true;
            this.touch = touch;
        }
    }

    public void DestroyBodies()
    {
        pulled = false;
        end = null;
        builder.World.RemoveBody(endBody);
        Body.DestroyFixture(middleFixture);
        Body.SetSensor(value: true);
        pimpaPosition = Vector2.Zero;
        bouncer.Start();
        bouncer.Start();
    }

    public virtual void CreateBodies()
    {
        //IL_0091: Unknown result type (might be due to invalid IL or missing references)
        //IL_0097: Expected O, but got Unknown
        SoundManager.PlaySound("vysovuvannja", 0.3f);
        creating = false;
        pulled = true;
        endBody = builder.World.CreateCircle(0.1f, createPosition);
        float num = createPosition.DistanceTo(Body.Position);
        float angle = VectorUtil.Atan2(Body.Position, createPosition);
        PolygonShape shape = new(builder.EngineConfig.Density);
        shape.SetAsBox(num / 2f, 0.1f, VectorUtil.Rotate(new Vector2(num / 2f, 0f), angle), angle);
        bounceAngle = angle;
        middleFixture = FarseerUtil.AddShape((Shape)(object)shape, Body, builder.EngineConfig.Density);
        Body.SetSensor(value: false);
        end = new SuckerEndBodyClip(this, endBody);
        pimpaPosition = builder.ToPoint(endBody.Position - Body.Position);
        bouncer.Start();
        neck.LightBounce();
        eye.Open = true;
        eye.PositionProvider = this;
        Schedule(RefreshPositionProvider, Maths.Random(1.5f, 2.5f));
        FinishDragEvent.SendEvent();
    }

    private void RefreshPositionProvider()
    {
        eye.PositionProvider = null;
    }

    public void CancelDrag()
    {
        touch = null;
    }

    public void FinishDrag()
    {
        if (creating)
        {
            return;
        }
        bool flag = pulled;
        if (pulled)
        {
            DestroyBodies();
        }
        if (TargetPosition.DistanceTo(Body.Position) > 1.5f)
        {
            createPosition = TargetPosition;
            RefreshGhostPosition(0f, createPosition);
            if (flag)
            {
                creating = true;
                Schedule(CreateBodies, 0.2f);
            }
            else
            {
                CreateBodies();
            }
        }
        else
        {
            eye.Open = false;
            ghostSprite.Visible = false;
            if (flag)
            {
                RemoveEvent.SendEvent();
            }
        }
        touch = null;
    }
}
