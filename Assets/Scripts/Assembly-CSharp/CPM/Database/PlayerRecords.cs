using System;
using System.Collections.Generic;

namespace CPM.Database
{
	[Serializable]
	public class PlayerRecords
	{
		public string Name;

		public int money;

		public int coin;

		public string localID;

		public List<int> boughtFsos;

		public List<PrefFriendItem> FriendsID;

		public List<float> LevelsDoneTime;

		public List<float> floats;

		public List<int> integers;

		public List<int> fcar;

		public EquipmentsData personEquipmentsMale;

		public EquipmentsData personEquipmentsFemale;

		public CarIDnStatus carIDnStatus;

		public string allData;

		public Dictionary<int, int> flags;

		public List<int> animations;

		public PlayerRecords()
		{
		}

		public PlayerRecords(string Name, int money, int coin, List<PrefFriendItem> FriendsID, CarIDnStatus carIDnStatus, List<float> levelsDoneTime, List<float> floats, List<int> ints, List<int> fcar, EquipmentsData equipmentMale, EquipmentsData equipmentFemale, string localId, Dictionary<int, int> flags, List<int> animations, List<int> boughtFsos)
		{
			this.Name = Name;
			this.money = money;
			this.coin = coin;
			this.FriendsID = FriendsID;
			localID = localId;
			LevelsDoneTime = levelsDoneTime;
			this.floats = floats;
			integers = ints;
			this.fcar = fcar;
			personEquipmentsMale = equipmentMale;
			personEquipmentsFemale = equipmentFemale;
			this.carIDnStatus = carIDnStatus;
			this.flags = flags;
			this.animations = animations;
			this.boughtFsos = boughtFsos;
		}
	}
}
