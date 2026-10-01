using System;
using System.Collections.Generic;
using System.Linq;

using ContreJour.Clips;

using Mokus2D.Graphics;
using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    public class ChapterItem : Node
    {
        private readonly List<IUpdatable> updating = [];

        protected List<object> AlphaItems { get; set; } = [];

        private Sprite backLight;

        protected Sprite Background { get; set; }

        protected Sprite BlurBackground { get; set; }

        private RadiusClickListener clickListener;

        protected Node Container { get; set; }

        private float depth;

        protected List<object> DepthDependent { get; set; } = [];
        protected List<object> HidingItems { get; set; } = [];

        public int Index { get; }

        private Color lightColor;

        protected MainMenu Menu { get; set; }

        public float Offset { get; }

        public virtual float Depth
        {
            get => depth;
            set
            {
                if (depth != value)
                {
                    depth = value;
                    RefreshDepth();
                }
            }
        }

        public Color LightColor
        {
            get => lightColor;
            set => lightColor = value;
        }

        public bool Enabled { get; set; }

        public override float OpacityFloat
        {
            set
            {
                base.OpacityFloat = value;
                RefreshDepth();
            }
        }

        public event Action<int> SelectEvent;

        public ChapterItem(int index, MainMenu menu)
        {
            Menu = menu;
            Index = index;
            depth = -1f;
            Container = new Node();
            AddChild(Container);
            CreateSprites();
            AddChild(BlurBackground);
            HidingItems.Add(Background);
            Offset = Index * (float)Math.PI * 2f / ContreJourConstants.PlanetsCount;
            CreateClickListener();
            Enabled = true;
        }

        public override void Update(float time)
        {
            if (!(depth > 0.75f))
            {
                return;
            }
            float time2 = Math.Max((depth - 0.75f) * 4f * time, 0f);
            foreach (IUpdatable item in updating)
            {
                item.Update(time2);
            }
        }

        protected virtual void CreateClickListener()
        {
            clickListener = new RadiusClickListener(this, 90f)
            {
                Radius = 40f,
                DisableDrag = true
            };
            clickListener.ClickEvent.AddListener(OnClick);
        }

        protected void AddUpdating(IUpdatable item)
        {
            updating.Add(item);
            if (item is Node node)
            {
                node.UpdateEnabled = false;
            }
        }

        protected virtual void CreateBackLight()
        {
            backLight = new Sprite(ClipIds.Menu.McChapterLight)
            {
                Scale = 2.5f
            };
            AddChild(backLight);
            AddAlphaItem(backLight);
        }

        public void AddHidingItem(Node item)
        {
            HidingItems.Add(item);
        }

        public void AddAlphaItem(Node item)
        {
            AlphaItems.Add(item);
        }

        protected virtual void CreateSprites()
        {
            Background = new Sprite(ClipIds.Planets.McPlanet1Background);
            BlurBackground = new Sprite(ClipIds.Planets.McChapter1Blur);
            AddChild(Background);
        }

        protected virtual void RefreshDepth()
        {
            backLight?.Color = lightColor;
            Color color = ColorUtil.Mult(lightColor, (1f - depth) * 0.3f);
            BlurBackground.Color = color;
            float num = Maths.Clamp((depth - 0.7f) * OpacityByte / 0.3f, 0f, 255f);
            foreach (Node alphaItem in AlphaItems.Cast<Node>())
            {
                alphaItem.OpacityByte = (int)num;
                alphaItem.Visible = num > 0f;
            }
            float num2 = Maths.Clamp((1f - depth) / 0.2f * OpacityByte, 0f, 255f);
            BlurBackground.OpacityByte = (int)num2;
            BlurBackground.Visible = num2 > 0f;
            Background.OpacityByte = (int)Maths.Clamp(num * 3f, 0f, 255f);
            Container.Visible = num > 0f;
            foreach (IDepthDependent item in DepthDependent.Cast<IDepthDependent>())
            {
                item.Depth = depth;
            }
        }

        public virtual void RemoveListeners()
        {
            clickListener.Enabled = false;
            clickListener.ClickEvent.RemoveListener(OnClick);
            clickListener.Remove();
        }

        protected virtual void OnClick()
        {
            if (depth > 0.99f)
            {
                OnSelect();
            }
        }

        // A click on the planet, from the keyboard.
        public void Click()
        {
            OnClick();
        }

        public void OnSelect()
        {
            SelectEvent.Dispatch(Index);
        }
    }
}
