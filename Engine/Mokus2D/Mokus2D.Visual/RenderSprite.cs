using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual;

public class RenderSprite : Sprite
{
    protected RootNode RenderRoot { get; }
    private Color ClearColor = Color.Black * 0f;

    private RenderTarget2D _renderTarget;

    public Node ChildrenContainer { get; }

    public RenderSprite(Vector2 size)
        : this(size, Vector2.One, Vector2.One)
    {
    }

    public RenderSprite(Vector2 size, RootNode root)
        : this(size, root.SpritesScaleFactor, root.ScaleVec)
    {
    }

    public RenderSprite(Vector2 size, Vector2 spritesScaleFactor, Vector2 rootScale)
        : this(GraphicsUtil.CreateRenderTarget(size), spritesScaleFactor, rootScale)
    {
    }

    public RenderSprite(RenderTarget2D texture, Vector2 spritesScaleFactor, Vector2 rootScale)
        : base(texture)
    {
        Anchor = Vector2.Zero;
        if (Mokus2DGame.Config.RenderTargetEnabled)
        {
            RenderRoot = new RootNode(Texture.Width, Texture.Height);
            _renderTarget = (RenderTarget2D)Texture;
            InitRoot(spritesScaleFactor, rootScale);
            ChildrenContainer = RenderRoot;
        }
        else
        {
            ChildrenContainer = new Node();
            base.AddChild(ChildrenContainer, 0);
        }
    }

    public static void ResetUpdateThread()
    {
        RootNode.ResetUpdateThread();
    }

    public override void ResetTexture(Texture2D texture)
    {
        base.ResetTexture(texture);
        _renderTarget = (RenderTarget2D)Texture;
    }

    public void SetRootEffect(ISpriteBatchEffect effect)
    {
        RenderRoot.Effect = effect;
        RenderRoot.ResetDefaultEffect = effect != null;
    }

    private void InitRoot(Vector2 spritesScaleFactor, Vector2 rootScale)
    {
        if (Mokus2DGame.Config.RenderTargetEnabled)
        {
            RenderRoot.ScaleX = Math.Sign(rootScale.X);
            RenderRoot.ScaleY = Math.Sign(rootScale.Y);
            RenderRoot.SpritesScaleFactor = spritesScaleFactor;
        }
    }

    private void InitRoot(Vector2 spritesScaleFactor)
    {
        if (Mokus2DGame.Config.RenderTargetEnabled)
        {
            RenderRoot.SpritesScaleFactor = spritesScaleFactor;
        }
    }

    protected override void OnAddedToStage()
    {
        base.OnAddedToStage();
        InitRoot(Root.SpritesScaleFactor);
    }

    public void UpdateAndDraw(float time = 0f)
    {
        Update(time);
        RedrawTexture();
    }

    public void RedrawTexture()
    {
        if (Mokus2DGame.Config.RenderTargetEnabled)
        {
            RenderRoot.Position = AnchorInPixels;
            Mokus2DGame.Device.SetRenderTarget(_renderTarget);
            Mokus2DGame.Device.Clear(ClearColor);
            DrawContent();
            Mokus2DGame.Device.SetRenderTarget(null);
        }
    }

    protected virtual void DrawContent()
    {
        if (Mokus2DGame.Config.RenderTargetEnabled)
        {
            RenderRoot.DrawAll();
        }
    }

    public override void RemoveChild(Node node)
    {
        ChildrenContainer.RemoveChild(node);
    }

    public override void Update(float time)
    {
        ChildrenContainer.UpdateNode(time);
    }

    public override void AddChild(Node node, int nodeLayer)
    {
        ChildrenContainer.AddChild(node, nodeLayer);
    }

    public override void AddChildAt(Node node, int index)
    {
        ChildrenContainer.AddChildAt(node, index);
    }

    public override int GetChildIndex(Node child)
    {
        return ChildrenContainer.GetChildIndex(child);
    }

    public override void RemoveAllChildren()
    {
        ChildrenContainer.RemoveAllChildren();
    }

    public void AddDebugLayer(string whiteRect)
    {
        AddDebugLayer(whiteRect, Color.Green);
    }

    public void AddDebugLayer(string whiteRect, Color color)
    {
        LayerColor layerColor = new(color, whiteRect)
        {
            OpacityFloat = 0.3f
        };
        AddChild(layerColor, int.MaxValue);
    }
}
