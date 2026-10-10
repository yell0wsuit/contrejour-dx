using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class Chapter6(int index, MainMenu menu) : ChapterItem(index, menu)
    {
        protected override void CreateSprites()
        {
            BlurBackground = new Sprite("newFriend/McChapterVBlur");
            RotatingSprite upper = new("newFriend/McPlanetVBackground1") { Speed = -float.RadiansToDegrees(1f), RotationRadians = -60f };
            RotatingSprite lower = new("newFriend/McPlanetVBackground2") { Speed = float.RadiansToDegrees(0.75f), RotationRadians = -30f };
            Container.AddChild(upper);
            Container.AddChild(lower);
            AddUpdating(upper);
            AddUpdating(lower);
            Background = new Sprite("newFriend/McPlanetVBackground");
            Container.AddChild(Background);
        }
    }
}
