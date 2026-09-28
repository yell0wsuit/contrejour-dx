using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;

namespace Default.Namespace;

public class GrassController : IGrassController, IUpdatable
{
    private const int FLYES_COUNT_LAST_CHAPTER = 1;

    private const int FLYES_COUNT = 2;

    private const float SMALL_GRASS_ANGLE_DIFF = 4f;

    private const float GRASS_ANGLE_DIFF = 2.5f;

    private const float GRASS_ANGLE_DIFF_BACK = 1f;

    private const float FLY_MIN_OFFSET = 1.3333334f;

    private const float FLY_MAX_OFFSET = 2f;

    private const float TRAMPLE_ANGLE = (float)Math.PI / 6f;

    private const int NO_TOUCH_FRAMES = 5;

    private const int SMALL_GRASS_ON_GROUND = 3;

    private const int SMALL_GRASS_COUNT = 5;

    private const float SMALL_GRASS_SCALE = 0.5f;

    private const float GRASS_SCALE = 0.65f;

    private const int GRASS_COUNT = 8;

    protected PlasticinePartBodyClip plasticine;

    protected ContreJourGame game;

    protected ContreJourLevelBuilder builder;

    protected Particle grass;

    protected List<GrassAndPosition> smallGrasses;

    protected WindData windData;

    protected List<object> flyes;

    protected float startAngle;

    protected bool touched;

    protected float touchDistance;

    protected int notTouchedFrames;

    protected BodyClip touchingObject;

    protected float touchStartOffset;

    protected float touchOffset;

    protected float smallGrassRotation;

    public Particle Grass => grass;

    public virtual float Y => grass.Position.Y;

    public virtual int GrassFrame => Maths.Random(8);

    public virtual float WindAngle => (float)Math.PI / 6f;

    public virtual int SmallGrassFrame => Maths.Random(5) + 8;

    public virtual float SmallGrassScale => 0.5f;

    public virtual float TrampleAngle => (float)Math.PI / 6f;

    public virtual float SmallGrassStep => !touched ? 1f : 4f;

    public virtual float GrassStep => !touched ? 1f : 2.5f;

    public virtual float GetSmallGrassOffset(int index)
    {
        return Maths.Random(0f - plasticine.Width, plasticine.Width);
    }

    public GrassController(PlasticinePartBodyClip _plasticine)
    {
        plasticine = _plasticine;
        builder = (ContreJourLevelBuilder)plasticine.Builder;
        game = (ContreJourGame)plasticine.Builder.Game;
        startAngle = Maths.Random(-(float)Math.PI / 12f, (float)Math.PI / 12f);
        touched = false;
        touchDistance = 0f;
        notTouchedFrames = 0;
        Create();
    }

    public void Create()
    {
        int frame = Math.Min(GrassFrame, game.Grass.TotalFrames - 1);
        grass = game.Grass.AddParticleWithFrame(frame);
        grass.Position = builder.ToIPadPoint(plasticine.GetSurfaceCenter());
        RandomizeClipMinScaleMaxScale(grass, 0.45499998f, 0.65f);
        windData = new WindData(WindAngle);
        CreateSmallGrass();
        if (!plasticine.Parent.Config.GetBool("disableFlyes"))
        {
            CreateFlyes();
        }
    }

    public void CreateSmallGrass()
    {
        smallGrasses = [];
        for (int i = 0; i < 3; i++)
        {
            int frame = Math.Min(SmallGrassFrame, game.Grass.TotalFrames - 1);
            Particle particle = game.Grass.AddParticleWithFrame(frame);
            RandomizeClipMinScaleMaxScale(particle, 0.6f * SmallGrassScale, SmallGrassScale);
            float smallGrassOffset = GetSmallGrassOffset(i);
            particle.Position = new Vector2(smallGrassOffset / builder.EngineConfig.SizeMultiplier, 2f) + grass.Position;
            smallGrasses.Add(new GrassAndPosition(particle, new Vector2(smallGrassOffset, 0f)));
        }
    }

