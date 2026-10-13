using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Rally.UI
{
	public class RallySettingsWindow : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Slider ManeuverIconsSlider;

		[SerializeField]
		private Slider PaceNotesSlider;

		[SerializeField]
		private TextMeshProUGUI PaceNotesValue;

		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private Button MinusButton;

		[SerializeField]
		private Button PlusButton;

		private readonly float _step = 1f;

		protected override void Start()
		{
			PaceNotesSlider.onValueChanged.AddListener(OnPaceNotesValueChanged);
			CloseButton.onClick.AddListener(Close);
			MinusButton.onClick.AddListener(MinusStep);
			PlusButton.onClick.AddListener(PlusStep);
			float value = PlayerPrefs.GetInt("PaceNotesValue", 2);
			PaceNotesSlider.value = value;
			_managerUI.OnRallyPopupShowed?.Invoke(obj: true);
			InitBackButton();
		}

		protected override void OnDestroy()
		{
			_managerUI.OnRallyPopupShowed?.Invoke(obj: false);
			PaceNotesSlider.onValueChanged.RemoveListener(OnPaceNotesValueChanged);
			CloseButton.onClick.RemoveListener(Close);
			MinusButton.onClick.RemoveListener(MinusStep);
			PlusButton.onClick.RemoveListener(PlusStep);
		}

		private void OnPaceNotesValueChanged(float value)
		{
			PaceNotesValue.text = (1f + value).ToString();
			ManeuverIconsSlider.value = value;
			PlayerPrefs.SetInt("PaceNotesValue", (int)value);
		}

		private void PlusStep()
		{
			PaceNotesSlider.value += _step;
		}

		private void MinusStep()
		{
			PaceNotesSlider.value -= _step;
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			Close();
		}
	}
}
