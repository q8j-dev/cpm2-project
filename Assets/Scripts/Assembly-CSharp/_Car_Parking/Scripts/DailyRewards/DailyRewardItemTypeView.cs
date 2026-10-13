using UnityEngine;

namespace _Car_Parking.Scripts.DailyRewards
{
	public abstract class DailyRewardItemTypeView : MonoBehaviour
	{
		public abstract DailyRewardData.TypeReward TypeReward { get; }

		public abstract void Init(DailyRewardData dailyRewardData);
	}
}
