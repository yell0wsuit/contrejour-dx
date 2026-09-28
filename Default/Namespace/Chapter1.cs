using System;

using ContreJour.Clips.menu;
using ContreJour.Clips.planets;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class Chapter1 : ChapterItem
{
    protected Sprite foreground;

    protected PlanetEye eye;

    public override float Depth
    {
        set
        {
            base.Depth = value;
            eye.AnimationsAllowed = Maths.FuzzyEquals(value, 1f);
        }
    }

    public Chapter1(int _index, MainMenu _menu)
        : base(_index, _menu)
    {
    }

    protected override void CreateSprites()
    {
        CreateSnotRotationScale(new Vector2(70f, 60f), -30f, 8f / 15f);
        CreateSnotRotationScale(new Vector2(83f, 44f), -80f, 0.4f);
        CreateSnotRotationScale(new Vector2(74f, -59f), -130f, 4f / 15f);
        CreateSnotRotationScale(new Vector2(54f, -72f), -180f, 1f / 3f);
        CreateSnotRotationScale(new Vector2(-83f, -35f), 90f, 1f / 3f);
        background = new McPlanet1Background();
        foreground = new McPlanet1Foreground();
        blurBackground = new McChapter1Blur();
        blurBackground.Scale = 1.1f;
        container.AddChild(background);
        Sprite sprite = new McPlanetRoseLight();
        container.AddChild(sprite);
        sprite.Tweener.RepeatSequenceForever(3f).FadeTo(10f / 51f).Next(3f)
            .FadeTo(31f / 51f);
        Sprite node = new McRoseForeground();
        container.AddChild(node);
        eye = new PlanetEye(null, _visible: true, Vector2.Zero);
        eye.Scale = 0.9f;
        eye.Position = new Vector2(10f, -10f);
        container.AddChild(eye);
        AddSpikes(new Vector2(-92f, 8f), 1f, -200f, 4f, 0f);
        AddSpikes(new Vector2(-90f, 23f), 0.85f, -300f, 3f, (float)Math.PI / 3f);
        AddSpikes(new Vector2(-88f, 37f), 0.65f, -400f, 2f, (float)Math.PI * 2f / 3f);
        container.AddChild(foreground);
    }

    private void AddSpikes(Vector2 position, float scale, float speed, float amplitude, float progress)
    {
        MovingRotatingSprite movingRotatingSprite = new MovingRotatingSprite("menu/McMenuCircleSpikes");
        movingRotatingSprite.Position = position;
        movingRotatingSprite.Scale = scale;
        movingRotatingSprite.Speed = speed;
        movingRotatingSprite.Initialize(amplitude, progress);
        container.AddChild(movingRotatingSprite);
    }

    public void CreateSnotRotationScale(Vector2 position, float rotation, float scale)
    {
        PlanetSnotContainer planetSnotContainer = new PlanetSnotContainer();
        planetSnotContainer.Position = position;
        planetSnotContainer.RotationDegrees = rotation;
        planetSnotContainer.Scale = scale;
        container.AddChild(planetSnotContainer);
        depthDependent.Add(planetSnotContainer);
    }
}
