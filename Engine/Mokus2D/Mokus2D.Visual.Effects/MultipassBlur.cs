using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Animation;
using Mokus2D.Visual.Shaders;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Effects;

public class MultipassBlur
{
    public int PassCount = 2;

    private readonly Texture2D _texture;

    private readonly Vector2 _scale;

    private readonly Vector2 _size;

    private readonly RenderTarget2D _firstTarget;

    private readonly RenderTarget2D _secondTarget;

    private readonly RootNode _root;

    private readonly Sprite _textureSprite;

    private readonly BlurEffect _blurEffect = new BlurEffect();

    public RenderTarget2D CurrentTarget { get; private set; }

    public MultipassBlur(Texture2D texture, Vector2 scale)
    {
        _texture = texture;
        _scale = scale;
        _size = VectorExtensions.Floor(_texture.Size() * _scale);
        _firstTarget = GraphicsUtil.CreateRenderTarget(_size);
        _secondTarget = GraphicsUtil.CreateRenderTarget(_size);
        _root = new RootNode((int)_size.X, (int)_size.Y);
        _textureSprite = new Sprite(texture);
        _textureSprite.Anchor = Vector2.Zero;
        _textureSprite.Effect = _blurEffect;
        _textureSprite.ResetDefaultEffect = true;
        _root.AddChild(_textureSprite);
        CurrentTarget = _firstTarget;
    }

    public void Apply()
    {
        CurrentTarget = _firstTarget;
        _textureSprite.ResetTexture(_texture);
        _textureSprite.ScaleVec = _scale;
        _blurEffect.IsVertical = false;
        for (int i = 0; i < PassCount; i++)
        {
            Mokus2DGame.Device.SetRenderTarget(CurrentTarget);
            Mokus2DGame.Device.Clear(Color.Green * 0f);
            _root.DrawAll();
            if (i > PassCount - 1)
            {
                _textureSprite.ResetTexture(CurrentTarget);
                SwapTargets();
                _textureSprite.Scale = 1f;
                _blurEffect.IsVertical = !_blurEffect.IsVertical;
            }
        }
        Mokus2DGame.Device.SetRenderTarget(null);
    }

    private void SwapTargets()
    {
        if (CurrentTarget == _firstTarget)
        {
            CurrentTarget = _secondTarget;
        }
        else
        {
            CurrentTarget = _firstTarget;
        }
    }
}
