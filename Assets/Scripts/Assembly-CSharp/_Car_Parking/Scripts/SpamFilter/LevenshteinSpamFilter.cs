using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.SpamFilter
{
	public class LevenshteinSpamFilter : ISpamFilter
	{
		private string _lastUniqueMessage;

		private bool _firstMessage = true;

		private const float UniquenessThreshold = 0.1f;

		private const int ConsequentSimilarMessagesThreshold = 3;

		private readonly ISpamPunishmentStrategy _spamPunishmentStrategy;

		private readonly List<string> _bannedMessages;

		private int _similarMessagesCount;

		public bool Spamming { get; private set; }

		public LevenshteinSpamFilter(ISpamPunishmentStrategy spamPunishmentStrategy)
		{
			_spamPunishmentStrategy = spamPunishmentStrategy;
			_bannedMessages = new List<string>();
		}

		public void RegisterMessage(string message)
		{
			if (message.Length == 0 || Spamming || IsMessageBanned(message))
			{
				return;
			}
			if (_firstMessage)
			{
				_lastUniqueMessage = message;
				_firstMessage = false;
				_similarMessagesCount = 1;
				return;
			}
			if (GetUniquenessPercent(_lastUniqueMessage, message) >= 0.1f)
			{
				_lastUniqueMessage = message;
				_similarMessagesCount = 1;
				return;
			}
			_similarMessagesCount++;
			if (_similarMessagesCount >= 3)
			{
				Punish().Forget();
			}
		}

		public bool IsMessageBanned(string message)
		{
			return _bannedMessages.Any((string m) => GetUniquenessPercent(m, message) <= 0.1f);
		}

		private static float GetUniquenessPercent(string message1, string message2)
		{
			return (float)LevenshteinDistance(message1, message2) / (float)Mathf.Max(message1.Length, message2.Length);
		}

		private async UniTaskVoid Punish()
		{
			_bannedMessages.Add(_lastUniqueMessage);
			Spamming = true;
			await UniTask.Delay(_spamPunishmentStrategy.GetNextPunishment());
			Spamming = false;
			_similarMessagesCount = 1;
		}

		private static int LevenshteinDistance(string s, string t)
		{
			int length = s.Length;
			int length2 = t.Length;
			int[,] array = new int[length + 1, length2 + 1];
			if (length == 0)
			{
				return length2;
			}
			if (length2 == 0)
			{
				return length;
			}
			int num = 0;
			while (num <= length)
			{
				array[num, 0] = num++;
			}
			int num2 = 0;
			while (num2 <= length2)
			{
				array[0, num2] = num2++;
			}
			for (int i = 1; i <= length; i++)
			{
				for (int j = 1; j <= length2; j++)
				{
					int num3 = ((t[j - 1] != s[i - 1]) ? 1 : 0);
					array[i, j] = Mathf.Min(array[i - 1, j] + 1, array[i, j - 1] + 1, array[i - 1, j - 1] + num3);
				}
			}
			return array[length, length2];
		}
	}
}
