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
    protected List<RotatorGrass> grass = new List<RotatorGrass>();

    protected FurCircle grassSystem;

    protected float grassStep;

    protected float trampleAngle;

    protected Sprite baseSprite;

    public FurBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        if (_clip == null)
        {
            _clip = new Node();
            _clip.Scale = _config.GetVector("scale").X;
            _builder.AddChild(_clip);
            clip = _clip;
        }
        grassStep = (float)Math.PI * 2f / (float)GrassCount();
        trampleAngle = 4f * grassStep;
        baseSprite = new McRotatorBase();
        baseSprite.Scale = Width() / baseSprite.TextureSize.X;
        clip.AddChild(baseSprite);
        grassSystem = CreateFur();
        CreateGrass();
    }

    public FurCircle CreateFur()
    {
        FurCircle furCircle = new FurCircle(GrassTexture(), GrassCount(), GrassRadius());
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
        rotatorGrass.ContactAngle = (trampleAngle - Math.Abs(angle - rotatorGrass.InitialAngle)) * 1.3f * (float)num;
    }

    public void AddContactAngle(float angle)
    {
        for (int i = 0; i < 3; i++)
        {
            AddContactAngleIndex(angle, (int)((angle - (float)i * grassStep) / grassStep));
            AddContactAngleIndex(angle, (int)((angle + (float)(i + 1) * grassStep) / grassStep));
        }
    }

    public void CreateGrass()
    {
        for (int i = 0; i < GrassCount(); i++)
        {
            RotatorGrass rotatorGrass = new RotatorGrass(grassSystem.Particles[i]);
            rotatorGrass.InitialAngle = grassSystem.GetItemAngle(i);
            grass.Add(rotatorGrass);
        }
    }
}
