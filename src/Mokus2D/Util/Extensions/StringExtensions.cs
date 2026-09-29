using System;
using System.Globalization;

namespace Mokus2D.Util.Extensions
{
    public static class StringExtensions
    {

        public static bool IsNotEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        public static string FormatThis(this string s, params object[] p)
        {
            return string.Format(CultureInfo.InvariantCulture, s, p);
        }

        public static int ToInt(this string s)
        {
            return Convert.ToInt32(s, CultureInfo.InvariantCulture);
        }
    }
}
