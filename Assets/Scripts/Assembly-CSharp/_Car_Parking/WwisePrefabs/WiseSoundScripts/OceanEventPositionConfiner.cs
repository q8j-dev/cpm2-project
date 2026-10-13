using System;
using System.Collections;
using AK.Wwise;
using UnityEngine;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;

namespace _Car_Parking.WwisePrefabs.WiseSoundScripts
{
	public class OceanEventPositionConfiner : MonoBehaviour
	{
		[Header("Event to clamp to AkAudioListener")]
		public AK.Wwise.Event Event = new AK.Wwise.Event();

		public SkyboxBlender DayNightCont;

		public RTPC daynightrtpc;

		[Header("Settings")]
		public bool lowInterval = true;

		public const float UPDATE_INTERNAL = 0.2f;

		private IEnumerator positionClamperRoutine;

		private Collider trigger;

		private Transform targetTransform;

		private GameObject eventEmitter;

		private WaitForSecondsRealtime _waitForSecondsRealtime = new WaitForSecondsRealtime(0.2f);

		private void Awake()
		{
			Init();
		}

		private void Start()
		{
			OnStart();
		}

		private void Init()
		{
			trigger = GetComponent<Collider>();
			trigger.isTrigger = true;
			eventEmitter = new GameObject("Clamped Emitter");
			eventEmitter.transform.parent = base.transform;
			eventEmitter.AddComponent<SphereCollider>().isTrigger = true;
			eventEmitter.AddComponent<AkGameObj>();
			V.OnSkyboxChanged = (Action)Delegate.Combine(V.OnSkyboxChanged, new Action(SetRTPCValue));
			DayNightCont = TimeProvider.skyboxBlender;
			InvokeRepeating("UpdateNights", 5f, 5f);
		}

		private void OnStart()
		{
			AkAudioListener componentInChildren = V.CoreGameManager.GetComponentInChildren<AkAudioListener>();
			if (componentInChildren != null)
			{
				targetTransform = componentInChildren.transform;
			}
			else
			{
				base.enabled = false;
			}
			Event.Post(eventEmitter);
			positionClamperRoutine = ClampEmitterPosition();
			StartCoroutine(positionClamperRoutine);
			SetRTPCValue();
			if (DayNightCont == null)
			{
				DayNightCont = TimeProvider.skyboxBlender;
			}
		}

		private void OnDestroy()
		{
			Event.Stop(eventEmitter);
			if (positionClamperRoutine != null)
			{
				StopCoroutine(positionClamperRoutine);
			}
			V.OnSkyboxChanged = (Action)Delegate.Remove(V.OnSkyboxChanged, new Action(SetRTPCValue));
		}

		private IEnumerator ClampEmitterPosition()
		{
			while (true)
			{
				Vector3 position = trigger.ClosestPoint(targetTransform.position);
				eventEmitter.transform.position = position;
				yield return _waitForSecondsRealtime;
			}
		}

		private void UpdateNights()
		{
			if (TimeProvider.skyboxSelectedRuntime == 2 && daynightrtpc != null && DayNightCont == null)
			{
				DayNightCont = TimeProvider.skyboxBlender;
			}
		}

		private void SetRTPCValue()
		{
			switch (TimeProvider.skyboxSelectedRuntime)
			{
			case 0:
				daynightrtpc.SetValue(eventEmitter, 0f);
				break;
			case 2:
				daynightrtpc.SetValue(eventEmitter, 0.5f);
				break;
			case 3:
				daynightrtpc.SetValue(eventEmitter, 1f);
				break;
			case 1:
				break;
			}
		}
	}
}
