using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyCrowd
{
	[CreateAssetMenu(menuName = "Rally/CrowdController")]
	public class RallyCrowdControllerConfig : ScriptableObject
	{
		public float SpawnRadius;

		public float DespawnRadius;

		public int MaxSpawnsPerTick = 8;

		public float ThrottleInterval = 0.2f;
	}
}
