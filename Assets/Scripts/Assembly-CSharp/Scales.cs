using System;
using System.Collections.Generic;
using ElectronicRoadSign;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class Scales : MonoBehaviour
{
	public ElectronicRoadSignScript Display;

	private int totalMass;

	private List<Rigidbody> _rbList;

	private FreeDriveProvider _freeDriveProvider;

	[Inject]
	public void Construct(FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
	}

	private void Start()
	{
		_rbList = new List<Rigidbody>();
		_freeDriveProvider.MultiPlayer.OnRemoveFromScale += RemoveRigidbody;
	}

	private void RemoveRigidbody()
	{
		Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
		{
			int num = 0;
			for (int num2 = _rbList.Count - 1; num2 >= 0; num2--)
			{
				Rigidbody rigidbody = _rbList[num2];
				if (rigidbody != null)
				{
					num += (int)(rigidbody.mass + 0.5f);
				}
				else
				{
					_rbList.Remove(rigidbody);
				}
			}
			totalMass = num;
			Display.MessageText = totalMass.ToString();
		}).AddTo(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (!_rbList.Contains(attachedRigidbody))
		{
			_rbList.Add(attachedRigidbody);
			totalMass += (int)(attachedRigidbody.mass + 0.5f);
			Display.MessageText = totalMass.ToString();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if (_rbList.Contains(attachedRigidbody))
		{
			_rbList.Remove(attachedRigidbody);
			totalMass -= (int)(attachedRigidbody.mass + 0.5f);
			Display.MessageText = totalMass.ToString();
		}
	}

	public void OnDestroy()
	{
		_freeDriveProvider.MultiPlayer.OnRemoveFromScale -= RemoveRigidbody;
	}
}
