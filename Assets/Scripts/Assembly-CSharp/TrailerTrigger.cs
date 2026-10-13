using System;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

public class TrailerTrigger : MonoBehaviour
{
	private LevelScript _levelScript;

	private LevelsProvider _levelsProvider;

	[Inject]
	public void Construct(LevelsProvider levelsProvider)
	{
		_levelsProvider = levelsProvider;
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!FlagsStorage.IsLevelScene || FlagsStorage.IsCarDamaged || !FlagsStorage.LevelStarted || FlagsStorage.LoadCollidedInvoked || other.transform.CompareTag("Road") || other.transform.CompareTag("DriverPerson") || other.transform.CompareTag("Car") || other.gameObject.layer == 8 || other.gameObject.layer == 15)
		{
			return;
		}
		float num = Mathf.Clamp01(other.relativeVelocity.magnitude * 0.1f);
		num *= Mathf.Clamp01(0.3f + Mathf.Abs(Vector3.Dot(other.relativeVelocity.normalized, other.contacts[0].normal)));
		if (other.gameObject.CompareTag("ped"))
		{
			num = 0.2f;
		}
		if (num <= 0.1f)
		{
			return;
		}
		int collidedObjectIndex;
		if (other.gameObject.CompareTag("TrafficCar"))
		{
			collidedObjectIndex = 1;
		}
		else if (other.gameObject.CompareTag("ped"))
		{
			collidedObjectIndex = 2;
		}
		else
		{
			collidedObjectIndex = 0;
		}
		if (collidedObjectIndex == 2 || (!FlagsStorage.EndlessCrashTime && !_levelsProvider.CurrentLevelData.ignoreCollision))
		{
			FlagsStorage.IsCarDamaged = true;
			Vector3 point = other.contacts[0].point;
			_levelsProvider.LevelScript.SetSmoothCamMove(point, point + Vector3.up * 3f);
			Observable.Timer(TimeSpan.FromSeconds(0.30000001192092896)).Subscribe((long _) =>
			{
				OnCarDamaged(collidedObjectIndex);
			}).AddTo(this);
		}
	}

	private void OnCarDamaged(int collidedObjectIndex)
	{
		LevelScript levelScript = _levelsProvider.LevelScript;
		levelScript.FailString = collidedObjectIndex switch
		{
			1 => new LocalizedString("TrailerTrigger.AvoidTraffic"), 
			2 => new LocalizedString("TrailerTrigger.AvoidPedestrian"), 
			_ => new LocalizedString("TrailerTrigger.TrailerDamaged"), 
		};
		_levelsProvider.LevelScript.OpenFailMenu();
	}
}
