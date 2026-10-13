using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	public class ReciprocatingEnginePart : PartItem
	{
		[Header("Input parameters")]
		[Tooltip("Maximum allowable RPM")]
		[SerializeField]
		internal float maxRPM = -1f;

		internal float inertia;

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;

		[ContextMenu("Calculate Inertia")]
		public virtual float GetInertia()
		{
			inertia = Utils.KgToInertia(Mass);
			return inertia;
		}
	}
}
