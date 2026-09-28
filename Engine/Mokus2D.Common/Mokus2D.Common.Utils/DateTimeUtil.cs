using System;

namespace Mokus2D.Common.Utils;

public static class DateTimeUtil
{
	public static DateTime FromUnixTime(double unixTimeStamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp);
	}
}
