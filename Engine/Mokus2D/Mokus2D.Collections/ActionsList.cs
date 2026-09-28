using System;
using System.Collections.Generic;

namespace Mokus2D.Collections;

public class ActionsList<T> : List<T>
{
    public Action<T> AddAction { get; private set; }

    public Func<T, bool> AddFunction { get; private set; }

    public ActionsList()
    {
        Initialize();
    }

    public ActionsList(int capacity)
        : base(capacity)
    {
        Initialize();
    }

    public ActionsList(IEnumerable<T> collection)
        : base(collection)
    {
        Initialize();
    }

    private void Initialize()
    {
        AddAction = base.Add;
        AddFunction = delegate (T arg)
        {
            Add(arg);
            return true;
        };
    }
}
