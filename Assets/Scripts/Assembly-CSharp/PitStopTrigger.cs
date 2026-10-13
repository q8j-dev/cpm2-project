using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking;

public class PitStopTrigger : MonoBehaviour
{
	public Transform PitStopStartTransform;

	public bool IsBusy;

	public int CarPlaceId;

	public MeshRenderer MeshRenderer;

	private void OnEnable()
	{
		CarPlaceId = -1;
	}

	public void Init(int index, bool showMeshRenderer)
	{
		MeshRenderer.enabled = showMeshRenderer;
		CarPlaceId = index;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!FlagsStorage.IsPhotoMode && other.gameObject.layer == 31)
		{
			LapRaceCar component = other.transform.parent.GetComponent<LapRaceCar>();
			if ((bool)component && CarPlaceId == component.GetRaceCarId())
			{
				IsBusy = true;
				component.StartPitStop();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!FlagsStorage.IsPhotoMode && other.gameObject.layer == 31)
		{
			LapRaceCar component = other.transform.parent.GetComponent<LapRaceCar>();
			if ((bool)component)
			{
				IsBusy = false;
				component.StopPitStop();
			}
		}
	}

	private async UniTaskVoid DelayedChangeState(bool state)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(2.0));
		IsBusy = state;
	}
}
