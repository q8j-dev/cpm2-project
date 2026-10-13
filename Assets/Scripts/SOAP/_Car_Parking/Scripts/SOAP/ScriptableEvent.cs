using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.SOAP
{
	[CreateAssetMenu(fileName = "New ScriptableEvent", menuName = "SOAP/ScriptableEvent")]
	public class ScriptableEvent : ScriptableObject
	{
		[SerializeField]
		private bool debugLog = default;

		private event Action _onRaised;

		public void Subscribe(Action listener)
		{
			if (debugLog)
			{
				if (listener.Target != null)
				{
					_ = listener.Target.GetType().FullName;
				}
				if (listener.Method != null)
				{
					_ = listener.Method.Name;
				}
			}
			_onRaised += listener;
		}

		public void Unsubscribe(Action listener)
		{
			if (debugLog)
			{
				if (listener.Target != null)
				{
					_ = listener.Target.GetType().FullName;
				}
				if (listener.Method != null)
				{
					_ = listener.Method.Name;
				}
			}
			_onRaised -= listener;
		}

		public void Raise()
		{
			if (_onRaised == null)
			{
				return;
			}
			Delegate[] invocationList = _onRaised.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				try
				{
					((Action)obj)();
				}
				catch (Exception)
				{
				}
			}
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		private void LogRaise()
		{
			if (!debugLog || _onRaised == null)
			{
				return;
			}
			Delegate[] invocationList = _onRaised.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				if (obj.Target != null)
				{
					_ = obj.Target.GetType().FullName;
				}
				if (obj.Method != null)
				{
					_ = obj.Method.Name;
				}
			}
		}

		private void OnDisable()
		{
			_onRaised = null;
		}

		public bool AnySubscribers()
		{
			return _onRaised != null;
		}

		public bool AnyOtherSubscribers(HashSet<string> otherThan)
		{
			return (from del in _onRaised?.GetInvocationList()
				let target = (del.Target != null) ? del.Target.GetType().FullName : "<static>"
				let method = (del.Method != null) ? del.Method.Name : "<null>"
				where otherThan == null || !otherThan.Contains(target + "." + method)
				select target).Any();
		}
	}
	public abstract class ScriptableEvent<T> : ScriptableObject
	{
		[SerializeField]
		protected bool debugLog;

		private event Action<T> _onRaised;

		public void Subscribe(Action<T> listener)
		{
			if (debugLog)
			{
				if (listener.Target != null)
				{
					_ = listener.Target.GetType().FullName;
				}
				if (listener.Method != null)
				{
					_ = listener.Method.Name;
				}
			}
			_onRaised += listener;
		}

		public void Unsubscribe(Action<T> listener)
		{
			if (debugLog)
			{
				if (listener.Target != null)
				{
					_ = listener.Target.GetType().FullName;
				}
				if (listener.Method != null)
				{
					_ = listener.Method.Name;
				}
			}
			_onRaised -= listener;
		}

		public void Raise(T value)
		{
			if (_onRaised == null)
			{
				return;
			}
			Delegate[] invocationList = _onRaised.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				try
				{
					((Action<T>)obj)(value);
				}
				catch (Exception)
				{
				}
			}
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private void LogRaise(T value)
		{
			if (!debugLog || _onRaised == null)
			{
				return;
			}
			Delegate[] invocationList = _onRaised.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				if (obj.Target != null)
				{
					_ = obj.Target.GetType().FullName;
				}
				if (obj.Method != null)
				{
					_ = obj.Method.Name;
				}
			}
		}

		private void OnDisable()
		{
			_onRaised = null;
		}

		public bool AnySubscribers()
		{
			return _onRaised != null;
		}

		public bool AnyOtherSubscribers(HashSet<string> otherThan)
		{
			return (from del in _onRaised?.GetInvocationList()
				let target = (del.Target != null) ? del.Target.GetType().FullName : "<static>"
				let method = (del.Method != null) ? del.Method.Name : "<null>"
				where otherThan == null || !otherThan.Contains(target + "." + method)
				select target).Any();
		}
	}
}
