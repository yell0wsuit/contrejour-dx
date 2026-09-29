using System;

using ContreJour.Content;

using ContreJourMono.ContreJour.Game.Eyes;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class MonsterEye : RandomAnimationEye, IPositionDepedent
{
    private Vector2 clipPosition;

    private float startAngle;

    protected IVectorPositionProvider positionProvider;
    public static readonly EyeAnimation[] SnotAnimations =
    [
        new("McEyeBlinkMonster"),
        new("McEyeBlinkOneTimeMonster")
    ];

    public IVectorPositionProvider PositionProvider
    {
        get => positionProvider;
        set => positionProvider = value;
    }

    public IVectorPositionProvider RandomPositionProvider { get; set; }

    public bool ProviderEnabled { get; set; }

    protected override EyeAnimation[] Animations => SnotAnimations;

    public bool HasToUpdate => Visible;

    public bool Open
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                if (value && !Visible)
                {
                    AnimationEndEvent.RemoveListener(OnCloseEnd);
                    PlayAnimation(new EyeAnimation("McEyeOpenMonster"), force: true);
                    Visible = true;
                }
                else if (!value && Visible)
                {
                    PlayAnimation(new EyeAnimation("McEyeCloseMonster"), force: true);
                    AnimationEndEvent.AddListener(OnCloseEnd);
                }
            }
        }
    }

    public MonsterEye(ContreJourGame game, bool visible, Vector2 position)
        : base(game)
    {
        ProviderEnabled = true;
        Open = visible;
        Visible = visible;
        _ = this.Schedule(0.1f, ChangePositionProvider);
        Game?.AddPositionDependent(this);
        clipPosition = position;
        startAngle = 0f;
    }

    public void RefreshRootAngle()
    {
        startAngle = this.GetRootRotationRadians();
    }

    public void ProviderAdded(IVectorPositionProvider provider)
    {
        if (Maths.Random() < 0.5f)
        {
            ChangePositionProvider();
        }
    }

    public void ProviderRemove(IVectorPositionProvider provider)
    {
        if (RandomPositionProvider == provider)
        {
            ChangePositionProvider();
        }
    }

    protected override void CreateDefaultView()
    {
        string name = BlackEye ? "McEyeMonsterBlack" : "McEyeMonster";
        string name2 = Game.ChooseSide("McEyeBallMonsterBlack", "McEyeBallMonsterWhite", "McEyeBallMonster", "McEyeBallMonster", "McEyeBallMonster_6");
        Background = (Sprite)ClipTypesCache.CreateNewNode(name);
        eyeBall = (Sprite)ClipTypesCache.CreateNewNode(name2);
    }

    public override void PlayAnimation(EyeAnimation animation, bool force)
    {
        base.PlayAnimation(animation, force);
    }

    protected virtual void ChangePositionProvider()
    {
        if (Game != null)
        {
            RandomPositionProvider = Game.GetRandomPositionProvider();
        }
        CallLater(ScheduleChangePositionProvider);
    }

    private void ScheduleChangePositionProvider(Node node)
    {
        Tweener.Stop(100);
        _ = this.Schedule(Maths.Random(3f, 15f), ChangePositionProvider, 100);
    }

    public override void Update(float time)
    {
        if (ProviderEnabled)
        {
            IVectorPositionProvider vectorPositionProvider = positionProvider ?? RandomPositionProvider;
            if (vectorPositionProvider != null)
            {
                _ = Vector2.Zero;
                if (Game != null)
                {
                    Vector2 zero = Game.Builder.ToRootChild(Vector2.Zero, this);
                    clipPosition = Game.Builder.ToIPhoneVec(zero);
                }
                Vector2 vector = vectorPositionProvider.PositionVec - clipPosition;
                ViewAngle = Maths.Atan2(vector.Y, vector.X) - startAngle;
                ViewDistance = Math.Min(vector.Length() / 6.6666665f, 1f);
            }
        }
        base.Update(time);
        currentBackground.Position = CurrentEyeBall.Position * 0.5f;
    }

    private void OnCloseEnd()
    {
        AnimationEndEvent.RemoveListener(OnCloseEnd);
        Visible = false;
        if (Open)
        {
            Open = false;
            Open = true;
        }
    }
}
