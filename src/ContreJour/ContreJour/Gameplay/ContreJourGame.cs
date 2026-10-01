using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using ContreJour.Config;
using ContreJour.Content;
using ContreJour.Gameplay.Interfaces;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using Mokus2D;
using Mokus2D.Data;
using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Events;
using Mokus2D.Graphics;
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

namespace ContreJour.Gameplay
{
    public class ContreJourGame : GameBase, IDisposable, ITouchListener, IActivatedDependent, IWindManager
    {
        public class ClickableComparer(Vector2 sourcePoint) : IComparer<BodyClip>
        {
            private readonly Vector2 sourcePoint = sourcePoint;

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

        public const float RestartTime = 1.5f;

        private const float RestartDisabledOpacity = 150f / 255f;

        public const float WindStepWhite = 0.02f;

        public const float WindStep = 0.03f;
        public const float ZoomScale = 1.3f;

        public const float ClickRadiusIphone = 1.5f;

        public const float ClickRadius = 1.1666666f;

        public List<SnotPoint> SnotPoints { get; } = new(64);

        public EventSender RestartEvent { get; } = new();

        private readonly PausePanel pausePanel;
        private readonly List<BackgroundBase> backgrounds = [];
        private Color buttonsColor;
        private readonly Dictionary<Touch, IClickable> draggingItems;
        private readonly FinishView finishView;
        private float flyOpacity;
        private readonly List<ForegroundBase> foregrounds = [];
        private readonly List<Touch> freeDisabledTouches;

        private readonly List<Touch> freeTouches;
        private readonly Button pauseButton;
        private Vector2 lightPoint;

        private float lightPower;
        private GravityParticleSystem particles;
        private readonly List<object> positionDependent;

        private readonly List<object> positionProviders;

        private float providersValue;
        private readonly Button restartButton;
        private readonly LayerColor restartLayer;
        private readonly LightColor startLightColor;

        private readonly Hashtable teleports;

        private readonly List<string> texturesToUnload;
        private int zoomOutCount;

        public static readonly int[] MinZoomLevels =
        [
            51, 53, 52, 54, 49, 37, 74, 79, 80, 76,
            55, 77, 86, 87, 83, 94, 84, 91, 95, 85,
            92, 93, 89
        ];

        public static readonly int[] LowFpsLevels = [4, 6, 44, 53, 54, 12];

        private readonly List<object> _toRemove = [];

        private readonly Scheduler _scheduler = new();

        public RectangleFloat LevelScreenPhysicsBounds { get; private set; }

        private Vector2 levelSize = Mokus2DGame.Instance.ScreenSize;

        public RectangleFloat LevelScreenBounds { get; private set; }

        public override bool Paused
        {
            set
            {
                if (Paused != value)
                {
                    ClickableLayer.InteractionsEnabled = !value;
                    GameRoot.UpdateEnabled = !value;
                    base.Paused = value;
                    if (!value && ContreJourConfig.BackButtonVisible)
                    {
                        pauseButton.Enabled = true;
                    }
                    particles?.Paused = value;
                }
            }
        }

        public int Frame { get; set; }

        public WindManager WindManager { get; }

        public Node AlphaBackground { get; }

        public bool BlackSide { get; }

        public bool WhiteSide { get; }

        public int Chapter { get; }

        public EventSender BackEvent { get; }

        public EventSender NextLevelEvent { get; }

        public HeroBodyClip Hero { get; private set; }

        public Vector2 HeroPositionVec => Hero.Body.Position;

        public float FlyOpacity
        {
            get => flyOpacity;
            set
            {
                if (Maths.FuzzyNotEquals(flyOpacity, value))
                {
                    flyOpacity = value;
                    Flyes.OpacityByte = (int)value;
                    Flyes.Visible = value > 0f;
                }
            }
        }

        public List<PlasticineBodyClip> Plasticine { get; } = new(8);

