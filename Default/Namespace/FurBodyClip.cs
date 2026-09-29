using System;
using System.Collections.Generic;

using ContreJour.Clips.chapter5;

using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public abstract class FurBodyClip : ContreJourBodyClip
{
    private readonly List<RotatorGrass> grass = [];

    private readonly FurCircle grassSystem;

    private readonly float grassStep;

    private readonly float trampleAngle;

    private readonly Sprite baseSprite;

    public FurBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        if (clip == null)
        {
            clip = new Node
            {
                Scale = config.GetVector("scale").X
            };
            _ = builder.AddChild(clip);
            this.clip = clip;
        }
        grassStep = (float)Math.PI * 2f / GrassCount();
        trampleAngle = 4f * grassStep;
        baseSprite = new McRotatorBase();
        baseSprite.Scale = Width() / baseSprite.TextureSize.X;
        this.clip.AddChild(baseSprite);
        grassSystem = CreateFur();
        CreateGrass();
    }

    public FurCircle CreateFur()
    {
        FurCircle furCircle = new(GrassTexture(), GrassCount(), GrassRadius());
        clip.AddChild(furCircle);
        return furCircle;
    }

    public abstract string GrassTexture();

    public abstract int GrassCount();

    public abstract int GrassRadius();

    public abstract float Width();

    public virtual void SetBaseWidth(float newWidth)
    {
        baseSprite.Scale = newWidth / baseSprite.TextureSize.X;
    }

    public override void Update(float time)
    {
        base.Update(time);
        float angle = 0f - Maths.Clamp(Body.AngularVelocity * 10f, -65f, 65f);
        UpdateContactAngles();
        foreach (RotatorGrass item in grass)
        {
            item.UpdateAngle(time, angle);
        }
    }

    public void UpdateContactAngles()
    {
        foreach (RotatorGrass item in grass)
        {
            item.ContactAngle = 0f;
        }
        for (ContactEdge val = Body.ContactList; val != null; val = val.Next)
        {
            if (val.Contact.IsTouching && val.Other.UserData != null && !val.Contact.IsSensor())
            {
                Vector2 worldPoint = FarseerUtil.GetWorldPoint(val.Contact);
                AddContactAngle(VectorUtil.Atan2(Body.GetLocalPoint(worldPoint)).SimplifyAngle(0f));
            }
        }
    }

    public void AddContactAngleIndex(float angle, int index)
    {
        index = Maths.ModPositive(index, GrassCount());
        RotatorGrass rotatorGrass = grass[index];
        angle = angle.SimplifyAngle(rotatorGrass.InitialAngle - (float)Math.PI);
        int num = Math.Sign(rotatorGrass.InitialAngle - angle);
        rotatorGrass.ContactAngle = (trampleAngle - Math.Abs(angle - rotatorGrass.InitialAngle)) * 1.3f * num;
    }

    public void AddContactAngle(float angle)
    {
        for (int i = 0; i < 3; i++)
        {
            AddContactAngleIndex(angle, (int)((angle - (i * grassStep)) / grassStep));
            AddContactAngleIndex(angle, (int)((angle + ((i + 1) * grassStep)) / grassStep));
        }
    }

    public void CreateGrass()
    {
        for (int i = 0; i < GrassCount(); i++)
        {
            RotatorGrass rotatorGrass = new(grassSystem.Particles[i])
            {
                InitialAngle = grassSystem.GetItemAngle(i)
            };
            grass.Add(rotatorGrass);
        }
    }
}
