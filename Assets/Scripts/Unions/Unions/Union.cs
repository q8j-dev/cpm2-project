using System;

namespace Unions
{
	public class Union<T1, T2>
	{
		private T1 _value1;

		private T2 _value2;

		private int _valueIndex;

		public static implicit operator Union<T1, T2>(T1 value1)
		{
			return FromValue1(value1);
		}

		public static implicit operator Union<T1, T2>(T2 value2)
		{
			return FromValue2(value2);
		}

		private static Union<T1, T2> FromValue1(T1 value)
		{
			return new Union<T1, T2>
			{
				_value1 = value,
				_valueIndex = 1
			};
		}

		private static Union<T1, T2> FromValue2(T2 value2)
		{
			return new Union<T1, T2>
			{
				_value2 = value2,
				_valueIndex = 2
			};
		}

		public void Match(Action<T1> handler1, Action<T2> handler2)
		{
			switch (_valueIndex)
			{
			case 1:
				handler1(_value1);
				break;
			case 2:
				handler2(_value2);
				break;
			default:
				throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union"));
			}
		}

		public T Match<T>(Func<T1, T> handler1, Func<T2, T> handler2)
		{
			return _valueIndex switch
			{
				1 => handler1(_value1), 
				2 => handler2(_value2), 
				_ => throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union")), 
			};
		}
	}
	public class Union<T1, T2, T3>
	{
		private T1 _value1;

		private T2 _value2;

		private T3 _value3;

		private int _valueIndex;

		public static implicit operator Union<T1, T2, T3>(T1 value1)
		{
			return FromValue1(value1);
		}

		public static implicit operator Union<T1, T2, T3>(T2 value2)
		{
			return FromValue2(value2);
		}

		public static implicit operator Union<T1, T2, T3>(T3 value3)
		{
			return FromValue3(value3);
		}

		private static Union<T1, T2, T3> FromValue1(T1 value)
		{
			return new Union<T1, T2, T3>
			{
				_value1 = value,
				_valueIndex = 1
			};
		}

		private static Union<T1, T2, T3> FromValue2(T2 value)
		{
			return new Union<T1, T2, T3>
			{
				_value2 = value,
				_valueIndex = 2
			};
		}

		private static Union<T1, T2, T3> FromValue3(T3 value)
		{
			return new Union<T1, T2, T3>
			{
				_value3 = value,
				_valueIndex = 3
			};
		}

		public void Match(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3)
		{
			switch (_valueIndex)
			{
			case 1:
				handler1(_value1);
				break;
			case 2:
				handler2(_value2);
				break;
			case 3:
				handler3(_value3);
				break;
			default:
				throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union"));
			}
		}

		public T Match<T>(Func<T1, T> handler1, Func<T2, T> handler2, Func<T3, T> handler3)
		{
			return _valueIndex switch
			{
				1 => handler1(_value1), 
				2 => handler2(_value2), 
				3 => handler3(_value3), 
				_ => throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union")), 
			};
		}
	}
	public class Union<T1, T2, T3, T4>
	{
		private T1 _value1;

		private T2 _value2;

		private T3 _value3;

		private T4 _value4;

		private int _valueIndex;

		public static implicit operator Union<T1, T2, T3, T4>(T1 value1)
		{
			return FromValue1(value1);
		}

		public static implicit operator Union<T1, T2, T3, T4>(T2 value2)
		{
			return FromValue2(value2);
		}

		public static implicit operator Union<T1, T2, T3, T4>(T3 value3)
		{
			return FromValue3(value3);
		}

		public static implicit operator Union<T1, T2, T3, T4>(T4 value4)
		{
			return FromValue4(value4);
		}

		private static Union<T1, T2, T3, T4> FromValue1(T1 value)
		{
			return new Union<T1, T2, T3, T4>
			{
				_value1 = value,
				_valueIndex = 1
			};
		}

		private static Union<T1, T2, T3, T4> FromValue2(T2 value)
		{
			return new Union<T1, T2, T3, T4>
			{
				_value2 = value,
				_valueIndex = 2
			};
		}

		private static Union<T1, T2, T3, T4> FromValue3(T3 value)
		{
			return new Union<T1, T2, T3, T4>
			{
				_value3 = value,
				_valueIndex = 3
			};
		}

