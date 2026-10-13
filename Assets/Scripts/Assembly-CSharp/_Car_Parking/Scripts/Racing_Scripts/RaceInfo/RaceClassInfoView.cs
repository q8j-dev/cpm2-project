using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Racing_Scripts.RewardUI;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Racing_Scripts.RaceInfo
{
	public class RaceClassInfoView : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI BestTime = default;

		[SerializeField]
		private RectTransform BestTimeParent = default;

		[SerializeField]
		private RaceRewardItem[] RaceRewardItems = default;

		public void SetBestTime(float time)
		{
			BestTime.text = time.ToString("F2");
			UIHelper.DelayedRebuildLayout(BestTimeParent).Forget();
		}

		public void SetRewards(int[] rewards, int[] winCounts)
		{
			for (int i = 0; i < RaceRewardItems.Length; i++)
			{
				if (rewards.Length > i && winCounts.Length > i)
				{
					RaceRewardItems[i].SetText(rewards[i], winCounts[i]);
				}
			}
		}
	}
}
