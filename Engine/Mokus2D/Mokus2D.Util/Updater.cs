using System;
using System.Collections.Generic;

using Mokus2D.Interfaces;

namespace Mokus2D.Util;

public class Updater : IUpdatable
{
    private sealed class ActionUpdatable(Action<float> action) : IUpdatable
    {
        private readonly Action<float> _action = action;

        public void Update(float time)
        {
            _action(time);
        }
    }

    public bool Paused { get; set; }

    private readonly LinkedList<IUpdatable> _updatables = new();

    public void AddUpdatable(Action<float> action)
    {
        AddUpdatable(new ActionUpdatable(action));
    }

    public void AddUpdatable(IUpdatable updatable)
    {
        _ = _updatables.AddLast(updatable);
    }

    public void RemoveUpdatable(IUpdatable updatable)
    {
        _ = _updatables.Remove(updatable);
    }

    public virtual void Update(float time)
    {
        if (Paused)
        {
            return;
        }
        LinkedListNode<IUpdatable> linkedListNode = _updatables.First;
        while (linkedListNode != null)
        {
            if (linkedListNode.Value is IRemovable { ShouldRemove: not false })
            {
                LinkedListNode<IUpdatable> node = linkedListNode;
                linkedListNode = linkedListNode.Next;
                _updatables.Remove(node);
            }
            else
            {
                linkedListNode.Value.Update(time);
                linkedListNode = linkedListNode.Next;
            }
        }
    }
}
