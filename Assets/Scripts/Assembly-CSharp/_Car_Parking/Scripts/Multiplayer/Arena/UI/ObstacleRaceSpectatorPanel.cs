using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public sealed class ObstacleRaceSpectatorPanel : MonoBehaviour, IBackableWindow
	{
		[SerializeField]
		private Button WatchButton;

		[SerializeField]
		private Button BackButton;

		[SerializeField]
		private Button NextButton;

		[SerializeField]
		private Button PrevButton;

		[Tooltip("Arrows and target name; shown only while watching.")]
		[SerializeField]
		private GameObject WatchingRoot;

		[Tooltip("The standings table, collapsed while watching so the runner is visible.")]
		[SerializeField]
		private GameObject TableRoot;

		[SerializeField]
		private TextMeshProUGUI TargetNameText;

		[SerializeField]
		private TextMeshProUGUI TargetCounterText;

		[SerializeField]
		[Tooltip("Must NOT live under TableRoot: the list is how the viewer switches target while watching.")]
		[Header("Standings list (optional — assign only what the prefab has)")]
		private ObstacleRaceSpectatorListView StandingsList;

		[SerializeField]
		private ObstacleRaceSpectatorRow LocalRow;

		[SerializeField]
		private TextMeshProUGUI PlaceText;

		[SerializeField]
		private TextMeshProUGUI PlaceTotalText;

		private bool _available;

		private bool _watching;

		private EventRegistry _eventRegistry;

		private InputMapSetEvent _inputMapSetEvent;

		private ManagerUI _managerUI;

		private RegistryHandle<BoolEvent> _onNextPreviousHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnNextPrevious);

		private DesktopControlsSettingsSO _controlsSettings;

		public EventRegistry EventRegistry => _eventRegistry;

		public DesktopControlsSettingsSO ControlsSettings => _controlsSettings;

		public bool IsWatching => _watching;

		public event Action WatchRequested;

		public event Action BackRequested;

		public event Action NextRequested;

		public event Action PrevRequested;

		public event Action<int> RowClicked;

		[Inject]
		public void Construct(EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, ManagerUI managerUI, OptionalDependency<DesktopControlsSettingsSO> controlsSettingsDep)
		{
			_eventRegistry = eventRegistry;
			_inputMapSetEvent = inputMapSetEvent;
			_managerUI = managerUI;
			if (controlsSettingsDep.HasValue)
			{
				_controlsSettings = controlsSettingsDep.Value;
			}
		}

		private void Awake()
		{
			if ((bool)WatchButton)
			{
				WatchButton.onClick.AddListener(OnWatchClicked);
			}
			if ((bool)BackButton)
			{
				BackButton.onClick.AddListener(OnBackClicked);
			}
			if ((bool)NextButton)
			{
				NextButton.onClick.AddListener(OnNextClicked);
			}
			if ((bool)PrevButton)
			{
				PrevButton.onClick.AddListener(OnPrevClicked);
			}
			if ((bool)StandingsList)
			{
				StandingsList.RowClicked += OnRowClicked;
			}
			HideStandings();
			Apply();
		}

		private void OnDestroy()
		{
			if ((bool)WatchButton)
			{
				WatchButton.onClick.RemoveListener(OnWatchClicked);
			}
			if ((bool)BackButton)
			{
				BackButton.onClick.RemoveListener(OnBackClicked);
			}
			if ((bool)NextButton)
			{
				NextButton.onClick.RemoveListener(OnNextClicked);
			}
			if ((bool)PrevButton)
			{
				PrevButton.onClick.RemoveListener(OnPrevClicked);
			}
			if ((bool)StandingsList)
			{
				StandingsList.RowClicked -= OnRowClicked;
			}
		}

		public void SetWatchAvailable(bool available)
		{
			_available = available;
			Apply();
		}

		public void SetWatching(bool watching)
		{
			_watching = watching;
			_inputMapSetEvent.TrySetGroup((!watching) ? InputGroupsEnum.Ui : InputGroupsEnum.Spectator, setRoot: true);
			if (watching)
			{
				InitBackButton();
			}
			Apply();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
			BackButton.SetActive(active: false);
			_onNextPreviousHandle.Get(_eventRegistry).Unsubscribe(OnNextPrevious);
			_onNextPreviousHandle.Get(_eventRegistry).Subscribe(OnNextPrevious);
		}

		public void Back()
		{
			OnBackClicked();
			_onNextPreviousHandle.Get(_eventRegistry).Unsubscribe(OnNextPrevious);
		}

		private void OnNextPrevious(bool isNext)
		{
			if (isNext)
			{
				OnNextClicked();
			}
			else
			{
				OnPrevClicked();
			}
		}

		public void SetTarget(string name, int position, int count)
		{
			if ((bool)TargetNameText)
			{
				TargetNameText.text = name;
			}
			if ((bool)TargetCounterText)
			{
				TargetCounterText.text = $"{position}/{count}";
			}
			bool interactable = count > 1;
			if ((bool)NextButton)
			{
				NextButton.interactable = interactable;
			}
			if ((bool)PrevButton)
			{
				PrevButton.interactable = interactable;
			}
		}

		public void SetStandings(IReadOnlyList<SpectatorListRow> others, SpectatorListRow? local, int total)
		{
			int num = local?.Place ?? 0;
			if ((bool)PlaceText)
			{
				PlaceText.text = string.Format("{0} {1}/", new LocalizedString("Circuitraceselectmapwindow.Place"), num);
			}
			if ((bool)PlaceTotalText)
			{
				PlaceTotalText.text = total.ToString();
			}
			if ((bool)LocalRow)
			{
				LocalRow.Show(local.HasValue);
				if (local.HasValue)
				{
					LocalRow.Bind(local.Value);
				}
			}
			if ((bool)StandingsList)
			{
				StandingsList.gameObject.SetActive(value: true);
				StandingsList.SetRows(others);
			}
		}

		public void HideStandings()
		{
			if ((bool)StandingsList)
			{
				StandingsList.gameObject.SetActive(value: false);
			}
			if ((bool)LocalRow)
			{
				LocalRow.Show(visible: false);
			}
		}

		private void Apply()
		{
			if ((bool)WatchButton)
			{
				WatchButton.gameObject.SetActive(_available && !_watching);
			}
			if ((bool)BackButton)
			{
				BackButton.gameObject.SetActive(_watching);
			}
			if ((bool)WatchingRoot)
			{
				WatchingRoot.SetActive(_watching);
			}
			if ((bool)TableRoot)
			{
				TableRoot.SetActive(!_watching);
			}
		}

		private void OnWatchClicked()
		{
			WatchRequested?.Invoke();
		}

		private void OnBackClicked()
		{
			BackRequested?.Invoke();
		}

		private void OnNextClicked()
		{
			NextRequested?.Invoke();
		}

		private void OnPrevClicked()
		{
			PrevRequested?.Invoke();
		}

		private void OnRowClicked(int actor)
		{
			RowClicked?.Invoke(actor);
		}
	}
}
