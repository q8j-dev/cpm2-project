using System;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

[Serializable]
public class BrakeDiskItemData
{
	public BrakeDiscType BrakeDiscType;

	[Header("if it brake disk skip this row")]
	public int index;

	public int order;

	public ObscuredInt price = 1000;

	[Header("If not 0 that Coin price is active")]
	public ObscuredBool isCoinPrice = true;

	public Texture PreviewTexture;
}
