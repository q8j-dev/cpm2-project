using System;

namespace _Car_Parking.Scripts.EventSystem
{
	[Serializable]
	public enum EventType
	{
		None = -1,
		TimeAttack = 0,
		NewYearSnowManAttack = 2,
		NewYearPresents = 3,
		NewYearDecorations = 4,
		DriftTimeAttack = 5,
		TimeAttackNurb = 6,
		RallyOffroadEvent = 7,
		ObstacleRace = 8,
		PromoSteamWishList = 9,
		PromoSteamPurchase = 10,
		TimeAttackRally = RallyOffroadEvent,
		SteamExclusiveCar = 1000
	}
}
