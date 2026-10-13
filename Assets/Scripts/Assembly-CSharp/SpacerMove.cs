using System;
using UniRx;
using UnityEngine;

public class SpacerMove : MonoBehaviour
{
	public Vector3 targetPos;

	public Transform FinalPos;

	public Transform rayPos;

	public bool Move;

	public bool targetReached;

	public bool DoRay;

	private float _fracComplete;

	private float _startTime;

	private Vector3 _dir;

	private Vector3 _initialLocalPos;

	private void Start()
	{
		_initialLocalPos = base.transform.localPosition;
		Observable.Timer(TimeSpan.FromSeconds(1.0)).Concat(Observable.Interval(TimeSpan.FromSeconds(1.0))).Subscribe((long _) =>
		{
			UpdateStepPos();
		})
			.AddTo(this);
		_startTime = Time.time;
		ExtendSpacer(extend: false);
	}

	private void Update()
	{
		if (!Move || targetReached)
		{
			return;
		}
		LayerMask layerMask = -1;
		if (Physics.Raycast(rayPos.position, rayPos.forward, out var _, 0.22f, layerMask.value, QueryTriggerInteraction.Ignore) && DoRay)
		{
			Move = false;
			return;
		}
		float t = (Time.time - _startTime) / 5f;
		base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, targetPos, t);
		if (Vector3.Distance(base.transform.localPosition, targetPos) < 0.1f)
		{
			targetReached = true;
		}
	}

	private void UpdateStepPos()
	{
		if (!targetReached)
		{
			Move = true;
			_startTime = Time.time;
		}
	}

	public void ExtendSpacer(bool extend)
	{
		if (extend)
		{
			targetPos = FinalPos.localPosition;
			DoRay = true;
		}
		else
		{
			targetPos = _initialLocalPos;
			DoRay = false;
		}
		Move = true;
		targetReached = false;
		_startTime = Time.time;
	}
}
