using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class PlayerStatusPrefsSaver : IAllPrefsSaver
	{
		private int _status;

		public void Save()
		{
			_status = PlayerPrefs.GetInt("GameModePlayed01", 0);
		}

		public void Load()
		{
			PlayerPrefs.SetInt("GameModePlayed01", _status);
		}
	}
}
