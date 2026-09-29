using System;
using System.Text;

namespace Mokus2D.Util;

public static class Strings
{
    private static readonly Random RandomGenerator = new();

    public static string GenerateRandomString(int length)
    {
        StringBuilder stringBuilder = new(length);
        for (int i = 0; i < length; i++)
        {
            _ = stringBuilder.Append("qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_"[RandomGenerator.Next("qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_".Length)]);
        }
        return stringBuilder.ToString();
    }
}
