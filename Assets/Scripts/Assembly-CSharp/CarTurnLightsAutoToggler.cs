using System;
using System.Collections;
using UnityEngine;

public class CarTurnLightsAutoToggler
{
	[Serializable]
	public class Data
	{
		public float targetRotationToPrepareTurnOff = 30f;

		public float minRotationToInitiateTurnOff = 10f;

		public float rotationTurnOffCheckStep = 0.8f;
	}

	private Data data;

	private Transform carTransform;

	private CustomCour<int> assessingTurning;

	private Action OnTurnOffPointReached;

	private WaitForSeconds _cachedWait;

	private float CurrentRotation => carTransform.rotation.eulerAngles.y;

	public CarTurnLightsAutoToggler(Transform transform, MonoBehaviour mono, Action turnOffCallback, Data data)
	{
		carTransform = transform;
		OnTurnOffPointReached = turnOffCallback;
		assessingTurning = new CustomCour<int>(mono, AssessingTurning);
		this.data = data;
		_cachedWait = new WaitForSeconds(data.rotationTurnOffCheckStep);
	}

	public void TurnLightToggledOn(int axis)
	{
		assessingTurning.Stop();
		assessingTurning.Run(axis);
	}

	public void TurnLightToggledOff()
	{
		assessingTurning.Stop();
	}

	private IEnumerator AssessingTurning(int axis)
	{
		Debug.Log($"Vehicle current rotation{CurrentRotation}");
		float startingRotation = CurrentRotation;
		float targetRotationUpper = CurrentRotation + data.targetRotationToPrepareTurnOff;
		float targetRotationLower = CurrentRotation - data.targetRotationToPrepareTurnOff;
		while (!(CurrentRotation >= targetRotationUpper) && !(CurrentRotation <= targetRotationLower))
		{
			yield return null;
		}
		Debug.Log("First turning off point reached");
		if (((CurrentRotation > startingRotation) ? 1 : (-1)) == axis)
		{
			while (true)
			{
				float minRotationToTurnoff = CurrentRotation + data.minRotationToInitiateTurnOff * (float)axis;
				yield return _cachedWait;
				if (axis > 0)
				{
					if (CurrentRotation <= minRotationToTurnoff)
					{
						break;
					}
				}
				else if (CurrentRotation >= minRotationToTurnoff)
				{
					break;
				}
			}
		}
		else
		{
			yield return _cachedWait;
		}
		Debug.Log("Second turning off point reached");
		OnTurnOffPointReached?.Invoke();
	}
}
