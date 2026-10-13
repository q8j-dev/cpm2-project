using UnityEngine;

namespace CPM.Ghost.Core
{
	[CreateAssetMenu(menuName = "CPM/Ghost/Config", fileName = "GhostConfig")]
	public sealed class GhostConfig : ScriptableObject
	{
		[Range(1f, 60f)]
		public int RecordHz = 30;

		[Range(1f, 50f)]
		public int MaxPersonalRecords = 1;

		[Range(30f, 3600f)]
		public int MaxRecordSeconds = 600;

		[Range(0f, 15f)]
		public float PlaybackReadyTimeoutSeconds = 5f;

		public GhostMaterialPreset MaterialPreset;

		public int[] LodWheelPool;

		public LayerMask GroundLayers;
	}
}
