using System;

namespace _Car_Parking.Scripts.DailyRewards
{
	[Serializable]
	public class DailyRewardData
	{
		public enum StatusType
		{
			Closed = 0,
			Opened = 1,
			Rewarded = 2
		}

		public enum TypeReward
		{
			None = -1,
			Money = 0,
			Coin = 1,
			Car = 2
		}

		public StatusType Status;

		public int Money;

		public int Coin;

		public int Car = -1;

		public int Day;

		public TypeReward Type
		{
			get
			{
				if (Car >= 0)
				{
					return TypeReward.Car;
				}
				if (Coin > 0)
				{
					return TypeReward.Coin;
				}
				if (Money > 0)
				{
					return TypeReward.Money;
				}
				return TypeReward.None;
			}
		}

		public event Action<DailyRewardData> OnDataChanged;

		public DailyRewardData()
		{
		}

		public DailyRewardData(StatusType status, int money, int coin, int car, int day)
		{
			Status = status;
			Money = money;
			Coin = coin;
			Car = car;
			Day = day;
		}

		public void OnDataChangedSend()
		{
			OnDataChanged?.Invoke(this);
		}

		public override string ToString()
		{
			return $"status={Status} type={Type} money={Money} coin={Coin} car={Car} day={Day}";
		}
	}
}
