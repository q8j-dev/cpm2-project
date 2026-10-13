using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Steam.Ui
{
	public class AimReticleController : MonoBehaviour
	{
		private EventRegistry _eventRegistry;

		private InputMapSetEvent _inputMapSetEvent;

		private FreeDriveProvider _freeDriveProvider;

		private Camera _camera;

		private RegistryHandle<BoolEvent> _aimAtHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.AimAt);

		private CanvasGroup _reticleCanvasGroup;

		private bool _reticleVisible;

		private bool _groupOwnsAimReticle;

		private PointerEventData _pointerEventData;

		private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>(16);

		private readonly RaycastHit[] _hitBuffer = new RaycastHit[5];

		private Vector2 _screenCenter;

		private int _lastResWidth;

		private int _lastResHeight;

		private static bool PointerAimActive
		{
			get
			{
				if (Cursor.visible)
				{
					return Cursor.lockState != CursorLockMode.Locked;
				}
				return false;
			}
		}

		[Inject]
		public void Construct(EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, FreeDriveProvider freeDriveProvider)
		{
			_eventRegistry = eventRegistry;
			_inputMapSetEvent = inputMapSetEvent;
			_freeDriveProvider = freeDriveProvider;
		}

		private void Start()
		{
			_inputMapSetEvent.Subscribe(OnInputGroupChanged);
			_aimAtHandle.Get(_eventRegistry).Subscribe(AimAt);
			_reticleCanvasGroup = GetComponentInChildren<CanvasGroup>(includeInactive: true);
			if (!_reticleCanvasGroup)
			{
				GameObject gameObject = GetComponentInChildren<Image>(includeInactive: true).gameObject;
				_reticleCanvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
			_pointerEventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
			UpdateScreenCenter();
			RefreshGroupOwnsAimReticle();
			SetReticleVisible(visible: false);
		}

		private void Update()
		{
			if (_groupOwnsAimReticle && FlagsStorage.IsMultiFree && PointerAimActive && !_reticleVisible)
			{
				Mouse current = Mouse.current;
				if (current != null && current.leftButton.wasReleasedThisFrame && (!UnityEngine.EventSystems.EventSystem.current || !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()))
				{
					TryHitPhysics(current.position.ReadValue());
				}
			}
		}

		private void OnDestroy()
		{
			_inputMapSetEvent.Unsubscribe(OnInputGroupChanged);
			_aimAtHandle.Get(_eventRegistry)?.Unsubscribe(AimAt);
		}

		private void OnInputGroupChanged(InputGroupsEnum group)
		{
			SetReticleVisible(visible: false);
			RefreshGroupOwnsAimReticle();
		}

		private void RefreshGroupOwnsAimReticle()
		{
			_groupOwnsAimReticle = false;
			if (!_inputMapSetEvent.TryGetCurrentMaps(out var maps))
			{
				return;
			}
			foreach (InputMapEnum item in maps)
			{
				if (item == InputMapEnum.AimReticle)
				{
					_groupOwnsAimReticle = true;
					break;
				}
			}
		}

		private void SetReticleVisible(bool visible)
		{
			_reticleVisible = visible;
			if ((bool)_reticleCanvasGroup)
			{
				_reticleCanvasGroup.alpha = (visible ? 1f : 0f);
			}
		}

		private void AimAt(bool started)
		{
			if (!_reticleVisible & started)
			{
				SetReticleVisible(visible: true);
			}
			else if (!started && _reticleVisible)
			{
				UpdateScreenCenter();
				if (!TryHitWorldSpaceUI())
				{
					TryHitPhysics(_screenCenter);
				}
				SetReticleVisible(visible: false);
			}
		}

		private bool TryHitWorldSpaceUI()
		{
			if (!UnityEngine.EventSystems.EventSystem.current)
			{
				return false;
			}
			_pointerEventData.position = _screenCenter;
			_raycastResults.Clear();
			UnityEngine.EventSystems.EventSystem.current.RaycastAll(_pointerEventData, _raycastResults);
			foreach (RaycastResult raycastResult in _raycastResults)
			{
				if ((bool)raycastResult.gameObject)
				{
					if (raycastResult.gameObject.GetComponentInParent<IPointerClickHandler>() != null)
					{
						ExecuteEvents.Execute(raycastResult.gameObject, _pointerEventData, ExecuteEvents.pointerClickHandler);
						return true;
					}
					Selectable componentInParent = raycastResult.gameObject.GetComponentInParent<Selectable>();
					if ((bool)componentInParent && componentInParent.interactable)
					{
						ExecuteEvents.Execute(componentInParent.gameObject, _pointerEventData, ExecuteEvents.pointerClickHandler);
						return true;
					}
				}
			}
			return false;
		}

		private void TryHitPhysics(Vector2 screenPoint)
		{
			if (!_camera)
			{
				_camera = Camera.main;
			}
			if (!_camera)
			{
				return;
			}
			Ray ray = _camera.ScreenPointToRay(screenPoint);
			int mask = LayerMask.GetMask("car", "UI");
			int num = Physics.RaycastNonAlloc(ray, _hitBuffer, float.PositiveInfinity, mask, QueryTriggerInteraction.Collide);
			if (num == 0)
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = _hitBuffer[i];
				if (!raycastHit.collider)
				{
					continue;
				}
				BuyCarWorldWindow componentInParent = raycastHit.collider.GetComponentInParent<BuyCarWorldWindow>();
				if ((bool)componentInParent)
				{
					componentInParent.TryBuyFromAim();
					break;
				}
				if ((bool)raycastHit.collider.attachedRigidbody && raycastHit.collider.attachedRigidbody.gameObject.CompareTag("Car"))
				{
					GameObject gameObject = raycastHit.collider.attachedRigidbody.gameObject;
					if (gameObject != V.Car)
					{
						_freeDriveProvider.FreeDriveControl?.AimAtCar(gameObject);
					}
					break;
				}
			}
		}

		private void UpdateScreenCenter()
		{
			if (_lastResWidth != Screen.width || _lastResHeight != Screen.height)
			{
				_lastResWidth = Screen.width;
				_lastResHeight = Screen.height;
				_screenCenter = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.7f);
			}
		}
	}
}
