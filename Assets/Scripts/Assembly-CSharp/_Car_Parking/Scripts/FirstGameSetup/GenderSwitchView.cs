using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	public class GenderSwitchView : MonoBehaviour
	{
		private const string LocalizeKeyMale = "Personcustomizationwindow.Male";

		private const string LocalizeKeyFemale = "Personcustomizationwindow.Female";

		private ICharacterEquipmentDataService _characterEquipmentDataService;

		private int _currentGender;

		[field: SerializeField]
		public TMP_Text GenderText { get; private set; }

		[field: SerializeField]
		public Image GenderIcon { get; private set; }

		[field: SerializeField]
		public Button LeftButton { get; private set; }

		[field: SerializeField]
		public Button RightButton { get; private set; }

		[field: SerializeField]
		public Sprite MaleIcon { get; private set; }

		[field: SerializeField]
		public Sprite FemaleIcon { get; private set; }

		public event Action<int> OnGenderChanged;

		[Inject]
		public void Construct(ICharacterEquipmentDataService characterEquipmentDataService)
		{
			_characterEquipmentDataService = characterEquipmentDataService;
		}

		private void Start()
		{
			LeftButton.onClick.AddListener(OnClicked);
			RightButton.onClick.AddListener(OnClicked);
			_currentGender = _characterEquipmentDataService.Gender;
			UpdateUI();
		}

		private void OnClicked()
		{
			_currentGender = ((_currentGender == 0) ? 1 : 0);
			UpdateUI();
			OnGenderChanged?.Invoke(_currentGender);
		}

		private void UpdateUI()
		{
			GenderIcon.sprite = ((_currentGender == 0) ? FemaleIcon : MaleIcon);
			GenderText.text = new LocalizedString((_currentGender == 0) ? "Personcustomizationwindow.Female" : "Personcustomizationwindow.Male");
		}

		private void OnDestroy()
		{
			LeftButton.onClick.RemoveAllListeners();
			RightButton.onClick.RemoveAllListeners();
		}
	}
}
