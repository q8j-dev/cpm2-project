using CPM.Ghost;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	public sealed class GhostToggleWidget : MonoBehaviour
	{
		[SerializeField]
		private Toggle Toggle;

		[SerializeField]
		private TMP_Text BestTimeLabel;

		private GhostStorage _storage;

		private GhostRaceState _raceState;

		[Inject]
		public void Construct(GhostStorage storage, GhostRaceState raceState)
		{
			_storage = storage;
			_raceState = raceState;
		}

		private void OnEnable()
		{
			if ((bool)Toggle)
			{
				if (_raceState != null)
				{
					Toggle.SetIsOnWithoutNotify(_raceState.RaceEnabled);
				}
				Toggle.onValueChanged.AddListener(OnToggleChanged);
			}
			if (_raceState != null)
			{
				_raceState.Changed += OnStateChanged;
			}
			Refresh();
		}

		private void OnDisable()
		{
			if ((bool)Toggle)
			{
				Toggle.onValueChanged.RemoveListener(OnToggleChanged);
			}
			if (_raceState != null)
			{
				_raceState.Changed -= OnStateChanged;
			}
		}

		private void OnStateChanged()
		{
			if ((bool)Toggle && _raceState != null && Toggle.isOn != _raceState.RaceEnabled)
			{
				Toggle.SetIsOnWithoutNotify(_raceState.RaceEnabled);
			}
		}

		public void Refresh()
		{
			if ((bool)BestTimeLabel && _storage != null)
			{
				string raceKey = RallyLeaderboardMapId.LocalKey();
				BestTimeLabel.text = (_storage.TryGetBestMeta(raceKey, out var meta) ? Format(meta.TimeMs) : "—");
			}
		}

		private void OnToggleChanged(bool value)
		{
			_raceState?.SetRaceEnabled(value);
		}

		private static string Format(long ms)
		{
			int num = (int)(ms / 1000);
			int num2 = num / 60;
			int num3 = num % 60;
			int num4 = (int)(ms % 1000 / 100);
			return $"{num2}:{num3:00}.{num4}";
		}
	}
}
