using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Sound;
using Mokus2D.Util;
using Mokus2D.Visual;

namespace Default.Namespace;

public class EndLevelBodyClip : RotatableBodyClip, IRestartable
{
    protected Portal Portal { get; set; }

    private bool finishing;

    public EndLevelBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        Portal = new Portal((ContreJourGame)this.builder.Game, Vector2.Zero)
        {
            Position = this.clip.Position,
            Scale = 1.3f
        };
        this.builder.Add(Portal, 11);
        ((ContreJourGame)this.builder.Game).EndLevel = this;
        this.clip.Visible = false;
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
            if (builder.Game.TotalTime <= 5f)
            {
                XBoxUtil.AwardAchievement("rush_hour");
            }
            if (builder.Game.TotalTime <= 10f && ((ContreJourGame)builder.Game).StarsCollected == 3)
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
