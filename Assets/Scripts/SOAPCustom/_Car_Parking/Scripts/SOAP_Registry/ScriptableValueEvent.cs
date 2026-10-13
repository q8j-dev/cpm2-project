using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.SOAP_Registry
{
	public abstract class ScriptableValueEvent<T> : ScriptableObject
	{
		[SerializeField]
		protected bool debugLog;

		[SerializeField]
		protected bool triggerOnChangeOnly;

		[SerializeField]
		protected bool assignAfterInvoke;

		[SerializeField]
		private T initialValue;

		private object _onRaised;

		public T StoredValue { get; private set; }

		private event Action<T> OnRaised;

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
			OnRaised += listener;
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
			OnRaised -= listener;
		}

		public void Raise(T value)
		{
			if (triggerOnChangeOnly && EqualityComparer<T>.Default.Equals(value, StoredValue))
			{
				return;
			}
			if (!assignAfterInvoke)
			{
				StoredValue = value;
			}
			if (OnRaised != null)
			{
				Delegate[] invocationList = OnRaised.GetInvocationList();
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
			if (assignAfterInvoke)
			{
				StoredValue = value;
			}
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		private void LogRaise(T value)
		{
			if (!debugLog || OnRaised == null)
			{
				return;
			}
			Delegate[] invocationList = OnRaised.GetInvocationList();
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

		public virtual void Reset()
		{
			OnRaised = null;
			_ = debugLog;
			StoredValue = initialValue;
		}

		public void ResetValue()
		{
			_ = debugLog;
			StoredValue = initialValue;
		}

		private void OnDisable()
		{
			Reset();
		}

		public bool AnySubscribers()
		{
			return OnRaised != null;
		}

		public bool AnyOtherSubscribers(HashSet<string> otherThan)
		{
			return (from del in OnRaised?.GetInvocationList()
				let target = (del.Target != null) ? del.Target.GetType().FullName : "<static>"
				let method = (del.Method != null) ? del.Method.Name : "<null>"
				where otherThan == null || !otherThan.Contains(target + "." + method)
				select target).Any();
		}
	}
}
