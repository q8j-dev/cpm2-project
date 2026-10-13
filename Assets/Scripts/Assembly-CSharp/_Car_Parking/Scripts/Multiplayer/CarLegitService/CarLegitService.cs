using System;
using System.Collections.Generic;
using System.Threading;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Multiplayer.CarLegitService
{
	public class CarLegitService : ICarLegitService
	{
		private class PendingCheck
		{
			public string FirebaseId;

			public string CarInstanceId;

			public int CarId;

			public string CacheKey;

			public UniTaskCompletionSource<bool> Tcs;
		}

		private const int ResultCacheCapacity = 40;

		private const int MaxBatchSize = 20;

		private const int BatchWindowFrame = 10;

		private const int CheckTimeoutSecs = 10;

		private readonly IRemoteConfigManager _remoteConfig;

		private readonly IProgressBackend _progressBackend;

		private CancellationTokenSource _batchDelayCts;

		private List<PendingCheck> _pendingBatch = new List<PendingCheck>();

		private readonly Queue<string> _resultCacheOrder = new Queue<string>(40);

		private readonly Dictionary<string, bool> _resultCache = new Dictionary<string, bool>(40);

		[Inject]
		public CarLegitService(IRemoteConfigManager remoteConfig, IProgressBackend progressBackend)
		{
			_remoteConfig = remoteConfig;
			_progressBackend = progressBackend;
		}

		public async UniTask<bool> IsCheckEnabledAsync(CancellationToken ct)
		{
			try
			{
				ConfigValuesStorage configValuesStorage;
				try
				{
					configValuesStorage = await _remoteConfig.GetConfigValuesStorage().Timeout(TimeSpan.FromSeconds(5.0)).AttachExternalCancellation(ct);
				}
				catch (Exception)
				{
					configValuesStorage = _remoteConfig.GetDefaultConfigStorage();
				}
				return configValuesStorage.multiplayerCarLegitCheckEnabled;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async UniTask<bool> IsCarLegitAsync(string firebaseId, string carInstanceId, int carId, CancellationToken ct, bool flushImmediately)
		{
			string cacheKey = GetCacheKey(firebaseId, carInstanceId, carId);
			if (_resultCache.TryGetValue(cacheKey, out var value))
			{
				return value;
			}
			if (!(await IsCheckEnabledAsync(ct)))
			{
				return true;
			}
			PendingCheck pendingCheck = new PendingCheck
			{
				FirebaseId = firebaseId,
				CarInstanceId = carInstanceId,
				CarId = carId,
				CacheKey = cacheKey,
				Tcs = new UniTaskCompletionSource<bool>()
			};
			_pendingBatch.Add(pendingCheck);
			CommonExtensions.CancelAndDisposeToken(ref _batchDelayCts);
			if (_pendingBatch.Count >= 20)
			{
				flushImmediately = true;
			}
			else
			{
				_batchDelayCts = new CancellationTokenSource();
				ScheduleFlush(_batchDelayCts.Token).Forget();
			}
			if (flushImmediately)
			{
				FlushBatch().Forget();
			}
			return await AwaitWithCancel(pendingCheck, ct);
		}

		private async UniTask ScheduleFlush(CancellationToken token)
		{
			try
			{
				await UniTask.DelayFrame(10, PlayerLoopTiming.Update, token);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			FlushBatch().Forget();
		}

		private async UniTask<bool> AwaitWithCancel(PendingCheck pending, CancellationToken ct)
		{
			using (ct.Register(() =>
			{
				pending.Tcs.TrySetResult(result: true);
			}))
			{
				return await pending.Tcs.Task;
			}
		}

		private async UniTask FlushBatch()
		{
			if (_pendingBatch.Count == 0)
			{
				return;
			}
			List<PendingCheck> batch = _pendingBatch;
			_pendingBatch = new List<PendingCheck>();
			CommonExtensions.CancelAndDisposeToken(ref _batchDelayCts);
			List<(string, string, int)> list = new List<(string, string, int)>(batch.Count);
			foreach (PendingCheck item in batch)
			{
				list.Add((item.FirebaseId, item.CarInstanceId, item.CarId));
			}
			try
			{
				Result<IReadOnlyList<bool>> result = await _progressBackend.CheckOtherCarsLegit(list, CancellationToken.None).Timeout(TimeSpan.FromSeconds(10.0));
				if (result.IsFail || result.Value == null || result.Value.Count != batch.Count)
				{
					FailOpenAll(batch);
					return;
				}
				for (int i = 0; i < batch.Count; i++)
				{
					bool flag = result.Value[i];
					StoreCachedResult(batch[i].CacheKey, flag);
					batch[i].Tcs.TrySetResult(flag);
				}
			}
			catch (TimeoutException)
			{
				FailOpenAll(batch);
			}
			catch (OperationCanceledException)
			{
				FailOpenAll(batch);
			}
			catch (Exception)
			{
				FailOpenAll(batch);
			}
		}

		private void FailOpenAll(List<PendingCheck> batch)
		{
			foreach (PendingCheck item in batch)
			{
				item.Tcs.TrySetResult(result: true);
			}
		}

		private string GetCacheKey(string firebaseId, string carInstanceId, int carId)
		{
			return $"{firebaseId}|{carInstanceId}|{carId}";
		}

		private void StoreCachedResult(string key, bool value)
		{
			if (_resultCache.ContainsKey(key))
			{
				_resultCache[key] = value;
				return;
			}
			if (_resultCache.Count >= 40)
			{
				string key2 = _resultCacheOrder.Dequeue();
				_resultCache.Remove(key2);
			}
			_resultCache.Add(key, value);
			_resultCacheOrder.Enqueue(key);
		}
	}
}
