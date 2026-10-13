using UnityEngine;

namespace _Car_Parking.Scripts.Levels
{
	[CreateAssetMenu(fileName = "LevelPreviews", menuName = "Levels/LevelPreviews", order = 21)]
	public class LevelPreviewsConfig : ScriptableObject
	{
		public LevelPreviewData[] LevelPreviews;
	}
}
