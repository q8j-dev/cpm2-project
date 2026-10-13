using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CPM.PlayerData;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.MenuUI.LanguagesServers;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class LobbyWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private SpawnPlaceItemUI[] SpawnPlaces = default;

		[SerializeField]
		private Button StartButton = default;

		[SerializeField]
		private Button AddFriendButton = default;

		[SerializeField]
		private Button BackButton = default;

		[SerializeField]
		private TextMeshProUGUI PlayersCount = default;

		[SerializeField]
		private TextMeshProUGUI PlayerName = default;

		[SerializeField]
		private TextMeshProUGUI PlayerId = default;

		[SerializeField]
		private TextMeshProUGUI RaceCount = default;

		[SerializeField]
		private TextMeshProUGUI WinCount = default;

		[SerializeField]
		private TextMeshProUGUI LoseCount = default;

		[SerializeField]
		private TMP_InputField FriendInputField = default;

		[SerializeField]
		private Transform FriendsListParent = default;

		[SerializeField]
		private Transform LoadingImage = default;

		[SerializeField]
		private GameObject FriendsListItemPrefab = default;

		[SerializeField]
		private Toggle ForFriendOnlyToggle = default;

		[SerializeField]
		private MultiplayerLobbyHandler LobbyHandler = default;

		[SerializeField]
		private TMP_Dropdown LanguageDropdown = default;

		[SerializeField]
		private LanguagesIconsConfig LanguagesSprites = default;

		[SerializeField]
		private Slider ProgressBar = default;

		[SerializeField]
		private TMP_InputField RoomPassword = default;

		[SerializeField]
		private CanvasGroup ConnectingPanel = default;

		[SerializeField]
		private CanvasGroup LocationPanel = default;

		[SerializeField]
		private CanvasGroup BottomPanel = default;

		public Button OnlinePlayerWarningButton;

		private List<FriendsListItemUI> _friendsList;

		private IPrefabFactory _prefabFactory;

		private PlayerDataStorage _playerDataStorage;

		private ICheckCarDataBase _checkCarDataBase;

		private LanguageIconsProvider _languageIconsProvider;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		public Action<SpawnPlaceItemUI> OnMultiplayerStarted { get; set; }

		public Action<string> OnNewFriendAdded { get; set; }

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, PlayerDataStorage playerDataStorage, ManagerUI managerUI, ICheckCarDataBase checkCarDataBase)
		{
			_prefabFactory = prefabFactory;
			_playerDataStorage = playerDataStorage;
			_managerUI = managerUI;
			_checkCarDataBase = checkCarDataBase;
		}

		protected override void Start()
		{
			base.Start();
			_languageIconsProvider = new LanguageIconsProvider(LanguagesSprites);
			InitBackButton();
			_managerUI.CloseWindow("UserBalanceWindow");
			StartButton.onClick.AddListener(StartMultiplayer);
			AddFriendButton.onClick.AddListener(AddNewFriend);
			BackButton.onClick.AddListener(Back);
			OnlinePlayerWarningButton.onClick.AddListener(ShowWarning);
			UpdatePlayerInfo();
			UpdateOnlinePlayersCount();
			InitLanguagesDropdown();
		}

		protected override void OnEnable()
		{
			BackButton.gameObject.SetActive(value: false);
			base.OnEnable();
		}

		private void InitLanguagesDropdown()
		{
			LanguageDropdown.options.Clear();
			foreach (TMP_Dropdown.OptionData languagesDropdown in _languageIconsProvider.GetLanguagesDropdownList())
			{
				LanguageDropdown.options.Add(languagesDropdown);
			}
			int num = PlayerPrefs.GetInt("languageIndex", (int)Application.systemLanguage);
			if (num == 40 || num == 41)
			{
				num = 6;
				PlayerPrefs.SetInt("languageIndex", 6);
			}
			if (_languageIconsProvider.Current == LanguageIconsProvider.CpmLanguages.Unknown)
			{
				string englishName = CultureInfo.CurrentUICulture.EnglishName;
				if (englishName.Contains("Malay") || englishName.Contains("malay"))
				{
					MonoBehaviour.print("Malay found!");
					num = 43;
					PlayerPrefs.SetInt("languageIndex", 43);
				}
			}
			_languageIconsProvider.Current = (LanguageIconsProvider.CpmLanguages)num;
			int valueWithoutNotify = _languageIconsProvider.IndexOfLanguage(num);
			LanguageDropdown.SetValueWithoutNotify(valueWithoutNotify);
			LanguageDropdown.onValueChanged.AddListener(LanguageChanged);
		}

		protected override void OnDestroy()
		{
			StartButton.onClick.RemoveAllListeners();
			AddFriendButton.onClick.RemoveAllListeners();
			BackButton.onClick.RemoveAllListeners();
			base.OnDestroy();
		}

		public override void ShowUI(bool state)
		{
			base.ShowUI(state);
			if ((bool)_canvasGroup)
			{
				_canvasGroup.interactable = state;
				_canvasGroup.blocksRaycasts = state;
			}
		}

		private void StartMultiplayer()
		{
			SpawnPlaceItemUI spawnPlaceItemUI = SpawnPlaces.FirstOrDefault((SpawnPlaceItemUI place) => place.IsChosen);
			if (!(spawnPlaceItemUI == null))
			{
				ShowUI(state: false);
				OnMultiplayerStarted?.Invoke(spawnPlaceItemUI);
			}
		}

		private void ShowWarning()
		{
			_managerUI.ShowWindow("OnlineWarningWindow", alwaysOnTop: true).Forget();
		}

		private void AddNewFriend()
		{
			string text = FriendInputField.text;
			if (!string.IsNullOrEmpty(text))
			{
				FriendInputField.text = "";
				OnNewFriendAdded?.Invoke(text);
			}
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_managerUI.ShowGameModeWindow();
			Close();
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.Disconnect();
				Debug.Log("Was Connect to photon - DISCONNECT");
			}
		}

		public void UpdateOnlinePlayersCount()
		{
			int countOfPlayers = PhotonNetwork.CountOfPlayers;
			PlayersCount.text = ((countOfPlayers != 0) ? countOfPlayers.ToString() : "");
		}

		private void UpdatePlayerInfo()
		{
			int num = ObscuredPrefs.GetInt("RaceWin");
			int num2 = ObscuredPrefs.GetInt("RaceLose");
			int num3 = num + num2;
			float value = 0f;
			if (num3 != 0)
			{
				value = (float)num / (float)num3;
			}
			PlayerName.text = _playerDataStorage.PlayerName;
			PlayerId.text = _playerDataStorage.PlayerID;
			WinCount.text = num.ToString();
			LoseCount.text = num2.ToString();
			RaceCount.text = num3.ToString();
			ProgressBar.value = value;
		}

		private void InitFriendItem(PrefFriendData friend)
		{
			string id = friend.Name;
			int num = 0;
			if (friend.Name == "")
			{
				id = friend.id;
			}
			FriendsListItemUI component = _prefabFactory.Instantiate(FriendsListItemPrefab, FriendsListParent).GetComponent<FriendsListItemUI>();
			component.Init(friend.id, id, LobbyHandler);
			string status = friend.Status;
			if (status == "online" || status == "inroom")
			{
				status = friend.Status;
				if (!(status == "online"))
				{
					if (status == "inroom")
					{
						component.SetRoomInfo(friend.RoomName, 20);
						if (ArenaRoomNames.TryParseArenaId(friend.RoomName, out var arenaId))
						{
							component.SetArenaInfo(arenaId);
						}
					}
				}
				else
				{
					component.SetOnline();
				}
				if (friend.RoomName != null)
				{
					num = int.Parse(friend.RoomName.Split("_"[0])[1]);
					component.PassWord = ((num <= 10000) ? num : 0);
				}
			}
			_friendsList.Add(component);
		}

		public void HideConnectingPanel()
		{
			ConnectingPanel.DOFade(0f, 0.1f).OnComplete(() =>
			{
				ConnectingPanel.gameObject.SetActive(value: false);
			});
			LocationPanel.DOFade(1f, 0.1f);
			BottomPanel.DOFade(1f, 0.1f);
		}

		public void ShowLoading()
		{
			LoadingImage.gameObject.SetActive(value: true);
			ShortcutExtensions.DOLocalRotate(endValue: new Vector3(0f, 0f, 360f), target: LoadingImage, duration: 1.5f).SetRelative().SetLoops(-1, LoopType.Incremental)
				.SetEase(Ease.Linear);
		}

		public void HideLoading()
		{
			LoadingImage.DOKill();
			LoadingImage.gameObject.SetActive(value: false);
		}

		public void UpdateFriendsList(List<PrefFriendData> friendOnlineList)
		{
			_friendsList = new List<FriendsListItemUI>();
			foreach (PrefFriendData friendOnline in friendOnlineList)
			{
				InitFriendItem(friendOnline);
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(FriendsListParent.GetComponent<RectTransform>());
		}

		public void ClearFriendsList()
		{
			List<FriendsListItemUI> friendsList = _friendsList;
			if (friendsList == null || friendsList.Count <= 0)
			{
				return;
			}
			foreach (FriendsListItemUI friends in _friendsList)
			{
				UnityEngine.Object.Destroy(friends.gameObject);
			}
			_friendsList.Clear();
			LayoutRebuilder.ForceRebuildLayoutImmediate(FriendsListParent.GetComponent<RectTransform>());
		}

		public bool IsForFriendOnly()
		{
			return ForFriendOnlyToggle.isOn;
		}

		public int GetLobbyPassword()
		{
			int result = -1;
			if (!string.IsNullOrEmpty(RoomPassword.text) && int.TryParse(RoomPassword.text, out result))
			{
				result = Mathf.Abs(result);
			}
			return result;
		}

		private void LanguageChanged(int dropDownLanguageIndex)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.SmallClickSwitch, base.gameObject);
			LanguageIconsProvider.CpmLanguages language = _languageIconsProvider.GetLanguage(dropDownLanguageIndex);
			if (language != _languageIconsProvider.Current)
			{
				_languageIconsProvider.Current = language;
				LobbyHandler.SetLanguage(language);
			}
		}
	}
}
