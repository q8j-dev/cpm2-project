using System.Collections.Generic;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhoneGameActivityFindRace : NavCanvas
	{
		public const string WindowId = "FindRace";

		[SerializeField]
		private Button QuickMatchButton;

		[SerializeField]
		private Button CreateMatchButton;

		[SerializeField]
		private Button MainMenuButton;

		[SerializeField]
		private GameObject FreeDriveRoot;

		[SerializeField]
		private GameObject MainMenuRoot;

		[SerializeField]
		private Transform Content;

		[SerializeField]
		private JoinToFriendElementView JoinToFriendElementViewPrefab;

		[SerializeField]
		private GameObject NoFriendsHint;

		private ArenaFlowController _arenaFlowController;

		private GameActivityEntry _entry;

		private ManagerUI _managerUI;

		private OptionalDependency<MenuLevelParent> _menuLevelParent;

		private ExitToMainMenuService _exitToMainMenu;

		private PlayerDatas.FriendsUtility _friendsUtility;

		private PhotonReconnectionHandler _reconnect;

		private ArenaFriendsPresenceService _presence;

		private readonly Dictionary<string, JoinToFriendElementView> _rowsByKey = new Dictionary<string, JoinToFriendElementView>();

		private readonly HashSet<string> _liveKeys = new HashSet<string>();

		private readonly List<string> _staleKeys = new List<string>();

		private bool _presenceStarted;

		private bool _entryReceived;

		public override string Id => "FindRace";

		public override string Title => new LocalizedString("Lobby.FindRace");

		[Inject]
		public void Construct(ArenaFlowController arenaFlowController, ManagerUI managerUI, OptionalDependency<MenuLevelParent> menuLevelParent, ExitToMainMenuService exitToMainMenu, PlayerDatas.FriendsUtility friendsUtility, PhotonReconnectionHandler reconnect)
		{
			_arenaFlowController = arenaFlowController;
			_managerUI = managerUI;
			_menuLevelParent = menuLevelParent;
			_exitToMainMenu = exitToMainMenu;
			_friendsUtility = friendsUtility;
			_reconnect = reconnect;
		}

		public override void Open(object data)
		{
			base.Open(data);
			_entry = (GameActivityEntry)data;
			_entryReceived = true;
			StartPresenceIfMenu();
		}

		protected override void Start()
		{
			base.Start();
			MainMenuRoot.SetActive(ScenesHelper.IsMenuSceneActive());
			FreeDriveRoot.SetActive(!ScenesHelper.IsMenuSceneActive());
			StartPresenceIfMenu();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			QuickMatchButton.onClick.AddListener(OnQuickMatchClicked);
			CreateMatchButton.onClick.AddListener(OnCreateMatchClicked);
			MainMenuButton.onClick.AddListener(OnMainMenuClicked);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			QuickMatchButton.onClick.RemoveListener(OnQuickMatchClicked);
			CreateMatchButton.onClick.RemoveListener(OnCreateMatchClicked);
			MainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
			StopPresence(disconnect: true);
		}

		private void StartPresenceIfMenu()
		{
			if (!_presenceStarted && _entryReceived && ScenesHelper.IsMenuSceneActive())
			{
				_presenceStarted = true;
				_presence = new ArenaFriendsPresenceService(_friendsUtility, _reconnect);
				_presence.OnPresenceUpdated += RenderPresence;
				_presence.Start((ArenaId)_entry.ActivityId);
			}
		}

		private static string RowKey(ArenaFriendPresence p)
		{
			return p.AccountId + "|" + p.RoomName;
		}

		private void RenderPresence()
		{
			IReadOnlyList<ArenaFriendPresence> current = _presence.Current;
			_liveKeys.Clear();
			foreach (ArenaFriendPresence item in current)
			{
				_liveKeys.Add(RowKey(item));
			}
			_staleKeys.Clear();
			foreach (string key2 in _rowsByKey.Keys)
			{
				if (!_liveKeys.Contains(key2))
				{
					_staleKeys.Add(key2);
				}
			}
			foreach (string staleKey in _staleKeys)
			{
				JoinToFriendElementView joinToFriendElementView = _rowsByKey[staleKey];
				if ((bool)joinToFriendElementView)
				{
					joinToFriendElementView.OnJoinConfirmed -= OnRowJoinConfirmed;
					Object.Destroy(joinToFriendElementView.gameObject);
				}
				_rowsByKey.Remove(staleKey);
			}
			foreach (ArenaFriendPresence item2 in current)
			{
				string key = RowKey(item2);
				if (_rowsByKey.TryGetValue(key, out var value))
				{
					value.RefreshStatus(item2);
					continue;
				}
				JoinToFriendElementView joinToFriendElementView2 = Object.Instantiate(JoinToFriendElementViewPrefab, Content);
				joinToFriendElementView2.SetPresence(item2);
				joinToFriendElementView2.OnJoinConfirmed += OnRowJoinConfirmed;
				_rowsByKey[key] = joinToFriendElementView2;
			}
			if ((bool)NoFriendsHint)
			{
				NoFriendsHint.SetActive(current.Count == 0);
			}
		}

		private void OnRowJoinConfirmed(JoinToFriendElementView row)
		{
			string roomName = row.RoomName;
			StopPresence(disconnect: false);
			_arenaFlowController.JoinArenaRoom((ArenaId)_entry.ActivityId, roomName, ArenaEntrySource.JoinFriend);
		}

		private void ClearRows()
		{
			foreach (JoinToFriendElementView value in _rowsByKey.Values)
			{
				if ((bool)value)
				{
					value.OnJoinConfirmed -= OnRowJoinConfirmed;
					Object.Destroy(value.gameObject);
				}
			}
			_rowsByKey.Clear();
		}

		private void StopPresence(bool disconnect)
		{
			if (_presenceStarted)
			{
				_presenceStarted = false;
				if (_presence != null)
				{
					_presence.OnPresenceUpdated -= RenderPresence;
					_presence.Stop(disconnect);
					_presence = null;
				}
				ClearRows();
			}
		}

		private void OnQuickMatchClicked()
		{
			ArenaId arenaId = (ArenaId)_entry.ActivityId;
			StopPresence(disconnect: false);
			_arenaFlowController.StartArena(arenaId, ArenaEntrySource.QuickMatch);
		}

		private void OnCreateMatchClicked()
		{
			StopPresence(disconnect: true);
			base.Navigator.OpenWindow("CreateMatch", _entry);
		}

		private void OnMainMenuClicked()
		{
			_managerUI.ShowConfirmWindow(() =>
			{
				_exitToMainMenu.Exit(_menuLevelParent.Value, "quit_manual");
			}, new LocalizedString("SettingsWindow.DoYouWantToReturnToMainMenu"), "", () =>
			{
				_managerUI.CloseWindow("ConfirmWindow");
				Show();
			}).Forget();
		}
	}
}
