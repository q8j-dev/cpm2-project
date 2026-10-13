using UnityEngine;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[CreateAssetMenu(fileName = "New UiNavigationValueEvent", menuName = "SOAP/UiNavigationValueEvent")]
	[Tooltip("There can only be one of these")]
	public class UiNavigationValueEvent : ScriptableValueEvent<UiNavigationEnum>
	{
	}
}
