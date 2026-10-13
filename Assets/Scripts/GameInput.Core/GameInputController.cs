using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class GameInputController : IInputActionCollection2, IInputActionCollection, IEnumerable<InputAction>, IEnumerable, IDisposable
{
	public struct UIActions
	{
		private GameInputController m_Wrapper;

		public InputAction Navigate => m_Wrapper.m_UI_Navigate;

		public InputAction goBack => m_Wrapper.m_UI_goBack;

		public bool enabled => Get().enabled;

		public UIActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_UI;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(UIActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IUIActions instance)
		{
			if (instance != null && !m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
				Navigate.started += instance.OnNavigate;
				Navigate.performed += instance.OnNavigate;
				Navigate.canceled += instance.OnNavigate;
				goBack.started += instance.OnGoBack;
				goBack.performed += instance.OnGoBack;
				goBack.canceled += instance.OnGoBack;
			}
		}

		private void UnregisterCallbacks(IUIActions instance)
		{
			Navigate.started -= instance.OnNavigate;
			Navigate.performed -= instance.OnNavigate;
			Navigate.canceled -= instance.OnNavigate;
			goBack.started -= instance.OnGoBack;
			goBack.performed -= instance.OnGoBack;
			goBack.canceled -= instance.OnGoBack;
		}

		public void RemoveCallbacks(IUIActions instance)
		{
			if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IUIActions instance)
		{
			foreach (IUIActions uIActionsCallbackInterface in m_Wrapper.m_UIActionsCallbackInterfaces)
			{
				UnregisterCallbacks(uIActionsCallbackInterface);
			}
			m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct UIAcceptCancelActions
	{
		private GameInputController m_Wrapper;

		public InputAction Submit => m_Wrapper.m_UIAcceptCancel_Submit;

		public InputAction Cancel => m_Wrapper.m_UIAcceptCancel_Cancel;

		public bool enabled => Get().enabled;

		public UIAcceptCancelActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_UIAcceptCancel;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(UIAcceptCancelActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IUIAcceptCancelActions instance)
		{
			if (instance != null && !m_Wrapper.m_UIAcceptCancelActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_UIAcceptCancelActionsCallbackInterfaces.Add(instance);
				Submit.started += instance.OnSubmit;
				Submit.performed += instance.OnSubmit;
				Submit.canceled += instance.OnSubmit;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
			}
		}

		private void UnregisterCallbacks(IUIAcceptCancelActions instance)
		{
			Submit.started -= instance.OnSubmit;
			Submit.performed -= instance.OnSubmit;
			Submit.canceled -= instance.OnSubmit;
			Cancel.started -= instance.OnCancel;
			Cancel.performed -= instance.OnCancel;
			Cancel.canceled -= instance.OnCancel;
		}

		public void RemoveCallbacks(IUIAcceptCancelActions instance)
		{
			if (m_Wrapper.m_UIAcceptCancelActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IUIAcceptCancelActions instance)
		{
			foreach (IUIAcceptCancelActions uIAcceptCancelActionsCallbackInterface in m_Wrapper.m_UIAcceptCancelActionsCallbackInterfaces)
			{
				UnregisterCallbacks(uIAcceptCancelActionsCallbackInterface);
			}
			m_Wrapper.m_UIAcceptCancelActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct AcceptCancelActions
	{
		private GameInputController m_Wrapper;

		public InputAction Submit => m_Wrapper.m_AcceptCancel_Submit;

		public InputAction Cancel => m_Wrapper.m_AcceptCancel_Cancel;

		public bool enabled => Get().enabled;

		public AcceptCancelActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_AcceptCancel;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(AcceptCancelActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IAcceptCancelActions instance)
		{
			if (instance != null && !m_Wrapper.m_AcceptCancelActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_AcceptCancelActionsCallbackInterfaces.Add(instance);
				Submit.started += instance.OnSubmit;
				Submit.performed += instance.OnSubmit;
				Submit.canceled += instance.OnSubmit;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
			}
		}

		private void UnregisterCallbacks(IAcceptCancelActions instance)
		{
			Submit.started -= instance.OnSubmit;
			Submit.performed -= instance.OnSubmit;
			Submit.canceled -= instance.OnSubmit;
			Cancel.started -= instance.OnCancel;
			Cancel.performed -= instance.OnCancel;
			Cancel.canceled -= instance.OnCancel;
		}

		public void RemoveCallbacks(IAcceptCancelActions instance)
		{
			if (m_Wrapper.m_AcceptCancelActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IAcceptCancelActions instance)
		{
			foreach (IAcceptCancelActions acceptCancelActionsCallbackInterface in m_Wrapper.m_AcceptCancelActionsCallbackInterfaces)
			{
				UnregisterCallbacks(acceptCancelActionsCallbackInterface);
			}
			m_Wrapper.m_AcceptCancelActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct UiPointerActions
	{
		private GameInputController m_Wrapper;

		public InputAction Point => m_Wrapper.m_UiPointer_Point;

		public InputAction CursorMove => m_Wrapper.m_UiPointer_CursorMove;

		public InputAction Click => m_Wrapper.m_UiPointer_Click;

		public InputAction RightClick => m_Wrapper.m_UiPointer_RightClick;

		public InputAction MiddleClick => m_Wrapper.m_UiPointer_MiddleClick;

		public InputAction ScrollWheel => m_Wrapper.m_UiPointer_ScrollWheel;

		public InputAction TrackedDevicePosition => m_Wrapper.m_UiPointer_TrackedDevicePosition;

		public InputAction TrackedDeviceOrientation => m_Wrapper.m_UiPointer_TrackedDeviceOrientation;

		public InputAction PointerDelta => m_Wrapper.m_UiPointer_PointerDelta;

		public bool enabled => Get().enabled;

		public UiPointerActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_UiPointer;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(UiPointerActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IUiPointerActions instance)
		{
			if (instance != null && !m_Wrapper.m_UiPointerActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_UiPointerActionsCallbackInterfaces.Add(instance);
				Point.started += instance.OnPoint;
				Point.performed += instance.OnPoint;
				Point.canceled += instance.OnPoint;
				CursorMove.started += instance.OnCursorMove;
				CursorMove.performed += instance.OnCursorMove;
				CursorMove.canceled += instance.OnCursorMove;
				Click.started += instance.OnClick;
				Click.performed += instance.OnClick;
				Click.canceled += instance.OnClick;
				RightClick.started += instance.OnRightClick;
				RightClick.performed += instance.OnRightClick;
				RightClick.canceled += instance.OnRightClick;
				MiddleClick.started += instance.OnMiddleClick;
				MiddleClick.performed += instance.OnMiddleClick;
				MiddleClick.canceled += instance.OnMiddleClick;
				ScrollWheel.started += instance.OnScrollWheel;
				ScrollWheel.performed += instance.OnScrollWheel;
				ScrollWheel.canceled += instance.OnScrollWheel;
				TrackedDevicePosition.started += instance.OnTrackedDevicePosition;
				TrackedDevicePosition.performed += instance.OnTrackedDevicePosition;
				TrackedDevicePosition.canceled += instance.OnTrackedDevicePosition;
				TrackedDeviceOrientation.started += instance.OnTrackedDeviceOrientation;
				TrackedDeviceOrientation.performed += instance.OnTrackedDeviceOrientation;
				TrackedDeviceOrientation.canceled += instance.OnTrackedDeviceOrientation;
				PointerDelta.started += instance.OnPointerDelta;
				PointerDelta.performed += instance.OnPointerDelta;
				PointerDelta.canceled += instance.OnPointerDelta;
			}
		}

		private void UnregisterCallbacks(IUiPointerActions instance)
		{
			Point.started -= instance.OnPoint;
			Point.performed -= instance.OnPoint;
			Point.canceled -= instance.OnPoint;
			CursorMove.started -= instance.OnCursorMove;
			CursorMove.performed -= instance.OnCursorMove;
			CursorMove.canceled -= instance.OnCursorMove;
			Click.started -= instance.OnClick;
			Click.performed -= instance.OnClick;
			Click.canceled -= instance.OnClick;
			RightClick.started -= instance.OnRightClick;
			RightClick.performed -= instance.OnRightClick;
			RightClick.canceled -= instance.OnRightClick;
			MiddleClick.started -= instance.OnMiddleClick;
			MiddleClick.performed -= instance.OnMiddleClick;
			MiddleClick.canceled -= instance.OnMiddleClick;
			ScrollWheel.started -= instance.OnScrollWheel;
			ScrollWheel.performed -= instance.OnScrollWheel;
			ScrollWheel.canceled -= instance.OnScrollWheel;
			TrackedDevicePosition.started -= instance.OnTrackedDevicePosition;
			TrackedDevicePosition.performed -= instance.OnTrackedDevicePosition;
			TrackedDevicePosition.canceled -= instance.OnTrackedDevicePosition;
			TrackedDeviceOrientation.started -= instance.OnTrackedDeviceOrientation;
			TrackedDeviceOrientation.performed -= instance.OnTrackedDeviceOrientation;
			TrackedDeviceOrientation.canceled -= instance.OnTrackedDeviceOrientation;
			PointerDelta.started -= instance.OnPointerDelta;
			PointerDelta.performed -= instance.OnPointerDelta;
			PointerDelta.canceled -= instance.OnPointerDelta;
		}

		public void RemoveCallbacks(IUiPointerActions instance)
		{
			if (m_Wrapper.m_UiPointerActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IUiPointerActions instance)
		{
			foreach (IUiPointerActions uiPointerActionsCallbackInterface in m_Wrapper.m_UiPointerActionsCallbackInterfaces)
			{
				UnregisterCallbacks(uiPointerActionsCallbackInterface);
			}
			m_Wrapper.m_UiPointerActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct UiNextPreviousActions
	{
		private GameInputController m_Wrapper;

		public InputAction Next => m_Wrapper.m_UiNextPrevious_Next;

		public InputAction Previous => m_Wrapper.m_UiNextPrevious_Previous;

		public bool enabled => Get().enabled;

		public UiNextPreviousActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_UiNextPrevious;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(UiNextPreviousActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IUiNextPreviousActions instance)
		{
			if (instance != null && !m_Wrapper.m_UiNextPreviousActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_UiNextPreviousActionsCallbackInterfaces.Add(instance);
				Next.started += instance.OnNext;
				Next.performed += instance.OnNext;
				Next.canceled += instance.OnNext;
				Previous.started += instance.OnPrevious;
				Previous.performed += instance.OnPrevious;
				Previous.canceled += instance.OnPrevious;
			}
		}

		private void UnregisterCallbacks(IUiNextPreviousActions instance)
		{
			Next.started -= instance.OnNext;
			Next.performed -= instance.OnNext;
			Next.canceled -= instance.OnNext;
			Previous.started -= instance.OnPrevious;
			Previous.performed -= instance.OnPrevious;
			Previous.canceled -= instance.OnPrevious;
		}

		public void RemoveCallbacks(IUiNextPreviousActions instance)
		{
			if (m_Wrapper.m_UiNextPreviousActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IUiNextPreviousActions instance)
		{
			foreach (IUiNextPreviousActions uiNextPreviousActionsCallbackInterface in m_Wrapper.m_UiNextPreviousActionsCallbackInterfaces)
			{
				UnregisterCallbacks(uiNextPreviousActionsCallbackInterface);
			}
			m_Wrapper.m_UiNextPreviousActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct UiThrottleActions
	{
		private GameInputController m_Wrapper;

		public InputAction Throttle => m_Wrapper.m_UiThrottle_Throttle;

		public bool enabled => Get().enabled;

		public UiThrottleActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_UiThrottle;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(UiThrottleActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IUiThrottleActions instance)
		{
			if (instance != null && !m_Wrapper.m_UiThrottleActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_UiThrottleActionsCallbackInterfaces.Add(instance);
				Throttle.started += instance.OnThrottle;
				Throttle.performed += instance.OnThrottle;
				Throttle.canceled += instance.OnThrottle;
			}
		}

		private void UnregisterCallbacks(IUiThrottleActions instance)
		{
			Throttle.started -= instance.OnThrottle;
			Throttle.performed -= instance.OnThrottle;
			Throttle.canceled -= instance.OnThrottle;
		}

		public void RemoveCallbacks(IUiThrottleActions instance)
		{
			if (m_Wrapper.m_UiThrottleActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IUiThrottleActions instance)
		{
			foreach (IUiThrottleActions uiThrottleActionsCallbackInterface in m_Wrapper.m_UiThrottleActionsCallbackInterfaces)
			{
				UnregisterCallbacks(uiThrottleActionsCallbackInterface);
			}
			m_Wrapper.m_UiThrottleActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct MenuActions
	{
		private GameInputController m_Wrapper;

		public InputAction Menu => m_Wrapper.m_Menu_Menu;

		public bool enabled => Get().enabled;

		public MenuActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Menu;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(MenuActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IMenuActions instance)
		{
			if (instance != null && !m_Wrapper.m_MenuActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_MenuActionsCallbackInterfaces.Add(instance);
				Menu.started += instance.OnMenu;
				Menu.performed += instance.OnMenu;
				Menu.canceled += instance.OnMenu;
			}
		}

		private void UnregisterCallbacks(IMenuActions instance)
		{
			Menu.started -= instance.OnMenu;
			Menu.performed -= instance.OnMenu;
			Menu.canceled -= instance.OnMenu;
		}

		public void RemoveCallbacks(IMenuActions instance)
		{
			if (m_Wrapper.m_MenuActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IMenuActions instance)
		{
			foreach (IMenuActions menuActionsCallbackInterface in m_Wrapper.m_MenuActionsCallbackInterfaces)
			{
				UnregisterCallbacks(menuActionsCallbackInterface);
			}
			m_Wrapper.m_MenuActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ShowMapActions
	{
		private GameInputController m_Wrapper;

		public InputAction Map => m_Wrapper.m_ShowMap_Map;

		public bool enabled => Get().enabled;

		public ShowMapActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_ShowMap;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ShowMapActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IShowMapActions instance)
		{
			if (instance != null && !m_Wrapper.m_ShowMapActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ShowMapActionsCallbackInterfaces.Add(instance);
				Map.started += instance.OnMap;
				Map.performed += instance.OnMap;
				Map.canceled += instance.OnMap;
			}
		}

		private void UnregisterCallbacks(IShowMapActions instance)
		{
			Map.started -= instance.OnMap;
			Map.performed -= instance.OnMap;
			Map.canceled -= instance.OnMap;
		}

		public void RemoveCallbacks(IShowMapActions instance)
		{
			if (m_Wrapper.m_ShowMapActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IShowMapActions instance)
		{
			foreach (IShowMapActions showMapActionsCallbackInterface in m_Wrapper.m_ShowMapActionsCallbackInterfaces)
			{
				UnregisterCallbacks(showMapActionsCallbackInterface);
			}
			m_Wrapper.m_ShowMapActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct MapUiActions
	{
		private GameInputController m_Wrapper;

		public InputAction HideShow => m_Wrapper.m_MapUi_HideShow;

		public InputAction HideShowAll => m_Wrapper.m_MapUi_HideShowAll;

		public InputAction Expand => m_Wrapper.m_MapUi_Expand;

		public InputAction CloseMap => m_Wrapper.m_MapUi_CloseMap;

		public InputAction Mark => m_Wrapper.m_MapUi_Mark;

		public InputAction ShowMarkDetails => m_Wrapper.m_MapUi_ShowMarkDetails;

		public bool enabled => Get().enabled;

		public MapUiActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_MapUi;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(MapUiActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IMapUiActions instance)
		{
			if (instance != null && !m_Wrapper.m_MapUiActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_MapUiActionsCallbackInterfaces.Add(instance);
				HideShow.started += instance.OnHideShow;
				HideShow.performed += instance.OnHideShow;
				HideShow.canceled += instance.OnHideShow;
				HideShowAll.started += instance.OnHideShowAll;
				HideShowAll.performed += instance.OnHideShowAll;
				HideShowAll.canceled += instance.OnHideShowAll;
				Expand.started += instance.OnExpand;
				Expand.performed += instance.OnExpand;
				Expand.canceled += instance.OnExpand;
				CloseMap.started += instance.OnCloseMap;
				CloseMap.performed += instance.OnCloseMap;
				CloseMap.canceled += instance.OnCloseMap;
				Mark.started += instance.OnMark;
				Mark.performed += instance.OnMark;
				Mark.canceled += instance.OnMark;
				ShowMarkDetails.started += instance.OnShowMarkDetails;
				ShowMarkDetails.performed += instance.OnShowMarkDetails;
				ShowMarkDetails.canceled += instance.OnShowMarkDetails;
			}
		}

		private void UnregisterCallbacks(IMapUiActions instance)
		{
			HideShow.started -= instance.OnHideShow;
			HideShow.performed -= instance.OnHideShow;
			HideShow.canceled -= instance.OnHideShow;
			HideShowAll.started -= instance.OnHideShowAll;
			HideShowAll.performed -= instance.OnHideShowAll;
			HideShowAll.canceled -= instance.OnHideShowAll;
			Expand.started -= instance.OnExpand;
			Expand.performed -= instance.OnExpand;
			Expand.canceled -= instance.OnExpand;
			CloseMap.started -= instance.OnCloseMap;
			CloseMap.performed -= instance.OnCloseMap;
			CloseMap.canceled -= instance.OnCloseMap;
			Mark.started -= instance.OnMark;
			Mark.performed -= instance.OnMark;
			Mark.canceled -= instance.OnMark;
			ShowMarkDetails.started -= instance.OnShowMarkDetails;
			ShowMarkDetails.performed -= instance.OnShowMarkDetails;
			ShowMarkDetails.canceled -= instance.OnShowMarkDetails;
		}

		public void RemoveCallbacks(IMapUiActions instance)
		{
			if (m_Wrapper.m_MapUiActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IMapUiActions instance)
		{
			foreach (IMapUiActions mapUiActionsCallbackInterface in m_Wrapper.m_MapUiActionsCallbackInterfaces)
			{
				UnregisterCallbacks(mapUiActionsCallbackInterface);
			}
			m_Wrapper.m_MapUiActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct MapMoveActions
	{
		private GameInputController m_Wrapper;

		public InputAction Move => m_Wrapper.m_MapMove_Move;

		public InputAction MouseDragMove => m_Wrapper.m_MapMove_MouseDragMove;

		public InputAction MouseDelta => m_Wrapper.m_MapMove_MouseDelta;

		public bool enabled => Get().enabled;

		public MapMoveActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_MapMove;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(MapMoveActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IMapMoveActions instance)
		{
			if (instance != null && !m_Wrapper.m_MapMoveActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_MapMoveActionsCallbackInterfaces.Add(instance);
				Move.started += instance.OnMove;
				Move.performed += instance.OnMove;
				Move.canceled += instance.OnMove;
				MouseDragMove.started += instance.OnMouseDragMove;
				MouseDragMove.performed += instance.OnMouseDragMove;
				MouseDragMove.canceled += instance.OnMouseDragMove;
				MouseDelta.started += instance.OnMouseDelta;
				MouseDelta.performed += instance.OnMouseDelta;
				MouseDelta.canceled += instance.OnMouseDelta;
			}
		}

		private void UnregisterCallbacks(IMapMoveActions instance)
		{
			Move.started -= instance.OnMove;
			Move.performed -= instance.OnMove;
			Move.canceled -= instance.OnMove;
			MouseDragMove.started -= instance.OnMouseDragMove;
			MouseDragMove.performed -= instance.OnMouseDragMove;
			MouseDragMove.canceled -= instance.OnMouseDragMove;
			MouseDelta.started -= instance.OnMouseDelta;
			MouseDelta.performed -= instance.OnMouseDelta;
			MouseDelta.canceled -= instance.OnMouseDelta;
		}

		public void RemoveCallbacks(IMapMoveActions instance)
		{
			if (m_Wrapper.m_MapMoveActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IMapMoveActions instance)
		{
			foreach (IMapMoveActions mapMoveActionsCallbackInterface in m_Wrapper.m_MapMoveActionsCallbackInterfaces)
			{
				UnregisterCallbacks(mapMoveActionsCallbackInterface);
			}
			m_Wrapper.m_MapMoveActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct DroneActions
	{
		private GameInputController m_Wrapper;

		public InputAction SpeedToggle => m_Wrapper.m_Drone_SpeedToggle;

		public InputAction Tilt => m_Wrapper.m_Drone_Tilt;

		public InputAction HideUi => m_Wrapper.m_Drone_HideUi;

		public InputAction Center => m_Wrapper.m_Drone_Center;

		public InputAction Back => m_Wrapper.m_Drone_Back;

		public InputAction OpenChat => m_Wrapper.m_Drone_OpenChat;

		public bool enabled => Get().enabled;

		public DroneActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Drone;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(DroneActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IDroneActions instance)
		{
			if (instance != null && !m_Wrapper.m_DroneActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_DroneActionsCallbackInterfaces.Add(instance);
				SpeedToggle.started += instance.OnSpeedToggle;
				SpeedToggle.performed += instance.OnSpeedToggle;
				SpeedToggle.canceled += instance.OnSpeedToggle;
				Tilt.started += instance.OnTilt;
				Tilt.performed += instance.OnTilt;
				Tilt.canceled += instance.OnTilt;
				HideUi.started += instance.OnHideUi;
				HideUi.performed += instance.OnHideUi;
				HideUi.canceled += instance.OnHideUi;
				Center.started += instance.OnCenter;
				Center.performed += instance.OnCenter;
				Center.canceled += instance.OnCenter;
				Back.started += instance.OnBack;
				Back.performed += instance.OnBack;
				Back.canceled += instance.OnBack;
				OpenChat.started += instance.OnOpenChat;
				OpenChat.performed += instance.OnOpenChat;
				OpenChat.canceled += instance.OnOpenChat;
			}
		}

		private void UnregisterCallbacks(IDroneActions instance)
		{
			SpeedToggle.started -= instance.OnSpeedToggle;
			SpeedToggle.performed -= instance.OnSpeedToggle;
			SpeedToggle.canceled -= instance.OnSpeedToggle;
			Tilt.started -= instance.OnTilt;
			Tilt.performed -= instance.OnTilt;
			Tilt.canceled -= instance.OnTilt;
			HideUi.started -= instance.OnHideUi;
			HideUi.performed -= instance.OnHideUi;
			HideUi.canceled -= instance.OnHideUi;
			Center.started -= instance.OnCenter;
			Center.performed -= instance.OnCenter;
			Center.canceled -= instance.OnCenter;
			Back.started -= instance.OnBack;
			Back.performed -= instance.OnBack;
			Back.canceled -= instance.OnBack;
			OpenChat.started -= instance.OnOpenChat;
			OpenChat.performed -= instance.OnOpenChat;
			OpenChat.canceled -= instance.OnOpenChat;
		}

		public void RemoveCallbacks(IDroneActions instance)
		{
			if (m_Wrapper.m_DroneActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IDroneActions instance)
		{
			foreach (IDroneActions droneActionsCallbackInterface in m_Wrapper.m_DroneActionsCallbackInterfaces)
			{
				UnregisterCallbacks(droneActionsCallbackInterface);
			}
			m_Wrapper.m_DroneActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct DroneCameraActions
	{
		private GameInputController m_Wrapper;

		public InputAction Reset => m_Wrapper.m_DroneCamera_Reset;

		public InputAction Snapshot => m_Wrapper.m_DroneCamera_Snapshot;

		public bool enabled => Get().enabled;

		public DroneCameraActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_DroneCamera;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(DroneCameraActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IDroneCameraActions instance)
		{
			if (instance != null && !m_Wrapper.m_DroneCameraActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_DroneCameraActionsCallbackInterfaces.Add(instance);
				Reset.started += instance.OnReset;
				Reset.performed += instance.OnReset;
				Reset.canceled += instance.OnReset;
				Snapshot.started += instance.OnSnapshot;
				Snapshot.performed += instance.OnSnapshot;
				Snapshot.canceled += instance.OnSnapshot;
			}
		}

		private void UnregisterCallbacks(IDroneCameraActions instance)
		{
			Reset.started -= instance.OnReset;
			Reset.performed -= instance.OnReset;
			Reset.canceled -= instance.OnReset;
			Snapshot.started -= instance.OnSnapshot;
			Snapshot.performed -= instance.OnSnapshot;
			Snapshot.canceled -= instance.OnSnapshot;
		}

		public void RemoveCallbacks(IDroneCameraActions instance)
		{
			if (m_Wrapper.m_DroneCameraActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IDroneCameraActions instance)
		{
			foreach (IDroneCameraActions droneCameraActionsCallbackInterface in m_Wrapper.m_DroneCameraActionsCallbackInterfaces)
			{
				UnregisterCallbacks(droneCameraActionsCallbackInterface);
			}
			m_Wrapper.m_DroneCameraActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PhotoActions
	{
		private GameInputController m_Wrapper;

		public InputAction Help => m_Wrapper.m_Photo_Help;

		public InputAction ToggleFocus => m_Wrapper.m_Photo_ToggleFocus;

		public InputAction Toggles => m_Wrapper.m_Photo_Toggles;

		public InputAction NavigateSettings => m_Wrapper.m_Photo_NavigateSettings;

		public InputAction Color => m_Wrapper.m_Photo_Color;

		public InputAction Optica => m_Wrapper.m_Photo_Optica;

		public InputAction Camera => m_Wrapper.m_Photo_Camera;

		public InputAction ResetFilter => m_Wrapper.m_Photo_ResetFilter;

		public bool enabled => Get().enabled;

		public PhotoActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Photo;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PhotoActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPhotoActions instance)
		{
			if (instance != null && !m_Wrapper.m_PhotoActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PhotoActionsCallbackInterfaces.Add(instance);
				Help.started += instance.OnHelp;
				Help.performed += instance.OnHelp;
				Help.canceled += instance.OnHelp;
				ToggleFocus.started += instance.OnToggleFocus;
				ToggleFocus.performed += instance.OnToggleFocus;
				ToggleFocus.canceled += instance.OnToggleFocus;
				Toggles.started += instance.OnToggles;
				Toggles.performed += instance.OnToggles;
				Toggles.canceled += instance.OnToggles;
				NavigateSettings.started += instance.OnNavigateSettings;
				NavigateSettings.performed += instance.OnNavigateSettings;
				NavigateSettings.canceled += instance.OnNavigateSettings;
				Color.started += instance.OnColor;
				Color.performed += instance.OnColor;
				Color.canceled += instance.OnColor;
				Optica.started += instance.OnOptica;
				Optica.performed += instance.OnOptica;
				Optica.canceled += instance.OnOptica;
				Camera.started += instance.OnCamera;
				Camera.performed += instance.OnCamera;
				Camera.canceled += instance.OnCamera;
				ResetFilter.started += instance.OnResetFilter;
				ResetFilter.performed += instance.OnResetFilter;
				ResetFilter.canceled += instance.OnResetFilter;
			}
		}

		private void UnregisterCallbacks(IPhotoActions instance)
		{
			Help.started -= instance.OnHelp;
			Help.performed -= instance.OnHelp;
			Help.canceled -= instance.OnHelp;
			ToggleFocus.started -= instance.OnToggleFocus;
			ToggleFocus.performed -= instance.OnToggleFocus;
			ToggleFocus.canceled -= instance.OnToggleFocus;
			Toggles.started -= instance.OnToggles;
			Toggles.performed -= instance.OnToggles;
			Toggles.canceled -= instance.OnToggles;
			NavigateSettings.started -= instance.OnNavigateSettings;
			NavigateSettings.performed -= instance.OnNavigateSettings;
			NavigateSettings.canceled -= instance.OnNavigateSettings;
			Color.started -= instance.OnColor;
			Color.performed -= instance.OnColor;
			Color.canceled -= instance.OnColor;
			Optica.started -= instance.OnOptica;
			Optica.performed -= instance.OnOptica;
			Optica.canceled -= instance.OnOptica;
			Camera.started -= instance.OnCamera;
			Camera.performed -= instance.OnCamera;
			Camera.canceled -= instance.OnCamera;
			ResetFilter.started -= instance.OnResetFilter;
			ResetFilter.performed -= instance.OnResetFilter;
			ResetFilter.canceled -= instance.OnResetFilter;
		}

		public void RemoveCallbacks(IPhotoActions instance)
		{
			if (m_Wrapper.m_PhotoActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPhotoActions instance)
		{
			foreach (IPhotoActions photoActionsCallbackInterface in m_Wrapper.m_PhotoActionsCallbackInterfaces)
			{
				UnregisterCallbacks(photoActionsCallbackInterface);
			}
			m_Wrapper.m_PhotoActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintBaseActions
	{
		private GameInputController m_Wrapper;

		public InputAction BlockCam => m_Wrapper.m_PaintBase_BlockCam;

		public InputAction ToggleUi => m_Wrapper.m_PaintBase_ToggleUi;

		public InputAction DeleteAll => m_Wrapper.m_PaintBase_DeleteAll;

		public InputAction DeleteSelected => m_Wrapper.m_PaintBase_DeleteSelected;

		public bool enabled => Get().enabled;

		public PaintBaseActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintBase;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintBaseActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintBaseActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintBaseActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintBaseActionsCallbackInterfaces.Add(instance);
				BlockCam.started += instance.OnBlockCam;
				BlockCam.performed += instance.OnBlockCam;
				BlockCam.canceled += instance.OnBlockCam;
				ToggleUi.started += instance.OnToggleUi;
				ToggleUi.performed += instance.OnToggleUi;
				ToggleUi.canceled += instance.OnToggleUi;
				DeleteAll.started += instance.OnDeleteAll;
				DeleteAll.performed += instance.OnDeleteAll;
				DeleteAll.canceled += instance.OnDeleteAll;
				DeleteSelected.started += instance.OnDeleteSelected;
				DeleteSelected.performed += instance.OnDeleteSelected;
				DeleteSelected.canceled += instance.OnDeleteSelected;
			}
		}

		private void UnregisterCallbacks(IPaintBaseActions instance)
		{
			BlockCam.started -= instance.OnBlockCam;
			BlockCam.performed -= instance.OnBlockCam;
			BlockCam.canceled -= instance.OnBlockCam;
			ToggleUi.started -= instance.OnToggleUi;
			ToggleUi.performed -= instance.OnToggleUi;
			ToggleUi.canceled -= instance.OnToggleUi;
			DeleteAll.started -= instance.OnDeleteAll;
			DeleteAll.performed -= instance.OnDeleteAll;
			DeleteAll.canceled -= instance.OnDeleteAll;
			DeleteSelected.started -= instance.OnDeleteSelected;
			DeleteSelected.performed -= instance.OnDeleteSelected;
			DeleteSelected.canceled -= instance.OnDeleteSelected;
		}

		public void RemoveCallbacks(IPaintBaseActions instance)
		{
			if (m_Wrapper.m_PaintBaseActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintBaseActions instance)
		{
			foreach (IPaintBaseActions paintBaseActionsCallbackInterface in m_Wrapper.m_PaintBaseActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintBaseActionsCallbackInterface);
			}
			m_Wrapper.m_PaintBaseActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintFavoritesToggleActions
	{
		private GameInputController m_Wrapper;

		public InputAction FavoritesToggle => m_Wrapper.m_PaintFavoritesToggle_FavoritesToggle;

		public bool enabled => Get().enabled;

		public PaintFavoritesToggleActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintFavoritesToggle;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintFavoritesToggleActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintFavoritesToggleActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintFavoritesToggleActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintFavoritesToggleActionsCallbackInterfaces.Add(instance);
				FavoritesToggle.started += instance.OnFavoritesToggle;
				FavoritesToggle.performed += instance.OnFavoritesToggle;
				FavoritesToggle.canceled += instance.OnFavoritesToggle;
			}
		}

		private void UnregisterCallbacks(IPaintFavoritesToggleActions instance)
		{
			FavoritesToggle.started -= instance.OnFavoritesToggle;
			FavoritesToggle.performed -= instance.OnFavoritesToggle;
			FavoritesToggle.canceled -= instance.OnFavoritesToggle;
		}

		public void RemoveCallbacks(IPaintFavoritesToggleActions instance)
		{
			if (m_Wrapper.m_PaintFavoritesToggleActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintFavoritesToggleActions instance)
		{
			foreach (IPaintFavoritesToggleActions paintFavoritesToggleActionsCallbackInterface in m_Wrapper.m_PaintFavoritesToggleActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintFavoritesToggleActionsCallbackInterface);
			}
			m_Wrapper.m_PaintFavoritesToggleActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintApplyCancelActions
	{
		private GameInputController m_Wrapper;

		public InputAction CancelChange => m_Wrapper.m_PaintApplyCancel_CancelChange;

		public bool enabled => Get().enabled;

		public PaintApplyCancelActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintApplyCancel;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintApplyCancelActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintApplyCancelActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintApplyCancelActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintApplyCancelActionsCallbackInterfaces.Add(instance);
				CancelChange.started += instance.OnCancelChange;
				CancelChange.performed += instance.OnCancelChange;
				CancelChange.canceled += instance.OnCancelChange;
			}
		}

		private void UnregisterCallbacks(IPaintApplyCancelActions instance)
		{
			CancelChange.started -= instance.OnCancelChange;
			CancelChange.performed -= instance.OnCancelChange;
			CancelChange.canceled -= instance.OnCancelChange;
		}

		public void RemoveCallbacks(IPaintApplyCancelActions instance)
		{
			if (m_Wrapper.m_PaintApplyCancelActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintApplyCancelActions instance)
		{
			foreach (IPaintApplyCancelActions paintApplyCancelActionsCallbackInterface in m_Wrapper.m_PaintApplyCancelActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintApplyCancelActionsCallbackInterface);
			}
			m_Wrapper.m_PaintApplyCancelActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintMultiSelectActions
	{
		private GameInputController m_Wrapper;

		public InputAction MultiSelect => m_Wrapper.m_PaintMultiSelect_MultiSelect;

		public bool enabled => Get().enabled;

		public PaintMultiSelectActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintMultiSelect;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintMultiSelectActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintMultiSelectActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintMultiSelectActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintMultiSelectActionsCallbackInterfaces.Add(instance);
				MultiSelect.started += instance.OnMultiSelect;
				MultiSelect.performed += instance.OnMultiSelect;
				MultiSelect.canceled += instance.OnMultiSelect;
			}
		}

		private void UnregisterCallbacks(IPaintMultiSelectActions instance)
		{
			MultiSelect.started -= instance.OnMultiSelect;
			MultiSelect.performed -= instance.OnMultiSelect;
			MultiSelect.canceled -= instance.OnMultiSelect;
		}

		public void RemoveCallbacks(IPaintMultiSelectActions instance)
		{
			if (m_Wrapper.m_PaintMultiSelectActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintMultiSelectActions instance)
		{
			foreach (IPaintMultiSelectActions paintMultiSelectActionsCallbackInterface in m_Wrapper.m_PaintMultiSelectActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintMultiSelectActionsCallbackInterface);
			}
			m_Wrapper.m_PaintMultiSelectActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintSelectNewActions
	{
		private GameInputController m_Wrapper;

		public InputAction SelectNew => m_Wrapper.m_PaintSelectNew_SelectNew;

		public bool enabled => Get().enabled;

		public PaintSelectNewActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintSelectNew;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintSelectNewActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintSelectNewActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintSelectNewActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintSelectNewActionsCallbackInterfaces.Add(instance);
				SelectNew.started += instance.OnSelectNew;
				SelectNew.performed += instance.OnSelectNew;
				SelectNew.canceled += instance.OnSelectNew;
			}
		}

		private void UnregisterCallbacks(IPaintSelectNewActions instance)
		{
			SelectNew.started -= instance.OnSelectNew;
			SelectNew.performed -= instance.OnSelectNew;
			SelectNew.canceled -= instance.OnSelectNew;
		}

		public void RemoveCallbacks(IPaintSelectNewActions instance)
		{
			if (m_Wrapper.m_PaintSelectNewActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintSelectNewActions instance)
		{
			foreach (IPaintSelectNewActions paintSelectNewActionsCallbackInterface in m_Wrapper.m_PaintSelectNewActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintSelectNewActionsCallbackInterface);
			}
			m_Wrapper.m_PaintSelectNewActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintPlatesActions
	{
		private GameInputController m_Wrapper;

		public InputAction NavigatePlateType => m_Wrapper.m_PaintPlates_NavigatePlateType;

		public InputAction FrontToggle => m_Wrapper.m_PaintPlates_FrontToggle;

		public InputAction BackToggle => m_Wrapper.m_PaintPlates_BackToggle;

		public bool enabled => Get().enabled;

		public PaintPlatesActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintPlates;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintPlatesActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintPlatesActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintPlatesActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintPlatesActionsCallbackInterfaces.Add(instance);
				NavigatePlateType.started += instance.OnNavigatePlateType;
				NavigatePlateType.performed += instance.OnNavigatePlateType;
				NavigatePlateType.canceled += instance.OnNavigatePlateType;
				FrontToggle.started += instance.OnFrontToggle;
				FrontToggle.performed += instance.OnFrontToggle;
				FrontToggle.canceled += instance.OnFrontToggle;
				BackToggle.started += instance.OnBackToggle;
				BackToggle.performed += instance.OnBackToggle;
				BackToggle.canceled += instance.OnBackToggle;
			}
		}

		private void UnregisterCallbacks(IPaintPlatesActions instance)
		{
			NavigatePlateType.started -= instance.OnNavigatePlateType;
			NavigatePlateType.performed -= instance.OnNavigatePlateType;
			NavigatePlateType.canceled -= instance.OnNavigatePlateType;
			FrontToggle.started -= instance.OnFrontToggle;
			FrontToggle.performed -= instance.OnFrontToggle;
			FrontToggle.canceled -= instance.OnFrontToggle;
			BackToggle.started -= instance.OnBackToggle;
			BackToggle.performed -= instance.OnBackToggle;
			BackToggle.canceled -= instance.OnBackToggle;
		}

		public void RemoveCallbacks(IPaintPlatesActions instance)
		{
			if (m_Wrapper.m_PaintPlatesActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintPlatesActions instance)
		{
			foreach (IPaintPlatesActions paintPlatesActionsCallbackInterface in m_Wrapper.m_PaintPlatesActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintPlatesActionsCallbackInterface);
			}
			m_Wrapper.m_PaintPlatesActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintSelectSetActions
	{
		private GameInputController m_Wrapper;

		public InputAction SelectSetVinyl => m_Wrapper.m_PaintSelectSet_SelectSetVinyl;

		public bool enabled => Get().enabled;

		public PaintSelectSetActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintSelectSet;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintSelectSetActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintSelectSetActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintSelectSetActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintSelectSetActionsCallbackInterfaces.Add(instance);
				SelectSetVinyl.started += instance.OnSelectSetVinyl;
				SelectSetVinyl.performed += instance.OnSelectSetVinyl;
				SelectSetVinyl.canceled += instance.OnSelectSetVinyl;
			}
		}

		private void UnregisterCallbacks(IPaintSelectSetActions instance)
		{
			SelectSetVinyl.started -= instance.OnSelectSetVinyl;
			SelectSetVinyl.performed -= instance.OnSelectSetVinyl;
			SelectSetVinyl.canceled -= instance.OnSelectSetVinyl;
		}

		public void RemoveCallbacks(IPaintSelectSetActions instance)
		{
			if (m_Wrapper.m_PaintSelectSetActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintSelectSetActions instance)
		{
			foreach (IPaintSelectSetActions paintSelectSetActionsCallbackInterface in m_Wrapper.m_PaintSelectSetActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintSelectSetActionsCallbackInterface);
			}
			m_Wrapper.m_PaintSelectSetActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintCopyActions
	{
		private GameInputController m_Wrapper;

		public InputAction Copy => m_Wrapper.m_PaintCopy_Copy;

		public bool enabled => Get().enabled;

		public PaintCopyActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintCopy;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintCopyActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintCopyActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintCopyActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintCopyActionsCallbackInterfaces.Add(instance);
				Copy.started += instance.OnCopy;
				Copy.performed += instance.OnCopy;
				Copy.canceled += instance.OnCopy;
			}
		}

		private void UnregisterCallbacks(IPaintCopyActions instance)
		{
			Copy.started -= instance.OnCopy;
			Copy.performed -= instance.OnCopy;
			Copy.canceled -= instance.OnCopy;
		}

		public void RemoveCallbacks(IPaintCopyActions instance)
		{
			if (m_Wrapper.m_PaintCopyActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintCopyActions instance)
		{
			foreach (IPaintCopyActions paintCopyActionsCallbackInterface in m_Wrapper.m_PaintCopyActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintCopyActionsCallbackInterface);
			}
			m_Wrapper.m_PaintCopyActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintPlateTypesActions
	{
		private GameInputController m_Wrapper;

		public InputAction EnterPlateTypes => m_Wrapper.m_PaintPlateTypes_EnterPlateTypes;

		public bool enabled => Get().enabled;

		public PaintPlateTypesActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintPlateTypes;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintPlateTypesActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintPlateTypesActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintPlateTypesActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintPlateTypesActionsCallbackInterfaces.Add(instance);
				EnterPlateTypes.started += instance.OnEnterPlateTypes;
				EnterPlateTypes.performed += instance.OnEnterPlateTypes;
				EnterPlateTypes.canceled += instance.OnEnterPlateTypes;
			}
		}

		private void UnregisterCallbacks(IPaintPlateTypesActions instance)
		{
			EnterPlateTypes.started -= instance.OnEnterPlateTypes;
			EnterPlateTypes.performed -= instance.OnEnterPlateTypes;
			EnterPlateTypes.canceled -= instance.OnEnterPlateTypes;
		}

		public void RemoveCallbacks(IPaintPlateTypesActions instance)
		{
			if (m_Wrapper.m_PaintPlateTypesActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintPlateTypesActions instance)
		{
			foreach (IPaintPlateTypesActions paintPlateTypesActionsCallbackInterface in m_Wrapper.m_PaintPlateTypesActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintPlateTypesActionsCallbackInterface);
			}
			m_Wrapper.m_PaintPlateTypesActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintFavoriteActions
	{
		private GameInputController m_Wrapper;

		public InputAction Favorite => m_Wrapper.m_PaintFavorite_Favorite;

		public bool enabled => Get().enabled;

		public PaintFavoriteActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintFavorite;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintFavoriteActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintFavoriteActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintFavoriteActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintFavoriteActionsCallbackInterfaces.Add(instance);
				Favorite.started += instance.OnFavorite;
				Favorite.performed += instance.OnFavorite;
				Favorite.canceled += instance.OnFavorite;
			}
		}

		private void UnregisterCallbacks(IPaintFavoriteActions instance)
		{
			Favorite.started -= instance.OnFavorite;
			Favorite.performed -= instance.OnFavorite;
			Favorite.canceled -= instance.OnFavorite;
		}

		public void RemoveCallbacks(IPaintFavoriteActions instance)
		{
			if (m_Wrapper.m_PaintFavoriteActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintFavoriteActions instance)
		{
			foreach (IPaintFavoriteActions paintFavoriteActionsCallbackInterface in m_Wrapper.m_PaintFavoriteActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintFavoriteActionsCallbackInterface);
			}
			m_Wrapper.m_PaintFavoriteActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintSelectionMadeActions
	{
		private GameInputController m_Wrapper;

		public InputAction LayerUpDown => m_Wrapper.m_PaintSelectionMade_LayerUpDown;

		public InputAction Move => m_Wrapper.m_PaintSelectionMade_Move;

		public InputAction SetSize => m_Wrapper.m_PaintSelectionMade_SetSize;

		public InputAction SetColor => m_Wrapper.m_PaintSelectionMade_SetColor;

		public bool enabled => Get().enabled;

		public PaintSelectionMadeActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintSelectionMade;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintSelectionMadeActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintSelectionMadeActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintSelectionMadeActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintSelectionMadeActionsCallbackInterfaces.Add(instance);
				LayerUpDown.started += instance.OnLayerUpDown;
				LayerUpDown.performed += instance.OnLayerUpDown;
				LayerUpDown.canceled += instance.OnLayerUpDown;
				Move.started += instance.OnMove;
				Move.performed += instance.OnMove;
				Move.canceled += instance.OnMove;
				SetSize.started += instance.OnSetSize;
				SetSize.performed += instance.OnSetSize;
				SetSize.canceled += instance.OnSetSize;
				SetColor.started += instance.OnSetColor;
				SetColor.performed += instance.OnSetColor;
				SetColor.canceled += instance.OnSetColor;
			}
		}

		private void UnregisterCallbacks(IPaintSelectionMadeActions instance)
		{
			LayerUpDown.started -= instance.OnLayerUpDown;
			LayerUpDown.performed -= instance.OnLayerUpDown;
			LayerUpDown.canceled -= instance.OnLayerUpDown;
			Move.started -= instance.OnMove;
			Move.performed -= instance.OnMove;
			Move.canceled -= instance.OnMove;
			SetSize.started -= instance.OnSetSize;
			SetSize.performed -= instance.OnSetSize;
			SetSize.canceled -= instance.OnSetSize;
			SetColor.started -= instance.OnSetColor;
			SetColor.performed -= instance.OnSetColor;
			SetColor.canceled -= instance.OnSetColor;
		}

		public void RemoveCallbacks(IPaintSelectionMadeActions instance)
		{
			if (m_Wrapper.m_PaintSelectionMadeActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintSelectionMadeActions instance)
		{
			foreach (IPaintSelectionMadeActions paintSelectionMadeActionsCallbackInterface in m_Wrapper.m_PaintSelectionMadeActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintSelectionMadeActionsCallbackInterface);
			}
			m_Wrapper.m_PaintSelectionMadeActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintColorActions
	{
		private GameInputController m_Wrapper;

		public InputAction ColorToggle => m_Wrapper.m_PaintColor_ColorToggle;

		public InputAction TintToggle => m_Wrapper.m_PaintColor_TintToggle;

		public InputAction AlphaValue => m_Wrapper.m_PaintColor_AlphaValue;

		public InputAction ColorMove => m_Wrapper.m_PaintColor_ColorMove;

		public InputAction ColorCode => m_Wrapper.m_PaintColor_ColorCode;

		public bool enabled => Get().enabled;

		public PaintColorActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintColor;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintColorActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintColorActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintColorActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintColorActionsCallbackInterfaces.Add(instance);
				ColorToggle.started += instance.OnColorToggle;
				ColorToggle.performed += instance.OnColorToggle;
				ColorToggle.canceled += instance.OnColorToggle;
				TintToggle.started += instance.OnTintToggle;
				TintToggle.performed += instance.OnTintToggle;
				TintToggle.canceled += instance.OnTintToggle;
				AlphaValue.started += instance.OnAlphaValue;
				AlphaValue.performed += instance.OnAlphaValue;
				AlphaValue.canceled += instance.OnAlphaValue;
				ColorMove.started += instance.OnColorMove;
				ColorMove.performed += instance.OnColorMove;
				ColorMove.canceled += instance.OnColorMove;
				ColorCode.started += instance.OnColorCode;
				ColorCode.performed += instance.OnColorCode;
				ColorCode.canceled += instance.OnColorCode;
			}
		}

		private void UnregisterCallbacks(IPaintColorActions instance)
		{
			ColorToggle.started -= instance.OnColorToggle;
			ColorToggle.performed -= instance.OnColorToggle;
			ColorToggle.canceled -= instance.OnColorToggle;
			TintToggle.started -= instance.OnTintToggle;
			TintToggle.performed -= instance.OnTintToggle;
			TintToggle.canceled -= instance.OnTintToggle;
			AlphaValue.started -= instance.OnAlphaValue;
			AlphaValue.performed -= instance.OnAlphaValue;
			AlphaValue.canceled -= instance.OnAlphaValue;
			ColorMove.started -= instance.OnColorMove;
			ColorMove.performed -= instance.OnColorMove;
			ColorMove.canceled -= instance.OnColorMove;
			ColorCode.started -= instance.OnColorCode;
			ColorCode.performed -= instance.OnColorCode;
			ColorCode.canceled -= instance.OnColorCode;
		}

		public void RemoveCallbacks(IPaintColorActions instance)
		{
			if (m_Wrapper.m_PaintColorActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintColorActions instance)
		{
			foreach (IPaintColorActions paintColorActionsCallbackInterface in m_Wrapper.m_PaintColorActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintColorActionsCallbackInterface);
			}
			m_Wrapper.m_PaintColorActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintSizeActions
	{
		private GameInputController m_Wrapper;

		public InputAction Mirror => m_Wrapper.m_PaintSize_Mirror;

		public InputAction Turn => m_Wrapper.m_PaintSize_Turn;

		public InputAction SizeMove => m_Wrapper.m_PaintSize_SizeMove;

		public InputAction SetSize => m_Wrapper.m_PaintSize_SetSize;

		public bool enabled => Get().enabled;

		public PaintSizeActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintSize;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintSizeActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintSizeActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintSizeActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintSizeActionsCallbackInterfaces.Add(instance);
				Mirror.started += instance.OnMirror;
				Mirror.performed += instance.OnMirror;
				Mirror.canceled += instance.OnMirror;
				Turn.started += instance.OnTurn;
				Turn.performed += instance.OnTurn;
				Turn.canceled += instance.OnTurn;
				SizeMove.started += instance.OnSizeMove;
				SizeMove.performed += instance.OnSizeMove;
				SizeMove.canceled += instance.OnSizeMove;
				SetSize.started += instance.OnSetSize;
				SetSize.performed += instance.OnSetSize;
				SetSize.canceled += instance.OnSetSize;
			}
		}

		private void UnregisterCallbacks(IPaintSizeActions instance)
		{
			Mirror.started -= instance.OnMirror;
			Mirror.performed -= instance.OnMirror;
			Mirror.canceled -= instance.OnMirror;
			Turn.started -= instance.OnTurn;
			Turn.performed -= instance.OnTurn;
			Turn.canceled -= instance.OnTurn;
			SizeMove.started -= instance.OnSizeMove;
			SizeMove.performed -= instance.OnSizeMove;
			SizeMove.canceled -= instance.OnSizeMove;
			SetSize.started -= instance.OnSetSize;
			SetSize.performed -= instance.OnSetSize;
			SetSize.canceled -= instance.OnSetSize;
		}

		public void RemoveCallbacks(IPaintSizeActions instance)
		{
			if (m_Wrapper.m_PaintSizeActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintSizeActions instance)
		{
			foreach (IPaintSizeActions paintSizeActionsCallbackInterface in m_Wrapper.m_PaintSizeActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintSizeActionsCallbackInterface);
			}
			m_Wrapper.m_PaintSizeActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintTextActions
	{
		private GameInputController m_Wrapper;

		public InputAction Font => m_Wrapper.m_PaintText_Font;

		public InputAction Bold => m_Wrapper.m_PaintText_Bold;

		public InputAction Italics => m_Wrapper.m_PaintText_Italics;

		public InputAction EnterText => m_Wrapper.m_PaintText_EnterText;

		public bool enabled => Get().enabled;

		public PaintTextActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintText;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintTextActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintTextActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintTextActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintTextActionsCallbackInterfaces.Add(instance);
				Font.started += instance.OnFont;
				Font.performed += instance.OnFont;
				Font.canceled += instance.OnFont;
				Bold.started += instance.OnBold;
				Bold.performed += instance.OnBold;
				Bold.canceled += instance.OnBold;
				Italics.started += instance.OnItalics;
				Italics.performed += instance.OnItalics;
				Italics.canceled += instance.OnItalics;
				EnterText.started += instance.OnEnterText;
				EnterText.performed += instance.OnEnterText;
				EnterText.canceled += instance.OnEnterText;
			}
		}

		private void UnregisterCallbacks(IPaintTextActions instance)
		{
			Font.started -= instance.OnFont;
			Font.performed -= instance.OnFont;
			Font.canceled -= instance.OnFont;
			Bold.started -= instance.OnBold;
			Bold.performed -= instance.OnBold;
			Bold.canceled -= instance.OnBold;
			Italics.started -= instance.OnItalics;
			Italics.performed -= instance.OnItalics;
			Italics.canceled -= instance.OnItalics;
			EnterText.started -= instance.OnEnterText;
			EnterText.performed -= instance.OnEnterText;
			EnterText.canceled -= instance.OnEnterText;
		}

		public void RemoveCallbacks(IPaintTextActions instance)
		{
			if (m_Wrapper.m_PaintTextActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintTextActions instance)
		{
			foreach (IPaintTextActions paintTextActionsCallbackInterface in m_Wrapper.m_PaintTextActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintTextActionsCallbackInterface);
			}
			m_Wrapper.m_PaintTextActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PaintEditTextActions
	{
		private GameInputController m_Wrapper;

		public InputAction EditTextStyle => m_Wrapper.m_PaintEditText_EditTextStyle;

		public bool enabled => Get().enabled;

		public PaintEditTextActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_PaintEditText;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PaintEditTextActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPaintEditTextActions instance)
		{
			if (instance != null && !m_Wrapper.m_PaintEditTextActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PaintEditTextActionsCallbackInterfaces.Add(instance);
				EditTextStyle.started += instance.OnEditTextStyle;
				EditTextStyle.performed += instance.OnEditTextStyle;
				EditTextStyle.canceled += instance.OnEditTextStyle;
			}
		}

		private void UnregisterCallbacks(IPaintEditTextActions instance)
		{
			EditTextStyle.started -= instance.OnEditTextStyle;
			EditTextStyle.performed -= instance.OnEditTextStyle;
			EditTextStyle.canceled -= instance.OnEditTextStyle;
		}

		public void RemoveCallbacks(IPaintEditTextActions instance)
		{
			if (m_Wrapper.m_PaintEditTextActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPaintEditTextActions instance)
		{
			foreach (IPaintEditTextActions paintEditTextActionsCallbackInterface in m_Wrapper.m_PaintEditTextActionsCallbackInterfaces)
			{
				UnregisterCallbacks(paintEditTextActionsCallbackInterface);
			}
			m_Wrapper.m_PaintEditTextActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct CameraActions
	{
		private GameInputController m_Wrapper;

		public InputAction Look => m_Wrapper.m_Camera_Look;

		public InputAction LookFree => m_Wrapper.m_Camera_LookFree;

		public bool enabled => Get().enabled;

		public CameraActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Camera;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CameraActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ICameraActions instance)
		{
			if (instance != null && !m_Wrapper.m_CameraActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_CameraActionsCallbackInterfaces.Add(instance);
				Look.started += instance.OnLook;
				Look.performed += instance.OnLook;
				Look.canceled += instance.OnLook;
				LookFree.started += instance.OnLookFree;
				LookFree.performed += instance.OnLookFree;
				LookFree.canceled += instance.OnLookFree;
			}
		}

		private void UnregisterCallbacks(ICameraActions instance)
		{
			Look.started -= instance.OnLook;
			Look.performed -= instance.OnLook;
			Look.canceled -= instance.OnLook;
			LookFree.started -= instance.OnLookFree;
			LookFree.performed -= instance.OnLookFree;
			LookFree.canceled -= instance.OnLookFree;
		}

		public void RemoveCallbacks(ICameraActions instance)
		{
			if (m_Wrapper.m_CameraActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ICameraActions instance)
		{
			foreach (ICameraActions cameraActionsCallbackInterface in m_Wrapper.m_CameraActionsCallbackInterfaces)
			{
				UnregisterCallbacks(cameraActionsCallbackInterface);
			}
			m_Wrapper.m_CameraActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct MoveActions
	{
		private GameInputController m_Wrapper;

		public InputAction Move => m_Wrapper.m_Move_Move;

		public InputAction Rotate => m_Wrapper.m_Move_Rotate;

		public InputAction Height => m_Wrapper.m_Move_Height;

		public bool enabled => Get().enabled;

		public MoveActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Move;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(MoveActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IMoveActions instance)
		{
			if (instance != null && !m_Wrapper.m_MoveActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_MoveActionsCallbackInterfaces.Add(instance);
				Move.started += instance.OnMove;
				Move.performed += instance.OnMove;
				Move.canceled += instance.OnMove;
				Rotate.started += instance.OnRotate;
				Rotate.performed += instance.OnRotate;
				Rotate.canceled += instance.OnRotate;
				Height.started += instance.OnHeight;
				Height.performed += instance.OnHeight;
				Height.canceled += instance.OnHeight;
			}
		}

		private void UnregisterCallbacks(IMoveActions instance)
		{
			Move.started -= instance.OnMove;
			Move.performed -= instance.OnMove;
			Move.canceled -= instance.OnMove;
			Rotate.started -= instance.OnRotate;
			Rotate.performed -= instance.OnRotate;
			Rotate.canceled -= instance.OnRotate;
			Height.started -= instance.OnHeight;
			Height.performed -= instance.OnHeight;
			Height.canceled -= instance.OnHeight;
		}

		public void RemoveCallbacks(IMoveActions instance)
		{
			if (m_Wrapper.m_MoveActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IMoveActions instance)
		{
			foreach (IMoveActions moveActionsCallbackInterface in m_Wrapper.m_MoveActionsCallbackInterfaces)
			{
				UnregisterCallbacks(moveActionsCallbackInterface);
			}
			m_Wrapper.m_MoveActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ZoomMouseActions
	{
		private GameInputController m_Wrapper;

		public InputAction Zoom => m_Wrapper.m_ZoomMouse_Zoom;

		public bool enabled => Get().enabled;

		public ZoomMouseActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_ZoomMouse;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ZoomMouseActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IZoomMouseActions instance)
		{
			if (instance != null && !m_Wrapper.m_ZoomMouseActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ZoomMouseActionsCallbackInterfaces.Add(instance);
				Zoom.started += instance.OnZoom;
				Zoom.performed += instance.OnZoom;
				Zoom.canceled += instance.OnZoom;
			}
		}

		private void UnregisterCallbacks(IZoomMouseActions instance)
		{
			Zoom.started -= instance.OnZoom;
			Zoom.performed -= instance.OnZoom;
			Zoom.canceled -= instance.OnZoom;
		}

		public void RemoveCallbacks(IZoomMouseActions instance)
		{
			if (m_Wrapper.m_ZoomMouseActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IZoomMouseActions instance)
		{
			foreach (IZoomMouseActions zoomMouseActionsCallbackInterface in m_Wrapper.m_ZoomMouseActionsCallbackInterfaces)
			{
				UnregisterCallbacks(zoomMouseActionsCallbackInterface);
			}
			m_Wrapper.m_ZoomMouseActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ZoomGamepadActions
	{
		private GameInputController m_Wrapper;

		public InputAction Zoom => m_Wrapper.m_ZoomGamepad_Zoom;

		public bool enabled => Get().enabled;

		public ZoomGamepadActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_ZoomGamepad;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ZoomGamepadActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IZoomGamepadActions instance)
		{
			if (instance != null && !m_Wrapper.m_ZoomGamepadActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ZoomGamepadActionsCallbackInterfaces.Add(instance);
				Zoom.started += instance.OnZoom;
				Zoom.performed += instance.OnZoom;
				Zoom.canceled += instance.OnZoom;
			}
		}

		private void UnregisterCallbacks(IZoomGamepadActions instance)
		{
			Zoom.started -= instance.OnZoom;
			Zoom.performed -= instance.OnZoom;
			Zoom.canceled -= instance.OnZoom;
		}

		public void RemoveCallbacks(IZoomGamepadActions instance)
		{
			if (m_Wrapper.m_ZoomGamepadActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IZoomGamepadActions instance)
		{
			foreach (IZoomGamepadActions zoomGamepadActionsCallbackInterface in m_Wrapper.m_ZoomGamepadActionsCallbackInterfaces)
			{
				UnregisterCallbacks(zoomGamepadActionsCallbackInterface);
			}
			m_Wrapper.m_ZoomGamepadActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ZoomMozaActions
	{
		private GameInputController m_Wrapper;

		public InputAction Zoom => m_Wrapper.m_ZoomMoza_Zoom;

		public bool enabled => Get().enabled;

		public ZoomMozaActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_ZoomMoza;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ZoomMozaActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IZoomMozaActions instance)
		{
			if (instance != null && !m_Wrapper.m_ZoomMozaActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ZoomMozaActionsCallbackInterfaces.Add(instance);
				Zoom.started += instance.OnZoom;
				Zoom.performed += instance.OnZoom;
				Zoom.canceled += instance.OnZoom;
			}
		}

		private void UnregisterCallbacks(IZoomMozaActions instance)
		{
			Zoom.started -= instance.OnZoom;
			Zoom.performed -= instance.OnZoom;
			Zoom.canceled -= instance.OnZoom;
		}

		public void RemoveCallbacks(IZoomMozaActions instance)
		{
			if (m_Wrapper.m_ZoomMozaActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IZoomMozaActions instance)
		{
			foreach (IZoomMozaActions zoomMozaActionsCallbackInterface in m_Wrapper.m_ZoomMozaActionsCallbackInterfaces)
			{
				UnregisterCallbacks(zoomMozaActionsCallbackInterface);
			}
			m_Wrapper.m_ZoomMozaActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct CameraCarActions
	{
		private GameInputController m_Wrapper;

		public InputAction CameraLookLeft => m_Wrapper.m_CameraCar_CameraLookLeft;

		public InputAction CameraLookRight => m_Wrapper.m_CameraCar_CameraLookRight;

		public InputAction CameraLookBack => m_Wrapper.m_CameraCar_CameraLookBack;

		public bool enabled => Get().enabled;

		public CameraCarActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_CameraCar;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CameraCarActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ICameraCarActions instance)
		{
			if (instance != null && !m_Wrapper.m_CameraCarActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_CameraCarActionsCallbackInterfaces.Add(instance);
				CameraLookLeft.started += instance.OnCameraLookLeft;
				CameraLookLeft.performed += instance.OnCameraLookLeft;
				CameraLookLeft.canceled += instance.OnCameraLookLeft;
				CameraLookRight.started += instance.OnCameraLookRight;
				CameraLookRight.performed += instance.OnCameraLookRight;
				CameraLookRight.canceled += instance.OnCameraLookRight;
				CameraLookBack.started += instance.OnCameraLookBack;
				CameraLookBack.performed += instance.OnCameraLookBack;
				CameraLookBack.canceled += instance.OnCameraLookBack;
			}
		}

		private void UnregisterCallbacks(ICameraCarActions instance)
		{
			CameraLookLeft.started -= instance.OnCameraLookLeft;
			CameraLookLeft.performed -= instance.OnCameraLookLeft;
			CameraLookLeft.canceled -= instance.OnCameraLookLeft;
			CameraLookRight.started -= instance.OnCameraLookRight;
			CameraLookRight.performed -= instance.OnCameraLookRight;
			CameraLookRight.canceled -= instance.OnCameraLookRight;
			CameraLookBack.started -= instance.OnCameraLookBack;
			CameraLookBack.performed -= instance.OnCameraLookBack;
			CameraLookBack.canceled -= instance.OnCameraLookBack;
		}

		public void RemoveCallbacks(ICameraCarActions instance)
		{
			if (m_Wrapper.m_CameraCarActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ICameraCarActions instance)
		{
			foreach (ICameraCarActions cameraCarActionsCallbackInterface in m_Wrapper.m_CameraCarActionsCallbackInterfaces)
			{
				UnregisterCallbacks(cameraCarActionsCallbackInterface);
			}
			m_Wrapper.m_CameraCarActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct AimReticleActions
	{
		private GameInputController m_Wrapper;

		public InputAction AimAt => m_Wrapper.m_AimReticle_AimAt;

		public bool enabled => Get().enabled;

		public AimReticleActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_AimReticle;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(AimReticleActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IAimReticleActions instance)
		{
			if (instance != null && !m_Wrapper.m_AimReticleActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_AimReticleActionsCallbackInterfaces.Add(instance);
				AimAt.started += instance.OnAimAt;
				AimAt.performed += instance.OnAimAt;
				AimAt.canceled += instance.OnAimAt;
			}
		}

		private void UnregisterCallbacks(IAimReticleActions instance)
		{
			AimAt.started -= instance.OnAimAt;
			AimAt.performed -= instance.OnAimAt;
			AimAt.canceled -= instance.OnAimAt;
		}

		public void RemoveCallbacks(IAimReticleActions instance)
		{
			if (m_Wrapper.m_AimReticleActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IAimReticleActions instance)
		{
			foreach (IAimReticleActions aimReticleActionsCallbackInterface in m_Wrapper.m_AimReticleActionsCallbackInterfaces)
			{
				UnregisterCallbacks(aimReticleActionsCallbackInterface);
			}
			m_Wrapper.m_AimReticleActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ChangeCameraActions
	{
		private GameInputController m_Wrapper;

		public InputAction ChangeCamera => m_Wrapper.m_ChangeCamera_ChangeCamera;

		public bool enabled => Get().enabled;

		public ChangeCameraActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_ChangeCamera;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ChangeCameraActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IChangeCameraActions instance)
		{
			if (instance != null && !m_Wrapper.m_ChangeCameraActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ChangeCameraActionsCallbackInterfaces.Add(instance);
				ChangeCamera.started += instance.OnChangeCamera;
				ChangeCamera.performed += instance.OnChangeCamera;
				ChangeCamera.canceled += instance.OnChangeCamera;
			}
		}

		private void UnregisterCallbacks(IChangeCameraActions instance)
		{
			ChangeCamera.started -= instance.OnChangeCamera;
			ChangeCamera.performed -= instance.OnChangeCamera;
			ChangeCamera.canceled -= instance.OnChangeCamera;
		}

		public void RemoveCallbacks(IChangeCameraActions instance)
		{
			if (m_Wrapper.m_ChangeCameraActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IChangeCameraActions instance)
		{
			foreach (IChangeCameraActions changeCameraActionsCallbackInterface in m_Wrapper.m_ChangeCameraActionsCallbackInterfaces)
			{
				UnregisterCallbacks(changeCameraActionsCallbackInterface);
			}
			m_Wrapper.m_ChangeCameraActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct CarRacingActions
	{
		private GameInputController m_Wrapper;

		public InputAction PhotoMode => m_Wrapper.m_CarRacing_PhotoMode;

		public InputAction Timer => m_Wrapper.m_CarRacing_Timer;

		public bool enabled => Get().enabled;

		public CarRacingActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_CarRacing;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CarRacingActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ICarRacingActions instance)
		{
			if (instance != null && !m_Wrapper.m_CarRacingActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_CarRacingActionsCallbackInterfaces.Add(instance);
				PhotoMode.started += instance.OnPhotoMode;
				PhotoMode.performed += instance.OnPhotoMode;
				PhotoMode.canceled += instance.OnPhotoMode;
				Timer.started += instance.OnTimer;
				Timer.performed += instance.OnTimer;
				Timer.canceled += instance.OnTimer;
			}
		}

		private void UnregisterCallbacks(ICarRacingActions instance)
		{
			PhotoMode.started -= instance.OnPhotoMode;
			PhotoMode.performed -= instance.OnPhotoMode;
			PhotoMode.canceled -= instance.OnPhotoMode;
			Timer.started -= instance.OnTimer;
			Timer.performed -= instance.OnTimer;
			Timer.canceled -= instance.OnTimer;
		}

		public void RemoveCallbacks(ICarRacingActions instance)
		{
			if (m_Wrapper.m_CarRacingActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ICarRacingActions instance)
		{
			foreach (ICarRacingActions carRacingActionsCallbackInterface in m_Wrapper.m_CarRacingActionsCallbackInterfaces)
			{
				UnregisterCallbacks(carRacingActionsCallbackInterface);
			}
			m_Wrapper.m_CarRacingActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct CarDragActions
	{
		private GameInputController m_Wrapper;

		public InputAction Burnout => m_Wrapper.m_CarDrag_Burnout;

		public InputAction KeepLane => m_Wrapper.m_CarDrag_KeepLane;

		public bool enabled => Get().enabled;

		public CarDragActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_CarDrag;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CarDragActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ICarDragActions instance)
		{
			if (instance != null && !m_Wrapper.m_CarDragActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_CarDragActionsCallbackInterfaces.Add(instance);
				Burnout.started += instance.OnBurnout;
				Burnout.performed += instance.OnBurnout;
				Burnout.canceled += instance.OnBurnout;
				KeepLane.started += instance.OnKeepLane;
				KeepLane.performed += instance.OnKeepLane;
				KeepLane.canceled += instance.OnKeepLane;
			}
		}

		private void UnregisterCallbacks(ICarDragActions instance)
		{
			Burnout.started -= instance.OnBurnout;
			Burnout.performed -= instance.OnBurnout;
			Burnout.canceled -= instance.OnBurnout;
			KeepLane.started -= instance.OnKeepLane;
			KeepLane.performed -= instance.OnKeepLane;
			KeepLane.canceled -= instance.OnKeepLane;
		}

		public void RemoveCallbacks(ICarDragActions instance)
		{
			if (m_Wrapper.m_CarDragActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ICarDragActions instance)
		{
			foreach (ICarDragActions carDragActionsCallbackInterface in m_Wrapper.m_CarDragActionsCallbackInterfaces)
			{
				UnregisterCallbacks(carDragActionsCallbackInterface);
			}
			m_Wrapper.m_CarDragActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct BackToCarActions
	{
		private GameInputController m_Wrapper;

		public InputAction BackToCar => m_Wrapper.m_BackToCar_BackToCar;

		public bool enabled => Get().enabled;

		public BackToCarActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_BackToCar;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(BackToCarActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IBackToCarActions instance)
		{
			if (instance != null && !m_Wrapper.m_BackToCarActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_BackToCarActionsCallbackInterfaces.Add(instance);
				BackToCar.started += instance.OnBackToCar;
				BackToCar.performed += instance.OnBackToCar;
				BackToCar.canceled += instance.OnBackToCar;
			}
		}

		private void UnregisterCallbacks(IBackToCarActions instance)
		{
			BackToCar.started -= instance.OnBackToCar;
			BackToCar.performed -= instance.OnBackToCar;
			BackToCar.canceled -= instance.OnBackToCar;
		}

		public void RemoveCallbacks(IBackToCarActions instance)
		{
			if (m_Wrapper.m_BackToCarActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IBackToCarActions instance)
		{
			foreach (IBackToCarActions backToCarActionsCallbackInterface in m_Wrapper.m_BackToCarActionsCallbackInterfaces)
			{
				UnregisterCallbacks(backToCarActionsCallbackInterface);
			}
			m_Wrapper.m_BackToCarActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ToRoadActions
	{
		private GameInputController m_Wrapper;

		public InputAction ToRoad => m_Wrapper.m_ToRoad_ToRoad;

		public bool enabled => Get().enabled;

		public ToRoadActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_ToRoad;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ToRoadActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IToRoadActions instance)
		{
			if (instance != null && !m_Wrapper.m_ToRoadActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ToRoadActionsCallbackInterfaces.Add(instance);
				ToRoad.started += instance.OnToRoad;
				ToRoad.performed += instance.OnToRoad;
				ToRoad.canceled += instance.OnToRoad;
			}
		}

		private void UnregisterCallbacks(IToRoadActions instance)
		{
			ToRoad.started -= instance.OnToRoad;
			ToRoad.performed -= instance.OnToRoad;
			ToRoad.canceled -= instance.OnToRoad;
		}

		public void RemoveCallbacks(IToRoadActions instance)
		{
			if (m_Wrapper.m_ToRoadActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IToRoadActions instance)
		{
			foreach (IToRoadActions toRoadActionsCallbackInterface in m_Wrapper.m_ToRoadActionsCallbackInterfaces)
			{
				UnregisterCallbacks(toRoadActionsCallbackInterface);
			}
			m_Wrapper.m_ToRoadActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ToRoadSteeringWheelActions
	{
		private GameInputController m_Wrapper;

		public InputAction ToRoad => m_Wrapper.m_ToRoadSteeringWheel_ToRoad;

		public bool enabled => Get().enabled;

		public ToRoadSteeringWheelActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_ToRoadSteeringWheel;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ToRoadSteeringWheelActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IToRoadSteeringWheelActions instance)
		{
			if (instance != null && !m_Wrapper.m_ToRoadSteeringWheelActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ToRoadSteeringWheelActionsCallbackInterfaces.Add(instance);
				ToRoad.started += instance.OnToRoad;
				ToRoad.performed += instance.OnToRoad;
				ToRoad.canceled += instance.OnToRoad;
			}
		}

		private void UnregisterCallbacks(IToRoadSteeringWheelActions instance)
		{
			ToRoad.started -= instance.OnToRoad;
			ToRoad.performed -= instance.OnToRoad;
			ToRoad.canceled -= instance.OnToRoad;
		}

		public void RemoveCallbacks(IToRoadSteeringWheelActions instance)
		{
			if (m_Wrapper.m_ToRoadSteeringWheelActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IToRoadSteeringWheelActions instance)
		{
			foreach (IToRoadSteeringWheelActions toRoadSteeringWheelActionsCallbackInterface in m_Wrapper.m_ToRoadSteeringWheelActionsCallbackInterfaces)
			{
				UnregisterCallbacks(toRoadSteeringWheelActionsCallbackInterface);
			}
			m_Wrapper.m_ToRoadSteeringWheelActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct HeadlightsActions
	{
		private GameInputController m_Wrapper;

		public InputAction Headlights => m_Wrapper.m_Headlights_Headlights;

		public bool enabled => Get().enabled;

		public HeadlightsActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Headlights;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(HeadlightsActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IHeadlightsActions instance)
		{
			if (instance != null && !m_Wrapper.m_HeadlightsActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_HeadlightsActionsCallbackInterfaces.Add(instance);
				Headlights.started += instance.OnHeadlights;
				Headlights.performed += instance.OnHeadlights;
				Headlights.canceled += instance.OnHeadlights;
			}
		}

		private void UnregisterCallbacks(IHeadlightsActions instance)
		{
			Headlights.started -= instance.OnHeadlights;
			Headlights.performed -= instance.OnHeadlights;
			Headlights.canceled -= instance.OnHeadlights;
		}

		public void RemoveCallbacks(IHeadlightsActions instance)
		{
			if (m_Wrapper.m_HeadlightsActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IHeadlightsActions instance)
		{
			foreach (IHeadlightsActions headlightsActionsCallbackInterface in m_Wrapper.m_HeadlightsActionsCallbackInterfaces)
			{
				UnregisterCallbacks(headlightsActionsCallbackInterface);
			}
			m_Wrapper.m_HeadlightsActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct DoorActions
	{
		private GameInputController m_Wrapper;

		public InputAction OpenDoor => m_Wrapper.m_Door_OpenDoor;

		public bool enabled => Get().enabled;

		public DoorActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Door;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(DoorActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IDoorActions instance)
		{
			if (instance != null && !m_Wrapper.m_DoorActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_DoorActionsCallbackInterfaces.Add(instance);
				OpenDoor.started += instance.OnOpenDoor;
				OpenDoor.performed += instance.OnOpenDoor;
				OpenDoor.canceled += instance.OnOpenDoor;
			}
		}

		private void UnregisterCallbacks(IDoorActions instance)
		{
			OpenDoor.started -= instance.OnOpenDoor;
			OpenDoor.performed -= instance.OnOpenDoor;
			OpenDoor.canceled -= instance.OnOpenDoor;
		}

		public void RemoveCallbacks(IDoorActions instance)
		{
			if (m_Wrapper.m_DoorActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IDoorActions instance)
		{
			foreach (IDoorActions doorActionsCallbackInterface in m_Wrapper.m_DoorActionsCallbackInterfaces)
			{
				UnregisterCallbacks(doorActionsCallbackInterface);
			}
			m_Wrapper.m_DoorActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct FreeRaceActions
	{
		private GameInputController m_Wrapper;

		public InputAction Restart => m_Wrapper.m_FreeRace_Restart;

		public InputAction CancelRace => m_Wrapper.m_FreeRace_CancelRace;

		public bool enabled => Get().enabled;

		public FreeRaceActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_FreeRace;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(FreeRaceActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IFreeRaceActions instance)
		{
			if (instance != null && !m_Wrapper.m_FreeRaceActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_FreeRaceActionsCallbackInterfaces.Add(instance);
				Restart.started += instance.OnRestart;
				Restart.performed += instance.OnRestart;
				Restart.canceled += instance.OnRestart;
				CancelRace.started += instance.OnCancelRace;
				CancelRace.performed += instance.OnCancelRace;
				CancelRace.canceled += instance.OnCancelRace;
			}
		}

		private void UnregisterCallbacks(IFreeRaceActions instance)
		{
			Restart.started -= instance.OnRestart;
			Restart.performed -= instance.OnRestart;
			Restart.canceled -= instance.OnRestart;
			CancelRace.started -= instance.OnCancelRace;
			CancelRace.performed -= instance.OnCancelRace;
			CancelRace.canceled -= instance.OnCancelRace;
		}

		public void RemoveCallbacks(IFreeRaceActions instance)
		{
			if (m_Wrapper.m_FreeRaceActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IFreeRaceActions instance)
		{
			foreach (IFreeRaceActions freeRaceActionsCallbackInterface in m_Wrapper.m_FreeRaceActionsCallbackInterfaces)
			{
				UnregisterCallbacks(freeRaceActionsCallbackInterface);
			}
			m_Wrapper.m_FreeRaceActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PhoneActions
	{
		private GameInputController m_Wrapper;

		public InputAction Phone => m_Wrapper.m_Phone_Phone;

		public bool enabled => Get().enabled;

		public PhoneActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Phone;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PhoneActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPhoneActions instance)
		{
			if (instance != null && !m_Wrapper.m_PhoneActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PhoneActionsCallbackInterfaces.Add(instance);
				Phone.started += instance.OnPhone;
				Phone.performed += instance.OnPhone;
				Phone.canceled += instance.OnPhone;
			}
		}

		private void UnregisterCallbacks(IPhoneActions instance)
		{
			Phone.started -= instance.OnPhone;
			Phone.performed -= instance.OnPhone;
			Phone.canceled -= instance.OnPhone;
		}

		public void RemoveCallbacks(IPhoneActions instance)
		{
			if (m_Wrapper.m_PhoneActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPhoneActions instance)
		{
			foreach (IPhoneActions phoneActionsCallbackInterface in m_Wrapper.m_PhoneActionsCallbackInterfaces)
			{
				UnregisterCallbacks(phoneActionsCallbackInterface);
			}
			m_Wrapper.m_PhoneActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct VoiceActions
	{
		private GameInputController m_Wrapper;

		public InputAction VoiceChat => m_Wrapper.m_Voice_VoiceChat;

		public InputAction Chat => m_Wrapper.m_Voice_Chat;

		public bool enabled => Get().enabled;

		public VoiceActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Voice;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(VoiceActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IVoiceActions instance)
		{
			if (instance != null && !m_Wrapper.m_VoiceActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_VoiceActionsCallbackInterfaces.Add(instance);
				VoiceChat.started += instance.OnVoiceChat;
				VoiceChat.performed += instance.OnVoiceChat;
				VoiceChat.canceled += instance.OnVoiceChat;
				Chat.started += instance.OnChat;
				Chat.performed += instance.OnChat;
				Chat.canceled += instance.OnChat;
			}
		}

		private void UnregisterCallbacks(IVoiceActions instance)
		{
			VoiceChat.started -= instance.OnVoiceChat;
			VoiceChat.performed -= instance.OnVoiceChat;
			VoiceChat.canceled -= instance.OnVoiceChat;
			Chat.started -= instance.OnChat;
			Chat.performed -= instance.OnChat;
			Chat.canceled -= instance.OnChat;
		}

		public void RemoveCallbacks(IVoiceActions instance)
		{
			if (m_Wrapper.m_VoiceActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IVoiceActions instance)
		{
			foreach (IVoiceActions voiceActionsCallbackInterface in m_Wrapper.m_VoiceActionsCallbackInterfaces)
			{
				UnregisterCallbacks(voiceActionsCallbackInterface);
			}
			m_Wrapper.m_VoiceActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ChatGamepadActions
	{
		private GameInputController m_Wrapper;

		public InputAction Chat => m_Wrapper.m_ChatGamepad_Chat;

		public bool enabled => Get().enabled;

		public ChatGamepadActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_ChatGamepad;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ChatGamepadActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IChatGamepadActions instance)
		{
			if (instance != null && !m_Wrapper.m_ChatGamepadActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ChatGamepadActionsCallbackInterfaces.Add(instance);
				Chat.started += instance.OnChat;
				Chat.performed += instance.OnChat;
				Chat.canceled += instance.OnChat;
			}
		}

		private void UnregisterCallbacks(IChatGamepadActions instance)
		{
			Chat.started -= instance.OnChat;
			Chat.performed -= instance.OnChat;
			Chat.canceled -= instance.OnChat;
		}

		public void RemoveCallbacks(IChatGamepadActions instance)
		{
			if (m_Wrapper.m_ChatGamepadActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IChatGamepadActions instance)
		{
			foreach (IChatGamepadActions chatGamepadActionsCallbackInterface in m_Wrapper.m_ChatGamepadActionsCallbackInterfaces)
			{
				UnregisterCallbacks(chatGamepadActionsCallbackInterface);
			}
			m_Wrapper.m_ChatGamepadActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct CircleMenuActions
	{
		private GameInputController m_Wrapper;

		public InputAction CircleMenuMove => m_Wrapper.m_CircleMenu_CircleMenuMove;

		public InputAction CircleMenuClose => m_Wrapper.m_CircleMenu_CircleMenuClose;

		public InputAction CircleMenuCloseCam => m_Wrapper.m_CircleMenu_CircleMenuCloseCam;

		public InputAction CircleMenuCloseAnim => m_Wrapper.m_CircleMenu_CircleMenuCloseAnim;

		public InputAction CircleMenuScroll => m_Wrapper.m_CircleMenu_CircleMenuScroll;

		public bool enabled => Get().enabled;

		public CircleMenuActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_CircleMenu;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CircleMenuActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ICircleMenuActions instance)
		{
			if (instance != null && !m_Wrapper.m_CircleMenuActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_CircleMenuActionsCallbackInterfaces.Add(instance);
				CircleMenuMove.started += instance.OnCircleMenuMove;
				CircleMenuMove.performed += instance.OnCircleMenuMove;
				CircleMenuMove.canceled += instance.OnCircleMenuMove;
				CircleMenuClose.started += instance.OnCircleMenuClose;
				CircleMenuClose.performed += instance.OnCircleMenuClose;
				CircleMenuClose.canceled += instance.OnCircleMenuClose;
				CircleMenuCloseCam.started += instance.OnCircleMenuCloseCam;
				CircleMenuCloseCam.performed += instance.OnCircleMenuCloseCam;
				CircleMenuCloseCam.canceled += instance.OnCircleMenuCloseCam;
				CircleMenuCloseAnim.started += instance.OnCircleMenuCloseAnim;
				CircleMenuCloseAnim.performed += instance.OnCircleMenuCloseAnim;
				CircleMenuCloseAnim.canceled += instance.OnCircleMenuCloseAnim;
				CircleMenuScroll.started += instance.OnCircleMenuScroll;
				CircleMenuScroll.performed += instance.OnCircleMenuScroll;
				CircleMenuScroll.canceled += instance.OnCircleMenuScroll;
			}
		}

		private void UnregisterCallbacks(ICircleMenuActions instance)
		{
			CircleMenuMove.started -= instance.OnCircleMenuMove;
			CircleMenuMove.performed -= instance.OnCircleMenuMove;
			CircleMenuMove.canceled -= instance.OnCircleMenuMove;
			CircleMenuClose.started -= instance.OnCircleMenuClose;
			CircleMenuClose.performed -= instance.OnCircleMenuClose;
			CircleMenuClose.canceled -= instance.OnCircleMenuClose;
			CircleMenuCloseCam.started -= instance.OnCircleMenuCloseCam;
			CircleMenuCloseCam.performed -= instance.OnCircleMenuCloseCam;
			CircleMenuCloseCam.canceled -= instance.OnCircleMenuCloseCam;
			CircleMenuCloseAnim.started -= instance.OnCircleMenuCloseAnim;
			CircleMenuCloseAnim.performed -= instance.OnCircleMenuCloseAnim;
			CircleMenuCloseAnim.canceled -= instance.OnCircleMenuCloseAnim;
			CircleMenuScroll.started -= instance.OnCircleMenuScroll;
			CircleMenuScroll.performed -= instance.OnCircleMenuScroll;
			CircleMenuScroll.canceled -= instance.OnCircleMenuScroll;
		}

		public void RemoveCallbacks(ICircleMenuActions instance)
		{
			if (m_Wrapper.m_CircleMenuActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ICircleMenuActions instance)
		{
			foreach (ICircleMenuActions circleMenuActionsCallbackInterface in m_Wrapper.m_CircleMenuActionsCallbackInterfaces)
			{
				UnregisterCallbacks(circleMenuActionsCallbackInterface);
			}
			m_Wrapper.m_CircleMenuActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct OpenCircleMenuActions
	{
		private GameInputController m_Wrapper;

		public InputAction OpenCircleMenu => m_Wrapper.m_OpenCircleMenu_OpenCircleMenu;

		public InputAction OpenCircleMenuCam => m_Wrapper.m_OpenCircleMenu_OpenCircleMenuCam;

		public bool enabled => Get().enabled;

		public OpenCircleMenuActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_OpenCircleMenu;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(OpenCircleMenuActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IOpenCircleMenuActions instance)
		{
			if (instance != null && !m_Wrapper.m_OpenCircleMenuActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_OpenCircleMenuActionsCallbackInterfaces.Add(instance);
				OpenCircleMenu.started += instance.OnOpenCircleMenu;
				OpenCircleMenu.performed += instance.OnOpenCircleMenu;
				OpenCircleMenu.canceled += instance.OnOpenCircleMenu;
				OpenCircleMenuCam.started += instance.OnOpenCircleMenuCam;
				OpenCircleMenuCam.performed += instance.OnOpenCircleMenuCam;
				OpenCircleMenuCam.canceled += instance.OnOpenCircleMenuCam;
			}
		}

		private void UnregisterCallbacks(IOpenCircleMenuActions instance)
		{
			OpenCircleMenu.started -= instance.OnOpenCircleMenu;
			OpenCircleMenu.performed -= instance.OnOpenCircleMenu;
			OpenCircleMenu.canceled -= instance.OnOpenCircleMenu;
			OpenCircleMenuCam.started -= instance.OnOpenCircleMenuCam;
			OpenCircleMenuCam.performed -= instance.OnOpenCircleMenuCam;
			OpenCircleMenuCam.canceled -= instance.OnOpenCircleMenuCam;
		}

		public void RemoveCallbacks(IOpenCircleMenuActions instance)
		{
			if (m_Wrapper.m_OpenCircleMenuActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IOpenCircleMenuActions instance)
		{
			foreach (IOpenCircleMenuActions openCircleMenuActionsCallbackInterface in m_Wrapper.m_OpenCircleMenuActionsCallbackInterfaces)
			{
				UnregisterCallbacks(openCircleMenuActionsCallbackInterface);
			}
			m_Wrapper.m_OpenCircleMenuActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct CarBaseActions
	{
		private GameInputController m_Wrapper;

		public InputAction Steering => m_Wrapper.m_CarBase_Steering;

		public InputAction SteeringMouse => m_Wrapper.m_CarBase_SteeringMouse;

		public InputAction SetGearVisible => m_Wrapper.m_CarBase_SetGearVisible;

		public InputAction Throttle => m_Wrapper.m_CarBase_Throttle;

		public InputAction Brake => m_Wrapper.m_CarBase_Brake;

		public InputAction Clutch => m_Wrapper.m_CarBase_Clutch;

		public InputAction Handbrake => m_Wrapper.m_CarBase_Handbrake;

		public InputAction HandbrakeHold => m_Wrapper.m_CarBase_HandbrakeHold;

		public InputAction StartEngine => m_Wrapper.m_CarBase_StartEngine;

		public InputAction Horn => m_Wrapper.m_CarBase_Horn;

		public bool enabled => Get().enabled;

		public CarBaseActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_CarBase;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CarBaseActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ICarBaseActions instance)
		{
			if (instance != null && !m_Wrapper.m_CarBaseActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_CarBaseActionsCallbackInterfaces.Add(instance);
				Steering.started += instance.OnSteering;
				Steering.performed += instance.OnSteering;
				Steering.canceled += instance.OnSteering;
				SteeringMouse.started += instance.OnSteeringMouse;
				SteeringMouse.performed += instance.OnSteeringMouse;
				SteeringMouse.canceled += instance.OnSteeringMouse;
				SetGearVisible.started += instance.OnSetGearVisible;
				SetGearVisible.performed += instance.OnSetGearVisible;
				SetGearVisible.canceled += instance.OnSetGearVisible;
				Throttle.started += instance.OnThrottle;
				Throttle.performed += instance.OnThrottle;
				Throttle.canceled += instance.OnThrottle;
				Brake.started += instance.OnBrake;
				Brake.performed += instance.OnBrake;
				Brake.canceled += instance.OnBrake;
				Clutch.started += instance.OnClutch;
				Clutch.performed += instance.OnClutch;
				Clutch.canceled += instance.OnClutch;
				Handbrake.started += instance.OnHandbrake;
				Handbrake.performed += instance.OnHandbrake;
				Handbrake.canceled += instance.OnHandbrake;
				HandbrakeHold.started += instance.OnHandbrakeHold;
				HandbrakeHold.performed += instance.OnHandbrakeHold;
				HandbrakeHold.canceled += instance.OnHandbrakeHold;
				StartEngine.started += instance.OnStartEngine;
				StartEngine.performed += instance.OnStartEngine;
				StartEngine.canceled += instance.OnStartEngine;
				Horn.started += instance.OnHorn;
				Horn.performed += instance.OnHorn;
				Horn.canceled += instance.OnHorn;
			}
		}

		private void UnregisterCallbacks(ICarBaseActions instance)
		{
			Steering.started -= instance.OnSteering;
			Steering.performed -= instance.OnSteering;
			Steering.canceled -= instance.OnSteering;
			SteeringMouse.started -= instance.OnSteeringMouse;
			SteeringMouse.performed -= instance.OnSteeringMouse;
			SteeringMouse.canceled -= instance.OnSteeringMouse;
			SetGearVisible.started -= instance.OnSetGearVisible;
			SetGearVisible.performed -= instance.OnSetGearVisible;
			SetGearVisible.canceled -= instance.OnSetGearVisible;
			Throttle.started -= instance.OnThrottle;
			Throttle.performed -= instance.OnThrottle;
			Throttle.canceled -= instance.OnThrottle;
			Brake.started -= instance.OnBrake;
			Brake.performed -= instance.OnBrake;
			Brake.canceled -= instance.OnBrake;
			Clutch.started -= instance.OnClutch;
			Clutch.performed -= instance.OnClutch;
			Clutch.canceled -= instance.OnClutch;
			Handbrake.started -= instance.OnHandbrake;
			Handbrake.performed -= instance.OnHandbrake;
			Handbrake.canceled -= instance.OnHandbrake;
			HandbrakeHold.started -= instance.OnHandbrakeHold;
			HandbrakeHold.performed -= instance.OnHandbrakeHold;
			HandbrakeHold.canceled -= instance.OnHandbrakeHold;
			StartEngine.started -= instance.OnStartEngine;
			StartEngine.performed -= instance.OnStartEngine;
			StartEngine.canceled -= instance.OnStartEngine;
			Horn.started -= instance.OnHorn;
			Horn.performed -= instance.OnHorn;
			Horn.canceled -= instance.OnHorn;
		}

		public void RemoveCallbacks(ICarBaseActions instance)
		{
			if (m_Wrapper.m_CarBaseActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ICarBaseActions instance)
		{
			foreach (ICarBaseActions carBaseActionsCallbackInterface in m_Wrapper.m_CarBaseActionsCallbackInterfaces)
			{
				UnregisterCallbacks(carBaseActionsCallbackInterface);
			}
			m_Wrapper.m_CarBaseActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct SteeringWheelCarActions
	{
		private GameInputController m_Wrapper;

		public InputAction gear1 => m_Wrapper.m_SteeringWheelCar_gear1;

		public InputAction gear2 => m_Wrapper.m_SteeringWheelCar_gear2;

		public InputAction gear3 => m_Wrapper.m_SteeringWheelCar_gear3;

		public InputAction gear4 => m_Wrapper.m_SteeringWheelCar_gear4;

		public InputAction gear5 => m_Wrapper.m_SteeringWheelCar_gear5;

		public InputAction gear6 => m_Wrapper.m_SteeringWheelCar_gear6;

		public InputAction gear7 => m_Wrapper.m_SteeringWheelCar_gear7;

		public InputAction gearR => m_Wrapper.m_SteeringWheelCar_gearR;

		public InputAction gearN => m_Wrapper.m_SteeringWheelCar_gearN;

		public InputAction gearUp => m_Wrapper.m_SteeringWheelCar_gearUp;

		public InputAction gearAMToggle => m_Wrapper.m_SteeringWheelCar_gearAMToggle;

		public InputAction gearDown => m_Wrapper.m_SteeringWheelCar_gearDown;

		public InputAction LeftSignal => m_Wrapper.m_SteeringWheelCar_LeftSignal;

		public InputAction RightSignal => m_Wrapper.m_SteeringWheelCar_RightSignal;

		public InputAction Emergency => m_Wrapper.m_SteeringWheelCar_Emergency;

		public bool enabled => Get().enabled;

		public SteeringWheelCarActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_SteeringWheelCar;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(SteeringWheelCarActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ISteeringWheelCarActions instance)
		{
			if (instance != null && !m_Wrapper.m_SteeringWheelCarActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_SteeringWheelCarActionsCallbackInterfaces.Add(instance);
				gear1.started += instance.OnGear1;
				gear1.performed += instance.OnGear1;
				gear1.canceled += instance.OnGear1;
				gear2.started += instance.OnGear2;
				gear2.performed += instance.OnGear2;
				gear2.canceled += instance.OnGear2;
				gear3.started += instance.OnGear3;
				gear3.performed += instance.OnGear3;
				gear3.canceled += instance.OnGear3;
				gear4.started += instance.OnGear4;
				gear4.performed += instance.OnGear4;
				gear4.canceled += instance.OnGear4;
				gear5.started += instance.OnGear5;
				gear5.performed += instance.OnGear5;
				gear5.canceled += instance.OnGear5;
				gear6.started += instance.OnGear6;
				gear6.performed += instance.OnGear6;
				gear6.canceled += instance.OnGear6;
				gear7.started += instance.OnGear7;
				gear7.performed += instance.OnGear7;
				gear7.canceled += instance.OnGear7;
				gearR.started += instance.OnGearR;
				gearR.performed += instance.OnGearR;
				gearR.canceled += instance.OnGearR;
				gearN.started += instance.OnGearN;
				gearN.performed += instance.OnGearN;
				gearN.canceled += instance.OnGearN;
				gearUp.started += instance.OnGearUp;
				gearUp.performed += instance.OnGearUp;
				gearUp.canceled += instance.OnGearUp;
				gearAMToggle.started += instance.OnGearAMToggle;
				gearAMToggle.performed += instance.OnGearAMToggle;
				gearAMToggle.canceled += instance.OnGearAMToggle;
				gearDown.started += instance.OnGearDown;
				gearDown.performed += instance.OnGearDown;
				gearDown.canceled += instance.OnGearDown;
				LeftSignal.started += instance.OnLeftSignal;
				LeftSignal.performed += instance.OnLeftSignal;
				LeftSignal.canceled += instance.OnLeftSignal;
				RightSignal.started += instance.OnRightSignal;
				RightSignal.performed += instance.OnRightSignal;
				RightSignal.canceled += instance.OnRightSignal;
				Emergency.started += instance.OnEmergency;
				Emergency.performed += instance.OnEmergency;
				Emergency.canceled += instance.OnEmergency;
			}
		}

		private void UnregisterCallbacks(ISteeringWheelCarActions instance)
		{
			gear1.started -= instance.OnGear1;
			gear1.performed -= instance.OnGear1;
			gear1.canceled -= instance.OnGear1;
			gear2.started -= instance.OnGear2;
			gear2.performed -= instance.OnGear2;
			gear2.canceled -= instance.OnGear2;
			gear3.started -= instance.OnGear3;
			gear3.performed -= instance.OnGear3;
			gear3.canceled -= instance.OnGear3;
			gear4.started -= instance.OnGear4;
			gear4.performed -= instance.OnGear4;
			gear4.canceled -= instance.OnGear4;
			gear5.started -= instance.OnGear5;
			gear5.performed -= instance.OnGear5;
			gear5.canceled -= instance.OnGear5;
			gear6.started -= instance.OnGear6;
			gear6.performed -= instance.OnGear6;
			gear6.canceled -= instance.OnGear6;
			gear7.started -= instance.OnGear7;
			gear7.performed -= instance.OnGear7;
			gear7.canceled -= instance.OnGear7;
			gearR.started -= instance.OnGearR;
			gearR.performed -= instance.OnGearR;
			gearR.canceled -= instance.OnGearR;
			gearN.started -= instance.OnGearN;
			gearN.performed -= instance.OnGearN;
			gearN.canceled -= instance.OnGearN;
			gearUp.started -= instance.OnGearUp;
			gearUp.performed -= instance.OnGearUp;
			gearUp.canceled -= instance.OnGearUp;
			gearAMToggle.started -= instance.OnGearAMToggle;
			gearAMToggle.performed -= instance.OnGearAMToggle;
			gearAMToggle.canceled -= instance.OnGearAMToggle;
			gearDown.started -= instance.OnGearDown;
			gearDown.performed -= instance.OnGearDown;
			gearDown.canceled -= instance.OnGearDown;
			LeftSignal.started -= instance.OnLeftSignal;
			LeftSignal.performed -= instance.OnLeftSignal;
			LeftSignal.canceled -= instance.OnLeftSignal;
			RightSignal.started -= instance.OnRightSignal;
			RightSignal.performed -= instance.OnRightSignal;
			RightSignal.canceled -= instance.OnRightSignal;
			Emergency.started -= instance.OnEmergency;
			Emergency.performed -= instance.OnEmergency;
			Emergency.canceled -= instance.OnEmergency;
		}

		public void RemoveCallbacks(ISteeringWheelCarActions instance)
		{
			if (m_Wrapper.m_SteeringWheelCarActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ISteeringWheelCarActions instance)
		{
			foreach (ISteeringWheelCarActions steeringWheelCarActionsCallbackInterface in m_Wrapper.m_SteeringWheelCarActionsCallbackInterfaces)
			{
				UnregisterCallbacks(steeringWheelCarActionsCallbackInterface);
			}
			m_Wrapper.m_SteeringWheelCarActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ShifterActions
	{
		private GameInputController m_Wrapper;

		public InputAction ShiftM => m_Wrapper.m_Shifter_ShiftM;

		public InputAction ToggleMA => m_Wrapper.m_Shifter_ToggleMA;

		public InputAction ShiftDown => m_Wrapper.m_Shifter_ShiftDown;

		public InputAction ShiftUp => m_Wrapper.m_Shifter_ShiftUp;

		public InputAction ShiftPRND => m_Wrapper.m_Shifter_ShiftPRND;

		public InputAction ShifterMove => m_Wrapper.m_Shifter_ShifterMove;

		public bool enabled => Get().enabled;

		public ShifterActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Shifter;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ShifterActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IShifterActions instance)
		{
			if (instance != null && !m_Wrapper.m_ShifterActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ShifterActionsCallbackInterfaces.Add(instance);
				ShiftM.started += instance.OnShiftM;
				ShiftM.performed += instance.OnShiftM;
				ShiftM.canceled += instance.OnShiftM;
				ToggleMA.started += instance.OnToggleMA;
				ToggleMA.performed += instance.OnToggleMA;
				ToggleMA.canceled += instance.OnToggleMA;
				ShiftDown.started += instance.OnShiftDown;
				ShiftDown.performed += instance.OnShiftDown;
				ShiftDown.canceled += instance.OnShiftDown;
				ShiftUp.started += instance.OnShiftUp;
				ShiftUp.performed += instance.OnShiftUp;
				ShiftUp.canceled += instance.OnShiftUp;
				ShiftPRND.started += instance.OnShiftPRND;
				ShiftPRND.performed += instance.OnShiftPRND;
				ShiftPRND.canceled += instance.OnShiftPRND;
				ShifterMove.started += instance.OnShifterMove;
				ShifterMove.performed += instance.OnShifterMove;
				ShifterMove.canceled += instance.OnShifterMove;
			}
		}

		private void UnregisterCallbacks(IShifterActions instance)
		{
			ShiftM.started -= instance.OnShiftM;
			ShiftM.performed -= instance.OnShiftM;
			ShiftM.canceled -= instance.OnShiftM;
			ToggleMA.started -= instance.OnToggleMA;
			ToggleMA.performed -= instance.OnToggleMA;
			ToggleMA.canceled -= instance.OnToggleMA;
			ShiftDown.started -= instance.OnShiftDown;
			ShiftDown.performed -= instance.OnShiftDown;
			ShiftDown.canceled -= instance.OnShiftDown;
			ShiftUp.started -= instance.OnShiftUp;
			ShiftUp.performed -= instance.OnShiftUp;
			ShiftUp.canceled -= instance.OnShiftUp;
			ShiftPRND.started -= instance.OnShiftPRND;
			ShiftPRND.performed -= instance.OnShiftPRND;
			ShiftPRND.canceled -= instance.OnShiftPRND;
			ShifterMove.started -= instance.OnShifterMove;
			ShifterMove.performed -= instance.OnShifterMove;
			ShifterMove.canceled -= instance.OnShifterMove;
		}

		public void RemoveCallbacks(IShifterActions instance)
		{
			if (m_Wrapper.m_ShifterActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IShifterActions instance)
		{
			foreach (IShifterActions shifterActionsCallbackInterface in m_Wrapper.m_ShifterActionsCallbackInterfaces)
			{
				UnregisterCallbacks(shifterActionsCallbackInterface);
			}
			m_Wrapper.m_ShifterActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct ToggleUiActions
	{
		private GameInputController m_Wrapper;

		public InputAction ToggleUi => m_Wrapper.m_ToggleUi_ToggleUi;

		public bool enabled => Get().enabled;

		public ToggleUiActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_ToggleUi;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(ToggleUiActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IToggleUiActions instance)
		{
			if (instance != null && !m_Wrapper.m_ToggleUiActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_ToggleUiActionsCallbackInterfaces.Add(instance);
				ToggleUi.started += instance.OnToggleUi;
				ToggleUi.performed += instance.OnToggleUi;
				ToggleUi.canceled += instance.OnToggleUi;
			}
		}

		private void UnregisterCallbacks(IToggleUiActions instance)
		{
			ToggleUi.started -= instance.OnToggleUi;
			ToggleUi.performed -= instance.OnToggleUi;
			ToggleUi.canceled -= instance.OnToggleUi;
		}

		public void RemoveCallbacks(IToggleUiActions instance)
		{
			if (m_Wrapper.m_ToggleUiActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IToggleUiActions instance)
		{
			foreach (IToggleUiActions toggleUiActionsCallbackInterface in m_Wrapper.m_ToggleUiActionsCallbackInterfaces)
			{
				UnregisterCallbacks(toggleUiActionsCallbackInterface);
			}
			m_Wrapper.m_ToggleUiActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct DebugActions
	{
		private GameInputController m_Wrapper;

		public InputAction RestoreInput => m_Wrapper.m_Debug_RestoreInput;

		public InputAction SendLogs => m_Wrapper.m_Debug_SendLogs;

		public InputAction SimulateDismissKeyboard => m_Wrapper.m_Debug_SimulateDismissKeyboard;

		public InputAction SimulateSubmitKeyboard => m_Wrapper.m_Debug_SimulateSubmitKeyboard;

		public InputAction togglecasual => m_Wrapper.m_Debug_togglecasual;

		public InputAction StartCursorOnly => m_Wrapper.m_Debug_StartCursorOnly;

		public InputAction StopCursorOnly => m_Wrapper.m_Debug_StopCursorOnly;

		public bool enabled => Get().enabled;

		public DebugActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Debug;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(DebugActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IDebugActions instance)
		{
			if (instance != null && !m_Wrapper.m_DebugActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_DebugActionsCallbackInterfaces.Add(instance);
				RestoreInput.started += instance.OnRestoreInput;
				RestoreInput.performed += instance.OnRestoreInput;
				RestoreInput.canceled += instance.OnRestoreInput;
				SendLogs.started += instance.OnSendLogs;
				SendLogs.performed += instance.OnSendLogs;
				SendLogs.canceled += instance.OnSendLogs;
				SimulateDismissKeyboard.started += instance.OnSimulateDismissKeyboard;
				SimulateDismissKeyboard.performed += instance.OnSimulateDismissKeyboard;
				SimulateDismissKeyboard.canceled += instance.OnSimulateDismissKeyboard;
				SimulateSubmitKeyboard.started += instance.OnSimulateSubmitKeyboard;
				SimulateSubmitKeyboard.performed += instance.OnSimulateSubmitKeyboard;
				SimulateSubmitKeyboard.canceled += instance.OnSimulateSubmitKeyboard;
				togglecasual.started += instance.OnTogglecasual;
				togglecasual.performed += instance.OnTogglecasual;
				togglecasual.canceled += instance.OnTogglecasual;
				StartCursorOnly.started += instance.OnStartCursorOnly;
				StartCursorOnly.performed += instance.OnStartCursorOnly;
				StartCursorOnly.canceled += instance.OnStartCursorOnly;
				StopCursorOnly.started += instance.OnStopCursorOnly;
				StopCursorOnly.performed += instance.OnStopCursorOnly;
				StopCursorOnly.canceled += instance.OnStopCursorOnly;
			}
		}

		private void UnregisterCallbacks(IDebugActions instance)
		{
			RestoreInput.started -= instance.OnRestoreInput;
			RestoreInput.performed -= instance.OnRestoreInput;
			RestoreInput.canceled -= instance.OnRestoreInput;
			SendLogs.started -= instance.OnSendLogs;
			SendLogs.performed -= instance.OnSendLogs;
			SendLogs.canceled -= instance.OnSendLogs;
			SimulateDismissKeyboard.started -= instance.OnSimulateDismissKeyboard;
			SimulateDismissKeyboard.performed -= instance.OnSimulateDismissKeyboard;
			SimulateDismissKeyboard.canceled -= instance.OnSimulateDismissKeyboard;
			SimulateSubmitKeyboard.started -= instance.OnSimulateSubmitKeyboard;
			SimulateSubmitKeyboard.performed -= instance.OnSimulateSubmitKeyboard;
			SimulateSubmitKeyboard.canceled -= instance.OnSimulateSubmitKeyboard;
			togglecasual.started -= instance.OnTogglecasual;
			togglecasual.performed -= instance.OnTogglecasual;
			togglecasual.canceled -= instance.OnTogglecasual;
			StartCursorOnly.started -= instance.OnStartCursorOnly;
			StartCursorOnly.performed -= instance.OnStartCursorOnly;
			StartCursorOnly.canceled -= instance.OnStartCursorOnly;
			StopCursorOnly.started -= instance.OnStopCursorOnly;
			StopCursorOnly.performed -= instance.OnStopCursorOnly;
			StopCursorOnly.canceled -= instance.OnStopCursorOnly;
		}

		public void RemoveCallbacks(IDebugActions instance)
		{
			if (m_Wrapper.m_DebugActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IDebugActions instance)
		{
			foreach (IDebugActions debugActionsCallbackInterface in m_Wrapper.m_DebugActionsCallbackInterfaces)
			{
				UnregisterCallbacks(debugActionsCallbackInterface);
			}
			m_Wrapper.m_DebugActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct MozaHelpActions
	{
		private GameInputController m_Wrapper;

		public InputAction xsouth => m_Wrapper.m_MozaHelp_xsouth;

		public InputAction beast => m_Wrapper.m_MozaHelp_beast;

		public InputAction bwest => m_Wrapper.m_MozaHelp_bwest;

		public InputAction bnorth => m_Wrapper.m_MozaHelp_bnorth;

		public InputAction dright => m_Wrapper.m_MozaHelp_dright;

		public InputAction dleft => m_Wrapper.m_MozaHelp_dleft;

		public InputAction ddown => m_Wrapper.m_MozaHelp_ddown;

		public InputAction dup => m_Wrapper.m_MozaHelp_dup;

		public InputAction menu => m_Wrapper.m_MozaHelp_menu;

		public InputAction home => m_Wrapper.m_MozaHelp_home;

		public InputAction START => m_Wrapper.m_MozaHelp_START;

		public InputAction radio => m_Wrapper.m_MozaHelp_radio;

		public InputAction cam => m_Wrapper.m_MozaHelp_cam;

		public InputAction WN => m_Wrapper.m_MozaHelp_WN;

		public InputAction WIP => m_Wrapper.m_MozaHelp_WIP;

		public InputAction WP => m_Wrapper.m_MozaHelp_WP;

		public InputAction BOX => m_Wrapper.m_MozaHelp_BOX;

		public InputAction WR => m_Wrapper.m_MozaHelp_WR;

		public InputAction FL => m_Wrapper.m_MozaHelp_FL;

		public InputAction PL => m_Wrapper.m_MozaHelp_PL;

		public InputAction S1 => m_Wrapper.m_MozaHelp_S1;

		public InputAction S2 => m_Wrapper.m_MozaHelp_S2;

		public InputAction leftwing => m_Wrapper.m_MozaHelp_leftwing;

		public InputAction rightwing => m_Wrapper.m_MozaHelp_rightwing;

		public InputAction wheel => m_Wrapper.m_MozaHelp_wheel;

		public InputAction throttle => m_Wrapper.m_MozaHelp_throttle;

		public InputAction @break => m_Wrapper.m_MozaHelp_break;

		public InputAction clutch => m_Wrapper.m_MozaHelp_clutch;

		public InputAction gear1 => m_Wrapper.m_MozaHelp_gear1;

		public InputAction gear2 => m_Wrapper.m_MozaHelp_gear2;

		public InputAction gear3 => m_Wrapper.m_MozaHelp_gear3;

		public InputAction gear4 => m_Wrapper.m_MozaHelp_gear4;

		public InputAction gear5 => m_Wrapper.m_MozaHelp_gear5;

		public InputAction gear6 => m_Wrapper.m_MozaHelp_gear6;

		public InputAction gear7 => m_Wrapper.m_MozaHelp_gear7;

		public InputAction gearR => m_Wrapper.m_MozaHelp_gearR;

		public InputAction gearN => m_Wrapper.m_MozaHelp_gearN;

		public InputAction wheelLButton => m_Wrapper.m_MozaHelp_wheelLButton;

		public InputAction wheelRButton => m_Wrapper.m_MozaHelp_wheelRButton;

		public InputAction gearboxButton1 => m_Wrapper.m_MozaHelp_gearboxButton1;

		public InputAction gearboxButton2 => m_Wrapper.m_MozaHelp_gearboxButton2;

		public InputAction gearboxButton3 => m_Wrapper.m_MozaHelp_gearboxButton3;

		public InputAction gearboxButton4 => m_Wrapper.m_MozaHelp_gearboxButton4;

		public InputAction gearup => m_Wrapper.m_MozaHelp_gearup;

		public InputAction geardown => m_Wrapper.m_MozaHelp_geardown;

		public InputAction handbrake => m_Wrapper.m_MozaHelp_handbrake;

		public bool enabled => Get().enabled;

		public MozaHelpActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_MozaHelp;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(MozaHelpActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IMozaHelpActions instance)
		{
			if (instance != null && !m_Wrapper.m_MozaHelpActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_MozaHelpActionsCallbackInterfaces.Add(instance);
				xsouth.started += instance.OnXsouth;
				xsouth.performed += instance.OnXsouth;
				xsouth.canceled += instance.OnXsouth;
				beast.started += instance.OnBeast;
				beast.performed += instance.OnBeast;
				beast.canceled += instance.OnBeast;
				bwest.started += instance.OnBwest;
				bwest.performed += instance.OnBwest;
				bwest.canceled += instance.OnBwest;
				bnorth.started += instance.OnBnorth;
				bnorth.performed += instance.OnBnorth;
				bnorth.canceled += instance.OnBnorth;
				dright.started += instance.OnDright;
				dright.performed += instance.OnDright;
				dright.canceled += instance.OnDright;
				dleft.started += instance.OnDleft;
				dleft.performed += instance.OnDleft;
				dleft.canceled += instance.OnDleft;
				ddown.started += instance.OnDdown;
				ddown.performed += instance.OnDdown;
				ddown.canceled += instance.OnDdown;
				dup.started += instance.OnDup;
				dup.performed += instance.OnDup;
				dup.canceled += instance.OnDup;
				menu.started += instance.OnMenu;
				menu.performed += instance.OnMenu;
				menu.canceled += instance.OnMenu;
				home.started += instance.OnHome;
				home.performed += instance.OnHome;
				home.canceled += instance.OnHome;
				START.started += instance.OnSTART;
				START.performed += instance.OnSTART;
				START.canceled += instance.OnSTART;
				radio.started += instance.OnRadio;
				radio.performed += instance.OnRadio;
				radio.canceled += instance.OnRadio;
				cam.started += instance.OnCam;
				cam.performed += instance.OnCam;
				cam.canceled += instance.OnCam;
				WN.started += instance.OnWN;
				WN.performed += instance.OnWN;
				WN.canceled += instance.OnWN;
				WIP.started += instance.OnWIP;
				WIP.performed += instance.OnWIP;
				WIP.canceled += instance.OnWIP;
				WP.started += instance.OnWP;
				WP.performed += instance.OnWP;
				WP.canceled += instance.OnWP;
				BOX.started += instance.OnBOX;
				BOX.performed += instance.OnBOX;
				BOX.canceled += instance.OnBOX;
				WR.started += instance.OnWR;
				WR.performed += instance.OnWR;
				WR.canceled += instance.OnWR;
				FL.started += instance.OnFL;
				FL.performed += instance.OnFL;
				FL.canceled += instance.OnFL;
				PL.started += instance.OnPL;
				PL.performed += instance.OnPL;
				PL.canceled += instance.OnPL;
				S1.started += instance.OnS1;
				S1.performed += instance.OnS1;
				S1.canceled += instance.OnS1;
				S2.started += instance.OnS2;
				S2.performed += instance.OnS2;
				S2.canceled += instance.OnS2;
				leftwing.started += instance.OnLeftwing;
				leftwing.performed += instance.OnLeftwing;
				leftwing.canceled += instance.OnLeftwing;
				rightwing.started += instance.OnRightwing;
				rightwing.performed += instance.OnRightwing;
				rightwing.canceled += instance.OnRightwing;
				wheel.started += instance.OnWheel;
				wheel.performed += instance.OnWheel;
				wheel.canceled += instance.OnWheel;
				throttle.started += instance.OnThrottle;
				throttle.performed += instance.OnThrottle;
				throttle.canceled += instance.OnThrottle;
				@break.started += instance.OnBreak;
				@break.performed += instance.OnBreak;
				@break.canceled += instance.OnBreak;
				clutch.started += instance.OnClutch;
				clutch.performed += instance.OnClutch;
				clutch.canceled += instance.OnClutch;
				gear1.started += instance.OnGear1;
				gear1.performed += instance.OnGear1;
				gear1.canceled += instance.OnGear1;
				gear2.started += instance.OnGear2;
				gear2.performed += instance.OnGear2;
				gear2.canceled += instance.OnGear2;
				gear3.started += instance.OnGear3;
				gear3.performed += instance.OnGear3;
				gear3.canceled += instance.OnGear3;
				gear4.started += instance.OnGear4;
				gear4.performed += instance.OnGear4;
				gear4.canceled += instance.OnGear4;
				gear5.started += instance.OnGear5;
				gear5.performed += instance.OnGear5;
				gear5.canceled += instance.OnGear5;
				gear6.started += instance.OnGear6;
				gear6.performed += instance.OnGear6;
				gear6.canceled += instance.OnGear6;
				gear7.started += instance.OnGear7;
				gear7.performed += instance.OnGear7;
				gear7.canceled += instance.OnGear7;
				gearR.started += instance.OnGearR;
				gearR.performed += instance.OnGearR;
				gearR.canceled += instance.OnGearR;
				gearN.started += instance.OnGearN;
				gearN.performed += instance.OnGearN;
				gearN.canceled += instance.OnGearN;
				wheelLButton.started += instance.OnWheelLButton;
				wheelLButton.performed += instance.OnWheelLButton;
				wheelLButton.canceled += instance.OnWheelLButton;
				wheelRButton.started += instance.OnWheelRButton;
				wheelRButton.performed += instance.OnWheelRButton;
				wheelRButton.canceled += instance.OnWheelRButton;
				gearboxButton1.started += instance.OnGearboxButton1;
				gearboxButton1.performed += instance.OnGearboxButton1;
				gearboxButton1.canceled += instance.OnGearboxButton1;
				gearboxButton2.started += instance.OnGearboxButton2;
				gearboxButton2.performed += instance.OnGearboxButton2;
				gearboxButton2.canceled += instance.OnGearboxButton2;
				gearboxButton3.started += instance.OnGearboxButton3;
				gearboxButton3.performed += instance.OnGearboxButton3;
				gearboxButton3.canceled += instance.OnGearboxButton3;
				gearboxButton4.started += instance.OnGearboxButton4;
				gearboxButton4.performed += instance.OnGearboxButton4;
				gearboxButton4.canceled += instance.OnGearboxButton4;
				gearup.started += instance.OnGearup;
				gearup.performed += instance.OnGearup;
				gearup.canceled += instance.OnGearup;
				geardown.started += instance.OnGeardown;
				geardown.performed += instance.OnGeardown;
				geardown.canceled += instance.OnGeardown;
				handbrake.started += instance.OnHandbrake;
				handbrake.performed += instance.OnHandbrake;
				handbrake.canceled += instance.OnHandbrake;
			}
		}

		private void UnregisterCallbacks(IMozaHelpActions instance)
		{
			xsouth.started -= instance.OnXsouth;
			xsouth.performed -= instance.OnXsouth;
			xsouth.canceled -= instance.OnXsouth;
			beast.started -= instance.OnBeast;
			beast.performed -= instance.OnBeast;
			beast.canceled -= instance.OnBeast;
			bwest.started -= instance.OnBwest;
			bwest.performed -= instance.OnBwest;
			bwest.canceled -= instance.OnBwest;
			bnorth.started -= instance.OnBnorth;
			bnorth.performed -= instance.OnBnorth;
			bnorth.canceled -= instance.OnBnorth;
			dright.started -= instance.OnDright;
			dright.performed -= instance.OnDright;
			dright.canceled -= instance.OnDright;
			dleft.started -= instance.OnDleft;
			dleft.performed -= instance.OnDleft;
			dleft.canceled -= instance.OnDleft;
			ddown.started -= instance.OnDdown;
			ddown.performed -= instance.OnDdown;
			ddown.canceled -= instance.OnDdown;
			dup.started -= instance.OnDup;
			dup.performed -= instance.OnDup;
			dup.canceled -= instance.OnDup;
			menu.started -= instance.OnMenu;
			menu.performed -= instance.OnMenu;
			menu.canceled -= instance.OnMenu;
			home.started -= instance.OnHome;
			home.performed -= instance.OnHome;
			home.canceled -= instance.OnHome;
			START.started -= instance.OnSTART;
			START.performed -= instance.OnSTART;
			START.canceled -= instance.OnSTART;
			radio.started -= instance.OnRadio;
			radio.performed -= instance.OnRadio;
			radio.canceled -= instance.OnRadio;
			cam.started -= instance.OnCam;
			cam.performed -= instance.OnCam;
			cam.canceled -= instance.OnCam;
			WN.started -= instance.OnWN;
			WN.performed -= instance.OnWN;
			WN.canceled -= instance.OnWN;
			WIP.started -= instance.OnWIP;
			WIP.performed -= instance.OnWIP;
			WIP.canceled -= instance.OnWIP;
			WP.started -= instance.OnWP;
			WP.performed -= instance.OnWP;
			WP.canceled -= instance.OnWP;
			BOX.started -= instance.OnBOX;
			BOX.performed -= instance.OnBOX;
			BOX.canceled -= instance.OnBOX;
			WR.started -= instance.OnWR;
			WR.performed -= instance.OnWR;
			WR.canceled -= instance.OnWR;
			FL.started -= instance.OnFL;
			FL.performed -= instance.OnFL;
			FL.canceled -= instance.OnFL;
			PL.started -= instance.OnPL;
			PL.performed -= instance.OnPL;
			PL.canceled -= instance.OnPL;
			S1.started -= instance.OnS1;
			S1.performed -= instance.OnS1;
			S1.canceled -= instance.OnS1;
			S2.started -= instance.OnS2;
			S2.performed -= instance.OnS2;
			S2.canceled -= instance.OnS2;
			leftwing.started -= instance.OnLeftwing;
			leftwing.performed -= instance.OnLeftwing;
			leftwing.canceled -= instance.OnLeftwing;
			rightwing.started -= instance.OnRightwing;
			rightwing.performed -= instance.OnRightwing;
			rightwing.canceled -= instance.OnRightwing;
			wheel.started -= instance.OnWheel;
			wheel.performed -= instance.OnWheel;
			wheel.canceled -= instance.OnWheel;
			throttle.started -= instance.OnThrottle;
			throttle.performed -= instance.OnThrottle;
			throttle.canceled -= instance.OnThrottle;
			@break.started -= instance.OnBreak;
			@break.performed -= instance.OnBreak;
			@break.canceled -= instance.OnBreak;
			clutch.started -= instance.OnClutch;
			clutch.performed -= instance.OnClutch;
			clutch.canceled -= instance.OnClutch;
			gear1.started -= instance.OnGear1;
			gear1.performed -= instance.OnGear1;
			gear1.canceled -= instance.OnGear1;
			gear2.started -= instance.OnGear2;
			gear2.performed -= instance.OnGear2;
			gear2.canceled -= instance.OnGear2;
			gear3.started -= instance.OnGear3;
			gear3.performed -= instance.OnGear3;
			gear3.canceled -= instance.OnGear3;
			gear4.started -= instance.OnGear4;
			gear4.performed -= instance.OnGear4;
			gear4.canceled -= instance.OnGear4;
			gear5.started -= instance.OnGear5;
			gear5.performed -= instance.OnGear5;
			gear5.canceled -= instance.OnGear5;
			gear6.started -= instance.OnGear6;
			gear6.performed -= instance.OnGear6;
			gear6.canceled -= instance.OnGear6;
			gear7.started -= instance.OnGear7;
			gear7.performed -= instance.OnGear7;
			gear7.canceled -= instance.OnGear7;
			gearR.started -= instance.OnGearR;
			gearR.performed -= instance.OnGearR;
			gearR.canceled -= instance.OnGearR;
			gearN.started -= instance.OnGearN;
			gearN.performed -= instance.OnGearN;
			gearN.canceled -= instance.OnGearN;
			wheelLButton.started -= instance.OnWheelLButton;
			wheelLButton.performed -= instance.OnWheelLButton;
			wheelLButton.canceled -= instance.OnWheelLButton;
			wheelRButton.started -= instance.OnWheelRButton;
			wheelRButton.performed -= instance.OnWheelRButton;
			wheelRButton.canceled -= instance.OnWheelRButton;
			gearboxButton1.started -= instance.OnGearboxButton1;
			gearboxButton1.performed -= instance.OnGearboxButton1;
			gearboxButton1.canceled -= instance.OnGearboxButton1;
			gearboxButton2.started -= instance.OnGearboxButton2;
			gearboxButton2.performed -= instance.OnGearboxButton2;
			gearboxButton2.canceled -= instance.OnGearboxButton2;
			gearboxButton3.started -= instance.OnGearboxButton3;
			gearboxButton3.performed -= instance.OnGearboxButton3;
			gearboxButton3.canceled -= instance.OnGearboxButton3;
			gearboxButton4.started -= instance.OnGearboxButton4;
			gearboxButton4.performed -= instance.OnGearboxButton4;
			gearboxButton4.canceled -= instance.OnGearboxButton4;
			gearup.started -= instance.OnGearup;
			gearup.performed -= instance.OnGearup;
			gearup.canceled -= instance.OnGearup;
			geardown.started -= instance.OnGeardown;
			geardown.performed -= instance.OnGeardown;
			geardown.canceled -= instance.OnGeardown;
			handbrake.started -= instance.OnHandbrake;
			handbrake.performed -= instance.OnHandbrake;
			handbrake.canceled -= instance.OnHandbrake;
		}

		public void RemoveCallbacks(IMozaHelpActions instance)
		{
			if (m_Wrapper.m_MozaHelpActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IMozaHelpActions instance)
		{
			foreach (IMozaHelpActions mozaHelpActionsCallbackInterface in m_Wrapper.m_MozaHelpActionsCallbackInterfaces)
			{
				UnregisterCallbacks(mozaHelpActionsCallbackInterface);
			}
			m_Wrapper.m_MozaHelpActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct PersonActions
	{
		private GameInputController m_Wrapper;

		public InputAction Run => m_Wrapper.m_Person_Run;

		public InputAction Crouch => m_Wrapper.m_Person_Crouch;

		public InputAction Jump => m_Wrapper.m_Person_Jump;

		public InputAction WalkRunToggle => m_Wrapper.m_Person_WalkRunToggle;

		public bool enabled => Get().enabled;

		public PersonActions(GameInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Person;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(PersonActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IPersonActions instance)
		{
			if (instance != null && !m_Wrapper.m_PersonActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_PersonActionsCallbackInterfaces.Add(instance);
				Run.started += instance.OnRun;
				Run.performed += instance.OnRun;
				Run.canceled += instance.OnRun;
				Crouch.started += instance.OnCrouch;
				Crouch.performed += instance.OnCrouch;
				Crouch.canceled += instance.OnCrouch;
				Jump.started += instance.OnJump;
				Jump.performed += instance.OnJump;
				Jump.canceled += instance.OnJump;
				WalkRunToggle.started += instance.OnWalkRunToggle;
				WalkRunToggle.performed += instance.OnWalkRunToggle;
				WalkRunToggle.canceled += instance.OnWalkRunToggle;
			}
		}

		private void UnregisterCallbacks(IPersonActions instance)
		{
			Run.started -= instance.OnRun;
			Run.performed -= instance.OnRun;
			Run.canceled -= instance.OnRun;
			Crouch.started -= instance.OnCrouch;
			Crouch.performed -= instance.OnCrouch;
			Crouch.canceled -= instance.OnCrouch;
			Jump.started -= instance.OnJump;
			Jump.performed -= instance.OnJump;
			Jump.canceled -= instance.OnJump;
			WalkRunToggle.started -= instance.OnWalkRunToggle;
			WalkRunToggle.performed -= instance.OnWalkRunToggle;
			WalkRunToggle.canceled -= instance.OnWalkRunToggle;
		}

		public void RemoveCallbacks(IPersonActions instance)
		{
			if (m_Wrapper.m_PersonActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IPersonActions instance)
		{
			foreach (IPersonActions personActionsCallbackInterface in m_Wrapper.m_PersonActionsCallbackInterfaces)
			{
				UnregisterCallbacks(personActionsCallbackInterface);
			}
			m_Wrapper.m_PersonActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public interface IUIActions
	{
		void OnNavigate(InputAction.CallbackContext context);

		void OnGoBack(InputAction.CallbackContext context);
	}

	public interface IUIAcceptCancelActions
	{
		void OnSubmit(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);
	}

	public interface IAcceptCancelActions
	{
		void OnSubmit(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);
	}

	public interface IUiPointerActions
	{
		void OnPoint(InputAction.CallbackContext context);

		void OnCursorMove(InputAction.CallbackContext context);

		void OnClick(InputAction.CallbackContext context);

		void OnRightClick(InputAction.CallbackContext context);

		void OnMiddleClick(InputAction.CallbackContext context);

		void OnScrollWheel(InputAction.CallbackContext context);

		void OnTrackedDevicePosition(InputAction.CallbackContext context);

		void OnTrackedDeviceOrientation(InputAction.CallbackContext context);

		void OnPointerDelta(InputAction.CallbackContext context);
	}

	public interface IUiNextPreviousActions
	{
		void OnNext(InputAction.CallbackContext context);

		void OnPrevious(InputAction.CallbackContext context);
	}

	public interface IUiThrottleActions
	{
		void OnThrottle(InputAction.CallbackContext context);
	}

	public interface IMenuActions
	{
		void OnMenu(InputAction.CallbackContext context);
	}

	public interface IShowMapActions
	{
		void OnMap(InputAction.CallbackContext context);
	}

	public interface IMapUiActions
	{
		void OnHideShow(InputAction.CallbackContext context);

		void OnHideShowAll(InputAction.CallbackContext context);

		void OnExpand(InputAction.CallbackContext context);

		void OnCloseMap(InputAction.CallbackContext context);

		void OnMark(InputAction.CallbackContext context);

		void OnShowMarkDetails(InputAction.CallbackContext context);
	}

	public interface IMapMoveActions
	{
		void OnMove(InputAction.CallbackContext context);

		void OnMouseDragMove(InputAction.CallbackContext context);

		void OnMouseDelta(InputAction.CallbackContext context);
	}

	public interface IDroneActions
	{
		void OnSpeedToggle(InputAction.CallbackContext context);

		void OnTilt(InputAction.CallbackContext context);

		void OnHideUi(InputAction.CallbackContext context);

		void OnCenter(InputAction.CallbackContext context);

		void OnBack(InputAction.CallbackContext context);

		void OnOpenChat(InputAction.CallbackContext context);
	}

	public interface IDroneCameraActions
	{
		void OnReset(InputAction.CallbackContext context);

		void OnSnapshot(InputAction.CallbackContext context);
	}

	public interface IPhotoActions
	{
		void OnHelp(InputAction.CallbackContext context);

		void OnToggleFocus(InputAction.CallbackContext context);

		void OnToggles(InputAction.CallbackContext context);

		void OnNavigateSettings(InputAction.CallbackContext context);

		void OnColor(InputAction.CallbackContext context);

		void OnOptica(InputAction.CallbackContext context);

		void OnCamera(InputAction.CallbackContext context);

		void OnResetFilter(InputAction.CallbackContext context);
	}

	public interface IPaintBaseActions
	{
		void OnBlockCam(InputAction.CallbackContext context);

		void OnToggleUi(InputAction.CallbackContext context);

		void OnDeleteAll(InputAction.CallbackContext context);

		void OnDeleteSelected(InputAction.CallbackContext context);
	}

	public interface IPaintFavoritesToggleActions
	{
		void OnFavoritesToggle(InputAction.CallbackContext context);
	}

	public interface IPaintApplyCancelActions
	{
		void OnCancelChange(InputAction.CallbackContext context);
	}

	public interface IPaintMultiSelectActions
	{
		void OnMultiSelect(InputAction.CallbackContext context);
	}

	public interface IPaintSelectNewActions
	{
		void OnSelectNew(InputAction.CallbackContext context);
	}

	public interface IPaintPlatesActions
	{
		void OnNavigatePlateType(InputAction.CallbackContext context);

		void OnFrontToggle(InputAction.CallbackContext context);

		void OnBackToggle(InputAction.CallbackContext context);
	}

	public interface IPaintSelectSetActions
	{
		void OnSelectSetVinyl(InputAction.CallbackContext context);
	}

	public interface IPaintCopyActions
	{
		void OnCopy(InputAction.CallbackContext context);
	}

	public interface IPaintPlateTypesActions
	{
		void OnEnterPlateTypes(InputAction.CallbackContext context);
	}

	public interface IPaintFavoriteActions
	{
		void OnFavorite(InputAction.CallbackContext context);
	}

	public interface IPaintSelectionMadeActions
	{
		void OnLayerUpDown(InputAction.CallbackContext context);

		void OnMove(InputAction.CallbackContext context);

		void OnSetSize(InputAction.CallbackContext context);

		void OnSetColor(InputAction.CallbackContext context);
	}

	public interface IPaintColorActions
	{
		void OnColorToggle(InputAction.CallbackContext context);

		void OnTintToggle(InputAction.CallbackContext context);

		void OnAlphaValue(InputAction.CallbackContext context);

		void OnColorMove(InputAction.CallbackContext context);

		void OnColorCode(InputAction.CallbackContext context);
	}

	public interface IPaintSizeActions
	{
		void OnMirror(InputAction.CallbackContext context);

		void OnTurn(InputAction.CallbackContext context);

		void OnSizeMove(InputAction.CallbackContext context);

		void OnSetSize(InputAction.CallbackContext context);
	}

	public interface IPaintTextActions
	{
		void OnFont(InputAction.CallbackContext context);

		void OnBold(InputAction.CallbackContext context);

		void OnItalics(InputAction.CallbackContext context);

		void OnEnterText(InputAction.CallbackContext context);
	}

	public interface IPaintEditTextActions
	{
		void OnEditTextStyle(InputAction.CallbackContext context);
	}

	public interface ICameraActions
	{
		void OnLook(InputAction.CallbackContext context);

		void OnLookFree(InputAction.CallbackContext context);
	}

	public interface IMoveActions
	{
		void OnMove(InputAction.CallbackContext context);

		void OnRotate(InputAction.CallbackContext context);

		void OnHeight(InputAction.CallbackContext context);
	}

	public interface IZoomMouseActions
	{
		void OnZoom(InputAction.CallbackContext context);
	}

	public interface IZoomGamepadActions
	{
		void OnZoom(InputAction.CallbackContext context);
	}

	public interface IZoomMozaActions
	{
		void OnZoom(InputAction.CallbackContext context);
	}

	public interface ICameraCarActions
	{
		void OnCameraLookLeft(InputAction.CallbackContext context);

		void OnCameraLookRight(InputAction.CallbackContext context);

		void OnCameraLookBack(InputAction.CallbackContext context);
	}

	public interface IAimReticleActions
	{
		void OnAimAt(InputAction.CallbackContext context);
	}

	public interface IChangeCameraActions
	{
		void OnChangeCamera(InputAction.CallbackContext context);
	}

	public interface ICarRacingActions
	{
		void OnPhotoMode(InputAction.CallbackContext context);

		void OnTimer(InputAction.CallbackContext context);
	}

	public interface ICarDragActions
	{
		void OnBurnout(InputAction.CallbackContext context);

		void OnKeepLane(InputAction.CallbackContext context);
	}

	public interface IBackToCarActions
	{
		void OnBackToCar(InputAction.CallbackContext context);
	}

	public interface IToRoadActions
	{
		void OnToRoad(InputAction.CallbackContext context);
	}

	public interface IToRoadSteeringWheelActions
	{
		void OnToRoad(InputAction.CallbackContext context);
	}

	public interface IHeadlightsActions
	{
		void OnHeadlights(InputAction.CallbackContext context);
	}

	public interface IDoorActions
	{
		void OnOpenDoor(InputAction.CallbackContext context);
	}

	public interface IFreeRaceActions
	{
		void OnRestart(InputAction.CallbackContext context);

		void OnCancelRace(InputAction.CallbackContext context);
	}

	public interface IPhoneActions
	{
		void OnPhone(InputAction.CallbackContext context);
	}

	public interface IVoiceActions
	{
		void OnVoiceChat(InputAction.CallbackContext context);

		void OnChat(InputAction.CallbackContext context);
	}

	public interface IChatGamepadActions
	{
		void OnChat(InputAction.CallbackContext context);
	}

	public interface ICircleMenuActions
	{
		void OnCircleMenuMove(InputAction.CallbackContext context);

		void OnCircleMenuClose(InputAction.CallbackContext context);

		void OnCircleMenuCloseCam(InputAction.CallbackContext context);

		void OnCircleMenuCloseAnim(InputAction.CallbackContext context);

		void OnCircleMenuScroll(InputAction.CallbackContext context);
	}

	public interface IOpenCircleMenuActions
	{
		void OnOpenCircleMenu(InputAction.CallbackContext context);

		void OnOpenCircleMenuCam(InputAction.CallbackContext context);
	}

	public interface ICarBaseActions
	{
		void OnSteering(InputAction.CallbackContext context);

		void OnSteeringMouse(InputAction.CallbackContext context);

		void OnSetGearVisible(InputAction.CallbackContext context);

		void OnThrottle(InputAction.CallbackContext context);

		void OnBrake(InputAction.CallbackContext context);

		void OnClutch(InputAction.CallbackContext context);

		void OnHandbrake(InputAction.CallbackContext context);

		void OnHandbrakeHold(InputAction.CallbackContext context);

		void OnStartEngine(InputAction.CallbackContext context);

		void OnHorn(InputAction.CallbackContext context);
	}

	public interface ISteeringWheelCarActions
	{
		void OnGear1(InputAction.CallbackContext context);

		void OnGear2(InputAction.CallbackContext context);

		void OnGear3(InputAction.CallbackContext context);

		void OnGear4(InputAction.CallbackContext context);

		void OnGear5(InputAction.CallbackContext context);

		void OnGear6(InputAction.CallbackContext context);

		void OnGear7(InputAction.CallbackContext context);

		void OnGearR(InputAction.CallbackContext context);

		void OnGearN(InputAction.CallbackContext context);

		void OnGearUp(InputAction.CallbackContext context);

		void OnGearAMToggle(InputAction.CallbackContext context);

		void OnGearDown(InputAction.CallbackContext context);

		void OnLeftSignal(InputAction.CallbackContext context);

		void OnRightSignal(InputAction.CallbackContext context);

		void OnEmergency(InputAction.CallbackContext context);
	}

	public interface IShifterActions
	{
		void OnShiftM(InputAction.CallbackContext context);

		void OnToggleMA(InputAction.CallbackContext context);

		void OnShiftDown(InputAction.CallbackContext context);

		void OnShiftUp(InputAction.CallbackContext context);

		void OnShiftPRND(InputAction.CallbackContext context);

		void OnShifterMove(InputAction.CallbackContext context);
	}

	public interface IToggleUiActions
	{
		void OnToggleUi(InputAction.CallbackContext context);
	}

	public interface IDebugActions
	{
		void OnRestoreInput(InputAction.CallbackContext context);

		void OnSendLogs(InputAction.CallbackContext context);

		void OnSimulateDismissKeyboard(InputAction.CallbackContext context);

		void OnSimulateSubmitKeyboard(InputAction.CallbackContext context);

		void OnTogglecasual(InputAction.CallbackContext context);

		void OnStartCursorOnly(InputAction.CallbackContext context);

		void OnStopCursorOnly(InputAction.CallbackContext context);
	}

	public interface IMozaHelpActions
	{
		void OnXsouth(InputAction.CallbackContext context);

		void OnBeast(InputAction.CallbackContext context);

		void OnBwest(InputAction.CallbackContext context);

		void OnBnorth(InputAction.CallbackContext context);

		void OnDright(InputAction.CallbackContext context);

		void OnDleft(InputAction.CallbackContext context);

		void OnDdown(InputAction.CallbackContext context);

		void OnDup(InputAction.CallbackContext context);

		void OnMenu(InputAction.CallbackContext context);

		void OnHome(InputAction.CallbackContext context);

		void OnSTART(InputAction.CallbackContext context);

		void OnRadio(InputAction.CallbackContext context);

		void OnCam(InputAction.CallbackContext context);

		void OnWN(InputAction.CallbackContext context);

		void OnWIP(InputAction.CallbackContext context);

		void OnWP(InputAction.CallbackContext context);

		void OnBOX(InputAction.CallbackContext context);

		void OnWR(InputAction.CallbackContext context);

		void OnFL(InputAction.CallbackContext context);

		void OnPL(InputAction.CallbackContext context);

		void OnS1(InputAction.CallbackContext context);

		void OnS2(InputAction.CallbackContext context);

		void OnLeftwing(InputAction.CallbackContext context);

		void OnRightwing(InputAction.CallbackContext context);

		void OnWheel(InputAction.CallbackContext context);

		void OnThrottle(InputAction.CallbackContext context);

		void OnBreak(InputAction.CallbackContext context);

		void OnClutch(InputAction.CallbackContext context);

		void OnGear1(InputAction.CallbackContext context);

		void OnGear2(InputAction.CallbackContext context);

		void OnGear3(InputAction.CallbackContext context);

		void OnGear4(InputAction.CallbackContext context);

		void OnGear5(InputAction.CallbackContext context);

		void OnGear6(InputAction.CallbackContext context);

		void OnGear7(InputAction.CallbackContext context);

		void OnGearR(InputAction.CallbackContext context);

		void OnGearN(InputAction.CallbackContext context);

		void OnWheelLButton(InputAction.CallbackContext context);

		void OnWheelRButton(InputAction.CallbackContext context);

		void OnGearboxButton1(InputAction.CallbackContext context);

		void OnGearboxButton2(InputAction.CallbackContext context);

		void OnGearboxButton3(InputAction.CallbackContext context);

		void OnGearboxButton4(InputAction.CallbackContext context);

		void OnGearup(InputAction.CallbackContext context);

		void OnGeardown(InputAction.CallbackContext context);

		void OnHandbrake(InputAction.CallbackContext context);
	}

	public interface IPersonActions
	{
		void OnRun(InputAction.CallbackContext context);

		void OnCrouch(InputAction.CallbackContext context);

		void OnJump(InputAction.CallbackContext context);

		void OnWalkRunToggle(InputAction.CallbackContext context);
	}

	private readonly InputActionMap m_UI;

	private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();

	private readonly InputAction m_UI_Navigate;

	private readonly InputAction m_UI_goBack;

	private readonly InputActionMap m_UIAcceptCancel;

	private List<IUIAcceptCancelActions> m_UIAcceptCancelActionsCallbackInterfaces = new List<IUIAcceptCancelActions>();

	private readonly InputAction m_UIAcceptCancel_Submit;

	private readonly InputAction m_UIAcceptCancel_Cancel;

	private readonly InputActionMap m_AcceptCancel;

	private List<IAcceptCancelActions> m_AcceptCancelActionsCallbackInterfaces = new List<IAcceptCancelActions>();

	private readonly InputAction m_AcceptCancel_Submit;

	private readonly InputAction m_AcceptCancel_Cancel;

	private readonly InputActionMap m_UiPointer;

	private List<IUiPointerActions> m_UiPointerActionsCallbackInterfaces = new List<IUiPointerActions>();

	private readonly InputAction m_UiPointer_Point;

	private readonly InputAction m_UiPointer_CursorMove;

	private readonly InputAction m_UiPointer_Click;

	private readonly InputAction m_UiPointer_RightClick;

	private readonly InputAction m_UiPointer_MiddleClick;

	private readonly InputAction m_UiPointer_ScrollWheel;

	private readonly InputAction m_UiPointer_TrackedDevicePosition;

	private readonly InputAction m_UiPointer_TrackedDeviceOrientation;

	private readonly InputAction m_UiPointer_PointerDelta;

	private readonly InputActionMap m_UiNextPrevious;

	private List<IUiNextPreviousActions> m_UiNextPreviousActionsCallbackInterfaces = new List<IUiNextPreviousActions>();

	private readonly InputAction m_UiNextPrevious_Next;

	private readonly InputAction m_UiNextPrevious_Previous;

	private readonly InputActionMap m_UiThrottle;

	private List<IUiThrottleActions> m_UiThrottleActionsCallbackInterfaces = new List<IUiThrottleActions>();

	private readonly InputAction m_UiThrottle_Throttle;

	private readonly InputActionMap m_Menu;

	private List<IMenuActions> m_MenuActionsCallbackInterfaces = new List<IMenuActions>();

	private readonly InputAction m_Menu_Menu;

	private readonly InputActionMap m_ShowMap;

	private List<IShowMapActions> m_ShowMapActionsCallbackInterfaces = new List<IShowMapActions>();

	private readonly InputAction m_ShowMap_Map;

	private readonly InputActionMap m_MapUi;

	private List<IMapUiActions> m_MapUiActionsCallbackInterfaces = new List<IMapUiActions>();

	private readonly InputAction m_MapUi_HideShow;

	private readonly InputAction m_MapUi_HideShowAll;

	private readonly InputAction m_MapUi_Expand;

	private readonly InputAction m_MapUi_CloseMap;

	private readonly InputAction m_MapUi_Mark;

	private readonly InputAction m_MapUi_ShowMarkDetails;

	private readonly InputActionMap m_MapMove;

	private List<IMapMoveActions> m_MapMoveActionsCallbackInterfaces = new List<IMapMoveActions>();

	private readonly InputAction m_MapMove_Move;

	private readonly InputAction m_MapMove_MouseDragMove;

	private readonly InputAction m_MapMove_MouseDelta;

	private readonly InputActionMap m_Drone;

	private List<IDroneActions> m_DroneActionsCallbackInterfaces = new List<IDroneActions>();

	private readonly InputAction m_Drone_SpeedToggle;

	private readonly InputAction m_Drone_Tilt;

	private readonly InputAction m_Drone_HideUi;

	private readonly InputAction m_Drone_Center;

	private readonly InputAction m_Drone_Back;

	private readonly InputAction m_Drone_OpenChat;

	private readonly InputActionMap m_DroneCamera;

	private List<IDroneCameraActions> m_DroneCameraActionsCallbackInterfaces = new List<IDroneCameraActions>();

	private readonly InputAction m_DroneCamera_Reset;

	private readonly InputAction m_DroneCamera_Snapshot;

	private readonly InputActionMap m_Photo;

	private List<IPhotoActions> m_PhotoActionsCallbackInterfaces = new List<IPhotoActions>();

	private readonly InputAction m_Photo_Help;

	private readonly InputAction m_Photo_ToggleFocus;

	private readonly InputAction m_Photo_Toggles;

	private readonly InputAction m_Photo_NavigateSettings;

	private readonly InputAction m_Photo_Color;

	private readonly InputAction m_Photo_Optica;

	private readonly InputAction m_Photo_Camera;

	private readonly InputAction m_Photo_ResetFilter;

	private readonly InputActionMap m_PaintBase;

	private List<IPaintBaseActions> m_PaintBaseActionsCallbackInterfaces = new List<IPaintBaseActions>();

	private readonly InputAction m_PaintBase_BlockCam;

	private readonly InputAction m_PaintBase_ToggleUi;

	private readonly InputAction m_PaintBase_DeleteAll;

	private readonly InputAction m_PaintBase_DeleteSelected;

	private readonly InputActionMap m_PaintFavoritesToggle;

	private List<IPaintFavoritesToggleActions> m_PaintFavoritesToggleActionsCallbackInterfaces = new List<IPaintFavoritesToggleActions>();

	private readonly InputAction m_PaintFavoritesToggle_FavoritesToggle;

	private readonly InputActionMap m_PaintApplyCancel;

	private List<IPaintApplyCancelActions> m_PaintApplyCancelActionsCallbackInterfaces = new List<IPaintApplyCancelActions>();

	private readonly InputAction m_PaintApplyCancel_CancelChange;

	private readonly InputActionMap m_PaintMultiSelect;

	private List<IPaintMultiSelectActions> m_PaintMultiSelectActionsCallbackInterfaces = new List<IPaintMultiSelectActions>();

	private readonly InputAction m_PaintMultiSelect_MultiSelect;

	private readonly InputActionMap m_PaintSelectNew;

	private List<IPaintSelectNewActions> m_PaintSelectNewActionsCallbackInterfaces = new List<IPaintSelectNewActions>();

	private readonly InputAction m_PaintSelectNew_SelectNew;

	private readonly InputActionMap m_PaintPlates;

	private List<IPaintPlatesActions> m_PaintPlatesActionsCallbackInterfaces = new List<IPaintPlatesActions>();

	private readonly InputAction m_PaintPlates_NavigatePlateType;

	private readonly InputAction m_PaintPlates_FrontToggle;

	private readonly InputAction m_PaintPlates_BackToggle;

	private readonly InputActionMap m_PaintSelectSet;

	private List<IPaintSelectSetActions> m_PaintSelectSetActionsCallbackInterfaces = new List<IPaintSelectSetActions>();

	private readonly InputAction m_PaintSelectSet_SelectSetVinyl;

	private readonly InputActionMap m_PaintCopy;

	private List<IPaintCopyActions> m_PaintCopyActionsCallbackInterfaces = new List<IPaintCopyActions>();

	private readonly InputAction m_PaintCopy_Copy;

	private readonly InputActionMap m_PaintPlateTypes;

	private List<IPaintPlateTypesActions> m_PaintPlateTypesActionsCallbackInterfaces = new List<IPaintPlateTypesActions>();

	private readonly InputAction m_PaintPlateTypes_EnterPlateTypes;

	private readonly InputActionMap m_PaintFavorite;

	private List<IPaintFavoriteActions> m_PaintFavoriteActionsCallbackInterfaces = new List<IPaintFavoriteActions>();

	private readonly InputAction m_PaintFavorite_Favorite;

	private readonly InputActionMap m_PaintSelectionMade;

	private List<IPaintSelectionMadeActions> m_PaintSelectionMadeActionsCallbackInterfaces = new List<IPaintSelectionMadeActions>();

	private readonly InputAction m_PaintSelectionMade_LayerUpDown;

	private readonly InputAction m_PaintSelectionMade_Move;

	private readonly InputAction m_PaintSelectionMade_SetSize;

	private readonly InputAction m_PaintSelectionMade_SetColor;

	private readonly InputActionMap m_PaintColor;

	private List<IPaintColorActions> m_PaintColorActionsCallbackInterfaces = new List<IPaintColorActions>();

	private readonly InputAction m_PaintColor_ColorToggle;

	private readonly InputAction m_PaintColor_TintToggle;

	private readonly InputAction m_PaintColor_AlphaValue;

	private readonly InputAction m_PaintColor_ColorMove;

	private readonly InputAction m_PaintColor_ColorCode;

	private readonly InputActionMap m_PaintSize;

	private List<IPaintSizeActions> m_PaintSizeActionsCallbackInterfaces = new List<IPaintSizeActions>();

	private readonly InputAction m_PaintSize_Mirror;

	private readonly InputAction m_PaintSize_Turn;

	private readonly InputAction m_PaintSize_SizeMove;

	private readonly InputAction m_PaintSize_SetSize;

	private readonly InputActionMap m_PaintText;

	private List<IPaintTextActions> m_PaintTextActionsCallbackInterfaces = new List<IPaintTextActions>();

	private readonly InputAction m_PaintText_Font;

	private readonly InputAction m_PaintText_Bold;

	private readonly InputAction m_PaintText_Italics;

	private readonly InputAction m_PaintText_EnterText;

	private readonly InputActionMap m_PaintEditText;

	private List<IPaintEditTextActions> m_PaintEditTextActionsCallbackInterfaces = new List<IPaintEditTextActions>();

	private readonly InputAction m_PaintEditText_EditTextStyle;

	private readonly InputActionMap m_Camera;

	private List<ICameraActions> m_CameraActionsCallbackInterfaces = new List<ICameraActions>();

	private readonly InputAction m_Camera_Look;

	private readonly InputAction m_Camera_LookFree;

	private readonly InputActionMap m_Move;

	private List<IMoveActions> m_MoveActionsCallbackInterfaces = new List<IMoveActions>();

	private readonly InputAction m_Move_Move;

	private readonly InputAction m_Move_Rotate;

	private readonly InputAction m_Move_Height;

	private readonly InputActionMap m_ZoomMouse;

	private List<IZoomMouseActions> m_ZoomMouseActionsCallbackInterfaces = new List<IZoomMouseActions>();

	private readonly InputAction m_ZoomMouse_Zoom;

	private readonly InputActionMap m_ZoomGamepad;

	private List<IZoomGamepadActions> m_ZoomGamepadActionsCallbackInterfaces = new List<IZoomGamepadActions>();

	private readonly InputAction m_ZoomGamepad_Zoom;

	private readonly InputActionMap m_ZoomMoza;

	private List<IZoomMozaActions> m_ZoomMozaActionsCallbackInterfaces = new List<IZoomMozaActions>();

	private readonly InputAction m_ZoomMoza_Zoom;

	private readonly InputActionMap m_CameraCar;

	private List<ICameraCarActions> m_CameraCarActionsCallbackInterfaces = new List<ICameraCarActions>();

	private readonly InputAction m_CameraCar_CameraLookLeft;

	private readonly InputAction m_CameraCar_CameraLookRight;

	private readonly InputAction m_CameraCar_CameraLookBack;

	private readonly InputActionMap m_AimReticle;

	private List<IAimReticleActions> m_AimReticleActionsCallbackInterfaces = new List<IAimReticleActions>();

	private readonly InputAction m_AimReticle_AimAt;

	private readonly InputActionMap m_ChangeCamera;

	private List<IChangeCameraActions> m_ChangeCameraActionsCallbackInterfaces = new List<IChangeCameraActions>();

	private readonly InputAction m_ChangeCamera_ChangeCamera;

	private readonly InputActionMap m_CarRacing;

	private List<ICarRacingActions> m_CarRacingActionsCallbackInterfaces = new List<ICarRacingActions>();

	private readonly InputAction m_CarRacing_PhotoMode;

	private readonly InputAction m_CarRacing_Timer;

	private readonly InputActionMap m_CarDrag;

	private List<ICarDragActions> m_CarDragActionsCallbackInterfaces = new List<ICarDragActions>();

	private readonly InputAction m_CarDrag_Burnout;

	private readonly InputAction m_CarDrag_KeepLane;

	private readonly InputActionMap m_BackToCar;

	private List<IBackToCarActions> m_BackToCarActionsCallbackInterfaces = new List<IBackToCarActions>();

	private readonly InputAction m_BackToCar_BackToCar;

	private readonly InputActionMap m_ToRoad;

	private List<IToRoadActions> m_ToRoadActionsCallbackInterfaces = new List<IToRoadActions>();

	private readonly InputAction m_ToRoad_ToRoad;

	private readonly InputActionMap m_ToRoadSteeringWheel;

	private List<IToRoadSteeringWheelActions> m_ToRoadSteeringWheelActionsCallbackInterfaces = new List<IToRoadSteeringWheelActions>();

	private readonly InputAction m_ToRoadSteeringWheel_ToRoad;

	private readonly InputActionMap m_Headlights;

	private List<IHeadlightsActions> m_HeadlightsActionsCallbackInterfaces = new List<IHeadlightsActions>();

	private readonly InputAction m_Headlights_Headlights;

	private readonly InputActionMap m_Door;

	private List<IDoorActions> m_DoorActionsCallbackInterfaces = new List<IDoorActions>();

	private readonly InputAction m_Door_OpenDoor;

	private readonly InputActionMap m_FreeRace;

	private List<IFreeRaceActions> m_FreeRaceActionsCallbackInterfaces = new List<IFreeRaceActions>();

	private readonly InputAction m_FreeRace_Restart;

	private readonly InputAction m_FreeRace_CancelRace;

	private readonly InputActionMap m_Phone;

	private List<IPhoneActions> m_PhoneActionsCallbackInterfaces = new List<IPhoneActions>();

	private readonly InputAction m_Phone_Phone;

	private readonly InputActionMap m_Voice;

	private List<IVoiceActions> m_VoiceActionsCallbackInterfaces = new List<IVoiceActions>();

	private readonly InputAction m_Voice_VoiceChat;

	private readonly InputAction m_Voice_Chat;

	private readonly InputActionMap m_ChatGamepad;

	private List<IChatGamepadActions> m_ChatGamepadActionsCallbackInterfaces = new List<IChatGamepadActions>();

	private readonly InputAction m_ChatGamepad_Chat;

	private readonly InputActionMap m_CircleMenu;

	private List<ICircleMenuActions> m_CircleMenuActionsCallbackInterfaces = new List<ICircleMenuActions>();

	private readonly InputAction m_CircleMenu_CircleMenuMove;

	private readonly InputAction m_CircleMenu_CircleMenuClose;

	private readonly InputAction m_CircleMenu_CircleMenuCloseCam;

	private readonly InputAction m_CircleMenu_CircleMenuCloseAnim;

	private readonly InputAction m_CircleMenu_CircleMenuScroll;

	private readonly InputActionMap m_OpenCircleMenu;

	private List<IOpenCircleMenuActions> m_OpenCircleMenuActionsCallbackInterfaces = new List<IOpenCircleMenuActions>();

	private readonly InputAction m_OpenCircleMenu_OpenCircleMenu;

	private readonly InputAction m_OpenCircleMenu_OpenCircleMenuCam;

	private readonly InputActionMap m_CarBase;

	private List<ICarBaseActions> m_CarBaseActionsCallbackInterfaces = new List<ICarBaseActions>();

	private readonly InputAction m_CarBase_Steering;

	private readonly InputAction m_CarBase_SteeringMouse;

	private readonly InputAction m_CarBase_SetGearVisible;

	private readonly InputAction m_CarBase_Throttle;

	private readonly InputAction m_CarBase_Brake;

	private readonly InputAction m_CarBase_Clutch;

	private readonly InputAction m_CarBase_Handbrake;

	private readonly InputAction m_CarBase_HandbrakeHold;

	private readonly InputAction m_CarBase_StartEngine;

	private readonly InputAction m_CarBase_Horn;

	private readonly InputActionMap m_SteeringWheelCar;

	private List<ISteeringWheelCarActions> m_SteeringWheelCarActionsCallbackInterfaces = new List<ISteeringWheelCarActions>();

	private readonly InputAction m_SteeringWheelCar_gear1;

	private readonly InputAction m_SteeringWheelCar_gear2;

	private readonly InputAction m_SteeringWheelCar_gear3;

	private readonly InputAction m_SteeringWheelCar_gear4;

	private readonly InputAction m_SteeringWheelCar_gear5;

	private readonly InputAction m_SteeringWheelCar_gear6;

	private readonly InputAction m_SteeringWheelCar_gear7;

	private readonly InputAction m_SteeringWheelCar_gearR;

	private readonly InputAction m_SteeringWheelCar_gearN;

	private readonly InputAction m_SteeringWheelCar_gearUp;

	private readonly InputAction m_SteeringWheelCar_gearAMToggle;

	private readonly InputAction m_SteeringWheelCar_gearDown;

	private readonly InputAction m_SteeringWheelCar_LeftSignal;

	private readonly InputAction m_SteeringWheelCar_RightSignal;

	private readonly InputAction m_SteeringWheelCar_Emergency;

	private readonly InputActionMap m_Shifter;

	private List<IShifterActions> m_ShifterActionsCallbackInterfaces = new List<IShifterActions>();

	private readonly InputAction m_Shifter_ShiftM;

	private readonly InputAction m_Shifter_ToggleMA;

	private readonly InputAction m_Shifter_ShiftDown;

	private readonly InputAction m_Shifter_ShiftUp;

	private readonly InputAction m_Shifter_ShiftPRND;

	private readonly InputAction m_Shifter_ShifterMove;

	private readonly InputActionMap m_ToggleUi;

	private List<IToggleUiActions> m_ToggleUiActionsCallbackInterfaces = new List<IToggleUiActions>();

	private readonly InputAction m_ToggleUi_ToggleUi;

	private readonly InputActionMap m_Debug;

	private List<IDebugActions> m_DebugActionsCallbackInterfaces = new List<IDebugActions>();

	private readonly InputAction m_Debug_RestoreInput;

	private readonly InputAction m_Debug_SendLogs;

	private readonly InputAction m_Debug_SimulateDismissKeyboard;

	private readonly InputAction m_Debug_SimulateSubmitKeyboard;

	private readonly InputAction m_Debug_togglecasual;

	private readonly InputAction m_Debug_StartCursorOnly;

	private readonly InputAction m_Debug_StopCursorOnly;

	private readonly InputActionMap m_MozaHelp;

	private List<IMozaHelpActions> m_MozaHelpActionsCallbackInterfaces = new List<IMozaHelpActions>();

	private readonly InputAction m_MozaHelp_xsouth;

	private readonly InputAction m_MozaHelp_beast;

	private readonly InputAction m_MozaHelp_bwest;

	private readonly InputAction m_MozaHelp_bnorth;

	private readonly InputAction m_MozaHelp_dright;

	private readonly InputAction m_MozaHelp_dleft;

	private readonly InputAction m_MozaHelp_ddown;

	private readonly InputAction m_MozaHelp_dup;

	private readonly InputAction m_MozaHelp_menu;

	private readonly InputAction m_MozaHelp_home;

	private readonly InputAction m_MozaHelp_START;

	private readonly InputAction m_MozaHelp_radio;

	private readonly InputAction m_MozaHelp_cam;

	private readonly InputAction m_MozaHelp_WN;

	private readonly InputAction m_MozaHelp_WIP;

	private readonly InputAction m_MozaHelp_WP;

	private readonly InputAction m_MozaHelp_BOX;

	private readonly InputAction m_MozaHelp_WR;

	private readonly InputAction m_MozaHelp_FL;

	private readonly InputAction m_MozaHelp_PL;

	private readonly InputAction m_MozaHelp_S1;

	private readonly InputAction m_MozaHelp_S2;

	private readonly InputAction m_MozaHelp_leftwing;

	private readonly InputAction m_MozaHelp_rightwing;

	private readonly InputAction m_MozaHelp_wheel;

	private readonly InputAction m_MozaHelp_throttle;

	private readonly InputAction m_MozaHelp_break;

	private readonly InputAction m_MozaHelp_clutch;

	private readonly InputAction m_MozaHelp_gear1;

	private readonly InputAction m_MozaHelp_gear2;

	private readonly InputAction m_MozaHelp_gear3;

	private readonly InputAction m_MozaHelp_gear4;

	private readonly InputAction m_MozaHelp_gear5;

	private readonly InputAction m_MozaHelp_gear6;

	private readonly InputAction m_MozaHelp_gear7;

	private readonly InputAction m_MozaHelp_gearR;

	private readonly InputAction m_MozaHelp_gearN;

	private readonly InputAction m_MozaHelp_wheelLButton;

	private readonly InputAction m_MozaHelp_wheelRButton;

	private readonly InputAction m_MozaHelp_gearboxButton1;

	private readonly InputAction m_MozaHelp_gearboxButton2;

	private readonly InputAction m_MozaHelp_gearboxButton3;

	private readonly InputAction m_MozaHelp_gearboxButton4;

	private readonly InputAction m_MozaHelp_gearup;

	private readonly InputAction m_MozaHelp_geardown;

	private readonly InputAction m_MozaHelp_handbrake;

	private readonly InputActionMap m_Person;

	private List<IPersonActions> m_PersonActionsCallbackInterfaces = new List<IPersonActions>();

	private readonly InputAction m_Person_Run;

	private readonly InputAction m_Person_Crouch;

	private readonly InputAction m_Person_Jump;

	private readonly InputAction m_Person_WalkRunToggle;

	private int m_DefaultSchemeIndex = -1;

	public InputActionAsset asset { get; }

	public InputBinding? bindingMask
	{
		get
		{
			return asset.bindingMask;
		}
		set
		{
			asset.bindingMask = value;
		}
	}

	public ReadOnlyArray<InputDevice>? devices
	{
		get
		{
			return asset.devices;
		}
		set
		{
			asset.devices = value;
		}
	}

	public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

	public IEnumerable<InputBinding> bindings => asset.bindings;

	public UIActions UI => new UIActions(this);

	public UIAcceptCancelActions UIAcceptCancel => new UIAcceptCancelActions(this);

	public AcceptCancelActions AcceptCancel => new AcceptCancelActions(this);

	public UiPointerActions UiPointer => new UiPointerActions(this);

	public UiNextPreviousActions UiNextPrevious => new UiNextPreviousActions(this);

	public UiThrottleActions UiThrottle => new UiThrottleActions(this);

	public MenuActions Menu => new MenuActions(this);

	public ShowMapActions ShowMap => new ShowMapActions(this);

	public MapUiActions MapUi => new MapUiActions(this);

	public MapMoveActions MapMove => new MapMoveActions(this);

	public DroneActions Drone => new DroneActions(this);

	public DroneCameraActions DroneCamera => new DroneCameraActions(this);

	public PhotoActions Photo => new PhotoActions(this);

	public PaintBaseActions PaintBase => new PaintBaseActions(this);

	public PaintFavoritesToggleActions PaintFavoritesToggle => new PaintFavoritesToggleActions(this);

	public PaintApplyCancelActions PaintApplyCancel => new PaintApplyCancelActions(this);

	public PaintMultiSelectActions PaintMultiSelect => new PaintMultiSelectActions(this);

	public PaintSelectNewActions PaintSelectNew => new PaintSelectNewActions(this);

	public PaintPlatesActions PaintPlates => new PaintPlatesActions(this);

	public PaintSelectSetActions PaintSelectSet => new PaintSelectSetActions(this);

	public PaintCopyActions PaintCopy => new PaintCopyActions(this);

	public PaintPlateTypesActions PaintPlateTypes => new PaintPlateTypesActions(this);

	public PaintFavoriteActions PaintFavorite => new PaintFavoriteActions(this);

	public PaintSelectionMadeActions PaintSelectionMade => new PaintSelectionMadeActions(this);

	public PaintColorActions PaintColor => new PaintColorActions(this);

	public PaintSizeActions PaintSize => new PaintSizeActions(this);

	public PaintTextActions PaintText => new PaintTextActions(this);

	public PaintEditTextActions PaintEditText => new PaintEditTextActions(this);

	public CameraActions Camera => new CameraActions(this);

	public MoveActions Move => new MoveActions(this);

	public ZoomMouseActions ZoomMouse => new ZoomMouseActions(this);

	public ZoomGamepadActions ZoomGamepad => new ZoomGamepadActions(this);

	public ZoomMozaActions ZoomMoza => new ZoomMozaActions(this);

	public CameraCarActions CameraCar => new CameraCarActions(this);

	public AimReticleActions AimReticle => new AimReticleActions(this);

	public ChangeCameraActions ChangeCamera => new ChangeCameraActions(this);

	public CarRacingActions CarRacing => new CarRacingActions(this);

	public CarDragActions CarDrag => new CarDragActions(this);

	public BackToCarActions BackToCar => new BackToCarActions(this);

	public ToRoadActions ToRoad => new ToRoadActions(this);

	public ToRoadSteeringWheelActions ToRoadSteeringWheel => new ToRoadSteeringWheelActions(this);

	public HeadlightsActions Headlights => new HeadlightsActions(this);

	public DoorActions Door => new DoorActions(this);

	public FreeRaceActions FreeRace => new FreeRaceActions(this);

	public PhoneActions Phone => new PhoneActions(this);

	public VoiceActions Voice => new VoiceActions(this);

	public ChatGamepadActions ChatGamepad => new ChatGamepadActions(this);

	public CircleMenuActions CircleMenu => new CircleMenuActions(this);

	public OpenCircleMenuActions OpenCircleMenu => new OpenCircleMenuActions(this);

	public CarBaseActions CarBase => new CarBaseActions(this);

	public SteeringWheelCarActions SteeringWheelCar => new SteeringWheelCarActions(this);

	public ShifterActions Shifter => new ShifterActions(this);

	public ToggleUiActions ToggleUi => new ToggleUiActions(this);

	public DebugActions Debug => new DebugActions(this);

	public MozaHelpActions MozaHelp => new MozaHelpActions(this);

	public PersonActions Person => new PersonActions(this);

	public InputControlScheme DefaultScheme
	{
		get
		{
			if (m_DefaultSchemeIndex == -1)
			{
				m_DefaultSchemeIndex = asset.FindControlSchemeIndex("Default");
			}
			return asset.controlSchemes[m_DefaultSchemeIndex];
		}
	}

	public GameInputController()
	{
		asset = InputActionAsset.FromJson("{\r\n    \"version\": 1,\r\n    \"name\": \"GameInputController\",\r\n    \"maps\": [\r\n        {\r\n            \"name\": \"UI\",\r\n            \"id\": \"06d14b4c-0300-4f2f-afed-8bf9cbc8a31d\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Navigate\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"dfefe9e8-cab0-4b8e-a532-5278234dcf41\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"goBack\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"4d66fccc-dfc2-4e7f-9daa-6386157136f8\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"299b59cd-04f6-4a61-8df3-f35c4da273b5\",\r\n                    \"path\": \"<Gamepad>/dpad\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Gamepad;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Keyboard\",\r\n                    \"id\": \"b9f18c8b-014c-42ae-a504-8309360d6f9e\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"ce3dd64a-50a6-4577-90c4-c5253a802f64\",\r\n                    \"path\": \"<Keyboard>/upArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"385b1a2d-c0f2-441a-be7d-2dbdfc57b61e\",\r\n                    \"path\": \"<Keyboard>/downArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"ae90fb95-2879-4e63-931a-d94e75814b83\",\r\n                    \"path\": \"<Keyboard>/leftArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"98de27c0-6471-4476-a596-2ba66d871322\",\r\n                    \"path\": \"<Keyboard>/rightArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Moza\",\r\n                    \"id\": \"83fe84f4-ea01-41e1-9a70-6f1334c46518\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"50a3d381-96f9-4f01-86db-069729f6b6b0\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"3981b667-6588-424e-9f45-4fe31d84ac54\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button7\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"bab50911-a4cc-472b-a211-a22863d53204\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button8\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"ebbc7b54-1514-4507-8708-89b8a22182eb\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"G25\",\r\n                    \"id\": \"d3716c25-6a58-43e6-ac5e-69425fe9d406\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"35cb4530-9a48-4700-9099-c4a7949a364a\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"11636a99-98e6-415e-8df7-498bc7e9af44\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"fdfb2081-4332-406f-bec3-7c347763c425\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"ee320364-918c-4220-85cf-e7339e8286bb\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"91b8d7d7-8f4c-41b7-9254-57b24f44020c\",\r\n                    \"path\": \"<Mouse>/backButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"goBack\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"UIAcceptCancel\",\r\n            \"id\": \"b3a2cb6f-068d-448d-86a5-0369daadf42c\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Submit\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6ebdb7a8-007e-4af4-941c-eace9ff4dd59\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Cancel\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"3b449b8a-0083-4a0b-bdde-7555776cdafb\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"01fa04a5-de04-4584-b973-f680d6e1825b\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bf5c29ae-2209-48bf-8173-ccba1bf953dc\",\r\n                    \"path\": \"<Keyboard>/escape\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4d6dc081-490b-4f1a-99f6-892ce412063b\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button2\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"04382ff4-ddd7-43f3-b50e-182db4809876\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button19\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6414c65d-9674-4e86-864b-76e593d9d3e7\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"590842e9-a9b8-4d4a-8e7b-7d996f097743\",\r\n                    \"path\": \"<Keyboard>/enter\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5aeff1cb-cd82-4637-9772-25fb2af0b7fd\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/trigger\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1ddedbda-da67-4156-8399-b1adf96f7b94\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button18\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"AcceptCancel\",\r\n            \"id\": \"4a2f1a42-d014-4af9-9ed9-f8e779d14e57\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Submit\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e87a6d5c-a287-4f4c-8e1c-2cc345e0ceb0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Cancel\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"4b9829f4-a60d-4aab-a6f4-fc6ddb1a99c4\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"571b0131-1b18-4aff-a053-c5a06db2d9a1\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fc335c70-739a-4b13-96f6-2a3ce2d79514\",\r\n                    \"path\": \"<Keyboard>/escape\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6d8b2338-5a15-44a2-83f5-88f25e199399\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button2\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1e543346-ac4b-4e7e-9118-b5c1f831cea2\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button19\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"99e34bc4-38f8-46e8-9c83-4ab096a0a4c2\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"53d1285b-c759-4ad7-9eb7-218bd0063034\",\r\n                    \"path\": \"<Keyboard>/enter\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e78ad380-1169-4bf4-98da-8bd722ae2b52\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/trigger\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cccd4caf-6259-4375-b5d1-449545b83c8f\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button18\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"UiPointer\",\r\n            \"id\": \"6c86b5d7-c0eb-4c70-a026-ecb9f4e65583\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Point\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"f720bc7a-16dc-40a1-a51e-1d6fa136f8d0\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"CursorMove\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"9787e019-0cfd-431e-8cc7-94b3ef6609cf\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Click\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"2783f2d9-9af7-46e8-9b86-891782767188\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"RightClick\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"691d57a6-779b-4bfd-8415-19ea6bde4cfd\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"MiddleClick\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"d1844aa8-20b8-49ff-9c72-1669c8250d1b\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ScrollWheel\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"ff9eba3e-2992-4923-8b2b-bd6cb2ffbc06\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"TrackedDevicePosition\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"e5b054ad-0ad4-4fe9-9c14-83675a63ae73\",\r\n                    \"expectedControlType\": \"Vector3\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"TrackedDeviceOrientation\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"ceac41af-f4cc-4d3d-9fa6-1d4f03a82dba\",\r\n                    \"expectedControlType\": \"Quaternion\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"PointerDelta\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"976defa9-0b96-495c-83fa-812089a7a905\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e9971f5f-e4a1-434b-8fa3-364763e63e1b\",\r\n                    \"path\": \"<Mouse>/position\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Point\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"76e4afd4-deb0-4056-982c-1b0ff080d029\",\r\n                    \"path\": \"<Pen>/position\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Point\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c29374f7-29d8-427c-b021-499440754624\",\r\n                    \"path\": \"<Touchscreen>/touch*/position\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Touch;Default\",\r\n                    \"action\": \"Point\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"adb28fa4-3f50-45fb-b6e0-82516dd76602\",\r\n                    \"path\": \"<Gamepad>/leftStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CursorMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0992325f-3bb6-45fd-8ae9-71fe202b1ab4\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3aaad145-a3f4-464e-bd2b-72e87c90a0ca\",\r\n                    \"path\": \"<Touchscreen>/touch*/press\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Touch\",\r\n                    \"action\": \"Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e70d8651-e100-4d0e-b664-301ca0fab628\",\r\n                    \"path\": \"<XRController>/trigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"XR\",\r\n                    \"action\": \"Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9d6ed9ec-0043-44d2-8581-b5e4d50162ce\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"223d6fcb-90c6-4ccf-90f6-1952345e60ec\",\r\n                    \"path\": \"<Mouse>/scroll\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard&Mouse;Default\",\r\n                    \"action\": \"ScrollWheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"856a00f1-4cc0-4bfc-b295-0563a84c387f\",\r\n                    \"path\": \"<Mouse>/rightButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"RightClick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fe3beb1c-14b3-49d1-9cbb-b2a42233030b\",\r\n                    \"path\": \"<Mouse>/middleButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Default\",\r\n                    \"action\": \"MiddleClick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b6b3595d-9f50-4ecc-9791-69cfbeb7a3a6\",\r\n                    \"path\": \"<XRController>/devicePosition\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"XR;Default\",\r\n                    \"action\": \"TrackedDevicePosition\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5d5e6c88-10b5-454f-bf6a-8b785b161382\",\r\n                    \"path\": \"<XRController>/deviceRotation\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"XR;Default\",\r\n                    \"action\": \"TrackedDeviceOrientation\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"08628023-4fba-4ff4-9906-0137123b72ab\",\r\n                    \"path\": \"<Mouse>/delta\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"PointerDelta\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"UiNextPrevious\",\r\n            \"id\": \"cc6d0d1c-2199-4b56-82e4-befd69aff0bf\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Next\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d1b7bf46-b6f5-4611-a390-42672d6e032c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Previous\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"1e4adb7b-9a77-46da-9be4-014d83bd2bce\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3eca9619-f6a1-4551-ad1f-15be2580dd2a\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Next\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"152f6ba2-31ca-4d0b-853d-c27ce15bca3a\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button13\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Next\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5d23ba8d-2202-47c7-a301-20925a9f7761\",\r\n                    \"path\": \"<Keyboard>/e\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Next\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8603ea5b-e2af-4760-b7ab-74129f90ed91\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button5\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Next\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1865be77-7b3c-44c2-812a-12eeb3896745\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Previous\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3b644c96-6b75-4a7b-9e79-ad4388803d9d\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button14\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Previous\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1c59cec5-865d-4139-b548-1f2b2859adea\",\r\n                    \"path\": \"<Keyboard>/q\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Previous\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"75b21cfc-6cfa-446a-bb83-65d74d16c0c0\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button6\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Previous\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"UiThrottle\",\r\n            \"id\": \"a038fd77-a575-4b7c-a94f-a5c90701736f\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Throttle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f49ba53d-32ff-4889-9a72-383e2fa66db2\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"94689961-913b-448d-b701-dd2dd1daa114\",\r\n                    \"path\": \"<Keyboard>/w\",\r\n                    \"interactions\": \"Press(pressPoint=0.001)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cc2a9619-cc15-48bc-9837-641bd91dc797\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"79269b31-c714-446e-b408-24584dde665a\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/z\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"Invert,SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a139462c-3e77-4d15-a6b8-a11ffa8ec5cc\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/z\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Menu\",\r\n            \"id\": \"1ebaddd3-2258-4a2e-9690-36f9670c55b5\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Menu\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e268090e-190d-4b4a-af11-af8b827fa590\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"578ab8eb-1c5f-4822-8e56-3ba9f03706a5\",\r\n                    \"path\": \"<Gamepad>/start\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Menu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"05b5596c-1552-4403-997e-9dca7fb9dc28\",\r\n                    \"path\": \"<Keyboard>/escape\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Menu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"46dfe28f-f333-4d2e-94fa-4b78d63f6093\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button38\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Menu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e12059af-e595-4d79-a444-60be8149d95a\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button4\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Menu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"ShowMap\",\r\n            \"id\": \"70dad4ca-ff1e-42c7-a2f1-a6f2801b5969\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Map\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"cdcc717b-08f5-4d47-8aad-61bc72480150\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"07b2953a-00f2-4108-b985-d64c2852e914\",\r\n                    \"path\": \"<Keyboard>/M\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Map\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"41f752a6-2169-449b-95b2-1d2dca440515\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"Tap,Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Map\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"08239912-09dc-4c1f-b7ea-e1be701335e6\",\r\n                    \"path\": \"VirtualPad/openMap\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Map\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6c0a2398-8711-49e0-95c2-269281f1b89f\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button25\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Map\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1e9346c3-8cff-44f7-b204-dd2cfb06aae8\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button3\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Map\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"MapUi\",\r\n            \"id\": \"675104a2-d53b-43f0-88c0-2ca39bba137d\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"HideShow\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"608a95fc-ed68-4a0c-9e72-2c085e0eff5e\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"HideShowAll\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"0c6ff4b3-834d-49e6-9282-516f8b0b0f26\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Expand\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"67c3e538-a779-4547-a3e8-9621ed27a7ce\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"CloseMap\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"8380ef70-8485-4dd5-9e66-50bb4bd8df76\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Mark\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b717110a-2cf5-4042-96fe-c1e5d8d92f9c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ShowMarkDetails\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b65376ed-b99f-4973-9acb-90fa61cc2a4b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"920213dd-81cc-4c87-9f89-5e403f03a257\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Mark\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"72881375-8a1b-4b03-9472-20f4e965befd\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/trigger\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Mark\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"49f2f081-95bf-4722-b579-c94c03ee4654\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button18\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Mark\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"20633d9e-e4ea-4d85-b261-b91c750ee063\",\r\n                    \"path\": \"<Keyboard>/space\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Mark\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"57de06a3-fcf6-4efd-b2b3-358cf356d0cc\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShowMarkDetails\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"47029fc7-a132-4fad-a7eb-c321f7ef62c2\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button8\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShowMarkDetails\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"597146d7-30ce-4016-9f17-1b49bee61f3c\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"HideShow\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9f41e68b-c453-4251-ab0d-0df1048f6e87\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button3\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"HideShow\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"dfc396fd-d87e-432e-b1f8-f3d67e190fd1\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button17\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"HideShow\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3721c5b3-f513-477b-b8f3-6be6c71eda37\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Hold(duration=0.8)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"HideShowAll\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9aeb95c3-6540-45f1-81ed-0cbe9005a3d2\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button3\",\r\n                    \"interactions\": \"Hold(duration=0.8)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"HideShowAll\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1cafd2c9-066e-40e3-9ce4-b6be3223d96a\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button17\",\r\n                    \"interactions\": \"Hold(duration=0.8)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"HideShowAll\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bd06d87f-4d85-4fa4-8334-66239538bffc\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Expand\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"61e89672-509b-4da1-b925-35adc9b83507\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button4\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Expand\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"236c6b53-ccd0-49a0-9054-d08e780f11a6\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button16\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Expand\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d8c314c7-31f7-47bb-9a31-20bed4099f0d\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CloseMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b8cbf11e-f634-40b5-9fe1-b474a1dfd483\",\r\n                    \"path\": \"<Keyboard>/M\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"CloseMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"64104607-c754-46a2-b49d-7255e541e66b\",\r\n                    \"path\": \"*/{Cancel}\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"CloseMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e253c344-dfe3-409d-ae9c-4f5e9531ef25\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button25\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CloseMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bd4cd8ae-92eb-4163-b436-64d5fdb1ae17\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button2\",\r\n                    \"interactions\": \"Tap,Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CloseMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4b7502d2-6715-4380-a182-ea071243b449\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button3\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CloseMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e8779f96-3f63-493d-a6ad-67ab5bbaf5f7\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button19\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CloseMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"MapMove\",\r\n            \"id\": \"99355849-d2b9-4bef-ae77-a9c639ed45a9\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Move\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"872a12ca-2f5f-432a-a2d4-82173bbf8093\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"MouseDragMove\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"6b97320d-2817-4888-b91d-7a756c562254\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"MouseDelta\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"8e34d4df-d59a-4a9d-be3c-ba2071f6f173\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c4843cd9-9eeb-4244-84ca-3280487706e5\",\r\n                    \"path\": \"<Gamepad>/rightStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"2D Vector\",\r\n                    \"id\": \"02908d83-cbb4-4aab-919d-39c9a81be854\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Up\",\r\n                    \"id\": \"11ead4de-48d0-461b-883c-11665b1db57c\",\r\n                    \"path\": \"<Keyboard>/w\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Down\",\r\n                    \"id\": \"79e598f6-1368-4702-9f87-05de241b3136\",\r\n                    \"path\": \"<Keyboard>/s\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Left\",\r\n                    \"id\": \"57128af0-11ec-47c0-a123-74eacfc77b9a\",\r\n                    \"path\": \"<Keyboard>/a\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Right\",\r\n                    \"id\": \"b85f4fbc-ab7b-471b-ad76-974c3148819a\",\r\n                    \"path\": \"<Keyboard>/d\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Moza\",\r\n                    \"id\": \"2445af4a-1694-4ced-92ba-b4624183d616\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Up\",\r\n                    \"id\": \"9ff25ae2-befb-40c8-9b1a-4534b73583b1\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Down\",\r\n                    \"id\": \"dfbf9663-2c5f-4ca7-8ab0-e5c9b145893e\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button7\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Left\",\r\n                    \"id\": \"55cc5f9c-48f2-4167-8d7c-e0205e3f1a0b\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button8\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Right\",\r\n                    \"id\": \"cea70089-b094-476f-8c68-5be738201f19\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"G25\",\r\n                    \"id\": \"526a5738-d85c-44c0-a718-54945aced779\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Up\",\r\n                    \"id\": \"debdc8c5-8dc2-4ec4-b2fa-7bc20dc79d5f\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Down\",\r\n                    \"id\": \"560f8a9f-1bb7-43af-9b85-6a9e49506b3d\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Left\",\r\n                    \"id\": \"b0696496-b3e6-4a2b-8e6a-a03b92a33e7e\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Right\",\r\n                    \"id\": \"26701a4a-377d-4933-b956-60038cce84fc\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"bf43c34e-49d7-43e6-b27f-cf3331a55880\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"MouseDragMove\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"5072ff80-93bf-41bb-817a-6fd8af5cd64c\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"MouseDragMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"ef192230-9a07-465d-9407-dc883d75a9f1\",\r\n                    \"path\": \"<Mouse>/position\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"MouseDragMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a1d29f9f-505c-4f08-a238-b3448075bd3c\",\r\n                    \"path\": \"<Mouse>/delta\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"MouseDelta\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Drone\",\r\n            \"id\": \"b7b938c4-b942-4db3-a084-ffe431bf8b8f\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"SpeedToggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"67cb912f-aa53-44b7-982c-5d2fd6f51b9d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Tilt\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"1fc9c76f-8794-4f03-9979-9f9717325925\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"HideUi\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"98ee4dfc-5dc8-4f7d-b047-f88090c14d37\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Center\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"4733635d-bc6d-455a-a6a7-d0b463dc4cb7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Back\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"bb609614-6227-459a-9c47-b89594f48b39\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"OpenChat\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"52b169ed-1f42-4d5f-8b70-0f7c0ead5d47\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c2a47653-6a3a-47c8-80e9-75589f2a6385\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Back\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a26a6b7d-c775-4f88-b5eb-3a287d808001\",\r\n                    \"path\": \"<Keyboard>/r\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Back\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"aacddfda-d5f0-49cb-b1e6-3903239f847e\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button2\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Back\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b4c1112f-567c-4f22-981c-9c72b34b0256\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"HideUi\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9d4366aa-1253-4e85-9050-95392cde2807\",\r\n                    \"path\": \"<Keyboard>/f\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"HideUi\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5a87dd05-c74d-4fd7-85c3-9d3762017238\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Center\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5ecbbd97-f2c3-4303-bcde-9be9e90e1780\",\r\n                    \"path\": \"<Keyboard>/c\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Center\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fbfdcb5e-bab1-44b2-b0ca-f1edbbc55e05\",\r\n                    \"path\": \"<Keyboard>/v\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SpeedToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1319bfc1-bb1b-44e6-b01b-8b98bc021a7e\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"SpeedToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"965b39b1-6e2f-488d-8c61-a2b00bab216d\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Tilt\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"ba0d2a94-f01d-4516-b8c7-08d42a58000c\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Tilt\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"6be565d7-3eba-4760-a4a1-fbb406b41f14\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Tilt\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"93091d1d-bd9c-4ab4-adaa-bcde81181cc5\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Tilt\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"ab8ce011-d6f9-4de7-85eb-058ffa0df811\",\r\n                    \"path\": \"<Keyboard>/q\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Tilt\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"9dfde43f-b2d2-442c-9be6-6503b9b12886\",\r\n                    \"path\": \"<Keyboard>/e\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Tilt\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8b9bad38-9765-4764-b77a-17da487dba60\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"OpenChat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cac702bc-0399-4520-993d-ca4a9d6cbdac\",\r\n                    \"path\": \"<Keyboard>/t\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"OpenChat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"DroneCamera\",\r\n            \"id\": \"bb4cb545-32e8-474e-bc7f-a8717810009a\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Reset\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"2147bc8a-a6c0-4245-82f9-a5cb3c39612a\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Snapshot\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"32270989-684b-440a-9045-e4547475973f\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b4e7d674-33cb-4cf2-8b09-1878b6274e3d\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Snapshot\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"24c19008-0cab-461a-bfeb-47bf026aca5a\",\r\n                    \"path\": \"<Keyboard>/space\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Snapshot\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c567ee88-a780-4595-ba2a-f0dd24de1114\",\r\n                    \"path\": \"<Keyboard>/z\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Reset\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"564dcca5-114d-4180-aeac-2ff8ede0b2c6\",\r\n                    \"path\": \"<Gamepad>/rightStickPress\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Reset\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Photo\",\r\n            \"id\": \"91fd6d06-4520-4065-ac71-36ffb20ce2a2\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Help\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c3096269-1474-4068-a9df-8308174acd64\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ToggleFocus\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"197d9b46-13be-405a-b9c3-8c32f5e6aea0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Toggles\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"a11bd2e9-692c-4974-ada1-69e8dcee99ec\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"NavigateSettings\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"93162d0a-bb6e-47c0-9a7e-062863a3e47b\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Color\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"ecfc8761-3ea2-400c-8bdf-af70d2048597\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Optica\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"347c24bb-aaf9-4774-90c0-b88311b7773b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Camera\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"76c0bd2b-0805-4c8a-bd3c-20f36cc95e38\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ResetFilter\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"10673ae1-bf44-4317-88d4-66a8f62eb5c8\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"60f71c0d-32fb-48bd-8e35-9f925da9d45c\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Camera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6ad3c39a-2ad9-4927-87a9-112679286dba\",\r\n                    \"path\": \"<Keyboard>/3\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Camera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f04c7f99-fcf9-4f15-8f74-db042f7c52e3\",\r\n                    \"path\": \"<Keyboard>/c\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"ResetFilter\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"19afcd17-2544-4fc1-ae5b-de1ad68d2a54\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"ResetFilter\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"042c1552-7bdb-42aa-b627-83857f8bd3bb\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Color\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"05324c01-9af5-4134-b11a-dd51478fea1d\",\r\n                    \"path\": \"<Keyboard>/2\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Color\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"79c47cbf-894d-4511-a700-0cff80b09a8a\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR;Default\",\r\n                    \"action\": \"Optica\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"34698524-a88f-472d-b6a4-4418332ff9f8\",\r\n                    \"path\": \"<Keyboard>/1\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Optica\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"29f0e473-479e-495a-99e4-8d4a57fc6b25\",\r\n                    \"path\": \"<Keyboard>/tab\",\r\n                    \"interactions\": \"Press,Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Help\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"908d71f6-473a-4eb4-8fbe-c2cdd8d32e04\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"Tap,Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Help\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d56b5c95-60c3-4679-aafe-24b0ae06acc6\",\r\n                    \"path\": \"<Gamepad>/rightStickPress\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"ToggleFocus\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8896c801-722e-4aeb-b692-b55f45fcda8b\",\r\n                    \"path\": \"VirtualPad/changeCamera\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"ToggleFocus\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"17ca9bed-75ea-4cf7-8651-ae4d39b47a39\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button22\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ToggleFocus\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"02342638-2689-427d-8cc7-327e093ce8b0\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Toggles\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"9af5f64a-1bfd-41e8-85b9-6e2acc9de922\",\r\n                    \"path\": \"<Keyboard>/leftArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Toggles\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"d444f847-5598-4e23-ac31-59cfd4bb1b70\",\r\n                    \"path\": \"<Keyboard>/rightArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Toggles\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"b9043487-4290-4605-ba3e-7f2977566a3a\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Toggles\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"48c74525-7bf2-458f-90ba-d0b4c03697ec\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Toggles\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"6f8f683e-3915-4e22-8477-3d84aeafb0e6\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Toggles\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"5d3884c8-f127-404a-b018-9ddfd5cd07d4\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"NavigateSettings\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"6f421096-aeec-4be0-89ae-7d4a1332ff66\",\r\n                    \"path\": \"<Keyboard>/downArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"NavigateSettings\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"c2922e1f-3f3d-4ec6-ae5f-949b3c0f2ff3\",\r\n                    \"path\": \"<Keyboard>/upArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"NavigateSettings\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"40c5e154-5527-4896-88fd-0b644a93954e\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"NavigateSettings\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"a4dd02a6-c02c-44f7-9f7b-988096943372\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"NavigateSettings\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"94f5bc18-637e-46fc-b5f5-42928a1061e5\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"NavigateSettings\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintBase\",\r\n            \"id\": \"db828ebd-ae10-4e0b-a1ea-257cb1a72159\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"BlockCam\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"3521a3a9-7b58-4eb5-853b-e9ba8bed822c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ToggleUi\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"8480f81d-839f-4f80-bef7-3024a6af6013\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"DeleteAll\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f4ddfcca-dc63-48da-8544-e06b9a6d6a72\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"DeleteSelected\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"a668bd8b-6215-466f-a198-e06ff75b8f30\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0f18188d-858e-4b49-8008-e55020ca870f\",\r\n                    \"path\": \"<Gamepad>/rightStickPress\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"BlockCam\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"991592d7-faf1-42f9-81ca-71fc496066b4\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ToggleUi\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d0c18de0-9d2d-4b91-91da-702bb6ef996a\",\r\n                    \"path\": \"<Gamepad>/start\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"DeleteAll\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3df90fb8-39df-43b8-beb1-38494aa0c01c\",\r\n                    \"path\": \"<Gamepad>/start\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"DeleteSelected\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintFavoritesToggle\",\r\n            \"id\": \"377a3513-177b-48bd-82ca-03dd7b3a9c35\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"FavoritesToggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"3084764f-431c-484d-b939-861cd96e1063\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9619f445-a885-438d-a126-fee099621976\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"FavoritesToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintApplyCancel\",\r\n            \"id\": \"2747251e-1b9d-42ad-a0c9-2ccbe0c9c6be\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"CancelChange\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d52c607c-f276-4de7-9056-10de5aff3144\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"380dd8b3-0931-4447-89cc-88e20fefe7a0\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CancelChange\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5520f182-5211-4e84-9fd1-c30e77f4b897\",\r\n                    \"path\": \"<Keyboard>/escape\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CancelChange\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintMultiSelect\",\r\n            \"id\": \"f067beb2-0157-48ae-97fe-5a620d3fb167\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"MultiSelect\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"85b6b6ba-7226-40a6-8eb9-22af7a75fac9\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"98b61325-17ec-4919-ac09-36f9a6282839\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"MultiSelect\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintSelectNew\",\r\n            \"id\": \"b4ac3e68-6303-4d47-a804-cb9bdeb80830\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"SelectNew\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"1ef56c19-7ac3-4e67-9145-dd49e2129489\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"a835ae3d-4ab2-4b1a-b942-6a67ba5f47fd\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SelectNew\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"a33af951-df96-48b0-8922-a78743ba7ca0\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SelectNew\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"9f4c57b9-788f-4a67-8766-459b481a0cbc\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SelectNew\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"577f216f-7d15-40c4-bcac-a1769dbc5647\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SelectNew\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"93c503be-38c5-453d-8050-40a4653fa525\",\r\n                    \"path\": \"<Keyboard>/leftArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SelectNew\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"6969ba2a-eca3-4fde-bfcd-6989d54d847d\",\r\n                    \"path\": \"<Keyboard>/rightArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SelectNew\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintPlates\",\r\n            \"id\": \"7d116866-8292-4b42-a7d6-9fc4b3e217fc\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"NavigatePlateType\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"b25b26bd-7d6d-431e-b315-4470863144f8\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"FrontToggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f9ec7421-d7b5-44c0-8f67-8bd8e5f3ee79\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"BackToggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e174b8b4-a18b-4ae6-8dc0-650b0cd959db\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"70e7d87e-e30a-416a-9797-3f2e8b1cc89d\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"NavigatePlateType\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"ee459bb4-c1e8-4acb-90ce-bbce68085540\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"NavigatePlateType\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"681a0c4c-37de-4c8f-838d-6af7f01ee587\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"NavigatePlateType\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"86db4871-d452-49c6-ae4c-f92a6a660ec1\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"NavigatePlateType\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"0780400d-6caa-49b8-8ca7-04dee7f8a36a\",\r\n                    \"path\": \"<Keyboard>/leftArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"NavigatePlateType\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"361f38fb-aed6-49bb-be5f-f242aa48c576\",\r\n                    \"path\": \"<Keyboard>/rightArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"NavigatePlateType\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"75db2c9e-2adf-48fe-9864-2a43cf3bcf8a\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"FrontToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4c432067-2e1f-43dc-baf0-f741ef452685\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"BackToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintSelectSet\",\r\n            \"id\": \"8a274f87-08b8-4e9c-a3cc-69d0cd3fe92c\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"SelectSetVinyl\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"3f7ed622-8929-475b-b732-419f3b803cdc\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"6298e106-a1dc-4586-b179-7d2f6d32073a\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SelectSetVinyl\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"76fd3737-5eaa-4b37-b911-cfa1fa6d9079\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SelectSetVinyl\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"ad520e61-f545-47dc-a50e-3982ae2e47f2\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SelectSetVinyl\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"96fd6418-53b5-4773-8524-ad794ba2cfa3\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SelectSetVinyl\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"901cbd67-7f5b-4168-86d0-b28e85225295\",\r\n                    \"path\": \"<Keyboard>/downArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SelectSetVinyl\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"f365c5d6-e258-49be-b4cd-805ac23a207f\",\r\n                    \"path\": \"<Keyboard>/upArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SelectSetVinyl\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintCopy\",\r\n            \"id\": \"8fcc3c6b-6368-4f0d-beac-fc5d49eecc62\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Copy\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e7527e9c-fed1-48ae-b80d-40420f475806\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8a7a7e88-38b3-45da-b47b-36491a3b6ca7\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Copy\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintPlateTypes\",\r\n            \"id\": \"a53eef29-8cf9-468e-820c-0967bd45e471\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"EnterPlateTypes\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"0e460aa6-cf67-4232-b41c-84ffe09a58b4\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1e08fee8-7423-49e9-b8bf-cf7f88e0b88c\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"EnterPlateTypes\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintFavorite\",\r\n            \"id\": \"34e340c7-c14f-46b3-8757-980997884db5\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Favorite\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"884867f1-ed90-4509-a0b9-0053245855b8\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"13df256e-7017-474f-8128-a0b80f6e2517\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Favorite\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintSelectionMade\",\r\n            \"id\": \"4626b384-23bf-4ab0-bbbd-10936e554a50\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"LayerUpDown\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"b3723dcf-bca7-4670-a5e8-6e4702928358\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Move\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"e945a168-71fb-4ce0-8858-911b16359444\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"SetSize\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d46b7e4b-690f-45ad-9279-eb2e7c1ae9c4\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"SetColor\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"2322ce07-c99f-4a93-befc-23216678f838\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"97bd8906-f7ac-4f83-a8c1-dc2033b5c26b\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"LayerUpDown\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"7b15f3ac-542c-435d-8ccc-5c78d8a15b6f\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"LayerUpDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"c1a118ca-935b-4635-b9ac-841b46fa3ee2\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"LayerUpDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"4334b8d8-de53-481c-8603-1f4bbc453871\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"LayerUpDown\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"b3fa4690-95e3-470b-afb4-61d4c0bcd23d\",\r\n                    \"path\": \"<Keyboard>/downArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"LayerUpDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"c0c5fa5b-b4bd-4b47-93a8-2dce65b3fd2a\",\r\n                    \"path\": \"<Keyboard>/upArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"LayerUpDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8ad74662-ea5c-46dc-b6f2-20047e9b2512\",\r\n                    \"path\": \"*/{Primary2DMotion}\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7ba4b023-b92b-43a6-83d8-e18ce57cd7bc\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"Press,Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SetSize\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5fee9773-0b83-4445-9873-e8a49752c1f0\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"Press,Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SetColor\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintColor\",\r\n            \"id\": \"98eced64-86bd-4707-91a1-a21eeae495ca\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"ColorToggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"218dccd1-7266-415f-99fc-62956e9fb713\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"TintToggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fe195f90-be5b-47cc-b861-66c968cfe5b3\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"AlphaValue\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"9e24d535-e8f5-4473-944a-9db2e1c88377\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ColorMove\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"86a4a1d9-e25f-4c3a-99e9-30acaf2a6834\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ColorCode\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"9575b984-8e37-4c72-a228-2d51e9627f68\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4a8ac789-f907-42ef-83ef-236767be41f1\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ColorToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6b3e01f2-844f-4305-8906-7e1c4ad28d58\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"TintToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"787bf9dc-b23c-4d70-9f91-59e97eb6c09c\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"AlphaValue\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"225a04bb-3316-491b-b2bd-beb94ef80ca5\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"AlphaValue\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"af436948-5596-497c-ac5c-77b39357fca0\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"AlphaValue\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"30ec728a-31f0-4554-bd80-d1192aff50fe\",\r\n                    \"path\": \"<Gamepad>/leftStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ColorMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7702ddb7-2fdf-49df-8a56-58fc06200406\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ColorCode\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintSize\",\r\n            \"id\": \"8777ffa1-3de4-4513-bccd-d5ee62cfc383\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Mirror\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"705644a4-0cac-450c-a098-3ca316caed3a\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Turn\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"d4cae4eb-a131-42e6-8549-9b4301b7dcd9\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"SizeMove\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"414d8873-eb49-4f0f-a7b8-7f939310762d\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"SetSize\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"1fccd20c-9651-489d-bad5-e28700b7f1ea\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"62823ac0-3252-4b64-98c6-84d72cbc4389\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Mirror\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"c23eb23d-e879-43b2-88c7-c7187429a6c7\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Turn\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"424eee36-a81b-44a1-b01d-d0f4a534edb1\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Turn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"b814f82e-9bc0-41ac-b01f-7eefa14c4c64\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Turn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"48ef42c1-123b-4470-a4d8-ea72a261e27e\",\r\n                    \"path\": \"<Gamepad>/leftStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SizeMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2326ac40-647b-4b32-9d8f-d48f5c64acdd\",\r\n                    \"path\": \"<Gamepad>/dpad\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SizeMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cf435b48-5804-4b3a-ab3b-a5ab2ed2b776\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SetSize\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintText\",\r\n            \"id\": \"1267273c-6df8-44fd-8637-31f1571dca9e\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Font\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"f62b37dd-d947-48fd-9897-5f292fee4cf5\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Bold\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"7585d6f4-368c-4708-b6e1-40f9de0bfbfd\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Italics\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"ea3fbc37-df1e-4184-a9c2-250c5a9e4b91\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"EnterText\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fd27eebb-3218-4746-ab16-463e92993014\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6692795f-0c0a-47ef-834c-de5f1b54c5f0\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Italics\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b41f98f9-b7d6-426e-a761-5516f211751c\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"EnterText\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a5beab78-7576-461c-aebf-2fb13ce0b4f3\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Bold\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"2baccc3d-9316-4145-8c52-5cd71fb3583c\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Font\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"3ac4f590-7ef1-4832-ace1-d1209f0c4c54\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Font\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"8ea00e60-ec26-4bd3-8ddc-9a7db4226264\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Font\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"PaintEditText\",\r\n            \"id\": \"34bbba4f-4295-4274-94cc-351b2a95a4c8\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"EditTextStyle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"0d9b39dc-1c75-4ce2-98af-824728377d71\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"da306c23-7533-4584-a17e-7826c9ebbc87\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"EditTextStyle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Camera\",\r\n            \"id\": \"dbf60d4e-c795-4f10-9a84-6da23905849b\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Look\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"8bfdd12c-67eb-44b1-960e-b35b75ce5437\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"LookFree\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"e9ca1d2b-5503-4b20-8514-91ab0f4ed3fe\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2f7bdb34-6b8b-49a8-b423-ec99952fad5e\",\r\n                    \"path\": \"<Gamepad>/{Secondary2DMotion}\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Look\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"c0422ae9-4928-423e-93f3-235177c55610\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Look\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"16e2ee12-969c-491d-856e-7c47ef7a73fd\",\r\n                    \"path\": \"<Mouse>/rightButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Look\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"a52f6e44-9d6a-4bb8-8013-ce9f4c9ab4df\",\r\n                    \"path\": \"<Mouse>/delta\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Look\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c0dd10bf-39a9-4eba-8b0a-26ab1106f93c\",\r\n                    \"path\": \"<Gamepad>/{Secondary2DMotion}\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"LookFree\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f631fa15-e719-4922-980f-5ec4c15c14fc\",\r\n                    \"path\": \"<Mouse>/delta\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"LookFree\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Move\",\r\n            \"id\": \"0f27815c-96e3-4777-afa3-686c0b1109d6\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Move\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"9ab96baf-1d2a-44cb-bc93-381b0bc844f5\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Rotate\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"eda8b048-61c1-4ce7-8c8b-3768c67dfe3e\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Height\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"f16b557d-fd39-453c-ad08-8e712fbd4558\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"dca38f77-cc57-47e1-b712-f5c2d511f913\",\r\n                    \"path\": \"<Gamepad>/{Primary2DMotion}\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Moza\",\r\n                    \"id\": \"bbf71316-b5f8-45b3-b16e-0631f2252e80\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"9a371604-aab3-4929-adf4-229b00049877\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/z\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"4f5d811f-58e7-45eb-9577-bdb2f47fc2b2\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/rz\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"aafd6691-f048-479c-81b2-5d2d238158ed\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"c0509049-5543-479e-9f36-651dbba99a95\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"G25\",\r\n                    \"id\": \"8074ff79-e8e3-4324-973f-71552b92746f\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"9e6fe324-f671-4320-a848-468a2a9e7f09\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/z\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Invert,SliderRemap\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"c4cb978a-accd-4e0f-b8db-36a8d9e96982\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/rz\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Invert,SliderRemap\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"bc1505a2-3ed6-4a04-b7f9-4bf304e2af6a\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"85f481f3-d14a-4cd8-afa0-8e54e5d24b14\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"2D Vector\",\r\n                    \"id\": \"99e92558-f1a5-4da4-b7b1-2715cb5181e9\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"6e9f08b4-983f-4b84-81b3-d277b4ad98aa\",\r\n                    \"path\": \"<Keyboard>/w\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"07979b99-24a5-4ad7-87d7-35bad541d9e2\",\r\n                    \"path\": \"<Keyboard>/s\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"9d90e04b-604e-4195-8302-68d2a8c0f697\",\r\n                    \"path\": \"<Keyboard>/a\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"3002f049-b15f-4071-9e96-1996a916bb03\",\r\n                    \"path\": \"<Keyboard>/d\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"a223344f-58c2-44cc-b955-f5b3e6b1c263\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Rotate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"0d200d4a-94e0-4b1e-8dc7-08d56b9ec99e\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button13\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Rotate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"594b59b7-f6f7-4e15-a8ea-4ef5b2e3da12\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button14\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Rotate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"ce9292c1-464a-4524-9aae-6db0619e8771\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Rotate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"870caafb-45fa-4100-81cb-274a5cc63ce5\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Rotate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"1d463ca3-e284-4ca6-b687-60f2e0b1f791\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Rotate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"f116877a-d8cb-4d16-9932-418b1ce4b7ce\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Height\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"9792b0f3-2a97-48a6-9eec-ea726021dd0e\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Height\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"c8886c15-4b97-4252-b653-355d1ff42a13\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Height\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"56051000-443f-4947-b315-d6b0e638779b\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Height\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"a2535f59-3425-454c-b4f0-9208ddbc02b2\",\r\n                    \"path\": \"<Keyboard>/leftCtrl\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Height\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"b143453f-a2c0-4532-92f5-a6330ff9723b\",\r\n                    \"path\": \"<Keyboard>/shift\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Height\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"ZoomMouse\",\r\n            \"id\": \"d87a54bf-af19-4d51-9717-e54228ecd963\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Zoom\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"274464a8-4f92-4a85-ada9-51b78a449bdf\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"Clamp(min=-1,max=1)\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6e5061b5-8845-4379-b07d-3d0267ac3023\",\r\n                    \"path\": \"<Mouse>/scroll/y\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"ZoomGamepad\",\r\n            \"id\": \"00d4e129-95c3-4c4a-a6df-24cc222ac139\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Zoom\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"9d33be8a-70ce-4c28-9684-f83c11f0fcbd\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"Clamp(min=-1,max=1)\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"d7968d75-9918-4885-b146-d66b6470f163\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"6ca88b43-4d78-47b7-aadf-8780980f6c84\",\r\n                    \"path\": \"<Gamepad>/leftTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"6ccb7705-29fa-472d-b246-199cac5060db\",\r\n                    \"path\": \"<Gamepad>/rightTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"ZoomMoza\",\r\n            \"id\": \"26b21266-1bfa-42cb-a64c-4dea815539d5\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Zoom\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"fe3bb074-3fe6-4ed6-beab-9ec65f6a46f7\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"Clamp(min=-1,max=1)\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"Moza\",\r\n                    \"id\": \"e6c58378-ca28-4baa-9dcd-6ac85be50fb6\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Negative\",\r\n                    \"id\": \"19547441-56af-4422-8859-f8f1e312989c\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button13\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Positive\",\r\n                    \"id\": \"d1678971-affc-415b-b4f5-11c7ebef194f\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button14\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Moza\",\r\n                    \"id\": \"2551c066-0e55-4030-a286-35b410247ec8\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Negative\",\r\n                    \"id\": \"5483de7e-9564-4944-b892-26a476a34935\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Positive\",\r\n                    \"id\": \"35b6b401-8cf5-48a6-bbf3-c7332d1b2ce1\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button14\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Zoom\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"CameraCar\",\r\n            \"id\": \"f8668dad-d1c6-45e2-b543-d3687be4d971\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"CameraLookLeft\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e4ae9e55-7a2c-4510-af12-6f247476894f\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"CameraLookRight\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6a7cdb52-4d0d-49e1-9765-f0c34d93274c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"CameraLookBack\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6a0dd2e5-8e13-4c6c-9c80-9c73f53413e9\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a5c1b305-b1c9-464b-b533-7f97ec8c5965\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button8\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookLeft\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"99f18ab9-1505-4e2b-9777-c74ee2eca64e\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/left\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookLeft\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fd180167-86f4-44d1-b0fa-49b925b4baf7\",\r\n                    \"path\": \"<Keyboard>/leftArrow\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookLeft\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"c7e26551-75b5-4451-83e1-129c861fe14a\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"AxisDeadzone(min=0.6)\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"CameraLookLeft\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"b7cef91e-ff9f-41c9-aff4-3cfc97cce836\",\r\n                    \"path\": \"<Gamepad>/rightStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookLeft\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"3b184e7d-8117-407d-8058-d90e118ac55a\",\r\n                    \"path\": \"<Gamepad>/rightStick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookLeft\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1dea0a5f-27ea-4b15-96a8-040e9d9c0105\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button6\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookRight\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c26245d9-07bb-4e09-be4a-34318071b3d2\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/right\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookRight\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6f21b337-c38b-49f0-83e2-7f068211fb9a\",\r\n                    \"path\": \"<Keyboard>/rightArrow\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookRight\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"5362156a-0fa2-42d4-ac54-4db130d1e413\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"AxisDeadzone(min=0.6)\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"CameraLookRight\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"7da80020-59e5-4a25-b518-76e74b323458\",\r\n                    \"path\": \"<Gamepad>/rightStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookRight\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"b79183f4-cfe7-40f1-abf2-aa95f90f1c6a\",\r\n                    \"path\": \"<Gamepad>/rightStick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookRight\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"dea6fcf9-a7b1-430a-ad67-08bb12cdc635\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button7\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookBack\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8c4986f6-0f2c-467a-96e6-b7ebff2509bb\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/down\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookBack\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"be488ec5-b33e-4efe-8cba-1857ef22ee91\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"AxisDeadzone(min=0.6)\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"CameraLookBack\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"b70bd848-c839-4bfc-b523-1e235c05ef26\",\r\n                    \"path\": \"<Gamepad>/rightStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookBack\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"4dedf829-4605-4914-b440-b9f3d981e16b\",\r\n                    \"path\": \"<Gamepad>/rightStick/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookBack\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5d5b9f93-c834-44e3-a436-ebd7cc7e9f6d\",\r\n                    \"path\": \"<Keyboard>/downArrow\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CameraLookBack\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"AimReticle\",\r\n            \"id\": \"0972783a-1b33-46f9-8c77-92d968710ade\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"AimAt\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"cd6efb24-725d-438c-92bc-fc81b06fe3be\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8a3fb6d1-3717-4543-ac06-f16c7abd1642\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"AimAt\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"adb3909a-1add-4824-9373-93cd87911b13\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"AimAt\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d30e7143-cdb9-4ef5-ab60-bf30524498e6\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/trigger\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"AimAt\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6527b941-2b52-4ad4-87c0-515c933282ad\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button18\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"AimAt\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"ChangeCamera\",\r\n            \"id\": \"08bec1b0-cb6c-4ae4-9455-03815d8190be\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"ChangeCamera\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"325d0aed-dd4b-4e3f-94f7-ea26394344b2\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5f31263d-e010-4126-b2f6-017407a2dd93\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button8\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"78e17fad-f70a-48e3-a43b-2d3b1c243796\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button22\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e6342764-a309-4eb2-997a-fe88cc63a01b\",\r\n                    \"path\": \"<Keyboard>/c\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"50f27c88-ff77-4e73-b32a-e4d42d025769\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"CarRacing\",\r\n            \"id\": \"214c41e3-1f2a-4546-ae9c-b173881386fa\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"PhotoMode\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d254c3aa-53d1-418d-b9cd-21ae00c8f905\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Timer\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"10be6956-c832-44ae-ad1e-7f20a729679e\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ea66e552-1664-498b-b8f2-a2559a93c9e6\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"PhotoMode\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"63dc4ed2-d6d7-4f9c-8d7e-75f283287f4c\",\r\n                    \"path\": \"<Keyboard>/upArrow\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"PhotoMode\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f309b515-87db-413b-a2bb-d68cf50a2d97\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button5\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"PhotoMode\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bf136fec-8c4b-4582-8b59-5b792d702f1a\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/up\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"PhotoMode\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"44153929-7c31-4f93-bbef-a52f5c946218\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Timer\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0514844a-e1cc-424b-955d-d03b6048447f\",\r\n                    \"path\": \"<Keyboard>/tab\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Timer\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8e736957-9e38-4c34-920c-fe8e0c17d71b\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button4\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Timer\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"acdd7cc3-0541-444f-bca4-29b9144bef2b\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button16\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Timer\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"CarDrag\",\r\n            \"id\": \"4781d685-b55f-4dc3-82ba-6a777621dfa5\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Burnout\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"092a06a6-45be-4303-aed9-598e93df27d6\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"KeepLane\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"2bf6803d-b34f-425a-b56f-a2b92ee2469b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0d0a0f96-6467-441c-84c7-86f332756903\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Burnout\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"be064a72-e46d-4b12-9598-daba0a1b750f\",\r\n                    \"path\": \"<Keyboard>/x\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Burnout\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"063df154-97fc-4258-8cd2-ac20b0478051\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button4\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Burnout\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d9333e52-de76-4f3e-b6fe-b2747aea1b0f\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button16\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Burnout\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4cceb756-2bef-4e45-8b0d-054b0d0927c6\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"KeepLane\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3fed9013-b26e-4d22-b6e9-3b2eac609fbb\",\r\n                    \"path\": \"<Keyboard>/r\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"KeepLane\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2164fee3-cf77-4a4c-b3b1-dfd3948412df\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button2\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"KeepLane\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9c7b9bd6-74e9-43a4-a9a2-92dd4e29b23d\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button19\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"KeepLane\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"BackToCar\",\r\n            \"id\": \"564c1eb2-1abb-416d-980a-a8eafd8cab3e\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"BackToCar\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d9a2c422-8528-4297-9703-a5d83eb57e9c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c39deaef-b137-475e-8f86-1a94ab0fe116\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"BackToCar\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7e074158-7e12-4a80-a39f-f601b152878b\",\r\n                    \"path\": \"<Keyboard>/f\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"BackToCar\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0d97fe96-b4d9-493b-b110-3fec98ed6f24\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button19\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"BackToCar\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9bff1b43-d932-43ef-a74d-649bb0806ce4\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button19\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"BackToCar\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"ToRoad\",\r\n            \"id\": \"3e4ed0a9-1308-44ab-a517-7f7d7a0a32f5\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"ToRoad\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"86b1c7c5-6355-47cd-a5c4-fb979d3d5895\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"44eec61a-249a-44ac-bd25-526ba80dfac5\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"Hold(duration=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ToRoad\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b9400020-73ab-41af-8d72-ea1a02a28ec0\",\r\n                    \"path\": \"<Keyboard>/x\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ToRoad\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"ToRoadSteeringWheel\",\r\n            \"id\": \"55145fa9-d369-47c1-a30e-577149d86f38\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"ToRoad\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f3d63d5b-da00-4ac9-b94e-d685ad38d0d3\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"20ae3375-6333-4369-a700-dead65da90f6\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button19\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ToRoad\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e53af699-4b94-4f2b-83f1-127043e65e9e\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button2\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ToRoad\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Headlights\",\r\n            \"id\": \"15af9844-f80e-41ef-b06e-e8bf58f6893d\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Headlights\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"433d9d25-5554-4238-8664-9046cc2556a3\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9ab0510b-3631-4a88-9908-747411438295\",\r\n                    \"path\": \"<Keyboard>/l\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e508161e-6046-422a-8524-fb6921323c5d\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"889be609-0bb0-4a35-bc5a-56fef0896bf0\",\r\n                    \"path\": \"VirtualPad/headlights\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"69ff7752-c86e-4559-a759-91f76dc37c88\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button32\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Door\",\r\n            \"id\": \"8b25afbc-ab5d-4acc-99c5-e2f317569083\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"OpenDoor\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b92a84a2-d7fb-44d7-8167-366959e86dda\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"64115e51-e0a7-4272-8bab-c388b79a62c5\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"OpenDoor\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"67335ecd-2adf-41e5-bfee-d17f829beca9\",\r\n                    \"path\": \"<Keyboard>/f\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"OpenDoor\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3127f8ed-8b0d-4ab2-ad5b-aac8186152ef\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button4\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"OpenDoor\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e7c3b2d1-fb26-45c9-b5f6-3445a16866f1\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button16\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"OpenDoor\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"FreeRace\",\r\n            \"id\": \"331011f3-7345-40cd-8171-5edd79a97b40\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Restart\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6c97b760-731a-436f-a2bd-241f457664f6\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"CancelRace\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"be950630-fffd-4679-8dc7-1b1f03640759\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cc3e2178-45b3-4027-b2bb-4f38a55d797b\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Restart\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6f760ba2-c243-43b3-986f-c78d399eb473\",\r\n                    \"path\": \"<Keyboard>/f\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Restart\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bbef1a65-7ba1-455a-9cdf-2d764adf4eaf\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button4\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Restart\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ce931536-2066-4358-9aa1-f02b2597f89d\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button16\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Restart\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"67b25126-5a38-4b9d-aadc-a6e920b85142\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"CancelRace\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f50c9984-97fb-4588-bfee-b822db3bfb74\",\r\n                    \"path\": \"<Keyboard>/upArrow\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CancelRace\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d7c6a3d5-8cb1-4a3b-8cb9-ebc5bba58b22\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button5\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CancelRace\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bb94c82d-eb6e-4b84-bbbe-3998dd8ecb7e\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/up\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CancelRace\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Phone\",\r\n            \"id\": \"8ce53280-9e9c-42bf-b283-bb2acdf00fb1\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Phone\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"1b8d03b5-43e9-4f00-abba-defa23de54df\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8b2c5002-c7ba-4f1a-84ce-2c162e10fee5\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Phone\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5965f1e6-62e6-4028-9408-7af42ae2622d\",\r\n                    \"path\": \"<Mouse>/middleButton\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Phone\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"dfa450e8-7a98-4e96-aef7-f7a2a20d32d3\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button5\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Phone\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"859d17f1-4898-492e-b0fb-7f8b58549eda\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/up\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Phone\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Voice\",\r\n            \"id\": \"29b38ba8-5f49-4812-a07b-d3dbea361702\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"VoiceChat\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"2b091b71-3fbd-4a8e-8c58-d73c06dd0201\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Chat\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"40888338-6d7b-43f7-b4c4-c3b4be3a1bb5\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f374ca17-f0e7-4592-a3eb-8035bd45001e\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"VoiceChat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f711ae14-d819-4209-900a-1bf09ef52978\",\r\n                    \"path\": \"<Keyboard>/v\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"VoiceChat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"20ab0567-9821-448a-98f9-bdce6ecd9758\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button23\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"VoiceChat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"be097600-aca0-48d5-9c60-691692d70017\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"VoiceChat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2aa6ca0f-2e1d-4941-b13e-bdaff6bd6c74\",\r\n                    \"path\": \"<Keyboard>/t\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Chat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"ChatGamepad\",\r\n            \"id\": \"0d3a478c-6872-4692-a8fd-f07e695ebcb7\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Chat\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fc3b0267-2e6a-4737-9091-a909a939e060\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"73e594e7-900c-40a7-80ab-35294946f644\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Chat\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"CircleMenu\",\r\n            \"id\": \"be86b661-0082-4413-81f9-bcb9236487e4\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"CircleMenuMove\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"82d6ed2c-742d-4c76-b977-94f1b2d4cbe9\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"CircleMenuClose\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"8724036e-03c3-4431-8f67-0b453f4d3585\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"CircleMenuCloseCam\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b7e3faeb-2561-48b3-ae18-7415c6ed11f1\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"CircleMenuCloseAnim\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"3e4f5a6b-7c8d-4e9f-a0b1-2c3d4e5f6a71\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"CircleMenuScroll\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"bc10865c-835a-4bc1-bc47-5337bd67f36c\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"861e7f8b-4d7a-45a8-a1da-dc1a15703e05\",\r\n                    \"path\": \"<Gamepad>/rightStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CircleMenuMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"48d43757-bb66-49ee-9d8e-3a0359b76d66\",\r\n                    \"path\": \"<Mouse>/delta\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CircleMenuMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c737140c-0dd6-4b0c-802d-61092a4a688f\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CircleMenuClose\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"88a65b1e-f7a5-45c4-ad6a-38a09d65a6e8\",\r\n                    \"path\": \"<Keyboard>/tab\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CircleMenuClose\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"34aa39d7-dc78-49d9-bd78-d7885e7d5926\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CircleMenuCloseCam\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bd701486-b14e-457c-9e6c-538c038972a5\",\r\n                    \"path\": \"<Keyboard>/c\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CircleMenuCloseCam\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4f5a6b7c-8d9e-4f0a-b1c2-3d4e5f6a7b82\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CircleMenuCloseAnim\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5a6b7c8d-9e0f-4a1b-c2d3-4e5f6a7b8c93\",\r\n                    \"path\": \"<Keyboard>/q\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CircleMenuCloseAnim\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"03b8b2ad-4d34-45c8-b62b-45f5a6e53e9b\",\r\n                    \"path\": \"<Mouse>/scroll/y\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"CircleMenuScroll\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"84ff233d-d6c6-4158-9f74-c3d73f820521\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"CircleMenuScroll\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"10ce2919-15ea-4003-a8b4-a9c48060fa4f\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"CircleMenuScroll\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"cdf05944-1f06-498d-bf3a-8bd210271d1b\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"CircleMenuScroll\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"OpenCircleMenu\",\r\n            \"id\": \"a43b2826-77a4-4fbb-87b3-f8ea9dc6be83\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"OpenCircleMenu\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"4e9562a2-52c1-46d0-bae5-0555a0442f6b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"OpenCircleMenuCam\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e4fcbcab-4796-47e6-9c73-0aeec1e2b946\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8530c839-c79b-48b9-b401-77b3a435f1bc\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"OpenCircleMenu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fa90c043-65a8-4a6a-b835-46fb2ae1c006\",\r\n                    \"path\": \"<Keyboard>/tab\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"OpenCircleMenu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"54ff0379-949e-4439-a70c-941bc35b058e\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"Hold(duration=0.3)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"OpenCircleMenuCam\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0318799d-d36b-4d76-85aa-334c91078ee9\",\r\n                    \"path\": \"<Keyboard>/c\",\r\n                    \"interactions\": \"Hold(duration=0.3)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"OpenCircleMenuCam\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"CarBase\",\r\n            \"id\": \"c688ecce-df88-40e7-a24c-80365b46eecf\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Steering\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"fbe1dafd-5060-4060-a312-54bc5b7e8406\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"SteeringMouse\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"085147cf-d5c7-401d-a071-fd6800943848\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"SetGearVisible\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"240093be-c784-4812-ad87-3518cd5a64bf\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Throttle\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"64264815-bb0e-44bd-bf88-64ba347856de\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Brake\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"b685e9cf-15da-4394-a0e3-9c89942556f6\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Clutch\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"837bed47-b8f6-4cbf-8793-26b083c23af1\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Handbrake\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"9dc2e65f-1cc6-45c3-a845-bf6248323697\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"HandbrakeHold\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"2b8accc0-d057-4270-aac4-ca605281f1f5\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"StartEngine\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e2a19586-e572-4b23-b44b-21bd432cede3\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Horn\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"1db291b7-8524-49a2-b1ad-ee857489fa66\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2dd65f49-a28c-4ce5-9391-f83bfdfe7239\",\r\n                    \"path\": \"<Keyboard>/w\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8b65ccfc-348f-4580-93f6-e1dd59917300\",\r\n                    \"path\": \"<Gamepad>/rightTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"67a7a150-42cf-4550-8606-4cc174c4f51d\",\r\n                    \"path\": \"VirtualPad/throttle\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"20ac63c6-6d5a-4bb2-929b-4d69c8af7b89\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/z\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"Invert,SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"53bdf077-208f-41bf-9b62-388ec0093428\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/z\",\r\n                    \"interactions\": \"SliderPress\",\r\n                    \"processors\": \"SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cf42e4b3-3e4e-43db-b71e-144400db6284\",\r\n                    \"path\": \"<Keyboard>/s\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ae0b214f-6102-4427-8ff7-c5a2c0abe5cb\",\r\n                    \"path\": \"<Gamepad>/leftTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"938a1fd9-a2a1-4cdc-8427-326bbd382428\",\r\n                    \"path\": \"VirtualPad/brake\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9208f4c3-b949-4bee-b7ad-4a5e925e9124\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/rz\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"Invert,SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"28f3435c-2243-4b68-b458-41231760e9c9\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/rz\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bcc64295-f8f3-4b91-b3e8-664c056450b5\",\r\n                    \"path\": \"<Keyboard>/shift\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e821bc74-7f0e-4018-8e68-2d5fc689fe90\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"Press(pressPoint=0.001)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5ea10a78-8744-4600-8a61-cea880d37502\",\r\n                    \"path\": \"VirtualPad/clutch\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5538a5f5-be36-44e7-ae63-ffe0a7ecdafa\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/slider\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"Invert,SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"36fa19f1-c7b3-44a5-bdbd-6ad34f4f7a45\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/slider\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"09a45b86-2cd7-41e7-93f5-f90375794c30\",\r\n                    \"path\": \"<Keyboard>/space\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e7cb4441-1a4c-48e6-87be-b5c46b012a24\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"398c51ed-a54f-4497-b871-4e4b910d9334\",\r\n                    \"path\": \"VirtualPad/handbrake\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"953d50a1-e124-48d2-8f24-35b53d8d44f7\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button2\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bd80bb7d-b50f-4bad-bcdb-2dc6a3ab5490\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button7\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7b93fffa-111e-4b6a-bc0a-ad4b62d4ece9\",\r\n                    \"path\": \"<HID::ZSC SIMAKER-Handbrake>/slider\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99,releasePoint=0.01)\",\r\n                    \"processors\": \"SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5d3dfc74-c227-4fd3-91dc-fb09ba5f1b81\",\r\n                    \"path\": \"<Keyboard>/leftCtrl\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"HandbrakeHold\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e1974111-4c57-4c0d-8add-fde3c664dfe6\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"HandbrakeHold\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b0f9bd73-addb-4edf-83ea-c42a5223d236\",\r\n                    \"path\": \"<Keyboard>/z\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9450aa8e-32d8-458d-8b47-45c1e54f30da\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"Hold,Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a3edfaa9-c95e-49b5-9a07-1a3cd6e22ac1\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button3\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cbeb35a1-d73f-4215-854c-7476ae157c40\",\r\n                    \"path\": \"VirtualPad/starter\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"245d9d0f-ee09-4e6c-bbb1-c35aa47ad9ad\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button36\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c168d0ed-4335-4006-bf7d-7a7aecd706fa\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button17\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"53e91f24-154b-425e-aaaf-8e4146850212\",\r\n                    \"path\": \"<Keyboard>/g\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"440d15cc-cf3a-4ec3-8df1-c163f93a870d\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0ac467d9-20cb-4f7d-860e-063b7494a566\",\r\n                    \"path\": \"VirtualPad/horn\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b9e3fca1-7173-4bbb-9251-49cb07073b30\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/trigger\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8eb82480-fa49-4bdb-bcb9-f89f73345ce7\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button18\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Keyboard\",\r\n                    \"id\": \"f8a16e0f-0ec5-49c9-a0cd-dff525538767\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"875c385b-3d7d-44b2-a64a-871cf669d3b1\",\r\n                    \"path\": \"<Keyboard>/a\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"f4768545-a892-42e6-98a7-7fd8551e8e70\",\r\n                    \"path\": \"<Keyboard>/d\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Default\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"359be4e4-523d-4ac2-9c70-ad1d079d249f\",\r\n                    \"path\": \"<Gamepad>/leftStick/x\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2e3e3903-9d35-40af-9f9e-907f6273799f\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/stick/x\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"Invert,SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0dc8fe64-bcd4-4bb2-b884-b6d90a55ea3e\",\r\n                    \"path\": \"<Joystick>/stick/x\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"16d46cc4-2d37-4d16-a26b-783f850680df\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SteeringMouse\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"2a3dac17-c9c2-4a75-a397-c91465256fb9\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SteeringMouse\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"bbe4aa90-bee0-41db-adf7-a263ff901511\",\r\n                    \"path\": \"<Mouse>/delta/x\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SteeringMouse\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d22f737c-3a75-4d5d-94d7-1ed11090a110\",\r\n                    \"path\": \"<Keyboard>/shift\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SetGearVisible\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7a54d3c4-c8bb-4ffe-9a72-759a4d53f3cc\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SetGearVisible\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0159639b-8bf7-4f28-8741-bb46b4882716\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/slider\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"Invert,SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SetGearVisible\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bd779c47-9f92-4c45-8105-87b2a7d6fa65\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/slider\",\r\n                    \"interactions\": \"SliderPress(pressPoint=0.99)\",\r\n                    \"processors\": \"SliderRemap\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SetGearVisible\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"SteeringWheelCar\",\r\n            \"id\": \"5d812162-dfc5-4574-a531-f6fdfd1d42a4\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"gear1\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f7627425-57e1-4aef-8287-6291564b174b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gear2\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"202f6fe8-04fd-42cf-ac73-47a64678d060\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gear3\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"82c74d3e-1461-4fc3-b9ad-da71421ae991\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gear4\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"541a67c0-84a9-4593-b50c-63af82aa2cf1\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gear5\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"48cfb531-c1c5-4bb2-a2a9-149267c9ea30\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gear6\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e1b6e907-302f-4322-9d11-ffa46a4922e9\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gear7\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"4b39a3ad-cc9a-4e36-a509-f082a1323ddc\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gearR\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"bb6dfbeb-9f82-402c-b911-abae6c2e325c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gearN\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"1b9d60b5-694f-4348-8bcf-1b158b9430f7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gearUp\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fc76ecab-d4eb-4af0-8e3f-3e651d465e06\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gearAMToggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"02203c48-b8b0-4178-b718-36dffda9f524\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gearDown\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"1f6cd2bd-0000-447b-8c71-a3e308d173f0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"LeftSignal\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"280bf0f7-7401-46c6-a40f-a6f51a57f54f\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"RightSignal\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"99b93b9a-d442-44e5-bdfc-9bd9a55a2e1b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Emergency\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"3f617b88-9e6d-4ee3-8d0f-d9b509436875\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2a97084c-7b58-452d-ab6a-b57b1c0b8ef3\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button114\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a41ca65b-48bb-4077-abf8-a4ff66e44948\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button9\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"100e1488-193b-4806-b698-1e5f991b2f70\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button115\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"02a7a568-8040-49d5-8ab0-ea9200a9e07e\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button10\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c20c32b5-86f5-4d5f-8d11-0a4b3316323b\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button116\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"680c3ab2-2348-4bb5-9e6a-a02c1762038a\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button11\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a6f060d7-40e2-4c07-a934-4cbe29da8c41\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button117\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3cb76f6a-df2e-45b7-a002-a3a9b0b47e90\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button12\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"620c0474-1ade-42ea-b872-c739889493ce\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button118\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear5\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a71fb124-0e32-4860-a24c-3e2247be9611\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button13\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear5\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"24f39afa-2f49-4c57-97e0-ada6af1e0511\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button119\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear6\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b7983ee4-5c2a-4fc6-8725-0105574fec8d\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button14\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear6\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"60f6e189-5915-4080-828d-6087b52836a4\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button120\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gear7\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7c485cfd-400f-42f7-b698-db9e89951302\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button113\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearR\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5145398f-1046-41b1-895a-803b3a81a8a9\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button15\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearR\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"df15e4fb-f7cf-4633-9299-a1b6760a7738\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button113\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"097a4890-014d-4b87-bde5-0c09f8ccd76b\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button120\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c815df80-658f-407c-aebd-187f903cffd8\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button119\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9374622d-954a-457b-99f7-1a2a3aee025d\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button118\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6034f3b6-5d15-4e17-9335-b10f1f155e5c\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button117\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c6efbc78-e789-4ae7-bdb7-6e41038e3040\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button116\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8b60e018-eaf7-427a-9d5a-6eb74b5a22dc\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button115\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"43b56528-5401-4bef-8004-7394a1d031a9\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button114\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"96c91863-5622-4ae8-9887-e4b0cfaf67aa\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button15\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"11cf90d8-ebf4-4f8a-9590-e7c319330d00\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button14\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6d019785-0e59-4819-aae4-8479368485bb\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button13\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b90fcf83-7dd5-4e18-b0aa-df2f7844b4a5\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button12\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e5e59e1f-7862-4897-8794-7b138e1ee5f0\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button11\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c8ed22c8-8c11-40dc-a765-bcbe994cc127\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button10\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8be52f2a-87c3-488f-9d00-abde5f75a5a6\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button9\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"13e59377-6379-46d2-a5b7-ca609b4a85b3\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button14\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3fd8f953-a1bb-40a1-96a8-54bfeea85695\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1a03ccd9-7084-4102-8d15-6359c7b6915e\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button9\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d378a366-0484-48da-8cbd-20de94c3c4b6\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button33\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearAMToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f5256300-e3fe-4626-be20-1b12539f20af\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/trigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearAMToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d725399e-02d1-41dc-91f5-117e286a7f74\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button13\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"53c6a5f0-ad27-4209-82e0-ceda8835ffe8\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"59641856-f103-43fc-8e9b-263333d035db\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button10\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"gearDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a84fef36-8ff1-4f53-a826-db1f8ad82337\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button21\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"LeftSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7df55f2e-ace3-4a52-9d03-9777220dd2e1\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button34\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"RightSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b0a9b6ad-4854-4b2a-8b5f-56cad5814d82\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button20\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Emergency\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Shifter\",\r\n            \"id\": \"b237cf9e-7086-40f2-99bd-39f6f7a9b17f\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"ShiftM\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"51b0564a-c4ad-48dd-8e05-96959d91e102\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ToggleMA\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"9e6ffc8f-24cb-4c31-8426-baa478122dc5\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"AxisDeadzone\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ShiftDown\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6de8bd5c-5594-42e5-b7c3-a7f779505063\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ShiftUp\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"f3c113af-90d7-493e-afb4-3dda797076c5\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ShiftPRND\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"5adb4b92-22c9-497b-b72a-5bdc46765dd1\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ShifterMove\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"0ec0a320-a65c-4d1a-87d8-97a2041228bd\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"c2f351bf-d4aa-4bca-bcd7-a90fbddad751\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"Tap(pressPoint=0.8)\",\r\n                    \"processors\": \"AxisDeadzone(min=0.5)\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShiftM\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"bfd70bee-7dbe-45b4-88cf-b54152d9f675\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftM\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"5ddea859-b8e7-4933-99f5-ff66250ef715\",\r\n                    \"path\": \"<Gamepad>/rightStick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftM\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"671d54d6-0db1-48d5-b416-a31bfa3d95cc\",\r\n                    \"path\": \"<Keyboard>/shift\",\r\n                    \"interactions\": \"Tap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ToggleMA\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"bb982170-22e6-48a6-8341-8d8cb3cc43d6\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"Tap(pressPoint=0.8)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShiftDown\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"e083698b-dddf-49a1-883e-e3ad5094fd4a\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"bcd5950d-c312-4fec-922b-810ccf7c75e2\",\r\n                    \"path\": \"<Gamepad>/rightStick/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"545977a6-aaa4-4109-8908-ed390a932c24\",\r\n                    \"path\": \"<Keyboard>/q\",\r\n                    \"interactions\": \"Tap(pressPoint=0.8)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"e94ca110-c08a-4d53-8318-3c731bd17fff\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"Tap(pressPoint=0.8)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShiftUp\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"c758d1c4-ceaf-4904-9714-f030fe6c44a1\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"02a1b7a6-2229-42e2-bd5d-bf3d8762a35c\",\r\n                    \"path\": \"<Gamepad>/rightStick/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f7f5cc33-b106-4e58-8135-ff7538d69a2e\",\r\n                    \"path\": \"<Keyboard>/e\",\r\n                    \"interactions\": \"Tap(pressPoint=0.8)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"45afdefe-be45-48ba-93ae-3aabed945e22\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"Tap(pressPoint=0.8)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShiftPRND\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"17a57cbf-3e0c-4f5c-bfde-052e6ef3ad1a\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftPRND\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"d2855e45-44db-4f04-8897-8740b935d8ff\",\r\n                    \"path\": \"<Gamepad>/rightStick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShiftPRND\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"b5010ff6-7a24-4355-b5f7-9b13436c6922\",\r\n                    \"path\": \"OneModifier(overrideModifiersNeedToBePressedFirst=true)\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShifterMove\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"94f2971e-d444-45d0-a4a2-520d05843fe8\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShifterMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"e9bf890e-4f84-471d-98b4-2f051abb7d3b\",\r\n                    \"path\": \"<Gamepad>/rightStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShifterMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"ee8daf59-4ac4-4b2f-be21-d836faa459c7\",\r\n                    \"path\": \"OneModifier(overrideModifiersNeedToBePressedFirst=true)\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShifterMove\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"26ffcba6-f53f-4c6a-b5ee-68e71ee020e4\",\r\n                    \"path\": \"<Keyboard>/shift\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShifterMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"938d9bef-b7ec-42e4-84cb-e3adea0d294e\",\r\n                    \"path\": \"<Mouse>/delta\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ShifterMove\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"ToggleUi\",\r\n            \"id\": \"17ddf5b9-23f8-4e3b-9290-3e4a37ba2c90\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"ToggleUi\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e28cc5d1-c55d-4fd4-b7c5-6ea8be079bd7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"907fabf6-9c93-46bc-a7cb-799f6ae9a182\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ToggleUi\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"801fe1e7-6b9b-4302-bb84-404c6bf3b953\",\r\n                    \"path\": \"<Keyboard>/f\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"ToggleUi\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Debug\",\r\n            \"id\": \"f7f44e57-810d-44d0-8728-c77a529d4e12\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"RestoreInput\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"230cce23-d903-4f0e-a7d1-364711f30d1c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"SendLogs\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"20fd177d-4534-43a7-8d16-5bf4b83edf8b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"SimulateDismissKeyboard\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"9e339d9d-3436-4359-855b-5d2aaa5b248d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"SimulateSubmitKeyboard\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"99169965-e8ae-4fc7-9b23-962d8e1e44c0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"togglecasual\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"676118bd-6bc6-470f-9299-f916b21f16cb\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"StartCursorOnly\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"77e5892b-2eb9-4333-95b8-f095386a42ca\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"StopCursorOnly\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"a0adca78-e715-4948-a348-e410fda137b8\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"dc6b00ab-8879-47ae-b95d-bfd07e8b023a\",\r\n                    \"path\": \"<Keyboard>/f5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"RestoreInput\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7c6ed4bf-f910-4493-a665-459d100d281e\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"MultiTap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"RestoreInput\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"437cbbda-5c51-43e8-944d-a2d910cccef9\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button2\",\r\n                    \"interactions\": \"MultiTap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"RestoreInput\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"16f3a7e4-d65b-4e83-b266-2fb22f425363\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button19\",\r\n                    \"interactions\": \"MultiTap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"RestoreInput\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Two Modifiers\",\r\n                    \"id\": \"6344372d-6f89-4174-a735-79bca88654ad\",\r\n                    \"path\": \"TwoModifiers\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SendLogs\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier1\",\r\n                    \"id\": \"e0caddf1-d608-4b06-a065-a6b2d0884afe\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SendLogs\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"modifier2\",\r\n                    \"id\": \"8ed1a0e9-850e-4750-aa2c-7cb2c6193f75\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SendLogs\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"4204a362-641c-4b04-8fbc-85c168df51bc\",\r\n                    \"path\": \"<Gamepad>/dpad\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SendLogs\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"76e395bf-59ab-441c-a965-23ae2dea3f3e\",\r\n                    \"path\": \"<Keyboard>/f9\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SendLogs\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"004ab6bf-e660-4a8d-88ec-706f68690007\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button23\",\r\n                    \"interactions\": \"MultiTap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SendLogs\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"52e54a2a-f21d-41e1-9a52-deaa08a2aa5d\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/trigger\",\r\n                    \"interactions\": \"MultiTap\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SendLogs\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8fed05a3-a091-47f3-b465-b0faa31d98e6\",\r\n                    \"path\": \"<Keyboard>/escape\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SimulateDismissKeyboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"09763165-81df-47da-b09f-7af05941b48b\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SimulateDismissKeyboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7f574884-9682-4c32-91c7-1618fdcf50d9\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button2\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SimulateDismissKeyboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"82a2e61f-5e63-4217-b78c-30aa621f302d\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button19\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SimulateDismissKeyboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7aebd1f6-e4ff-4298-b725-f96c1a92cc0d\",\r\n                    \"path\": \"<Keyboard>/enter\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SimulateSubmitKeyboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5915e173-f504-4702-9ba8-afa01ecb76db\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SimulateSubmitKeyboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f25be929-3be9-406d-ab78-e09912de63da\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/trigger\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SimulateSubmitKeyboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c9f43530-8e29-4c8b-a823-0cda33e4a73e\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button18\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"SimulateSubmitKeyboard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"One Modifier\",\r\n                    \"id\": \"6f9bb7ab-fddc-4c02-a92b-2c2e8152edac\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"togglecasual\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"631d94d8-bba1-41a0-93df-491ae9985782\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"togglecasual\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"d68c4b11-af5c-4a8a-a682-ce66c0112308\",\r\n                    \"path\": \"<Gamepad>/rightStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"togglecasual\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"G25\",\r\n                    \"id\": \"eba175f8-0337-4ce3-904e-499fbaaee4b0\",\r\n                    \"path\": \"OneModifier\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"togglecasual\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"modifier\",\r\n                    \"id\": \"93425055-3c33-40ec-baef-9ec3ef866f99\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button8\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"togglecasual\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"binding\",\r\n                    \"id\": \"28565c30-c8b6-470d-a3d4-3867560631aa\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button7\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"togglecasual\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5fec88bb-739e-4c51-9b05-8f3959286e0f\",\r\n                    \"path\": \"<Keyboard>/f6\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"togglecasual\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d42c68f7-8465-409d-8379-fd7bcfb3b236\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button24\",\r\n                    \"interactions\": \"Hold(duration=1.5)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"togglecasual\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5a514dbf-d1de-4fb0-95e1-d8e3969eaa1d\",\r\n                    \"path\": \"<Keyboard>/alt\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"StartCursorOnly\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"276a40af-ceff-4650-adeb-b12f64ac4897\",\r\n                    \"path\": \"<Keyboard>/alt\",\r\n                    \"interactions\": \"Press(behavior=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"StopCursorOnly\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"MozaHelp\",\r\n            \"id\": \"386dfe3c-664e-4e31-b060-6bcef3a2a04a\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"x-south\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c522204c-5f52-4156-8ecd-7be5669241e9\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"b-east\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"dd897ec2-ff84-4951-81df-18e88bd1926d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"b-west\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"2fe655e0-8282-4d1b-807e-a6cdaab651c6\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"b-north\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"5ee35907-f082-4337-8476-fc618e39b436\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"d-right\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"819e1d3f-440d-49f8-9bd9-d5c3314b6e7c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"d-left\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"a0554e7e-0e3f-452c-8880-0da3168fabc0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"d-down\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fb045cbd-a58d-4cde-9ecc-fbed69088563\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"d-up\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b4b52bb5-8991-4264-bed3-5dac3da620c2\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"menu\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"a00b9fc6-e851-4c54-a1dc-94eac94e224b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"home\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"679d7594-cb84-4e80-8899-42831736e36c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"START\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f721d0b9-3314-49d5-ab99-26972745be3b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"radio\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"9af31c23-0198-4ace-97cc-c7f3453c0387\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"cam\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fb16b95d-897a-498e-a57d-8cc973c75c53\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"W-N\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"9217ec91-fcb4-4df1-a87d-df43afef2a9d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"WIP\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"10fea721-93ff-4c11-b237-1588c101ed18\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"W-P\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"7e7bb3f3-d74b-4333-b7c8-7133db33cf8c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"BOX\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"4549f6be-5dea-4355-8ab8-c78ea19484a1\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"W-R\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"8c1f56fe-c533-4a37-874a-5c5ef55effbb\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"FL\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"0285e52a-c829-4dd3-a089-6a5505e466ad\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"PL\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"07c47238-30dc-4c7e-837c-753e5ce4a95e\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"S1\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"7fbe0e0c-ff95-4ffd-9992-915eb2b124bb\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"S2\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"482a7c4f-1dbd-4ab6-85a9-a7f43c1d93bc\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"left-wing\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"3e8add8b-95c3-4148-b0c5-2f3ebf58bca9\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"right-wing\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"709092cb-d8fd-4e3a-b3d2-b5078bab2749\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"wheel\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"1af6ff58-a188-4296-baf4-2871cfe4c77a\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"throttle\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"3dca28b8-96a9-417a-8795-440dfe874e5b\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"break\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"39e47f1b-ca9d-46a2-aef3-03bd08c3a782\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"clutch\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"a4809043-043c-4c64-a994-86f41614b902\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"gear-1\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"261cc0e7-28c9-46eb-a953-4a4afc4aa4fd\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gear-2\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"311b558f-793d-4034-8bf1-8f739c680441\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gear-3\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"08d25ff0-62b0-4960-ae78-ad687e21488b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gear-4\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fd887947-78a7-4372-b373-a662792cf021\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gear-5\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6aef210b-aab0-4320-bbf8-20fc85451a4d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gear-6\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"ee837080-7a37-44fe-9b96-c4c33db51cb7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gear-7\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e1d0a6d7-eb22-4953-bb9a-e24d0c77a161\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gear-R\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"21fc2d57-5a5e-4615-9a21-13078f771e99\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gear-N\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"12ee5931-f013-4a23-bc77-6fe8ef4be06f\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"wheelLButton\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"62531dac-97e6-4573-b997-3e4699354957\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"wheelRButton\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"61786225-fdd0-48df-aea5-63d0c08f47b5\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gearboxButton1\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"0271b1b7-01a4-4483-a45d-305943649c13\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gearboxButton2\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"5b2101b2-c987-456e-99e7-90588b0050b7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gearboxButton3\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b8e67cc3-266b-4a6a-a13e-476605f1056c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gearboxButton4\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"dc670455-8d3a-4731-9c08-ce796799580a\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"gearup\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"567f9793-2464-45bd-bd4c-8cbf100b1ce5\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"geardown\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e83ddbe5-c5d0-4718-be9b-55f3b56685a2\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"handbrake\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"d2ad2971-a3f4-48b3-921b-ca14721867da\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f40e8241-377f-4dce-ab8d-a4eb3e1e0d37\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"x-south\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"76438f38-a5f6-4bd8-b606-b59627f0dbaf\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/trigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"x-south\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2d34d127-3ea9-47fd-b184-6f390eea82bb\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button18\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"x-south\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c8ff0079-ebd4-46d9-bd85-beb9f7870d55\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"b-east\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"652b8de9-4a9a-44b6-8d8f-f8034bfcf58d\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button2\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"b-east\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"73a6f634-179d-4ed0-9fcf-b19ef374f12d\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button19\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"b-east\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e9e0c033-05c7-42a6-b054-d423f1bbd799\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"b-west\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a091bd72-c890-492d-9948-e91c79e7f994\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button3\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"b-west\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c8f1fe3d-d6a0-41b7-9db9-67fcc3a32dcc\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button17\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"b-west\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"33caa6ad-ce99-4bfe-8ab3-3c25b834d22d\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"b-north\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a891730b-44c9-4664-820a-48ac9fadbcb2\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button4\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"b-north\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5a0fc42a-f167-4f16-b0b6-865c1c701df6\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button16\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"b-north\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9ceda465-4507-4abf-9df4-6a4881c3c18f\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-right\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"204738e9-1915-4ff0-8d32-7e89b1a7bcb4\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-right\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6c24acac-faf3-442c-a756-6951b292e7a2\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-right\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e7285f02-f65a-4582-a315-bceca1b97054\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-left\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e81b4d79-9371-48b6-8467-8bbffb3587ac\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button8\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-left\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"58f8be8f-3e43-4e60-950a-d3f5ac24f94f\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-left\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"57099b8d-fa68-427a-85df-b4f16c2af3f3\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-down\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b8233be7-7ca0-4804-8ef6-8d74d768cebf\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button7\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-down\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f0b4552e-7873-45ea-bd71-43a7f8252f96\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-down\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f38c8006-08e2-47cd-a259-f46fcce62baf\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-up\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"de906743-85bb-4a14-9f1c-fcf40f9273a1\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-up\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d214eba2-9e67-4189-87ca-f2d03381fd81\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/hat/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"d-up\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a9cd898f-9a6a-4b19-aecd-b83c19764692\",\r\n                    \"path\": \"<Gamepad>/start\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"menu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b08647d8-1b9b-4b76-b25a-9d53944d5d5a\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button38\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"menu\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c0c46199-fa16-4325-a0e4-64ffd43c2e68\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"home\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"06e96945-b92a-423e-8111-934b5781b378\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button25\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"home\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7d4509af-0e4a-4820-867a-c1b11921fbf7\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button36\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"START\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f0cb0abc-d075-49a6-8ed7-9f24322dccf1\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button23\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"radio\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b7e89869-a6c2-4e60-bb4a-eaa1011b8cfc\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button22\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"cam\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5798f2b1-cacb-427b-a8e1-fb7f4e1e9d99\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button19\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"W-N\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3a6881c7-705f-4322-8f06-1f7e50952790\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button20\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"WIP\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6de12970-2da0-4da0-8cd6-085954da0925\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button32\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"W-P\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"32543b73-b0fc-4f57-9e92-f507b75ab1da\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button33\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"BOX\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1d23b85-37c5-49a0-bd4d-874e95874663\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button35\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"W-R\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cb4e51cc-45b1-40fd-87bd-8196e00b53c5\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button21\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"FL\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c268738e-8068-4c84-8cdd-bd9b8f69d72c\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button34\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"PL\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3b4d2e7c-8bfa-4b57-b231-203cc342d67d\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button24\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"S1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"22bae6ac-1c45-46da-a242-17cd6189fa40\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button37\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"S2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ef68204a-eb4d-49d1-8ad0-8552a650da13\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button13\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"left-wing\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f94ea507-8274-48d6-827f-691f06e7b4f8\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"left-wing\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e83953a4-9ff9-4651-97ab-6d9613a5a668\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button14\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"right-wing\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"42cffcb9-de78-4b08-9be5-778526ec3659\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"right-wing\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4d6f61f5-b8fd-402f-88bc-e3082648a67d\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/stick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d3cadadb-de07-4d94-874a-9df66488e17d\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/stick/x\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"d43818c5-fe8e-4ff1-adb6-7ccf171fbab4\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheel\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"33dc426d-77d9-40cb-86d9-4215bcc86ada\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/stick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"c3d97334-50d7-4b94-919f-4336f4d201df\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/stick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"4071ea89-d898-41e6-b79f-f5df5c47b36d\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheel\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"79debd88-bbe3-45c4-a7f6-784f9c98e455\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/stick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"bb04ebd3-796a-4438-9b5e-88c1ed918952\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/stick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2af0b389-1db9-41ee-affd-c81748597562\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/z\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"119584e8-34bb-4c9f-aee5-7ffb530ad69f\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/z\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7c4da551-96f8-4931-84cf-3cbe70ba135c\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/rz\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"break\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e7a0323b-d43b-453e-9bb3-1cc97edfa768\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/rz\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"break\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9685f78e-baf3-4b8a-b55c-019630c529be\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/slider\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"964738be-728a-4001-ba86-9afaa3b388e3\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/slider\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0e37c438-5454-45ec-81fd-489f438eb9a0\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button114\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e4dfd4aa-6507-4c5e-89f9-666d72476bc6\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button9\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"923dcec1-c4a8-44b5-a3f5-63b03a9ea11e\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button115\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7a632b26-2e67-466d-aee1-ffdb4885ef3e\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button10\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"67c53e7d-0a1f-420b-98f6-1daf635cbe0e\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button116\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ac8671ee-9152-45d8-aafe-dc454cf59dfb\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button11\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"91cf740d-e9f9-4a01-b060-9fc241998eb8\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button117\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"42ca31ed-1be5-4c3e-b503-469fe2fde651\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button12\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4e330300-52e4-4f9e-aefe-54701d3dd5aa\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button118\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-5\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b70e67a0-975c-46ec-8eca-c2a6cc220c53\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button13\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-5\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9ee1594d-521d-4b49-abdf-7e7877c65a67\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button119\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-6\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8b423ffd-6172-4854-9d29-4b6306cdd38d\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button14\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-6\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b2775b6c-6670-4c40-9343-ca92d5bea2d1\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button120\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-7\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b6ace4b3-e349-42b8-b260-61e4149d7f33\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base>/button113\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-R\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8fa2075a-a4e1-45a5-a0d5-758df51ff704\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button15\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-R\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fa1b0722-7e1e-4d82-8286-0c239e5b1976\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gear-N\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0c945b98-e082-4c2e-87e0-5cdbd972e4ba\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button8\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheelLButton\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9f6cf4d0-fc07-4706-b141-fd633b7dac8f\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button7\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"wheelRButton\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ba80d80e-b694-4c31-b02b-32f6e3fd5dce\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/trigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gearboxButton1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"afef39dd-546f-4d3f-9651-de7ad2d94460\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button2\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gearboxButton2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6df303c0-4d95-4d32-bbd1-783e0cd27224\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button3\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gearboxButton3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7e8c800b-b613-49fa-8eff-aa060f19d59c\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button4\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gearboxButton4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c8a01cb7-56b9-4fd0-9c92-b9ee3333b3af\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button9\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"gearup\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"009a5227-f708-47dd-bcab-03ce06ae179c\",\r\n                    \"path\": \"<HID::G25 Racing Wheel>/button10\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"geardown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e001124b-fe6e-4afd-8159-4cad4054265f\",\r\n                    \"path\": \"<HID::ZSC SIMAKER-Handbrake>/slider\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Person\",\r\n            \"id\": \"3e31e888-4b5d-4452-ad3e-f4aa1293c8e7\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Run\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b1a10001-0000-4000-8000-000000000001\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Crouch\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b1a10002-0000-4000-8000-000000000002\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Jump\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b1a10003-0000-4000-8000-000000000003\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"WalkRunToggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b1a10004-0000-4000-8000-000000000004\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1a10011-0000-4000-8000-000000000011\",\r\n                    \"path\": \"<Keyboard>/shift\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Run\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1a10012-0000-4000-8000-000000000012\",\r\n                    \"path\": \"<Gamepad>/rightTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Run\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1a10021-0000-4000-8000-000000000021\",\r\n                    \"path\": \"<Keyboard>/leftCtrl\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Crouch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1a10022-0000-4000-8000-000000000022\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Crouch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1a10031-0000-4000-8000-000000000031\",\r\n                    \"path\": \"<Keyboard>/space\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Jump\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1a10032-0000-4000-8000-000000000032\",\r\n                    \"path\": \"<Gamepad>/leftTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"Jump\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1a10041-0000-4000-8000-000000000041\",\r\n                    \"path\": \"<Keyboard>/capsLock\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"WalkRunToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1a10042-0000-4000-8000-000000000042\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Default\",\r\n                    \"action\": \"WalkRunToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"controlSchemes\": [\r\n        {\r\n            \"name\": \"Default\",\r\n            \"bindingGroup\": \"Default\",\r\n            \"devices\": []\r\n        }\r\n    ]\r\n}");
		m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
		m_UI_Navigate = m_UI.FindAction("Navigate", throwIfNotFound: true);
		m_UI_goBack = m_UI.FindAction("goBack", throwIfNotFound: true);
		m_UIAcceptCancel = asset.FindActionMap("UIAcceptCancel", throwIfNotFound: true);
		m_UIAcceptCancel_Submit = m_UIAcceptCancel.FindAction("Submit", throwIfNotFound: true);
		m_UIAcceptCancel_Cancel = m_UIAcceptCancel.FindAction("Cancel", throwIfNotFound: true);
		m_AcceptCancel = asset.FindActionMap("AcceptCancel", throwIfNotFound: true);
		m_AcceptCancel_Submit = m_AcceptCancel.FindAction("Submit", throwIfNotFound: true);
		m_AcceptCancel_Cancel = m_AcceptCancel.FindAction("Cancel", throwIfNotFound: true);
		m_UiPointer = asset.FindActionMap("UiPointer", throwIfNotFound: true);
		m_UiPointer_Point = m_UiPointer.FindAction("Point", throwIfNotFound: true);
		m_UiPointer_CursorMove = m_UiPointer.FindAction("CursorMove", throwIfNotFound: true);
		m_UiPointer_Click = m_UiPointer.FindAction("Click", throwIfNotFound: true);
		m_UiPointer_RightClick = m_UiPointer.FindAction("RightClick", throwIfNotFound: true);
		m_UiPointer_MiddleClick = m_UiPointer.FindAction("MiddleClick", throwIfNotFound: true);
		m_UiPointer_ScrollWheel = m_UiPointer.FindAction("ScrollWheel", throwIfNotFound: true);
		m_UiPointer_TrackedDevicePosition = m_UiPointer.FindAction("TrackedDevicePosition", throwIfNotFound: true);
		m_UiPointer_TrackedDeviceOrientation = m_UiPointer.FindAction("TrackedDeviceOrientation", throwIfNotFound: true);
		m_UiPointer_PointerDelta = m_UiPointer.FindAction("PointerDelta", throwIfNotFound: true);
		m_UiNextPrevious = asset.FindActionMap("UiNextPrevious", throwIfNotFound: true);
		m_UiNextPrevious_Next = m_UiNextPrevious.FindAction("Next", throwIfNotFound: true);
		m_UiNextPrevious_Previous = m_UiNextPrevious.FindAction("Previous", throwIfNotFound: true);
		m_UiThrottle = asset.FindActionMap("UiThrottle", throwIfNotFound: true);
		m_UiThrottle_Throttle = m_UiThrottle.FindAction("Throttle", throwIfNotFound: true);
		m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
		m_Menu_Menu = m_Menu.FindAction("Menu", throwIfNotFound: true);
		m_ShowMap = asset.FindActionMap("ShowMap", throwIfNotFound: true);
		m_ShowMap_Map = m_ShowMap.FindAction("Map", throwIfNotFound: true);
		m_MapUi = asset.FindActionMap("MapUi", throwIfNotFound: true);
		m_MapUi_HideShow = m_MapUi.FindAction("HideShow", throwIfNotFound: true);
		m_MapUi_HideShowAll = m_MapUi.FindAction("HideShowAll", throwIfNotFound: true);
		m_MapUi_Expand = m_MapUi.FindAction("Expand", throwIfNotFound: true);
		m_MapUi_CloseMap = m_MapUi.FindAction("CloseMap", throwIfNotFound: true);
		m_MapUi_Mark = m_MapUi.FindAction("Mark", throwIfNotFound: true);
		m_MapUi_ShowMarkDetails = m_MapUi.FindAction("ShowMarkDetails", throwIfNotFound: true);
		m_MapMove = asset.FindActionMap("MapMove", throwIfNotFound: true);
		m_MapMove_Move = m_MapMove.FindAction("Move", throwIfNotFound: true);
		m_MapMove_MouseDragMove = m_MapMove.FindAction("MouseDragMove", throwIfNotFound: true);
		m_MapMove_MouseDelta = m_MapMove.FindAction("MouseDelta", throwIfNotFound: true);
		m_Drone = asset.FindActionMap("Drone", throwIfNotFound: true);
		m_Drone_SpeedToggle = m_Drone.FindAction("SpeedToggle", throwIfNotFound: true);
		m_Drone_Tilt = m_Drone.FindAction("Tilt", throwIfNotFound: true);
		m_Drone_HideUi = m_Drone.FindAction("HideUi", throwIfNotFound: true);
		m_Drone_Center = m_Drone.FindAction("Center", throwIfNotFound: true);
		m_Drone_Back = m_Drone.FindAction("Back", throwIfNotFound: true);
		m_Drone_OpenChat = m_Drone.FindAction("OpenChat", throwIfNotFound: true);
		m_DroneCamera = asset.FindActionMap("DroneCamera", throwIfNotFound: true);
		m_DroneCamera_Reset = m_DroneCamera.FindAction("Reset", throwIfNotFound: true);
		m_DroneCamera_Snapshot = m_DroneCamera.FindAction("Snapshot", throwIfNotFound: true);
		m_Photo = asset.FindActionMap("Photo", throwIfNotFound: true);
		m_Photo_Help = m_Photo.FindAction("Help", throwIfNotFound: true);
		m_Photo_ToggleFocus = m_Photo.FindAction("ToggleFocus", throwIfNotFound: true);
		m_Photo_Toggles = m_Photo.FindAction("Toggles", throwIfNotFound: true);
		m_Photo_NavigateSettings = m_Photo.FindAction("NavigateSettings", throwIfNotFound: true);
		m_Photo_Color = m_Photo.FindAction("Color", throwIfNotFound: true);
		m_Photo_Optica = m_Photo.FindAction("Optica", throwIfNotFound: true);
		m_Photo_Camera = m_Photo.FindAction("Camera", throwIfNotFound: true);
		m_Photo_ResetFilter = m_Photo.FindAction("ResetFilter", throwIfNotFound: true);
		m_PaintBase = asset.FindActionMap("PaintBase", throwIfNotFound: true);
		m_PaintBase_BlockCam = m_PaintBase.FindAction("BlockCam", throwIfNotFound: true);
		m_PaintBase_ToggleUi = m_PaintBase.FindAction("ToggleUi", throwIfNotFound: true);
		m_PaintBase_DeleteAll = m_PaintBase.FindAction("DeleteAll", throwIfNotFound: true);
		m_PaintBase_DeleteSelected = m_PaintBase.FindAction("DeleteSelected", throwIfNotFound: true);
		m_PaintFavoritesToggle = asset.FindActionMap("PaintFavoritesToggle", throwIfNotFound: true);
		m_PaintFavoritesToggle_FavoritesToggle = m_PaintFavoritesToggle.FindAction("FavoritesToggle", throwIfNotFound: true);
		m_PaintApplyCancel = asset.FindActionMap("PaintApplyCancel", throwIfNotFound: true);
		m_PaintApplyCancel_CancelChange = m_PaintApplyCancel.FindAction("CancelChange", throwIfNotFound: true);
		m_PaintMultiSelect = asset.FindActionMap("PaintMultiSelect", throwIfNotFound: true);
		m_PaintMultiSelect_MultiSelect = m_PaintMultiSelect.FindAction("MultiSelect", throwIfNotFound: true);
		m_PaintSelectNew = asset.FindActionMap("PaintSelectNew", throwIfNotFound: true);
		m_PaintSelectNew_SelectNew = m_PaintSelectNew.FindAction("SelectNew", throwIfNotFound: true);
		m_PaintPlates = asset.FindActionMap("PaintPlates", throwIfNotFound: true);
		m_PaintPlates_NavigatePlateType = m_PaintPlates.FindAction("NavigatePlateType", throwIfNotFound: true);
		m_PaintPlates_FrontToggle = m_PaintPlates.FindAction("FrontToggle", throwIfNotFound: true);
		m_PaintPlates_BackToggle = m_PaintPlates.FindAction("BackToggle", throwIfNotFound: true);
		m_PaintSelectSet = asset.FindActionMap("PaintSelectSet", throwIfNotFound: true);
		m_PaintSelectSet_SelectSetVinyl = m_PaintSelectSet.FindAction("SelectSetVinyl", throwIfNotFound: true);
		m_PaintCopy = asset.FindActionMap("PaintCopy", throwIfNotFound: true);
		m_PaintCopy_Copy = m_PaintCopy.FindAction("Copy", throwIfNotFound: true);
		m_PaintPlateTypes = asset.FindActionMap("PaintPlateTypes", throwIfNotFound: true);
		m_PaintPlateTypes_EnterPlateTypes = m_PaintPlateTypes.FindAction("EnterPlateTypes", throwIfNotFound: true);
		m_PaintFavorite = asset.FindActionMap("PaintFavorite", throwIfNotFound: true);
		m_PaintFavorite_Favorite = m_PaintFavorite.FindAction("Favorite", throwIfNotFound: true);
		m_PaintSelectionMade = asset.FindActionMap("PaintSelectionMade", throwIfNotFound: true);
		m_PaintSelectionMade_LayerUpDown = m_PaintSelectionMade.FindAction("LayerUpDown", throwIfNotFound: true);
		m_PaintSelectionMade_Move = m_PaintSelectionMade.FindAction("Move", throwIfNotFound: true);
		m_PaintSelectionMade_SetSize = m_PaintSelectionMade.FindAction("SetSize", throwIfNotFound: true);
		m_PaintSelectionMade_SetColor = m_PaintSelectionMade.FindAction("SetColor", throwIfNotFound: true);
		m_PaintColor = asset.FindActionMap("PaintColor", throwIfNotFound: true);
		m_PaintColor_ColorToggle = m_PaintColor.FindAction("ColorToggle", throwIfNotFound: true);
		m_PaintColor_TintToggle = m_PaintColor.FindAction("TintToggle", throwIfNotFound: true);
		m_PaintColor_AlphaValue = m_PaintColor.FindAction("AlphaValue", throwIfNotFound: true);
		m_PaintColor_ColorMove = m_PaintColor.FindAction("ColorMove", throwIfNotFound: true);
		m_PaintColor_ColorCode = m_PaintColor.FindAction("ColorCode", throwIfNotFound: true);
		m_PaintSize = asset.FindActionMap("PaintSize", throwIfNotFound: true);
		m_PaintSize_Mirror = m_PaintSize.FindAction("Mirror", throwIfNotFound: true);
		m_PaintSize_Turn = m_PaintSize.FindAction("Turn", throwIfNotFound: true);
		m_PaintSize_SizeMove = m_PaintSize.FindAction("SizeMove", throwIfNotFound: true);
		m_PaintSize_SetSize = m_PaintSize.FindAction("SetSize", throwIfNotFound: true);
		m_PaintText = asset.FindActionMap("PaintText", throwIfNotFound: true);
		m_PaintText_Font = m_PaintText.FindAction("Font", throwIfNotFound: true);
		m_PaintText_Bold = m_PaintText.FindAction("Bold", throwIfNotFound: true);
		m_PaintText_Italics = m_PaintText.FindAction("Italics", throwIfNotFound: true);
		m_PaintText_EnterText = m_PaintText.FindAction("EnterText", throwIfNotFound: true);
		m_PaintEditText = asset.FindActionMap("PaintEditText", throwIfNotFound: true);
		m_PaintEditText_EditTextStyle = m_PaintEditText.FindAction("EditTextStyle", throwIfNotFound: true);
		m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
		m_Camera_Look = m_Camera.FindAction("Look", throwIfNotFound: true);
		m_Camera_LookFree = m_Camera.FindAction("LookFree", throwIfNotFound: true);
		m_Move = asset.FindActionMap("Move", throwIfNotFound: true);
		m_Move_Move = m_Move.FindAction("Move", throwIfNotFound: true);
		m_Move_Rotate = m_Move.FindAction("Rotate", throwIfNotFound: true);
		m_Move_Height = m_Move.FindAction("Height", throwIfNotFound: true);
		m_ZoomMouse = asset.FindActionMap("ZoomMouse", throwIfNotFound: true);
		m_ZoomMouse_Zoom = m_ZoomMouse.FindAction("Zoom", throwIfNotFound: true);
		m_ZoomGamepad = asset.FindActionMap("ZoomGamepad", throwIfNotFound: true);
		m_ZoomGamepad_Zoom = m_ZoomGamepad.FindAction("Zoom", throwIfNotFound: true);
		m_ZoomMoza = asset.FindActionMap("ZoomMoza", throwIfNotFound: true);
		m_ZoomMoza_Zoom = m_ZoomMoza.FindAction("Zoom", throwIfNotFound: true);
		m_CameraCar = asset.FindActionMap("CameraCar", throwIfNotFound: true);
		m_CameraCar_CameraLookLeft = m_CameraCar.FindAction("CameraLookLeft", throwIfNotFound: true);
		m_CameraCar_CameraLookRight = m_CameraCar.FindAction("CameraLookRight", throwIfNotFound: true);
		m_CameraCar_CameraLookBack = m_CameraCar.FindAction("CameraLookBack", throwIfNotFound: true);
		m_AimReticle = asset.FindActionMap("AimReticle", throwIfNotFound: true);
		m_AimReticle_AimAt = m_AimReticle.FindAction("AimAt", throwIfNotFound: true);
		m_ChangeCamera = asset.FindActionMap("ChangeCamera", throwIfNotFound: true);
		m_ChangeCamera_ChangeCamera = m_ChangeCamera.FindAction("ChangeCamera", throwIfNotFound: true);
		m_CarRacing = asset.FindActionMap("CarRacing", throwIfNotFound: true);
		m_CarRacing_PhotoMode = m_CarRacing.FindAction("PhotoMode", throwIfNotFound: true);
		m_CarRacing_Timer = m_CarRacing.FindAction("Timer", throwIfNotFound: true);
		m_CarDrag = asset.FindActionMap("CarDrag", throwIfNotFound: true);
		m_CarDrag_Burnout = m_CarDrag.FindAction("Burnout", throwIfNotFound: true);
		m_CarDrag_KeepLane = m_CarDrag.FindAction("KeepLane", throwIfNotFound: true);
		m_BackToCar = asset.FindActionMap("BackToCar", throwIfNotFound: true);
		m_BackToCar_BackToCar = m_BackToCar.FindAction("BackToCar", throwIfNotFound: true);
		m_ToRoad = asset.FindActionMap("ToRoad", throwIfNotFound: true);
		m_ToRoad_ToRoad = m_ToRoad.FindAction("ToRoad", throwIfNotFound: true);
		m_ToRoadSteeringWheel = asset.FindActionMap("ToRoadSteeringWheel", throwIfNotFound: true);
		m_ToRoadSteeringWheel_ToRoad = m_ToRoadSteeringWheel.FindAction("ToRoad", throwIfNotFound: true);
		m_Headlights = asset.FindActionMap("Headlights", throwIfNotFound: true);
		m_Headlights_Headlights = m_Headlights.FindAction("Headlights", throwIfNotFound: true);
		m_Door = asset.FindActionMap("Door", throwIfNotFound: true);
		m_Door_OpenDoor = m_Door.FindAction("OpenDoor", throwIfNotFound: true);
		m_FreeRace = asset.FindActionMap("FreeRace", throwIfNotFound: true);
		m_FreeRace_Restart = m_FreeRace.FindAction("Restart", throwIfNotFound: true);
		m_FreeRace_CancelRace = m_FreeRace.FindAction("CancelRace", throwIfNotFound: true);
		m_Phone = asset.FindActionMap("Phone", throwIfNotFound: true);
		m_Phone_Phone = m_Phone.FindAction("Phone", throwIfNotFound: true);
		m_Voice = asset.FindActionMap("Voice", throwIfNotFound: true);
		m_Voice_VoiceChat = m_Voice.FindAction("VoiceChat", throwIfNotFound: true);
		m_Voice_Chat = m_Voice.FindAction("Chat", throwIfNotFound: true);
		m_ChatGamepad = asset.FindActionMap("ChatGamepad", throwIfNotFound: true);
		m_ChatGamepad_Chat = m_ChatGamepad.FindAction("Chat", throwIfNotFound: true);
		m_CircleMenu = asset.FindActionMap("CircleMenu", throwIfNotFound: true);
		m_CircleMenu_CircleMenuMove = m_CircleMenu.FindAction("CircleMenuMove", throwIfNotFound: true);
		m_CircleMenu_CircleMenuClose = m_CircleMenu.FindAction("CircleMenuClose", throwIfNotFound: true);
		m_CircleMenu_CircleMenuCloseCam = m_CircleMenu.FindAction("CircleMenuCloseCam", throwIfNotFound: true);
		m_CircleMenu_CircleMenuCloseAnim = m_CircleMenu.FindAction("CircleMenuCloseAnim", throwIfNotFound: true);
		m_CircleMenu_CircleMenuScroll = m_CircleMenu.FindAction("CircleMenuScroll", throwIfNotFound: true);
		m_OpenCircleMenu = asset.FindActionMap("OpenCircleMenu", throwIfNotFound: true);
		m_OpenCircleMenu_OpenCircleMenu = m_OpenCircleMenu.FindAction("OpenCircleMenu", throwIfNotFound: true);
		m_OpenCircleMenu_OpenCircleMenuCam = m_OpenCircleMenu.FindAction("OpenCircleMenuCam", throwIfNotFound: true);
		m_CarBase = asset.FindActionMap("CarBase", throwIfNotFound: true);
		m_CarBase_Steering = m_CarBase.FindAction("Steering", throwIfNotFound: true);
		m_CarBase_SteeringMouse = m_CarBase.FindAction("SteeringMouse", throwIfNotFound: true);
		m_CarBase_SetGearVisible = m_CarBase.FindAction("SetGearVisible", throwIfNotFound: true);
		m_CarBase_Throttle = m_CarBase.FindAction("Throttle", throwIfNotFound: true);
		m_CarBase_Brake = m_CarBase.FindAction("Brake", throwIfNotFound: true);
		m_CarBase_Clutch = m_CarBase.FindAction("Clutch", throwIfNotFound: true);
		m_CarBase_Handbrake = m_CarBase.FindAction("Handbrake", throwIfNotFound: true);
		m_CarBase_HandbrakeHold = m_CarBase.FindAction("HandbrakeHold", throwIfNotFound: true);
		m_CarBase_StartEngine = m_CarBase.FindAction("StartEngine", throwIfNotFound: true);
		m_CarBase_Horn = m_CarBase.FindAction("Horn", throwIfNotFound: true);
		m_SteeringWheelCar = asset.FindActionMap("SteeringWheelCar", throwIfNotFound: true);
		m_SteeringWheelCar_gear1 = m_SteeringWheelCar.FindAction("gear1", throwIfNotFound: true);
		m_SteeringWheelCar_gear2 = m_SteeringWheelCar.FindAction("gear2", throwIfNotFound: true);
		m_SteeringWheelCar_gear3 = m_SteeringWheelCar.FindAction("gear3", throwIfNotFound: true);
		m_SteeringWheelCar_gear4 = m_SteeringWheelCar.FindAction("gear4", throwIfNotFound: true);
		m_SteeringWheelCar_gear5 = m_SteeringWheelCar.FindAction("gear5", throwIfNotFound: true);
		m_SteeringWheelCar_gear6 = m_SteeringWheelCar.FindAction("gear6", throwIfNotFound: true);
		m_SteeringWheelCar_gear7 = m_SteeringWheelCar.FindAction("gear7", throwIfNotFound: true);
		m_SteeringWheelCar_gearR = m_SteeringWheelCar.FindAction("gearR", throwIfNotFound: true);
		m_SteeringWheelCar_gearN = m_SteeringWheelCar.FindAction("gearN", throwIfNotFound: true);
		m_SteeringWheelCar_gearUp = m_SteeringWheelCar.FindAction("gearUp", throwIfNotFound: true);
		m_SteeringWheelCar_gearAMToggle = m_SteeringWheelCar.FindAction("gearAMToggle", throwIfNotFound: true);
		m_SteeringWheelCar_gearDown = m_SteeringWheelCar.FindAction("gearDown", throwIfNotFound: true);
		m_SteeringWheelCar_LeftSignal = m_SteeringWheelCar.FindAction("LeftSignal", throwIfNotFound: true);
		m_SteeringWheelCar_RightSignal = m_SteeringWheelCar.FindAction("RightSignal", throwIfNotFound: true);
		m_SteeringWheelCar_Emergency = m_SteeringWheelCar.FindAction("Emergency", throwIfNotFound: true);
		m_Shifter = asset.FindActionMap("Shifter", throwIfNotFound: true);
		m_Shifter_ShiftM = m_Shifter.FindAction("ShiftM", throwIfNotFound: true);
		m_Shifter_ToggleMA = m_Shifter.FindAction("ToggleMA", throwIfNotFound: true);
		m_Shifter_ShiftDown = m_Shifter.FindAction("ShiftDown", throwIfNotFound: true);
		m_Shifter_ShiftUp = m_Shifter.FindAction("ShiftUp", throwIfNotFound: true);
		m_Shifter_ShiftPRND = m_Shifter.FindAction("ShiftPRND", throwIfNotFound: true);
		m_Shifter_ShifterMove = m_Shifter.FindAction("ShifterMove", throwIfNotFound: true);
		m_ToggleUi = asset.FindActionMap("ToggleUi", throwIfNotFound: true);
		m_ToggleUi_ToggleUi = m_ToggleUi.FindAction("ToggleUi", throwIfNotFound: true);
		m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
		m_Debug_RestoreInput = m_Debug.FindAction("RestoreInput", throwIfNotFound: true);
		m_Debug_SendLogs = m_Debug.FindAction("SendLogs", throwIfNotFound: true);
		m_Debug_SimulateDismissKeyboard = m_Debug.FindAction("SimulateDismissKeyboard", throwIfNotFound: true);
		m_Debug_SimulateSubmitKeyboard = m_Debug.FindAction("SimulateSubmitKeyboard", throwIfNotFound: true);
		m_Debug_togglecasual = m_Debug.FindAction("togglecasual", throwIfNotFound: true);
		m_Debug_StartCursorOnly = m_Debug.FindAction("StartCursorOnly", throwIfNotFound: true);
		m_Debug_StopCursorOnly = m_Debug.FindAction("StopCursorOnly", throwIfNotFound: true);
		m_MozaHelp = asset.FindActionMap("MozaHelp", throwIfNotFound: true);
		m_MozaHelp_xsouth = m_MozaHelp.FindAction("x-south", throwIfNotFound: true);
		m_MozaHelp_beast = m_MozaHelp.FindAction("b-east", throwIfNotFound: true);
		m_MozaHelp_bwest = m_MozaHelp.FindAction("b-west", throwIfNotFound: true);
		m_MozaHelp_bnorth = m_MozaHelp.FindAction("b-north", throwIfNotFound: true);
		m_MozaHelp_dright = m_MozaHelp.FindAction("d-right", throwIfNotFound: true);
		m_MozaHelp_dleft = m_MozaHelp.FindAction("d-left", throwIfNotFound: true);
		m_MozaHelp_ddown = m_MozaHelp.FindAction("d-down", throwIfNotFound: true);
		m_MozaHelp_dup = m_MozaHelp.FindAction("d-up", throwIfNotFound: true);
		m_MozaHelp_menu = m_MozaHelp.FindAction("menu", throwIfNotFound: true);
		m_MozaHelp_home = m_MozaHelp.FindAction("home", throwIfNotFound: true);
		m_MozaHelp_START = m_MozaHelp.FindAction("START", throwIfNotFound: true);
		m_MozaHelp_radio = m_MozaHelp.FindAction("radio", throwIfNotFound: true);
		m_MozaHelp_cam = m_MozaHelp.FindAction("cam", throwIfNotFound: true);
		m_MozaHelp_WN = m_MozaHelp.FindAction("W-N", throwIfNotFound: true);
		m_MozaHelp_WIP = m_MozaHelp.FindAction("WIP", throwIfNotFound: true);
		m_MozaHelp_WP = m_MozaHelp.FindAction("W-P", throwIfNotFound: true);
		m_MozaHelp_BOX = m_MozaHelp.FindAction("BOX", throwIfNotFound: true);
		m_MozaHelp_WR = m_MozaHelp.FindAction("W-R", throwIfNotFound: true);
		m_MozaHelp_FL = m_MozaHelp.FindAction("FL", throwIfNotFound: true);
		m_MozaHelp_PL = m_MozaHelp.FindAction("PL", throwIfNotFound: true);
		m_MozaHelp_S1 = m_MozaHelp.FindAction("S1", throwIfNotFound: true);
		m_MozaHelp_S2 = m_MozaHelp.FindAction("S2", throwIfNotFound: true);
		m_MozaHelp_leftwing = m_MozaHelp.FindAction("left-wing", throwIfNotFound: true);
		m_MozaHelp_rightwing = m_MozaHelp.FindAction("right-wing", throwIfNotFound: true);
		m_MozaHelp_wheel = m_MozaHelp.FindAction("wheel", throwIfNotFound: true);
		m_MozaHelp_throttle = m_MozaHelp.FindAction("throttle", throwIfNotFound: true);
		m_MozaHelp_break = m_MozaHelp.FindAction("break", throwIfNotFound: true);
		m_MozaHelp_clutch = m_MozaHelp.FindAction("clutch", throwIfNotFound: true);
		m_MozaHelp_gear1 = m_MozaHelp.FindAction("gear-1", throwIfNotFound: true);
		m_MozaHelp_gear2 = m_MozaHelp.FindAction("gear-2", throwIfNotFound: true);
		m_MozaHelp_gear3 = m_MozaHelp.FindAction("gear-3", throwIfNotFound: true);
		m_MozaHelp_gear4 = m_MozaHelp.FindAction("gear-4", throwIfNotFound: true);
		m_MozaHelp_gear5 = m_MozaHelp.FindAction("gear-5", throwIfNotFound: true);
		m_MozaHelp_gear6 = m_MozaHelp.FindAction("gear-6", throwIfNotFound: true);
		m_MozaHelp_gear7 = m_MozaHelp.FindAction("gear-7", throwIfNotFound: true);
		m_MozaHelp_gearR = m_MozaHelp.FindAction("gear-R", throwIfNotFound: true);
		m_MozaHelp_gearN = m_MozaHelp.FindAction("gear-N", throwIfNotFound: true);
		m_MozaHelp_wheelLButton = m_MozaHelp.FindAction("wheelLButton", throwIfNotFound: true);
		m_MozaHelp_wheelRButton = m_MozaHelp.FindAction("wheelRButton", throwIfNotFound: true);
		m_MozaHelp_gearboxButton1 = m_MozaHelp.FindAction("gearboxButton1", throwIfNotFound: true);
		m_MozaHelp_gearboxButton2 = m_MozaHelp.FindAction("gearboxButton2", throwIfNotFound: true);
		m_MozaHelp_gearboxButton3 = m_MozaHelp.FindAction("gearboxButton3", throwIfNotFound: true);
		m_MozaHelp_gearboxButton4 = m_MozaHelp.FindAction("gearboxButton4", throwIfNotFound: true);
		m_MozaHelp_gearup = m_MozaHelp.FindAction("gearup", throwIfNotFound: true);
		m_MozaHelp_geardown = m_MozaHelp.FindAction("geardown", throwIfNotFound: true);
		m_MozaHelp_handbrake = m_MozaHelp.FindAction("handbrake", throwIfNotFound: true);
		m_Person = asset.FindActionMap("Person", throwIfNotFound: true);
		m_Person_Run = m_Person.FindAction("Run", throwIfNotFound: true);
		m_Person_Crouch = m_Person.FindAction("Crouch", throwIfNotFound: true);
		m_Person_Jump = m_Person.FindAction("Jump", throwIfNotFound: true);
		m_Person_WalkRunToggle = m_Person.FindAction("WalkRunToggle", throwIfNotFound: true);
	}

	~GameInputController()
	{
	}

	public void Dispose()
	{
		UnityEngine.Object.Destroy(asset);
	}

	public bool Contains(InputAction action)
	{
		return asset.Contains(action);
	}

	public IEnumerator<InputAction> GetEnumerator()
	{
		return asset.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Enable()
	{
		asset.Enable();
	}

	public void Disable()
	{
		asset.Disable();
	}

	public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
	{
		return asset.FindAction(actionNameOrId, throwIfNotFound);
	}

	public int FindBinding(InputBinding bindingMask, out InputAction action)
	{
		return asset.FindBinding(bindingMask, out action);
	}
}
