using UnityEngine;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[Tooltip("There can only be one of these")]
	[CreateAssetMenu(fileName = "New BoolValueEvent", menuName = "SOAP/BoolValueEvent")]
	public class BoolValueEvent : ScriptableValueEvent<bool>
	{
	}
}
