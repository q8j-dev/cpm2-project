using UnityEngine;
using VLB;

namespace _Car_Parking.Scripts.Other
{
	public class LampLightVolumetricLightBeam : LampLight
	{
		[SerializeField]
		private VolumetricLightBeam LightBeam;

		[SerializeField]
		private float MaxBeamIntensity = 6f;

		public override float Intensity
		{
			get
			{
				if (!LightBeam)
				{
					return 0f;
				}
				return LightBeam.intensityInside / MaxBeamIntensity;
			}
			protected set
			{
				if ((bool)LightBeam)
				{
					LightBeam.intensityInside = value * MaxBeamIntensity;
					LightBeam.UpdateAfterManualPropertyChange();
				}
			}
		}
	}
}
