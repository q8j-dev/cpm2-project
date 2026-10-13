using System;
using UnityEngine.Rendering.Universal;

public static class LightLayerUtil
{
	public static int CalculateBitmask(int currentBitmask, Array enumValues, LightLayerEnum LightLayer)
	{
		foreach (LightLayerEnum enumValue in enumValues)
		{
			if (enumValue != LightLayerEnum.Everything)
			{
				int bitVal = (int)enumValue;
				bool set = enumValue == LightLayer;
				currentBitmask = SetBitmask(currentBitmask, bitVal, set);
			}
		}
		return currentBitmask;
	}

	public static int SetBitmask(int bitmask, int bitVal, bool set)
	{
		bitmask = ((!set) ? (bitmask & ~bitVal) : (bitmask | bitVal));
		return bitmask;
	}
}
