using System;
using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.CustomDataTypes
{
	public class AdvancedBoolean
	{
		private readonly ObscuredBool _v;

		private readonly ObscuredInt _value2;

		public AdvancedBoolean(bool v)
		{
			Random random = new Random();
			_value2 = (v ? random.Next(536, 1000) : random.Next(-1000, 536));
		}

		public void Get(out bool value)
		{
			value = (int)_value2 >= 536;
		}

		public static AdvancedBoolean operator |(AdvancedBoolean a, AdvancedBoolean b)
		{
			a.Get(out var value);
			b.Get(out var value2);
			return new AdvancedBoolean(value | value2);
		}

		public static AdvancedBoolean operator &(AdvancedBoolean a, AdvancedBoolean b)
		{
			a.Get(out var value);
			b.Get(out var value2);
			return new AdvancedBoolean(value & value2);
		}

		public static AdvancedBoolean operator !(AdvancedBoolean a)
		{
			a.Get(out var value);
			return new AdvancedBoolean(!value);
		}

		public static bool operator true(AdvancedBoolean a)
		{
			a.Get(out var value);
			return value;
		}

		public static bool operator false(AdvancedBoolean a)
		{
			a.Get(out var value);
			return !value;
		}

		public static bool operator ==(AdvancedBoolean a, AdvancedBoolean b)
		{
			return (int?)a?._value2 == (int?)b?._value2;
		}

		public static bool operator !=(AdvancedBoolean a, AdvancedBoolean b)
		{
			return (int?)a?._value2 != (int?)b?._value2;
		}

		public override bool Equals(object obj)
		{
			if (obj is AdvancedBoolean advancedBoolean)
			{
				return (int)_value2 == (int)advancedBoolean._value2;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _value2.GetHashCode();
		}
	}
}
