using System;
using System.Text;

namespace Mokus2D.Util;

public static class Strings
{
	private const string AllowedSymbols = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_";

	private static readonly Random RandomGenerator = new Random();

	public static string GenerateRandomString(int length)
	{
		StringBuilder stringBuilder = new StringBuilder(length);
		for (int i = 0; i < length; i++)
		{
			stringBuilder.Append("qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_"[RandomGenerator.Next("qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM_".Length)]);
		}
		return stringBuilder.ToString();
	}
}
