using System;
using UnityEngine;
using VContainer.Unity;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class RamHelper : IInitializable, IDisposable
	{
		private bool _isLowMemoryHandled;

		public bool ReachedRamLimit => _isLowMemoryHandled;

		public void Initialize()
		{
		}

		private void OnApplicationLowMemoryHandler()
		{
			_isLowMemoryHandled = true;
		}

		public void Dispose()
		{
			Application.lowMemory -= OnApplicationLowMemoryHandler;
		}

		public void SwitchLowMemoryFlag(bool value)
		{
			_isLowMemoryHandled = value;
		}
	}
}
