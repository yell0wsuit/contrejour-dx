using System;
using System.Collections.Generic;

using Default.Namespace;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Input;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Menu.SnapPanel;

public class SnapPanel : AccelerometerNode, ITouchListener
{
    private const float BackgroundOffset = 0.1f;

    private Sprite background;

    private readonly Dictionary<Node, Tuple<float, Vector2>> accelerometerChildren = new Dictionary<Node, Tuple<float, Vector2>>();

    public Vector2 Size => background.Size * 0.8f;

    public SnapPanel()
    {
        maxAccOffset = new Vector2(1f, 0.6f);
        background = AddAccelerometerChild("Win8Background", 70f, Vector2.Zero);
    }

    public void Initialize(Vector2 visibleSize)
    {
        AddAccelerometerChild("Win8Background1", 35f, new Vector2(40f, -360f));
        AddAccelerometerChild("Win8Background2", 29f, new Vector2(0f, visibleSize.Y / 2f - 40f)).Anchor = new Vector2(0.5f, 0f);
        AddAccelerometerChild(new SnapParticles(50f), 20f, new Vector2(-100f, -460f));
        AddAccelerometerChild("Win8Petit", 18f, new Vector2(30f, -144f));
        AddAccelerometerChild(new SnapParticles(30f), 20f, new Vector2(-130f, -500f));
        SidePanelParticles sidePanelParticles = new SidePanelParticles();
        AddAccelerometerChild(sidePanelParticles, -10f, Vector2.Zero);
        sidePanelParticles.BottomLeftBound = -visibleSize / 2f - new Vector2(150f, 200f);
        sidePanelParticles.TopRightBound = visibleSize / 2f + new Vector2(50f, 200f);
        sidePanelParticles.HorizontalPosition = new RandomRange(0f, 0.5f * visibleSize.X);
        sidePanelParticles.VerticalPosition = new RandomRange(visibleSize.Y / 2f + 30f, 0f);
        sidePanelParticles.CreateBetweenBounds(40);
        AddAccelerometerChild("Win8TopLeafs", -20f, new Vector2(0f, visibleSize.Y / 2f + 30f)).Anchor = new Vector2(0.5f, 0f);
        AddAccelerometerChild("Win8BottomLeafs", -20f, new Vector2(0f, (0f - visibleSize.Y) / 2f - 30f)).Anchor = new Vector2(0.5f, 1f);
        Sprite topLeft = CreateCorner(visibleSize, new Vector2(-1f, 1f), 0f);
        CreateCorner(visibleSize, new Vector2(1f, 1f), 270f);
        CreateCorner(visibleSize, new Vector2(1f, -1f), 180f);
        CreateCorner(visibleSize, new Vector2(-1f, -1f), 90f);
        CreateBorder(visibleSize, new Vector2(0f, 1f), topLeft, 0f, visibleSize.X);
        CreateBorder(visibleSize, new Vector2(1f, 0f), topLeft, 270f, visibleSize.Y);
        CreateBorder(visibleSize, new Vector2(0f, -1f), topLeft, 180f, visibleSize.X);
        CreateBorder(visibleSize, new Vector2(-1f, 0f), topLeft, 90f, visibleSize.Y);
    }

    private Sprite CreateBorder(Vector2 visibleSize, Vector2 relativePosition, Sprite topLeft, float rotation, float sideSize)
    {
        Sprite sprite = new Sprite("Win8FramePart");
        sprite.Position = visibleSize * relativePosition / 2f;
        sprite.Anchor = new Vector2(0.5f, 0f);
        sprite.RotationDegrees = rotation;
        sprite.ScaleX = (sideSize - topLeft.Size.X * 2f) / sprite.Size.X;
        AddChild(sprite);
        return sprite;
    }

    private Sprite CreateCorner(Vector2 visibleSize, Vector2 relativePosition, float rotation)
    {
        Sprite sprite = new Sprite("Win8FrameAngle");
        sprite.Anchor = Vector2.Zero;
        sprite.Position = visibleSize * relativePosition / 2f;
        sprite.RotationDegrees = rotation;
        sprite.Color = Color.Red;
        AddChild(sprite);
        return sprite;
    }

    private Sprite AddAccelerometerChild(string name, float offset, Vector2 position)
    {
        Sprite sprite = new Sprite(name);
        AddAccelerometerChild(sprite, offset, position);
        return sprite;
    }

    private void AddAccelerometerChild(Node child, float offset, Vector2 position)
    {
        AddChild(child);
        accelerometerChildren[child] = new Tuple<float, Vector2>(offset, position);
        child.Position = position;
    }

    protected override void OnAddedToStage()
    {
        base.OnAddedToStage();
        Mokus2DGame.Instance.TouchController.AddListener(this);
    }

    protected override void OnRemovedFromStage()
    {
        base.OnRemovedFromStage();
        Mokus2DGame.Instance.TouchController.RemoveListener(this);
    }

    public override void Update(float time)
    {
        base.Update(time);
        foreach (KeyValuePair<Node, Tuple<float, Vector2>> accelerometerChild in accelerometerChildren)
        {
            accelerometerChild.Key.Position = accelerometerChild.Value.Item2 + accelerometerChild.Value.Item1 * accelerometerOffset;
        }
    }

    public bool TouchBegin(Touch touch)
    {
        return true;
    }

    public bool TouchMove(Touch touch)
    {
        return true;
    }

    public void TouchEnd(Touch touch)
    {
        Visible = false;
    }
}