		private static Union<T1, T2, T3, T4> FromValue4(T4 value)
		{
			return new Union<T1, T2, T3, T4>
			{
				_value4 = value,
				_valueIndex = 4
			};
		}

		public void Match(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4)
		{
			switch (_valueIndex)
			{
			case 1:
				handler1(_value1);
				break;
			case 2:
				handler2(_value2);
				break;
			case 3:
				handler3(_value3);
				break;
			case 4:
				handler4(_value4);
				break;
			default:
				throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union"));
			}
		}

		public T Match<T>(Func<T1, T> handler1, Func<T2, T> handler2, Func<T3, T> handler3, Func<T4, T> handler4)
		{
			return _valueIndex switch
			{
				1 => handler1(_value1), 
				2 => handler2(_value2), 
				3 => handler3(_value3), 
				4 => handler4(_value4), 
				_ => throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union")), 
			};
		}
	}
	public class Union<T1, T2, T3, T4, T5>
	{
		private T1 _value1;

		private T2 _value2;

		private T3 _value3;

		private T4 _value4;

		private T5 _value5;

		private int _valueIndex;

		public static implicit operator Union<T1, T2, T3, T4, T5>(T1 value1)
		{
			return FromValue1(value1);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5>(T2 value2)
		{
			return FromValue2(value2);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5>(T3 value3)
		{
			return FromValue3(value3);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5>(T4 value4)
		{
			return FromValue4(value4);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5>(T5 value5)
		{
			return FromValue5(value5);
		}

		private static Union<T1, T2, T3, T4, T5> FromValue1(T1 value)
		{
			return new Union<T1, T2, T3, T4, T5>
			{
				_value1 = value,
				_valueIndex = 1
			};
		}

		private static Union<T1, T2, T3, T4, T5> FromValue2(T2 value)
		{
			return new Union<T1, T2, T3, T4, T5>
			{
				_value2 = value,
				_valueIndex = 2
			};
		}

		private static Union<T1, T2, T3, T4, T5> FromValue3(T3 value)
		{
			return new Union<T1, T2, T3, T4, T5>
			{
				_value3 = value,
				_valueIndex = 3
			};
		}

		private static Union<T1, T2, T3, T4, T5> FromValue4(T4 value)
		{
			return new Union<T1, T2, T3, T4, T5>
			{
				_value4 = value,
				_valueIndex = 4
			};
		}

		private static Union<T1, T2, T3, T4, T5> FromValue5(T5 value)
		{
			return new Union<T1, T2, T3, T4, T5>
			{
				_value5 = value,
				_valueIndex = 5
			};
		}

		public void Match(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5)
		{
			switch (_valueIndex)
			{
			case 1:
				handler1(_value1);
				break;
			case 2:
				handler2(_value2);
				break;
			case 3:
				handler3(_value3);
				break;
			case 4:
				handler4(_value4);
				break;
			case 5:
				handler5(_value5);
				break;
			default:
				throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union"));
			}
		}

		public T Match<T>(Func<T1, T> handler1, Func<T2, T> handler2, Func<T3, T> handler3, Func<T4, T> handler4, Func<T5, T> handler5)
		{
			return _valueIndex switch
			{
				1 => handler1(_value1), 
				2 => handler2(_value2), 
				3 => handler3(_value3), 
				4 => handler4(_value4), 
				5 => handler5(_value5), 
				_ => throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union")), 
			};
		}
	}
	public class Union<T1, T2, T3, T4, T5, T6>
	{
		private T1 _value1;

		private T2 _value2;

		private T3 _value3;

		private T4 _value4;

		private T5 _value5;

		private T6 _value6;

		private int _valueIndex;

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T1 value1)
		{
			return FromValue1(value1);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T2 value2)
		{
			return FromValue2(value2);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T3 value3)
		{
			return FromValue3(value3);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T4 value4)
		{
			return FromValue4(value4);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T5 value5)
		{
			return FromValue5(value5);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T6 value6)
		{
			return FromValue6(value6);
		}

		private static Union<T1, T2, T3, T4, T5, T6> FromValue1(T1 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6>
			{
				_value1 = value,
				_valueIndex = 1
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6> FromValue2(T2 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6>
			{
				_value2 = value,
				_valueIndex = 2
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6> FromValue3(T3 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6>
			{
				_value3 = value,
				_valueIndex = 3
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6> FromValue4(T4 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6>
			{
				_value4 = value,
				_valueIndex = 4
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6> FromValue5(T5 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6>
			{
				_value5 = value,
				_valueIndex = 5
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6> FromValue6(T6 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6>
			{
				_value6 = value,
				_valueIndex = 6
			};
		}

		public void Match(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6)
		{
			switch (_valueIndex)
			{
			case 1:
				handler1(_value1);
				break;
			case 2:
				handler2(_value2);
				break;
			case 3:
				handler3(_value3);
				break;
			case 4:
				handler4(_value4);
				break;
			case 5:
				handler5(_value5);
				break;
			case 6:
				handler6(_value6);
				break;
			default:
				throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union"));
			}
		}

		public T Match<T>(Func<T1, T> handler1, Func<T2, T> handler2, Func<T3, T> handler3, Func<T4, T> handler4, Func<T5, T> handler5, Func<T6, T> handler6)
		{
			return _valueIndex switch
			{
				1 => handler1(_value1), 
				2 => handler2(_value2), 
				3 => handler3(_value3), 
				4 => handler4(_value4), 
				5 => handler5(_value5), 
				6 => handler6(_value6), 
				_ => throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union")), 
			};
		}
	}
	public class Union<T1, T2, T3, T4, T5, T6, T7>
	{
		private T1 _value1;

		private T2 _value2;

		private T3 _value3;

		private T4 _value4;

		private T5 _value5;

		private T6 _value6;

		private T7 _value7;

		private int _valueIndex;

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T1 value1)
		{
			return FromValue1(value1);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T2 value2)
		{
			return FromValue2(value2);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T3 value3)
		{
			return FromValue3(value3);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T4 value4)
		{
			return FromValue4(value4);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T5 value5)
		{
			return FromValue5(value5);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T6 value6)
		{
			return FromValue6(value6);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T7 value7)
		{
			return FromValue7(value7);
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7> FromValue1(T1 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>
			{
				_value1 = value,
				_valueIndex = 1
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7> FromValue2(T2 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>
			{
				_value2 = value,
				_valueIndex = 2
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7> FromValue3(T3 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>
			{
				_value3 = value,
				_valueIndex = 3
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7> FromValue4(T4 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>
			{
				_value4 = value,
				_valueIndex = 4
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7> FromValue5(T5 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>
			{
				_value5 = value,
				_valueIndex = 5
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7> FromValue6(T6 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>
			{
				_value6 = value,
				_valueIndex = 6
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7> FromValue7(T7 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7>
			{
				_value7 = value,
				_valueIndex = 7
			};
		}

		public void Match(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7)
		{
			switch (_valueIndex)
			{
			case 1:
				handler1(_value1);
				break;
			case 2:
				handler2(_value2);
				break;
			case 3:
				handler3(_value3);
				break;
			case 4:
				handler4(_value4);
				break;
			case 5:
				handler5(_value5);
				break;
			case 6:
				handler6(_value6);
				break;
			case 7:
				handler7(_value7);
				break;
			default:
				throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union"));
			}
		}

		public T Match<T>(Func<T1, T> handler1, Func<T2, T> handler2, Func<T3, T> handler3, Func<T4, T> handler4, Func<T5, T> handler5, Func<T6, T> handler6, Func<T7, T> handler7)
		{
			return _valueIndex switch
			{
				1 => handler1(_value1), 
				2 => handler2(_value2), 
				3 => handler3(_value3), 
				4 => handler4(_value4), 
				5 => handler5(_value5), 
				6 => handler6(_value6), 
				7 => handler7(_value7), 
				_ => throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union")), 
			};
		}
	}
	public class Union<T1, T2, T3, T4, T5, T6, T7, T8>
	{
		private T1 _value1;

		private T2 _value2;

		private T3 _value3;

		private T4 _value4;

		private T5 _value5;

		private T6 _value6;

		private T7 _value7;

		private T8 _value8;

		private int _valueIndex;

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T1 value1)
		{
			return FromValue1(value1);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T2 value2)
		{
			return FromValue2(value2);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T3 value3)
		{
			return FromValue3(value3);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T4 value4)
		{
			return FromValue4(value4);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T5 value5)
		{
			return FromValue5(value5);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T6 value6)
		{
			return FromValue6(value6);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T7 value7)
		{
			return FromValue7(value7);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T8 value8)
		{
			return FromValue8(value8);
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8> FromValue1(T1 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>
			{
				_value1 = value,
				_valueIndex = 1
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8> FromValue2(T2 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>
			{
				_value2 = value,
				_valueIndex = 2
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8> FromValue3(T3 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>
			{
				_value3 = value,
				_valueIndex = 3
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8> FromValue4(T4 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>
			{
				_value4 = value,
				_valueIndex = 4
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8> FromValue5(T5 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>
			{
				_value5 = value,
				_valueIndex = 5
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8> FromValue6(T6 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>
			{
				_value6 = value,
				_valueIndex = 6
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8> FromValue7(T7 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>
			{
				_value7 = value,
				_valueIndex = 7
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8> FromValue8(T8 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8>
			{
				_value8 = value,
				_valueIndex = 8
			};
		}

		public void Match(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8)
		{
			switch (_valueIndex)
			{
			case 1:
				handler1(_value1);
				break;
			case 2:
				handler2(_value2);
				break;
			case 3:
				handler3(_value3);
				break;
			case 4:
				handler4(_value4);
				break;
			case 5:
				handler5(_value5);
				break;
			case 6:
				handler6(_value6);
				break;
			case 7:
				handler7(_value7);
				break;
			case 8:
				handler8(_value8);
				break;
			default:
				throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union"));
			}
		}

		public T Match<T>(Func<T1, T> handler1, Func<T2, T> handler2, Func<T3, T> handler3, Func<T4, T> handler4, Func<T5, T> handler5, Func<T6, T> handler6, Func<T7, T> handler7, Func<T8, T> handler8)
		{
			return _valueIndex switch
			{
				1 => handler1(_value1), 
				2 => handler2(_value2), 
				3 => handler3(_value3), 
				4 => handler4(_value4), 
				5 => handler5(_value5), 
				6 => handler6(_value6), 
				7 => handler7(_value7), 
				8 => handler8(_value8), 
				_ => throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union")), 
			};
		}
	}
	public class Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		private T1 _value1;

		private T2 _value2;

		private T3 _value3;

		private T4 _value4;

		private T5 _value5;

		private T6 _value6;

		private T7 _value7;

		private T8 _value8;

		private T9 _value9;

		private int _valueIndex;

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 value1)
		{
			return FromValue1(value1);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T2 value2)
		{
			return FromValue2(value2);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T3 value3)
		{
			return FromValue3(value3);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T4 value4)
		{
			return FromValue4(value4);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T5 value5)
		{
			return FromValue5(value5);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T6 value6)
		{
			return FromValue6(value6);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T7 value7)
		{
			return FromValue7(value7);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T8 value8)
		{
			return FromValue8(value8);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T9 value9)
		{
			return FromValue9(value9);
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromValue1(T1 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			{
				_value1 = value,
				_valueIndex = 1
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromValue2(T2 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			{
				_value2 = value,
				_valueIndex = 2
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromValue3(T3 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			{
				_value3 = value,
				_valueIndex = 3
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromValue4(T4 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			{
				_value4 = value,
				_valueIndex = 4
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromValue5(T5 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			{
				_value5 = value,
				_valueIndex = 5
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromValue6(T6 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			{
				_value6 = value,
				_valueIndex = 6
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromValue7(T7 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			{
				_value7 = value,
				_valueIndex = 7
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromValue8(T8 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			{
				_value8 = value,
				_valueIndex = 8
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9> FromValue9(T9 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9>
			{
				_value9 = value,
				_valueIndex = 9
			};
		}

		public void Match(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8, Action<T9> handler9)
		{
			switch (_valueIndex)
			{
			case 1:
				handler1(_value1);
				break;
			case 2:
				handler2(_value2);
				break;
			case 3:
				handler3(_value3);
				break;
			case 4:
				handler4(_value4);
				break;
			case 5:
				handler5(_value5);
				break;
			case 6:
				handler6(_value6);
				break;
			case 7:
				handler7(_value7);
				break;
			case 8:
				handler8(_value8);
				break;
			case 9:
				handler9(_value9);
				break;
			default:
				throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union"));
			}
		}

		public T Match<T>(Func<T1, T> handler1, Func<T2, T> handler2, Func<T3, T> handler3, Func<T4, T> handler4, Func<T5, T> handler5, Func<T6, T> handler6, Func<T7, T> handler7, Func<T8, T> handler8, Func<T9, T> handler9)
		{
			return _valueIndex switch
			{
				1 => handler1(_value1), 
				2 => handler2(_value2), 
				3 => handler3(_value3), 
				4 => handler4(_value4), 
				5 => handler5(_value5), 
				6 => handler6(_value6), 
				7 => handler7(_value7), 
				8 => handler8(_value8), 
				9 => handler9(_value9), 
				_ => throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union")), 
			};
		}
	}
	public class Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
	{
		private T1 _value1;

		private T2 _value2;

		private T3 _value3;

		private T4 _value4;

		private T5 _value5;

		private T6 _value6;

		private T7 _value7;

		private T8 _value8;

		private T9 _value9;

		private T10 _value10;

		private int _valueIndex;

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 value1)
		{
			return FromValue1(value1);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T2 value2)
		{
			return FromValue2(value2);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T3 value3)
		{
			return FromValue3(value3);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T4 value4)
		{
			return FromValue4(value4);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T5 value5)
		{
			return FromValue5(value5);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T6 value6)
		{
			return FromValue6(value6);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T7 value7)
		{
			return FromValue7(value7);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T8 value8)
		{
			return FromValue8(value8);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T9 value9)
		{
			return FromValue9(value9);
		}

		public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T10 value10)
		{
			return FromValue10(value10);
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue1(T1 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value1 = value,
				_valueIndex = 1
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue2(T2 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value2 = value,
				_valueIndex = 2
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue3(T3 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value3 = value,
				_valueIndex = 3
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue4(T4 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value4 = value,
				_valueIndex = 4
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue5(T5 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value5 = value,
				_valueIndex = 5
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue6(T6 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value6 = value,
				_valueIndex = 6
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue7(T7 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value7 = value,
				_valueIndex = 7
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue8(T8 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value8 = value,
				_valueIndex = 8
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue9(T9 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value9 = value,
				_valueIndex = 9
			};
		}

		private static Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> FromValue10(T10 value)
		{
			return new Union<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
			{
				_value10 = value,
				_valueIndex = 10
			};
		}

		public void Match(Action<T1> handler1, Action<T2> handler2, Action<T3> handler3, Action<T4> handler4, Action<T5> handler5, Action<T6> handler6, Action<T7> handler7, Action<T8> handler8, Action<T9> handler9, Action<T10> handler10)
		{
			switch (_valueIndex)
			{
			case 1:
				handler1(_value1);
				break;
			case 2:
				handler2(_value2);
				break;
			case 3:
				handler3(_value3);
				break;
			case 4:
				handler4(_value4);
				break;
			case 5:
				handler5(_value5);
				break;
			case 6:
				handler6(_value6);
				break;
			case 7:
				handler7(_value7);
				break;
			case 8:
				handler8(_value8);
				break;
			case 9:
				handler9(_value9);
				break;
			case 10:
				handler10(_value10);
				break;
			default:
				throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union"));
			}
		}

		public T Match<T>(Func<T1, T> handler1, Func<T2, T> handler2, Func<T3, T> handler3, Func<T4, T> handler4, Func<T5, T> handler5, Func<T6, T> handler6, Func<T7, T> handler7, Func<T8, T> handler8, Func<T9, T> handler9, Func<T10, T> handler10)
		{
			return _valueIndex switch
			{
				1 => handler1(_value1), 
				2 => handler2(_value2), 
				3 => handler3(_value3), 
				4 => handler4(_value4), 
				5 => handler5(_value5), 
				6 => handler6(_value6), 
				7 => handler7(_value7), 
				8 => handler8(_value8), 
				9 => handler9(_value9), 
				10 => handler10(_value10), 
				_ => throw new Exception(string.Format("Match exception: {0} in {1}", _valueIndex, "Union")), 
			};
		}
	}
}
