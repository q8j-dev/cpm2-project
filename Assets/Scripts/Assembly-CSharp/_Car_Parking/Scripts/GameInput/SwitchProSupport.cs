using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;

namespace _Car_Parking.Scripts.GameInput
{
	public static class SwitchProSupport
	{
		public static bool IsSwitchPro(InputDevice device)
		{
			return device is SwitchProControllerHID;
		}
	}
}
