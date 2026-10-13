using System;
using System.Collections.Generic;
using Service;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Steam.World;

public class MoveElements : MonoBehaviour
{
	public static MoveElements _object;

	public List<Transform> globalParents = new List<Transform>();

	public Transform _camTransform;

	public float _dist;

	public Transform _absolutePos;

	public Vector3 _distance;

	public Vector3 lastPos = new Vector3(0f, 0f, 0f);

	public static event Action GlobalPointMoved;

	private void Start()
	{
		_dist = 500f;
		_object = this;
		_camTransform = V.CoreGameManager.transform;
		_absolutePos = new GameObject("absolute point").transform;
		_absolutePos.parent = null;
		_absolutePos.transform.position = Vector3.zero;
		Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
		{
			CheckDistance();
		}).AddTo(this);
	}

	private void OnDisable()
	{
		Shader.SetGlobalVector("_GlobalPointOffset", Vector4.zero);
	}

	private void OnDestroy()
	{
		_object = null;
	}

	public void CheckDistance()
	{
		if (V.DriverPerson == null || Vector3.Distance(_camTransform.position, V.DriverPerson.transform.position) > 1000f || globalParents.Count == 0)
		{
			return;
		}
		Vector3 position = _camTransform.position;
		position.y = 0f;
		lastPos.y = 0f;
		if (Mathf.Abs(position.x) > 900f || Mathf.Abs(position.z) > 900f)
		{
			Vector3 distance = _absolutePos.position - position;
			_distance = distance;
			V.GlobalPoint.position += new Vector3(_distance.x, 0f, _distance.z);
			V.GlobalOffset = V.GlobalPoint.position;
			Global.GetTrailer(out var trailer);
			Move();
			if ((bool)trailer)
			{
				trailer.Attach(trailer.Car);
			}
			lastPos = position;
			Physics.SyncTransforms();
		}
	}

	public void MoveGlobalPointOnGarageIn()
	{
		if (V.GlobalPoint.position.y != 100f)
		{
			V.GlobalPoint.position = new Vector3(V.GlobalPoint.position.x, 100f, V.GlobalPoint.position.z);
			V.GlobalOffset = V.GlobalPoint.position;
			Move();
			Physics.SyncTransforms();
			SteamCarTeleportSnap.SnapCar();
		}
	}

	public void MoveGlobalPointOnGarageOut()
	{
		if (V.GlobalPoint.position.y != 0f)
		{
			V.GlobalPoint.position = new Vector3(V.GlobalPoint.position.x, 0f, V.GlobalPoint.position.z);
			V.GlobalOffset = V.GlobalPoint.position;
			Move();
			Physics.SyncTransforms();
			SteamCarTeleportSnap.SnapCar();
		}
	}

	public void Move()
	{
		for (int i = 0; i < globalParents.Count; i++)
		{
			if (!(globalParents[i] == null))
			{
				globalParents[i].position = V.GlobalPoint.position;
			}
		}
		Shader.SetGlobalVector("_GlobalPointOffset", new Vector4(V.GlobalPoint.position.x, V.GlobalPoint.position.z, 0f, 0f));
		GlobalPointMoved?.Invoke();
	}
}
