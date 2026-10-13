using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class RequestWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Image CarIcon = default;

		[SerializeField]
		private Image RankIcon = default;

		[SerializeField]
		private TextMeshProUGUI Name = default;

		[SerializeField]
		private TextMeshProUGUI Power = default;

		[SerializeField]
		private TextMeshProUGUI Turbo = default;

		[SerializeField]
		private Button AcceptButton = default;

		[SerializeField]
		private Button IgnoreButton = default;

		[SerializeField]
		private Button DeclineButton = default;

		private bool _isClickToClose;

		private IPricesService _pricesService;

		private FreeDriveProvider _freeDriveProvider;

		private MultiPlayer _multiPlayer;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.CarOther;

		[Inject]
		public void Construct(ManagerUI managerUI, IPricesService pricesService, FreeDriveProvider freeDriveProvider, MultiPlayer multiPlayer)
		{
			_freeDriveProvider = freeDriveProvider;
			_managerUI = managerUI;
			_pricesService = pricesService;
			_multiPlayer = multiPlayer;
		}

		protected override void Start()
		{
			base.Start();
			_freeDriveProvider.FreeDriveControl.questionWindowRacing = base.gameObject;
			AcceptButton.onClick.AddListener(Accept);
			IgnoreButton.onClick.AddListener(Ignore);
			DeclineButton.onClick.AddListener(Decline);
			if (!_freeDriveProvider.RacingCar2 || !_freeDriveProvider.RacingCar2.TryGetComponent<VehicleData>(out var component))
			{
				Close();
				return;
			}
			MultiPersonControl multiPersonControl = _multiPlayer.GetMultiPersonControl(component.photonView.CreatorActorNr, checkVDriverPerson: true);
			if (!multiPersonControl)
			{
				Close();
				return;
			}
			_freeDriveProvider.RacingCar2.GetComponent<Powertrain>().CalcMaxPowerMaxTorque(out var maxHp, out var _, out var maxTq, out var _, out var _);
			fillData(multiPersonControl.rank2, multiPersonControl.ThisPlayerName, maxHp, maxTq, component.CarId);
			AddDoorListeners();
		}

		public void Back()
		{
			Decline();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			AcceptButton.onClick.RemoveAllListeners();
			IgnoreButton.onClick.RemoveAllListeners();
			DeclineButton.onClick.RemoveAllListeners();
			RemoveDoorListeners();
		}

		public void fillData(int rank, string enemyName, float power, float torque, int carId)
		{
			Name.text = enemyName + " " + new LocalizedString("RequestWindow.WantsRaceWithYou");
			Sprite sprite = UtilsRank.LoadRankSpriteFromResources((UtilsRank.RankType)rank);
			RankIcon.sprite = sprite;
			Power.text = $"{power:F0}";
			Turbo.text = $"{power:F0}";
			Sprite sprite2 = Resources.Load<Sprite>("UI/CarImages/b" + carId);
			CarIcon.sprite = sprite2;
		}

		private void Accept()
		{
			if (V.DriverPerson.TryGetComponent<MainPerson>(out var component) && (bool)component.PersonAnimationsController && component.PersonAnimationsController.AnimationService.IsAnimatingPlayer())
			{
				Decline();
				return;
			}
			if ((int)CurrencyManagerController.Money < (int)_pricesService.MultiplayerRaceLose)
			{
				Decline();
				_ = _managerUI.ShowNotificationWindow(new LocalizedString("RequestWindow.Youdon'thaveenoughmoney"), 3f);
				return;
			}
			RemoveDoorListeners();
			_isClickToClose = true;
			_freeDriveProvider.FreeDriveControl.InvitationAccept();
			Close();
		}

		private void Ignore()
		{
			_isClickToClose = true;
			_freeDriveProvider.FreeDriveControl.InvitationRejectIgnore();
			Close();
		}

		private void Decline()
		{
			_isClickToClose = true;
			_freeDriveProvider.FreeDriveControl.InvitationReject();
			Close();
		}

		private void AddDoorListeners()
		{
			_freeDriveProvider.QuickCarSettings.Door.AddListener(Decline);
			_freeDriveProvider.QuickCarSettings.DoorPassenger.AddListener(Decline);
		}

		private void RemoveDoorListeners()
		{
			_freeDriveProvider.QuickCarSettings.Door.RemoveListener(Decline);
			_freeDriveProvider.QuickCarSettings.DoorPassenger.RemoveListener(Decline);
		}

		public override void Close()
		{
			if (!base.IsCloseCanceled)
			{
				base.Close();
				if (!_isClickToClose)
				{
					_freeDriveProvider.FreeDriveControl.InvitationReject();
				}
			}
		}
	}
}
