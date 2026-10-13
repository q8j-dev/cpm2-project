using UnityEngine;

namespace CPM.Chunking
{
	public static class ChunkVisibilityRule
	{
		public static bool ShouldEnable(int forceState, Vector3 chunkPosition, Vector3 cameraPosition, float farClipPlane, float chunkSizeX)
		{
			switch (forceState)
			{
			case 1:
				return true;
			case 0:
				return false;
			default:
			{
				Vector3 a = chunkPosition;
				a.y = cameraPosition.y;
				float num = Vector3.Distance(a, cameraPosition);
				float num2 = farClipPlane + chunkSizeX;
				return num <= num2;
			}
			}
		}
	}
}
