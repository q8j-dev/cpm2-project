using System;
using CPMEngine;
using UnityEngine;

[Serializable]
public class TuningLimitsData
{
	[Header("Spring Length Limits in inch")]
	public float SpringLengthMin = 2f;

	public float SpringLengthMax = 20f;

	[Header("Spring Rate Limits in kgf/mm")]
	public float SpringRateMin = 0.8f;

	public float SpringRateMax = 10f;

	[Header("ShockAbsorber Limits in Nm")]
	public float BumpMin = 1000f;

	public float BumpMax = 13000f;

	public float ReboundMin = 2000f;

	public float ReboundMax = 14000f;

	[Header("AntiSwayBar Limits in Nm")]
	public float AntiSwayBarForceMin;

	public float AntiSwayBarForceMax = 10000f;

	[Header("Suspensions Limits in Degrees")]
	public float CamberMin = -10f;

	public float CamberMax = 5f;

	public float SteerAngleMin = 28f;

	public float SteerAngleMax = 70f;

	public float AckermanMin;

	public float AckermanMax = 100f;

	public float CasterMin = -10f;

	public float CasterMax = 10f;

	public float ToeMin = -10f;

	public float ToeMax = 10f;

	[Header("Brakes")]
	public float BrakesPistonsMax = 6f;

	[Header("Wheels")]
	public float MaxDiskDiameter = 22f;

	public float MaxWheelRadius = 0.4f;

	public float MaxWheelPressure = 3.5f;

	public float GetMinWheelPressure(TyreType tyreType)
	{
		return TyreTypeRanges.PressureFloor(tyreType);
	}

	public float GetMaxWheelPressure(float stockPressureBar, TyreType tyreType)
	{
		return TyreTypeRanges.PressureCeiling(tyreType, stockPressureBar, MaxWheelPressure);
	}
}
