using System;

using FarseerPhysics.Dynamics;

namespace FarseerPhysics;

public static class Settings
{
    public const float MaxFloat = float.MaxValue;

    public const float Epsilon = 1.1920929E-07f;

    public const float Pi = (float)Math.PI;

    public const bool AllCollisionCallbacksAgree = true;

    public const bool EnableDiagnostics = true;

    public const bool SkipSanityChecks = false;

    public const int MaxSubSteps = 8;

    public const bool EnableWarmstarting = true;

    public const int MaxManifoldPoints = 2;

    public const float AABBExtension = 0.1f;

    public const float AABBMultiplier = 2f;

    public const float LinearSlop = 0.005f;

    public const float AngularSlop = (float)Math.PI / 90f;

    public const float PolygonRadius = 0.01f;

    public const int MaxTOIContacts = 32;

    public const float VelocityThreshold = 1f;

    public const float MaxLinearCorrection = 0.2f;

    public const float MaxAngularCorrection = (float)Math.PI * 2f / 45f;

    public const float Baumgarte = 0.2f;

    public const float TimeToSleep = 0.5f;

    public const float LinearSleepTolerance = 0.01f;

    public const float AngularSleepTolerance = (float)Math.PI / 90f;

    public const float MaxTranslation = 2f;

    public const float MaxTranslationSquared = 4f;

    public const float MaxRotation = (float)Math.PI / 2f;

    public const float MaxRotationSquared = 2.4674013f;

    public const int MaxGJKIterations = 20;

    public const bool EnableSubStepping = false;

    public const bool AutoClearForces = true;

    public static int VelocityIterations { get; set; } = 8;

    public static int PositionIterations { get; set; } = 3;

    public static bool ContinuousPhysics { get; set; } = true;

    public static readonly bool UseConvexHullPolygons = true;

    public static readonly int TOIVelocityIterations = VelocityIterations;

    public static readonly int TOIPositionIterations = 20;

    public static readonly bool AllowSleep = true;

    public static readonly int MaxPolygonVertices = 8;

    public static readonly bool UseFPECollisionCategories;

    public static readonly Category DefaultFixtureCollisionCategories = Category.Cat1;

    public static readonly Category DefaultFixtureCollidesWith = Category.All;

    public static readonly Category DefaultFixtureIgnoreCCDWith = Category.None;

    public static float MixFriction(float friction1, float friction2)
    {
        return (float)Math.Sqrt(friction1 * friction2);
    }

    public static float MixRestitution(float restitution1, float restitution2)
    {
        return !(restitution1 > restitution2) ? restitution2 : restitution1;
    }
}
