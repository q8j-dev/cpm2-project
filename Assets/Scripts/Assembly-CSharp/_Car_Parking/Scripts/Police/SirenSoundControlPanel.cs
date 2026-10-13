using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Police
{
	public class SirenSoundControlPanel : MonoBehaviour
	{
		[SerializeField]
		private VisualTuningWindow VisualTuningWindow;

		[SerializeField]
		private TMP_Text CurrentSoundName;

		[SerializeField]
		private Button NextSoundButton;

		[SerializeField]
		private Button PrevSoundButton;

		[SerializeField]
		private GameObject ApplyRoot;

		[SerializeField]
		private Button ApplyButton;

		[SerializeField]
		private GameObject BuyRoot;

		[SerializeField]
		private CombinedPriceView SoundPriceView;

		[SerializeField]
		private Button BuyButton;

		private VehicleData _vehicleData;

		private PoliceSystemConfig _policeConfig;

		private IPoliceSystem _policeSystem;

		private ManagerUI _managerUI;

		[Inject]
		public void Construct(PoliceSystemConfig policeConfig, ManagerUI managerUI, IPoliceSystem policeSystem)
		{
			_policeConfig = policeConfig;
			_managerUI = managerUI;
			_policeSystem = policeSystem;
		}

		private void OnEnable()
		{
			_vehicleData = null;
			if ((bool)V.Car)
			{
				_vehicleData = V.Car.GetComponent<VehicleData>();
			}
			NextSoundButton.onClick.AddListener(OnNextSoundClick);
			PrevSoundButton.onClick.AddListener(OnPrevSoundClick);
			ApplyButton.onClick.AddListener(OnApplyClick);
			BuyButton.onClick.AddListener(OnBuyClick);
			VisualTuningWindow.OnTuningValuesChanged += OnTuningValuesChanged;
			UpdateUI();
		}

		private void OnDisable()
		{
			NextSoundButton.onClick.RemoveListener(OnNextSoundClick);
			PrevSoundButton.onClick.RemoveListener(OnPrevSoundClick);
			ApplyButton.onClick.RemoveListener(OnApplyClick);
			BuyButton.onClick.RemoveListener(OnBuyClick);
			VisualTuningWindow.OnTuningValuesChanged -= OnTuningValuesChanged;
		}

		private void OnTuningValuesChanged()
		{
			UpdateUI();
		}

		private void OnNextSoundClick()
		{
			UpdateSoundIndex(1);
		}

		private void OnPrevSoundClick()
		{
			UpdateSoundIndex(-1);
		}

		private void OnApplyClick()
		{
			VisualTuningWindow.OnClickExecute(VisualTuningTypes.Police, "ApplySirenSound");
		}

		private void OnBuyClick()
		{
			PoliceSystemConfig.SirenSoundData sirenSound = _policeConfig.GetSirenSound(VisualTuningWindow.PoliceVisualScope.SelectedSirenSoundIndex);
			if (sirenSound != null)
			{
				VisualTuningWindow.visualTuningCanvasGroup.alpha = 0f;
				string format = new LocalizedString("VisualTuningWindow.Buythisitemfor");
				_managerUI.ShowConfirmWindow(() =>
				{
					VisualTuningWindow.visualTuningCanvasGroup.alpha = 1f;
					VisualTuningWindow.OnClickExecute(VisualTuningTypes.Police, "ApplySirenSound");
				}, new LocalizedString("Playerprofilewindow.Message"), string.Format(format, sirenSound.Price), () =>
				{
					VisualTuningWindow.visualTuningCanvasGroup.alpha = 1f;
				}).Forget();
			}
		}

		private void UpdateSoundIndex(int add)
		{
			if ((bool)_vehicleData && VisualTuningWindow.PoliceVisualScope != null)
			{
				VisualTuningWindow.PoliceVisualScope.UpdateSelectedSirenSound(add);
				UpdateUI();
			}
		}

		private void UpdateUI()
		{
			if (!_vehicleData || VisualTuningWindow.PoliceVisualScope == null)
			{
				return;
			}
			PoliceSystemConfig.SirenSoundData sirenSound = _policeConfig.GetSirenSound(VisualTuningWindow.PoliceVisualScope.SelectedSirenSoundIndex);
			if (sirenSound != null)
			{
				CurrentSoundName.text = $"{VisualTuningWindow.PoliceVisualScope.SelectedSirenSoundIndex + 1}";
				if (_policeSystem.BoughtSiren.Contains(VisualTuningWindow.PoliceVisualScope.SelectedSirenSoundIndex) || (int)sirenSound.Price <= 0)
				{
					ApplyRoot.SetActive(value: true);
					BuyRoot.SetActive(value: false);
					return;
				}
				ApplyRoot.SetActive(value: false);
				BuyRoot.SetActive(value: true);
				CombinedPrice price = new CombinedPrice(sirenSound.Price, 0);
				SoundPriceView.Init(price);
			}
		}
	}
}
