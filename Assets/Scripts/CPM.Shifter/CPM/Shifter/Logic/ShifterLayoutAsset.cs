using UnityEngine;

namespace CPM.Shifter.Logic
{
	[CreateAssetMenu(fileName = "ShifterLayout", menuName = "CarPartsTuning/EngineParts/ShifterLayout", order = 6)]
	public class ShifterLayoutAsset : ScriptableObject
	{
		[Range(3f, 7f)]
		public int NumGears = 6;

		public ReversePosition ReversePos = ReversePosition.BottomRight;

		[Tooltip("True → 1st gear in bottom-left slot, others shift up (racing dogleg pattern).")]
		public bool DoglegFirstGear;

		[Tooltip("Отметить, когда сочетание DoglegFirstGear + DoglegTopLeft подтверждено как задуманное. Гасит предупреждение OnValidate для этого ассета.")]
		public bool DoglegLayoutConfirmed;
	}
}
