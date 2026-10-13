using System;
using UniRx;
using UnityEngine;

public class ShakeFinishGate : MonoBehaviour
{
	public Quaternion TargetX;

	private float ref3;

	private IDisposable _setNormalSpeedDisposable;

	public float speedShake = 1f;

	public Quaternion targetDirection;

	public Vector3 eulerTarget;

	public float speed = 1f;

	private void Start()
	{
		SetNormalSpeed();
		eulerTarget = base.transform.eulerAngles;
		targetDirection = base.transform.rotation;
	}

	public void SetHighSpeed(float speed)
	{
		float value = speed / 6f;
		value = Mathf.Clamp(value, 1f, 14f);
		speedShake = value;
		_setNormalSpeedDisposable?.Dispose();
		_setNormalSpeedDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			SetNormalSpeed();
		}).AddTo(this);
	}

	private void SetNormalSpeed()
	{
		speedShake = 2f;
	}
}
