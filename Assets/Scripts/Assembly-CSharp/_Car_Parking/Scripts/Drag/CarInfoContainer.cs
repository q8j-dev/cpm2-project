using CPM.Database;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.Drag.Engine;

namespace _Car_Parking.Scripts.Drag
{
	public class CarInfoContainer : MonoBehaviour
	{
		public CarInfo CarInfo;

		public MultiplayerDataSending.CarVinyls VinylsData;

		public bool CarBodyWasLoaded;

		public VehicleDataResult vehicleDataResult;
	}
}
