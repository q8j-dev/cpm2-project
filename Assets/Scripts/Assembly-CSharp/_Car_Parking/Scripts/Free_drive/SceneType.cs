using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Free_drive
{
	public enum SceneType
	{
		[EnumMember(Value = "None")]
		None = -1,
		[EnumMember(Value = "Main City")]
		MainCity = 0,
		[EnumMember(Value = "Highway")]
		Highway2 = 1,
		[EnumMember(Value = "Desert")]
		Desert = 2,
		[EnumMember(Value = "Mountain")]
		Mountain = 3,
		[EnumMember(Value = "Race Track")]
		RaceTrack = 4,
		[EnumMember(Value = "City 2")]
		City2 = 5,
		[EnumMember(Value = "Island")]
		Island = 6,
		[EnumMember(Value = "Road 1")]
		Road1 = 7,
		[EnumMember(Value = "Road 2")]
		Road2 = 8,
		[EnumMember(Value = "Straight Road")]
		StraightRoad = 9,
		[EnumMember(Value = "Home 2")]
		Home2 = 10,
		[EnumMember(Value = "Off Road")]
		OffRoad = 11,
		[EnumMember(Value = "Richmond")]
		Richmond = 12,
		[EnumMember(Value = "Japan")]
		Japan = 13,
		[EnumMember(Value = "Japan Suburban")]
		JapanSuburban = 14,
		[EnumMember(Value = "Rich Island")]
		RichIsland = 15,
		[EnumMember(Value = "City Main Race Track")]
		CityMainRaceTrack = 16,
		[EnumMember(Value = "Tropical Island")]
		TropicalIsland = 17,
		[EnumMember(Value = "Nurburgring")]
		Nurburgring = 18,
		[EnumMember(Value = "JunctionNurburgring")]
		JunctionNurburgring = 19,
		[EnumMember(Value = "Italy")]
		Italy = 20,
		[EnumMember(Value = "RichIslandRoad")]
		RichIslandRoad = 21
	}
}
