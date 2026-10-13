using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class SpawnLayoutProvider : MonoBehaviour
	{
		[SerializeField]
		private SpawnLayoutSettings Settings = new SpawnLayoutSettings
		{
			Shape = SpawnLayoutShape.Grid,
			ColumnsPerRow = 4,
			GridSpacing = new Vector2(3f, 5f),
			Radius = 10f
		};

		public PositionsAndRotation GetPoint(int index, int count)
		{
			return SpawnLayout.GetPoint(index, count, in Settings, base.transform.position, base.transform.rotation);
		}
	}
}
