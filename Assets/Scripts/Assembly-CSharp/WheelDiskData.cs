using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

[Serializable]
public class WheelDiskData
{
	public Color color = Color.white;

	public int index;

	public int order;

	public ObscuredInt price = 1000;

	[Header("If not 0 that Coin price is active")]
	public bool isCoinPrice;

	public bool isTwoPartDisk;

	public bool IsInteractableUiItem { get; set; } = true;

	public bool ShowPrice { get; set; } = true;
}
