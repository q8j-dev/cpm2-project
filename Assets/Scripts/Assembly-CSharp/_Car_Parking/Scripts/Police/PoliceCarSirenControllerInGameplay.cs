using UnityEngine;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarSirenControllerInGameplay : MonoBehaviour
	{
		public const string RpcCodeStartSiren = "1";

		public const string RpcCodeStopSiren = "0";

		[SerializeField]
		private PoliceCarSirenController PoliceCarSirenController;

		private VehicleData _vehicleData;

		public bool SirenActive => PoliceCarSirenController.SirenActive;

		public void Init(VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
		}

		public void SwitchSiren()
		{
			string text;
			if (SirenActive)
			{
				StopSiren();
				text = "0";
			}
			else
			{
				StartSiren();
				text = "1";
			}
			if ((bool)_vehicleData && _vehicleData.IsMultiplayerAndMine)
			{
				_vehicleData.FreeDriveProvider.MultiPlayer.SendValueStr<RpcPoliceSiren>(_vehicleData.photonView.ViewID.ToString(), text);
			}
		}

		public void StartSiren()
		{
			PoliceCarSirenController.StartSirens();
		}

		public void StopSiren()
		{
			PoliceCarSirenController.StopSirens();
		}
	}
}
