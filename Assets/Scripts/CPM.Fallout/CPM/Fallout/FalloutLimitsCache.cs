using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.GameActivity;

namespace CPM.Fallout
{
	[Preserve]
	public sealed class FalloutLimitsCache : IDeleteUserData, IDisposable
	{
		private sealed class Entry
		{
			public FalloutLimits Data;

			public double ResetAt;

			public bool HasData;

			public bool IsFetching;

			public readonly List<UniTaskCompletionSource<bool>> Waiters = new List<UniTaskCompletionSource<bool>>();
		}

		private readonly Dictionary<GameActivityType, Entry> _byActivity = new Dictionary<GameActivityType, Entry>();

		private readonly IFalloutBackend _backend;

		private readonly Func<double> _now;

		private int _generation;

		private CancellationTokenSource _sessionCts = new CancellationTokenSource();

		private bool _disposed;

		public event Action<GameActivityType, FalloutLimits> Updated;

		public FalloutLimitsCache(IFalloutBackend backend, Func<double> now)
		{
			_backend = backend;
			_now = now;
		}

		public async UniTask<Result<FalloutLimits>> GetLimits(GameActivityType activityType, CancellationToken ct = default(CancellationToken))
		{
			if (_disposed)
			{
				return Result<FalloutLimits>.Fail();
			}
			Entry e = GetOrCreate(activityType);
			if (IsFresh(e))
			{
				return Result<FalloutLimits>.Success(Snapshot(e));
			}
			int generation = _generation;
			bool flag;
			if (e.IsFetching)
			{
				UniTaskCompletionSource<bool> uniTaskCompletionSource = new UniTaskCompletionSource<bool>();
				e.Waiters.Add(uniTaskCompletionSource);
				flag = await uniTaskCompletionSource.Task;
			}
			else
			{
				e.IsFetching = true;
				flag = await FetchAsync(activityType, e);
			}
			if (generation != _generation || ct.IsCancellationRequested)
			{
				return Result<FalloutLimits>.Fail();
			}
			return flag ? Result<FalloutLimits>.Success(Snapshot(e)) : Result<FalloutLimits>.Fail();
		}

		public bool TryGet(GameActivityType activityType, out FalloutLimits limits)
		{
			if (_byActivity.TryGetValue(activityType, out var value) && IsFresh(value))
			{
				limits = Snapshot(value);
				return true;
			}
			limits = null;
			return false;
		}

		public void AddEarned(GameActivityType activityType, long coins, long money)
		{
			if (_byActivity.TryGetValue(activityType, out var value) && value.HasData)
			{
				long num = Math.Min(value.Data.CoinLimit, value.Data.CoinsEarned + coins);
				long num2 = Math.Min(value.Data.MoneyLimit, value.Data.MoneyEarned + money);
				if (num != value.Data.CoinsEarned || num2 != value.Data.MoneyEarned)
				{
					value.Data.CoinsEarned = num;
					value.Data.MoneyEarned = num2;
					Updated?.Invoke(activityType, Snapshot(value));
				}
			}
		}

		public void Invalidate(GameActivityType activityType)
		{
			_byActivity.Remove(activityType);
		}

		public void ClearAll()
		{
			_byActivity.Clear();
			_generation++;
			_sessionCts.Cancel();
			_sessionCts.Dispose();
			_sessionCts = new CancellationTokenSource();
		}

		public void DeleteData()
		{
			ClearAll();
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_byActivity.Clear();
				_generation++;
				_sessionCts.Cancel();
				_sessionCts.Dispose();
			}
		}

		private async UniTask<bool> FetchAsync(GameActivityType activityType, Entry e)
		{
			int generation = _generation;
			bool ok = false;
			try
			{
				using CancellationTokenSource owned = CancellationTokenSource.CreateLinkedTokenSource(_sessionCts.Token);
				Result<FalloutLimits> result = await _backend.GetLimits((int)activityType, owned.Token);
				if (generation == _generation && result.IsSuccess && result.Value != null)
				{
					e.Data = result.Value;
					e.ResetAt = _now() + (double)result.Value.SecondsUntilReset;
					e.HasData = true;
					ok = true;
				}
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				e.IsFetching = false;
				ReleaseWaiters(e, ok);
			}
			if (ok)
			{
				Updated?.Invoke(activityType, Snapshot(e));
			}
			return ok;
		}

		private static void ReleaseWaiters(Entry e, bool ok)
		{
			if (e.Waiters.Count != 0)
			{
				UniTaskCompletionSource<bool>[] array = e.Waiters.ToArray();
				e.Waiters.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].TrySetResult(ok);
				}
			}
		}

		private bool IsFresh(Entry e)
		{
			if (e.HasData)
			{
				return _now() < e.ResetAt;
			}
			return false;
		}

		private FalloutLimits Snapshot(Entry e)
		{
			double num = e.ResetAt - _now();
			return new FalloutLimits
			{
				CoinLimit = e.Data.CoinLimit,
				MoneyLimit = e.Data.MoneyLimit,
				CoinsEarned = e.Data.CoinsEarned,
				MoneyEarned = e.Data.MoneyEarned,
				SecondsUntilReset = ((num > 0.0) ? ((long)num) : 0)
			};
		}

		private Entry GetOrCreate(GameActivityType activityType)
		{
			if (!_byActivity.TryGetValue(activityType, out var value))
			{
				value = new Entry();
				_byActivity[activityType] = value;
			}
			return value;
		}
	}
}
