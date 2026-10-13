using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Car.Visual
{
	[Preserve]
	public class WindshieldBrakeService : IDisposable
	{
		private const int MaxLifeTimeMs = 1500;

		private readonly WindshieldBrakeVfxPool _windshieldBrakeVfxPool;

		private List<WindshieldBrakeVfx> _windshieldBrakeVfxList = new List<WindshieldBrakeVfx>();

		private CancellationTokenSource _lifetimeCts = new CancellationTokenSource();

		public WindshieldBrakeService(WindshieldBrakeVfxPool windshieldBrakeVfxPool)
		{
			_windshieldBrakeVfxPool = windshieldBrakeVfxPool;
		}

		public void Dispose()
		{
			if (_lifetimeCts != null)
			{
				_lifetimeCts.Cancel();
				_lifetimeCts.Dispose();
				_lifetimeCts = null;
			}
		}

		public async UniTaskVoid ShowBrakeVfx(Transform transformParent)
		{
			CancellationTokenSource lifetimeCts = _lifetimeCts;
			if (lifetimeCts != null)
			{
				CancellationToken token = lifetimeCts.Token;
				WindshieldBrakeVfx windshieldBrakeVfx = _windshieldBrakeVfxPool.Spawn(transformParent);
				_windshieldBrakeVfxList.Add(windshieldBrakeVfx);
				bool num = await UniTask.Delay(1500, ignoreTimeScale: false, PlayerLoopTiming.Update, token).SuppressCancellationThrow();
				_windshieldBrakeVfxList.Remove(windshieldBrakeVfx);
				if (!num && (bool)windshieldBrakeVfx)
				{
					_windshieldBrakeVfxPool.Despawn(windshieldBrakeVfx);
				}
			}
		}
	}
}
