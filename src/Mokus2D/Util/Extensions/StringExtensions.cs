using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Mokus2D.Util.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        public static string FormatThis(this string s, params object[] p)
        {
            return string.Format(CultureInfo.InvariantCulture, s, p);
        }

        public static List<string> ToList(this string s, params char[] p)
        {
            return [.. s.Split(p)];
        }

        public static byte[] ToBytesBase64(this string s)
        {
            return Convert.FromBase64String(s);
        }

        public static string ToStringBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static int ToInt(this string s)
        {
            return Convert.ToInt32(s, CultureInfo.InvariantCulture);
        }

        public static string Repeat(this string str, int count)
        {
            StringBuilder stringBuilder = new();
            while (count-- > 0)
            {
                _ = stringBuilder.Append(str);
            }
            return stringBuilder.ToString();
        }

        public static void Split(this StringBuilder input, char separator, List<StringBuilder> result)
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == separator)
                {
                    result.Add(stringBuilder);
                    stringBuilder = new StringBuilder();
                }
                else
                {
                    _ = stringBuilder.Append(input[i]);
                }
            }
            if (stringBuilder.Length > 0)
            {
                result.Add(stringBuilder);
            }
        }
    }
}
