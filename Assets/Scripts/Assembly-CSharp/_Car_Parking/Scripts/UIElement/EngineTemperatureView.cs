using CPMEngine;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	public class EngineTemperatureView : MonoBehaviour
	{
		[SerializeField]
		private Image Fill;

		[SerializeField]
		private Gradient TemperatureColors = DefaultColors();

		private CarInputManager _carInputManager;

		[Inject]
		public void Construct(CarInputManager carInputManager)
		{
			_carInputManager = carInputManager;
		}

		private void Update()
		{
			Powertrain powertrain = (_carInputManager ? _carInputManager.GetPowertrain : null);
			bool flag = powertrain != null;
			if ((bool)Fill)
			{
				Fill.enabled = flag;
			}
			if (flag && (bool)Fill)
			{
				float num = powertrain.EngineThermal.Gauge01(25f);
				Fill.fillAmount = num;
				Fill.color = TemperatureColors.Evaluate(num);
			}
		}

		private static Gradient DefaultColors()
		{
			Gradient gradient = new Gradient();
			gradient.SetKeys(new GradientColorKey[3]
			{
				new GradientColorKey(new Color(0.35f, 0.6f, 1f), 0f),
				new GradientColorKey(new Color(0.3f, 0.85f, 0.4f), 0.5f),
				new GradientColorKey(new Color(1f, 0.25f, 0.2f), 1f)
			}, new GradientAlphaKey[2]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			});
			return gradient;
		}
	}
}
