using System;
using System.IO;

using ContreJourMono.ContreJour.Game.Hero;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourMono.ContreJour.Menu.LevelComplete;

public class FakeHero : Node
{
    private const float ViewDistance = 200f;

    protected static readonly string TextureFolder = "fakeHero";

    private readonly Sprite background;

    private readonly FakeHeroEye eye;

    private readonly Sprite hotSpot;

    private readonly Sprite shadow;

    private readonly HeroTail tail;

    private float speed;

    private Vector2 viewTarget;

    public HeroTail Tail => tail;

    protected virtual Color TailColor => Color.Black;

    public Sprite Background => background;

    public FakeHeroEye Eye => eye;

    public float Speed
    {
        get => speed;
        set
        {
            speed = value;
            tail.Speed = value;
        }
    }

    public Sprite HotSpot => hotSpot;

    public new Vector2 Position
    {
        get => base.Position;
        set
        {
            base.Position = value;
        }
    }

    public Vector2 ViewTarget
    {
        set
        {
            viewTarget = value;
            Vector2 vector = Parent.LocalToNode(value, this);
            eye.ViewAngle = (float)Math.Atan2(vector.Y, vector.X);
            eye.ViewDistance = vector.Length() / 200f;
        }
    }

    public FakeHero()
    {
        background = new Sprite(ProcessName("McFakeHeroBackground"));
        shadow = new Sprite(ProcessName("McFakeHeroShadow"));
        hotSpot = new Sprite(ProcessName("McFakeHeroHotspot"));
        tail = new HeroTail(TailColor);
        AddChild(tail);
        tail.LimitAngles = true;
        tail.Scale = 2f;
        tail.Speed = 0f;
        AddChild(shadow);
        AddChild(background);
        AddChild(hotSpot);
        eye = CreateEye();
        AddChild(eye);
    }

    public void LookAt(Node node)
    {
        ViewTarget = node.LocalToNode(Vector2.Zero, Parent);
    }

    protected virtual string ProcessName(string name)
    {
        return Path.Combine([TextureFolder, name]);
    }

    protected virtual FakeHeroEye CreateEye()
    {
        return new FakeHeroEye();
    }

    public void SetMoveAngle(float angle, float speed)
    {
        tail.SetMovementDirection(angle);
        Speed = speed;
    }

    public void SetViewAngle(float angle, float ratio)
    {
        eye.ViewAngle = angle;
        eye.ViewDistance = ratio;
    }

    public override void Update(float time)
    {
        base.Update(time);
    }
}
