using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Mokus2D.Sound;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class DestroyOnHitClip : BodyClip
{
    private const int EXPLOSION_PARTICLES = 25;

    private const float MIN_IMPULSE = 7f;

    protected Explosion explosion;

    protected int snotJoinedCount;

    public int SnotJoinedCount
    {
        get
        {
            return snotJoinedCount;
        }
        set
        {
            snotJoinedCount = value;
        }
    }

    public DestroyOnHitClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
    }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
        if (body2.UserData is HeroBodyClip)
        {
            Explode();
        }
    }

    public override void PostSolvePointImpulse(Body body2, Contact point, ContactVelocityConstraint impulse)
    {
        if (impulse.points[0].normalImpulse > 7f)
        {
            Explode();
        }
    }

    public void Explode()
    {
        UserData.Instance.BlocksDestroyed++;
        SoundManager.PlayRandomSound(Sounds.EXPLOSIONS, Maths.Random(0.2f, 0.6f));
        DestroyLater();
        explosion = new Explosion("McGreySmoke");
        builder.Add(explosion, 10);
        explosion.Position = clip.Position;
        explosion.Speed = new RandomRange(100f, 20f);
        explosion.CreateOnStartPosition(25);
        Vector2 textureSize = ((Sprite)clip).TextureSize;
        int num = 0;
        foreach (GravityParticle particle in explosion.Particles)
        {
            Vector2 source = new Vector2(0f, (float)num * textureSize.Y / 25f - textureSize.Y / 2f);
            particle.Position = builder.ToRootChild(source, clip) - clip.Position;
            num++;
        }
    }
}
