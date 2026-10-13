using System;
using UniRx;
using UnityEngine;

public class Movements : MonoBehaviour
{
	public bool Rotate;

	public bool Y;

	public bool X;

	public bool Z;

	public float speed = 1f;

	public float distance;

	public bool ignoreDistance;

	public float DistanceStartMove;

	private void Start()
	{
		Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			CheckDistance();
		}).AddTo(this);
		speed = UnityEngine.Random.Range(60, 90);
		if (DistanceStartMove == 0f)
		{
			DistanceStartMove = 70f;
		}
	}

	private void CheckDistance()
	{
		distance = Vector3.Distance(V.CoreGameManager.transform.position, base.transform.position);
	}

	private void Update()
	{
		if (Rotate && (distance < DistanceStartMove || ignoreDistance))
		{
			if (Y)
			{
				base.transform.Rotate(Vector3.up * Time.deltaTime * speed);
			}
			else if (X)
			{
				base.transform.Rotate(Vector3.right * Time.deltaTime * speed);
			}
			else if (Z)
			{
				base.transform.Rotate(Vector3.forward * Time.deltaTime * speed);
			}
		}
	}
}
