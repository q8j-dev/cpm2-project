using UnityEngine;

namespace Service.Cargo
{
	[CreateAssetMenu(fileName = "Service Data", menuName = "Scriptable/Service Data")]
	public class ServiceData : ScriptableObject
	{
		public int scriptablesCount;

		public GameObject groundLightPrefab;
	}
}
