using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.CpmCustomFeatures.BenchmarkUsers
{
	public sealed class BenchmarkDrivePoint : MonoBehaviour
	{
		[SerializeField]
		private string BenchmarkName = "Drive Point Benchmark";

		[SerializeField]
		private float Duration = 5f;

		[SerializeField]
		private AnimationCurve ThrottleCurve = AnimationCurve.Constant(0f, 1f, 1f);

		[SerializeField]
		private AnimationCurve SteerCurve = AnimationCurve.Constant(0f, 1f, 0f);

		private float? _startTime;

		public static bool BenchmarkDrive { get; set; }

		public static float BenchmarkSteerInput { get; set; }

		public static float BenchmarkThrottleInput { get; set; }

		private void Awake()
		{
			Object.Destroy(base.gameObject);
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}

		private async UniTask Runner(Dictionary<string, object> data)
		{
		}

		private void Update()
		{
		}
	}
}