    public virtual void Update(float time)
    {
        if (!touched)
        {
            notTouchedFrames++;
        }
        UpdateGrassRotation(time);
        UpdateGrassPosition();
        foreach (FlyController flye in flyes.Cast<FlyController>())
        {
            flye.Update(time);
        }
    }

    public void UpdateGrassRotation(float time)
    {
        float wind = game.WindManager.GetWind(windData.WindOffset);
        float num = plasticine.Body.Rotation + windData.GetAngle(wind) + startAngle;
        float num2 = num;
        if (notTouchedFrames >= 5)
        {
            touchDistance = 0f;
            touchingObject = null;
        }
        if (touchingObject != null)
        {
            float num3 = touchOffset - touchStartOffset;
            float num4 = 2.6666667f;
            if (num3 / touchStartOffset <= 0f && Math.Abs(num3) < num4)
            {
                touchDistance = Math.Sign(num3) * Math.Min(Math.Abs(num3 / 1.3333334f), 1f);
                float num5 = (0f - Math.Min(num3 / 2f, 1f)) * TrampleAngle;
                num += num5 * 3f / 2f;
                num2 += num5 * 2f;
            }
            else
            {
                touchingObject = null;
                touchDistance = 0f;
            }
        }
        float target = num.ToDegrees();
        float num6 = time * 30f;
        float smallGrassStep = SmallGrassStep;
        float grassStep = GrassStep;
        smallGrassRotation = Maths.StepTo(smallGrassRotation, num2.ToDegrees(), smallGrassStep * num6);
        grass.RotationDegrees = Maths.StepTo(grass.RotationDegrees, target, grassStep * num6);
        touched = false;
    }

    public void UpdateGrassPosition()
    {
        Vector2 surfaceCenter = plasticine.GetSurfaceCenter();
        grass.Position = builder.ToIPadPoint(surfaceCenter);
        foreach (GrassAndPosition smallGrass in smallGrasses)
        {
            Vector2 vector = smallGrass.Position + PlasticinePartBodyClip.GetLocalSurfaceCenter();
            Vector2 worldPoint = plasticine.Body.GetWorldPoint(vector);
            smallGrass.Particle.Position = builder.ToIPadPoint(worldPoint);
            smallGrass.Particle.RotationDegrees = LevelBuilderBase.ToRotation(plasticine.Body.Rotation);
            smallGrass.Particle.RotationDegrees = smallGrassRotation;
        }
    }

    public void CreateFlyes()
    {
        flyes = [];
        int num = (game.RoseChapter || game.BonusChapter) ? 1 : 2;
        for (int i = 0; i < num; i++)
        {
            Vector2 vec = new(Maths.Random(0f - plasticine.Width, plasticine.Width), Maths.Random(1.3333334f, 2f));
            vec += plasticine.Body.Position;
            Particle particle = game.Flyes.AddParticle(builder.ToIPadPoint(vec));
            flyes.Add(new FlyController(game, plasticine, particle));
        }
    }

    public void OnTouchWith(float offset, BodyClip objectP)
    {
        ScareFlyes(offset);
        if (touchingObject == null || (objectP != touchingObject && Math.Abs(offset) < Math.Abs(touchOffset)))
        {
            touchStartOffset = Math.Sign(offset) * 1.3333334f;
            touchingObject = objectP;
        }
        if (touchingObject == objectP)
        {
            touchOffset = offset;
            notTouchedFrames = 0;
        }
        touched = true;
    }

    public void ScareFlyes(float offset)
    {
        if (Maths.FuzzyEquals(offset, 0f))
        {
            offset = (Maths.Random() > 0.5f) ? 1 : (-1);
        }
        foreach (FlyController flye in flyes.Cast<FlyController>())
        {
            flye.Scare(-Math.Sign(offset));
        }
    }

    public static void RandomizeClipMinScaleMaxScale(Particle _clip, float minScale, float maxScale)
    {
        _clip.Scale = Maths.Random(minScale, maxScale);
    }
}
