namespace CPM.Shifter.Logic
{
	public static class BridgeMapping
	{
		public const byte EnumN = 0;

		public const byte EnumG1 = 1;

		public const byte EnumG2 = 2;

		public const byte EnumG3 = 3;

		public const byte EnumG4 = 4;

		public const byte EnumG5 = 5;

		public const byte EnumG6 = 6;

		public const byte EnumG7 = 7;

		public const byte EnumR = 8;

		public const byte EnumA = 9;

		public const byte EnumM = 10;

		public const byte EnumUp = 11;

		public const byte EnumDown = 12;

		public const byte EnumAM = 13;

		public const byte EnumP = 14;

		public const byte EnumS = 15;

		public const byte EnumL = 16;

		public const int SelectorPark = 1;

		public const int SelectorReverse = 2;

		public const int SelectorNeutral = 3;

		public const int SelectorDrive = 4;

		public const int SelectorManual = 5;

		public const int SelectorPlus = 6;

		public const int SelectorMinus = 7;

		public const int SelectorSport = 8;

		public const int SelectorLow = 9;

		public static int? MapGearEnumToAutoSelector(byte gearEnumValue, int currentSelector)
		{
			return gearEnumValue switch
			{
				14 => 1, 
				8 => 2, 
				0 => 3, 
				9 => 4, 
				10 => 5, 
				11 => 6, 
				12 => 7, 
				15 => 8, 
				16 => 9, 
				13 => (currentSelector == 5) ? 4 : 5, 
				_ => null, 
			};
		}

		public static int? MapGearEnumToManualGear(byte gearEnumValue)
		{
			return gearEnumValue switch
			{
				0 => 0, 
				8 => -1, 
				1 => 1, 
				2 => 2, 
				3 => 3, 
				4 => 4, 
				5 => 5, 
				6 => 6, 
				7 => 7, 
				_ => null, 
			};
		}
	}
}
