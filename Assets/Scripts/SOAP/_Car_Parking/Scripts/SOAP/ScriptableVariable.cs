using System;
using System.Diagnostics;
using UnityEngine;

namespace _Car_Parking.Scripts.SOAP
{
	public abstract class ScriptableVariable<T> : ScriptableObject where T : struct
	{
		[SerializeField]
		protected bool debugLog;

		[SerializeField]
		protected bool debugLogRead;

		[SerializeField]
		private T initialValue = default;

		[NonSerialized]
		protected T _runtimeValue;

		public virtual T Value
		{
			get
			{
				return ResolveValue();
			}
			set
			{
				_runtimeValue = value;
			}
		}

		public void SetSilent(T value)
		{
			_runtimeValue = value;
		}

		public void Reset()
		{
			_runtimeValue = initialValue;
		}

		private void OnEnable()
		{
			_runtimeValue = initialValue;
		}

		private void OnDisable()
		{
			_runtimeValue = initialValue;
		}

		protected T ResolveValue()
		{
			return _runtimeValue;
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		protected void LogChange(T value)
		{
			_ = debugLog;
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		protected void LogRead(T value)
		{
			_ = debugLogRead;
		}
	}
}
