using UnityEngine;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[CreateAssetMenu(fileName = "New ScrollValueEvent", menuName = "SOAP/ScrollValueEvent")]
	[Tooltip("There can only be one of these")]
	public class ScrollValueEvent : ScriptableValueEvent<ScrollEnum>
	{
	}
}
