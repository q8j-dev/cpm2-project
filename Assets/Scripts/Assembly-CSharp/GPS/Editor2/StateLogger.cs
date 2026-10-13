using System;
using UnityEngine;

namespace GPS.Editor2
{
	public class StateLogger : IDisposable
	{
		private readonly string _description;

		private readonly Func<object> _object;

		public StateLogger(string description, Func<object> o)
		{
			_description = description;
			_object = o;
			Debug.Log($"BEFORE: {_description} : {_object()}");
		}

		public void Dispose()
		{
			Debug.Log($"AFTER: {_description} : {_object()}");
		}
	}
}
