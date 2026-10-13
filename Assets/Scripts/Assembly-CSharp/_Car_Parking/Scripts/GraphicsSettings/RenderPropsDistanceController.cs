using System;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class RenderPropsDistanceController : GraphicsDistanceController
	{
		public (float, float) Distances
		{
			get
			{
				float currentDistance = base.CurrentDistance;
				return (currentDistance - 0.1f, currentDistance / 3f - 0.1f);
			}
		}

		protected override float DefaultDistance => 60f;

		public event Action<float, float> OnDistanceChanged;

		public RenderPropsDistanceController()
		{
			SetSettings(PriorityType.Default, DefaultDistance);
		}

		protected override void OnCurrentSettingsDistanceChanged()
		{
			base.OnCurrentSettingsDistanceChanged();
			(float, float) distances = Distances;
			OnDistanceChanged?.Invoke(distances.Item1, distances.Item2);
		}
	}
}
