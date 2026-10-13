using System;
using System.Threading;
using AlmostEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using _Car_Parking.Scripts.GraphicsSettings;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	public class ShaderPrewarmer : MonoBehaviour
	{
		[SerializeField]
		private SerializableDictionary<int, string> ShaderVariantCollections = new SerializableDictionary<int, string>();

		[Min(1f)]
		[SerializeField]
		private int BatchSize = 50;

		[SerializeField]
		private int DelayFrame = 2;

		[SerializeField]
		private float DelayInSeconds = 0.01f;

		[SerializeField]
		private int SecondsToStopPrewarm = 300;

		private readonly PerformanceDetector _performanceDetector = new PerformanceDetector();

		private readonly CancellationTokenSource _cancellationTokenSourcePrewarm = new CancellationTokenSource();

		private int _currentDeviceGrade = 1;

		private int _counterSendToPrewarm;

		private int _countOfShaderVariants;

		private int _countOfPrewarmedShaders;

		private AsyncOperationHandle<ShaderVariantCollection> _loadTask;

		public bool Finished { get; private set; }

		public bool IsDelayInSeconds { get; set; }

		public event Action<bool> OnPrewarmFinished;

		private void Awake()
		{
			OnFinished(null);
		}

		public async UniTaskVoid StartPrewarm()
		{
			_currentDeviceGrade = _performanceDetector.DetectPerformanceTier();
			if (_currentDeviceGrade == 0)
			{
				OnFinished(null);
				return;
			}
			ShaderVariantCollection shaderVariantCollection = await GetShaderVariantCollection(_currentDeviceGrade);
			if (!shaderVariantCollection)
			{
				OnFinished(null);
				return;
			}
			_countOfShaderVariants = shaderVariantCollection.variantCount;
			_cancellationTokenSourcePrewarm.CancelAfterSlim(TimeSpan.FromSeconds(SecondsToStopPrewarm));
			PrewarmAllShaders(shaderVariantCollection, BatchSize, _cancellationTokenSourcePrewarm.Token).Forget();
		}

		public float GetCurrentPrewarm()
		{
			if (_countOfShaderVariants == 0 || Finished)
			{
				return 1f;
			}
			return Mathf.Clamp01((float)_countOfPrewarmedShaders / (float)_countOfShaderVariants);
		}

		public void SetDelayInSeconds(float value)
		{
			DelayInSeconds = value;
		}

		public void SetFrameDelay(int value)
		{
			DelayFrame = value;
		}

		public void SetFrameDelayOrSeconds(bool value)
		{
			IsDelayInSeconds = value;
		}

		public void SetBatchSize(int value)
		{
			BatchSize = value;
		}

		private async UniTask<ShaderVariantCollection> GetShaderVariantCollection(int deviceGrade)
		{
			if (!ShaderVariantCollections.TryGetValue(deviceGrade, out var value))
			{
				return null;
			}
			try
			{
				_loadTask = Addressables.LoadAssetAsync<ShaderVariantCollection>(value);
				return await _loadTask;
			}
			catch (Exception ex)
			{
				if (_loadTask.IsValid())
				{
					_loadTask.Release();
				}
				Debug.LogError("Shader prewarm failed: " + ex.Message);
				return null;
			}
		}

		private async UniTaskVoid PrewarmAllShaders(ShaderVariantCollection currentGradeShaderVariants, int batchSize, CancellationToken token)
		{
			_ = 2;
			try
			{
				if (currentGradeShaderVariants == null)
				{
					return;
				}
				while (!currentGradeShaderVariants.isWarmedUp)
				{
					_countOfPrewarmedShaders = currentGradeShaderVariants.warmedUpVariantCount;
					if (currentGradeShaderVariants.warmedUpVariantCount < _counterSendToPrewarm)
					{
						await UniTask.DelayFrame(1, PlayerLoopTiming.Update, token);
						continue;
					}
					PrewarmShaderVariants(currentGradeShaderVariants, batchSize);
					if (IsDelayInSeconds)
					{
						await UniTask.Delay((int)(DelayInSeconds * 1000f), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
					}
					else
					{
						await UniTask.DelayFrame(DelayFrame, PlayerLoopTiming.Update, token);
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				OnFinished(currentGradeShaderVariants);
			}
		}

		private void PrewarmShaderVariants(ShaderVariantCollection currentGradeShaderVariants, int batchSize)
		{
			if (!(currentGradeShaderVariants == null) && currentGradeShaderVariants.shaderCount != 0)
			{
				currentGradeShaderVariants.WarmUpProgressively(batchSize);
				_counterSendToPrewarm += batchSize;
			}
		}

		private void OnFinished(ShaderVariantCollection currentGradeShaderVariants)
		{
			if (currentGradeShaderVariants != null)
			{
				_ = currentGradeShaderVariants.isWarmedUp;
			}
			Finished = true;
			if (currentGradeShaderVariants != null)
			{
				OnPrewarmFinished?.Invoke(currentGradeShaderVariants.isWarmedUp);
			}
			else
			{
				OnPrewarmFinished?.Invoke(obj: false);
			}
		}

		private void OnDestroy()
		{
			_cancellationTokenSourcePrewarm?.Cancel();
			_cancellationTokenSourcePrewarm?.Dispose();
			if (_loadTask.IsValid())
			{
				_loadTask.Release();
			}
		}
	}
}
