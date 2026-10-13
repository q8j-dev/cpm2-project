using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class PatchNotesSection : SettingsSectionBase
	{
		[Serializable]
		private class ScrollAndTitleData
		{
			public GameObject Root = default;

			public Transform Content = default;

			public GameObject Title = default;
		}

		private const string LocalizationSpecialEventCarKey = "Settingsmenuwindow.SpecialEventCar";

		private const string LocalizationDailyRewardsCarKey = "Settingsmenuwindow.DailyRewardCar";

		private const string LocalizationBackToMainMenu = "Settingsmenuwindow.ToViewReturnToMainMenu";

		private ManagerUI _managerUI;

		private List<PatchNotesItemWithImage> _currentPatchNotesItemWithImages = new List<PatchNotesItemWithImage>();

		private void OnEnable()
		{
			base.gameObject.SetActive(value: false);
		}

		public override void Init()
		{
		}
	}
}
