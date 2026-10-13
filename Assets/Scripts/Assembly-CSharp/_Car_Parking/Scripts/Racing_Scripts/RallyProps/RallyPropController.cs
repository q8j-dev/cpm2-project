using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using RacingScripts.RaceControllers;
using UnityEngine;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	public class RallyPropController : IInitializable, ITickable, IDisposable
	{
		private readonly IRallyPropManager _propManager;

		private readonly RaceModeController _raceModeController;

		private readonly RallyPropControllerConfig _config;

		private readonly Camera _camera;

		private RallyController _rally;

		private RallyPropDataSO _currentSource;

		private IReadOnlyList<RallyPropData> _props;

		private PooledProp[] _spawned;

		private float _throttleTimer;

		private bool _enabled;

		private CancellationTokenSource _cts;

		public RallyPropController(IRallyPropManager propManager, RaceModeController raceModeController, RallyPropControllerConfig config, Camera camera)
		{
			_propManager = propManager;
			_raceModeController = raceModeController;
			_config = config;
			_camera = camera;
		}

		public void Initialize()
		{
			_throttleTimer = _config.ThrottleInterval;
			_cts = new CancellationTokenSource();
			InitAsync(_cts.Token).Forget();
		}

		private async UniTaskVoid InitAsync(CancellationToken ct)
		{
			await UniTask.WaitUntil(() => _raceModeController.SpawnedObject, PlayerLoopTiming.Update, ct);
			_rally = _raceModeController.SpawnedObject.GetComponentInChildren<RallyController>();
			if ((bool)_rally)
			{
				_rally.OnStateChanged += DataUpdated;
				DataUpdated();
			}
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cts);
			if ((bool)_rally)
			{
				_rally.OnStateChanged -= DataUpdated;
			}
			DespawnAll();
		}

		public void Tick()
		{
			if (!_enabled)
			{
				return;
			}
			_throttleTimer += Time.deltaTime;
			if (_throttleTimer < _config.ThrottleInterval)
			{
				return;
			}
			Vector3 position = _camera.transform.position;
			float num = _config.SpawnRadius * _config.SpawnRadius;
			float num2 = _config.DespawnRadius * _config.DespawnRadius;
			int num3 = 0;
			bool flag = false;
			for (int i = 0; i < _props.Count; i++)
			{
				float num4 = _props[i].Position.x - position.x;
				float num5 = _props[i].Position.z - position.z;
				float num6 = num4 * num4 + num5 * num5;
				if (num6 <= num)
				{
					if (_spawned[i] == null)
					{
						if (num3 < _config.MaxSpawnsPerTick)
						{
							_spawned[i] = _propManager.Spawn(_props[i]);
							num3++;
						}
						else
						{
							flag = true;
						}
					}
				}
				else if (num6 > num2 && (bool)_spawned[i])
				{
					_propManager.Despawn(_spawned[i]);
					_spawned[i] = null;
				}
			}
			_throttleTimer = (flag ? _config.ThrottleInterval : 0f);
		}

		private void DataUpdated()
		{
			RallyPropDataSO activeSource = GetActiveSource();
			_enabled = activeSource != null;
			if (!_enabled)
			{
				if (_spawned != null)
				{
					DespawnAll();
				}
			}
			else if (activeSource != _currentSource)
			{
				DespawnAll();
				_currentSource = activeSource;
				_props = activeSource.Props;
				_spawned = new PooledProp[_props.Count];
				_throttleTimer = _config.ThrottleInterval;
			}
		}

		private void DespawnAll()
		{
			if (_spawned != null)
			{
				for (int i = 0; i < _spawned.Length; i++)
				{
					if ((bool)_spawned[i])
					{
						_propManager.Despawn(_spawned[i]);
						_spawned[i] = null;
					}
				}
				_spawned = null;
			}
			_props = null;
			_currentSource = null;
		}

		private RallyPropDataSO GetActiveSource()
		{
			if (!_rally)
			{
				return null;
			}
			if (_rally.RallyState == RallyState.Garage)
			{
				return null;
			}
			return _rally.RallySequenceData?.PropData;
		}
	}
}
