using UnityEngine;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[CreateAssetMenu(fileName = "New Vector2ValueEvent", menuName = "SOAP/Vector2ValueEvent")]
	[Tooltip("There can only be one of these")]
	public class Vector2ValueEvent : ScriptableValueEvent<Vector2>
	{
	}
}
