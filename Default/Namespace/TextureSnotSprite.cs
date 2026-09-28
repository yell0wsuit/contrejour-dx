using Microsoft.Xna.Framework;

using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class TextureSnotSprite : SpringSnotSprite
{
    protected float opacity;

    protected float targetOpacity;

    protected Color textureColor;

    public float TargetOpacity
    {
        get => targetOpacity;
        set => targetOpacity = value;
    }

    public Color TextureColor
    {
        get => textureColor;
        set
        {
            if (textureColor != value)
            {
                textureColor = value;
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].Color = textureColor;
                }
            }
        }
    }

    public TextureSnotSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth, string textureFile)
        : base(game, snot, startWidth, centerWidth, endWidth)
    {
        Texture = ClipFactory.GetTexture(textureFile);
        targetOpacity = 255f;
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
        opacity = Maths.StepTo(opacity, targetOpacity, 10f);
    }

    public override void CreateVectors(int allPointsSize)
    {
        base.CreateVectors(allPointsSize);
        GraphUtil.CreateTextureCoordsVerticesStep((allPointsSize / 2) - 1, vertices, 0.1f);
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
        GraphUtil.FillTrianglesList(vertices);
    }
}
