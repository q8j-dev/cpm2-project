using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility.ScriptableObjects
{
	[CreateAssetMenu(fileName = "SteamUISceneConfig", menuName = "Configs/SteamUISceneConfig")]
	public class SteamUISceneConfig : ScriptableObject
	{
		public UIItemValues[] items;

		[Tooltip("Screen aspect ratio (width / height) at and above which 'Wide Aspect Override' values are used for items that have them enabled. E.g. 2.0 catches 21:9 (~2.33) and 32:9 (~3.55) but not 16:9 (~1.78).")]
		public float wideAspectThreshold = 2f;
	}
}
