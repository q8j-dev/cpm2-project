using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhoneGameActivityCreateMatch : NavCanvas
	{
		public const string WindowId = "CreateMatch";

		[SerializeField]
		private Button CreateMatchButton;

		[SerializeField]
		private Toggle AllToggle;

		[SerializeField]
		private Toggle FriendsToggle;

		[SerializeField]
		private TMP_InputField PasswordedInputField;

		[SerializeField]
		private MatchmakingDataView MatchmakingDataView;

		private ArenaFlowController _arenaFlowController;

		private ArenaModeRegistry _arenaModeRegistry;

		private GameActivityEntry _entry;

		public override string Id => "CreateMatch";

		public override string Title => new LocalizedString("Lobby.CreateMatch");

		[Inject]
		public void Construct(ArenaFlowController arenaFlowController, ArenaModeRegistry arenaModeRegistry)
		{
			_arenaFlowController = arenaFlowController;
			_arenaModeRegistry = arenaModeRegistry;
		}

		public override void Open(object data)
		{
			base.Open(data);
			_entry = (GameActivityEntry)data;
			if (_arenaModeRegistry.TryGetConfig((ArenaId)_entry.ActivityId, out var config) && (bool)MatchmakingDataView)
			{
				MatchmakingDataView.UpdateView(config);
			}
			AllToggle.SetIsOnWithoutNotify(value: true);
			OnTogglesUpdated();
		}

		protected override void OnEnable()
		{
			CreateMatchButton.onClick.AddListener(OnCreateMatchClicked);
			AllToggle.onValueChanged.AddListener(OnAllToggleChanged);
			FriendsToggle.onValueChanged.AddListener(OnFriendsChanged);
		}

		protected override void OnDisable()
		{
			CreateMatchButton.onClick.RemoveListener(OnCreateMatchClicked);
			AllToggle.onValueChanged.RemoveListener(OnAllToggleChanged);
			FriendsToggle.onValueChanged.RemoveListener(OnFriendsChanged);
		}

		private void OnCreateMatchClicked()
		{
			ArenaId arenaId = (ArenaId)_entry.ActivityId;
			if (FriendsToggle.isOn)
			{
				_arenaFlowController.StartArenaForFriends(arenaId, GetPassword(), ArenaEntrySource.Create);
			}
			else
			{
				_arenaFlowController.StartArena(arenaId, ArenaEntrySource.Create);
			}
		}

		private void OnAllToggleChanged(bool value)
		{
			OnTogglesUpdated();
		}

		private void OnFriendsChanged(bool value)
		{
			AllToggle.SetIsOnWithoutNotify(!value);
			OnTogglesUpdated();
		}

		private void OnTogglesUpdated()
		{
			FriendsToggle.SetIsOnWithoutNotify(!AllToggle.isOn);
			PasswordedInputField.gameObject.SetActive(FriendsToggle.isOn);
		}

		private int GetPassword()
		{
			if (!string.IsNullOrEmpty(PasswordedInputField.text) && int.TryParse(PasswordedInputField.text, out var result))
			{
				return Mathf.Abs(result);
			}
			return -1;
		}
	}
}
