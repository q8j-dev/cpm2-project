using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.Scripts.SignBehaviour
{
	[CreateAssetMenu(fileName = "HydrantBehaviour", menuName = "Scriptable/SignHandler/HydrantBehaviour")]
	public class HydrantBehaviourSO : SignHandlerBehaviourBaseSO
	{
		private const string FireHydrantEvent = "Play_Fire_Hydrant";

		[SerializeField]
		private ParticleSystem WaterParticleSystem;

		private SignHandler _signHandler;

		private int _roadLayer;

		private void OnEnable()
		{
			_roadLayer = LayerMask.NameToLayer("Road");
		}

		public override void Execute(SignHandler signHandler)
		{
			_signHandler = signHandler;
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				if ((bool)signHandler && signHandler.TryGetComponent<Rigidbody>(out var component))
				{
					component.velocity = Vector3.zero;
					component.angularVelocity = Vector3.zero;
					component.AddForce(Vector3.up * 200f, ForceMode.Impulse);
				}
			}).AddTo(_signHandler);
			ParticleSystem particleSystem = UnityEngine.Object.Instantiate(WaterParticleSystem, _signHandler.transform.position, Quaternion.identity);
			particleSystem.Play(withChildren: false);
			GameObject inGameObjectID = particleSystem.gameObject;
			AudioWrapper.PostEvent("Play_water_splash", inGameObjectID);
			Observable.Timer(TimeSpan.FromSeconds(particleSystem.main.duration)).Subscribe((long _) =>
			{
				if ((bool)inGameObjectID)
				{
					AudioWrapper.PostEvent("Stop_water_splash", inGameObjectID);
				}
			});
			particleSystem.OnDestroyAsObservable().Subscribe((Unit _) =>
			{
				AudioWrapper.PostEvent("Stop_water_splash", inGameObjectID);
			});
		}

		public override void OnCollisionEnterExecute(SignHandler signHandler, Collision other)
		{
			if (_signHandler.IsTriggered && other.gameObject.layer == _roadLayer)
			{
				AudioWrapper.PostEvent("Play_Fire_Hydrant", signHandler.gameObject);
			}
		}
	}
}
