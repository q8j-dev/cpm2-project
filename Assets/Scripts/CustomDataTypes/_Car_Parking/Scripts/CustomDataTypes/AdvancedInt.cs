using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace _Car_Parking.Scripts.CustomDataTypes
{
	public class AdvancedInt
	{
		private ObscuredInt _value1;

		private ObscuredInt _value2;

		private ObscuredInt _value33;

		public AdvancedInt(ObscuredInt value)
		{
			_value33 = Random.Range(0, (int)value / 2);
			_value1 = (int)value - (int)_value33;
			_value2 = Random.Range(0, value);
		}

		public void Get(out int value)
		{
			value = (int)_value1 + (int)_value33;
		}

		public void Get(out ObscuredInt value)
		{
			value = (int)_value1 + (int)_value33;
		}

		public override string ToString()
		{
			Get(out int value);
			return value.ToString();
		}

		public static AdvancedInt Abs(AdvancedInt input)
		{
			input.Get(out int value);
			return new AdvancedInt(Mathf.Abs(value));
		}

		public static AdvancedInt operator +(AdvancedInt left, AdvancedInt right)
		{
			left.Get(out int value);
			right.Get(out int value2);
			return new AdvancedInt(value + value2);
		}

		public static AdvancedInt operator +(AdvancedInt left, int right)
		{
			left.Get(out int value);
			return new AdvancedInt(value + right);
		}

		public static AdvancedInt operator -(AdvancedInt left, AdvancedInt right)
		{
			left.Get(out int value);
			right.Get(out int value2);
			return new AdvancedInt(value - value2);
		}

		public static AdvancedInt operator -(AdvancedInt left, int right)
		{
			left.Get(out int value);
			return new AdvancedInt(value - right);
		}

		public static AdvancedInt Min(AdvancedInt a, AdvancedInt b)
		{
			a.Get(out int value);
			b.Get(out int value2);
			if (value >= value2)
			{
				return b;
			}
			return a;
		}

		public void Clamp(int minValue, int maxValue)
		{
			Get(out int value);
			value = Mathf.Clamp(value, minValue, maxValue);
			_value33 = Random.Range(0, value / 2);
			_value1 = value - (int)_value33;
			_value2 = Random.Range(0, value);
		}

		public static AdvancedInt Min(AdvancedInt a, int b)
		{
			a.Get(out int value);
			if (value >= b)
			{
				return new AdvancedInt(b);
			}
			return a;
		}

		public static AdvancedInt Max(AdvancedInt a, AdvancedInt b)
		{
			a.Get(out int value);
			b.Get(out int value2);
			if (value <= value2)
			{
				return b;
			}
			return a;
		}

		public static AdvancedInt Max(AdvancedInt a, int b)
		{
			a.Get(out int value);
			if (value <= b)
			{
				return new AdvancedInt(b);
			}
			return a;
		}

		public static bool operator ==(AdvancedInt left, AdvancedInt right)
		{
			if ((object)left == null && (object)right == null)
			{
				return true;
			}
			if ((object)left == null || (object)right == null)
			{
				return false;
			}
			left.Get(out int value);
			right.Get(out int value2);
			return value == value2;
		}

		public static bool operator ==(AdvancedInt a, int b)
		{
			if (a == null)
			{
				return false;
			}
			a.Get(out int value);
			return value == b;
		}

		public static bool operator !=(AdvancedInt left, AdvancedInt right)
		{
			return !(left == right);
		}

		public static bool operator !=(AdvancedInt a, int b)
		{
			if (a == null)
			{
				return true;
			}
			a.Get(out int value);
			return value != b;
		}

		public static bool operator >(AdvancedInt left, AdvancedInt right)
		{
			left.Get(out int value);
			right.Get(out int value2);
			return value > value2;
		}

		public static bool operator >(AdvancedInt a, int b)
		{
			a.Get(out int value);
			return value > b;
		}

		public static bool operator <(AdvancedInt left, AdvancedInt right)
		{
			left.Get(out int value);
			right.Get(out int value2);
			return value < value2;
		}

		public static bool operator <(AdvancedInt a, int b)
		{
			a.Get(out int value);
			return value < b;
		}

		public static bool operator >=(AdvancedInt left, AdvancedInt right)
		{
			left.Get(out int value);
			right.Get(out int value2);
			return value >= value2;
		}

		public static bool operator >=(AdvancedInt a, int b)
		{
			a.Get(out int value);
			return value >= b;
		}

		public static bool operator <=(AdvancedInt left, AdvancedInt right)
		{
			left.Get(out int value);
			right.Get(out int value2);
			return value <= value2;
		}

		public static bool operator <=(AdvancedInt a, int b)
		{
			a.Get(out int value);
			return value <= b;
		}

		public override bool Equals(object obj)
		{
			if (obj is AdvancedInt)
			{
				return this == (AdvancedInt)obj;
			}
			return false;
		}

		public override int GetHashCode()
		{
			Get(out int value);
			return value.GetHashCode();
		}
	}
}
