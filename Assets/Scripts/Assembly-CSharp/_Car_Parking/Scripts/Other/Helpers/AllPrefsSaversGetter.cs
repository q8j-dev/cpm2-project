using System.Collections.Generic;
using CPM.Account;
using UnityEngine.Scripting;
using _Car_Parking.Garage.Poses;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.EditableHUD;
using _Car_Parking.Scripts.MenuUI;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	public class AllPrefsSaversGetter : IPrefsSaversGetter
	{
		public List<IAllPrefsSaver> GetPrefsSavers()
		{
			return new List<IAllPrefsSaver>
			{
				new EditablePrefsSaver(),
				new FirebaseCarsServicePrefsSaver(),
				new NewCarsMarkController(),
				new PlayerStatusPrefsSaver(),
				new SettingsPrefsSaver(),
				new GarageCarPoseStash()
			};
		}
	}
}
