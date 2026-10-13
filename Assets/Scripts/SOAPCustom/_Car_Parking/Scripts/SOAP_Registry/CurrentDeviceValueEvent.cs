using UnityEngine;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[Tooltip("There can only be one of these")]
	[CreateAssetMenu(fileName = "New CurrentDeviceValueEvent", menuName = "SOAP/CurrentDeviceValueEvent")]
	public class CurrentDeviceValueEvent : ScriptableValueEvent<SupportedInputDevicesEnum>
	{
	}
}
