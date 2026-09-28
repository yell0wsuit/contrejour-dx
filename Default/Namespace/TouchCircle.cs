using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class TouchCircle : BodyClip
{
    public class ClosestItem
    {
        public PlasticinePartBodyClip Clip;

        public bool Negative;

        public bool Refreshed;

        public ClosestItem()
        {
            Clip = null;
            Refreshed = false;
        }

        public ClosestItem(PlasticinePartBodyClip clip, bool refreshed)
        {
            Clip = clip;
            Refreshed = refreshed;
        }
    }

    private const float NEGATIVE_MULT = -0.25f;

    private const float FORCE = 350f;

    private const float MAX_OFFSET = 2.3333333f;

    private const float ACTION_RADIUS = 3.3333333f;

    private const float RADIUS = 4f;

    protected Dictionary<PlasticineBodyClip, ClosestItem> closestMap;

    protected bool enabled;

    protected bool free;

    protected Touch touch;

    public Touch Touch => touch;

    public bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            enabled = value;
        }
    }

    public bool Free => free;

    public new Vector2 Position => builder.TouchRootPoint(touch);

    public TouchCircle(Touch _touch, LevelBuilderBase _builder)
        : base(_builder, CreateBody(_builder, _touch), null, null)
    {
        touch = _touch;
        enabled = true;
        free = true;
    }

    public static Body CreateBody(LevelBuilderBase _builder, Touch _touch)
    {
        Body result = _builder.World.CreateCircle(4f, _builder.TouchRootVec(_touch), 0f, 0f, dynamic: true);
        result.SetSensor(value: true);
        return result;
    }

    public override void Update(float time)
    {
        base.Update(time);
        Body.SetTransform(builder.TouchRootVec(touch), 0f);
        if (enabled)
        {
            ProcessContacts();
        }
    }

    public void ProcessContacts()
    {
        ContactEdge val = Body.ContactList;
        bool flag = false;
        while (val != null)
        {
            if (val.Contact.IsTouching && val.Other.UserData is PlasticinePartBodyClip)
            {
                flag = true;
            }
            val = val.Next;
        }
        free = !flag;
    }

    public bool IsNegative(PlasticinePartBodyClip part)
    {
        if (!closestMap.ContainsKey(part.Parent))
        {
            closestMap[part.Parent] = new ClosestItem(null, refreshed: false);
        }
        ClosestItem closestItem = closestMap[part.Parent];
        if (!closestItem.Refreshed)
        {
            RefreshClosestPlasticineDefaultItem(closestItem, part);
        }
        return closestItem.Negative;
    }

    public void RefreshClosestPlasticineDefaultItem(ClosestItem item, PlasticinePartBodyClip defaultItem)
    {
        if (item.Clip == null)
        {
            item.Clip = defaultItem;
        }
        float num = Body.Position.DistanceTo(item.Clip.Body.Position);
        float num2 = Body.Position.DistanceTo(defaultItem.Body.Position);
        if (num > num2)
        {
            num = num2;
            item.Clip = defaultItem;
        }
        PlasticineItem plasticineItem = item.Clip.Item;
        bool flag = false;
        bool flag2 = false;
        do
        {
            flag = false;
            plasticineItem = plasticineItem.PreviousItem;
            float num3 = plasticineItem.Body.Position.DistanceTo(Body.Position);
            if (num3 < num)
            {
                flag = true;
                flag2 = true;
                num = num3;
            }
        }
        while (flag);
        plasticineItem = plasticineItem.NextItem;
        if (!flag2)
        {
            do
            {
                flag = false;
                plasticineItem = plasticineItem.NextItem;
                float num4 = plasticineItem.Body.Position.DistanceTo(Body.Position);
                if (num4 < num)
                {
                    flag = true;
                    num = num4;
                }
            }
            while (flag);
            plasticineItem = plasticineItem.PreviousItem;
        }
        item.Clip = plasticineItem.BodyClip;
        item.Negative = VectorUtil.Projection(Body.Position - item.Clip.Body.Position, item.Clip.Normal) > 0.7f;
        item.Refreshed = true;
    }

    private void Dealloc()
    {
        builder.World.RemoveBody(Body);
    }
}
