using System.Collections.Generic;
using System.Text;

namespace System;

public static class StringExtensions
{
    public static void Intern(this string value)
    {
    }

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
        return string.Format(s, p);
    }

    public static List<string> ToList(this string s, params char[] p)
    {
        return new List<string>(s.Split(p));
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
        return Convert.ToInt32(s);
    }

    public static string Repeat(this string str, int count)
    {
        StringBuilder stringBuilder = new StringBuilder();
        while (count-- > 0)
        {
            stringBuilder.Append(str);
        }
        return stringBuilder.ToString();
    }

    public static void Split(this StringBuilder input, char separator, List<StringBuilder> result)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == separator)
            {
                result.Add(stringBuilder);
                stringBuilder = new StringBuilder();
            }
            else
            {
                stringBuilder.Append(input[i]);
            }
        }
        if (stringBuilder.Length > 0)
        {
            result.Add(stringBuilder);
        }
    }
}
