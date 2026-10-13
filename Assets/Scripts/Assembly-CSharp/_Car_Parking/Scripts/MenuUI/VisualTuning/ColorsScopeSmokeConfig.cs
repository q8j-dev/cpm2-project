using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[CreateAssetMenu(fileName = "ColorsScopeSmokeConfig", menuName = "Configs/VisualTuning/ColorsScopeSmokeConfig")]
	public class ColorsScopeSmokeConfig : ScriptableObject
	{
		public float SmokeParticleAmountPerSec = 10f;

		[SerializeField]
		private float SmokePositionXRandom = 0.3f;

		[SerializeField]
		private float SmokeSizeMin = 0.7f;

		[SerializeField]
		private float SmokeSizeMax = 1f;

		[SerializeField]
		private float SmokeLifeTimeMin = 0.7f;

		[SerializeField]
		private float SmokeLifeTimeMax = 1f;

		[SerializeField]
		private float SmokeVelocityForwardMin = 0.7f;

		[SerializeField]
		private float SmokeVelocityForwardMax = 1f;

		[SerializeField]
		private float SmokeVelocityUpMin = 0.7f;

		[SerializeField]
		private float SmokeVelocityUpMax = 1f;

		[SerializeField]
		private float SmokeVelocityRightMin = 0.7f;

		[SerializeField]
		private float SmokeVelocityRightMax = 1f;

		[Preserve]
		public ColorsScopeSmokeConfig()
		{
		}

		public Vector3 GetPosition(VehicleData vehicleData)
		{
			Vector3 position = vehicleData.Axles.RearAxle.RightWheel.transform.position;
			float num = Random.Range(0f - SmokePositionXRandom, SmokePositionXRandom);
			return position + vehicleData.transform.right * num;
		}

		public float GetRandomSize()
		{
			return Random.Range(SmokeSizeMin, SmokeSizeMax);
		}

		public float GetRandomLifeTime()
		{
			return Random.Range(SmokeLifeTimeMin, SmokeLifeTimeMax);
		}

		public Vector3 GetVelocity(VehicleData vehicleData)
		{
			return -vehicleData.transform.forward * Random.Range(SmokeVelocityForwardMin, SmokeVelocityForwardMax) + Vector3.up * Random.Range(SmokeVelocityUpMin, SmokeVelocityUpMax) + vehicleData.transform.right * Random.Range(SmokeVelocityRightMin, SmokeVelocityRightMax);
		}
	}
}
