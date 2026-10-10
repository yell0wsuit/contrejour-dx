using System.Numerics;

using ContreJourDX.Clips;
using ContreJourDX.Clips.planets;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class Chapter3(int index, MainMenu menu) : ChapterItem(index, menu)
    {
        protected override void CreateSprites()
        {
            AddShesterna("planets/McPlanetShesterna", 5, new Vector2(-40f, 60f)).Color = new Color(100, 100, 100);
            BlurBackground = new Sprite(ClipIds.Planets.McChapter3Blur);
            Background = new Sprite(ClipIds.Planets.McPlanet3Background);
            Container.AddChild(Background);
            Sprite sprite = new(ClipIds.Planets.McPlanet3Light);
            Container.AddChild(sprite);
            _ = sprite.Tweener.RepeatSequenceForever(4f).FadeTo(20f / 51f).Next(5f)
                .FadeTo(0.7058824f);
            RotatingSprite rotatingSprite = AddShesterna("planets/McPlanetShesterna", 10, new Vector2(-27f, -5f));
            rotatingSprite.Scale = 1.8f;
            rotatingSprite.Color = new Color(100, 100, 100);
            _ = AddShesterna("planets/McPlanetShesternaBlur", 90, new Vector2(19f, -58f));
            _ = AddShesterna("planets/McPlanetShesterna2", -20, new Vector2(40f, 30f));
            _ = AddShesterna("planets/McPlanetShesterna3", -400, new Vector2(74f, 25f));
            rotatingSprite = AddShesterna("planets/McPlanetCross", -200, new Vector2(104f, 24f));
            AlphaItems.Add(rotatingSprite);
            rotatingSprite = AddShesterna("menu/McMenuCircleSpikes", -400, new Vector2(50f, 69f));
            AlphaItems.Add(rotatingSprite);
            rotatingSprite = AddShesterna("menu/McMenuCircleSpikes", -200, new Vector2(67f, 54f));
            AlphaItems.Add(rotatingSprite);
            McPlanetStick mcPlanetStick = new()
            {
                Position = new Vector2(30f, 96f)
            };
            Container.AddChild(mcPlanetStick);
            mcPlanetStick.RotationDegrees = -26f;
            mcPlanetStick.Speed = 1.3f;
            AddUpdating(mcPlanetStick.content);
            AlphaItems.Add(mcPlanetStick);
            Sprite node = new(ClipIds.Planets.McPlanet3Foreground);
            Container.AddChild(node);
        }

        private RotatingSprite AddShesterna(string spriteName, int speed, Vector2 position)
        {
            RotatingSprite rotatingSprite = new(spriteName)
            {
                Speed = speed,
                Position = position
            };
            Container.AddChild(rotatingSprite);
            AddUpdating(rotatingSprite);
            return rotatingSprite;
        }
    }
}
