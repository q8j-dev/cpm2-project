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
			public GameObject Root;

			public Transform Content;

			public GameObject Title;
		}

		private const string LocalizationSpecialEventCarKey = "Settingsmenuwindow.SpecialEventCar";

		private const string LocalizationDailyRewardsCarKey = "Settingsmenuwindow.DailyRewardCar";

		private const string LocalizationBackToMainMenu = "Settingsmenuwindow.ToViewReturnToMainMenu";

		[SerializeField]
		private TMP_Dropdown Dropdown;

		[SerializeField]
		private PatchNotesConfig PatchNotesConfig;

		[SerializeField]
		private PatchNotesItem PatchNotesItemPrefab;

		[SerializeField]
		private PatchNotesItemWithImage PatchNotesItemWithImagePrefab;

		[SerializeField]
		private ScrollAndTitleData FeaturesContent;

		[SerializeField]
		private ScrollAndTitleData ItemsContent;

		[SerializeField]
		private ScrollAndTitleData FixContent;

		private CoreGameManager _coreGameManager;

		private ManagerUI _managerUI;

		private GarageController _garageController;

		private OptionalDependency<GarageController> _garageControllerDep;

		private IEventService _eventService;

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
