using System;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;

public class GetterCoinsManager
{
	public enum Source
	{
		Default = 0,
		Synchronization = 1
	}

	public static Action<int, Source> CoinsChanged;

	public static void AddCoinsToPrefs(AdvancedInt addCoins)
	{
		(Get4LocalPrefs() + AdvancedInt.Abs(addCoins)).Get(out int value);
		ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "coins1", value);
		CoinsChanged?.Invoke(value, Source.Default);
	}

	public static int RemoveCoinsToPrefs(AdvancedInt removeCoins)
	{
		(Get4LocalPrefs() - AdvancedInt.Abs(removeCoins)).Get(out int value);
		ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "coins1", value);
		CoinsChanged?.Invoke(value, Source.Default);
		return value;
	}

	public static void Set2LocalPrefs(AdvancedInt newCoins, Source source = Source.Default)
	{
		newCoins.Get(out int value);
		value = Mathf.Clamp(value, 0, 50000000);
		ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "coins1", value);
		CoinsChanged?.Invoke(value, source);
	}

	public static AdvancedInt Get4LocalPrefs()
	{
		return new AdvancedInt(Mathf.Clamp(ObscuredPrefs.GetInt(SystemInfo.deviceUniqueIdentifier + "coins1"), 0, 50000000));
	}
}
