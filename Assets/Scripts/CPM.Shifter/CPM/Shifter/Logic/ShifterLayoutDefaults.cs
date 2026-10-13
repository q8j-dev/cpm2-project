using UnityEngine;

namespace CPM.Shifter.Logic
{
	[CreateAssetMenu(fileName = "ShifterLayoutDefaults", menuName = "CarPartsTuning/EngineParts/ShifterLayoutDefaults", order = 7)]
	public class ShifterLayoutDefaults : ScriptableObject
	{
		public const string ResourcePath = "ShifterLayoutDefaults";

		[Tooltip("Index = NumGears - 3, so [0]=3-speed, [1]=4, [2]=5, [3]=6, [4]=7. Length must be 5.")]
		public ShifterLayoutAsset[] Defaults;

		public static ShifterLayoutDefaults Instance => Resources.Load<ShifterLayoutDefaults>("ShifterLayoutDefaults");

		public ShifterLayoutAsset GetByGearCount(int numGears)
		{
			int num = Mathf.Clamp(numGears, 3, 7);
			int num2 = num - 3;
			ShifterLayoutAsset[] defaults = Defaults;
			if (defaults == null || defaults.Length != 5 || !Defaults[num2])
			{
				Debug.LogError($"ShifterLayoutDefaults: missing entry for {num}-speed. Using hardcoded fallback.");
				ShifterLayoutAsset shifterLayoutAsset = ScriptableObject.CreateInstance<ShifterLayoutAsset>();
				shifterLayoutAsset.NumGears = num;
				shifterLayoutAsset.ReversePos = ReversePosition.BottomRight;
				shifterLayoutAsset.DoglegFirstGear = false;
				shifterLayoutAsset.name = $"HardcodedFallback_{num}sp";
				return shifterLayoutAsset;
			}
			return Defaults[num2];
		}

		public static void ResetInstanceForTests()
		{
		}
	}
}
