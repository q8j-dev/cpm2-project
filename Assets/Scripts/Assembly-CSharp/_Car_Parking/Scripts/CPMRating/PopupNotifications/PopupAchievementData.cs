using UnityEngine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.CPMRating.PopupNotifications
{
	public class PopupAchievementData : PopupNotificationData
	{
		public string Name;

		public string Description;

		public string Score;

		public Texture Icon;

		public string Rang;

		public string MainDescription;

		private string _name;

		private string _description;

		public PopupAchievementData(string name, string description, string score, Texture icon, string rang, string mainDescription)
		{
			Name = name;
			Description = description;
			Score = score;
			Icon = icon;
			Rang = rang;
			MainDescription = mainDescription;
		}

		public override void InitPrefab(PopupNotificationWindow popupNotificationWindow)
		{
			if ((bool)popupNotificationWindow)
			{
				Texture bckgTexture = Resources.Load("icons/Rating/Ach_BG_" + (int.Parse(Rang) + 1), typeof(Texture)) as Texture;
				popupNotificationWindow.InitAchievementNotification(Name, Description, new LocalizedString("PopupAchievementData.NewAchievement!"), Icon, bckgTexture);
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
