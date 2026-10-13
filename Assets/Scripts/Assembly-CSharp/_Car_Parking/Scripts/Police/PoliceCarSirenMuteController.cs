using UniRx;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarSirenMuteController
	{
		public const string RpcCodeMute = "0";

		public const string RpcCodeUnMute = "1";

		public ReactiveProperty<bool> IsMute = new ReactiveProperty<bool>(initialValue: false);

		private VehicleData _vehicleData;

		public void Init(VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
		}

		public void SwitchMute()
		{
			if (IsMute.Value)
			{
				SetUnmute();
			}
			else
			{
				SetMute();
			}
			if ((bool)_vehicleData && _vehicleData.IsMultiplayerAndMine)
			{
				_vehicleData.FreeDriveProvider.MultiPlayer.SendValueStr<RpcPoliceMute>(_vehicleData.photonView.ViewID.ToString(), IsMute.Value ? "0" : "1");
			}
		}

		public void SetMute()
		{
			IsMute.Value = true;
		}

		public void SetUnmute()
		{
			IsMute.Value = false;
		}
	}
}
