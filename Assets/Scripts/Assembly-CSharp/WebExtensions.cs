using System;

public static class WebExtensions
{
	public static bool IsWebUrl(string url)
	{
		if (Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			if (!(result.Scheme == Uri.UriSchemeHttp))
			{
				return result.Scheme == Uri.UriSchemeHttps;
			}
			return true;
		}
		return false;
	}
}
