using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.PlatformSupport.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.GameDebug;

namespace Mokus2D.Input;

public class TouchController : IUpdatable
{
    public const int MaxTouches = 16;

    private static readonly Pool<Touch> poolTouches = new(() => new Touch());

    private static readonly Pool<List<ITouchListener>> poolListeners = new(() => []);

    public Matrix TransformMatrix = Matrix.Identity;

    private readonly List<ITouchListener> listenersCopy = new(64);

    private readonly List<Touch> newTouches = new(16);

    private readonly SortedList<int> prioritiesList = new(64, Comparisons.IntReverseComparizon);

    private readonly List<Touch> toBegin = new(16);

    private readonly List<Touch> toEnd = new(16);

    private readonly List<Touch> toMove = new(16);

    private readonly Dictionary<int, List<ITouchListener>> listeners = [];

    private readonly Dictionary<ITouchListener, int> priorities = new(64);

    private readonly Dictionary<Touch, List<ITouchListener>> touches = new(16);

    public void Update(float time)
    {
        newTouches.Clear();
        toBegin.Clear();
        toMove.Clear();
        List<CursorPoint> cursorPoints = CursorPoints.GetCursorPoints();
        TransformTouchesCoords(cursorPoints);
        foreach (CursorPoint item in cursorPoints)
        {
            Touch touch = null;
            foreach (KeyValuePair<Touch, List<ITouchListener>> touch2 in touches)
            {
                if (touch2.Key.Id == item.Id)
                {
                    touch = touch2.Key;
                    break;
                }
            }
            if (touch == null)
            {
                touch = poolTouches.New();
                touch.Initialize(item);
                toBegin.Add(touch);
                touch.Active = true;
            }
            else
            {
                if (touch.Position != item.Position)
                {
                    toMove.Add(touch);
                }
                touch.Position = item.Position;
            }
            newTouches.Add(touch);
            touch.Refresh();
        }
        toEnd.Clear();
        foreach (Touch key in touches.Keys)
        {
            if (!newTouches.Contains(key))
            {
                toEnd.Add(key);
                key.Refresh();
                key.Active = false;
            }
        }
        SendBegin(toBegin);
        SendMove(toMove);
        SendEndAndRemove(toEnd);
        listenersCopy.Clear();
    }

    private void TransformTouchesCoords(List<CursorPoint> currentTouches)
    {
        if (TransformMatrix != Matrix.Identity)
        {
            for (int i = 0; i < currentTouches.Count; i++)
            {
                CursorPoint cursorPoint = currentTouches[i];
                currentTouches[i] = new CursorPoint(cursorPoint.Position.Transform(ref TransformMatrix), cursorPoint.Id, cursorPoint.Type);
            }
        }
    }

    public void AddListener(ITouchListener listener, int priority = 0)
    {
        GetListeners(priority).Add(listener);
        priorities[listener] = priority;
    }

    public void RemoveListener(ITouchListener listener)
    {
        int priority = priorities[listener];
        _ = priorities.Remove(listener);
        foreach (KeyValuePair<Touch, List<ITouchListener>> touch in touches)
        {
            if (touch.Value.Contains(listener))
            {
                _ = touch.Value.Remove(listener);
            }
        }
        _ = GetListeners(priority).Remove(listener);
    }

    private List<ITouchListener> GetListeners(int priority)
    {
        if (!listeners.ContainsKey(priority))
        {
            listeners[priority] = [];
            prioritiesList.Add(priority);
        }
        return listeners[priority];
    }

    private void SendEndAndRemove(List<Touch> toEnd)
    {
        foreach (Touch item in toEnd)
        {
            List<ITouchListener> list = touches[item];
            _ = touches.Remove(item);
            poolTouches.Free(item);
            if (!item.Stoped)
            {
                SendEnd(item, list);
            }
            list.Clear();
            poolListeners.Free(list);
        }
    }

    private void SendEnd(Touch touch, List<ITouchListener> listeners)
    {
        foreach (ITouchListener listener in listeners)
        {
            listener.TouchEnd(touch);
            if (touch.Stoped)
            {
                break;
            }
        }
    }

    private void SendMove(List<Touch> toMove)
    {
        foreach (Touch item in toMove)
        {
            if (!item.Stoped)
            {
                List<ITouchListener> list = touches[item];
                listenersCopy.Clear();
                SendMove(item, list);
                list.Clear();
                list.AddItemsNoGarbage(listenersCopy);
            }
        }
    }

    private void SendMove(Touch touch, List<ITouchListener> listeners)
    {
        foreach (ITouchListener listener in listeners)
        {
            using (new GarbageTracer(listener.GetType().Name))
            {
                if (listener.TouchMove(touch))
                {
                    listenersCopy.Add(listener);
                }
                if (touch.Stoped)
                {
                    break;
                }
            }
        }
    }

    private void SendBegin(List<Touch> toBegin)
    {
        if (toBegin.Count == 0)
        {
            return;
        }
        foreach (Touch item in toBegin)
        {
            if (!item.Stoped)
            {
                List<ITouchListener> list = poolListeners.New();
                list.Clear();
                touches[item] = list;
                SendBegin(item, list);
            }
        }
    }

    private void SendBegin(Touch touch, List<ITouchListener> confirmedListeners)
    {
        foreach (int priorities in prioritiesList)
        {
            listenersCopy.Clear();
            listenersCopy.AddItemsNoGarbage(listeners[priorities]);
            foreach (ITouchListener item in listenersCopy)
            {
                if (item.TouchBegin(touch))
                {
                    confirmedListeners.Add(item);
                }
                if (touch.Stoped)
                {
                    return;
                }
            }
        }
    }
}
