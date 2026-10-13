using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CPM.VFX
{
	[RequireComponent(typeof(Light))]
	public class BonfireLight : MonoBehaviour
	{
		public float MaxReduction;

		public float MaxIncrease;

		public float RateDamping;

		public float Strength;

		private Light _lightSource;

		private float _baseIntensity;

		private CancellationTokenSource _cts;

		public void Reset()
		{
			MaxReduction = 0.2f;
			MaxIncrease = 0.2f;
			RateDamping = 0.1f;
			Strength = 300f;
		}

		public void Awake()
		{
			_lightSource = GetComponent<Light>();
		}

		private void OnEnable()
		{
			if (_lightSource == null)
			{
				Debug.LogError("Flicker script must have a Light Component on the same GameObject.");
				return;
			}
			_baseIntensity = _lightSource.intensity;
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = new CancellationTokenSource();
			DoFlicker(_cts.Token).Forget();
		}

		private void OnDisable()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
		}

		private async UniTaskVoid DoFlicker(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				_lightSource.intensity = Mathf.Lerp(_lightSource.intensity, Random.Range(_baseIntensity - MaxReduction, _baseIntensity + MaxIncrease), Strength * Time.deltaTime);
				await UniTask.Delay((int)((RateDamping + Random.Range((0f - RateDamping) * 0.5f, RateDamping * 0.5f)) * 1000f), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
		}
	}
}
