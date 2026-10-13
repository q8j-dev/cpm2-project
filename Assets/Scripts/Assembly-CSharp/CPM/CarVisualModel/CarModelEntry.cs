using System;
using UnityEngine;

namespace CPM.CarVisualModel
{
	[Serializable]
	public class CarModelEntry
	{
		[Tooltip("Car id — the builder looks up the entry by this")]
		public int CarId;

		[Tooltip("Body anchor local position = VehicleData.MainModelPosition from car{id}; where body/b{id} is placed")]
		public Vector3 BodyAnchorLocal;

		[Tooltip("Wheel mounts, root-local (positions as in car{id}, i.e. at suspension-top; builder drops them 0.2m to visual rest). The whole model is then lifted so the lowest tyre touches the floor (y=0)")]
		public WheelMount[] Wheels = Array.Empty<WheelMount>();

		[Tooltip("Per-car hand-tune: extra gap (m) between the tyres and the floor after normalization (0 = lowest tyre exactly on the floor)")]
		public float FloorGapMeters;

		[Tooltip("Целевой зазор (м) от верха шины до кромки арки у осевшей машины. Одинаковый для передней и задней оси; у внедорожников и гиперкаров своё значение. 0 = взять значение по умолчанию из инструмента")]
		public float WheelArchGapMeters;

		[Tooltip("Mounts for complex body kits (spoiler/airintake/trim), root-local. Empty if the car has none.")]
		public BodyKitMount[] BodyKitMounts = Array.Empty<BodyKitMount>();
	}
}
