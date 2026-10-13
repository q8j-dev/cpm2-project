using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.EditableHUD;

namespace _Car_Parking.Scripts.WindowSystem
{
	[RequireComponent(typeof(RectTransform))]
	public sealed class HudSlotFollower : MonoBehaviour
	{
		[SerializeField]
		private HudSlot Slot;

		[SerializeField]
		private bool CanReturnHome = true;

		[SerializeField]
		private int SettleFrames = 3;

		private RectTransform _rect;

		private EditableHUDElement _editableElement;

		private RectTransform _cachedParent;

		private Camera _cachedParentCamera;

		private Transform _homeParent;

		private Vector2 _homePosition;

		private RectTransform _anchor;

		private Camera _anchorCamera;

		private bool _displaced;

		private CancellationTokenSource _settleCts;

		private bool IsOwnedByEditableHUD
		{
			get
			{
				if (!EditableHUDEditingSession.IsOpen)
				{
					if ((bool)_editableElement)
					{
						return _editableElement.HaveChanges;
					}
					return false;
				}
				return true;
			}
		}

		private void Awake()
		{
			_rect = (RectTransform)base.transform;
			_editableElement = GetComponent<EditableHUDElement>();
		}

		private void OnEnable()
		{
			HudSlotRegistry.OnChanged += OnSlotChanged;
			EditableHUDEditingSession.OnChanged += Resolve;
			if ((bool)_editableElement)
			{
				_editableElement.OnChangesChanged += OnElementChangesChanged;
			}
			Resolve();
		}

		private void OnDisable()
		{
			HudSlotRegistry.OnChanged -= OnSlotChanged;
			EditableHUDEditingSession.OnChanged -= Resolve;
			if ((bool)_editableElement)
			{
				_editableElement.OnChangesChanged -= OnElementChangesChanged;
			}
			StopSettling();
		}

		private void OnDestroy()
		{
			StopSettling();
		}

		private void OnTransformParentChanged()
		{
			if (_displaced)
			{
				Resolve();
			}
		}

		private void OnRectTransformDimensionsChange()
		{
			if (_displaced)
			{
				FollowAndSettle();
			}
		}

		private void OnSlotChanged(HudSlot slot)
		{
			if (slot == Slot)
			{
				Resolve();
			}
		}

		private void OnElementChangesChanged(EditableHUDElement element)
		{
			Resolve();
		}

		private void Resolve()
		{
			RectTransform rectTransform = null;
			if (!IsOwnedByEditableHUD && HudSlotRegistry.TryGet(Slot, out var anchor))
			{
				rectTransform = anchor;
			}
			if ((bool)rectTransform)
			{
				if (!_displaced)
				{
					_homeParent = _rect.parent;
					_homePosition = _rect.anchoredPosition;
					_displaced = true;
				}
				_anchor = rectTransform;
				_anchorCamera = CameraOf(rectTransform);
				FollowAndSettle();
			}
			else
			{
				StopSettling();
				if (_displaced)
				{
					Release();
				}
				_anchor = null;
				_anchorCamera = null;
			}
		}

		private void Release()
		{
			if (CanReturnHome && _rect.parent == _homeParent)
			{
				_rect.anchoredPosition = _homePosition;
			}
			_displaced = false;
			_homeParent = null;
		}

		private void FollowAndSettle()
		{
			Follow();
			StopSettling();
			_settleCts = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken);
			SettleAsync(_settleCts.Token).Forget();
		}

		private async UniTaskVoid SettleAsync(CancellationToken cancellationToken)
		{
			for (int i = 0; i < SettleFrames; i++)
			{
				await UniTask.NextFrame(PlayerLoopTiming.LastPostLateUpdate, cancellationToken);
				if (!_displaced)
				{
					break;
				}
				Follow();
			}
		}

		private void StopSettling()
		{
			CommonExtensions.CancelAndDisposeToken(ref _settleCts);
		}

		private void Follow()
		{
			RectTransform rectTransform = ResolveParent();
			if ((bool)rectTransform && (bool)_anchor)
			{
				Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_anchorCamera, _anchor.position);
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, _cachedParentCamera, out var localPoint))
				{
					_rect.anchoredPosition = localPoint - AnchorReferencePoint(rectTransform);
				}
			}
		}

		private Vector2 AnchorReferencePoint(RectTransform parent)
		{
			Rect rect = parent.rect;
			Vector2 vector = (_rect.anchorMin + _rect.anchorMax) * 0.5f;
			return new Vector2(rect.xMin + vector.x * rect.width, rect.yMin + vector.y * rect.height);
		}

		private RectTransform ResolveParent()
		{
			RectTransform rectTransform = _rect.parent as RectTransform;
			if (rectTransform != _cachedParent)
			{
				_cachedParent = rectTransform;
				_cachedParentCamera = (rectTransform ? CameraOf(rectTransform) : null);
			}
			return rectTransform;
		}

		private static Camera CameraOf(RectTransform rect)
		{
			Canvas componentInParent = rect.GetComponentInParent<Canvas>();
			if (!componentInParent)
			{
				return null;
			}
			componentInParent = componentInParent.rootCanvas;
			if (componentInParent.renderMode != RenderMode.ScreenSpaceOverlay)
			{
				return componentInParent.worldCamera;
			}
			return null;
		}
	}
}
