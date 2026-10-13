using System.Collections.Generic;
using CPMEngine;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Car
{
	public class SoundObjectTrigger : MonoBehaviour
	{
		private SoundController _soundController;

		private List<Transform> _leavesTransforms;

		private Powertrain _powertrain;

		private LevelsProvider _levelsProvider;

		private CompositeDisposable _disposables = new CompositeDisposable();

		public void Init(SoundController sound, Powertrain powertrain, LevelsProvider levelsProvider)
		{
			_levelsProvider = levelsProvider;
			_powertrain = powertrain;
			_soundController = sound;
			_leavesTransforms = new List<Transform>();
			if ((bool)_soundController && (bool)_powertrain)
			{
				_disposables?.Dispose();
				_disposables = new CompositeDisposable();
				_powertrain.SpeedMpsReactive.Subscribe(UpdateSound).AddTo(_disposables);
			}
		}

		private void OnDestroy()
		{
			_disposables?.Dispose();
		}

		private void UpdateSound(float speed)
		{
			if (_leavesTransforms.Count != 0)
			{
				if (speed > 0.1f)
				{
					_soundController.HitLeaves();
				}
				else
				{
					_soundController.StopGrassAudio();
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((bool)_levelsProvider.CurrentLevelData && _levelsProvider.CurrentLevelData.Type != LevelType.Chasing && (bool)_soundController && other.CompareTag("Leaves"))
			{
				_soundController.HitLeaves();
				_soundController.SetGrassVolume(0f);
				_leavesTransforms.Add(other.transform);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if ((bool)_levelsProvider.CurrentLevelData && _levelsProvider.CurrentLevelData.Type != LevelType.Chasing && (bool)_soundController && other.CompareTag("Leaves"))
			{
				_leavesTransforms.Remove(other.transform);
				if (_leavesTransforms.Count == 0)
				{
					_soundController.StopGrassAudio();
				}
			}
		}
	}
}
