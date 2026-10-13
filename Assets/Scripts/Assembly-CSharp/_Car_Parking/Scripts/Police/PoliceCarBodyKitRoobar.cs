using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarBodyKitRoobar : PoliceCarBodyKit
	{
		private BodyKits _bodyKits;

		private bool _initializedDefault;

		private Vector3 _defaultPosition;

		private Quaternion _defaultRotation;

		private Vector3 _defaultScale;

		private CancellationTokenSource _cancellationTokenSourceInitFirstPosition;

		public override PoliceBodyKitType BodyKitType => PoliceBodyKitType.Roobar;

		private void OnEnable()
		{
			if (!_bodyKits)
			{
				_bodyKits = GetComponentInParent<BodyKits>();
			}
			if ((bool)_bodyKits)
			{
				_bodyKits.OnInstalledBodyKitUpdated += CarInstalledBodyKitUpdated;
				CancelInitFirstPosition();
				_cancellationTokenSourceInitFirstPosition = new CancellationTokenSource();
				InitFirstPosition(_cancellationTokenSourceInitFirstPosition.Token).Forget();
			}
		}

		private void OnDisable()
		{
			CancelInitFirstPosition();
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
				CarInstalledBodyKitUpdated(_bodyKits, _Car_Parking.Scripts.Drag.BodyKitType.FrontBumper, _bodyKits.InstalledFrontBumper);
			}
		}

		private void CarInstalledBodyKitUpdated(BodyKits bodyKits, BodyKitType bodyKitType, GameObject bodyKitGameObject)
		{
			if (bodyKitType != _Car_Parking.Scripts.Drag.BodyKitType.FrontBumper)
			{
				return;
			}
			if (!_initializedDefault)
			{
				_initializedDefault = true;
				_defaultPosition = base.transform.localPosition;
				_defaultRotation = base.transform.localRotation;
				_defaultScale = base.transform.localScale;
			}
			if ((bool)bodyKitGameObject && bodyKitGameObject.TryGetComponentInChildren<PoliceRoobarSpecificTransform>(out var component))
			{
				base.transform.position = component.GetGlobalPosition(base.Index);
				base.transform.rotation = component.GetGlobalRotation(base.Index);
				base.transform.localScale = component.GetScale(base.Index);
				return;
			}
			if ((bool)bodyKitGameObject)
			{
				Transform transform = bodyKitGameObject.transform.FindChildRecursive("roo bar point fbumper");
				if ((bool)transform)
				{
					base.transform.position = transform.position;
					base.transform.rotation = transform.rotation;
					base.transform.localScale = transform.localScale;
					return;
				}
			}
			base.transform.localPosition = _defaultPosition;
			base.transform.localRotation = _defaultRotation;
			base.transform.localScale = _defaultScale;
		}

		private void CancelInitFirstPosition()
		{
			_cancellationTokenSourceInitFirstPosition?.Cancel();
			_cancellationTokenSourceInitFirstPosition?.Dispose();
			_cancellationTokenSourceInitFirstPosition = null;
		}
	}
}
