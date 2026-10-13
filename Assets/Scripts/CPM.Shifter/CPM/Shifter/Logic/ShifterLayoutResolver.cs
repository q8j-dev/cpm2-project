using UnityEngine;

namespace CPM.Shifter.Logic
{
	public static class ShifterLayoutResolver
	{
		public static ShifterLayoutAsset Resolve(ShifterLayoutAsset explicitLayout, int numGears, ShifterLayoutDefaults defaults)
		{
			if (explicitLayout != null)
			{
				return explicitLayout;
			}
			if (defaults != null)
			{
				return defaults.GetByGearCount(numGears);
			}
			Debug.LogError("ShifterLayoutResolver: both explicit and defaults are null. Using hardcoded fallback.");
			ShifterLayoutAsset shifterLayoutAsset = ScriptableObject.CreateInstance<ShifterLayoutAsset>();
			shifterLayoutAsset.NumGears = Mathf.Clamp(numGears, 3, 7);
			shifterLayoutAsset.ReversePos = ReversePosition.BottomRight;
			return shifterLayoutAsset;
		}
	}
}
