using System;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Car.Sounds;

public class ExhaustSystem : MonoBehaviour
{
	public Animator _animator;

	private IDisposable _disposeExhaust;

	private static readonly int VibrationOnStringHash = Animator.StringToHash("vibrationOn");

	private void Start()
	{
		if ((bool)base.gameObject.transform.root.GetComponent<SoundController>())
		{
			base.gameObject.transform.root.GetComponent<SoundController>().ExhaustSystem = this;
		}
		if (TryGetComponent<Animator>(out _animator))
		{
			_animator.enabled = false;
			_animator.cullingMode = AnimatorCullingMode.CullCompletely;
		}
	}

	public void Play()
	{
		if ((bool)_animator)
		{
			_animator.enabled = true;
			_animator.SetBool(VibrationOnStringHash, value: true);
			_disposeExhaust?.Dispose();
			_disposeExhaust = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				Stop();
			}).AddTo(this);
		}
	}

	public void Stop()
	{
		if ((bool)_animator)
		{
			_animator.SetBool(VibrationOnStringHash, value: false);
			_animator.enabled = false;
		}
	}
}
