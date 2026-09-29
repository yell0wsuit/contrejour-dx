using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class TouchCircle(Touch _touch, LevelBuilderBase builder) : BodyClip(builder, CreateBody(builder, _touch), null, null)
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

    public Touch Touch { get; } = _touch;

    public bool Enabled { get; set; } = true;

    public bool Free { get; private set; } = true;

    public new Vector2 Position => builder.TouchRootPoint(Touch);

    public static Body CreateBody(LevelBuilderBase builder, Touch touch)
    {
        Body result = builder.World.CreateCircle(4f, builder.TouchRootVec(touch), 0f, 0f, dynamic: true);
        result.SetSensor(value: true);
        return result;
    }

    public override void Update(float time)
    {
        base.Update(time);
        Body.SetTransform(builder.TouchRootVec(Touch), 0f);
        if (Enabled)
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
        Free = !flag;
    }

    public void RefreshClosestPlasticineDefaultItem(ClosestItem item, PlasticinePartBodyClip defaultItem)
    {
        item.Clip ??= defaultItem;
        float num = Body.Position.DistanceTo(item.Clip.Body.Position);
        float num2 = Body.Position.DistanceTo(defaultItem.Body.Position);
        if (num > num2)
        {
            num = num2;
            item.Clip = defaultItem;
        }
        PlasticineItem plasticineItem = item.Clip.Item;
        bool flag2 = false;
        bool flag;
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
}
