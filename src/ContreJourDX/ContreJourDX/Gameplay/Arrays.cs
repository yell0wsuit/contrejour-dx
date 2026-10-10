using System.Collections.Generic;

using Mokus2D.Util.MathUtils;

namespace ContreJourDX.Gameplay
{
    public class Arrays
    {
        public static object RandomItem(List<object> source)
        {
            return source[Maths.Random(source.Count)];
        }

        public static object MaxItem<T>(List<T> source, MaxItemScore getValueDelegate, object param)
        {
            float num = float.NegativeInfinity;
            object result = null;
            foreach (T item in source)
            {
                object obj = item;
                float num2 = getValueDelegate(obj, param);
                if (num2 > num)
                {
                    num = num2;
                    result = obj;
                }
            }
            return result;
        }
    }
}
