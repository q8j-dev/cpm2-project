using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace CPM.TuningTimer.Integrity
{
	public static class TuningTimerSignature
	{
		public static string Compute(string secret, TuningTimerFacts facts)
		{
			using HMACSHA256 hMACSHA = new HMACSHA256(Encoding.UTF8.GetBytes(secret ?? string.Empty));
			return Convert.ToBase64String(hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(Canonical(facts))));
		}

		public static bool Verify(string signature, string secret, TuningTimerFacts facts)
		{
			if (string.IsNullOrEmpty(signature))
			{
				return false;
			}
			return FixedTimeEquals(signature, Compute(secret, facts));
		}

		private static string Canonical(TuningTimerFacts facts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int ownerId = facts.OwnerId;
			stringBuilder.Append(ownerId.ToString(CultureInfo.InvariantCulture));
			stringBuilder.Append('|');
			double timeOfStart = facts.TimeOfStart;
			stringBuilder.Append(timeOfStart.ToString("R", CultureInfo.InvariantCulture));
			stringBuilder.Append('|');
			timeOfStart = facts.DurationSeconds;
			stringBuilder.Append(timeOfStart.ToString("R", CultureInfo.InvariantCulture));
			stringBuilder.Append('|');
			IReadOnlyList<int> payloadIds = facts.PayloadIds;
			int num = payloadIds?.Count ?? 0;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = payloadIds[i];
			}
			Array.Sort(array);
			for (int j = 0; j < array.Length; j++)
			{
				if (j > 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(array[j].ToString(CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		private static bool FixedTimeEquals(string a, string b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < a.Length; i++)
			{
				num |= a[i] ^ b[i];
			}
			return num == 0;
		}
	}
}
