using System;
using System.Collections.Generic;
using System.Linq;

using ContreJour.Clips.menu;
using ContreJour.Clips.planets;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class ChapterItem : Node
{
    private readonly List<IUpdatable> updating = [];

    protected List<object> alphaItems = [];

    protected Sprite backLight;

    protected Sprite background;

    protected Sprite blurBackground;

    protected RadiusClickListener clickListener;

    protected Node container;

    protected float depth;

    protected List<object> depthDependent = [];

    protected bool enabled;

    protected List<object> hidingItems = [];

    protected int index;

    protected Color lightColor;

    protected MainMenu menu;

    protected float offset;

    public float Offset => offset;

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

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    public int Index => index;

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
        this.menu = menu;
        this.index = index;
        depth = -1f;
        container = new Node();
        AddChild(container);
        CreateSprites();
        AddChild(blurBackground);
        hidingItems.Add(background);
        offset = this.index * (float)Math.PI * 2f / ContreJourConstants.PlanetsCount;
        CreateClickListener();
        enabled = true;
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
        if (item is Node)
        {
            ((Node)item).UpdateEnabled = false;
        }
    }

    protected virtual void CreateBackLight()
    {
        backLight = new McChapterLight
        {
            Scale = 2.5f
        };
        AddChild(backLight);
        AddAlphaItem(backLight);
    }

    public void AddHidingItem(Node item)
    {
        hidingItems.Add(item);
    }

    public void AddAlphaItem(Node item)
    {
        alphaItems.Add(item);
    }

    protected virtual void CreateSprites()
    {
        background = new McPlanet1Background();
        blurBackground = new McChapter1Blur();
        AddChild(background);
    }

    protected virtual void RefreshDepth()
    {
        backLight?.Color = lightColor;
        Color color = ColorUtil.Mult(lightColor, (1f - depth) * 0.3f);
        blurBackground.Color = color;
        float num = Maths.Clamp((depth - 0.7f) * OpacityByte / 0.3f, 0f, 255f);
        foreach (Node alphaItem in alphaItems.Cast<Node>())
        {
            alphaItem.OpacityByte = (int)num;
            alphaItem.Visible = num > 0f;
        }
        float num2 = Maths.Clamp((1f - depth) / 0.2f * OpacityByte, 0f, 255f);
        blurBackground.OpacityByte = (int)num2;
        blurBackground.Visible = num2 > 0f;
        background.OpacityByte = (int)Maths.Clamp(num * 3f, 0f, 255f);
        container.Visible = num > 0f;
        foreach (IDepthDependent item in depthDependent.Cast<IDepthDependent>())
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

    public void OnSelect()
    {
        SelectEvent.Dispatch(index);
    }
}