        public Vector2 LightPoint
        {
            get => lightPoint;
            set => lightPoint = value;
        }

        public bool LightPowerChanged { get; private set; }

        public LightColor LightColor { get; private set; }

        public ParticleSystem Flyes { get; private set; }

        public ParticleSystem Dust { get; private set; }

        public ParticleSystem Grass { get; private set; }

        public ParticleSystem Energy { get; private set; }

        public GroundFall GroundFall { get; private set; }

        public EndLevelBodyClip EndLevel { get; set; }

        public bool TouchEnabled { get; set; }

        public int LevelIndex { get; private set; }

        public IBonusAcceptable BonusTarget { get => field ?? Hero; set; }

        public bool CanShowIntro { get; set; }

        public int StarsCollected { get; private set; }

        public bool SnotSend { get; set; }

        public ClickableLayer ClickableLayer { get; }

        public Color ButtonsColor => buttonsColor;

        public bool RestartEnabled
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    RefreshRestartButton();
                }
            }
        }

        public bool Finished { get; set; }

        public new ContreJourLevelBuilder Builder => (ContreJourLevelBuilder)base.Builder;

        public bool RoseChapter => Chapter == 4;

        public bool BonusChapter => Chapter == 5;

        public int HeroIndex => Builder.GameRoot.Children.IndexOf(Hero.Clip);

        public Vector2 HeroPositionPixels => Hero.Clip.Position;

        public float LightPower
        {
            get => lightPower;
            set
            {
                if (Maths.FuzzyNotEquals(lightPower, value))
                {
                    lightPower = value;
                    LightPowerChanged = true;
                    RefreshLightColor();
                }
            }
        }

        public ContreJourGame(int chapter)
        {
            freeDisabledTouches = [];
            Chapter = chapter;
            BlackSide = Chapter == 1;
            WhiteSide = Chapter == 3;
            TouchEnabled = true;
            Vector2 w7FromIPhoneSize = ScreenConstants.W7FromIPhoneSize;
            Vector2 vector = new(w7FromIPhoneSize.X, w7FromIPhoneSize.Y);
            Vector2 point = BlackSide ? new Vector2(w7FromIPhoneSize.X / 2f, w7FromIPhoneSize.Y * 2f) : vector;
            lightPoint = Box2DConfig.DefaultConfig.ToVec(point);
            lightPower = 1f;
            LightColor = ChooseSide(PlasticineConstants.BLUE, PlasticineConstants.BlackLight, PlasticineConstants.LastLight, PlasticineConstants.WHITE, PlasticineConstants.Green);
            startLightColor = LightColor;
            flyOpacity = 255f;
            Mokus2DGame.Instance.TouchController.AddListener(this);
            draggingItems = [];
            providersValue = 0f;
            positionProviders = [];
            positionDependent = [];
            WindManager = new WindManager(WhiteSide ? 0.02f : 0.03f);
            AlphaBackground = new Node();
            GameRoot.AddChild(AlphaBackground, -10);
            freeTouches = [];
            BackEvent = new EventSender();
            NextLevelEvent = new EventSender();
            ClickableLayer = new ClickableLayer();
            AddChild(ClickableLayer, 15);
            restartLayer = new LayerColor(Color.Black, "menu/whitePixel");
            AddChild(restartLayer, 100);
            restartLayer.Visible = false;
            Color color = ColorUtil.Mult(ContreJourConstants.BlueLightColor, 2f);
            buttonsColor = BlackSide ? color : ContreJourConstants.GreyColor;
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
                ClickableLayer.AddChild(pauseButton);
                // As on the iPad: a small restart button left of the pause button, 64 points apart at scale 1.
                restartButton = new Button("menu/McRestartIcon")
                {
                    RealScale = 1.3f
                };
                restartButton.Icon.Scale = 0.6f;
                restartButton.TouchEndEvent += delegate
                {
                    Restart();
                };
                restartButton.Position = ContreJourConfig.BackButtonPosition - new Vector2(64f * restartButton.RealScale, 0f);
                restartButton.Color = buttonsColor;
                restartButton.Enabled = false;
                restartButton.OpacityFloat = RestartDisabledOpacity;
                ClickableLayer.AddChild(restartButton);
            }
            pausePanel = new PausePanel(this);
            AddChild(pausePanel, 15);
            texturesToUnload = [];
            finishView = new FinishView(this);
            StarsCollected = 0;
            teleports = [];
            _ = GameRoot.Schedule(1.5f, EnableRestart);
            Mokus2DGame.Instance.KeysController.AddBackKeyListener(OnBackPress);
        }

        public override LevelBuilderBase CreateLevelBuilder()
        {
            ContreJourLevelBuilder contreJourLevelBuilder = new(this)
            {
                NamespacePrefix = "ContreJour.Gameplay."
            };
            return contreJourLevelBuilder;
        }

        public void LoadLevelIndex(int index)
        {
            Maths.Randomize(LevelIndex);
            LevelIndex = index;
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
            if (!BlackSide)
            {
                CreateFlyes();
                CreateGrass();
            }
            CreateEnergy();
            base.ProcessLevel(level);
            Hashtable levelProperties = level.LevelProperties;
            levelSize = new Vector2(levelProperties.GetFloat("Width"), levelProperties.GetFloat("Height"));
            GameRoot.Scale = ContreJourConfig.RootSize.X / levelSize.X;
            float num = GameRoot.Scale * levelSize.Y;
            GameRoot.Y = ContreJourConfig.RootSize.Y - num;
            Vector2 point = levelSize.AddY(GameRoot.Y / GameRoot.Scale);
            LevelScreenPhysicsBounds = new RectangleFloat(Builder.ToVec(new Vector2(0f, (0f - GameRoot.Y) / GameRoot.Scale)), Builder.ToVec(point));
            LevelScreenBounds = LevelScreenPhysicsBounds * (1f / Builder.SizeMult);
            AlphaBackground.Scale = Math.Max(levelSize.X / ScreenConstants.OsSizes.IPhoneRetina.X, (LevelSize.Y + (GameRoot.Y / GameRoot.Scale)) / ScreenConstants.OsSizes.IPhoneRetina.Y);
            AlphaBackground.Y = (0f - GameRoot.Y) / GameRoot.Scale;
            Builder.Add(Energy, 9);
            CreateBackgrounds(level);
            CreateParticles();
            CreateDust();
            CreateGroundFall();
            if (!BlackSide)
            {
                Builder.Add(Flyes, 6);
                Builder.Add(Grass, -1);
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
            if (!TouchEnabled || Paused)
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
            if (freeTouches.Contains(touch))
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
            return BlackSide ? black : WhiteSide ? white : normal;
        }

        public static void AddShadowSource()
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
            StarsCollected++;
        }

        public void HardRestart()
        {
            Hero?.Removed = true;
            RestartEvent.SendEvent();
            DisableEvents();
        }

        public void SoftRestart()
        {
            TotalTime = 0f;
            StarsCollected = 0;
            foreach (Body body in Builder.World.BodyList)
            {
                if (body.UserData is IRestartable restartable)
                {
                    restartable.Restart();
                }
            }
            foreach (IRemovable updatable in Updatables)
            {
                if (updatable is IRestartable)
                {
                    (updatable as IRestartable).Restart();
                }
            }
        }

        public void SetRestartEnabled(bool value)
        {
            if (value != RestartEnabled)
            {
                RestartEnabled = value;
            }
        }

        public void Restart()
        {
            if (LevelIndex is 0 or 66)
            {
                HardRestart();
            }
            else if (RestartEnabled)
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
            if (!Finished)
            {
                RestartEnabled = true;
            }
        }

        // The iOS SpriteFader: the restart button dims while a restart is unavailable.
        private void RefreshRestartButton()
        {
            if (restartButton == null)
            {
                return;
            }
            restartButton.Enabled = RestartEnabled;
            _ = restartButton.FadeTo(0.15f, RestartEnabled ? 1f : RestartDisabledOpacity);
        }

        public void Back()
        {
            Hero?.Removed = true;
            BackEvent.SendEvent();
            DisableEvents();
        }

        public void Skip()
        {
            Hero?.Removed = true;
            LevelPosition position = LevelsMenu.GetLevelPosition(LevelIndex);
            UserData.Instance.SkipLevel(position);
            NextLevelEvent.SendEvent();
            DisableEvents();
        }

        public void DisableEvents()
        {
            RestartEvent.Enabled = false;
            BackEvent.Enabled = false;
            NextLevelEvent.Enabled = false;
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
            LightColor = startLightColor.Clone();
            LightColor.LightOutColor.R = (byte)(LightColor.LightOutColor.R * lightPower);
            LightColor.LightOutColor.G = (byte)(LightColor.LightOutColor.G * lightPower);
            LightColor.LightOutColor.B = (byte)(LightColor.LightOutColor.B * lightPower);
        }

        public void RegisterPlasticine(PlasticineBodyClip item)
        {
            Plasticine.Add(item);
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
            if (Finished)
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
            if (Finished || pausePanel.Visible)
            {
                Back();
                Mokus2DGame.Instance.KeysController.RemoveBackKeyListener(OnBackPress);
                return;
            }
            OpenPausePanel();
        }

        // Losing focus pauses the level as the pause button does, unless it is over or already paused.
        public void PauseForFocusLoss()
        {
            if (Finished || pausePanel.Visible)
            {
                return;
            }
            OpenPausePanel();
        }

        private void OpenPausePanel()
        {
            if (ContreJourConfig.BackButtonVisible)
            {
                pauseButton.Enabled = false;
            }
            pausePanel.Show();
            Paused = true;
        }

        private void CreateFlyes()
        {
            Flyes = new ParticleSystem(Mokus2DGame.LoadSpriteData("common/McFly"));
            if (BonusChapter)
            {
                Flyes.Color = ContreJourConstants.GreenLightColor;
            }
        }

        private void CreateGroundFall()
        {
            GroundFall = new GroundFall(this);
            Builder.Add(GroundFall, -1);
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
            if (Chapter == 5)
            {
                node.RotationDegrees = 0f;
                node.Scale = ScreenConstants.W7FromIPhoneSize.X / ((Sprite)node).TextureSize.X;
            }
            AddBackgroundColorConfig(node, hashtable);
            if (hashtable.Exists("foreground"))
            {
                Builder.AddForeground(node);
            }
            else if (BlackSide)
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
                BackgroundBase item = (BackgroundBase)ReflectUtil.CreateInstance(typeName: "ContreJour.Gameplay." + hashtable.GetString("type"), mainAssemblyClass: typeof(ContreJourApplication), parameters: [node, hashtable, this]);
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
            Grass = new ParticleSystem(Mokus2DGame.LoadMovieClipData(ChooseSide(null, "chapter4/McWhiteGrass", "chapter5/McGrass_5", "common/McTotalGrass", "McGrass_6")));
        }

        public void CreateDust()
        {
            Dust = new ParticleSystem(WhiteSide ? "chapter4/McDustWhite" : "common/McDust");
            Builder.Add(Dust, 1);
        }

        public void CreateEnergy()
        {
            Energy = new ParticleSystem("common/McEnergyBall");
        }

        public void IncreaseZoomOut()
        {
            if (zoomOutCount == 0)
            {
            }
            zoomOutCount++;
        }

        public override void DecreaseZoomOut()
        {
            zoomOutCount--;
            if (zoomOutCount == 0)
            {
            }
        }

        public void CreateParticles()
        {
            if (!BonusChapter)
            {
                if (WhiteSide)
                {
                    particles = new WhiteSnow();
                    Builder.Add(particles, -1);
                    particles.CreateBetweenBounds(40);
                }
                else if (Chapter == 2)
                {
                    particles = new SnowFall();
                    Builder.Add(particles, 11);
                    particles.CreateBetweenBounds(40);
                }
                else if (BlackSide)
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
            EndLevel.SetVisible(value: false);
            Energy.Visible = false;
            Energy.OpacityByte = 0;
        }

        public static void FocusOnHero()
        {
        }

        public void ShowBonuses()
        {
            EndLevel.SetVisible(value: true);
            EndLevel.ShowPortal();
            Energy.Visible = true;
            _ = Energy.FadeIn(2f);
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
            LevelPosition levelPosition = LevelsMenu.GetLevelPosition(LevelIndex);
            LevelData levelDataByPosition = UserData.Instance.GetLevelDataByPosition(levelPosition);
            int num = UserData.Instance.CompleteLevel(levelPosition, StarsCollected, TotalTime);
            bool newHighScore = levelDataByPosition != null && (levelDataByPosition.Score < num || levelDataByPosition.StarsCount < StarsCollected);
            finishView.Show(levelPosition, StarsCollected, num, TotalTime, newHighScore);
            finishView.NextLevelEvent.AddListener(NextLevelEvent.SendEvent);
            FinishWithViewPosition(finishView, zoomPoint);
        }

        public void FinishWithViewPosition(MovieStripesView view, Vector2 zoomPoint)
        {
            if (ContreJourConfig.BackButtonVisible)
            {
                HidePause();
            }
            Finished = true;
            if (view != null)
            {
                AddChild(view, 16);
                view.RestartEvent.AddListener(HardRestart);
                view.MenuEvent.AddListener(BackEvent.SendEvent);
            }
            Vector2 rootSize = ContreJourConfig.RootSize;
            ZoomToScaleRightTopLeftBottomTime(rightTop: new Vector2(0f, 30f), leftBottom: new Vector2(rootSize.X * -0.29999995f, (rootSize.Y * -0.29999995f) - 30f), zoomPoint: zoomPoint * GameRoot.Scale, scale: 1.3f, time: 2.4f);
        }

        // Covers both a level finish and the game ending, where the back button takes the pause button's place.
        public void HidePause()
        {
            HideButton(pauseButton);
            HideButton(restartButton);
        }

        private static void HideButton(Button button)
        {
            if (button != null)
            {
                button.Enabled = false;
                button.InteractionsEnabled = false;
                _ = button.FadeOutAndHide(0.3f);
            }
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
            _ = GameRoot.MoveTo(time, position, Cubic.EaseInOut).ScaleTo(scale * GameRoot.Scale, Cubic.EaseInOut);
        }

        public void RegisterHero(HeroBodyClip hero)
        {
            Hero = hero;
            AddPositionProvider(new PositionProviderValue(hero, 5f));
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

        private static void UpdateZoomOut()
        {
        }

        public override void UpdateGame(float time)
        {
            _scheduler.Update(time);
            UpdateZoomOut();
            Frame++;
            base.UpdateGame(time);
            foreach (PlasticineBodyClip item in Plasticine)
            {
                item.UpdateGraphics(time);
            }
            Grass?.Update(time);
            LightPowerChanged = false;
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
            WindManager.Update(time);
        }

        public void RenewGround()
        {
            foreach (PlasticineBodyClip item in Plasticine)
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
            return !freeDisabledTouches.Contains(touch);
        }

        public void DisableFreeing(Touch touch)
        {
            freeDisabledTouches.Add(touch);
        }

        public void FreeTouch(Touch touch)
        {
            _ = draggingItems.Remove(touch);
            if (!freeTouches.Contains(touch))
            {
                freeTouches.Add(touch);
            }
        }

    }
}
