using System;
using System.Collections.Generic;
using System.Linq;

namespace _Car_Parking.Scripts.DeepLink
{
	public struct DeepLinkData
	{
		public string RawUrl { get; }

		public string Scheme { get; }

		public string Host { get; }

		public string Path { get; }

		public Dictionary<string, string> Parameters { get; }

		public DeepLinkData(string rawUrl)
		{
			RawUrl = rawUrl;
			Uri uri = new Uri(rawUrl);
			Scheme = uri.Scheme;
			Host = uri.Host;
			Path = uri.AbsolutePath;
			Parameters = (from q in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries)
				select q.Split('=', 2)).ToDictionary((string[] parts) => Uri.UnescapeDataString(parts[0]), (string[] parts) => (parts.Length <= 1) ? "" : Uri.UnescapeDataString(parts[1]));
		}

		public bool TryGetParam(string key, out string value)
		{
			return Parameters.TryGetValue(key, out value);
		}

		public override string ToString()
		{
			return RawUrl;
		}
	}
}
