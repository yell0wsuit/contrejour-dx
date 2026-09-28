using ContreJour.Clips.planets;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class Chapter3(int _index, MainMenu _menu) : ChapterItem(_index, _menu)
{
    protected override void CreateSprites()
    {
        AddShesterna("planets/McPlanetShesterna", 5, new Vector2(-40f, 60f)).Color = new Color(100, 100, 100);
        blurBackground = new McChapter3Blur();
        background = new McPlanet3Background();
        container.AddChild(background);
        Sprite sprite = new McPlanet3Light();
        container.AddChild(sprite);
        _ = sprite.Tweener.RepeatSequenceForever(4f).FadeTo(20f / 51f).Next(5f)
            .FadeTo(0.7058824f);
        RotatingSprite rotatingSprite = AddShesterna("planets/McPlanetShesterna", 10, new Vector2(-27f, -5f));
        rotatingSprite.Scale = 1.8f;
        rotatingSprite.Color = new Color(100, 100, 100);
        _ = AddShesterna("planets/McPlanetShesternaBlur", 90, new Vector2(19f, -58f));
        _ = AddShesterna("planets/McPlanetShesterna2", -20, new Vector2(40f, 30f));
        _ = AddShesterna("planets/McPlanetShesterna3", -400, new Vector2(74f, 25f));
        rotatingSprite = AddShesterna("planets/McPlanetCross", -200, new Vector2(104f, 24f));
        alphaItems.Add(rotatingSprite);
        rotatingSprite = AddShesterna("menu/McMenuCircleSpikes", -400, new Vector2(50f, 69f));
        alphaItems.Add(rotatingSprite);
        rotatingSprite = AddShesterna("menu/McMenuCircleSpikes", -200, new Vector2(67f, 54f));
        alphaItems.Add(rotatingSprite);
        McPlanetStick mcPlanetStick = new()
        {
            Position = new Vector2(30f, 96f)
        };
        container.AddChild(mcPlanetStick);
        mcPlanetStick.RotationDegrees = -26f;
        mcPlanetStick.Speed = 1.3f;
        AddUpdating(mcPlanetStick.content);
        alphaItems.Add(mcPlanetStick);
        Sprite node = new McPlanet3Foreground();
        container.AddChild(node);
    }

    private RotatingSprite AddShesterna(string spriteName, int speed, Vector2 position)
    {
        RotatingSprite rotatingSprite = new(spriteName)
        {
            Speed = speed,
            Position = position
        };
        container.AddChild(rotatingSprite);
        AddUpdating(rotatingSprite);
        return rotatingSprite;
    }
}
