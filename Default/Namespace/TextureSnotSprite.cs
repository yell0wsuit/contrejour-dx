using Microsoft.Xna.Framework;

using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class TextureSnotSprite : SpringSnotSprite
{
    private const float TEXTURE_STEP = 0.1f;

    private const float OPACITY_STEP = 10f;

    protected float opacity;

    protected float targetOpacity;

    protected Color textureColor;

    public float TargetOpacity
    {
        get
        {
            return targetOpacity;
        }
        set
        {
            targetOpacity = value;
        }
    }

    public Color TextureColor
    {
        get
        {
            return textureColor;
        }
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

    public TextureSnotSprite(ContreJourGame _game, SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth, string textureFile)
        : base(_game, _snot, _startWidth, _centerWidth, _endWidth)
    {
        Texture = ClipFactory.GetTexture(textureFile);
        targetOpacity = 255f;
        opacity = 255f;
        textureColor = new Color(255, 255, 255);
        base.NeckColor = Color.White;
    }

    public TextureSnotSprite(ContreJourGame _game, SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth)
        : this(_game, _snot, _startWidth, _centerWidth, _endWidth, _game.ChooseSide("blackStrongSnotTexture", "whiteStrongSnotTexture", "strongSnotTexture", "strongSnotTexture", "greenStrongSnotTexture"))
    {
    }

    public override void Update(float time)
    {
        base.Update(time);
        opacity = Maths.StepTo(opacity, targetOpacity, 10f);
    }

    public override void CreateVectors(int _allPointsSize)
    {
        base.CreateVectors(_allPointsSize);
        GraphUtil.CreateTextureCoordsVerticesStep(_allPointsSize / 2 - 1, vertices, 0.1f);
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
