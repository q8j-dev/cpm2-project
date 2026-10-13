using System;
using UnityEngine.Rendering;

[Serializable]
public sealed class RainConeFadeVolume : VolumeComponent
{
	public BoolParameter EnableFade = new BoolParameter(value: false);

	public ClampedFloatParameter Target = new ClampedFloatParameter(1f, 0f, 1f);

	public ClampedFloatParameter InSpeed = new ClampedFloatParameter(16f, 0.01f, 100f);

	public ClampedFloatParameter OutSpeed = new ClampedFloatParameter(16f, 0.01f, 100f);

	public BoolParameter UseUnscaledTime = new BoolParameter(value: false);
}
