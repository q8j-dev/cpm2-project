using CPM.Leaderboard.UI;
using CPM.RacingRecords;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard
{
	public sealed class LeaderboardPickController : MonoBehaviour
	{
		private const string SelectGhostButtonKey = "RallyMode.SelectGhostText";

		private const string ChangeGhostButtonKey = "RallyMode.ChangeGhostText";

		[SerializeField]
		private LeaderboardListView _listView = default;

		[SerializeField]
		private Button _changeGhostButton = default;

		[SerializeField]
		private TextMeshProUGUI _label = default;

		[SerializeField]
		private GameObject _pickModeHighlight = default;

		private GhostRaceState _raceState;

		[Inject]
		public void Construct(GhostRaceState raceState)
		{
			_raceState = raceState;
		}

		private void OnEnable()
		{
			SetGhostButtonText(isSelect: false);
			if (_changeGhostButton != null)
			{
				_changeGhostButton.onClick.AddListener(OnChangeGhost);
			}
			if (_listView != null)
			{
				_listView.RowTapped += OnRowTapped;
				_listView.LocalBestTapped += OnLocalBestTapped;
				_listView.Rendered += Apply;
			}
			if (_raceState != null)
			{
				_raceState.Changed += Apply;
			}
			Apply();
		}

		private void SetGhostButtonText(bool isSelect)
		{
			if ((bool)_label)
			{
				_label.text = (isSelect ? new LocalizedString("RallyMode.ChangeGhostText") : new LocalizedString("RallyMode.SelectGhostText"));
			}
		}

		private void OnDisable()
		{
			if (_changeGhostButton != null)
			{
				_changeGhostButton.onClick.RemoveListener(OnChangeGhost);
			}
			if (_listView != null)
			{
				_listView.RowTapped -= OnRowTapped;
				_listView.LocalBestTapped -= OnLocalBestTapped;
				_listView.Rendered -= Apply;
			}
			if (_raceState != null)
			{
				_raceState.Changed -= Apply;
			}
		}

		private void OnChangeGhost()
		{
			if (_raceState != null)
			{
				_raceState.ArmPick();
			}
		}

		private void OnRowTapped(RacingRecord record)
		{
			if (_raceState != null)
			{
				_raceState.PickOpponent(record);
			}
			SetGhostButtonText(isSelect: true);
		}

		private void OnLocalBestTapped()
		{
			if (_raceState != null)
			{
				_raceState.PickLocal();
			}
		}

		private void Apply()
		{
			if ((bool)_listView && _raceState != null)
			{
				bool pickArmed = _raceState.PickArmed;
				_listView.SetSelectable(pickArmed);
				if ((bool)_pickModeHighlight)
				{
					_pickModeHighlight.SetActive(pickArmed);
				}
				switch (_raceState.EffectiveKind)
				{
				case GhostSelectionKind.Opponent:
					_listView.SetSelectedByUserId(_raceState.EffectiveOpponent?.UserId);
					break;
				case GhostSelectionKind.Local:
					_listView.SetSelectedLocal();
					break;
				default:
					_listView.ClearSelected();
					break;
				}
			}
		}
	}
}
