using System;
using UniRx;
using UnityEngine;

public class FreeDriveGlobalElement : MonoBehaviour
{
	public static FreeDriveGlobalElement _object;

	private IDisposable _findTick;

	private void Start()
	{
		_object = this;
		if ((bool)MoveElements._object)
		{
			MoveElements._object.globalParents.Add(base.gameObject.transform);
			return;
		}
		_findTick = Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			Find();
		}).AddTo(this);
	}

	private void Find()
	{
		if ((bool)MoveElements._object)
		{
			MoveElements._object.globalParents.Add(base.gameObject.transform);
			_findTick?.Dispose();
			_findTick = null;
		}
	}
}
