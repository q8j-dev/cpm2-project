using System.Linq;
using CPM.PlayerData;
using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using MVPFree.MapIcons.ScrObjs.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Drag
{
	public class TimeAttackMapRacePopupWindow : WindowScript
	{
		[SerializeField]
		private Button LeaderBoardButton;

		[SerializeField]
		private TextMeshProUGUI TitleText;

		[SerializeField]
		private TextMeshProUGUI DescriptionText;

		[SerializeField]
		private TextMeshProUGUI RecordText;

		[SerializeField]
		private Transform EventPanelParent;

		private TimeBannersManager _timeBannersManager;

		private PlayerDataStorage _playerDataStorage;

		private int _currentTrackIndex;

		private IEventService _eventService;

		private _Car_Parking.Scripts.EventSystem.Data.Event _currentEvent;

		private FreeDriveProvider _freeDriveProvider;

		private TimeAttackDataSO _timeAttackDataSo;

		private EventRegistry _eventRegistry;

		private RegistryHandle<BoolEvent> _onMapMoveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnMapMove);

		private RegistryHandle<Vector2ValueEvent> _mapDragMoveValueEventHandle = new RegistryHandle<Vector2ValueEvent>(EventRegistryIDs.MapDragMoveValueEvent);

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(TimeBannersManager timeBannersManager, PlayerDataStorage playerDataStorage, TimeAttackDataSO timeAttackDataSo, IEventService eventService, FreeDriveProvider freeDriveProvider, EventRegistry eventRegistry)
		{
			_timeBannersManager = timeBannersManager;
			_playerDataStorage = playerDataStorage;
			_timeAttackDataSo = timeAttackDataSo;
			_eventService = eventService;
			_freeDriveProvider = freeDriveProvider;
			_eventRegistry = eventRegistry;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_onMapMoveHandle.Get(_eventRegistry)?.Subscribe(OnMapMove);
		}

		private void OnDisable()
		{
			_onMapMoveHandle.Get(_eventRegistry)?.Unsubscribe(OnMapMove);
		}

		private void OnMapMove(bool isStarted)
		{
			if (isStarted)
			{
				Close();
			}
		}

		protected override void Start()
		{
			base.Start();
			LeaderBoardButton.onClick.AddListener(() =>
			{
				ShowLeaderBoard().Forget();
			});
		}

		private async UniTaskVoid ShowLeaderBoard()
		{
			(await _managerUI.CreateWindowGeneric<UIMapLeaderboardWindow>("UIMapLeaderboardWindow", onTop: true)).Init(_currentTrackIndex);
		}

		public void Init(int index)
		{
			_currentTrackIndex = index;
			FillTexts(index);
		}

		private void FillTexts(int index)
		{
			TimeAttackData timeAttackData = _timeAttackDataSo.TimeAttackDatas.FirstOrDefault((TimeAttackData x) => x.TrackIndex == index);
			if (timeAttackData == null)
			{
				Close();
				return;
			}
			TitleText.text = new LocalizedString(timeAttackData.Name ?? "");
			DescriptionText.text = new LocalizedString(timeAttackData.Description ?? "");
			string personFormattedResult = _timeBannersManager.GetTimeBannersManager(index).GetPersonFormattedResult(_playerDataStorage.PlayerID, timeAttackData.TimeAttackMode);
			bool flag = !string.IsNullOrEmpty(personFormattedResult);
			RecordText.transform.parent.gameObject.SetActive(flag);
			if (flag)
			{
				string text = new LocalizedString("TimeAttack.PersonalRecord");
				RecordText.text = text + ": " + personFormattedResult;
			}
			ShowEventBannerAsync(index).Forget();
		}

		private async UniTaskVoid ShowEventBannerAsync(int index)
		{
			_currentEvent = _eventService.GetEventByName(_timeAttackDataSo.TimeAttackDatas[index].EventType);
			if (_timeAttackDataSo.TimeAttackDatas[index].TimeAttackMode != TimeAttackMode.Rally && _currentEvent != null)
			{
				EventMapPopupPanel eventMapPopupPanel = await _managerUI.CreateWindowGeneric<EventMapPopupPanel>($"{_currentEvent.Type}EventMapPanel", onTop: false);
				if (eventMapPopupPanel != null)
				{
					eventMapPopupPanel.transform.SetParent(EventPanelParent);
					eventMapPopupPanel.Init(_currentEvent);
				}
			}
		}

		private void Update()
		{
			if (_freeDriveProvider.MapCamera.beganTouching || !_freeDriveProvider.MapCamera.ShowingBigMap)
			{
				Close();
			}
		}
	}
}
