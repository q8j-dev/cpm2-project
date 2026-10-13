namespace _Car_Parking.Scripts.Audio
{
	public class RainSoundState
	{
		private readonly uint _rainStateGroup = AudioWrapper.GetId("State_Rain");

		private readonly uint _exteriorState = AudioWrapper.GetId("rain_ext");

		private readonly uint _buildingState = AudioWrapper.GetId("rain_int_building");

		private readonly uint _tunnelState = AudioWrapper.GetId("rain_int_tunnel");

		private readonly uint _carState = AudioWrapper.GetId("rain_int_car");

		private readonly uint _offState = AudioWrapper.GetId("rain_off");

		private bool _inCar;

		private int _tunnelZones;

		private int _buildingZones;

		private int _rainOffZones;

		private uint _appliedState;

		public void SetInCar(bool inCar)
		{
			if (_inCar != inCar)
			{
				_inCar = inCar;
				Apply();
			}
		}

		public void ZoneEntered(RainZoneType type)
		{
			switch (type)
			{
			case RainZoneType.None:
				return;
			case RainZoneType.Tunnel:
				_tunnelZones++;
				break;
			case RainZoneType.RainOff:
				_rainOffZones++;
				break;
			default:
				_buildingZones++;
				break;
			}
			Apply();
		}

		public void ZoneExited(RainZoneType type)
		{
			switch (type)
			{
			case RainZoneType.None:
				return;
			case RainZoneType.Tunnel:
				if (_tunnelZones > 0)
				{
					_tunnelZones--;
				}
				break;
			case RainZoneType.RainOff:
				if (_rainOffZones > 0)
				{
					_rainOffZones--;
				}
				break;
			default:
				if (_buildingZones > 0)
				{
					_buildingZones--;
				}
				break;
			}
			Apply();
		}

		public void EnsureApplied()
		{
			if (_appliedState == 0)
			{
				Apply();
			}
		}

		public void Reset()
		{
			_inCar = false;
			_tunnelZones = 0;
			_buildingZones = 0;
			_rainOffZones = 0;
			_appliedState = 0u;
		}

		private void Apply()
		{
			uint num = ((_rainOffZones > 0) ? _offState : ((_tunnelZones > 0) ? _tunnelState : ((_buildingZones > 0) ? _buildingState : ((!_inCar) ? _exteriorState : _carState))));
			if (num != _appliedState)
			{
				_appliedState = num;
				AudioWrapper.SetState(_rainStateGroup, num);
			}
		}
	}
}
