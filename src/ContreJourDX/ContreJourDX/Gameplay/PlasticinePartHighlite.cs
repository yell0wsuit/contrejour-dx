using System;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace ContreJourDX.Gameplay
{
    public class PlasticinePartHighlite : IUpdatable
    {
        private readonly PlasticinePartBodyClip plasticine;

        private readonly LevelBuilderBase builder;

        private readonly ContreJourDXGame game;

        private readonly PlasticineHighliteBorder parent;

        private readonly int index;

        private bool dirty;
        private Vector2 lightBottom;
        private bool highliteSet;

        private Color noLightBorderOut;

        private static readonly Color NoLightBorderOutBlue = ContreJourDXConstants.BlueLightColor.ChangeAlpha(0);

        public static readonly Color NoLightBorderOut = new(0, 0, 0, 0);

        public float LightLength { get; set; }

        public Vector2 LightBottom
        {
            get => lightBottom;
            set => lightBottom = value;
        }

        public bool HasLight { get; set; }

        private bool MirrorLight => game.WhiteSide || game.BlackSide || game.BonusChapter;

        public PlasticinePartHighlite(PlasticinePartBodyClip plasticine, PlasticineHighliteBorder parent, int index)
        {
            this.plasticine = plasticine;
            builder = this.plasticine.Builder;
            game = (ContreJourDXGame)builder.Game;
            plasticine.Highlite = this;
            this.parent = parent;
            this.index = index;
            noLightBorderOut = game.BlackSide ? NoLightBorderOutBlue : NoLightBorderOut;
            noLightBorderOut = NoLightBorderOut;
        }

        public void SetDirty()
        {
            dirty = true;
        }

        private void RefreshPositions()
        {
            float num = Math.Abs((VectorUtil.Atan2(NextBodyClip().Body.Position, game.LightPoint) - ((float)Math.PI / 2f)).SimplifyAngle(NextBodyClip().Body.Rotation - (float)Math.PI) - NextBodyClip().Body.Rotation);
            HasLight = num < 1.3463969f;
            if (MirrorLight && num > (float)Math.PI / 2f)
            {
                num = (float)Math.PI - num;
                HasLight = num < 1.3463969f;
            }
            LightLength = 0f;
            if (HasLight)
            {
                LightLength = 1.2f * Math.Max(1f - (num / 1.3463969f), 0f);
            }
            if (MirrorLight)
            {
                LightLength = Math.Max(LightLength, 0.1f);
                HasLight = true;
            }
            Vector2 vector = new(0f, 0f - LightLength + (7f / 12f));
            Vector2 worldPoint = NextBodyClip().Body.GetWorldPoint(vector);
            lightBottom = builder.ToPoint(worldPoint);
        }

        public void Update(float time)
        {
            if (dirty)
            {
                RefreshPositions();
            }
        }

        public void TryRefresh()
        {
            bool flag = !highliteSet || game.LightPowerChanged;
            if (dirty || flag)
            {
                RefreshBorderColors();
                highliteSet = true;
            }
            if (dirty)
            {
                Refresh();
                dirty = false;
            }
        }

        public void RefreshBorderColors()
        {
            bool flag = PreviousHighlite().HasLight;
            Vertex[] vertices = parent.Vertices;
            Vertex[] array = parent.OutBorder();
            Color mainColor = parent.MainColor;
            LightColor lightColor = game.LightColor;
            if (plasticine.Index == 0)
            {
                vertices[0].Color = array[0].Color = flag ? lightColor.LightOutColor : mainColor;
                vertices[1].Color = flag ? lightColor.LightInColor : mainColor;
                array[1].Color = flag ? lightColor.LightBorderColor : noLightBorderOut;
            }
            vertices[index].Color = array[index].Color = flag ? lightColor.LightOutColor : mainColor;
            vertices[index + 1].Color = flag ? lightColor.LightInColor : mainColor;
            array[index + 1].Color = flag ? lightColor.LightBorderColor : noLightBorderOut;
            vertices[index + 2].Color = array[index + 2].Color = HasLight ? lightColor.LightOutColor : mainColor;
            vertices[index + 3].Color = HasLight ? lightColor.LightInColor : mainColor;
            array[index + 3].Color = HasLight ? lightColor.LightBorderColor : noLightBorderOut;
        }

        public PlasticinePartHighlite PreviousHighlite()
        {
            return Previous().BodyClip.Highlite;
        }

        public PlasticinePartHighlite NextHighlite()
        {
            return plasticine.Item.NextItem.BodyClip.Highlite;
        }

        public PlasticineItem Previous()
        {
            return plasticine.Item.PreviousItem;
        }

        public PlasticineItem Next()
        {
            return plasticine.Item.NextItem;
        }

        public PlasticinePartBodyClip NextBodyClip()
        {
            return plasticine.Item.NextItem.BodyClip;
        }

        public void Refresh()
        {
            Vertex[] vertices = parent.Vertices;
            Vertex[] inBorder = parent.InBorder;
            bool flag = PreviousHighlite().HasLight;
            if (plasticine.Index == 0)
            {
                vertices[0].Position = inBorder[0].Position;
                vertices[1].Position = flag ? new Vector3(PreviousHighlite().LightBottom, 0f) : vertices[0].Position;
            }
            vertices[index].Position = inBorder[index].Position;
            vertices[index + 2].Position = inBorder[index + 2].Position;
            vertices[index + 1].Position = flag ? new Vector3(PreviousHighlite().LightBottom.Middle(lightBottom), 0f) : vertices[index].Position;
            vertices[index + 3].Position = HasLight ? new Vector3(lightBottom, 0f) : vertices[index + 2].Position;
        }
    }
}
