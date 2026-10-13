using System.Threading;
using CPM.Ghost.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CPM.Ghost
{
	public sealed class GhostPlayback
	{
		private GameObject _ghost;

		private GhostInstance _instance;

		private GhostPlayer _player;

		private float _startTimeSec;

		private bool _started;

		public bool IsActive
		{
			get
			{
				if (_ghost != null)
				{
					return _player != null;
				}
				return false;
			}
		}

		public Transform GhostTransform
		{
			get
			{
				if (!(_ghost != null))
				{
					return null;
				}
				return _ghost.transform;
			}
		}

		public int CarId { get; private set; } = -1;

		public async UniTask PrepareAsync(IGhostCarFactory factory, GhostRecording recording, GhostMaterialPreset preset, int[] lodWheelPool, LayerMask groundLayers, Transform parent, Transform observer, CancellationToken ct)
		{
			_ghost = await factory.CreateAsync(recording.CarId, preset, lodWheelPool, groundLayers, observer, ct);
			if (!(_ghost == null))
			{
				_instance = _ghost.GetComponent<GhostInstance>();
				_ghost.transform.SetParent(parent, worldPositionStays: false);
				CarId = recording.CarId;
				_player = new GhostPlayer(_ghost.transform, recording);
				_player.ApplyFirstFrame();
				_started = false;
			}
		}

		public void Rebind(GhostRecording recording)
		{
			if (!(_ghost == null) && recording != null)
			{
				CarId = recording.CarId;
				_player = new GhostPlayer(_ghost.transform, recording);
				_player.ApplyFirstFrame();
				_started = false;
			}
		}

		public void Pause()
		{
			_started = false;
		}

		public void StartFrom(float startTimeSec)
		{
			_startTimeSec = startTimeSec;
			_started = true;
		}

		public void Tick(float nowSec)
		{
			if (_started && _player != null)
			{
				_player.Tick(nowSec - _startTimeSec);
			}
		}

		public void UpdateAlpha()
		{
			_instance?.UpdateAlpha();
		}

		public void Release(IGhostCarFactory factory)
		{
			if (_ghost != null)
			{
				factory.Release(_ghost);
			}
			_ghost = null;
			_instance = null;
			_player = null;
			_started = false;
		}
	}
}
