using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class MatchmakingDataView : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text PlayersRequaredText = default;

		[SerializeField]
		private TMP_Text MinimumToStartText = default;

		[SerializeField]
		private TMP_Text AverageWaitTimeText = default;

		[SerializeField]
		private Color SelectedColor = default;

		public void UpdateView(ArenaConfig config)
		{
			if ((bool)config)
			{
				if ((bool)PlayersRequaredText)
				{
					PlayersRequaredText.text = string.Format(new LocalizedString("Lobby.Match.PlayersRequired"), config.MaxPlayers.ToString().Colored(SelectedColor));
				}
				if ((bool)MinimumToStartText)
				{
					MinimumToStartText.text = string.Format(new LocalizedString("Lobby.Match.MinimumToStart"), config.MinPlayers.ToString().Colored(SelectedColor));
				}
				if ((bool)AverageWaitTimeText)
				{
					AverageWaitTimeText.text = string.Format(new LocalizedString("Lobby.Match.AverageWaitTime"), TimeFormatter.FormatMinutesSeconds(Random.Range(config.AverageWaitTime.x, config.AverageWaitTime.y)).Colored(SelectedColor));
				}
			}
		}
	}
}
