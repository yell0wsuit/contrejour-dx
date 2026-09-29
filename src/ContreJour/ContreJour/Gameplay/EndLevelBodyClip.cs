using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Sound;
using Mokus2D.Util;
using Mokus2D.Visual;

namespace ContreJour.Gameplay;

public class EndLevelBodyClip : RotatableBodyClip, IRestartable
{
    protected Portal Portal { get; set; }

    private bool finishing;

    public EndLevelBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        Portal = new Portal((ContreJourGame)Builder.Game, Vector2.Zero)
        {
            Position = Clip.Position,
            Scale = 1.3f
        };
        Builder.Add(Portal, 11);
        ((ContreJourGame)Builder.Game).EndLevel = this;
        Clip.Visible = false;
        Portal.ItemsScale = 0f;
    }

    public void ShowPortal()
    {
        Portal.ItemsScale = 0f;
        Portal.TargetScale = 1f;
    }

    public void SetVisible(bool value)
    {
        Portal.Visible = value;
    }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
        if (!finishing && body2.UserData is HeroBodyClip heroBodyClip && heroBodyClip.CanDie())
        {
            if (Builder.Game.TotalTime <= 5f)
            {
                XBoxUtil.AwardAchievement("rush_hour");
            }
            if (Builder.Game.TotalTime <= 10f && ((ContreJourGame)Builder.Game).StarsCollected == 3)
            {
                XBoxUtil.AwardAchievement("fast_perfect");
            }
            finishing = true;
            CompleteLevel(heroBodyClip);
        }
    }

    protected virtual void CompleteLevel(HeroBodyClip bodyClip)
    {
        bodyClip.CompleteLevelSpeed(Body.Position, 0.3f);
        bodyClip.SetScaleTime(0f, 0.5f);
        SoundManager.PlaySound("end", 0.5f);
        Portal.TargetScale = 0f;
    }

    public virtual void Restart()
    {
        finishing = false;
    }
}
