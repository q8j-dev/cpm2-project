using System;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Racing_Scripts;

public class RallyLockableUiToggleElement : LockableUiToggleElement
{
	public V.placeToSpawn PlaceToSpawn;

	public void Init(RallyLocationData rallyLocationData, int index, Action<int> onTrue, Action<int> onFalse = null)
	{
		_onTrue = onTrue;
		_onFalse = onFalse;
		PlaceToSpawn = rallyLocationData.PlaceToSpawn;
		Index = index;
		SetSprite(rallyLocationData.LocationLogo);
		SetText(new LocalizedString(rallyLocationData.NameKey));
		ReuseListeners();
	}
}
