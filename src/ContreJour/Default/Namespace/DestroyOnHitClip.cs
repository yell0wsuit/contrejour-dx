using System.Linq;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Sound;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class DestroyOnHitClip(LevelBuilderBase builder, object body, Node clip, Hashtable config) : BodyClip(builder, body, clip, config)
{
    private Explosion explosion;

    public int SnotJoinedCount { get; set; }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
        if (body2.UserData is HeroBodyClip)
        {
            Explode();
        }
    }

    public override void PostSolvePointImpulse(Body body2, Contact point, ContactVelocityConstraint impulse)
    {
        if (impulse.Points[0].NormalImpulse > 7f)
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
        Builder.Add(explosion, 10);
        explosion.Position = Clip.Position;
        explosion.Speed = new RandomRange(100f, 20f);
        explosion.CreateOnStartPosition(25);
        Vector2 textureSize = ((Sprite)Clip).TextureSize;
        int num = 0;
        foreach (GravityParticle particle in explosion.Particles.Cast<GravityParticle>())
        {
            Vector2 source = new(0f, (num * textureSize.Y / 25f) - (textureSize.Y / 2f));
            particle.Position = Builder.ToRootChild(source, Clip) - Clip.Position;
            num++;
        }
    }
}
