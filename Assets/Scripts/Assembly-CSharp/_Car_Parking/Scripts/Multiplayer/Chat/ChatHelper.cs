using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public static class ChatHelper
	{
		public static string AddUrlPlayerId(string playerName, string playerId)
		{
			return "[url=" + playerId + "]" + playerName + "[/url]";
		}

		public static string RemoveRichText(string censuredEnteredText)
		{
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[b]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[B]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[u]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[U]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[i]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[I]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[S]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[s]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[sub]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[SUB]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[sup]");
			censuredEnteredText = RemoveBBCode(censuredEnteredText, "[SUP]");
			for (int i = 0; i < 40; i++)
			{
				censuredEnteredText = RemoveColor(censuredEnteredText, removeOnlyAlphaCode: true);
			}
			return censuredEnteredText;
		}

		public static string RemoveBBCode(string censuredEnteredText, string bbcode)
		{
			for (int i = 0; i < 50; i++)
			{
				if (censuredEnteredText.Contains(bbcode))
				{
					censuredEnteredText = censuredEnteredText.Replace(bbcode, "");
					continue;
				}
				return censuredEnteredText;
			}
			return censuredEnteredText;
		}

		public static string AddLineInChatString(string chatText, string message)
		{
			return CheckLength(chatText + "\n" + message);
		}

		private static string CheckLength(string str)
		{
			if (str.Split('\n').Length > 100)
			{
				int num = str.IndexOf(Environment.NewLine);
				str = str.Substring(num + Environment.NewLine.Length);
			}
			return str;
		}

		public static async UniTask DelayFrameAsync(Action actionBeforeFrame, Action actionAfterFrame, CancellationToken ct = default(CancellationToken))
		{
			if (actionBeforeFrame != null)
			{
				actionBeforeFrame?.Invoke();
			}
			await UniTask.Yield(ct);
			if (actionAfterFrame != null)
			{
				actionAfterFrame?.Invoke();
			}
		}

		public static string GetRandomString(int length = -1)
		{
			string text = "";
			if (length == -1)
			{
				length = UnityEngine.Random.Range(3, 30);
			}
			for (int i = 0; i < length; i++)
			{
				char c = (char)UnityEngine.Random.Range(48, 122);
				text = ((c >= 'F') ? (text + c) : (text + " "));
			}
			return text;
		}

		public static string CreateFormattedMessageString(string playerName, string input, bool isMyMessage)
		{
			string text = RemoveRichText(playerName);
			if (isMyMessage)
			{
				text = "#6aa84f" + text;
			}
			else
			{
				text = "[u]" + text + "[/u]";
				text = "#6aa84f" + text;
			}
			return text + ": #ffffff" + RemoveRichText(input);
		}

		public static string RemoveColor(string enteredText, bool removeOnlyAlphaCode = false)
		{
			if (enteredText.Contains("[") && enteredText.Contains("]"))
			{
				int num = enteredText.IndexOf('[');
				int num2 = enteredText.IndexOf(']');
				string text = enteredText.Substring(num, num2 - num + 1);
				if (removeOnlyAlphaCode)
				{
					if (text.Length == 4)
					{
						enteredText = enteredText.Replace(text, "");
					}
				}
				else
				{
					enteredText = enteredText.Replace(text, "");
				}
			}
			return enteredText;
		}

		public static string GetUrlTextAtCharacterIndex(int characterIndex)
		{
			string text = "";
			if (characterIndex != -1 && characterIndex < text.Length - 6)
			{
				int num = ((text[characterIndex] != '[' || text[characterIndex + 1] != 'u' || text[characterIndex + 2] != 'r' || text[characterIndex + 3] != 'l' || text[characterIndex + 4] != '=') ? text.LastIndexOf("[url=", characterIndex) : characterIndex);
				if (num == -1)
				{
					return null;
				}
				num += 5;
				int num2 = text.IndexOf("]", num);
				if (num2 == -1)
				{
					return null;
				}
				int num3 = text.IndexOf("[/url]", num2);
				if (num3 == -1 || characterIndex <= num3)
				{
					return text.Substring(num2 + 1, num3 - num2 - 1);
				}
			}
			return null;
		}
	}
}
