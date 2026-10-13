using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	public class LevelsProvider
	{
		public LevelScript LevelScript { get; set; }

		public LevelDataScript CurrentLevelData { get; set; }

		public TreeControll TreeControl { get; set; }

		public GameObject Trailer { get; set; }

		public GameObject RacingCarLevel { get; set; }

		public void TryClearProvider()
		{
			CurrentLevelData = null;
			Trailer = null;
			RacingCarLevel = null;
		}
	}
}
