using System;

namespace _Car_Parking.Scripts.Notification
{
	[Serializable]
	public class EquipmentKey : IEquatable<EquipmentKey>
	{
		private readonly int _type;

		private readonly int _index;

		private readonly int _gender;

		public EquipmentKey(int type, int index, int gender)
		{
			_type = type;
			_index = index;
			_gender = gender;
		}

		public bool Equals(EquipmentKey other)
		{
			if (other == null)
			{
				return false;
			}
			if (_type == other._type && _index == other._index)
			{
				return _gender == other._gender;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((17 * 31 + _type) * 31 + _index) * 31 + _gender;
		}
	}
}
