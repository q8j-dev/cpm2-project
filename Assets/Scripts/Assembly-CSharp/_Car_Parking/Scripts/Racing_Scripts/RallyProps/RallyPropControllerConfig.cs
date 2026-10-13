using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts.RallyProps
{
	[CreateAssetMenu(menuName = "Rally/PropController")]
	public class RallyPropControllerConfig : ScriptableObject
	{
		public float SpawnRadius = 250f;

		public float DespawnRadius = 300f;

		public int MaxSpawnsPerTick = 20;

		public float ThrottleInterval = 0.2f;
	}
}
