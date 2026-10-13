using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.UIElement.HUD
{
	public class HudColorChanger : MonoBehaviour
	{
		[SerializeField]
		private Image[] ImagesToSetColor;

		[SerializeField]
		private TextMeshProUGUI[] TextsToSetColor;

		public static Color DefaultColor = Color.white;

		public static Color DarkColor = Color.black;

		private RacingProvider _racingProvider;

		public Color CurrentColor
		{
			get
			{
				return DefaultColor;
			}
			private set
			{
				DefaultColor = value;
			}
		}

		[Inject]
		public void Construct(RacingProvider racingProvider)
		{
			_racingProvider = racingProvider;
		}

		private void Awake()
		{
			if (_racingProvider != null)
			{
				_racingProvider.HudColorChanger = this;
			}
		}

		private void OnDestroy()
		{
			if (_racingProvider != null && _racingProvider.HudColorChanger == this)
			{
				_racingProvider.HudColorChanger = null;
			}
		}

		private void Start()
		{
			SetColor(CurrentColor);
		}

		private void SetColor(Color color)
		{
			Image[] imagesToSetColor = ImagesToSetColor;
			foreach (Image image in imagesToSetColor)
			{
				if ((bool)image)
				{
					SetCurrentColor(image);
				}
			}
			TextMeshProUGUI[] textsToSetColor = TextsToSetColor;
			foreach (TextMeshProUGUI textMeshProUGUI in textsToSetColor)
			{
				if ((bool)textMeshProUGUI)
				{
					float a = textMeshProUGUI.color.a;
					color.a = a;
					textMeshProUGUI.color = color;
				}
			}
		}

		private void SetCarIndicatorColor(Color color)
		{
			color.a = 0.8f;
			if ((bool)V.Car && FlagsStorage.IsFreeDriveMode)
			{
				V.Car.GetComponent<LightController>().indicatorColor = color;
			}
		}

		public void SetDarkColor(bool isDark)
		{
			CurrentColor = (isDark ? DarkColor : DefaultColor);
			SetColor(CurrentColor);
			SetCarIndicatorColor(CurrentColor);
		}

		public void SetCurrentColor(Image image)
		{
			if ((bool)image)
			{
				float a = image.color.a;
				Color currentColor = CurrentColor;
				currentColor.a = a;
				image.color = currentColor;
			}
		}
	}
}
