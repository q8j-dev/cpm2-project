using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Audio;

namespace CPM.Scripts.Garage
{
	[RequireComponent(typeof(Collider))]
	public class GarageDoor : MonoBehaviour
	{
		[SerializeField]
		private BoxCollider doorCollider;

		[SerializeField]
		private Animation animationComp;

		[SerializeField]
		private BoxCollider checkCollider;

		[SerializeField]
		private LayerMask obstaclesLayers;

		[SerializeField]
		private float speedMultiplier = 1f;

		private string _doorClipName;

		private bool _isOpen;

		[HideInInspector]
		public Collider[] checkResults;

		[HideInInspector]
		private RaycastHit[] _results;

		private CancellationTokenSource _autoCloseCts;

		private CancellationTokenSource _playSoundCancellationTokenSource;

		private void Awake()
		{
			if (doorCollider == null)
			{
				doorCollider = GetComponent<BoxCollider>();
			}
			if (animationComp == null)
			{
				animationComp = GetComponentInChildren<Animation>();
			}
			_doorClipName = animationComp.clip.name;
			checkResults = new Collider[5];
			obstaclesLayers = (int)obstaclesLayers | (1 << LayerMask.NameToLayer("car"));
			obstaclesLayers = (int)obstaclesLayers | (1 << LayerMask.NameToLayer("DriverPerson"));
		}

		private void OnEnable()
		{
			Close();
		}

		private void OnDestroy()
		{
			AudioWrapper.PostEvent("Stop_Gate_Opening_Loop", base.gameObject);
			CommonExtensions.CancelAndDisposeToken(ref _autoCloseCts);
			CommonExtensions.CancelAndDisposeToken(ref _playSoundCancellationTokenSource);
		}

		public void Open()
		{
			if (!_isOpen)
			{
				animationComp[_doorClipName].speed = 1f * speedMultiplier;
				animationComp[_doorClipName].time = 0f;
				animationComp.Play(_doorClipName);
				PlaySound();
				doorCollider.enabled = false;
				_isOpen = true;
				CommonExtensions.CancelAndDisposeToken(ref _autoCloseCts);
				_autoCloseCts = new CancellationTokenSource();
				TryAutoClose(_autoCloseCts.Token).Forget();
			}
		}

		public void Close()
		{
			if (_isOpen)
			{
				CommonExtensions.CancelAndDisposeToken(ref _autoCloseCts);
				if ((bool)checkCollider && CheckForObstacles(out var _))
				{
					_autoCloseCts = new CancellationTokenSource();
					TryAutoClose(_autoCloseCts.Token).Forget();
					return;
				}
				animationComp[_doorClipName].speed = -1f * speedMultiplier;
				animationComp[_doorClipName].time = animationComp[_doorClipName].length;
				animationComp.Play(_doorClipName);
				PlaySound();
				doorCollider.enabled = true;
				_isOpen = false;
			}
		}

		private async UniTaskVoid TryAutoClose(CancellationToken token)
		{
			if (!checkCollider)
			{
				return;
			}
			int resCount;
			while (CheckForObstacles(out resCount))
			{
				await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
				if (token.IsCancellationRequested)
				{
					return;
				}
			}
			Close();
		}

		public bool CheckForObstacles(out int resCount)
		{
			Bounds bounds = checkCollider.bounds;
			return (resCount = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, checkResults, checkCollider.transform.rotation, obstaclesLayers)) > 0;
		}

		private void PlaySound()
		{
			CommonExtensions.CancelAndDisposeToken(ref _playSoundCancellationTokenSource);
			_playSoundCancellationTokenSource = new CancellationTokenSource();
			CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_playSoundCancellationTokenSource.Token, base.destroyCancellationToken);
			PlaySoundAsync(cancellationTokenSource.Token).Forget();
		}

		private async UniTask PlaySoundAsync(CancellationToken cancellationToken)
		{
			AudioWrapper.PostEvent("Stop_Gate_Opening_Loop", base.gameObject);
			await UniTask.Yield();
			if (!cancellationToken.IsCancellationRequested)
			{
				AudioWrapper.PostEvent("Play_Gate_Opening_Loop", base.gameObject);
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (!cancellationToken.IsCancellationRequested)
				{
					AudioWrapper.PostEvent("Stop_Gate_Opening_Loop", base.gameObject);
				}
			}
		}
	}
}
