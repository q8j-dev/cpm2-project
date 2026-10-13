using UnityEngine;

namespace TouchControlsKit
{
	public sealed class TCKInput : MonoBehaviour
	{
		private static ControllerBase[] controllers;

		private static int controllersCount;

		private static TCKButton[] buttons;

		private static int buttonsCount;

		private static TCKInput instance;

		internal static TCKInput Instance
		{
			get
			{
				if (!instance)
				{
					instance = Object.FindObjectOfType<TCKInput>();
				}
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
			SetActive(value: true);
		}

		private void Update()
		{
			for (int i = 0; i < controllersCount; i++)
			{
				controllers[i].PressHandler();
			}
		}

		internal static void SetActive(bool value)
		{
			Instance.enabled = value;
			Instance.gameObject.SetActive(value);
			if (Instance.enabled)
			{
				controllers = Instance.gameObject.GetComponentsInChildren<ControllerBase>();
				controllersCount = controllers.Length;
				buttons = Instance.gameObject.GetComponentsInChildren<TCKButton>();
				buttonsCount = buttons.Length;
				for (int i = 0; i < controllersCount; i++)
				{
					controllers[i].ControlAwake();
				}
			}
			else
			{
				controllers = (buttons = null);
				controllersCount = (buttonsCount = 0);
			}
		}

		public static float GetAxis(string controllerName, string axisName)
		{
			if (!Instance.enabled)
			{
				return 0f;
			}
			for (int i = 0; i < controllersCount; i++)
			{
				if (controllers[i].MyName == controllerName)
				{
					if (axisName == controllers[i].axisX.Name)
					{
						return controllers[i].axisX.value;
					}
					if (axisName == controllers[i].axisY.Name)
					{
						return controllers[i].axisY.value;
					}
					Debug.LogError("Axis name: " + axisName + " not found!");
					return 0f;
				}
			}
			Debug.LogError("Controller name: " + controllerName + " not found!");
			return 0f;
		}

		public static bool GetAxisEnable(string controllerName, string axisName)
		{
			if (!Instance.enabled)
			{
				return false;
			}
			for (int i = 0; i < controllersCount; i++)
			{
				if (controllers[i].MyName == controllerName)
				{
					if (axisName == controllers[i].axisX.Name)
					{
						return controllers[i].axisX.enabled;
					}
					if (axisName == controllers[i].axisY.Name)
					{
						return controllers[i].axisY.enabled;
					}
					Debug.LogError("Axis name: " + axisName + " not found!");
					return false;
				}
			}
			Debug.LogError("Controller name: " + controllerName + " not found!");
			return false;
		}

		public static void SetAxisEnable(string controllerName, string axisName, bool value)
		{
			if (!Instance.enabled)
			{
				return;
			}
			for (int i = 0; i < controllersCount; i++)
			{
				if (controllers[i].MyName == controllerName)
				{
					if (axisName == controllers[i].axisX.Name)
					{
						controllers[i].axisX.enabled = value;
					}
					else if (axisName == controllers[i].axisY.Name)
					{
						controllers[i].axisY.enabled = value;
					}
					else
					{
						Debug.LogError("Axis name: " + axisName + " not found!");
					}
					return;
				}
			}
			Debug.LogError("Controller name: " + controllerName + " not found!");
		}

		public static bool GetAxisInverse(string controllerName, string axisName)
		{
			if (!Instance.enabled)
			{
				return false;
			}
			for (int i = 0; i < controllersCount; i++)
			{
				if (controllers[i].MyName == controllerName)
				{
					if (axisName == controllers[i].axisX.Name)
					{
						return controllers[i].axisX.inverse;
					}
					if (axisName == controllers[i].axisY.Name)
					{
						return controllers[i].axisY.inverse;
					}
					Debug.LogError("Axis name: " + axisName + " not found!");
					return false;
				}
			}
			Debug.LogError("Controller name: " + controllerName + " not found!");
			return false;
		}

		public static void SetAxisInverse(string controllerName, string axisName, bool value)
		{
			if (!Instance.enabled)
			{
				return;
			}
			for (int i = 0; i < controllersCount; i++)
			{
				if (controllers[i].MyName == controllerName)
				{
					if (axisName == controllers[i].axisX.Name)
					{
						controllers[i].axisX.inverse = value;
					}
					else if (axisName == controllers[i].axisY.Name)
					{
						controllers[i].axisY.inverse = value;
					}
					else
					{
						Debug.LogError("Axis name: " + axisName + " not found!");
					}
					return;
				}
			}
			Debug.LogError("Controller name: " + controllerName + " not found!");
		}

		public static float GetSensitivity(string controllerName)
		{
			if (!Instance.enabled)
			{
				return 0f;
			}
			for (int i = 0; i < controllersCount; i++)
			{
				if (controllers[i].MyName == controllerName)
				{
					return controllers[i].sensitivity;
				}
			}
			Debug.LogError("Controller name: " + controllerName + " not found!");
			return 0f;
		}

		public static void SetSensitivity(string controllerName, float value)
		{
			if (!Instance.enabled)
			{
				return;
			}
			for (int i = 0; i < controllersCount; i++)
			{
				if (controllers[i].MyName == controllerName)
				{
					controllers[i].sensitivity = value;
					return;
				}
			}
			Debug.LogError("Controller name: " + controllerName + " not found!");
		}

		public static void ShowingTouchZone(bool value)
		{
			if (Instance.enabled)
			{
				for (int i = 0; i < controllersCount; i++)
				{
					controllers[i].ShowTouchZone = value;
				}
			}
		}

		public static bool GetButtonDown(string buttonName)
		{
			if (!Instance.enabled)
			{
				return false;
			}
			for (int i = 0; i < buttonsCount; i++)
			{
				if (buttons[i].MyName == buttonName)
				{
					return buttons[i].ButtonDOWN;
				}
			}
			Debug.LogError("Button name: " + buttonName + " not found!");
			return false;
		}

		public static bool GetButton(string buttonName)
		{
			if (!Instance.enabled)
			{
				return false;
			}
			for (int i = 0; i < buttonsCount; i++)
			{
				if (buttons[i].MyName == buttonName)
				{
					return buttons[i].ButtonPRESSED;
				}
			}
			Debug.LogError("Button name: " + buttonName + " not found!");
			return false;
		}

		public static bool GetButtonUp(string buttonName)
		{
			if (!Instance.enabled)
			{
				return false;
			}
			for (int i = 0; i < buttonsCount; i++)
			{
				if (buttons[i].MyName == buttonName)
				{
					return buttons[i].ButtonUP;
				}
			}
			Debug.LogError("Button name: " + buttonName + " not found!");
			return false;
		}
	}
}
