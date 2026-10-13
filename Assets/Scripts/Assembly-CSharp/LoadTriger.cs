using System;
using UniRx;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Localization;

public class LoadTriger : MonoBehaviour
{
	private IDisposable _disposableOnLevelFail;

	public LevelScript LevelScript { get; set; }

	private void OnCollisionEnter(Collision other)
	{
		if (FlagsStorage.LoadCollidedInvoked || FlagsStorage.IsCarDamaged)
		{
			return;
		}
		int layer = other.gameObject.layer;
		if (layer != LayerViaName.PedestrainLayer && layer != LayerViaName.LoadLayer && layer != LayerViaName.CarLayer && layer != LayerViaName.TrailerLayer)
		{
			_disposableOnLevelFail?.Dispose();
			_disposableOnLevelFail = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				OnLevelFail();
			}).AddTo(this);
			FlagsStorage.LoadCollidedInvoked = true;
			LevelScript.LastDroppedLoad = base.transform;
			LevelScript.SetSmoothCamMove(other.contacts[0].point, other.contacts[0].point + Vector3.up * 3f);
		}
	}

	private void OnLevelFail()
	{
		LevelScript.FailString = new LocalizedString("LoadTriger.LoadLost");
		LevelScript.OpenFailMenu();
	}
}
