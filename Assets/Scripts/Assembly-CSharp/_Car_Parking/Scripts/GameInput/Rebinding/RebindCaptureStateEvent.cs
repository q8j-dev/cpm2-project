using UnityEngine;
using _Car_Parking.Scripts.SOAP;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[CreateAssetMenu(fileName = "New RebindCaptureStateEvent", menuName = "SOAP/RebindCaptureStateEvent")]
	public class RebindCaptureStateEvent : ScriptableEvent<RebindCaptureStateEnum>
	{
	}
}
