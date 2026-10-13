using System;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

[Serializable]
public class BrakeDiskItemData
{
	private float _randomOffset;

	private char _tempChar;

	private long _fakeSerialNumber;

	public BrakeDiscType BrakeDiscType;

	[Header("if it brake disk skip this row")]
	public int index;

	private int _placeholderIndex;

	private ObscuredBool _coinPrice;

	private string _tempLabel;

	private bool _tempCondition;

	public int order;

	private int _tempIndex;

	private string _extraField;

	public ObscuredInt price = 1000;

	[Header("If not 0 that Coin price is active")]
	public ObscuredBool isCoinPrice = true;

	public Texture PreviewTexture;
}
