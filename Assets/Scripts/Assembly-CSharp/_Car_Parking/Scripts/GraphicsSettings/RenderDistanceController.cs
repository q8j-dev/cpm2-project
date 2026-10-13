using System;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class RenderDistanceController : GraphicsDistanceController
	{
		public float Distance => base.CurrentDistance;

		protected override float DefaultDistance => 130f;

		public event Action<float> OnDistanceChanged;

		public RenderDistanceController()
		{
			SetSettings(PriorityType.Default, DefaultDistance);
		}

		protected override void OnCurrentSettingsDistanceChanged()
		{
			base.OnCurrentSettingsDistanceChanged();
			OnDistanceChanged?.Invoke(Distance);
		}
	}
}
