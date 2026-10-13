using System;

public static class FirebaseUtility
{
	public static DateTime ConvertTimeFromTimestamp(string dateTime)
	{
		return DateTime.ParseExact(dateTime.Trim(), "yyyy-MM-ddTHH:mm:ss.fffZ", null);
	}
}
