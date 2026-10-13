using System;
using Photon.Pun;

namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	public class PackerPipeline<T1>
	{
		private readonly PackerBase<T1> _packer1;

		public PackerPipeline(PackerBase<T1> packer1)
		{
			_packer1 = packer1;
		}

		public long Pack(T1 t1)
		{
			long current = 0L;
			_packer1.Pack(ref current, t1);
			return current;
		}

		public T1 Unpack(long packed)
		{
			return _packer1.Unpack(ref packed);
		}

		public bool TrySendIfChanged<T>(ref long prev, PhotonStream stream, T1 t1, Func<long, T> transformer, bool ignoreCondition = false)
		{
			if (ignoreCondition)
			{
				return false;
			}
			long num = Pack(t1);
			if (num == prev)
			{
				return false;
			}
			prev = num;
			stream.SendNext(transformer(num));
			return true;
		}

		public bool TryReceive<T>(PhotonStream stream, Func<T, long> transformer, Action<T1> callback)
		{
			if (stream.currentItem == stream.Count)
			{
				return false;
			}
			object obj = stream.PeekNext();
			if (obj is T)
			{
				_ = (T)obj;
				long packed = transformer((T)stream.ReceiveNext());
				T1 obj2 = Unpack(packed);
				callback(obj2);
				return true;
			}
			return false;
		}
	}
	public class PackerPipeline<T1, T2>
	{
		private readonly PackerBase<T1> _packer1;

		private readonly PackerBase<T2> _packer2;

		public PackerPipeline(PackerBase<T1> packer1, PackerBase<T2> packer2)
		{
			_packer1 = packer1;
			_packer2 = packer2;
		}

		public long Pack(T1 t1, T2 t2)
		{
			long current = 0L;
			_packer1.Pack(ref current, t1);
			_packer2.Pack(ref current, t2);
			return current;
		}

		public (T1, T2) Unpack(long packed)
		{
			T2 item = _packer2.Unpack(ref packed);
			return (_packer1.Unpack(ref packed), item);
		}

		public bool TrySendIfChanged<T>(ref long prev, PhotonStream stream, T1 t1, T2 t2, Func<long, T> transformer, bool ignoreCondition = false)
		{
			if (ignoreCondition)
			{
				return false;
			}
			long num = Pack(t1, t2);
			if (num == prev)
			{
				return false;
			}
			prev = num;
			stream.SendNext(transformer(num));
			return true;
		}

		public bool TryReceive<T>(PhotonStream stream, Func<T, long> transformer, Action<T1, T2> callback)
		{
			if (stream.currentItem == stream.Count)
			{
				return false;
			}
			object obj = stream.PeekNext();
			if (obj is T)
			{
				_ = (T)obj;
				long packed = transformer((T)stream.ReceiveNext());
				var (arg, arg2) = Unpack(packed);
				callback(arg, arg2);
				return true;
			}
			return false;
		}
	}
	public class PackerPipeline<T1, T2, T3>
	{
		private readonly PackerBase<T1> _packer1;

		private readonly PackerBase<T2> _packer2;

		private readonly PackerBase<T3> _packer3;

		public PackerPipeline(PackerBase<T1> packer1, PackerBase<T2> packer2, PackerBase<T3> packer3)
		{
			_packer1 = packer1;
			_packer2 = packer2;
			_packer3 = packer3;
		}

		public long Pack(T1 t1, T2 t2, T3 t3)
		{
			long current = 0L;
			_packer1.Pack(ref current, t1);
			_packer2.Pack(ref current, t2);
			_packer3.Pack(ref current, t3);
			return current;
		}

		public (T1, T2, T3) Unpack(long packed)
		{
			T3 item = _packer3.Unpack(ref packed);
			T2 item2 = _packer2.Unpack(ref packed);
			return (_packer1.Unpack(ref packed), item2, item);
		}

		public bool TrySendIfChanged<T>(ref long prev, PhotonStream stream, T1 t1, T2 t2, T3 t3, Func<long, T> transformer, bool ignoreCondition = false)
		{
			if (ignoreCondition)
			{
				return false;
			}
			long num = Pack(t1, t2, t3);
			if (num == prev)
			{
				return false;
			}
			prev = num;
			stream.SendNext(transformer(num));
			return true;
		}

		public bool TryReceive<T>(PhotonStream stream, Func<T, long> transformer, Action<T1, T2, T3> callback)
		{
			if (stream.currentItem == stream.Count)
			{
				return false;
			}
			object obj = stream.PeekNext();
			if (obj is T)
			{
				_ = (T)obj;
				long packed = transformer((T)stream.ReceiveNext());
				var (arg, arg2, arg3) = Unpack(packed);
				callback(arg, arg2, arg3);
				return true;
			}
			return false;
		}
	}
	public class PackerPipeline<T1, T2, T3, T4>
	{
		private readonly PackerBase<T1> _packer1;

		private readonly PackerBase<T2> _packer2;

		private readonly PackerBase<T3> _packer3;

		private readonly PackerBase<T4> _packer4;

		public PackerPipeline(PackerBase<T1> packer1, PackerBase<T2> packer2, PackerBase<T3> packer3, PackerBase<T4> packer4)
		{
			_packer1 = packer1;
			_packer2 = packer2;
			_packer3 = packer3;
			_packer4 = packer4;
		}

		public long Pack(T1 t1, T2 t2, T3 t3, T4 t4)
		{
			long current = 0L;
			_packer1.Pack(ref current, t1);
			_packer2.Pack(ref current, t2);
			_packer3.Pack(ref current, t3);
			_packer4.Pack(ref current, t4);
			return current;
		}

		public (T1, T2, T3, T4) Unpack(long packed)
		{
			T4 item = _packer4.Unpack(ref packed);
			T3 item2 = _packer3.Unpack(ref packed);
			T2 item3 = _packer2.Unpack(ref packed);
			return (_packer1.Unpack(ref packed), item3, item2, item);
		}

		public bool TrySendIfChanged<T>(ref long prev, PhotonStream stream, T1 t1, T2 t2, T3 t3, T4 t4, Func<long, T> transformer, bool ignoreCondition = false)
		{
			if (ignoreCondition)
			{
				return false;
			}
			long num = Pack(t1, t2, t3, t4);
			if (num == prev)
			{
				return false;
			}
			prev = num;
			stream.SendNext(transformer(num));
			return true;
		}

		public bool TryReceive<T>(PhotonStream stream, Func<T, long> transformer, Action<T1, T2, T3, T4> callback)
		{
			if (stream.currentItem == stream.Count)
			{
				return false;
			}
			object obj = stream.PeekNext();
			if (obj is T)
			{
				_ = (T)obj;
				long packed = transformer((T)stream.ReceiveNext());
				var (arg, arg2, arg3, arg4) = Unpack(packed);
				callback(arg, arg2, arg3, arg4);
				return true;
			}
			return false;
		}
	}
	public class PackerPipeline<T1, T2, T3, T4, T5, T6>
	{
		private readonly PackerBase<T1> _packer1;

		private readonly PackerBase<T2> _packer2;

		private readonly PackerBase<T3> _packer3;

		private readonly PackerBase<T4> _packer4;

		private readonly PackerBase<T5> _packer5;

		private readonly PackerBase<T6> _packer6;

		public PackerPipeline(PackerBase<T1> packer1, PackerBase<T2> packer2, PackerBase<T3> packer3, PackerBase<T4> packer4, PackerBase<T5> packer5, PackerBase<T6> packer6)
		{
			_packer1 = packer1;
			_packer2 = packer2;
			_packer3 = packer3;
			_packer4 = packer4;
			_packer5 = packer5;
			_packer6 = packer6;
		}

		public long Pack(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
		{
			long current = 0L;
			_packer1.Pack(ref current, t1);
			_packer2.Pack(ref current, t2);
			_packer3.Pack(ref current, t3);
			_packer4.Pack(ref current, t4);
			_packer5.Pack(ref current, t5);
			_packer6.Pack(ref current, t6);
			return current;
		}

		public (T1, T2, T3, T4, T5, T6) Unpack(long packed)
		{
			T6 item = _packer6.Unpack(ref packed);
			T5 item2 = _packer5.Unpack(ref packed);
			T4 item3 = _packer4.Unpack(ref packed);
			T3 item4 = _packer3.Unpack(ref packed);
			T2 item5 = _packer2.Unpack(ref packed);
			return (_packer1.Unpack(ref packed), item5, item4, item3, item2, item);
		}

		public bool TrySendIfChanged<T>(ref long prev, PhotonStream stream, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, Func<long, T> transformer, bool ignoreCondition = false)
		{
			if (ignoreCondition)
			{
				return false;
			}
			long num = Pack(t1, t2, t3, t4, t5, t6);
			if (num == prev)
			{
				return false;
			}
			prev = num;
			stream.SendNext(transformer(num));
			return true;
		}

		public bool TryReceive<T>(PhotonStream stream, Func<T, long> transformer, Action<T1, T2, T3, T4, T5, T6> callback)
		{
			if (stream.currentItem == stream.Count)
			{
				return false;
			}
			object obj = stream.PeekNext();
			if (obj is T)
			{
				_ = (T)obj;
				long packed = transformer((T)stream.ReceiveNext());
				var (arg, arg2, arg3, arg4, arg5, arg6) = Unpack(packed);
				callback(arg, arg2, arg3, arg4, arg5, arg6);
				return true;
			}
			return false;
		}
	}
	public class PackerPipeline<T1, T2, T3, T4, T5, T6, T7>
	{
		private readonly PackerBase<T1> _packer1;

		private readonly PackerBase<T2> _packer2;

		private readonly PackerBase<T3> _packer3;

		private readonly PackerBase<T4> _packer4;

		private readonly PackerBase<T5> _packer5;

		private readonly PackerBase<T6> _packer6;

		private readonly PackerBase<T7> _packer7;

		public PackerPipeline(PackerBase<T1> packer1, PackerBase<T2> packer2, PackerBase<T3> packer3, PackerBase<T4> packer4, PackerBase<T5> packer5, PackerBase<T6> packer6, PackerBase<T7> packer7)
		{
			_packer1 = packer1;
			_packer2 = packer2;
			_packer3 = packer3;
			_packer4 = packer4;
			_packer5 = packer5;
			_packer6 = packer6;
			_packer7 = packer7;
		}

		public long Pack(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
		{
			long current = 0L;
			_packer1.Pack(ref current, t1);
			_packer2.Pack(ref current, t2);
			_packer3.Pack(ref current, t3);
			_packer4.Pack(ref current, t4);
			_packer5.Pack(ref current, t5);
			_packer6.Pack(ref current, t6);
			_packer7.Pack(ref current, t7);
			return current;
		}

		public (T1, T2, T3, T4, T5, T6, T7) Unpack(long packed)
		{
			T7 item = _packer7.Unpack(ref packed);
			T6 item2 = _packer6.Unpack(ref packed);
			T5 item3 = _packer5.Unpack(ref packed);
			T4 item4 = _packer4.Unpack(ref packed);
			T3 item5 = _packer3.Unpack(ref packed);
			T2 item6 = _packer2.Unpack(ref packed);
			return (_packer1.Unpack(ref packed), item6, item5, item4, item3, item2, item);
		}

		public bool TrySendIfChanged<T>(ref long prev, PhotonStream stream, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, Func<long, T> transformer, bool ignoreCondition = false)
		{
			if (ignoreCondition)
			{
				return false;
			}
			long num = Pack(t1, t2, t3, t4, t5, t6, t7);
			if (num == prev)
			{
				return false;
			}
			prev = num;
			stream.SendNext(transformer(num));
			return true;
		}

		public bool TryReceive<T>(PhotonStream stream, Func<T, long> transformer, Action<T1, T2, T3, T4, T5, T6, T7> callback)
		{
			if (stream.currentItem == stream.Count)
			{
				return false;
			}
			object obj = stream.PeekNext();
			if (obj is T)
			{
				_ = (T)obj;
				long packed = transformer((T)stream.ReceiveNext());
				var (arg, arg2, arg3, arg4, arg5, arg6, arg7) = Unpack(packed);
				callback(arg, arg2, arg3, arg4, arg5, arg6, arg7);
				return true;
			}
			return false;
		}
	}
}
