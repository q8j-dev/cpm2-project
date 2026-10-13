using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Person.Locomotion.Config;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class PersonSlopeRules
	{
		private readonly PersonLocomotionConfig _config;

		public float MaxSlopeAngle
		{
			get
			{
				if (!UseArenaRules)
				{
					return _config.MaxSlopeAngle;
				}
				return _config.ArenaMaxSlopeAngle;
			}
		}

		public float SlideStartAngle
		{
			get
			{
				if (!UseArenaRules)
				{
					return _config.SlopeSlideStartAngle;
				}
				return _config.ArenaSlopeSlideStartAngle;
			}
		}

		public float SlideFullAngle
		{
			get
			{
				if (!UseArenaRules)
				{
					return _config.SlopeSlideFullAngle;
				}
				return _config.ArenaSlopeSlideFullAngle;
			}
		}

		private static FreeDriveProvider Provider
		{
			get
			{
				if (!V.CoreGameManager)
				{
					return null;
				}
				return V.CoreGameManager.FreeDriveProvider;
			}
		}

		private bool UseArenaRules
		{
			get
			{
				FreeDriveProvider provider = Provider;
				if (provider != null)
				{
					return provider.RoomArenaId == ArenaId.ObstacleRace_1;
				}
				return false;
			}
		}

		public PersonSlopeRules(PersonLocomotionConfig config)
		{
			_config = config;
		}
	}
}
