using System;
using System.Collections.Generic;
using System.Linq;
using AK.Wwise;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.WwisePrefabs.WiseSoundScripts
{
	public class WwiseAmbienceStateWithParts : MonoBehaviour, IAmbienceStateZone
	{
		public State OntriggerEnterState;

		public State OntriggerExitState;

		[SerializeField]
		private List<WwiseAmbiencePart> WwiseAmbienceParts;

		private IDisposable _checkTriggerEnterDisposable;

		private bool isStateEnabled;

		public string MainCameraTag { get; private set; } = "MainCamera";

		private void Start()
		{
			_checkTriggerEnterDisposable = Observable.Interval(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				CheckTriggerEnter();
			}).AddTo(this);
		}

		public void ApplyEnterState()
		{
			OntriggerEnterState.SetValue();
			isStateEnabled = true;
		}

		private void CheckTriggerEnter()
		{
			if (WwiseAmbienceParts.Any((WwiseAmbiencePart part) => part.MainCamera))
			{
				if (!isStateEnabled)
				{
					OntriggerEnterState.SetValue();
					isStateEnabled = true;
				}
			}
			else if (isStateEnabled)
			{
				OntriggerExitState.SetValue();
				isStateEnabled = false;
			}
		}

		private void OnDestroy()
		{
			if (isStateEnabled)
			{
				OntriggerExitState.SetValue();
			}
			_checkTriggerEnterDisposable?.Dispose();
			_checkTriggerEnterDisposable = null;
		}
	}
}
