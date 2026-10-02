using Mokus2D.Graphics;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    public class TextureSnotSprite : SpringSnotSprite
    {
        private float opacity;
        private Color textureColor;

        public float TargetOpacity { get; set; }

        public Color TextureColor
        {
            get => textureColor;
            set
            {
                if (textureColor != value)
                {
                    textureColor = value;
                    for (int i = 0; i < Vertices.Length; i++)
                    {
                        Vertices[i].Color = textureColor;
                    }
                }
            }
        }

        public TextureSnotSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth, string textureFile)
            : base(game, snot, startWidth, centerWidth, endWidth)
        {
            Texture = ClipFactory.GetTexture(textureFile);
            TargetOpacity = 255f;
            opacity = 255f;
            textureColor = new Color(255, 255, 255);
            NeckColor = Color.White;
        }

        public TextureSnotSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth)
            : this(game, snot, startWidth, centerWidth, endWidth, game.ChooseSide("blackStrongSnotTexture", "whiteStrongSnotTexture", "strongSnotTexture", "strongSnotTexture", "greenStrongSnotTexture"))
        {
        }

        public override void Update(float time)
        {
            base.Update(time);
            opacity = Maths.StepTo(opacity, TargetOpacity, 10f);
        }

        public override void CreateVectors(int allPointsSize)
        {
            base.CreateVectors(allPointsSize);
            GraphUtil.CreateTextureCoordsVerticesStep((allPointsSize / 2) - 1, Vertices, 0.1f);
        }

        protected override void RefreshTextureCoords(int i, int start)
        {
        }

        public override void DrawCircles()
        {
        }

        public override void DrawBorder()
        {
        }

        public override void DrawPolygons()
        {
            GraphUtil.FillTrianglesList(Vertices);
        }
    }
}
