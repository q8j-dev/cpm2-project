using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	public class FsoStrobeLightButton : MonoBehaviour
	{
		private void Start()
		{
			Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				TryDisableButton();
			}).AddTo(base.gameObject);
			Observable.Interval(TimeSpan.FromSeconds(5.0)).Take(10).Subscribe((long _) =>
			{
				TryDisableButton();
			})
				.AddTo(base.gameObject);
			Observable.TimerFrame(1).Subscribe((long _) =>
			{
				V.CoreGameManager.CarOutInControl.OnCarMenuSet += UpdateButtonState;
			}).AddTo(base.gameObject);
		}

		private void OnDestroy()
		{
			V.CoreGameManager.CarOutInControl.OnCarMenuSet -= UpdateButtonState;
		}

		private void UpdateButtonState(bool isDriver)
		{
			if (isDriver)
			{
				TryDisableButton();
			}
		}

		private void TryDisableButton()
		{
			Button component = GetComponent<Button>();
			Image component2 = GetComponent<Image>();
			FsoStrobeLights component3;
			if (V.Car == null)
			{
				component2.enabled = false;
				component.enabled = false;
			}
			else if (V.Car.TryGetComponentInChildren<FsoStrobeLights>(out component3))
			{
				if (component3.HasFso)
				{
					component2.enabled = true;
					component.enabled = true;
				}
				else
				{
					component2.enabled = false;
					component.enabled = false;
				}
			}
			else
			{
				component2.enabled = false;
				component.enabled = false;
			}
		}

		public void FsoClick()
		{
			if (V.Car.TryGetComponentInChildren<FsoStrobeLights>(out var component))
			{
				component.ToggleFromButton();
			}
			else
			{
				Debug.LogWarning("No FsoStrobeLights on this car.");
			}
		}
	}
}
