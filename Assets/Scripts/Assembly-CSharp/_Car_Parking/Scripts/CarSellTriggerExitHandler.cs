using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts
{
	public class CarSellTriggerExitHandler : IInitializable, IDisposable
	{
		private class Data
		{
			public int Index;

			public Collider CarCollider;

			public Bounds FakeSellingCarBounds;

			public Bounds TriggerColliderBounds;

			public Vector3 TriggerColliderCenterOffsetPosition;
		}

		private readonly List<Data> _data = new List<Data>();

		private readonly CommonProvider _commonProvider;

		private readonly FreeDriveProvider _freeDriveProvider;

		private CancellationTokenSource _cts;

		[Inject]
		public CarSellTriggerExitHandler(CommonProvider commonProvider, FreeDriveProvider freeDriveProvider)
		{
			_commonProvider = commonProvider;
			_freeDriveProvider = freeDriveProvider;
		}

		public void Initialize()
		{
			_cts = new CancellationTokenSource();
			ExecuteAsync(_cts.Token).Forget();
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cts);
		}

		public void Clear(int index)
		{
			for (int i = 0; i < _data.Count; i++)
			{
				if (_data[i].Index == index)
				{
					_data.RemoveAt(i);
					break;
				}
			}
		}

		public void Add(GameObject car, int index, Collider carCollider, Bounds triggerColliderBounds)
		{
			if (!car || _freeDriveProvider.FirstCarMultiPlayer != car)
			{
				return;
			}
			for (int i = 0; i < _data.Count; i++)
			{
				if (_data[i].Index == index)
				{
					_data[i].CarCollider = carCollider;
					_data[i].TriggerColliderBounds = triggerColliderBounds;
					_data[i].TriggerColliderCenterOffsetPosition = triggerColliderBounds.center - V.GlobalOffset;
					return;
				}
			}
			_data.Add(new Data
			{
				Index = index,
				CarCollider = carCollider,
				FakeSellingCarBounds = default,
				TriggerColliderBounds = triggerColliderBounds,
				TriggerColliderCenterOffsetPosition = triggerColliderBounds.center - V.GlobalOffset
			});
		}

		private async UniTask ExecuteAsync(CancellationToken cancellationToken)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			while (!cancellationToken.IsCancellationRequested)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				for (int num = _data.Count - 1; num >= 0; num--)
				{
					if (!_data[num].CarCollider)
					{
						_data.RemoveAt(num);
					}
					else
					{
						_data[num].FakeSellingCarBounds.center = _data[num].CarCollider.bounds.center;
						Vector3 size = _data[num].CarCollider.bounds.size;
						if (size.x > 0.02f && size.y > 0.02f && size.z > 0.02f)
						{
							_data[num].FakeSellingCarBounds.size = _data[num].CarCollider.bounds.size;
						}
						_data[num].TriggerColliderBounds.center = V.GlobalOffset + _data[num].TriggerColliderCenterOffsetPosition;
						if (!_data[num].TriggerColliderBounds.Intersects(_data[num].FakeSellingCarBounds))
						{
							Data data = _data[num];
							_data.RemoveAt(num);
							_commonProvider.SellExchangeFreeService.SellCarExit(data.Index);
						}
					}
				}
			}
		}
	}
}
