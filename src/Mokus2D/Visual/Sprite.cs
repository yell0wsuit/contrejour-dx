using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual;

public class Sprite : AnchorNode, ITextureNode, IAnchorNode, ISizeNode, IBlendable, IDataReloadable
{
    public IQuad Quad { get; }

    private bool _quadDirty;

    private Rectangle _textureRectangle;

    private bool _textureRectangleDirty;

    private ISpriteData _data;

    public bool IgnoreIfTextureDisposed { get; set; }

    protected Rectangle TextureRectangle
    {
        get => _textureRectangle;
        set
        {
            _textureRectangle = value;
            _textureRectangleDirty = true;
            ResetBounds();
        }
    }

    public override Vector2 TextureSize => TextureRectangle.Size();

    public override Vector2 Anchor
    {
        get => base.Anchor;
        set
        {
            base.Anchor = value;
            SetQuadDirty();
            ResetBounds();
        }
    }

    public Sprite(string name)
        : this(Mokus2DGame.LoadResource<ISpriteData>(name))
    {
    }

    public Sprite(ISpriteData data)
        : this(data.Texture)
    {
        ResetData(data);
        Initialize();
    }

    protected Sprite()
        : base(null)
    {
        Quad = CreateQuad();
    }

    public Sprite(Texture2D texture, IQuad quad = null)
        : base(texture)
    {
        Quad = quad ?? CreateQuad();
        RefreshTexture();
        Anchor = Vector2.Zero;
    }

    public virtual void ReloadData()
    {
        string text = null;
        if (_data != null)
        {
            text = _data.Id;
        }
        else if (this is IId id)
        {
            text = id.Id;
        }
        if (text != null)
        {
            ResetData(text);
        }
    }

    public override void Draw(VisualState state)
    {
        if (Texture.IsDisposed)
        {
            ReloadData();
        }
        base.Draw(state);
    }

    public virtual void ResetTexture(Texture2D texture)
    {
        Texture = texture;
        RefreshTexture();
        SetQuadDirty();
    }

    public virtual void ResetData(string id)
    {
        ResetData(Mokus2DGame.LoadSpriteData(id));
    }

    public void ResetData(ISpriteData data)
    {
        _data = data;
        Anchor = data.Anchor;
        ScaleFactor = data.ScaleFactor;
        ResetTexture(data.Texture);
        TextureRectangle = data.TextureRect;
        InitializeConfig(data);
        SetQuadDirty();
        SetTextureRectangleDirty();
    }

    protected virtual void Initialize()
    {
    }

    protected void RefreshTexture()
    {
        TextureRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
    }

    protected void InitializeConfig(IConfig data)
    {
        SetMainConfig(data.Config);
        if (Config != null)
        {
            if (Config.ContainsKey("premultiply"))
            {
                Blend = Config.GetBool("premultiply") ? BlendState.AlphaBlend : BlendState.NonPremultiplied;
            }
            if (Config.ContainsKey("clickable"))
            {
                Clickable = Config.GetBool("clickable");
            }
        }
    }

    protected virtual IQuad CreateQuad()
    {
        return Mokus2DGame.Config.GraphicsConfig.CreateDefaultQuad();
    }

    protected virtual Rectangle GetTileRectangle()
    {
        return TextureRectangle;
    }

    protected virtual Vector2 GetCurrentAnchor()
    {
        return AnchorInPixels;
    }

    protected void SetTextureRectangleDirty()
    {
        _textureRectangleDirty = true;
    }

    protected void SetQuadDirty()
    {
        _quadDirty = true;
    }

    protected override void RefreshTransformations(VisualState parentState)
    {
        base.RefreshTransformations(parentState);
        if (CompositeState.TransformationDirty || _quadDirty)
        {
            RefreshQuad();
            _quadDirty = false;
        }
    }

    protected virtual void RefreshQuad()
    {
        Vector2 spritesScaleFactor = Root.SpritesScaleFactor;
        Quad.RefreshTransformation(CompositeState.Matrix, GetCurrentAnchor() * spritesScaleFactor, GetTileRectangle().Size() * ScaleFactor * spritesScaleFactor);
    }

    protected override void DrawSprite(VisualState state, Color color)
    {
        if (Texture != null && (!Texture.IsDisposed || !IgnoreIfTextureDisposed))
        {
            Quad.RefreshColor(color, CompositeState.ColorRatio);
            if (_textureRectangleDirty)
            {
                Quad.RefreshTextureRect(GetTileRectangle(), Texture.Bounds.Size());
                _textureRectangleDirty = false;
            }
            Quad.Draw(Drawer);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
public class Sprite<T>(Texture2D texture) : Sprite(texture) where T : struct, IVertex
{
    public Sprite(string name)
        : this(Mokus2DGame.LoadResource<ISpriteData>(name))
    {
    }

    public Sprite(ISpriteData data)
        : this(data.Texture)
    {
        ResetData(data);
        Initialize();
    }

    protected override IQuad CreateQuad()
    {
        return new Quad<T>();
    }
}
