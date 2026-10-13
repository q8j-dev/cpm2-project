using System;
using UniRx;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
	private float speed = 10f;

	public Vector3 initPos;

	public float maxDistance = 100f;

	private void Start()
	{
		speed = UnityEngine.Random.Range(40, 100);
		initPos = base.transform.position;
		Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			CheckDistance();
		}).AddTo(this);
	}

	private void CheckDistance()
	{
		if (Vector3.Distance(initPos, base.transform.position) > maxDistance)
		{
			base.transform.position = initPos;
		}
	}

	private void Update()
	{
		float maxDistanceDelta = speed * Time.deltaTime;
		Vector3 target = base.transform.position + base.transform.forward;
		base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, target, maxDistanceDelta);
	}
}
