using System;
using UniRx;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class AkTriggerObjectSetter : MonoBehaviour
	{
		private AkTriggerEnter _triggerEnter;

		private AkTriggerExit _triggerExit;

		private IDisposable _disposableCheckTriggers;

		private void Start()
		{
			CheckTriggers();
			StartCheckCoreGameManager();
		}

		private void CheckTriggers()
		{
			_disposableCheckTriggers = Observable.Timer(TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				if (!_triggerEnter || !_triggerExit)
				{
					_triggerEnter = GetComponent<AkTriggerEnter>();
					_triggerExit = GetComponent<AkTriggerExit>();
				}
				if ((bool)_triggerEnter && (bool)_triggerExit)
				{
					_disposableCheckTriggers?.Dispose();
					_disposableCheckTriggers = null;
				}
			}).AddTo(this);
			Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
			{
				_disposableCheckTriggers?.Dispose();
			}).AddTo(this);
		}

		private void StartCheckCoreGameManager()
		{
			(from _ in Observable.EveryUpdate()
				where (bool)V.CoreGameManager && (bool)_triggerEnter && _triggerEnter.triggerObject == null && (bool)_triggerExit && _triggerExit.triggerObject == null
				select _).Subscribe((long _) =>
			{
				SetTriggerObject(V.CoreGameManager.CameraListener);
			}).AddTo(this);
		}

		private void SetTriggerObject(GameObject triggerObject)
		{
			_triggerEnter.triggerObject = triggerObject;
			_triggerExit.triggerObject = triggerObject;
		}
	}
}
