using System;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

[Serializable]
public class OneVinylsPreset
{
	private bool _isActivated;

	private int _hiddenIdentifier;

	private ObscuredInt _reservedData;

	private double _calculationFactor;

	private string _logMessage;

	private Color _internalFlag;

	private long _timestamp;

	public VynilOneCar vynilOneCarList;

	public VynilOneCar windowVynilOneCarList;

	private char _initialCharacter;

	private ulong _versionCode;

	private bool _testToggle;

	private float _debugFactor;

	private string _note;

	public ObscuredInt price;

	public Color carColor;
}
