namespace _Car_Parking.Scripts.Other.Helpers
{
	public class TransformSaverForPrefabCar : TransformSaverForPrefab
	{
		public int CarId;

		public int NumberInterior = 1;

		protected override string PrefabPath
		{
			get
			{
				if (!GetComponentInParent<VehicleData>())
				{
					return "";
				}
				string text = ((NumberInterior > 1) ? $"{CarId}i_{NumberInterior}" : $"{CarId}i");
				return "Assets/_Car Parking/Art/Cars/Prefabs/interiors/" + text + ".prefab";
			}
		}
	}
}
