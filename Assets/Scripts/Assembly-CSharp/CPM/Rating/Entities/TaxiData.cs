using System;

namespace CPM.Rating.Entities
{
	[Serializable]
	public class TaxiData
	{
		public readonly byte status;

		public readonly float distance;

		public readonly float givenDistance;

		public readonly float time;

		public readonly float givenTime;

		public readonly uint price;

		public readonly uint money;

		public readonly uint extraForTime;

		public readonly uint extraForSafety;

		public readonly uint collisionCount;

		public readonly uint starCount;

		public TaxiData(byte status, float distance, float time, uint price, uint money, uint extraForTime, uint extraForSafety, uint collisionCount, uint starCount, float givenDistance, float givenTime)
		{
			this.status = status;
			this.distance = (float)Math.Round(distance, 5);
			this.time = (float)Math.Round(time, 5);
			this.price = price;
			this.money = money;
			this.extraForTime = extraForTime;
			this.extraForSafety = extraForSafety;
			this.collisionCount = collisionCount;
			this.starCount = starCount;
			this.givenTime = (float)Math.Round(givenTime, 5);
			this.givenDistance = (float)Math.Round(givenDistance, 5);
		}

		public override string ToString()
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(time);
			float num = distance;
			string text = "Distance: " + num + "\n";
			string text2 = "Time left: " + timeSpan.ToString("hh\\:mm\\:ss") + "\n";
			uint num2 = price;
			string text3 = "Price per km: " + num2 + "\n";
			string text4 = "Total money: " + (money + extraForSafety + extraForTime) + "\n";
			num2 = extraForTime;
			string text5 = "Extra for time: " + num2 + "\n";
			num2 = extraForSafety;
			string text6 = "Extra for safety: " + num2 + "\n";
			num2 = collisionCount;
			string text7 = "Collision count: " + num2 + "\n";
			num2 = starCount;
			string text8 = "Star Count: " + num2 + "\n";
			num = givenDistance;
			string text9 = "Given Distance: " + num + "\n";
			return text + text9 + text2 + text3 + text4 + text5 + text6 + text7 + text8;
		}
	}
}
