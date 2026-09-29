using System;

namespace Mokus2D.Util
{
    public static class EventUtil
    {
        public static void Dispatch(this EventHandler value, object sender)
        {
            value?.Invoke(sender, EventArgs.Empty);
        }

        public static void Dispatch<T>(this Action<T> value, T argument)
        {
            value?.Invoke(argument);
        }

        public static void Dispatch<T1, T2>(this Action<T1, T2> value, T1 argument1, T2 argument2)
        {
            value?.Invoke(argument1, argument2);
        }

        public static void Dispatch<T1, T2, T3>(this Action<T1, T2, T3> value, T1 argument1, T2 argument2, T3 argument3)
        {
            value?.Invoke(argument1, argument2, argument3);
        }

        public static void Dispatch(this Action value)
        {
            value?.Invoke();
        }
    }
}
