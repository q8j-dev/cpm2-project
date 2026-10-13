using System;
using CPMEngine;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	public class SpeedometerTireView : MonoBehaviour
	{
		[Serializable]
		private class TireImages
		{
			public Image Outline;

			public Image Fill;
		}

		[SerializeField]
		private TireImages FrontLeft;

		[SerializeField]
		private TireImages FrontRight;

		[SerializeField]
		private TireImages RearLeft;

		[SerializeField]
		private TireImages RearRight;

		[SerializeField]
		private Gradient TemperatureColors = DefaultTemperatureColors();

		[SerializeField]
		private bool TintFillByTemperature = true;

		private CarInputManager _carInputManager;

		[Inject]
		public void Construct(CarInputManager carInputManager)
		{
			_carInputManager = carInputManager;
		}

		private void Update()
		{
			Axles getAxles = _carInputManager.GetAxles;
			if ((bool)getAxles)
			{
				bool has = TryAverage(out var temperature, out var health, getAxles.FrontAxle.LeftWheel);
				bool has2 = TryAverage(out var temperature2, out var health2, getAxles.FrontAxle.RightWheel);
				bool has3 = TryAverage(out var temperature3, out var health3, getAxles.RearAxle.LeftWheel, getAxles.Rear1Axle.LeftWheel, getAxles.Rear2Axle.LeftWheel);
				bool has4 = TryAverage(out var temperature4, out var health4, getAxles.RearAxle.RightWheel, getAxles.Rear1Axle.RightWheel, getAxles.Rear2Axle.RightWheel);
				MirrorMissingSide(ref has, ref temperature, ref health, has2, temperature2, health2);
				MirrorMissingSide(ref has2, ref temperature2, ref health2, has, temperature, health);
				MirrorMissingSide(ref has3, ref temperature3, ref health3, has4, temperature4, health4);
				MirrorMissingSide(ref has4, ref temperature4, ref health4, has3, temperature3, health3);
				Apply(FrontLeft, has, temperature, health);
				Apply(FrontRight, has2, temperature2, health2);
				Apply(RearLeft, has3, temperature3, health3);
				Apply(RearRight, has4, temperature4, health4);
			}
		}

		private void Apply(TireImages tire, bool hasData, float temperatureGauge, float health)
		{
			if (hasData)
			{
				Color color = TemperatureColors.Evaluate(temperatureGauge);
				tire.Outline.color = color;
				tire.Fill.color = (TintFillByTemperature ? color : Color.white);
				tire.Fill.fillAmount = Mathf.Clamp01(health);
			}
		}

		private static bool TryAverage(out float temperature, out float health, WheelProvider wheel0, WheelProvider wheel1 = null, WheelProvider wheel2 = null)
		{
			float tempSum = 0f;
			float healthSum = 0f;
			int count = 0;
			Accumulate(wheel0, ref tempSum, ref healthSum, ref count);
			Accumulate(wheel1, ref tempSum, ref healthSum, ref count);
			Accumulate(wheel2, ref tempSum, ref healthSum, ref count);
			if (count == 0)
			{
				temperature = 0f;
				health = 0f;
				return false;
			}
			temperature = tempSum / (float)count;
			health = healthSum / (float)count;
			return true;
		}

		private static void Accumulate(WheelProvider wheel, ref float tempSum, ref float healthSum, ref int count)
		{
			if ((bool)wheel)
			{
				tempSum += wheel.TemperatureGauge01;
				healthSum += wheel.Wheel.TyreHealth;
				count++;
			}
		}

		private static void MirrorMissingSide(ref bool has, ref float temperature, ref float health, bool otherHas, float otherTemperature, float otherHealth)
		{
			if (!has && otherHas)
			{
				has = true;
				temperature = otherTemperature;
				health = otherHealth;
			}
		}

		private static Gradient DefaultTemperatureColors()
		{
			Gradient gradient = new Gradient();
			gradient.SetKeys(new GradientColorKey[6]
			{
				new GradientColorKey(new Color(0.25f, 0.55f, 1f), 0f),
				new GradientColorKey(new Color(0.25f, 0.55f, 1f), 0.2f),
				new GradientColorKey(new Color(0.2f, 0.85f, 0.3f), 0.45f),
				new GradientColorKey(new Color(0.2f, 0.85f, 0.3f), 0.55f),
				new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0.75f),
				new GradientColorKey(new Color(1f, 0.25f, 0.2f), 0.95f)
			}, new GradientAlphaKey[2]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			});
			return gradient;
		}
	}
}
