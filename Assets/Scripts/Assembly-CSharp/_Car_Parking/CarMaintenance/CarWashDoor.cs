using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace _Car_Parking.CarMaintenance
{
	public class CarWashDoor : MonoBehaviour
	{
		private const string PlayCarWashDoor = "Play_carwash_door";

		private const string StopCarWashDoor = "Stop_carwash_door";

		[SerializeField]
		private BoxCollider _collider;

		[SerializeField]
		private BoxCollider _trigger;

		[SerializeField]
		private Animation _animation;

		[SerializeField]
		private LayerMask _layerMask;

		private string _doorClipName;

		private bool _isDoorOpen;

		private Collider[] _checkResults = new Collider[5];

		private RaycastHit[] _raycastHits = new RaycastHit[5];

		private CancellationTokenSource _autoCloseCts;

		private TimeSpan _autoCloseDelay;

		private void Awake()
		{
			if (_collider == null)
			{
				_collider = GetComponentInChildren<BoxCollider>();
			}
			if (_animation == null)
			{
				_animation = GetComponentInChildren<Animation>();
			}
			if (_trigger == null)
			{
				_trigger = GetComponent<BoxCollider>();
			}
			_doorClipName = _animation.clip.name;
			_layerMask = 1 << LayerMask.NameToLayer("car");
			_layerMask = (int)_layerMask | (1 << LayerMask.NameToLayer("DriverPerson"));
			_layerMask = (int)_layerMask | (1 << LayerMask.NameToLayer("Trailer"));
			_layerMask = (int)_layerMask | (1 << LayerMask.NameToLayer("CarBody"));
			_autoCloseDelay = TimeSpan.FromSeconds(2.0);
		}

		private void OnDestroy()
		{
			_autoCloseCts?.Cancel();
			_autoCloseCts?.Dispose();
			_autoCloseCts = null;
		}

		public void OpenDoor()
		{
			if (!_isDoorOpen)
			{
				_animation[_doorClipName].speed = 1f;
				_animation[_doorClipName].time = 0f;
				_animation.Play(_doorClipName);
				PlaySound();
				_collider.enabled = false;
				_isDoorOpen = true;
				CancelAutoClose();
				StartAutoClose();
			}
		}

		public void CloseDoor()
		{
			if (_isDoorOpen)
			{
				CancelAutoClose();
				if ((bool)_trigger && CheckForObstacles(out var _))
				{
					StartAutoClose();
					return;
				}
				_animation[_doorClipName].speed = -1f;
				_animation[_doorClipName].time = _animation[_doorClipName].length;
				_animation.Play(_doorClipName);
				PlaySound();
				_collider.enabled = true;
				_isDoorOpen = false;
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Car") || other.CompareTag("OnlineCar"))
			{
				OpenDoor();
			}
		}

		public void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Car") || other.CompareTag("OnlineCar"))
			{
				CloseDoor();
			}
		}

		private void PlaySound()
		{
			AudioWrapper.PostEvent("Play_carwash_door", base.gameObject);
			Observable.Timer(TimeSpan.FromSeconds(1.2000000476837158)).Subscribe((long _) =>
			{
				AudioWrapper.PostEvent("Stop_carwash_door", base.gameObject);
			}).AddTo(this);
		}

		private bool CheckForObstacles(out int resCount)
		{
			try
			{
				Bounds bounds = _trigger.bounds;
				resCount = Physics.BoxCastNonAlloc(bounds.center, bounds.extents, Vector3.up, _raycastHits, Quaternion.identity, 0f, _layerMask, QueryTriggerInteraction.Ignore);
				return resCount > 0;
			}
			catch
			{
			}
			resCount = 0;
			return true;
		}

		private void CancelAutoClose()
		{
			if (_autoCloseCts != null)
			{
				_autoCloseCts.Cancel();
				_autoCloseCts.Dispose();
				_autoCloseCts = null;
			}
		}

		private void StartAutoClose()
		{
			_autoCloseCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			TryAutoCloseAsync(_autoCloseCts.Token).Forget();
		}

		private async UniTask TryAutoCloseAsync(CancellationToken ct)
		{
			try
			{
				if ((bool)_trigger)
				{
					int resCount;
					while (CheckForObstacles(out resCount))
					{
						await UniTask.Delay(_autoCloseDelay, ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					}
					CloseDoor();
				}
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				if (_autoCloseCts != null && _autoCloseCts.Token == ct)
				{
					_autoCloseCts.Dispose();
					_autoCloseCts = null;
				}
			}
		}
	}
}
