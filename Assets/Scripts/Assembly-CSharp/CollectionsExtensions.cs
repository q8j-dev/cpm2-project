using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class CollectionsExtensions
{
	public class EnumerationEntry<T>
	{
		public readonly int Index;

		public readonly T Value;

		public readonly bool First;

		public readonly bool Last;

		public EnumerationEntry(int index, T value, bool first, bool last)
		{
			Index = index;
			Value = value;
			First = first;
			Last = last;
		}

		public void Deconstruct(out int index, out T item)
		{
			index = Index;
			item = Value;
		}
	}

	[CompilerGenerated]
	private sealed class _003CEnumerateWithFirst_003Ed__4<T> : IEnumerable<(int, T, bool)>, IEnumerable, IEnumerator<(int, T, bool)>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private (int, T, bool) _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private IEnumerable<T> source;

		public IEnumerable<T> _003C_003E3__source;

		private int _003Cindex_003E5__2;

		private IEnumerator<T> _003C_003E7__wrap2;

		(int, T, bool) IEnumerator<(int, T, bool)>.Current
		{
			[DebuggerHidden]
			get
			{
				return System_002ECollections_002EGeneric_002EIEnumerator_003C_0028System_002EInt32_002CT_002CSystem_002EBoolean_0029_003E_002Eget_Current();
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CEnumerateWithFirst_003Ed__4(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
		}

		private bool MoveNext()
		{
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Cindex_003E5__2 = 0;
					_003C_003E7__wrap2 = source.GetEnumerator();
					_003C_003E1__state = -3;
					break;
				case 1:
					_003C_003E1__state = -3;
					break;
				}
				if (_003C_003E7__wrap2.MoveNext())
				{
					T current = _003C_003E7__wrap2.Current;
					_003C_003E2__current = (_003Cindex_003E5__2, current, _003Cindex_003E5__2++ == 0);
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003Em__Finally1();
				_003C_003E7__wrap2 = null;
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003C_003E7__wrap2 != null)
			{
				_003C_003E7__wrap2.Dispose();
			}
		}

		[DebuggerHidden]
		private (int, T, bool) System_002ECollections_002EGeneric_002EIEnumerator_003C_0028System_002EInt32_002CT_002CSystem_002EBoolean_0029_003E_002Eget_Current()
		{
			return _003C_003E2__current;
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<(int, T, bool)> IEnumerable<(int, T, bool)>.GetEnumerator()
		{
			_003CEnumerateWithFirst_003Ed__4<T> obj;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				obj = this;
			}
			else
			{
				obj = new _003CEnumerateWithFirst_003Ed__4<T>(0);
			}
			obj.source = _003C_003E3__source;
			return obj;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<(int, T, bool)>)this).GetEnumerator();
		}
	}

	[CompilerGenerated]
	private sealed class _003CEnumerateWithLast_003Ed__5<T> : IEnumerable<(int, T, bool)>, IEnumerable, IEnumerator<(int, T, bool)>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private (int, T, bool) _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private IEnumerable<T> source;

		public IEnumerable<T> _003C_003E3__source;

		private int _003Cindex_003E5__2;

		private List<T> _003CsourceList_003E5__3;

		private List<T>.Enumerator _003C_003E7__wrap3;

		(int, T, bool) IEnumerator<(int, T, bool)>.Current
		{
			[DebuggerHidden]
			get
			{
				return System_002ECollections_002EGeneric_002EIEnumerator_003C_0028System_002EInt32_002CT_002CSystem_002EBoolean_0029_003E_002Eget_Current();
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CEnumerateWithLast_003Ed__5(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if (num == -3 || num == 1)
			{
				try
				{
				}
				finally
				{
					_003C_003Em__Finally1();
				}
			}
		}

		private bool MoveNext()
		{
			try
			{
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003Cindex_003E5__2 = 0;
					_003CsourceList_003E5__3 = source.ToList();
					_003C_003E7__wrap3 = _003CsourceList_003E5__3.GetEnumerator();
					_003C_003E1__state = -3;
					break;
				case 1:
					_003C_003E1__state = -3;
					break;
				}
				if (_003C_003E7__wrap3.MoveNext())
				{
					T current = _003C_003E7__wrap3.Current;
					_003C_003E2__current = (_003Cindex_003E5__2, current, _003CsourceList_003E5__3.Count - 1 == _003Cindex_003E5__2++);
					_003C_003E1__state = 1;
					return true;
				}
				_003C_003Em__Finally1();
				_003C_003E7__wrap3 = default;
				return false;
			}
			catch
			{
				//try-fault
				((IDisposable)this).Dispose();
				throw;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			((IDisposable)_003C_003E7__wrap3/*cast due to constrained. prefix*/).Dispose();
		}

		[DebuggerHidden]
		private (int, T, bool) System_002ECollections_002EGeneric_002EIEnumerator_003C_0028System_002EInt32_002CT_002CSystem_002EBoolean_0029_003E_002Eget_Current()
		{
			return _003C_003E2__current;
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		IEnumerator<(int, T, bool)> IEnumerable<(int, T, bool)>.GetEnumerator()
		{
			_003CEnumerateWithLast_003Ed__5<T> obj;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				obj = this;
			}
			else
			{
				obj = new _003CEnumerateWithLast_003Ed__5<T>(0);
			}
			obj.source = _003C_003E3__source;
			return obj;
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<(int, T, bool)>)this).GetEnumerator();
		}
	}

	public static IEnumerable<int> Range(this int n)
	{
		return Enumerable.Range(0, n);
	}

	public static IEnumerable<int> RangeTo0(this int n)
	{
		foreach (int item in n.Range())
		{
			yield return n - item;
		}
		yield return 0;
	}

	public static IEnumerable<EnumerationEntry<T>> Enumerate<T>(this IEnumerable<T> source)
	{
		int index = 0;
		List<T> sourceList = source.ToList();
		foreach (T item in sourceList)
		{
			yield return new EnumerationEntry<T>(index, item, index == 0, sourceList.Count - 1 == index++);
		}
	}

	[IteratorStateMachine(typeof(_003CEnumerateWithFirst_003Ed__4<>))]
	public static IEnumerable<(int, T, bool)> EnumerateWithFirst<T>(this IEnumerable<T> source)
	{
		//yield-return decompiler failed: Could not find currentField
		return new _003CEnumerateWithFirst_003Ed__4<T>(-2)
		{
			_003C_003E3__source = source
		};
	}

	[IteratorStateMachine(typeof(_003CEnumerateWithLast_003Ed__5<>))]
	public static IEnumerable<(int, T, bool)> EnumerateWithLast<T>(this IEnumerable<T> source)
	{
		//yield-return decompiler failed: Could not find currentField
		return new _003CEnumerateWithLast_003Ed__5<T>(-2)
		{
			_003C_003E3__source = source
		};
	}

	public static IEnumerable<T> ResizeTo<T>(this IEnumerable<T> source, int targetCount, T defaultItem)
	{
		int index = 0;
		foreach (T item in source)
		{
			if (index++ != targetCount)
			{
				yield return item;
				continue;
			}
			yield break;
		}
		for (int i = index; i < targetCount; i++)
		{
			yield return defaultItem;
		}
	}

	public static string Beautify<T>(this IEnumerable<T> source, string separator = ", ")
	{
		if (source == null)
		{
			return "null";
		}
		return string.Join(separator, source);
	}

	public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
	{
		if (source != null)
		{
			return !source.Any();
		}
		return true;
	}

	public static bool IsNullOrEmptyCollection(this IEnumerable source)
	{
		if (source == null)
		{
			return true;
		}
		return !source.Cast<object>().Any();
	}

	public static IEnumerable<T> Children<T>(this Transform t) where T : MonoBehaviour
	{
		return from Transform child in t
			select child.GetComponent<T>();
	}

	public static IEnumerable<GameObject> Children(this Transform t)
	{
		return from Transform child in t
			select child.gameObject;
	}

	public static void SetActiveStatusAll(this ICollection<GameObject> source, bool isActive)
	{
		if (source.IsNullOrEmpty())
		{
			return;
		}
		foreach (GameObject item in source)
		{
			if ((bool)item)
			{
				item.SetActive(isActive);
			}
		}
	}
}
