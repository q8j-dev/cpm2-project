using System.Collections.Generic;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public class Player
	{
		private int actorID = -1;

		public readonly bool IsLocal;

		private string nickName;

		public object TagObject;

		protected internal Room RoomReference { get; set; }

		public int ID => actorID;

		public string NickName
		{
			get
			{
				return nickName;
			}
			set
			{
				if (string.IsNullOrEmpty(nickName) || !nickName.Equals(value))
				{
					nickName = value;
					if (IsLocal && RoomReference != null && RoomReference.IsLocalClientInside)
					{
						SetPlayerNameProperty();
					}
				}
			}
		}

		public string UserId { get; internal set; }

		public bool IsMasterClient
		{
			get
			{
				if (RoomReference == null)
				{
					return false;
				}
				return ID == RoomReference.MasterClientId;
			}
		}

		public bool IsInactive { get; set; }

		public Hashtable CustomProperties { get; private set; }

		public Hashtable AllProperties
		{
			get
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Merge(CustomProperties);
				hashtable[byte.MaxValue] = nickName;
				return hashtable;
			}
		}

		protected internal Player(string nickName, int actorID, bool isLocal)
			: this(nickName, actorID, isLocal, null)
		{
		}

		protected internal Player(string nickName, int actorID, bool isLocal, Hashtable playerProperties)
		{
			IsLocal = isLocal;
			this.actorID = actorID;
			NickName = nickName;
			CustomProperties = new Hashtable();
			InternalCacheProperties(playerProperties);
		}

		public Player Get(int id)
		{
			if (RoomReference == null)
			{
				return null;
			}
			return RoomReference.GetPlayer(id);
		}

		public Player GetNext()
		{
			return GetNextFor(ID);
		}

		public Player GetNextFor(Player currentPlayer)
		{
			if (currentPlayer == null)
			{
				return null;
			}
			return GetNextFor(currentPlayer.ID);
		}

		public Player GetNextFor(int currentPlayerId)
		{
			if (RoomReference == null || RoomReference.Players == null || RoomReference.Players.Count < 2)
			{
				return null;
			}
			Dictionary<int, Player> players = RoomReference.Players;
			int num = int.MaxValue;
			int num2 = currentPlayerId;
			foreach (int key in players.Keys)
			{
				if (key < num2)
				{
					num2 = key;
				}
				else if (key > currentPlayerId && key < num)
				{
					num = key;
				}
			}
			if (num == int.MaxValue)
			{
				return players[num2];
			}
			return players[num];
		}

		public virtual void InternalCacheProperties(Hashtable properties)
		{
			if (properties == null || properties.Count == 0 || CustomProperties.Equals(properties))
			{
				return;
			}
			if (properties.ContainsKey(byte.MaxValue))
			{
				string text = (string)properties[byte.MaxValue];
				if (text != null)
				{
					if (IsLocal)
					{
						if (!text.Equals(nickName))
						{
							SetPlayerNameProperty();
						}
					}
					else
					{
						NickName = text;
					}
				}
			}
			if (properties.ContainsKey(253))
			{
				UserId = (string)properties[253];
			}
			if (properties.ContainsKey(254))
			{
				IsInactive = (bool)properties[254];
			}
			CustomProperties.MergeStringKeys(properties);
			CustomProperties.StripKeysWithNullValues();
		}

		public override string ToString()
		{
			return NickName + " " + SupportClass.DictionaryToString(CustomProperties);
		}

		public string ToStringFull()
		{
			return string.Format("#{0:00} '{1}'{2} {3}", ID, NickName, IsInactive ? " (inactive)" : "", CustomProperties.ToStringFull());
		}

		public override bool Equals(object p)
		{
			if (p is Player player)
			{
				return GetHashCode() == player.GetHashCode();
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ID;
		}

		protected internal void ChangeLocalID(int newID)
		{
			if (IsLocal)
			{
				actorID = newID;
			}
		}

		public void SetCustomProperties(Hashtable propertiesToSet, Hashtable expectedValues = null, WebFlags webFlags = null)
		{
			if (propertiesToSet != null)
			{
				Hashtable hashtable = propertiesToSet.StripToStringKeys();
				Hashtable hashtable2 = expectedValues.StripToStringKeys();
				if (hashtable2 == null || hashtable2.Count == 0)
				{
					CustomProperties.Merge(hashtable);
					CustomProperties.StripKeysWithNullValues();
				}
				if (RoomReference != null && RoomReference.IsLocalClientInside)
				{
					RoomReference.LoadBalancingClient.loadBalancingPeer.OpSetPropertiesOfActor(actorID, hashtable, hashtable2, webFlags);
				}
			}
		}

		private void SetPlayerNameProperty()
		{
			if (RoomReference != null && RoomReference.IsLocalClientInside)
			{
				Hashtable hashtable = new Hashtable();
				hashtable[byte.MaxValue] = nickName;
				RoomReference.LoadBalancingClient.loadBalancingPeer.OpSetPropertiesOfActor(ID, hashtable);
			}
		}
	}
}
