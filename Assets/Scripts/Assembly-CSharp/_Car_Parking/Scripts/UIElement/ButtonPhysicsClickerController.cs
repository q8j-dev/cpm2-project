using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.UIElement
{
	public class ButtonPhysicsClickerController : MonoBehaviour
	{
		private Camera _mainCam;

		private int _layerMask;

		private UITouchHelper _uiTouchHelper;

		private EventRegistry _eventRegistry;

		private UiNavigationValueEvent _uiNav;

		private RegistryHandle<BoolEvent> _onConfirmClickHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnConfirmClick);

		private string UITouchID => base.gameObject.GetInstanceID().ToString();

		[Inject]
		public void Construct(EventRegistry eventRegistry, UiNavigationValueEvent uiNav, UITouchHelper uiTouchHelper)
		{
			_uiTouchHelper = uiTouchHelper;
			_eventRegistry = eventRegistry;
			_uiNav = uiNav;
		}

		private void Awake()
		{
			_mainCam = Camera.main;
			_layerMask = LayerMask.GetMask("Ignore Raycast");
		}

		private void OnDisable()
		{
			RemoveTouchUI();
			_onConfirmClickHandle.Get(_eventRegistry)?.Unsubscribe(OnGamepadConfirmClick);
		}

		private void OnEnable()
		{
			_onConfirmClickHandle.Get(_eventRegistry).Subscribe(OnGamepadConfirmClick);
		}

		private void AddTouchUI()
		{
			_uiTouchHelper.AddTouchId(UITouchID);
		}

		private void RemoveTouchUI()
		{
			_uiTouchHelper.RemoveTouchId(UITouchID);
		}

		private void OnGamepadConfirmClick(bool isStarted)
		{
			if (!base.gameObject || !base.isActiveAndEnabled)
			{
				_onConfirmClickHandle.Get(_eventRegistry).Unsubscribe(OnGamepadConfirmClick);
			}
			else
			{
				if (_uiNav == null || _uiNav.StoredValue == UiNavigationEnum.Keys)
				{
					return;
				}
				if (!isStarted)
				{
					RemoveTouchUI();
					return;
				}
				Vector2 vector = ((Mouse.current != null) ? Mouse.current.position.ReadValue() : ((Vector2)Input.mousePosition));
				if (Physics.Raycast(_mainCam.ScreenPointToRay(vector), out var hitInfo, 99f, _layerMask) && hitInfo.collider.TryGetComponent<ButtonPhysics>(out var component))
				{
					component.OnClick();
					AddTouchUI();
				}
			}
		}
	}
}
