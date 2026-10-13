using Car_Parking.Scripts.Racing;
using Photon.Pun;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcInvitationReceived : RpcObjectBase
	{
		public override byte Code => 7;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			if (!multiPlayerPosControl.IsMyVcarAndNotPassenger(b))
			{
				return;
			}
			FreeDriveProvider freeDriveProvider = multiPlayerPosControl.FreeDriveProvider;
			FreeDriveControl freeDriveControl = freeDriveProvider.FreeDriveControl;
			bool num = !freeDriveControl.newNegotiationWindow && !freeDriveControl.questionWindowRacing && !freeDriveControl.notificationCircleRacing && !FlagsStorage.ChasingRaceModeStarted && !freeDriveControl.InvitationJustSend && (bool)multiPlayerPosControl.VehicleData && !freeDriveProvider.RacingCar2 && !multiPlayerPosControl.VehicleData.Towing && !freeDriveProvider.MultiPlayer.ignoreRacingRequest && !FlagsStorage.RaceStarted && !freeDriveControl.isSelectingCar && V.CoreGameManager.ManagerUI.IsParkingMenuActive;
			bool flag = V.DriverPerson.TryGetComponent<MainPerson>(out var component) && (bool)component.PersonAnimationsController && component.PersonAnimationsController.AnimationService.IsAnimatingPlayer();
			bool flag2 = ((bool)freeDriveProvider.FirstCarMultiPlayer && (bool)freeDriveProvider.FirstCarMultiPlayer.GetComponent<FreeRaceCar>()) || (freeDriveControl.TimeAttackController != null && freeDriveControl.TimeAttackController.RaceStatus == TimeAttackStatus.Prepare);
			if (num && !flag && !flag2)
			{
				PhotonView photonView = multiPlayerPosControl.CarComponents.PhotonView;
				freeDriveControl.LastInvitationReceivedId = photonView.Owner.ActorNumber;
				freeDriveProvider.RacingCar2 = multiPlayerPosControl.gameObject;
				string nickName = photonView.Owner.NickName;
				if ((bool)multiPlayerPosControl.VehicleData.Driver)
				{
					nickName = multiPlayerPosControl.VehicleData.Driver.GetComponent<PhotonView>().Owner.NickName;
				}
				multiPlayerPosControl.CarComponents.Powertrain.CalcMaxPowerMaxTorque(out var maxHp, out var _, out var _, out var _, out var _);
				string text = new LocalizedString("RequestWindow.WantsRaceWithYou");
				string text2 = new LocalizedString("Utils.hp");
				V.CoreGameManager.SetMessage($"{nickName} ({maxHp:F0} {text2}) {text.Trim()}", 3f);
				NewRacingMultiplayerController newRacingMultiplayerController = freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController;
				newRacingMultiplayerController.CurrentRacingTree = newRacingMultiplayerController.TreeList[int.Parse(name)];
				freeDriveControl.InvitationReceived();
			}
			else
			{
				PhotonView photonView2 = multiPlayerPosControl.CarComponents.PhotonView;
				V.CoreGameManager.gameObject.GetComponent<MultiPlayer>().SendValueStr(11, photonView2.Owner.ActorNumber.ToString(), "");
			}
		}
	}
}
