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
    private const int PositionProviderTag = 100;

    private const float MAX_DISTANCE = 6.6666665f;

    private const float PROVIDER_MAX_TIME = 15f;

    private const float PROVIDER_MIN_TIME = 3f;

    protected Vector2 clipPosition;

    protected float startAngle;

    protected IVectorPositionProvider positionProvider;

    protected IVectorPositionProvider randomPositionProvider;

    protected bool providerEnabled;

    private bool open;

    public static readonly EyeAnimation[] SNOT_ANIMATIONS =
    [
        new("McEyeBlinkMonster"),
        new("McEyeBlinkOneTimeMonster")
    ];

    public IVectorPositionProvider PositionProvider
    {
        get => positionProvider;
        set => positionProvider = value;
    }

    public IVectorPositionProvider RandomPositionProvider
    {
        get => randomPositionProvider;
        set => randomPositionProvider = value;
    }

    public bool ProviderEnabled
    {
        get => providerEnabled;
        set => providerEnabled = value;
    }

    protected override EyeAnimation[] Animations => SNOT_ANIMATIONS;

    public bool HasToUpdate => Visible;

    public bool Open
    {
        get => open;
        set
        {
            if (open != value)
            {
                open = value;
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

    public MonsterEye(ContreJourGame _game, bool _visible, Vector2 position)
        : base(_game)
    {
        providerEnabled = true;
        Open = _visible;
        Visible = _visible;
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

    public void ProviderRemove(IVectorPositionProvider _provider)
    {
        if (randomPositionProvider == _provider)
        {
            ChangePositionProvider();
        }
    }

    protected override void CreateDefaultView()
    {
        string name = BlackEye ? "McEyeMonsterBlack" : "McEyeMonster";
        string name2 = Game.ChooseSide("McEyeBallMonsterBlack", "McEyeBallMonsterWhite", "McEyeBallMonster", "McEyeBallMonster", "McEyeBallMonster_6");
        background = (Sprite)ClipTypesCache.CreateNewNode(name);
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
            randomPositionProvider = Game.GetRandomPositionProvider();
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
        if (providerEnabled)
        {
            IVectorPositionProvider vectorPositionProvider = positionProvider ?? randomPositionProvider;
            if (vectorPositionProvider != null)
            {
                Vector2 zero = Vector2.Zero;
                if (Game != null)
                {
                    zero = Game.Builder.ToRootChild(Vector2.Zero, this);
                    clipPosition = Game.Builder.ToIPhoneVec(zero);
                }
                Vector2 vector = vectorPositionProvider.PositionVec - clipPosition;
                ViewAngle = Maths.Atan2(vector.Y, vector.X) - startAngle;
                ViewDistance = Math.Min(vector.Length() / 6.6666665f, 1f);
            }
        }
        base.Update(time);
        currentBackground.Position = currentEyeBall.Position * 0.5f;
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
