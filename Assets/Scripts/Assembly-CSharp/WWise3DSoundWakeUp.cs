using System;
using UniRx;
using UnityEngine;

public class WWise3DSoundWakeUp : MonoBehaviour
{
	private void Start()
	{
		Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			Delay();
		}).AddTo(this);
	}

	private void Delay()
	{
		if (TryGetComponent<AkRoomPortal>(out var component))
		{
			component.OnEnable();
		}
		if (TryGetComponent<AkRoom>(out var component2))
		{
			component2.OnEnable();
		}
	}
}
