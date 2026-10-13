using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public sealed class ArenaLobbyWindow : WindowScript
	{
		[SerializeField]
		private TextMeshProUGUI Title;

		[SerializeField]
		private TextMeshProUGUI TimeText;

		[SerializeField]
		private TextMeshProUGUI StatusText;

		[SerializeField]
		private TextMeshProUGUI PlayersText;

		[SerializeField]
		private TextMeshProUGUI PlayersMinText;

		[SerializeField]
		private Button StartNowButton;

		[SerializeField]
		private Color SearchPlayerColor;

		[SerializeField]
		private Color MatchGuaranteedColor;

		private IArenaNetwork _net;

		private ArenaModeRegistry _registry;

		private FreeDriveProvider _freeDriveProvider;

		private ArenaFlowController _flow;

		private HudMenuAutoHide _menuAutoHide;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Person;

		protected override bool ChangeRootGameInputGroup => true;

		private bool MatchIsGuaranteed
		{
			get
			{
				if (_net == null)
				{
					return false;
				}
				if (_net.ReadLobbyState().Phase != ArenaPhase.InProgress && _net.ReadLobbyState().Phase != ArenaPhase.Returning)
				{
					return _net.ReadLobbyState().Phase == ArenaPhase.Countdown;
				}
				return true;
			}
		}

		[Inject]
		public void Construct(IArenaNetwork net, ArenaModeRegistry registry, FreeDriveProvider freeDriveProvider, ArenaFlowController flow)
		{
			_net = net;
			_registry = registry;
			_freeDriveProvider = freeDriveProvider;
			_flow = flow;
		}

		protected override void Start()
		{
			base.Start();
			if (_net != null)
			{
				_net.OnRoomStateChanged += Refresh;
			}
			LocalizationManager.LocalizationChanged += Refresh;
			if ((bool)StartNowButton)
			{
				StartNowButton.onClick.AddListener(OnStartNowClicked);
			}
			_menuAutoHide = new HudMenuAutoHide(this, _managerUI);
			Refresh();
		}

		protected override void OnDestroy()
		{
			if (_net != null)
			{
				_net.OnRoomStateChanged -= Refresh;
			}
			LocalizationManager.LocalizationChanged -= Refresh;
			if ((bool)StartNowButton)
			{
				StartNowButton.onClick.RemoveListener(OnStartNowClicked);
			}
			_menuAutoHide?.Dispose();
			base.OnDestroy();
		}

		private void OnStartNowClicked()
		{
			_flow?.ForceStartCountdown();
			Refresh();
		}

		private void Update()
		{
			if ((bool)TimeText && _net != null)
			{
				TimeText.text = TimeFormatter.FormatMinutesSeconds(Mathf.RoundToInt((float)_net.TimeEndDeadline)) ?? "";
			}
		}

		private void Refresh()
		{
			if (_net == null)
			{
				return;
			}
			_net.ReadLobbyState();
			ArenaId arenaId = ((_freeDriveProvider != null) ? _freeDriveProvider.RoomArenaId : ArenaId.Free);
			if ((bool)Title)
			{
				Title.text = (_registry.TryGetActivityTitle(arenaId, out var title) ? ((string)title) : "Lobby");
			}
			if ((bool)StatusText)
			{
				if (MatchIsGuaranteed)
				{
					StatusText.text = new LocalizedString("Lobby.MatchIsGuaranteed");
					StatusText.color = MatchGuaranteedColor;
				}
				else
				{
					StatusText.text = new LocalizedString("Lobby.SearchForPlayers");
					StatusText.color = SearchPlayerColor;
				}
			}
			int num = 0;
			int num2 = 0;
			if (_registry != null && _registry.TryGet(arenaId, out var _, out var config) && (bool)config)
			{
				num = config.MinPlayers;
				num2 = config.MaxPlayers;
			}
			if ((bool)PlayersText)
			{
				string s = $"{_net.PlayerCount}";
				s = s.Colored(MatchIsGuaranteed ? MatchGuaranteedColor : SearchPlayerColor);
				PlayersText.text = string.Format("{0} {1}/{2}", new LocalizedString("Lobby.PlayersNotFormat"), s, num2);
			}
			if ((bool)PlayersMinText)
			{
				PlayersMinText.text = string.Format(new LocalizedString("Lobby.MinimumToStart"), num);
			}
			if ((bool)StartNowButton)
			{
				StartNowButton.gameObject.SetActive(_flow?.CanForceStartCountdown ?? false);
			}
		}
	}
}
