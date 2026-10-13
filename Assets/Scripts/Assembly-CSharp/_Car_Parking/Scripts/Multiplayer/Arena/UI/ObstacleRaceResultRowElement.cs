using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public sealed class ObstacleRaceResultRowElement : ResultRowElement<ObstacleRaceResultRow>
	{
		[SerializeField]
		private TMP_Text TimeText;

		protected override void OnBind(in ObstacleRaceResultRow row)
		{
			if (row.Status == ObstacleRacePlayerStatus.Playing)
			{
				SetNickName(string.Empty);
			}
			if ((bool)TimeText)
			{
				TimeText.text = (row.EliminatedAt.HasValue ? TimeFormatter.FormatElapsed(row.EliminatedAt.Value) : string.Empty);
			}
		}
	}
}
