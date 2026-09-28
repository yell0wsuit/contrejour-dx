using System;

using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Data;

[Serializable]
public class AnimationFrameData
{
    public string Id;

    public Vector2 Position;

    public float Rotation;

    public Vector2 Scale;

    public float Alpha;

    public Color Color;

    public float ColorRatio;

    public bool Visible;
}
