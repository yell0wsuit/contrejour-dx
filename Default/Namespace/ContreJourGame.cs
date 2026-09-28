using System;
using System.Collections.Generic;
using System.Linq;

using ContreJour;
using ContreJour.Config;
using ContreJour.Content;

using Default.Namespace.Interfaces;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Data;
using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Integration.Farseer.Util;
using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Util.Schedule;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class ContreJourGame : GameBase, IDisposable, ITouchListener, IActivatedDependent, IWindManager
{
    public class ClickableComparer : IComparer<BodyClip>
    {
        private readonly Vector2 sourcePoint;

        public ClickableComparer(Vector2 sourcePoint)
        {
            this.sourcePoint = sourcePoint;
        }

        public int Compare(BodyClip clip1, BodyClip clip2)
        {
            IClickable clickable = (IClickable)clip1;
            IClickable clickable2 = (IClickable)clip2;
            if (clickable2.Priority(sourcePoint) > clickable.Priority(sourcePoint))
            {
                return 1;
            }
            if (clickable2.Priority(sourcePoint) < clickable.Priority(sourcePoint))
            {
                return -1;
            }
            float num = clickable.TouchDistance(sourcePoint);
            float num2 = clickable2.TouchDistance(sourcePoint);
            return Maths.FuzzyEquals(num, num2) ? 0 : !(num < num2) ? 1 : -1;
        }
    }

    public const float RESTART_TIME = 1.5f;

    public const float WIND_STEP_WHITE = 0.02f;

    public const float WIND_STEP = 0.03f;

    private const float STRIPES_HEIGHT = 30f;

    public const float ZOOM_SCALE = 1.3f;

    public const float CLICK_RADIUS_IPHONE = 1.5f;

    public const float CLICK_RADIUS = 1.1666666f;

    public readonly List<SnotPoint> SnotPoints = new(64);

    public readonly EventSender RestartEvent = new();

    private readonly PausePanel pausePanel;

    protected Node alphaBackground;

    protected EventSender backEvent;

    protected List<BackgroundBase> backgrounds = [];

    protected bool blackSide;

    protected IBonusAcceptable bonusTarget;

    protected Color buttonsColor;

    protected int chapter;

    protected ClickableLayer clickableLayer;

    protected Dictionary<Touch, IClickable> draggingItems;

    protected ParticleSystem dust;

    protected EndLevelBodyClip endLevel;

    protected ParticleSystem energy;

    protected FinishView finishView;

    protected bool finished;

    protected float flyOpacity;

    protected ParticleSystem flyes;

    protected List<ForegroundBase> foregrounds = [];

    protected int frame;

    protected List<Touch> freeDisabledTouches;

    protected List<Touch> freeTouches;

    protected ParticleSystem grass;

    protected Button pauseButton;

    protected GroundFall groundFall;

    private HeroBodyClip hero;

    protected int levelIndex;

    protected int levelPosition;

    protected LightColor lightColor;

    protected Vector2 lightPoint;

    protected float lightPower;

    protected bool lightPowerChanged;

    protected EventSender nextLevelEvent;

    protected GravityParticleSystem particles;

    protected List<PlasticineBodyClip> plasticine = new(8);

    protected List<object> positionDependent;

    protected List<object> positionProviders;

    protected float providersValue;

    protected bool restartEnabled;

    protected LayerColor restartLayer;

    protected bool snotSend;

    protected int starsCollected;

    protected LightColor startLightColor;

    protected Hashtable teleports;

    protected List<string> texturesToUnload;

    protected bool touchEnabled;

    protected Vector2 touchFixPoint;

    protected bool whiteSide;

    protected WindManager windManager;

    protected int zoomOutCount;

    protected float zoomOutTime;

    public static readonly int[] MIN_ZOOM_LEVELS =
    [
        51, 53, 52, 54, 49, 37, 74, 79, 80, 76,
        55, 77, 86, 87, 83, 94, 84, 91, 95, 85,
        92, 93, 89
    ];

    public static readonly int[] LOW_FPS_LEVELS = [4, 6, 44, 53, 54, 12];

    private readonly List<object> _toRemove = [];

    private readonly Scheduler _scheduler = new();

    public RectangleFloat LevelScreenPhysicsBounds { get; private set; }

    public RectangleFloat LevelScreenBounds { get; private set; }

    public override bool Paused
    {
        set
        {
            if (Paused != value)
            {
                clickableLayer.InteractionsEnabled = !value;
                GameRoot.UpdateEnabled = !value;
                paused = value;
                if (!value && ContreJourConfig.BackButtonVisible)
                {
                    pauseButton.Enabled = true;
                }
                particles?.Paused = value;
            }
        }
    }

    public int Frame
    {
        get => frame;
        set => frame = value;
    }

    public WindManager WindManager => windManager;

    public Node AlphaBackground => alphaBackground;

    public bool BlackSide => blackSide;

    public bool WhiteSide => whiteSide;

    public int Chapter => chapter;

    public EventSender BackEvent => backEvent;

    public EventSender NextLevelEvent => nextLevelEvent;

    public HeroBodyClip Hero => hero;

    public Vector2 HeroPositionVec => hero.Body.Position;

    public float FlyOpacity
    {
        get => flyOpacity;
        set
        {
            if (Maths.FuzzyNotEquals(flyOpacity, value))
            {
                flyOpacity = value;
                flyes.OpacityByte = (int)value;
                flyes.Visible = value > 0f;
            }
        }
    }

    public List<PlasticineBodyClip> Plasticine => plasticine;

    public Vector2 LightPoint
    {
        get => lightPoint;
        set => lightPoint = value;
    }

    public bool LightPowerChanged => lightPowerChanged;

    public LightColor LightColor => lightColor;

    public ParticleSystem Flyes => flyes;

    public ParticleSystem Dust => dust;

    public ParticleSystem Grass => grass;

    public ParticleSystem Energy => energy;

    public GroundFall GroundFall => groundFall;

    public EndLevelBodyClip EndLevel
    {
        get => endLevel;
        set => endLevel = value;
    }

    public bool TouchEnabled
    {
        get => touchEnabled;
        set => touchEnabled = value;
    }

    public int LevelIndex => levelIndex;

    public int LevelPosition => levelPosition;

    public IBonusAcceptable BonusTarget
    {
        get => bonusTarget ?? hero;
        set => bonusTarget = value;
    }

    public bool CanShowIntro { get; set; }

    public int StarsCollected => starsCollected;

    public bool SnotSend
    {
        get => snotSend;
        set => snotSend = value;
    }

    public ClickableLayer ClickableLayer => clickableLayer;

    public Color ButtonsColor => buttonsColor;

    public bool RestartEnabled
    {
        get => restartEnabled;
        set => restartEnabled = value;
    }

    public bool Finished
    {
        get => finished;
        set => finished = value;
    }

    public new ContreJourLevelBuilder Builder => (ContreJourLevelBuilder)base.Builder;

    public bool RoseChapter => chapter == 4;

    public bool BonusChapter => chapter == 5;

    public int HeroIndex => Builder.GameRoot.Children.IndexOf(hero.Clip);

    public Vector2 HeroPositionPixels => hero.Clip.Position;

    public float LightPower
    {
        get => lightPower;
        set
        {
            if (Maths.FuzzyNotEquals(lightPower, value))
            {
                lightPower = value;
                lightPowerChanged = true;
                RefreshLightColor();
            }
        }
    }

    public ContreJourGame(int _chapter)
    {
        freeDisabledTouches = [];
        chapter = _chapter;
        blackSide = chapter == 1;
        whiteSide = chapter == 3;
        touchEnabled = true;
        Vector2 w7FromIPhoneSize = ScreenConstants.W7FromIPhoneSize;
        Vector2 vector = new(w7FromIPhoneSize.X, w7FromIPhoneSize.Y);
        Vector2 point = blackSide ? new Vector2(w7FromIPhoneSize.X / 2f, w7FromIPhoneSize.Y * 2f) : vector;
        lightPoint = Box2DConfig.DefaultConfig.ToVec(point);
        lightPower = 1f;
        lightColor = ChooseSide(PlasticineConstants.BLUE, PlasticineConstants.BLACK_LIGHT, PlasticineConstants.LAST_LIGHT, PlasticineConstants.WHITE, PlasticineConstants.Green);
        startLightColor = lightColor;
        flyOpacity = 255f;
        Mokus2DGame.Instance.TouchController.AddListener(this);
        draggingItems = [];
        providersValue = 0f;
        positionProviders = [];
        positionDependent = [];
        windManager = new WindManager(whiteSide ? 0.02f : 0.03f);
        alphaBackground = new Node();
        gameRoot.AddChild(alphaBackground, -10);
        freeTouches = [];
        backEvent = new EventSender();
        nextLevelEvent = new EventSender();
        clickableLayer = new ClickableLayer();
        AddChild(clickableLayer, 15);
        restartLayer = new LayerColor(Color.Black, "menu/whitePixel");
        AddChild(restartLayer, 100);
        restartLayer.Visible = false;
        Color color = ColorUtil.Mult(ContreJourConstants.BLUE_LIGHT_COLOR, 2f);
        buttonsColor = blackSide ? color : ContreJourConstants.GREY_COLOR;
        _ = ScreenConstants.W7FromIPhoneSize;
        if (ContreJourConfig.BackButtonVisible)
        {
            pauseButton = new Button("menu/McPauseIcon")
            {
                RealScale = 1.3f
            };
            pauseButton.TouchEndEvent += delegate
            {
                OnBackPress();
            };
            pauseButton.Position = ContreJourConfig.BackButtonPosition;
            pauseButton.Color = buttonsColor;
            clickableLayer.AddChild(pauseButton);
        }
        pausePanel = new PausePanel(this);
        AddChild(pausePanel, 15);
        texturesToUnload = [];
        finishView = new FinishView(this);
        starsCollected = 0;
        teleports = [];
        _ = GameRoot.Schedule(1.5f, EnableRestart);
        Mokus2DGame.Instance.KeysController.AddBackKeyListener(OnBackPress);
    }

    public override LevelBuilderBase CreateLevelBuilder()
    {
        ContreJourLevelBuilder contreJourLevelBuilder = new(this)
        {
            NamespacePrefix = "Default.Namespace."
        };
        return contreJourLevelBuilder;
    }

    public void LoadLevelIndex(int index)
    {
        Maths.Randomize(levelIndex);
        levelIndex = index;
        pausePanel.SetLevelIndex(index);
        LoadLevel($"level{index}iPhone");
    }

    public override void LoadLevel(string levelName)
    {
        base.LoadLevel(levelName);
        Builder.PhysicsSpeed = 1.2f;
    }

    public override void ProcessLevel(Level level)
    {
        if (!blackSide)
        {
            CreateFlyes();
            CreateGrass();
        }
        CreateEnergy();
        base.ProcessLevel(level);
        Hashtable levelProperties = level.levelProperties;
        levelSize = new Vector2(levelProperties.GetFloat("Width"), levelProperties.GetFloat("Height"));
        GameRoot.Scale = ContreJourConfig.RootSize.X / levelSize.X;
        float num = GameRoot.Scale * levelSize.Y;
        GameRoot.Y = ContreJourConfig.RootSize.Y - num;
        Vector2 point = levelSize.AddY(GameRoot.Y / GameRoot.Scale);
        LevelScreenPhysicsBounds = new RectangleFloat(Builder.ToVec(new Vector2(0f, (0f - GameRoot.Y) / GameRoot.Scale)), Builder.ToVec(point));
        LevelScreenBounds = LevelScreenPhysicsBounds * (1f / builder.SizeMult);
        alphaBackground.Scale = Math.Max(levelSize.X / ScreenConstants.OsSizes.IPhoneRetina.X, (LevelSize.Y + (GameRoot.Y / GameRoot.Scale)) / ScreenConstants.OsSizes.IPhoneRetina.Y);
        alphaBackground.Y = (0f - GameRoot.Y) / GameRoot.Scale;
        Builder.Add(energy, 9);
        CreateBackgrounds(level);
        CreateParticles();
        CreateDust();
        CreateGroundFall();
        if (!blackSide)
        {
            Builder.Add(flyes, 6);
            Builder.Add(grass, -1);
        }
    }

    private void CreateBackgrounds(Level level)
    {
        foreach (Hashtable array in level.LevelProperties.GetArrayList("backgrounds").Cast<Hashtable>())
        {
            ProcessBackgroundItem(array);
        }
    }

    public override void OnLoadLevelLevel(string levelName, Level level)
    {
        FarseerUtil.CreateLevelBorders(Builder.GroundBody, PhysicsLevelSize, new FarseerUtil.Borders(left: true, top: true, right: true, bottom: false));
        PlasticineConstants.ApplyStaticBodiesFilter(Builder.GroundBody);
    }

    public void OnGameActivated()
    {
        pausePanel.RefreshSoundButtons();
    }

    protected override void Dispose(bool disposing)
    {
        Mokus2DGame.Instance.TouchController.RemoveListener(this);
        Mokus2DGame.Instance.KeysController.RemoveBackKeyListener(OnBackPress);
        pausePanel.Dispose();
        finishView.Dispose();
        base.Dispose(disposing);
    }

    public bool TouchBegin(Touch touch)
    {
        if (!touchEnabled || paused)
        {
            return false;
        }
        AddPositionProvider(new PositionProviderValue(new TouchPositionProvider(touch, Builder), 5f));
        if (!ProcessTouchIsFree(touch, isFree: false))
        {
            freeTouches.Add(touch);
        }
        return true;
    }

    public bool TouchMove(Touch touch)
    {
        if (freeTouches.Contains(touch))
        {
            UpdateFreeTouch(touch);
        }
        if (draggingItems.TryGetValue(touch, out IClickable dragged))
        {
            _ = dragged.TouchMove(touch);
        }
        return true;
    }

    public void TouchEnd(Touch touch)
    {
        if (freeTouches.Exists(touch))
        {
            _ = freeTouches.Remove(touch);
        }
        _ = freeDisabledTouches.Remove(touch);
        if (draggingItems.Remove(touch, out IClickable clickable))
        {
            clickable.TouchEnd(touch);
        }
        foreach (PositionProviderValue positionProvider in positionProviders.Cast<PositionProviderValue>())
        {
            if (positionProvider.Provider is not TouchPositionProvider || (positionProvider.Provider as TouchPositionProvider).Touch != touch)
            {
                continue;
            }
            RemovePositionProvider(positionProvider);
            {
                foreach (IPositionDepedent item in positionDependent.Cast<IPositionDepedent>())
                {
                    item.ProviderRemove(positionProvider.Provider);
                }
                break;
            }
        }
    }

    public List<string> TexturesToUnload()
    {
        return texturesToUnload;
    }

    public void Schedule(Action action, float seconds)
    {
        _scheduler.Schedule(action, seconds);
    }

    public void UnSchedule(Action action)
    {
        _scheduler.Cancel(action);
    }

    private void SetFinished(bool value)
    {
        finished = value;
        RestartEnabled = !finished;
    }

    public T Choose<T>(T normal = null, T blue = null, T white = null, T last = null, T green = null) where T : class
    {
        return BonusChapter
            ? green ?? normal
            : RoseChapter ? last ?? normal : WhiteSide ? white ?? normal : BlackSide ? blue ?? normal : normal;
    }

    public T ChooseSide<T>(T black, T white, T last, T normal, T green)
    {
        return BonusChapter ? green : ChooseSide(black, white, last, normal);
    }

    public T ChooseSide<T>(T black, T white, T last, T normal)
    {
        return RoseChapter ? last : ChooseSide(black, white, normal);
    }

    public T ChooseSide<T>(T black, T white, T normal)
    {
        return blackSide ? black : whiteSide ? white : normal;
    }

    public static void AddShadowSource(BodyClip source)
    {
    }

    public static void AddColorOverlay()
    {
    }

    public static int GetRandomColor()
    {
        return !(Maths.Random() < 0.3f) ? 0 : 255;
    }

    public void CollectStar()
    {
        starsCollected++;
    }

    public void HardRestart()
    {
        hero?.Removed = true;
        RestartEvent.SendEvent();
        DisableEvents();
    }

    public void SoftRestart()
    {
        totalTime = 0f;
        starsCollected = 0;
        foreach (Body body in Builder.World.BodyList)
        {
            if (body.UserData is IRestartable restartable)
            {
                restartable.Restart();
            }
        }
        foreach (IRemovable updatable in updatables)
        {
            if (updatable is IRestartable)
            {
                (updatable as IRestartable).Restart();
            }
        }
    }

    public void SetRestartEnabled(bool value)
    {
        if (value != restartEnabled)
        {
            restartEnabled = value;
        }
    }

    public void Restart()
    {
        if (levelIndex is 0 or 66)
        {
            HardRestart();
        }
        else if (restartEnabled)
        {
            RestartEnabled = false;
            restartLayer.Visible = true;
            restartLayer.OpacityByte = 0;
            _ = restartLayer.Tweener.StartSequence(0.3f).FadeTo(0.5882353f, Cubic.EaseIn).Next(0.1f)
                .Next(0.6f)
                .FadeOut(Cubic.EaseOut)
                .AndHide();
            SoftRestart();
            _ = GameRoot.Schedule(1.5f, EnableRestart);
        }
    }

    private void EnableRestart()
    {
        if (!finished)
        {
            RestartEnabled = true;
        }
    }

    public void Back()
    {
        hero?.Removed = true;
        backEvent.SendEvent();
        DisableEvents();
    }

    public void Skip()
    {
        hero?.Removed = true;
        LevelPosition position = LevelsMenu.GetLevelPosition(levelIndex);
        UserData.Instance.SkipLevel(position);
        nextLevelEvent.SendEvent();
        DisableEvents();
    }

    public void DisableEvents()
    {
        RestartEvent.Enabled = false;
        backEvent.Enabled = false;
        nextLevelEvent.Enabled = false;
    }

    public void AddTextureToUnload(string name)
    {
        if (!texturesToUnload.Contains(name))
        {
            texturesToUnload.Add(name);
        }
    }

    public void AddForeground(ForegroundBase foreground)
    {
        foregrounds.Add(foreground);
    }

    private void RefreshLightColor()
    {
        lightColor = startLightColor.Clone();
        lightColor.LightOutColor.R = (byte)(lightColor.LightOutColor.R * lightPower);
        lightColor.LightOutColor.G = (byte)(lightColor.LightOutColor.G * lightPower);
        lightColor.LightOutColor.B = (byte)(lightColor.LightOutColor.B * lightPower);
    }

    public void RegisterPlasticine(PlasticineBodyClip item)
    {
        plasticine.Add(item);
    }

    private void OnRestartClick(Button item)
    {
        Restart();
    }

    public TeleportBodyClip GetTeleport(string color)
    {
        return teleports.ContainsKey(color) ? (TeleportBodyClip)teleports.GetObject(color) : null;
    }

    public void RegisterTeleportColor(TeleportBodyClip teleport, string color)
    {
        teleports[color] = teleport;
    }

    public void OnMenuPressed()
    {
        if (finished)
        {
            return;
        }
        if (pausePanel.Visible)
        {
            pausePanel.Hide();
            Paused = false;
            return;
        }
        if (ContreJourConfig.BackButtonVisible)
        {
            pauseButton.Enabled = false;
        }
        pausePanel.Show();
        Paused = true;
    }

    public void OnBackPress()
    {
        if (finished || pausePanel.Visible)
        {
            Back();
            Mokus2DGame.Instance.KeysController.RemoveBackKeyListener(OnBackPress);
            return;
        }
        if (ContreJourConfig.BackButtonVisible)
        {
            pauseButton.Enabled = false;
        }
        pausePanel.Show();
        Paused = true;
    }

    private void CreateFlyes()
    {
        flyes = new ParticleSystem(Mokus2DGame.LoadSpriteData("common/McFly"));
        if (BonusChapter)
        {
            flyes.Color = ContreJourConstants.GreenLightColor;
        }
    }

    private void CreateGroundFall()
    {
        groundFall = new GroundFall(this);
        Builder.Add(groundFall, -1);
    }

    public void ProcessBackgroundItem(Hashtable background)
    {
        string text = background.GetString("type");
        texturesToUnload.Add(text);
        Node node = ClipTypesCache.CreateNewNode(text);
        Hashtable hashtable = background.GetHashtable("config");
        Vector2 vector = background.GetVector("position");
        Vector2 vector2 = background.GetVector("scale");
        vector.Y = ScreenConstants.OsSizes.IPhoneRetina.Y + vector.Y - levelSize.Y;
        node.Position = vector;
        float num = background.GetFloat("initialScale", 1f);
        node.ScaleX = vector2.X / num;
        node.ScaleY = vector2.Y / num;
        node.RotationDegrees = 0f - background.GetFloat("rotation");
        if (chapter == 5)
        {
            node.RotationDegrees = 0f;
            node.Scale = ScreenConstants.W7FromIPhoneSize.X / ((Sprite)node).TextureSize.X;
        }
        AddBackgroundColorConfig(node, hashtable);
        if (hashtable.Exists("foreground"))
        {
            Builder.AddForeground(node);
        }
        else if (blackSide)
        {
            int z = hashtable.GetInt("z", -10);
            Builder.AddAlphaBackgroundZ(node, z);
        }
        else
        {
            Builder.AddAlphaBackground(node);
        }
        if (hashtable.Exists("type"))
        {
            BackgroundBase item = (BackgroundBase)ReflectUtil.CreateInstance(typeName: "Default.Namespace." + hashtable.GetString("type"), mainAssemblyClass: typeof(ContreJourApplication), parameters: [node, hashtable, this]);
            backgrounds.Add(item);
        }
    }

    public static void AddBackgroundColorConfig(Node background, Hashtable config)
    {
        if (config.ContainsKey("color"))
        {
            background.Color = config.GetString("color").ToColor();
        }
    }

    public void CreateGrass()
    {
        grass = new ParticleSystem(Mokus2DGame.LoadMovieClipData(ChooseSide(null, "chapter4/McWhiteGrass", "chapter5/McGrass_5", "common/McTotalGrass", "McGrass_6")));
    }

    public void CreateDust()
    {
        dust = new ParticleSystem(whiteSide ? "chapter4/McDustWhite" : "common/McDust");
        Builder.Add(dust, 1);
    }

    public void CreateEnergy()
    {
        energy = new ParticleSystem("common/McEnergyBall");
    }

    public void IncreaseZoomOut()
    {
        if (zoomOutCount == 0)
        {
            zoomOutTime = 0f;
        }
        zoomOutCount++;
    }

    public override void DecreaseZoomOut()
    {
        zoomOutCount--;
        if (zoomOutCount == 0)
        {
            zoomOutTime = 0f;
        }
    }

    public void CreateParticles()
    {
        if (!BonusChapter)
        {
            if (whiteSide)
            {
                particles = new WhiteSnow();
                Builder.Add(particles, -1);
                particles.CreateBetweenBounds(40);
            }
            else if (chapter == 2)
            {
                particles = new SnowFall();
                Builder.Add(particles, 11);
                particles.CreateBetweenBounds(40);
            }
            else if (blackSide)
            {
                particles = new BlueLights();
                Builder.AddAlphaBackgroundZ(particles, -9);
                particles.CreateBetweenBounds(20);
            }
            else if (RoseChapter)
            {
                particles = new LastParticles();
                Builder.Add(particles, 11);
                particles.CreateBetweenBounds(40);
            }
            else
            {
                particles = new BlackFall();
                Builder.Add(particles, -2);
                particles.CreateBetweenBounds(40);
            }
        }
    }

    public void HideBonuses()
    {
        endLevel.SetVisible(value: false);
        energy.Visible = false;
        energy.OpacityByte = 0;
    }

    public static void FocusOnHero()
    {
    }

    public void ShowBonuses()
    {
        endLevel.SetVisible(value: true);
        endLevel.ShowPortal();
        energy.Visible = true;
        _ = energy.FadeIn(2f);
    }

    public void Fail(float restartTime)
    {
        if (Maths.FuzzyEquals(restartTime, 0f))
        {
            Restart();
        }
        else
        {
            _ = GameRoot.Schedule(restartTime, Restart);
        }
    }

    public void Finish(Vector2 zoomPoint)
    {
        RestartEnabled = false;
        LevelPosition levelPosition = LevelsMenu.GetLevelPosition(levelIndex);
        LevelData levelDataByPosition = UserData.Instance.GetLevelDataByPosition(levelPosition);
        int num = UserData.Instance.CompleteLevel(levelPosition, starsCollected, totalTime);
        bool newHighScore = levelDataByPosition != null && (levelDataByPosition.Score < num || levelDataByPosition.StarsCount < starsCollected);
        pauseButton.InteractionsEnabled = false;
        _ = pauseButton.FadeOutAndHide(0.3f);
        finishView.Show(levelPosition, starsCollected, num, totalTime, newHighScore);
        finishView.NextLevelEvent.AddListener(nextLevelEvent.SendEvent);
        FinishWithViewPosition(finishView, zoomPoint);
    }

    public void FinishWithViewPosition(MovieStripesView view, Vector2 zoomPoint)
    {
        if (ContreJourConfig.BackButtonVisible)
        {
            HidePause();
        }
        finished = true;
        if (view != null)
        {
            AddChild(view, 16);
            view.RestartEvent.AddListener(HardRestart);
            view.MenuEvent.AddListener(backEvent.SendEvent);
        }
        Vector2 rootSize = ContreJourConfig.RootSize;
        ZoomToScaleRightTopLeftBottomTime(rightTop: new Vector2(0f, 30f), leftBottom: new Vector2(rootSize.X * -0.29999995f, (rootSize.Y * -0.29999995f) - 30f), zoomPoint: zoomPoint * gameRoot.Scale, scale: 1.3f, time: 2.4f);
    }

    public void HidePause()
    {
        pauseButton?.Enabled = false;
    }

    public void ZoomOut(float time)
    {
        ZoomToScaleTime(new Vector2(0f, 0f), 1f, time);
    }

    public void ZoomToScaleTime(Vector2 zoomPoint, float scale, float time)
    {
        Vector2 w7FromIPhoneSize = ScreenConstants.W7FromIPhoneSize;
        ZoomToScaleRightTopLeftBottomTime(zoomPoint, scale, new Vector2(0f, 0f), new Vector2(w7FromIPhoneSize.X * (1f - scale), w7FromIPhoneSize.Y * (1f - scale)), time);
    }

    public void ZoomToScaleRightTopLeftBottomTime(Vector2 zoomPoint, float scale, Vector2 rightTop, Vector2 leftBottom, float time)
    {
        Vector2 w7FromIPhoneSize = ScreenConstants.W7FromIPhoneSize;
        Vector2 position = new((w7FromIPhoneSize.X / 2f) - (zoomPoint.X * scale), (w7FromIPhoneSize.Y / 2f) - (zoomPoint.Y * scale));
        position.X = position.X.Clamp(leftBottom.X, rightTop.X);
        position.Y = position.Y.Clamp(leftBottom.Y, rightTop.Y);
        _ = gameRoot.MoveTo(time, position, Cubic.EaseInOut).ScaleTo(scale * gameRoot.Scale, Cubic.EaseInOut);
    }

    public void RegisterHero(HeroBodyClip _hero)
    {
        hero = _hero;
        AddPositionProvider(new PositionProviderValue(_hero, 5f));
    }

    private void RemovePositionProvider(PositionProviderValue provider)
    {
        _ = positionProviders.Remove(provider);
        providersValue -= provider.Value;
    }

    public void AddPositionDependent(IPositionDepedent dependent)
    {
        positionDependent.Add(dependent);
    }

    public TouchPositionProvider GetTouchProvider(Touch touch)
    {
        foreach (PositionProviderValue positionProvider in positionProviders.Cast<PositionProviderValue>())
        {
            if (positionProvider.Provider is TouchPositionProvider touchPositionProvider && touchPositionProvider.Touch == touch)
            {
                return touchPositionProvider;
            }
        }
        return null;
    }

    public void AddPositionProvider(PositionProviderValue provider)
    {
        providersValue += provider.Value;
        positionProviders.Add(provider);
        foreach (IPositionDepedent item in positionDependent.Cast<IPositionDepedent>())
        {
            item.ProviderAdded(provider.Provider);
        }
    }

    public IVectorPositionProvider GetRandomPositionProvider()
    {
        if (positionProviders.Count == 0)
        {
            return null;
        }
        float num = Maths.Random(0f, providersValue);
        float num2 = 0f;
        int num3 = 0;
        while (num > num2)
        {
            num2 += ((PositionProviderValue)positionProviders[num3]).Value;
            num3++;
        }
        return ((PositionProviderValue)positionProviders[num3 - 1]).Provider;
    }

    private static void UpdateZoomOut(float time)
    {
    }

    public override void UpdateGame(float time)
    {
        _scheduler.Update(time);
        UpdateZoomOut(time);
        frame++;
        base.UpdateGame(time);
        foreach (PlasticineBodyClip item in plasticine)
        {
            item.UpdateGraphics(time);
        }
        grass?.Update(time);
        lightPowerChanged = false;
        _toRemove.Clear();
        foreach (Touch freeTouch in freeTouches)
        {
            if (ProcessTouchIsFree(freeTouch, isFree: true))
            {
                _toRemove.Add(freeTouch);
            }
        }
        freeTouches.RemoveList(_toRemove);
        _toRemove.Clear();
        foreach (BackgroundBase background in backgrounds)
        {
            ((IUpdatable)background).Update(time);
        }
        for (int i = 0; i < foregrounds.Count; i++)
        {
            foregrounds[i].Update(time);
        }
        windManager.Update(time);
    }

    public void RenewGround()
    {
        foreach (PlasticineBodyClip item in plasticine)
        {
            item.Restart();
        }
    }

    public void UpdateFreeTouch(Touch touch)
    {
        if (ProcessTouchIsFree(touch, isFree: true))
        {
            _ = freeTouches.Remove(touch);
        }
    }

    public void AddView(MovieStripesView view)
    {
        AddChild(view, 16);
    }

    public bool ProcessTouchIsFree(Touch touch, bool isFree)
    {
        //IL_002c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0031: Unknown result type (might be due to invalid IL or missing references)
        if (Root == null)
        {
            return false;
        }
        Vector2 vector = Builder.TouchRootVec(touch);
        float num = 1.5f;
        AABB val = FarseerUtil.CreateAABB(vector, num * 2f, num * 2f);
        AABBQuery aABBQuery = new();
        Builder.World.QueryAABB(aABBQuery.CallbackReportFixture, ref val);
        List<BodyClip> list = [];
        for (int i = 0; i < aABBQuery.Fixtures.Count; i++)
        {
            if (aABBQuery.Fixtures[i].Body.UserData is BodyClip bodyClip && bodyClip is IClickable clickable && (!isFree || clickable.AcceptFreeTouches()))
            {
                list.Add(bodyClip);
            }
        }
        bool result = false;
        if (list.Count != 0)
        {
            list.Sort(new ClickableComparer(vector));
            foreach (IClickable item in list.Cast<IClickable>())
            {
                if (item.TouchBegan(touch))
                {
                    draggingItems[touch] = item;
                    result = true;
                    break;
                }
            }
        }
        return result;
    }

    public bool IsFreeEnabled(Touch touch)
    {
        return freeDisabledTouches.NotExists(touch);
    }

    public void DisableFreeing(Touch touch)
    {
        freeDisabledTouches.Add(touch);
    }

    public void FreeTouch(Touch touch)
    {
        _ = draggingItems.Remove(touch);
        if (freeTouches.NotExists(touch))
        {
            freeTouches.Add(touch);
        }
    }
}
