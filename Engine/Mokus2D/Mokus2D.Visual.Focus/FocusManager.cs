using System;
using System.Collections.Generic;

namespace Mokus2D.Visual.Focus;

public static class FocusManager
{
    private static readonly HashSet<IFocus> FocusItems = [];

    private static readonly Action<IFocus> FocusInHandler = OnFocusIn;

    private static readonly Action<IFocus> FocusOutHandler = OnFocusOut;

    public static IFocus CurrentFocus { get; private set; }

    public static void AddItem(IFocus item)
    {
        _ = FocusItems.Add(item);
        item.FocusInEvent += FocusInHandler;
        item.FocusOutEvent += FocusOutHandler;
    }

    public static void RemoveItem(IFocus item)
    {
        _ = FocusItems.Remove(item);
        item.FocusInEvent -= FocusInHandler;
        item.FocusOutEvent -= FocusOutHandler;
    }

    private static void OnFocusOut(IFocus item)
    {
        if (item == CurrentFocus)
        {
            CurrentFocus.HasFocus = false;
            CurrentFocus = null;
        }
    }

    private static void OnFocusIn(IFocus item)
    {
        if (CurrentFocus != item && CurrentFocus != null)
        {
            CurrentFocus.HasFocus = false;
        }
        CurrentFocus = item;
        CurrentFocus.HasFocus = true;
    }
}
