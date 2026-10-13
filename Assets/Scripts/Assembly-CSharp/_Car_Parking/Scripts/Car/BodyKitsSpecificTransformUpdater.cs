using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Car
{
	public class BodyKitsSpecificTransformUpdater : MonoBehaviour
	{
		[SerializeField]
		private string ID = default;

		private BodyKits _bodyKits;

		private bool _initializedDefault;

		private Vector3 _defaultPosition;

		private Quaternion _defaultRotation;

		private Vector3 _defaultScale;

		private CancellationTokenSource _cancellationTokenSourceInitFirstPosition;

		private void OnEnable()
		{
			if (!_bodyKits)
			{
				_bodyKits = GetComponentInParent<BodyKits>();
			}
			if ((bool)_bodyKits)
			{
				_bodyKits.OnInstalledBodyKitUpdated += CarInstalledBodyKitUpdated;
				CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceInitFirstPosition);
				_cancellationTokenSourceInitFirstPosition = new CancellationTokenSource();
				InitFirstPosition(_cancellationTokenSourceInitFirstPosition.Token).Forget();
			}
		}

		private void OnDisable()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceInitFirstPosition);
			if ((bool)_bodyKits)
			{
				_bodyKits.OnInstalledBodyKitUpdated -= CarInstalledBodyKitUpdated;
			}
		}

		private async UniTaskVoid InitFirstPosition(CancellationToken cancellationToken)
		{
			await UniTask.Yield();
			if ((bool)_bodyKits && !cancellationToken.IsCancellationRequested)
			{
				CarInstalledBodyKitUpdated(_bodyKits, BodyKitType.FrontBumper, _bodyKits.InstalledFrontBumper);
			}
		}

		private void CarInstalledBodyKitUpdated(BodyKits bodyKits, BodyKitType bodyKitType, GameObject bodyKitGameObject)
		{
			if (!_initializedDefault)
			{
				_initializedDefault = true;
				_defaultPosition = base.transform.localPosition;
				_defaultRotation = base.transform.localRotation;
				_defaultScale = base.transform.localScale;
			}
			if ((bool)bodyKitGameObject && bodyKitGameObject.TryGetComponentInChildren<BodyKitsSpecificTransform>(out var component) && component.ContainsID(ID))
			{
				base.transform.position = component.GetGlobalPosition(ID);
				base.transform.rotation = component.GetGlobalRotation(ID);
				base.transform.localScale = component.GetScale(ID);
			}
			else
			{
				base.transform.localPosition = _defaultPosition;
				base.transform.localRotation = _defaultRotation;
				base.transform.localScale = _defaultScale;
			}
		}
	}
}
