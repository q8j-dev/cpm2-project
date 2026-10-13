using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class CircuitRacingWaitingWindow : MonoBehaviour
	{
		private const float RotateSpeed = 360f;

		[SerializeField]
		private Transform WheelImage;

		[SerializeField]
		private float MaxLoadingTime = 8f;

		[SerializeField]
		private GameObject Root;

		private float _maxLoadingTimeNow;

		private bool _isDestroyed;

		[Inject]
		public InputMapSetEvent inputMapSetEvent;

		private void OnEnable()
		{
			_maxLoadingTimeNow = MaxLoadingTime;
			Root.SetActive(value: true);
			inputMapSetEvent.TrySetGroup(InputGroupsEnum.NoInput);
		}

		private void OnDisable()
		{
			inputMapSetEvent.TrySetPreviousGroup();
		}

		private void Update()
		{
			WheelImage.Rotate(Vector3.back, 360f * Time.unscaledTime, Space.Self);
			if (inputMapSetEvent.CurrentInputGroup != InputGroupsEnum.NoInput)
			{
				inputMapSetEvent.TrySetGroup(InputGroupsEnum.NoInput);
			}
		}

		public async UniTaskVoid DelayedDisableWindow(float duration)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(duration));
			if (!_isDestroyed)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
