using System;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	[Serializable]
	public enum InputGroupsEnum : byte
	{
		None = 0,
		Ui = 1,
		Garage = 2,
		Person = 3,
		PaintBase = 4,
		CarOther = 5,
		Map = 6,
		RacingMap = 7,
		Drone = 8,
		PhotoMod = 9,
		CarRacing = 10,
		CarDrag = 11,
		DynoRun = 12,
		VisualTuning = 13,
		PaintSelection = 14,
		PaintColor = 15,
		PaintSize = 16,
		CarFree = 17,
		CarCircleMenu = 18,
		PersonCircleMenu = 19,
		PaintSelectedOne = 20,
		PaintMultiSelection = 21,
		PaintMultiDone = 22,
		CarPassenger = 23,
		CarPassengerCircleMenu = 24,
		PaintText = 25,
		PaintPlates = 26,
		PaintNone = 27,
		NoInput = 28,
		CarRally = 29,
		PersonDrag = 30,
		Spectator = 31,
		CarFreeRace = 32,
		CarFreeRaceCircleMenu = 33,
		MenuWithRotation = 34,
		CameraSetup = 35,
		UiWithChat = 36,
		PersonOther = 37,
		DragSpectator = 38
	}
}
