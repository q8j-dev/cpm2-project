using UnityEngine;

namespace _Car_Parking.Scripts.MenuUI
{
	[CreateAssetMenu(menuName = "CarParking/Configs/MessageFromDeveloperConfig")]
	public class MessageFromDeveloperConfig : ScriptableObject
	{
		public bool Debug;

		[TextArea]
		public string DebugMessage;
	}
}
