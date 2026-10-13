using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	public enum NameType
	{
		[EnumMember(Value = "Teddy Donat")]
		TeddyDonat = 0,
		[EnumMember(Value = "Red Rocks Hot")]
		RedRocksHot = 1,
		[EnumMember(Value = "Real Burgers")]
		RealBurgers = 2,
		[EnumMember(Value = "10 Ten")]
		Ten10 = 3,
		[EnumMember(Value = "Starbooks Coffee")]
		StarbooksCoffee = 4,
		[EnumMember(Value = "Grand Rock Motel")]
		GrandRockMotel = 5
	}
}
