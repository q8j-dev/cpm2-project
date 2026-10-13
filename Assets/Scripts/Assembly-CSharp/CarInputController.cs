using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class CarInputController : IInputActionCollection2, IInputActionCollection, IEnumerable<InputAction>, IEnumerable, IDisposable
{
	public struct PersonActions
	{
		private CarInputController m_Wrapper;

		public InputAction LeftRight => m_Wrapper.m_Person_LeftRight;

		public InputAction Move => m_Wrapper.m_Person_Move;

		public InputAction Stop => m_Wrapper.m_Person_Stop;

		public bool enabled => Get().enabled;

		public PersonActions(CarInputController wrapper)
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
				LeftRight.started += instance.OnLeftRight;
				LeftRight.performed += instance.OnLeftRight;
				LeftRight.canceled += instance.OnLeftRight;
				Move.started += instance.OnMove;
				Move.performed += instance.OnMove;
				Move.canceled += instance.OnMove;
				Stop.started += instance.OnStop;
				Stop.performed += instance.OnStop;
				Stop.canceled += instance.OnStop;
			}
		}

		private void UnregisterCallbacks(IPersonActions instance)
		{
			LeftRight.started -= instance.OnLeftRight;
			LeftRight.performed -= instance.OnLeftRight;
			LeftRight.canceled -= instance.OnLeftRight;
			Move.started -= instance.OnMove;
			Move.performed -= instance.OnMove;
			Move.canceled -= instance.OnMove;
			Stop.started -= instance.OnStop;
			Stop.performed -= instance.OnStop;
			Stop.canceled -= instance.OnStop;
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

	public struct CarActions
	{
		private CarInputController m_Wrapper;

		public InputAction Steering => m_Wrapper.m_Car_Steering;

		public InputAction Throttle => m_Wrapper.m_Car_Throttle;

		public InputAction Brake => m_Wrapper.m_Car_Brake;

		public InputAction Clutch => m_Wrapper.m_Car_Clutch;

		public InputAction Handbrake => m_Wrapper.m_Car_Handbrake;

		public InputAction StartEngine => m_Wrapper.m_Car_StartEngine;

		public InputAction Horn => m_Wrapper.m_Car_Horn;

		public InputAction LeftSignal => m_Wrapper.m_Car_LeftSignal;

		public InputAction RightSignal => m_Wrapper.m_Car_RightSignal;

		public InputAction EmergencySignal => m_Wrapper.m_Car_EmergencySignal;

		public InputAction Headlights => m_Wrapper.m_Car_Headlights;

		public InputAction ChangeCamera => m_Wrapper.m_Car_ChangeCamera;

		public InputAction OpenMap => m_Wrapper.m_Car_OpenMap;

		public InputAction ShiftUp => m_Wrapper.m_Car_ShiftUp;

		public InputAction ShiftDown => m_Wrapper.m_Car_ShiftDown;

		public InputAction Gear1 => m_Wrapper.m_Car_Gear1;

		public InputAction Gear2 => m_Wrapper.m_Car_Gear2;

		public InputAction Gear3 => m_Wrapper.m_Car_Gear3;

		public InputAction Gear4 => m_Wrapper.m_Car_Gear4;

		public InputAction Gear5 => m_Wrapper.m_Car_Gear5;

		public InputAction Gear6 => m_Wrapper.m_Car_Gear6;

		public InputAction Gear7 => m_Wrapper.m_Car_Gear7;

		public InputAction GearR => m_Wrapper.m_Car_GearR;

		public InputAction GearN => m_Wrapper.m_Car_GearN;

		public bool enabled => Get().enabled;

		public CarActions(CarInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Car;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(CarActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ICarActions instance)
		{
			if (instance != null && !m_Wrapper.m_CarActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_CarActionsCallbackInterfaces.Add(instance);
				Steering.started += instance.OnSteering;
				Steering.performed += instance.OnSteering;
				Steering.canceled += instance.OnSteering;
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
				StartEngine.started += instance.OnStartEngine;
				StartEngine.performed += instance.OnStartEngine;
				StartEngine.canceled += instance.OnStartEngine;
				Horn.started += instance.OnHorn;
				Horn.performed += instance.OnHorn;
				Horn.canceled += instance.OnHorn;
				LeftSignal.started += instance.OnLeftSignal;
				LeftSignal.performed += instance.OnLeftSignal;
				LeftSignal.canceled += instance.OnLeftSignal;
				RightSignal.started += instance.OnRightSignal;
				RightSignal.performed += instance.OnRightSignal;
				RightSignal.canceled += instance.OnRightSignal;
				EmergencySignal.started += instance.OnEmergencySignal;
				EmergencySignal.performed += instance.OnEmergencySignal;
				EmergencySignal.canceled += instance.OnEmergencySignal;
				Headlights.started += instance.OnHeadlights;
				Headlights.performed += instance.OnHeadlights;
				Headlights.canceled += instance.OnHeadlights;
				ChangeCamera.started += instance.OnChangeCamera;
				ChangeCamera.performed += instance.OnChangeCamera;
				ChangeCamera.canceled += instance.OnChangeCamera;
				OpenMap.started += instance.OnOpenMap;
				OpenMap.performed += instance.OnOpenMap;
				OpenMap.canceled += instance.OnOpenMap;
				ShiftUp.started += instance.OnShiftUp;
				ShiftUp.performed += instance.OnShiftUp;
				ShiftUp.canceled += instance.OnShiftUp;
				ShiftDown.started += instance.OnShiftDown;
				ShiftDown.performed += instance.OnShiftDown;
				ShiftDown.canceled += instance.OnShiftDown;
				Gear1.started += instance.OnGear1;
				Gear1.performed += instance.OnGear1;
				Gear1.canceled += instance.OnGear1;
				Gear2.started += instance.OnGear2;
				Gear2.performed += instance.OnGear2;
				Gear2.canceled += instance.OnGear2;
				Gear3.started += instance.OnGear3;
				Gear3.performed += instance.OnGear3;
				Gear3.canceled += instance.OnGear3;
				Gear4.started += instance.OnGear4;
				Gear4.performed += instance.OnGear4;
				Gear4.canceled += instance.OnGear4;
				Gear5.started += instance.OnGear5;
				Gear5.performed += instance.OnGear5;
				Gear5.canceled += instance.OnGear5;
				Gear6.started += instance.OnGear6;
				Gear6.performed += instance.OnGear6;
				Gear6.canceled += instance.OnGear6;
				Gear7.started += instance.OnGear7;
				Gear7.performed += instance.OnGear7;
				Gear7.canceled += instance.OnGear7;
				GearR.started += instance.OnGearR;
				GearR.performed += instance.OnGearR;
				GearR.canceled += instance.OnGearR;
				GearN.started += instance.OnGearN;
				GearN.performed += instance.OnGearN;
				GearN.canceled += instance.OnGearN;
			}
		}

		private void UnregisterCallbacks(ICarActions instance)
		{
			Steering.started -= instance.OnSteering;
			Steering.performed -= instance.OnSteering;
			Steering.canceled -= instance.OnSteering;
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
			StartEngine.started -= instance.OnStartEngine;
			StartEngine.performed -= instance.OnStartEngine;
			StartEngine.canceled -= instance.OnStartEngine;
			Horn.started -= instance.OnHorn;
			Horn.performed -= instance.OnHorn;
			Horn.canceled -= instance.OnHorn;
			LeftSignal.started -= instance.OnLeftSignal;
			LeftSignal.performed -= instance.OnLeftSignal;
			LeftSignal.canceled -= instance.OnLeftSignal;
			RightSignal.started -= instance.OnRightSignal;
			RightSignal.performed -= instance.OnRightSignal;
			RightSignal.canceled -= instance.OnRightSignal;
			EmergencySignal.started -= instance.OnEmergencySignal;
			EmergencySignal.performed -= instance.OnEmergencySignal;
			EmergencySignal.canceled -= instance.OnEmergencySignal;
			Headlights.started -= instance.OnHeadlights;
			Headlights.performed -= instance.OnHeadlights;
			Headlights.canceled -= instance.OnHeadlights;
			ChangeCamera.started -= instance.OnChangeCamera;
			ChangeCamera.performed -= instance.OnChangeCamera;
			ChangeCamera.canceled -= instance.OnChangeCamera;
			OpenMap.started -= instance.OnOpenMap;
			OpenMap.performed -= instance.OnOpenMap;
			OpenMap.canceled -= instance.OnOpenMap;
			ShiftUp.started -= instance.OnShiftUp;
			ShiftUp.performed -= instance.OnShiftUp;
			ShiftUp.canceled -= instance.OnShiftUp;
			ShiftDown.started -= instance.OnShiftDown;
			ShiftDown.performed -= instance.OnShiftDown;
			ShiftDown.canceled -= instance.OnShiftDown;
			Gear1.started -= instance.OnGear1;
			Gear1.performed -= instance.OnGear1;
			Gear1.canceled -= instance.OnGear1;
			Gear2.started -= instance.OnGear2;
			Gear2.performed -= instance.OnGear2;
			Gear2.canceled -= instance.OnGear2;
			Gear3.started -= instance.OnGear3;
			Gear3.performed -= instance.OnGear3;
			Gear3.canceled -= instance.OnGear3;
			Gear4.started -= instance.OnGear4;
			Gear4.performed -= instance.OnGear4;
			Gear4.canceled -= instance.OnGear4;
			Gear5.started -= instance.OnGear5;
			Gear5.performed -= instance.OnGear5;
			Gear5.canceled -= instance.OnGear5;
			Gear6.started -= instance.OnGear6;
			Gear6.performed -= instance.OnGear6;
			Gear6.canceled -= instance.OnGear6;
			Gear7.started -= instance.OnGear7;
			Gear7.performed -= instance.OnGear7;
			Gear7.canceled -= instance.OnGear7;
			GearR.started -= instance.OnGearR;
			GearR.performed -= instance.OnGearR;
			GearR.canceled -= instance.OnGearR;
			GearN.started -= instance.OnGearN;
			GearN.performed -= instance.OnGearN;
			GearN.canceled -= instance.OnGearN;
		}

		public void RemoveCallbacks(ICarActions instance)
		{
			if (m_Wrapper.m_CarActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ICarActions instance)
		{
			foreach (ICarActions carActionsCallbackInterface in m_Wrapper.m_CarActionsCallbackInterfaces)
			{
				UnregisterCallbacks(carActionsCallbackInterface);
			}
			m_Wrapper.m_CarActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct SteeringWheelActions
	{
		private CarInputController m_Wrapper;

		public InputAction Steering => m_Wrapper.m_SteeringWheel_Steering;

		public InputAction Throttle => m_Wrapper.m_SteeringWheel_Throttle;

		public InputAction Brake => m_Wrapper.m_SteeringWheel_Brake;

		public InputAction Clutch => m_Wrapper.m_SteeringWheel_Clutch;

		public InputAction Handbrake => m_Wrapper.m_SteeringWheel_Handbrake;

		public InputAction StartEngine => m_Wrapper.m_SteeringWheel_StartEngine;

		public InputAction Horn => m_Wrapper.m_SteeringWheel_Horn;

		public InputAction LeftSignal => m_Wrapper.m_SteeringWheel_LeftSignal;

		public InputAction RightSignal => m_Wrapper.m_SteeringWheel_RightSignal;

		public InputAction EmergencySignal => m_Wrapper.m_SteeringWheel_EmergencySignal;

		public InputAction Headlights => m_Wrapper.m_SteeringWheel_Headlights;

		public InputAction ChangeCamera => m_Wrapper.m_SteeringWheel_ChangeCamera;

		public InputAction OpenMap => m_Wrapper.m_SteeringWheel_OpenMap;

		public InputAction ShiftUp => m_Wrapper.m_SteeringWheel_ShiftUp;

		public InputAction ShiftDown => m_Wrapper.m_SteeringWheel_ShiftDown;

		public InputAction Gear1 => m_Wrapper.m_SteeringWheel_Gear1;

		public InputAction Gear2 => m_Wrapper.m_SteeringWheel_Gear2;

		public InputAction Gear3 => m_Wrapper.m_SteeringWheel_Gear3;

		public InputAction Gear4 => m_Wrapper.m_SteeringWheel_Gear4;

		public InputAction Gear5 => m_Wrapper.m_SteeringWheel_Gear5;

		public InputAction Gear6 => m_Wrapper.m_SteeringWheel_Gear6;

		public InputAction Gear7 => m_Wrapper.m_SteeringWheel_Gear7;

		public InputAction GearR => m_Wrapper.m_SteeringWheel_GearR;

		public InputAction GearN => m_Wrapper.m_SteeringWheel_GearN;

		public bool enabled => Get().enabled;

		public SteeringWheelActions(CarInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_SteeringWheel;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(SteeringWheelActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ISteeringWheelActions instance)
		{
			if (instance != null && !m_Wrapper.m_SteeringWheelActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_SteeringWheelActionsCallbackInterfaces.Add(instance);
				Steering.started += instance.OnSteering;
				Steering.performed += instance.OnSteering;
				Steering.canceled += instance.OnSteering;
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
				StartEngine.started += instance.OnStartEngine;
				StartEngine.performed += instance.OnStartEngine;
				StartEngine.canceled += instance.OnStartEngine;
				Horn.started += instance.OnHorn;
				Horn.performed += instance.OnHorn;
				Horn.canceled += instance.OnHorn;
				LeftSignal.started += instance.OnLeftSignal;
				LeftSignal.performed += instance.OnLeftSignal;
				LeftSignal.canceled += instance.OnLeftSignal;
				RightSignal.started += instance.OnRightSignal;
				RightSignal.performed += instance.OnRightSignal;
				RightSignal.canceled += instance.OnRightSignal;
				EmergencySignal.started += instance.OnEmergencySignal;
				EmergencySignal.performed += instance.OnEmergencySignal;
				EmergencySignal.canceled += instance.OnEmergencySignal;
				Headlights.started += instance.OnHeadlights;
				Headlights.performed += instance.OnHeadlights;
				Headlights.canceled += instance.OnHeadlights;
				ChangeCamera.started += instance.OnChangeCamera;
				ChangeCamera.performed += instance.OnChangeCamera;
				ChangeCamera.canceled += instance.OnChangeCamera;
				OpenMap.started += instance.OnOpenMap;
				OpenMap.performed += instance.OnOpenMap;
				OpenMap.canceled += instance.OnOpenMap;
				ShiftUp.started += instance.OnShiftUp;
				ShiftUp.performed += instance.OnShiftUp;
				ShiftUp.canceled += instance.OnShiftUp;
				ShiftDown.started += instance.OnShiftDown;
				ShiftDown.performed += instance.OnShiftDown;
				ShiftDown.canceled += instance.OnShiftDown;
				Gear1.started += instance.OnGear1;
				Gear1.performed += instance.OnGear1;
				Gear1.canceled += instance.OnGear1;
				Gear2.started += instance.OnGear2;
				Gear2.performed += instance.OnGear2;
				Gear2.canceled += instance.OnGear2;
				Gear3.started += instance.OnGear3;
				Gear3.performed += instance.OnGear3;
				Gear3.canceled += instance.OnGear3;
				Gear4.started += instance.OnGear4;
				Gear4.performed += instance.OnGear4;
				Gear4.canceled += instance.OnGear4;
				Gear5.started += instance.OnGear5;
				Gear5.performed += instance.OnGear5;
				Gear5.canceled += instance.OnGear5;
				Gear6.started += instance.OnGear6;
				Gear6.performed += instance.OnGear6;
				Gear6.canceled += instance.OnGear6;
				Gear7.started += instance.OnGear7;
				Gear7.performed += instance.OnGear7;
				Gear7.canceled += instance.OnGear7;
				GearR.started += instance.OnGearR;
				GearR.performed += instance.OnGearR;
				GearR.canceled += instance.OnGearR;
				GearN.started += instance.OnGearN;
				GearN.performed += instance.OnGearN;
				GearN.canceled += instance.OnGearN;
			}
		}

		private void UnregisterCallbacks(ISteeringWheelActions instance)
		{
			Steering.started -= instance.OnSteering;
			Steering.performed -= instance.OnSteering;
			Steering.canceled -= instance.OnSteering;
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
			StartEngine.started -= instance.OnStartEngine;
			StartEngine.performed -= instance.OnStartEngine;
			StartEngine.canceled -= instance.OnStartEngine;
			Horn.started -= instance.OnHorn;
			Horn.performed -= instance.OnHorn;
			Horn.canceled -= instance.OnHorn;
			LeftSignal.started -= instance.OnLeftSignal;
			LeftSignal.performed -= instance.OnLeftSignal;
			LeftSignal.canceled -= instance.OnLeftSignal;
			RightSignal.started -= instance.OnRightSignal;
			RightSignal.performed -= instance.OnRightSignal;
			RightSignal.canceled -= instance.OnRightSignal;
			EmergencySignal.started -= instance.OnEmergencySignal;
			EmergencySignal.performed -= instance.OnEmergencySignal;
			EmergencySignal.canceled -= instance.OnEmergencySignal;
			Headlights.started -= instance.OnHeadlights;
			Headlights.performed -= instance.OnHeadlights;
			Headlights.canceled -= instance.OnHeadlights;
			ChangeCamera.started -= instance.OnChangeCamera;
			ChangeCamera.performed -= instance.OnChangeCamera;
			ChangeCamera.canceled -= instance.OnChangeCamera;
			OpenMap.started -= instance.OnOpenMap;
			OpenMap.performed -= instance.OnOpenMap;
			OpenMap.canceled -= instance.OnOpenMap;
			ShiftUp.started -= instance.OnShiftUp;
			ShiftUp.performed -= instance.OnShiftUp;
			ShiftUp.canceled -= instance.OnShiftUp;
			ShiftDown.started -= instance.OnShiftDown;
			ShiftDown.performed -= instance.OnShiftDown;
			ShiftDown.canceled -= instance.OnShiftDown;
			Gear1.started -= instance.OnGear1;
			Gear1.performed -= instance.OnGear1;
			Gear1.canceled -= instance.OnGear1;
			Gear2.started -= instance.OnGear2;
			Gear2.performed -= instance.OnGear2;
			Gear2.canceled -= instance.OnGear2;
			Gear3.started -= instance.OnGear3;
			Gear3.performed -= instance.OnGear3;
			Gear3.canceled -= instance.OnGear3;
			Gear4.started -= instance.OnGear4;
			Gear4.performed -= instance.OnGear4;
			Gear4.canceled -= instance.OnGear4;
			Gear5.started -= instance.OnGear5;
			Gear5.performed -= instance.OnGear5;
			Gear5.canceled -= instance.OnGear5;
			Gear6.started -= instance.OnGear6;
			Gear6.performed -= instance.OnGear6;
			Gear6.canceled -= instance.OnGear6;
			Gear7.started -= instance.OnGear7;
			Gear7.performed -= instance.OnGear7;
			Gear7.canceled -= instance.OnGear7;
			GearR.started -= instance.OnGearR;
			GearR.performed -= instance.OnGearR;
			GearR.canceled -= instance.OnGearR;
			GearN.started -= instance.OnGearN;
			GearN.performed -= instance.OnGearN;
			GearN.canceled -= instance.OnGearN;
		}

		public void RemoveCallbacks(ISteeringWheelActions instance)
		{
			if (m_Wrapper.m_SteeringWheelActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ISteeringWheelActions instance)
		{
			foreach (ISteeringWheelActions steeringWheelActionsCallbackInterface in m_Wrapper.m_SteeringWheelActionsCallbackInterfaces)
			{
				UnregisterCallbacks(steeringWheelActionsCallbackInterface);
			}
			m_Wrapper.m_SteeringWheelActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct TouchWheelActions
	{
		private CarInputController m_Wrapper;

		public InputAction Steering => m_Wrapper.m_TouchWheel_Steering;

		public InputAction Throttle => m_Wrapper.m_TouchWheel_Throttle;

		public InputAction Brake => m_Wrapper.m_TouchWheel_Brake;

		public InputAction Clutch => m_Wrapper.m_TouchWheel_Clutch;

		public InputAction Handbrake => m_Wrapper.m_TouchWheel_Handbrake;

		public bool enabled => Get().enabled;

		public TouchWheelActions(CarInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_TouchWheel;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(TouchWheelActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ITouchWheelActions instance)
		{
			if (instance != null && !m_Wrapper.m_TouchWheelActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_TouchWheelActionsCallbackInterfaces.Add(instance);
				Steering.started += instance.OnSteering;
				Steering.performed += instance.OnSteering;
				Steering.canceled += instance.OnSteering;
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
			}
		}

		private void UnregisterCallbacks(ITouchWheelActions instance)
		{
			Steering.started -= instance.OnSteering;
			Steering.performed -= instance.OnSteering;
			Steering.canceled -= instance.OnSteering;
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
		}

		public void RemoveCallbacks(ITouchWheelActions instance)
		{
			if (m_Wrapper.m_TouchWheelActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ITouchWheelActions instance)
		{
			foreach (ITouchWheelActions touchWheelActionsCallbackInterface in m_Wrapper.m_TouchWheelActionsCallbackInterfaces)
			{
				UnregisterCallbacks(touchWheelActionsCallbackInterface);
			}
			m_Wrapper.m_TouchWheelActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct TiltActions
	{
		private CarInputController m_Wrapper;

		public InputAction Steering => m_Wrapper.m_Tilt_Steering;

		public InputAction Throttle => m_Wrapper.m_Tilt_Throttle;

		public InputAction Brake => m_Wrapper.m_Tilt_Brake;

		public InputAction Clutch => m_Wrapper.m_Tilt_Clutch;

		public InputAction Handbrake => m_Wrapper.m_Tilt_Handbrake;

		public bool enabled => Get().enabled;

		public TiltActions(CarInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_Tilt;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(TiltActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(ITiltActions instance)
		{
			if (instance != null && !m_Wrapper.m_TiltActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_TiltActionsCallbackInterfaces.Add(instance);
				Steering.started += instance.OnSteering;
				Steering.performed += instance.OnSteering;
				Steering.canceled += instance.OnSteering;
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
			}
		}

		private void UnregisterCallbacks(ITiltActions instance)
		{
			Steering.started -= instance.OnSteering;
			Steering.performed -= instance.OnSteering;
			Steering.canceled -= instance.OnSteering;
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
		}

		public void RemoveCallbacks(ITiltActions instance)
		{
			if (m_Wrapper.m_TiltActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(ITiltActions instance)
		{
			foreach (ITiltActions tiltActionsCallbackInterface in m_Wrapper.m_TiltActionsCallbackInterfaces)
			{
				UnregisterCallbacks(tiltActionsCallbackInterface);
			}
			m_Wrapper.m_TiltActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public struct UIActions
	{
		private CarInputController m_Wrapper;

		public InputAction Navigate => m_Wrapper.m_UI_Navigate;

		public InputAction Submit => m_Wrapper.m_UI_Submit;

		public InputAction Cancel => m_Wrapper.m_UI_Cancel;

		public InputAction Point => m_Wrapper.m_UI_Point;

		public InputAction Click => m_Wrapper.m_UI_Click;

		public InputAction RightClick => m_Wrapper.m_UI_RightClick;

		public InputAction MiddleClick => m_Wrapper.m_UI_MiddleClick;

		public InputAction ScrollWheel => m_Wrapper.m_UI_ScrollWheel;

		public InputAction TrackedDevicePosition => m_Wrapper.m_UI_TrackedDevicePosition;

		public InputAction TrackedDeviceOrientation => m_Wrapper.m_UI_TrackedDeviceOrientation;

		public bool enabled => Get().enabled;

		public UIActions(CarInputController wrapper)
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
				Submit.started += instance.OnSubmit;
				Submit.performed += instance.OnSubmit;
				Submit.canceled += instance.OnSubmit;
				Cancel.started += instance.OnCancel;
				Cancel.performed += instance.OnCancel;
				Cancel.canceled += instance.OnCancel;
				Point.started += instance.OnPoint;
				Point.performed += instance.OnPoint;
				Point.canceled += instance.OnPoint;
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
			}
		}

		private void UnregisterCallbacks(IUIActions instance)
		{
			Navigate.started -= instance.OnNavigate;
			Navigate.performed -= instance.OnNavigate;
			Navigate.canceled -= instance.OnNavigate;
			Submit.started -= instance.OnSubmit;
			Submit.performed -= instance.OnSubmit;
			Submit.canceled -= instance.OnSubmit;
			Cancel.started -= instance.OnCancel;
			Cancel.performed -= instance.OnCancel;
			Cancel.canceled -= instance.OnCancel;
			Point.started -= instance.OnPoint;
			Point.performed -= instance.OnPoint;
			Point.canceled -= instance.OnPoint;
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

	public struct GamepadExtrasActions
	{
		private CarInputController m_Wrapper;

		public InputAction Pause => m_Wrapper.m_GamepadExtras_Pause;

		public InputAction Select => m_Wrapper.m_GamepadExtras_Select;

		public InputAction ManualAutoToggle => m_Wrapper.m_GamepadExtras_ManualAutoToggle;

		public InputAction ToRoad => m_Wrapper.m_GamepadExtras_ToRoad;

		public InputAction Door => m_Wrapper.m_GamepadExtras_Door;

		public InputAction BigMap => m_Wrapper.m_GamepadExtras_BigMap;

		public InputAction SelectorUp => m_Wrapper.m_GamepadExtras_SelectorUp;

		public InputAction SelectorDown => m_Wrapper.m_GamepadExtras_SelectorDown;

		public InputAction ShifterStick => m_Wrapper.m_GamepadExtras_ShifterStick;

		public InputAction MoveStick => m_Wrapper.m_GamepadExtras_MoveStick;

		public InputAction LeftIndicator => m_Wrapper.m_GamepadExtras_LeftIndicator;

		public InputAction RightIndicator => m_Wrapper.m_GamepadExtras_RightIndicator;

		public InputAction Hazard => m_Wrapper.m_GamepadExtras_Hazard;

		public InputAction Headlights => m_Wrapper.m_GamepadExtras_Headlights;

		public InputAction Horn => m_Wrapper.m_GamepadExtras_Horn;

		public InputAction ChangeCamera => m_Wrapper.m_GamepadExtras_ChangeCamera;

		public InputAction Jump => m_Wrapper.m_GamepadExtras_Jump;

		public bool enabled => Get().enabled;

		public GamepadExtrasActions(CarInputController wrapper)
		{
			m_Wrapper = wrapper;
		}

		public InputActionMap Get()
		{
			return m_Wrapper.m_GamepadExtras;
		}

		public void Enable()
		{
			Get().Enable();
		}

		public void Disable()
		{
			Get().Disable();
		}

		public static implicit operator InputActionMap(GamepadExtrasActions set)
		{
			return set.Get();
		}

		public void AddCallbacks(IGamepadExtrasActions instance)
		{
			if (instance != null && !m_Wrapper.m_GamepadExtrasActionsCallbackInterfaces.Contains(instance))
			{
				m_Wrapper.m_GamepadExtrasActionsCallbackInterfaces.Add(instance);
				Pause.started += instance.OnPause;
				Pause.performed += instance.OnPause;
				Pause.canceled += instance.OnPause;
				Select.started += instance.OnSelect;
				Select.performed += instance.OnSelect;
				Select.canceled += instance.OnSelect;
				ManualAutoToggle.started += instance.OnManualAutoToggle;
				ManualAutoToggle.performed += instance.OnManualAutoToggle;
				ManualAutoToggle.canceled += instance.OnManualAutoToggle;
				ToRoad.started += instance.OnToRoad;
				ToRoad.performed += instance.OnToRoad;
				ToRoad.canceled += instance.OnToRoad;
				Door.started += instance.OnDoor;
				Door.performed += instance.OnDoor;
				Door.canceled += instance.OnDoor;
				BigMap.started += instance.OnBigMap;
				BigMap.performed += instance.OnBigMap;
				BigMap.canceled += instance.OnBigMap;
				SelectorUp.started += instance.OnSelectorUp;
				SelectorUp.performed += instance.OnSelectorUp;
				SelectorUp.canceled += instance.OnSelectorUp;
				SelectorDown.started += instance.OnSelectorDown;
				SelectorDown.performed += instance.OnSelectorDown;
				SelectorDown.canceled += instance.OnSelectorDown;
				ShifterStick.started += instance.OnShifterStick;
				ShifterStick.performed += instance.OnShifterStick;
				ShifterStick.canceled += instance.OnShifterStick;
				MoveStick.started += instance.OnMoveStick;
				MoveStick.performed += instance.OnMoveStick;
				MoveStick.canceled += instance.OnMoveStick;
				LeftIndicator.started += instance.OnLeftIndicator;
				LeftIndicator.performed += instance.OnLeftIndicator;
				LeftIndicator.canceled += instance.OnLeftIndicator;
				RightIndicator.started += instance.OnRightIndicator;
				RightIndicator.performed += instance.OnRightIndicator;
				RightIndicator.canceled += instance.OnRightIndicator;
				Hazard.started += instance.OnHazard;
				Hazard.performed += instance.OnHazard;
				Hazard.canceled += instance.OnHazard;
				Headlights.started += instance.OnHeadlights;
				Headlights.performed += instance.OnHeadlights;
				Headlights.canceled += instance.OnHeadlights;
				Horn.started += instance.OnHorn;
				Horn.performed += instance.OnHorn;
				Horn.canceled += instance.OnHorn;
				ChangeCamera.started += instance.OnChangeCamera;
				ChangeCamera.performed += instance.OnChangeCamera;
				ChangeCamera.canceled += instance.OnChangeCamera;
				Jump.started += instance.OnJump;
				Jump.performed += instance.OnJump;
				Jump.canceled += instance.OnJump;
			}
		}

		private void UnregisterCallbacks(IGamepadExtrasActions instance)
		{
			Pause.started -= instance.OnPause;
			Pause.performed -= instance.OnPause;
			Pause.canceled -= instance.OnPause;
			Select.started -= instance.OnSelect;
			Select.performed -= instance.OnSelect;
			Select.canceled -= instance.OnSelect;
			ManualAutoToggle.started -= instance.OnManualAutoToggle;
			ManualAutoToggle.performed -= instance.OnManualAutoToggle;
			ManualAutoToggle.canceled -= instance.OnManualAutoToggle;
			ToRoad.started -= instance.OnToRoad;
			ToRoad.performed -= instance.OnToRoad;
			ToRoad.canceled -= instance.OnToRoad;
			Door.started -= instance.OnDoor;
			Door.performed -= instance.OnDoor;
			Door.canceled -= instance.OnDoor;
			BigMap.started -= instance.OnBigMap;
			BigMap.performed -= instance.OnBigMap;
			BigMap.canceled -= instance.OnBigMap;
			SelectorUp.started -= instance.OnSelectorUp;
			SelectorUp.performed -= instance.OnSelectorUp;
			SelectorUp.canceled -= instance.OnSelectorUp;
			SelectorDown.started -= instance.OnSelectorDown;
			SelectorDown.performed -= instance.OnSelectorDown;
			SelectorDown.canceled -= instance.OnSelectorDown;
			ShifterStick.started -= instance.OnShifterStick;
			ShifterStick.performed -= instance.OnShifterStick;
			ShifterStick.canceled -= instance.OnShifterStick;
			MoveStick.started -= instance.OnMoveStick;
			MoveStick.performed -= instance.OnMoveStick;
			MoveStick.canceled -= instance.OnMoveStick;
			LeftIndicator.started -= instance.OnLeftIndicator;
			LeftIndicator.performed -= instance.OnLeftIndicator;
			LeftIndicator.canceled -= instance.OnLeftIndicator;
			RightIndicator.started -= instance.OnRightIndicator;
			RightIndicator.performed -= instance.OnRightIndicator;
			RightIndicator.canceled -= instance.OnRightIndicator;
			Hazard.started -= instance.OnHazard;
			Hazard.performed -= instance.OnHazard;
			Hazard.canceled -= instance.OnHazard;
			Headlights.started -= instance.OnHeadlights;
			Headlights.performed -= instance.OnHeadlights;
			Headlights.canceled -= instance.OnHeadlights;
			Horn.started -= instance.OnHorn;
			Horn.performed -= instance.OnHorn;
			Horn.canceled -= instance.OnHorn;
			ChangeCamera.started -= instance.OnChangeCamera;
			ChangeCamera.performed -= instance.OnChangeCamera;
			ChangeCamera.canceled -= instance.OnChangeCamera;
			Jump.started -= instance.OnJump;
			Jump.performed -= instance.OnJump;
			Jump.canceled -= instance.OnJump;
		}

		public void RemoveCallbacks(IGamepadExtrasActions instance)
		{
			if (m_Wrapper.m_GamepadExtrasActionsCallbackInterfaces.Remove(instance))
			{
				UnregisterCallbacks(instance);
			}
		}

		public void SetCallbacks(IGamepadExtrasActions instance)
		{
			foreach (IGamepadExtrasActions gamepadExtrasActionsCallbackInterface in m_Wrapper.m_GamepadExtrasActionsCallbackInterfaces)
			{
				UnregisterCallbacks(gamepadExtrasActionsCallbackInterface);
			}
			m_Wrapper.m_GamepadExtrasActionsCallbackInterfaces.Clear();
			AddCallbacks(instance);
		}
	}

	public interface IPersonActions
	{
		void OnLeftRight(InputAction.CallbackContext context);

		void OnMove(InputAction.CallbackContext context);

		void OnStop(InputAction.CallbackContext context);
	}

	public interface ICarActions
	{
		void OnSteering(InputAction.CallbackContext context);

		void OnThrottle(InputAction.CallbackContext context);

		void OnBrake(InputAction.CallbackContext context);

		void OnClutch(InputAction.CallbackContext context);

		void OnHandbrake(InputAction.CallbackContext context);

		void OnStartEngine(InputAction.CallbackContext context);

		void OnHorn(InputAction.CallbackContext context);

		void OnLeftSignal(InputAction.CallbackContext context);

		void OnRightSignal(InputAction.CallbackContext context);

		void OnEmergencySignal(InputAction.CallbackContext context);

		void OnHeadlights(InputAction.CallbackContext context);

		void OnChangeCamera(InputAction.CallbackContext context);

		void OnOpenMap(InputAction.CallbackContext context);

		void OnShiftUp(InputAction.CallbackContext context);

		void OnShiftDown(InputAction.CallbackContext context);

		void OnGear1(InputAction.CallbackContext context);

		void OnGear2(InputAction.CallbackContext context);

		void OnGear3(InputAction.CallbackContext context);

		void OnGear4(InputAction.CallbackContext context);

		void OnGear5(InputAction.CallbackContext context);

		void OnGear6(InputAction.CallbackContext context);

		void OnGear7(InputAction.CallbackContext context);

		void OnGearR(InputAction.CallbackContext context);

		void OnGearN(InputAction.CallbackContext context);
	}

	public interface ISteeringWheelActions
	{
		void OnSteering(InputAction.CallbackContext context);

		void OnThrottle(InputAction.CallbackContext context);

		void OnBrake(InputAction.CallbackContext context);

		void OnClutch(InputAction.CallbackContext context);

		void OnHandbrake(InputAction.CallbackContext context);

		void OnStartEngine(InputAction.CallbackContext context);

		void OnHorn(InputAction.CallbackContext context);

		void OnLeftSignal(InputAction.CallbackContext context);

		void OnRightSignal(InputAction.CallbackContext context);

		void OnEmergencySignal(InputAction.CallbackContext context);

		void OnHeadlights(InputAction.CallbackContext context);

		void OnChangeCamera(InputAction.CallbackContext context);

		void OnOpenMap(InputAction.CallbackContext context);

		void OnShiftUp(InputAction.CallbackContext context);

		void OnShiftDown(InputAction.CallbackContext context);

		void OnGear1(InputAction.CallbackContext context);

		void OnGear2(InputAction.CallbackContext context);

		void OnGear3(InputAction.CallbackContext context);

		void OnGear4(InputAction.CallbackContext context);

		void OnGear5(InputAction.CallbackContext context);

		void OnGear6(InputAction.CallbackContext context);

		void OnGear7(InputAction.CallbackContext context);

		void OnGearR(InputAction.CallbackContext context);

		void OnGearN(InputAction.CallbackContext context);
	}

	public interface ITouchWheelActions
	{
		void OnSteering(InputAction.CallbackContext context);

		void OnThrottle(InputAction.CallbackContext context);

		void OnBrake(InputAction.CallbackContext context);

		void OnClutch(InputAction.CallbackContext context);

		void OnHandbrake(InputAction.CallbackContext context);
	}

	public interface ITiltActions
	{
		void OnSteering(InputAction.CallbackContext context);

		void OnThrottle(InputAction.CallbackContext context);

		void OnBrake(InputAction.CallbackContext context);

		void OnClutch(InputAction.CallbackContext context);

		void OnHandbrake(InputAction.CallbackContext context);
	}

	public interface IUIActions
	{
		void OnNavigate(InputAction.CallbackContext context);

		void OnSubmit(InputAction.CallbackContext context);

		void OnCancel(InputAction.CallbackContext context);

		void OnPoint(InputAction.CallbackContext context);

		void OnClick(InputAction.CallbackContext context);

		void OnRightClick(InputAction.CallbackContext context);

		void OnMiddleClick(InputAction.CallbackContext context);

		void OnScrollWheel(InputAction.CallbackContext context);

		void OnTrackedDevicePosition(InputAction.CallbackContext context);

		void OnTrackedDeviceOrientation(InputAction.CallbackContext context);
	}

	public interface IGamepadExtrasActions
	{
		void OnPause(InputAction.CallbackContext context);

		void OnSelect(InputAction.CallbackContext context);

		void OnManualAutoToggle(InputAction.CallbackContext context);

		void OnToRoad(InputAction.CallbackContext context);

		void OnDoor(InputAction.CallbackContext context);

		void OnBigMap(InputAction.CallbackContext context);

		void OnSelectorUp(InputAction.CallbackContext context);

		void OnSelectorDown(InputAction.CallbackContext context);

		void OnShifterStick(InputAction.CallbackContext context);

		void OnMoveStick(InputAction.CallbackContext context);

		void OnLeftIndicator(InputAction.CallbackContext context);

		void OnRightIndicator(InputAction.CallbackContext context);

		void OnHazard(InputAction.CallbackContext context);

		void OnHeadlights(InputAction.CallbackContext context);

		void OnHorn(InputAction.CallbackContext context);

		void OnChangeCamera(InputAction.CallbackContext context);

		void OnJump(InputAction.CallbackContext context);
	}

	private readonly InputActionMap m_Person;

	private List<IPersonActions> m_PersonActionsCallbackInterfaces = new List<IPersonActions>();

	private readonly InputAction m_Person_LeftRight;

	private readonly InputAction m_Person_Move;

	private readonly InputAction m_Person_Stop;

	private readonly InputActionMap m_Car;

	private List<ICarActions> m_CarActionsCallbackInterfaces = new List<ICarActions>();

	private readonly InputAction m_Car_Steering;

	private readonly InputAction m_Car_Throttle;

	private readonly InputAction m_Car_Brake;

	private readonly InputAction m_Car_Clutch;

	private readonly InputAction m_Car_Handbrake;

	private readonly InputAction m_Car_StartEngine;

	private readonly InputAction m_Car_Horn;

	private readonly InputAction m_Car_LeftSignal;

	private readonly InputAction m_Car_RightSignal;

	private readonly InputAction m_Car_EmergencySignal;

	private readonly InputAction m_Car_Headlights;

	private readonly InputAction m_Car_ChangeCamera;

	private readonly InputAction m_Car_OpenMap;

	private readonly InputAction m_Car_ShiftUp;

	private readonly InputAction m_Car_ShiftDown;

	private readonly InputAction m_Car_Gear1;

	private readonly InputAction m_Car_Gear2;

	private readonly InputAction m_Car_Gear3;

	private readonly InputAction m_Car_Gear4;

	private readonly InputAction m_Car_Gear5;

	private readonly InputAction m_Car_Gear6;

	private readonly InputAction m_Car_Gear7;

	private readonly InputAction m_Car_GearR;

	private readonly InputAction m_Car_GearN;

	private readonly InputActionMap m_SteeringWheel;

	private List<ISteeringWheelActions> m_SteeringWheelActionsCallbackInterfaces = new List<ISteeringWheelActions>();

	private readonly InputAction m_SteeringWheel_Steering;

	private readonly InputAction m_SteeringWheel_Throttle;

	private readonly InputAction m_SteeringWheel_Brake;

	private readonly InputAction m_SteeringWheel_Clutch;

	private readonly InputAction m_SteeringWheel_Handbrake;

	private readonly InputAction m_SteeringWheel_StartEngine;

	private readonly InputAction m_SteeringWheel_Horn;

	private readonly InputAction m_SteeringWheel_LeftSignal;

	private readonly InputAction m_SteeringWheel_RightSignal;

	private readonly InputAction m_SteeringWheel_EmergencySignal;

	private readonly InputAction m_SteeringWheel_Headlights;

	private readonly InputAction m_SteeringWheel_ChangeCamera;

	private readonly InputAction m_SteeringWheel_OpenMap;

	private readonly InputAction m_SteeringWheel_ShiftUp;

	private readonly InputAction m_SteeringWheel_ShiftDown;

	private readonly InputAction m_SteeringWheel_Gear1;

	private readonly InputAction m_SteeringWheel_Gear2;

	private readonly InputAction m_SteeringWheel_Gear3;

	private readonly InputAction m_SteeringWheel_Gear4;

	private readonly InputAction m_SteeringWheel_Gear5;

	private readonly InputAction m_SteeringWheel_Gear6;

	private readonly InputAction m_SteeringWheel_Gear7;

	private readonly InputAction m_SteeringWheel_GearR;

	private readonly InputAction m_SteeringWheel_GearN;

	private readonly InputActionMap m_TouchWheel;

	private List<ITouchWheelActions> m_TouchWheelActionsCallbackInterfaces = new List<ITouchWheelActions>();

	private readonly InputAction m_TouchWheel_Steering;

	private readonly InputAction m_TouchWheel_Throttle;

	private readonly InputAction m_TouchWheel_Brake;

	private readonly InputAction m_TouchWheel_Clutch;

	private readonly InputAction m_TouchWheel_Handbrake;

	private readonly InputActionMap m_Tilt;

	private List<ITiltActions> m_TiltActionsCallbackInterfaces = new List<ITiltActions>();

	private readonly InputAction m_Tilt_Steering;

	private readonly InputAction m_Tilt_Throttle;

	private readonly InputAction m_Tilt_Brake;

	private readonly InputAction m_Tilt_Clutch;

	private readonly InputAction m_Tilt_Handbrake;

	private readonly InputActionMap m_UI;

	private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();

	private readonly InputAction m_UI_Navigate;

	private readonly InputAction m_UI_Submit;

	private readonly InputAction m_UI_Cancel;

	private readonly InputAction m_UI_Point;

	private readonly InputAction m_UI_Click;

	private readonly InputAction m_UI_RightClick;

	private readonly InputAction m_UI_MiddleClick;

	private readonly InputAction m_UI_ScrollWheel;

	private readonly InputAction m_UI_TrackedDevicePosition;

	private readonly InputAction m_UI_TrackedDeviceOrientation;

	private readonly InputActionMap m_GamepadExtras;

	private List<IGamepadExtrasActions> m_GamepadExtrasActionsCallbackInterfaces = new List<IGamepadExtrasActions>();

	private readonly InputAction m_GamepadExtras_Pause;

	private readonly InputAction m_GamepadExtras_Select;

	private readonly InputAction m_GamepadExtras_ManualAutoToggle;

	private readonly InputAction m_GamepadExtras_ToRoad;

	private readonly InputAction m_GamepadExtras_Door;

	private readonly InputAction m_GamepadExtras_BigMap;

	private readonly InputAction m_GamepadExtras_SelectorUp;

	private readonly InputAction m_GamepadExtras_SelectorDown;

	private readonly InputAction m_GamepadExtras_ShifterStick;

	private readonly InputAction m_GamepadExtras_MoveStick;

	private readonly InputAction m_GamepadExtras_LeftIndicator;

	private readonly InputAction m_GamepadExtras_RightIndicator;

	private readonly InputAction m_GamepadExtras_Hazard;

	private readonly InputAction m_GamepadExtras_Headlights;

	private readonly InputAction m_GamepadExtras_Horn;

	private readonly InputAction m_GamepadExtras_ChangeCamera;

	private readonly InputAction m_GamepadExtras_Jump;

	private int m_CarSchemeIndex = -1;

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

	public PersonActions Person => new PersonActions(this);

	public CarActions Car => new CarActions(this);

	public SteeringWheelActions SteeringWheel => new SteeringWheelActions(this);

	public TouchWheelActions TouchWheel => new TouchWheelActions(this);

	public TiltActions Tilt => new TiltActions(this);

	public UIActions UI => new UIActions(this);

	public GamepadExtrasActions GamepadExtras => new GamepadExtrasActions(this);

	public InputControlScheme CarScheme
	{
		get
		{
			if (m_CarSchemeIndex == -1)
			{
				m_CarSchemeIndex = asset.FindControlSchemeIndex("Car");
			}
			return asset.controlSchemes[m_CarSchemeIndex];
		}
	}

	public CarInputController()
	{
		asset = InputActionAsset.FromJson("{\r\n    \"version\": 1,\r\n    \"name\": \"CarInputController\",\r\n    \"maps\": [\r\n        {\r\n            \"name\": \"Person\",\r\n            \"id\": \"ed724750-1d0a-4247-b5cc-935f822ddc69\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"LeftRight\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d5a52c44-da31-484b-9831-88203da5c852\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Move\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"5938f843-7cdc-4da9-ac8f-29c84ccf5df7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Stop\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"415e3380-0b60-496d-9f92-15a6be5eb934\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cf030823-417e-42dc-b874-49f202ad92ea\",\r\n                    \"path\": \"<Keyboard>/w\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Move\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"97e97719-c210-4fed-911e-f5cc6ce94b49\",\r\n                    \"path\": \"<Keyboard>/s\",\r\n                    \"interactions\": \"Hold\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Stop\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"1D Axis\",\r\n                    \"id\": \"17f58e43-f637-46d5-b2eb-37db75bc05eb\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"LeftRight\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"6fb304dc-e71c-46a2-b510-f3502807945c\",\r\n                    \"path\": \"<Keyboard>/a\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"LeftRight\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"afeb6f3a-e80e-4b34-865a-4f060ba2fc41\",\r\n                    \"path\": \"<Keyboard>/d\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"LeftRight\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Car\",\r\n            \"id\": \"bb8d867f-0ee1-4638-b98c-983638aa774e\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Steering\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e9ed8cac-f8ac-4efa-8434-6332afdd7b7a\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Throttle\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"764ae767-a9c8-4a8f-b244-f7e7ab03f87d\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Brake\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"62271fb7-d52e-4f09-979c-af681601fb9c\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Clutch\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"c34ea42f-ae6d-45e5-a566-15d479fb4954\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Handbrake\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"5dd7ed0b-35ae-4a4d-8490-c1b2b70d62cb\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Press\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"StartEngine\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"1e189df7-364c-4701-91e4-50181eb75420\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Press\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Horn\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"226976ea-fa19-473a-b5a5-084907afdde2\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Press\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"LeftSignal\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c82763be-f7c7-472a-8ac9-5b12b27ec443\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Press\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"RightSignal\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b8018bc2-fa79-4a1b-a624-be2ea1848ff0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Press\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"EmergencySignal\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fc31b07a-d4ac-47e1-9167-357c65682b62\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Press\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Headlights\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"93606928-5bda-456b-bb00-d45f490f43b6\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ChangeCamera\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"5079a0d1-7e79-45b9-9283-1bc29f3874cc\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Press\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"OpenMap\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"4d237a3a-83e4-4461-a88a-f6b066f98dee\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ShiftUp\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"1884bac2-7dcd-460d-8298-ee3abb229ec1\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Press\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ShiftDown\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c9a31dec-0bae-40fc-83b3-cec82f3a251d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"Press\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear1\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e4d5d550-0067-4865-ba65-5f44902e0050\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear2\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"22f5618d-79e4-4530-800c-174a9a1774ce\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear3\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6a2b7562-1d29-4953-93a7-b7dd4cfa55c7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear4\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"9dab1363-1e99-47da-876e-fe4ff10aa922\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear5\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6770c238-b599-465c-86ab-15e5a10ef9fe\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear6\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"2dd3ef70-bd6f-42c6-a93a-1ad36f86e4a9\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear7\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"5cf269c3-aa63-47a7-82cf-f70190a9ca52\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"GearR\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"22031177-8258-4b69-8875-cae42afad05e\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"GearN\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f02e484d-124d-4ea4-abb0-720e29d43928\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ad7e842e-ee69-4fd2-a061-a3aa6025ee3c\",\r\n                    \"path\": \"<Keyboard>/w\",\r\n                    \"interactions\": \"Press(pressPoint=0.001)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"33e4138b-eb26-4f53-b280-dc2ace1bf1c2\",\r\n                    \"path\": \"VirtualPad/throttle\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3f45c72c-aaa1-4f8d-8de2-7455d2dc7065\",\r\n                    \"path\": \"<Gamepad>/rightTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6ef97c7d-c04b-4ed4-bff9-86beb7584623\",\r\n                    \"path\": \"<HID::1949-402>/button10\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6ee67cc8-0fb5-4ee3-8edf-7bef30c1bb19\",\r\n                    \"path\": \"<Keyboard>/s\",\r\n                    \"interactions\": \"Press(pressPoint=0.001)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cb849f8d-87d2-4ee8-8048-e2c6b0f2cc3b\",\r\n                    \"path\": \"VirtualPad/brake\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"16655b86-86e1-4bec-8a58-efef1e456b48\",\r\n                    \"path\": \"<Gamepad>/leftTrigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9ca90672-1618-449d-bdea-9f268ce63ccf\",\r\n                    \"path\": \"<HID::1949-402>/button9\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"70a3fe4b-61d3-4078-92c0-f2e0a26cbef7\",\r\n                    \"path\": \"<Keyboard>/x\",\r\n                    \"interactions\": \"Press(pressPoint=0.001)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"49a263a1-b80a-4199-a29b-64e51ae295ae\",\r\n                    \"path\": \"VirtualPad/clutch\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c66d1ce5-81ea-488a-8b9a-dc0ccce3bf31\",\r\n                    \"path\": \"<Keyboard>/space\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8e7b0f28-2eab-46b0-b1c3-388f0a610679\",\r\n                    \"path\": \"VirtualPad/handbrake\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"64336385-3edc-4647-a4f7-3fe45982a122\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2febb281-9a0a-4aeb-b22b-10a40d85c2e8\",\r\n                    \"path\": \"<HID::1949-402>/button2\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e5b072a0-793f-43d1-aa65-26f976ba4887\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button32\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9c87e642-00ac-4930-9ead-35cb977f13c9\",\r\n                    \"path\": \"<Keyboard>/f\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"ShiftUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fa85d085-83a0-4499-8d46-02c67a9424d8\",\r\n                    \"path\": \"<Keyboard>/v\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"ShiftDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bb4572e7-7ade-41a7-9928-72ecbff0c42e\",\r\n                    \"path\": \"<Keyboard>/i\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"021288b8-14ad-4314-a795-54d26808536c\",\r\n                    \"path\": \"VirtualPad/starter\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6e52578c-cb87-40c7-a355-32060f64a94f\",\r\n                    \"path\": \"<HID::1949-402>/button4\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"42a5f066-f01e-4150-b05e-37c18cd937c0\",\r\n                    \"path\": \"<Gamepad>/buttonWest\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3d92d5ac-3efd-45f2-a71b-fb8ae442c507\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button36\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"76687343-dfcc-4955-a5d0-b8469f5364a5\",\r\n                    \"path\": \"<Keyboard>/h\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"463ca3ad-3c98-4c75-a379-5c73bea3a50f\",\r\n                    \"path\": \"VirtualPad/horn\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7f4e96c5-3f8d-4dc1-a023-c058a14457a5\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button23\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cbf6250f-726a-4702-a931-03d98e2bc7be\",\r\n                    \"path\": \"<Keyboard>/q\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"LeftSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0805e4c3-53a2-4868-afca-d5e504aed0ff\",\r\n                    \"path\": \"VirtualPad/leftSignal\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"LeftSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7cc36bc3-724d-4c79-b158-c5fcee12b006\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button8\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"LeftSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8dadff96-b83e-41fc-ad88-6e23bfc30a1f\",\r\n                    \"path\": \"<Keyboard>/e\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"RightSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"386a752a-5e96-4951-8ac4-4aa5057ad248\",\r\n                    \"path\": \"VirtualPad/rightSignal\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"RightSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5e77d155-2739-4141-b439-4270c75e06be\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"RightSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c1f4dc9e-c4fa-4737-9cf1-1158fe202f06\",\r\n                    \"path\": \"<Keyboard>/r\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"EmergencySignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f26e8f43-0630-42bf-859e-9fc42c160916\",\r\n                    \"path\": \"VirtualPad/emergencySignal\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"EmergencySignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"dbfd711f-5edd-4060-951d-4d18aa643251\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"EmergencySignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"513c7642-ce1c-4a23-872d-c00e2b6daff8\",\r\n                    \"path\": \"<Keyboard>/l\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"905dc4cd-61fa-42b3-bc15-64e88246c176\",\r\n                    \"path\": \"VirtualPad/headlights\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"180cffcc-ef41-4c8c-89b1-bd2b69374ab1\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button19\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"314c1655-8d51-4fd3-b3b4-5c101d2b1ce5\",\r\n                    \"path\": \"<Keyboard>/C\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"06bd1a3a-d0a3-4038-853a-034eef24a8e0\",\r\n                    \"path\": \"VirtualPad/changeCamera\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"af48bc61-eb0f-4197-b89d-115179e3cda3\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button22\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bf613c82-3c84-484e-b6ff-c5dfa8082e84\",\r\n                    \"path\": \"<Keyboard>/M\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"OpenMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"bb31d2e3-27e8-4316-8b47-8146b284efb5\",\r\n                    \"path\": \"VirtualPad/openMap\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"OpenMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"90866aea-51bf-405c-880f-e0114b017c8c\",\r\n                    \"path\": \"<HID::1949-402>/hat/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"OpenMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0750a2cb-e2a8-4f38-9dc9-01e91b1e2f36\",\r\n                    \"path\": \"<Gamepad>/start\",\r\n                    \"interactions\": \"Hold(duration=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"OpenMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8d16d280-e7b5-488a-b003-a954e3b3ab2e\",\r\n                    \"path\": \"<Keyboard>/1\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3531ae9f-1f0b-436e-bba5-12d10e1a2a85\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button114\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f9b30266-cbc9-4446-8832-fd8e2ef836c3\",\r\n                    \"path\": \"<Keyboard>/2\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ff28fbc6-3be8-4b1d-acc3-ef7168876286\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button115\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"190a72ef-c748-4990-a293-306d06591e3f\",\r\n                    \"path\": \"<Keyboard>/3\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"50458b22-e370-4c00-b53a-28cd672132ec\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button116\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5e118d9f-6bff-46ed-9309-a7c9d3e17efc\",\r\n                    \"path\": \"<Keyboard>/4\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"df10d464-ff94-4cd3-8261-9fecd832abca\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button117\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a71248eb-94be-44d7-8c41-c4f3ced812d6\",\r\n                    \"path\": \"<Keyboard>/5\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear5\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fbb34d59-f402-4714-a3bd-a75a58aab563\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button118\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear5\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"75c57370-2d62-4a9c-8ab5-2fa604137c59\",\r\n                    \"path\": \"<Keyboard>/6\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear6\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"dd07c02c-2562-480d-a748-75a94230b2f8\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button119\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear6\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"08f7240a-d852-4031-b627-af639fe40eee\",\r\n                    \"path\": \"<Keyboard>/7\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear7\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1178e26c-ba1a-4b00-93d9-930c88c5f221\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button120\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear7\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"56edbd46-f091-4cbd-ab6a-b1b01bb75e9c\",\r\n                    \"path\": \"<Keyboard>/9\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"GearR\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cd58e79b-70b7-4f86-b6fb-3fca0c2d2548\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button113\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"GearR\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e7eb0f00-1844-4363-96ac-07e99633ac94\",\r\n                    \"path\": \"<Keyboard>/0\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"GearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Keyboard\",\r\n                    \"id\": \"9ad22878-2894-47f4-9fd1-89d920b54a99\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"c7676a02-003d-439b-88f0-b1d6ab7e7c39\",\r\n                    \"path\": \"<Keyboard>/a\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Car\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"ddcf8707-5e78-41d0-ab70-6a64134739c9\",\r\n                    \"path\": \"<Keyboard>/d\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Car\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"GamePad\",\r\n                    \"id\": \"f3e29b91-9f37-4103-acc0-8fbf6515af96\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"da83f328-0239-455c-80d8-1d2b0fcc737c\",\r\n                    \"path\": \"<Gamepad>/leftStick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"421a8c7f-6c11-4d6d-bc7d-5f58e404e9d4\",\r\n                    \"path\": \"<Gamepad>/leftStick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Joystick\",\r\n                    \"id\": \"0de6cf20-d843-49be-8931-2b742249c74f\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Clamp(min=-1,max=1)\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"95e8cdc7-a587-463b-8d25-ba43a49a246d\",\r\n                    \"path\": \"<Joystick>/stick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"30340efa-9a99-4e4f-b20a-c6d13d581971\",\r\n                    \"path\": \"<Joystick>/stick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"SteeringWheel\",\r\n            \"id\": \"6e65cef2-1cc3-4bcb-98d5-a4812ba02429\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Steering\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"5e9e05d2-d1f0-4791-8df8-6567857d109d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Throttle\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"ab441cdf-9e41-4c1c-8702-dfc96a47e2d1\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Brake\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"39bd1a12-0c22-4b26-989f-06fba8371c03\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Clutch\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"3615314f-a24c-4427-b08a-7f9dd640075a\",\r\n                    \"expectedControlType\": \"Axis\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Handbrake\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b38ebab5-0123-4c46-9923-3e7d15fcd67c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"StartEngine\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f854a1b3-3be4-4491-978b-a2bbc4c81fbc\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Horn\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"edece8a7-ee11-46c5-80f7-94f4a1bfe07b\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"LeftSignal\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b155c382-bef5-4662-9eca-01c8f36a00da\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"RightSignal\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"fe4b93ec-7fbb-4857-a4d2-b61ca5be8c66\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"EmergencySignal\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"17fecd99-8331-4bd1-817f-349e22fd1bf9\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Headlights\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"5afa8df4-6532-41bd-9794-dd261bb814a5\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ChangeCamera\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"465a9837-f522-4c57-9fae-3101ebef3ab6\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"OpenMap\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"6ef17d68-c4fc-423b-9cbf-f488eda3a16c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"ShiftUp\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"82a865ca-1d2d-47bf-83af-2d51a579c6c0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ShiftDown\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"a7c070b5-d69b-4c23-96fa-3946394e26c6\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear1\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"8e1a0237-c4a8-405a-82c3-0f56c663ae68\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear2\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e23ac4f9-63d2-43f3-ba5e-09eaebedfe65\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear3\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"1650e313-d74b-4271-8dfa-1136850451bb\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear4\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"05c6e7b8-e95c-4e4d-9dc0-c39e00803c4a\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear5\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"3f1a0602-a295-477e-8117-2268626c0115\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear6\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"38d3570f-214d-4d29-80a7-df358ae1b732\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Gear7\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"8b2c375f-618d-44c9-a3e3-fb56cb1e32eb\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"GearR\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"88a29240-9fb5-4da4-ab0b-d55f4138365e\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"GearN\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"cf1aaf56-d1bf-4c51-953f-dde09a73533d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"447322be-a871-41ec-b28d-ee34395264e2\",\r\n                    \"path\": \"VirtualPad/throttle\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a2ecb37d-d282-4b19-a3de-8e7b7815303f\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/z\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"888a04d0-d5d8-460f-ab53-613414617152\",\r\n                    \"path\": \"VirtualPad/brake\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b62eeced-dc1f-4591-b082-3f65a06b6b41\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/rz\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8134f06b-e341-4460-9b03-a7e145cae266\",\r\n                    \"path\": \"VirtualPad/clutch\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Clamp(max=1)\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6a149816-d3ed-48ee-b702-7cbaae2e7ebd\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/slider\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2920afb5-558f-4b0e-a687-693dc83a246a\",\r\n                    \"path\": \"VirtualPad/handbrake\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"33b6b608-b514-44e3-9007-7f11a38a04e2\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button32\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d25c386e-3ac0-47d3-97b3-828097cba336\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button13\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShiftUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"03d9c7e0-1cff-4d44-b882-00492c9310d5\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button14\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShiftDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"995d02f7-7865-497f-8a44-d17bda21cc3c\",\r\n                    \"path\": \"VirtualPad/starter\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4b044d41-4530-4740-9085-502021b947fc\",\r\n                    \"path\": \"<HID::1949-402>/button12\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"StartEngine\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0581bbc4-409b-489c-a0d6-1aa0cf2eeb1d\",\r\n                    \"path\": \"VirtualPad/horn\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3a868ddf-685a-4765-b0f8-479b83f5dc03\",\r\n                    \"path\": \"<HID::1949-402>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"15535686-fc34-4cd6-b380-4d945dd62169\",\r\n                    \"path\": \"VirtualPad/leftSignal\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"LeftSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9a9bfc1b-1985-44a8-b80c-40ea52a9a972\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button8\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"LeftSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fb32024f-6bd2-4ff0-b636-aa5c227bfae8\",\r\n                    \"path\": \"VirtualPad/rightSignal\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"RightSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"75db095d-59aa-4734-b972-9244809603f5\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button6\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"RightSignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"13357c75-c144-431d-804b-4946bd8e29f3\",\r\n                    \"path\": \"VirtualPad/emergencySignal\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"EmergencySignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"675397ac-d1b4-4f1b-a57a-57c0f3e42a61\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"EmergencySignal\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"47b5e2bd-bc8a-4522-9cc3-00a08f3f8251\",\r\n                    \"path\": \"VirtualPad/headlights\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8f705c5e-ef99-4390-a971-4ebd24809ca2\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button19\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"78c893a7-e487-4dc8-b22b-c73592835650\",\r\n                    \"path\": \"VirtualPad/changeCamera\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"229a3ad9-fd77-4435-851b-7184d849595a\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button22\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2e343e0e-e9e0-4726-8360-43932aaffe1b\",\r\n                    \"path\": \"<Keyboard>/M\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"OpenMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6b4c65d2-c49e-401f-be2f-9d9e083a8d4b\",\r\n                    \"path\": \"VirtualPad/openMap\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"OpenMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7ef16127-ce8e-4854-ab63-ae0e1c522b83\",\r\n                    \"path\": \"<HID::1949-402>/hat/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"OpenMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ee234e0a-c525-4117-a5b5-6b28bf8a664f\",\r\n                    \"path\": \"<Gamepad>/start\",\r\n                    \"interactions\": \"Hold(duration=1)\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"OpenMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2bfb8fdb-4fc7-4a2a-a6ba-f96fdc5849a6\",\r\n                    \"path\": \"<Keyboard>/1\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e2813c0b-2447-4245-9e0d-ec8f3e0b2941\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button114\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear1\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"65ca827a-cb21-4070-a56d-1986d24a42dd\",\r\n                    \"path\": \"<Keyboard>/2\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2c6e4d28-f1c3-4f74-a33e-23e64959c7bc\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button115\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear2\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6f5cec3d-c211-4fbd-8a9f-0993c1029a8e\",\r\n                    \"path\": \"<Keyboard>/3\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"fb5510d5-7a7b-4250-979a-fe6569380177\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button116\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear3\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"00bbef43-ef73-4d02-9e40-abf8deaddb13\",\r\n                    \"path\": \"<Keyboard>/4\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f60c51df-68dc-4cf4-9470-e306c67a67bc\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button117\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear4\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"261bc71e-a8ae-4c90-8bc9-6551841e484f\",\r\n                    \"path\": \"<Keyboard>/5\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear5\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"dc67f17a-4772-4608-8850-a8e0cea564b4\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button118\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear5\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5b1dfd15-ec42-40c0-8745-a62ef6e2b15f\",\r\n                    \"path\": \"<Keyboard>/6\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear6\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9458f0b6-7b97-4ff0-91b9-e14521fd0e97\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button119\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear6\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"92ec5d23-4cb2-4f29-a970-fa8b7bc4fbc0\",\r\n                    \"path\": \"<Keyboard>/7\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"Gear7\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8dcd6998-e665-420e-b12a-a2814b262f3f\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button120\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Gear7\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"32df9639-e3a9-4cae-a362-5eac1da4a058\",\r\n                    \"path\": \"<Keyboard>/9\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"GearR\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b626c962-8a5b-4eb6-a99c-000b962d3895\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/button113\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"GearR\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ead4448d-2be7-4b77-87b2-8ef7ab682a0b\",\r\n                    \"path\": \"<Keyboard>/0\",\r\n                    \"interactions\": \"Press\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Car\",\r\n                    \"action\": \"GearN\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Moza\",\r\n                    \"id\": \"5730e94a-7fef-49f8-889e-028675a56263\",\r\n                    \"path\": \"1DAxis\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Clamp(min=-1,max=1)\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"negative\",\r\n                    \"id\": \"c272fe79-73db-499d-b787-e76f0acbd46f\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/stick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"positive\",\r\n                    \"id\": \"077157f8-ac2e-4af3-b7a5-5750420e0fa4\",\r\n                    \"path\": \"<HID::Gudsen MOZA R5 Base\u0090>/stick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"TouchWheel\",\r\n            \"id\": \"3e58c13b-9ed6-48e2-ab59-f3a895137d90\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Steering\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"196c34d6-3d75-4aeb-9f11-3428053d5ded\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Throttle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f5bc1766-7999-4ee0-9362-beadd0c1f894\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Brake\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"18bb60dc-3f36-4093-bc2f-f6c19c8cffd5\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Clutch\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"078105c3-5c0d-4537-8b8d-bfcf9b167200\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Handbrake\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"ab57c3e2-249e-4c8d-b934-52824abf7ad7\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"afeaa371-207f-4bbc-bae6-41e7c8f23d2b\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"20c08e6d-1eb4-45a9-b9a4-943af8931a56\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b1b10ed4-d77c-4fcb-a763-830363953c2e\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"18a419ff-2fc5-456e-966c-442cf61ea4a1\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e57159ba-bd0b-492c-a001-554048f9fd70\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"Tilt\",\r\n            \"id\": \"f6d17db7-e498-4395-b735-8d30c14c67c8\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Steering\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"e6d46a37-ba7f-4e82-9ffc-7f999e38558d\",\r\n                    \"expectedControlType\": \"Vector3\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Throttle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"30021bb4-2783-4cc0-893e-92011fda66a3\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Brake\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"c89efe51-1cf1-42fa-99e0-60b3dd950453\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Clutch\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"03ba3100-f602-4234-a40b-0a1ac64c76be\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Handbrake\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"4c1f5628-b7d0-4cb0-abfd-4265031a1c47\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"a930d15a-4e5b-414c-b0cb-557569c6ca9b\",\r\n                    \"path\": \"<Accelerometer>/acceleration\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Steering\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"8b9ee2af-f960-40f3-a1cc-1cd3b1afbc09\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Throttle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1ef28e5b-7e2e-48e7-87d8-6bbee88eb2e0\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Brake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7349eefc-145e-476e-9c0a-8bbf9ac80240\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Clutch\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f0708d61-4e51-4147-9429-d633816c4ab5\",\r\n                    \"path\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Handbrake\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"UI\",\r\n            \"id\": \"5f35b1e2-a93c-421c-9672-376d66136534\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Navigate\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"7ecbfeef-7687-440a-8ec0-0953bfd6fbc9\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Submit\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"25d8779a-2bc0-4390-a108-db5bc2be3ef1\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Cancel\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f23948d2-f337-4fb7-8af9-ee3cb470ec0c\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Point\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"97acb88c-a058-4a7e-9320-eb4ce82c4200\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"Click\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"212a5029-8806-40ff-a86a-eba61a262684\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"RightClick\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"da90d684-c527-4e4e-b069-4516114e46f5\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"MiddleClick\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"7d7ea9e7-a516-4748-9e59-0510340765e2\",\r\n                    \"expectedControlType\": \"Button\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ScrollWheel\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"780da86a-2c4f-4126-a664-ac79836e58e2\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"TrackedDevicePosition\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"ef2dfff1-1558-4203-80b0-ad2871fb7769\",\r\n                    \"expectedControlType\": \"Vector3\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"TrackedDeviceOrientation\",\r\n                    \"type\": \"PassThrough\",\r\n                    \"id\": \"ce9c863a-2b9b-4f79-bcdf-a94e93f57f4f\",\r\n                    \"expectedControlType\": \"Quaternion\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"Gamepad\",\r\n                    \"id\": \"5ea687b1-1faf-4f5e-9041-25b4bfd29bd4\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"9268739a-6bd6-4825-82b7-e4d0571174e4\",\r\n                    \"path\": \"<Gamepad>/leftStick/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"6451cfd9-5055-4af6-8de0-a7751b25b546\",\r\n                    \"path\": \"<Gamepad>/rightStick/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"3423f886-7019-4142-9a22-353c6c7777d0\",\r\n                    \"path\": \"<Gamepad>/leftStick/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"f6911727-70df-4a4a-82a2-61adf9909cfd\",\r\n                    \"path\": \"<Gamepad>/rightStick/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"fd4e9797-1358-4034-ab3a-43349ea9c76b\",\r\n                    \"path\": \"<Gamepad>/leftStick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"56eab8ce-24aa-423e-9296-e9b5c23d815b\",\r\n                    \"path\": \"<Gamepad>/rightStick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"1c607a3f-be7c-4cef-9d05-4b38530b2ae9\",\r\n                    \"path\": \"<Gamepad>/leftStick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"95a79062-852f-46d8-a50f-2a2b9d4a208a\",\r\n                    \"path\": \"<Gamepad>/rightStick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2fe93921-2a77-438a-9bcf-9c24b7e01d75\",\r\n                    \"path\": \"<Gamepad>/dpad\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Gamepad\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"Joystick\",\r\n                    \"id\": \"9f7eb4e8-be05-47f4-87e4-764b730f8b14\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"cf501c82-8345-40b4-adba-4613c7806542\",\r\n                    \"path\": \"<Joystick>/stick/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Joystick\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"acd297af-2f84-4936-b8b8-92b9183de40e\",\r\n                    \"path\": \"<Joystick>/stick/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Joystick\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"1880de4b-2fb9-4338-8ea8-c7a5a03bcc47\",\r\n                    \"path\": \"<Joystick>/stick/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Joystick\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"b0cee987-418c-4bc9-b17b-4393ffde2b3d\",\r\n                    \"path\": \"<Joystick>/stick/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Joystick\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"Keyboard\",\r\n                    \"id\": \"daa3648f-fc47-4817-a654-7b4e17481e40\",\r\n                    \"path\": \"2DVector\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"41d3afc2-b221-4c3a-8b50-304f706e5060\",\r\n                    \"path\": \"<Keyboard>/w\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"46b52a5f-597e-4261-8f77-e3918df541da\",\r\n                    \"path\": \"<Keyboard>/upArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"d1ad576c-db4d-4566-9fdb-cf7c3b293d0b\",\r\n                    \"path\": \"<Keyboard>/s\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"299acb87-caba-45f4-a9d7-7c1a02ee42db\",\r\n                    \"path\": \"<Keyboard>/downArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"5ce92315-73dc-4c53-a012-f658b13035e2\",\r\n                    \"path\": \"<Keyboard>/a\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"ca1878a3-974d-4b40-97e3-0a702be1096d\",\r\n                    \"path\": \"<Keyboard>/leftArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"1baf3281-4f2d-47cd-b926-2d2420a1353e\",\r\n                    \"path\": \"<Keyboard>/d\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"b43cfab2-3ac9-458e-bccf-2bd758ad522c\",\r\n                    \"path\": \"<Keyboard>/rightArrow\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Navigate\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1c52ef7c-1a68-4f5a-9c2a-e613a10a79f1\",\r\n                    \"path\": \"*/{Submit}\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR\",\r\n                    \"action\": \"Submit\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9915b37c-fe19-4bc4-b0d4-7c0da061e645\",\r\n                    \"path\": \"*/{Cancel}\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse;Gamepad;Touch;Joystick;XR\",\r\n                    \"action\": \"Cancel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"34805e22-f6c8-41a9-8a94-38f19bb187d5\",\r\n                    \"path\": \"<Mouse>/position\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Point\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"414deff7-bf4d-42dd-929b-25c612832bed\",\r\n                    \"path\": \"<Pen>/position\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"Point\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4fe63eea-3482-455d-9e25-46fbe4226679\",\r\n                    \"path\": \"<Touchscreen>/touch*/position\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Touch\",\r\n                    \"action\": \"Point\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e3d0211d-127a-4606-b988-672f8a133e28\",\r\n                    \"path\": \"<Mouse>/leftButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard&Mouse\",\r\n                    \"action\": \"Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"b88d4e40-4625-47fc-a563-62d7fdc3ce30\",\r\n                    \"path\": \"<Pen>/tip\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard&Mouse\",\r\n                    \"action\": \"Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9ef30d1a-ae7c-4b23-8ad0-5c6cdafe8849\",\r\n                    \"path\": \"<Touchscreen>/touch*/press\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Touch\",\r\n                    \"action\": \"Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"60fced2d-63ab-4682-924e-4663f951662a\",\r\n                    \"path\": \"<XRController>/trigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"XR\",\r\n                    \"action\": \"Click\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0689fac4-52a1-4b43-82bd-a9581adba204\",\r\n                    \"path\": \"<Mouse>/scroll\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \";Keyboard&Mouse\",\r\n                    \"action\": \"ScrollWheel\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"16109009-b211-4294-91f3-29d370780973\",\r\n                    \"path\": \"<Mouse>/rightButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"RightClick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"77156772-2bbc-42d6-bd46-dc33674d9919\",\r\n                    \"path\": \"<Mouse>/middleButton\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"Keyboard&Mouse\",\r\n                    \"action\": \"MiddleClick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"03415918-8e04-425d-9f5f-532c10e27b12\",\r\n                    \"path\": \"<XRController>/devicePosition\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"XR\",\r\n                    \"action\": \"TrackedDevicePosition\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c8cbf72d-eaf0-4729-9476-ec8f490a4dda\",\r\n                    \"path\": \"<XRController>/deviceRotation\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"XR\",\r\n                    \"action\": \"TrackedDeviceOrientation\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        },\r\n        {\r\n            \"name\": \"GamepadExtras\",\r\n            \"id\": \"79f03cc8-f354-4cac-98c6-a0f3b1f40f19\",\r\n            \"actions\": [\r\n                {\r\n                    \"name\": \"Pause\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"0e3df7be-f055-45b7-889a-67e1e8c721c0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Select\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"f4613098-861c-42b6-9262-2fcdd04913c7\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ManualAutoToggle\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"19edb13d-f08b-4e1b-b1e0-4ac931a9572f\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ToRoad\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"67c717bd-6bb2-407f-8b08-d68fcf592743\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Door\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"95d6a0b4-475e-4d6b-ab2f-1549b8c5bd63\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"BigMap\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"03777cc8-746a-4e12-9eb1-11bbd78a886d\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"SelectorUp\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b137d100-475d-41c4-a77f-7ec285f3b492\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"SelectorDown\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"3b7bc4b4-a234-4f0c-9bc8-d78ce41d2566\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ShifterStick\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"5769473c-5316-4e79-b357-9f16ad09df4d\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"MoveStick\",\r\n                    \"type\": \"Value\",\r\n                    \"id\": \"1e853460-0569-4825-9514-836f0f03f103\",\r\n                    \"expectedControlType\": \"Vector2\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": true\r\n                },\r\n                {\r\n                    \"name\": \"LeftIndicator\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"883fe38e-93ea-48a0-abc1-3ff5ef74a8d3\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"RightIndicator\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"58ce1f13-8b57-4a3d-a9e9-db187bdeafa0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Hazard\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"d4e828db-1190-4b00-9883-ce546de9e333\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Headlights\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"e82ed77e-ecdd-4d2f-bbdd-e1433ab6f6c0\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Horn\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"78ffb17d-2d10-4459-8c42-f1e33f7282de\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"ChangeCamera\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"582ac272-e064-4c25-bb53-e703e735c2f2\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                },\r\n                {\r\n                    \"name\": \"Jump\",\r\n                    \"type\": \"Button\",\r\n                    \"id\": \"b1f7a24c-9d55-4e6f-8f21-3c0a9e5d7b41\",\r\n                    \"expectedControlType\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"interactions\": \"\",\r\n                    \"initialStateCheck\": false\r\n                }\r\n            ],\r\n            \"bindings\": [\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"263f894a-6ba4-44b9-8fb9-bafaebe4f178\",\r\n                    \"path\": \"<Gamepad>/start\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Pause\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"9f9ee7c8-0043-4b58-8f0d-3770db9b2089\",\r\n                    \"path\": \"<HID::1949-402>/button12\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Pause\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"5a69f748-2184-4a7e-9536-2cc8f091c840\",\r\n                    \"path\": \"<Gamepad>/buttonEast\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Select\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"794e61ec-febe-4708-93b7-47c098696b40\",\r\n                    \"path\": \"<HID::1949-402>/button2\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Select\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e9c70c93-0ddd-4cf0-b89b-be2ee39d27ee\",\r\n                    \"path\": \"<Gamepad>/buttonNorth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ManualAutoToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"586dc4bc-2ffc-4b00-9f76-38970842b85e\",\r\n                    \"path\": \"<HID::1949-402>/button5\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ManualAutoToggle\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"318f32aa-da75-4f72-aabe-b9a2ff23e994\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ToRoad\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"af0b513d-a09b-4f38-a7e5-e8c4a52c31fc\",\r\n                    \"path\": \"<HID::1949-402>/trigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ToRoad\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"0c9e5a13-4d72-4b88-9a6f-7e1d2c8b45f0\",\r\n                    \"path\": \"<Gamepad>/buttonSouth\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Jump\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"2f6b81d4-70a5-4c19-8d3e-51b9f0a6cc27\",\r\n                    \"path\": \"<HID::1949-402>/trigger\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Jump\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"47f0903e-500e-4086-9326-b0a056946f5b\",\r\n                    \"path\": \"<Gamepad>/dpad/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Door\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3e187c2f-ef7e-43cf-8ae2-24790d8d84e4\",\r\n                    \"path\": \"<HID::1949-402>/hat/down\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Door\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"79c8aca1-f3bb-47f1-b50b-3482da2a6b5b\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"BigMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c3936da2-e22c-42fa-9590-bc68ff3192a5\",\r\n                    \"path\": \"<HID::1949-402>/hat/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"BigMap\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f1b4e8f1-83ed-47e5-b266-e05d47251146\",\r\n                    \"path\": \"<Gamepad>/rightShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SelectorUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"cf706435-6ff1-4dd6-9407-7d471d394ab3\",\r\n                    \"path\": \"<HID::1949-402>/button8\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SelectorUp\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"f81b2aa2-33d5-4c43-8549-4d73205d5042\",\r\n                    \"path\": \"<Gamepad>/leftShoulder\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SelectorDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"1d4b8f89-908e-4a52-9505-c45a2d7e68dc\",\r\n                    \"path\": \"<HID::1949-402>/button7\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"SelectorDown\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"52474bad-a8ca-4a4a-a860-beb1535d4054\",\r\n                    \"path\": \"<Gamepad>/rightStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShifterStick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"HID Right Stick\",\r\n                    \"id\": \"77e91075-53dd-473f-acd2-68845afdfbe8\",\r\n                    \"path\": \"2DVector(mode=2)\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShifterStick\",\r\n                    \"isComposite\": true,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"up\",\r\n                    \"id\": \"16c87f79-a7a4-43af-8afc-4fda7b8980e2\",\r\n                    \"path\": \"<HID::1949-402>/rz\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Invert\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShifterStick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"down\",\r\n                    \"id\": \"9c8ea966-a9cb-4c77-a792-911e20c54733\",\r\n                    \"path\": \"<HID::1949-402>/rz\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShifterStick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"left\",\r\n                    \"id\": \"82c06f90-0dd8-45fa-9a13-0d3c79a629d7\",\r\n                    \"path\": \"<HID::1949-402>/z\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"Invert\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShifterStick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"right\",\r\n                    \"id\": \"f710ee2c-9183-4809-b523-24982c8417ee\",\r\n                    \"path\": \"<HID::1949-402>/z\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ShifterStick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": true\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"4d2da1ca-cf53-4254-ad34-2f70891bc492\",\r\n                    \"path\": \"<Gamepad>/leftStick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"MoveStick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"d4aaba28-ef5b-4307-8825-2d06c90a4296\",\r\n                    \"path\": \"<HID::1949-402>/stick\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"MoveStick\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"ef491b37-d526-4758-bb0c-20c01aa1387b\",\r\n                    \"path\": \"<Gamepad>/dpad/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"LeftIndicator\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"01978720-b99a-4497-ab35-276453bde4c2\",\r\n                    \"path\": \"<HID::1949-402>/hat/left\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"LeftIndicator\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"3f632015-ca28-49e6-bb8a-c2431c4b4f82\",\r\n                    \"path\": \"<Gamepad>/dpad/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"RightIndicator\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e6b694df-c7d9-4044-a57e-20447e3a8cd2\",\r\n                    \"path\": \"<HID::1949-402>/hat/right\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"RightIndicator\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"06c6fd3b-0ff1-4fc9-98f7-797e57154a39\",\r\n                    \"path\": \"<Gamepad>/dpad/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Hazard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6d3afbe9-3b30-4786-a14c-d8a477d0905a\",\r\n                    \"path\": \"<HID::1949-402>/hat/up\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Hazard\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7a880fc3-7e0a-464e-bad5-546b5db51628\",\r\n                    \"path\": \"<Gamepad>/rightStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"e6723c6c-cba6-4385-bb85-5a9aa6db5368\",\r\n                    \"path\": \"<HID::1949-402>/button15\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Headlights\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"7f202789-1e63-45fc-8274-bf89b179cd60\",\r\n                    \"path\": \"<Gamepad>/leftStickPress\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"6bab25ec-b3b5-4eee-86f2-410273a05ff2\",\r\n                    \"path\": \"<HID::1949-402>/button14\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"Horn\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"26ae2df5-a61d-4b94-b6b5-5b2b2920edd5\",\r\n                    \"path\": \"<Gamepad>/select\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                },\r\n                {\r\n                    \"name\": \"\",\r\n                    \"id\": \"c7381a60-ece4-4303-8fc9-7db243d64423\",\r\n                    \"path\": \"<HID::1949-402>/button11\",\r\n                    \"interactions\": \"\",\r\n                    \"processors\": \"\",\r\n                    \"groups\": \"\",\r\n                    \"action\": \"ChangeCamera\",\r\n                    \"isComposite\": false,\r\n                    \"isPartOfComposite\": false\r\n                }\r\n            ]\r\n        }\r\n    ],\r\n    \"controlSchemes\": [\r\n        {\r\n            \"name\": \"Car\",\r\n            \"bindingGroup\": \"Car\",\r\n            \"devices\": []\r\n        }\r\n    ]\r\n}");
		m_Person = asset.FindActionMap("Person", throwIfNotFound: true);
		m_Person_LeftRight = m_Person.FindAction("LeftRight", throwIfNotFound: true);
		m_Person_Move = m_Person.FindAction("Move", throwIfNotFound: true);
		m_Person_Stop = m_Person.FindAction("Stop", throwIfNotFound: true);
		m_Car = asset.FindActionMap("Car", throwIfNotFound: true);
		m_Car_Steering = m_Car.FindAction("Steering", throwIfNotFound: true);
		m_Car_Throttle = m_Car.FindAction("Throttle", throwIfNotFound: true);
		m_Car_Brake = m_Car.FindAction("Brake", throwIfNotFound: true);
		m_Car_Clutch = m_Car.FindAction("Clutch", throwIfNotFound: true);
		m_Car_Handbrake = m_Car.FindAction("Handbrake", throwIfNotFound: true);
		m_Car_StartEngine = m_Car.FindAction("StartEngine", throwIfNotFound: true);
		m_Car_Horn = m_Car.FindAction("Horn", throwIfNotFound: true);
		m_Car_LeftSignal = m_Car.FindAction("LeftSignal", throwIfNotFound: true);
		m_Car_RightSignal = m_Car.FindAction("RightSignal", throwIfNotFound: true);
		m_Car_EmergencySignal = m_Car.FindAction("EmergencySignal", throwIfNotFound: true);
		m_Car_Headlights = m_Car.FindAction("Headlights", throwIfNotFound: true);
		m_Car_ChangeCamera = m_Car.FindAction("ChangeCamera", throwIfNotFound: true);
		m_Car_OpenMap = m_Car.FindAction("OpenMap", throwIfNotFound: true);
		m_Car_ShiftUp = m_Car.FindAction("ShiftUp", throwIfNotFound: true);
		m_Car_ShiftDown = m_Car.FindAction("ShiftDown", throwIfNotFound: true);
		m_Car_Gear1 = m_Car.FindAction("Gear1", throwIfNotFound: true);
		m_Car_Gear2 = m_Car.FindAction("Gear2", throwIfNotFound: true);
		m_Car_Gear3 = m_Car.FindAction("Gear3", throwIfNotFound: true);
		m_Car_Gear4 = m_Car.FindAction("Gear4", throwIfNotFound: true);
		m_Car_Gear5 = m_Car.FindAction("Gear5", throwIfNotFound: true);
		m_Car_Gear6 = m_Car.FindAction("Gear6", throwIfNotFound: true);
		m_Car_Gear7 = m_Car.FindAction("Gear7", throwIfNotFound: true);
		m_Car_GearR = m_Car.FindAction("GearR", throwIfNotFound: true);
		m_Car_GearN = m_Car.FindAction("GearN", throwIfNotFound: true);
		m_SteeringWheel = asset.FindActionMap("SteeringWheel", throwIfNotFound: true);
		m_SteeringWheel_Steering = m_SteeringWheel.FindAction("Steering", throwIfNotFound: true);
		m_SteeringWheel_Throttle = m_SteeringWheel.FindAction("Throttle", throwIfNotFound: true);
		m_SteeringWheel_Brake = m_SteeringWheel.FindAction("Brake", throwIfNotFound: true);
		m_SteeringWheel_Clutch = m_SteeringWheel.FindAction("Clutch", throwIfNotFound: true);
		m_SteeringWheel_Handbrake = m_SteeringWheel.FindAction("Handbrake", throwIfNotFound: true);
		m_SteeringWheel_StartEngine = m_SteeringWheel.FindAction("StartEngine", throwIfNotFound: true);
		m_SteeringWheel_Horn = m_SteeringWheel.FindAction("Horn", throwIfNotFound: true);
		m_SteeringWheel_LeftSignal = m_SteeringWheel.FindAction("LeftSignal", throwIfNotFound: true);
		m_SteeringWheel_RightSignal = m_SteeringWheel.FindAction("RightSignal", throwIfNotFound: true);
		m_SteeringWheel_EmergencySignal = m_SteeringWheel.FindAction("EmergencySignal", throwIfNotFound: true);
		m_SteeringWheel_Headlights = m_SteeringWheel.FindAction("Headlights", throwIfNotFound: true);
		m_SteeringWheel_ChangeCamera = m_SteeringWheel.FindAction("ChangeCamera", throwIfNotFound: true);
		m_SteeringWheel_OpenMap = m_SteeringWheel.FindAction("OpenMap", throwIfNotFound: true);
		m_SteeringWheel_ShiftUp = m_SteeringWheel.FindAction("ShiftUp", throwIfNotFound: true);
		m_SteeringWheel_ShiftDown = m_SteeringWheel.FindAction("ShiftDown", throwIfNotFound: true);
		m_SteeringWheel_Gear1 = m_SteeringWheel.FindAction("Gear1", throwIfNotFound: true);
		m_SteeringWheel_Gear2 = m_SteeringWheel.FindAction("Gear2", throwIfNotFound: true);
		m_SteeringWheel_Gear3 = m_SteeringWheel.FindAction("Gear3", throwIfNotFound: true);
		m_SteeringWheel_Gear4 = m_SteeringWheel.FindAction("Gear4", throwIfNotFound: true);
		m_SteeringWheel_Gear5 = m_SteeringWheel.FindAction("Gear5", throwIfNotFound: true);
		m_SteeringWheel_Gear6 = m_SteeringWheel.FindAction("Gear6", throwIfNotFound: true);
		m_SteeringWheel_Gear7 = m_SteeringWheel.FindAction("Gear7", throwIfNotFound: true);
		m_SteeringWheel_GearR = m_SteeringWheel.FindAction("GearR", throwIfNotFound: true);
		m_SteeringWheel_GearN = m_SteeringWheel.FindAction("GearN", throwIfNotFound: true);
		m_TouchWheel = asset.FindActionMap("TouchWheel", throwIfNotFound: true);
		m_TouchWheel_Steering = m_TouchWheel.FindAction("Steering", throwIfNotFound: true);
		m_TouchWheel_Throttle = m_TouchWheel.FindAction("Throttle", throwIfNotFound: true);
		m_TouchWheel_Brake = m_TouchWheel.FindAction("Brake", throwIfNotFound: true);
		m_TouchWheel_Clutch = m_TouchWheel.FindAction("Clutch", throwIfNotFound: true);
		m_TouchWheel_Handbrake = m_TouchWheel.FindAction("Handbrake", throwIfNotFound: true);
		m_Tilt = asset.FindActionMap("Tilt", throwIfNotFound: true);
		m_Tilt_Steering = m_Tilt.FindAction("Steering", throwIfNotFound: true);
		m_Tilt_Throttle = m_Tilt.FindAction("Throttle", throwIfNotFound: true);
		m_Tilt_Brake = m_Tilt.FindAction("Brake", throwIfNotFound: true);
		m_Tilt_Clutch = m_Tilt.FindAction("Clutch", throwIfNotFound: true);
		m_Tilt_Handbrake = m_Tilt.FindAction("Handbrake", throwIfNotFound: true);
		m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
		m_UI_Navigate = m_UI.FindAction("Navigate", throwIfNotFound: true);
		m_UI_Submit = m_UI.FindAction("Submit", throwIfNotFound: true);
		m_UI_Cancel = m_UI.FindAction("Cancel", throwIfNotFound: true);
		m_UI_Point = m_UI.FindAction("Point", throwIfNotFound: true);
		m_UI_Click = m_UI.FindAction("Click", throwIfNotFound: true);
		m_UI_RightClick = m_UI.FindAction("RightClick", throwIfNotFound: true);
		m_UI_MiddleClick = m_UI.FindAction("MiddleClick", throwIfNotFound: true);
		m_UI_ScrollWheel = m_UI.FindAction("ScrollWheel", throwIfNotFound: true);
		m_UI_TrackedDevicePosition = m_UI.FindAction("TrackedDevicePosition", throwIfNotFound: true);
		m_UI_TrackedDeviceOrientation = m_UI.FindAction("TrackedDeviceOrientation", throwIfNotFound: true);
		m_GamepadExtras = asset.FindActionMap("GamepadExtras", throwIfNotFound: true);
		m_GamepadExtras_Pause = m_GamepadExtras.FindAction("Pause", throwIfNotFound: true);
		m_GamepadExtras_Select = m_GamepadExtras.FindAction("Select", throwIfNotFound: true);
		m_GamepadExtras_ManualAutoToggle = m_GamepadExtras.FindAction("ManualAutoToggle", throwIfNotFound: true);
		m_GamepadExtras_ToRoad = m_GamepadExtras.FindAction("ToRoad", throwIfNotFound: true);
		m_GamepadExtras_Door = m_GamepadExtras.FindAction("Door", throwIfNotFound: true);
		m_GamepadExtras_BigMap = m_GamepadExtras.FindAction("BigMap", throwIfNotFound: true);
		m_GamepadExtras_SelectorUp = m_GamepadExtras.FindAction("SelectorUp", throwIfNotFound: true);
		m_GamepadExtras_SelectorDown = m_GamepadExtras.FindAction("SelectorDown", throwIfNotFound: true);
		m_GamepadExtras_ShifterStick = m_GamepadExtras.FindAction("ShifterStick", throwIfNotFound: true);
		m_GamepadExtras_MoveStick = m_GamepadExtras.FindAction("MoveStick", throwIfNotFound: true);
		m_GamepadExtras_LeftIndicator = m_GamepadExtras.FindAction("LeftIndicator", throwIfNotFound: true);
		m_GamepadExtras_RightIndicator = m_GamepadExtras.FindAction("RightIndicator", throwIfNotFound: true);
		m_GamepadExtras_Hazard = m_GamepadExtras.FindAction("Hazard", throwIfNotFound: true);
		m_GamepadExtras_Headlights = m_GamepadExtras.FindAction("Headlights", throwIfNotFound: true);
		m_GamepadExtras_Horn = m_GamepadExtras.FindAction("Horn", throwIfNotFound: true);
		m_GamepadExtras_ChangeCamera = m_GamepadExtras.FindAction("ChangeCamera", throwIfNotFound: true);
		m_GamepadExtras_Jump = m_GamepadExtras.FindAction("Jump", throwIfNotFound: true);
	}

	~CarInputController()
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
