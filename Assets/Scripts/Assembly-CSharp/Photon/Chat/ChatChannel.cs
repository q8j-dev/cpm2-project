using System.Collections.Generic;
using System.Text;

namespace Photon.Chat
{
	public class ChatChannel
	{
		public readonly string Name;

		public readonly List<string> Senders = new List<string>();

		public readonly List<object> Messages = new List<object>();

		public int MessageLimit;

		public int ChannelID;

		private Dictionary<object, object> properties;

		public readonly HashSet<string> Subscribers = new HashSet<string>();

		public bool IsPrivate { get; protected internal set; }

		public int MessageCount => Messages.Count;

		public int LastMsgId { get; protected set; }

		public bool PublishSubscribers { get; protected set; }

		public int MaxSubscribers { get; protected set; }

		public ChatChannel(string name)
		{
			Name = name;
		}

		public void Add(string sender, object message, int msgId)
		{
			Senders.Add(sender);
			Messages.Add(message);
			LastMsgId = msgId;
			TruncateMessages();
		}

		public void Add(string[] senders, object[] messages, int lastMsgId)
		{
			Senders.AddRange(senders);
			Messages.AddRange(messages);
			LastMsgId = lastMsgId;
			TruncateMessages();
		}

		public void TruncateMessages()
		{
			if (MessageLimit > 0 && Messages.Count > MessageLimit)
			{
				int count = Messages.Count - MessageLimit;
				Senders.RemoveRange(0, count);
				Messages.RemoveRange(0, count);
			}
		}

		public void ClearMessages()
		{
			Senders.Clear();
			Messages.Clear();
		}

		public string ToStringMessages()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < Messages.Count; i++)
			{
				stringBuilder.AppendLine($"{Senders[i]}: {Messages[i]}");
			}
			return stringBuilder.ToString();
		}

		internal void ReadChannelProperties(Dictionary<object, object> newProperties)
		{
			if (newProperties == null || newProperties.Count <= 0)
			{
				return;
			}
			if (properties == null)
			{
				properties = new Dictionary<object, object>(newProperties.Count);
			}
			foreach (KeyValuePair<object, object> newProperty in newProperties)
			{
				if (newProperty.Value == null)
				{
					properties.Remove(newProperty.Key);
				}
				else
				{
					properties[newProperty.Key] = newProperty.Value;
				}
			}
			if (properties.TryGetValue((byte)254, out var value))
			{
				PublishSubscribers = (bool)value;
			}
			if (properties.TryGetValue(byte.MaxValue, out value))
			{
				MaxSubscribers = (int)value;
			}
		}

		internal void AddSubscribers(string[] users)
		{
			if (users != null)
			{
				for (int i = 0; i < users.Length; i++)
				{
					Subscribers.Add(users[i]);
				}
			}
		}
	}
}
