using System;
using UniRx;
using UnityEngine;

public class GlobalParent : MonoBehaviour
{
	private IDisposable _findTick;

	private void Start()
	{
		InitStart();
	}

	public void InitStart()
	{
		if ((bool)MoveElements._object)
		{
			MoveElements._object.globalParents.Add(base.gameObject.transform);
			MoveElements._object.Move();
		}
		else
		{
			_findTick = Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				Find();
			}).AddTo(this);
		}
	}

	private void Find()
	{
		if ((bool)MoveElements._object)
		{
			MoveElements._object.globalParents.Add(base.gameObject.transform);
			MoveElements._object.Move();
			_findTick?.Dispose();
			_findTick = null;
		}
	}

	public void DisableAllTargetObjects()
	{
	}
}
