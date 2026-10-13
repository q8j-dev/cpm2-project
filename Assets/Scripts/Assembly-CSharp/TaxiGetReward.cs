using System;
using Photon.Pun;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other;

public class TaxiGetReward : MonoBehaviourPunCallbacks
{
	public int reward;

	public Rigidbody carRigidbody;

	public Transform icon;

	private bool isDone = default;

	private Collider carCollider;

	public GameObject taxiPerson;

	private IDisposable _checkDistanceTick;

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)other.transform.parent.GetComponent<VehicleData>())
		{
			GameObject car = other.transform.parent.gameObject;
			if (getTaxiPerson(car) == null)
			{
				return;
			}
		}
		if (!(V.Car == other.transform.parent.gameObject))
		{
			return;
		}
		GameObject gameObject = other.transform.parent.gameObject;
		if (gameObject != null && (bool)gameObject.GetComponent<Rigidbody>())
		{
			carRigidbody = gameObject.GetComponent<Rigidbody>();
			carCollider = other;
			_checkDistanceTick?.Dispose();
			_checkDistanceTick = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				CheckDistance();
			}).AddTo(this);
			if (carRigidbody.velocity.magnitude > 2f)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("TaxiGetReward.StopHere"), 5f);
			}
			else
			{
				FinishQuest(gameObject);
			}
		}
	}

	private GameObject getTaxiPerson(GameObject car)
	{
		car.GetComponent<VehicleData>().VehicleMembersController.TryGetFirstMember(out var passenger);
		return passenger;
	}

	private void FinishQuest(GameObject _car)
	{
	}

	private void CreateAfter()
	{
		TaxiController._taxiController.Create();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void CheckDistance()
	{
		Collider collider = carCollider;
		if (Vector3.Distance(collider.transform.position, base.transform.position) > 5f)
		{
			_checkDistanceTick?.Dispose();
			_checkDistanceTick = null;
		}
		else
		{
			if (isDone || !(V.Car == collider.transform.parent.gameObject))
			{
				return;
			}
			GameObject gameObject = collider.transform.parent.gameObject;
			if (gameObject != null && (bool)gameObject.GetComponent<Rigidbody>())
			{
				carRigidbody = gameObject.GetComponent<Rigidbody>();
				if (carRigidbody != null && !(carRigidbody.velocity.magnitude > 2f))
				{
					FinishQuest(gameObject);
				}
			}
		}
	}
}
