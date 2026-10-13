using System;

namespace _Car_Parking.Scripts.Rally.Data
{
	[Serializable]
	public enum RallySequenceTurnType
	{
		None = 0,
		Start = 1,
		KinkLeft = 2,
		KinkRight = 3,
		FlatLeft = 4,
		FlatRight = 5,
		Left6 = 6,
		Right6 = 7,
		Left5 = 8,
		Right5 = 9,
		Left4 = 10,
		Right4 = 11,
		Left3 = 12,
		Right3 = 13,
		Left2 = 14,
		Right2 = 15,
		Left1 = 16,
		Right1 = 17,
		SquareRight = 18,
		SquareLeft = 19,
		SuddenLeft = 20,
		SuddenRight = 21,
		HairpinLeft = 22,
		HairpinRight = 23,
		TightLeft = 24,
		TightRight = 25,
		ChicaneLeft = 26,
		ChicaneRight = 27,
		Finish = 28,
		Stop = 29,
		Kinks = 30
	}
}
