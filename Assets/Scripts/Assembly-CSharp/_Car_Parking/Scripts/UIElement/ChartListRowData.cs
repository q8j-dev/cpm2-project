using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public readonly struct ChartListRowData
	{
		public readonly int Place;

		public readonly string UserName;

		public readonly string ValueText;

		public readonly bool IsUser;

		public readonly Texture RankIcon;

		public ChartListRowData(int place, string userName, string valueText, bool isUser, Texture rankIcon)
		{
			Place = place;
			UserName = userName;
			ValueText = valueText;
			IsUser = isUser;
			RankIcon = rankIcon;
		}
	}
}
