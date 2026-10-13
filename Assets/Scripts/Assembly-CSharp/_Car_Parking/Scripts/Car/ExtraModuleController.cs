using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Car
{
	public class ExtraModuleController : IDisposable
	{
		private readonly List<VehicleDataExtraModule> _extraModules = new List<VehicleDataExtraModule>();

		public void Dispose()
		{
			foreach (VehicleDataExtraModule extraModule in _extraModules)
			{
				extraModule.Dispose();
			}
			_extraModules.Clear();
		}

		public void Init(VehicleData vehicleData)
		{
			List<VehicleDataExtraModule> extraModules = VehicleDataExtraModuleGetter.GetExtraModules(vehicleData);
			for (int i = 0; i < extraModules.Count; i++)
			{
				AddExtraModule(extraModules[i]);
			}
		}

		public void AddExtraModule(VehicleDataExtraModule extraModule)
		{
			_extraModules.Add(extraModule);
		}

		public void EnableCarDisplay(bool isEngineOn)
		{
			foreach (VehicleDataExtraModule extraModule in _extraModules)
			{
				extraModule.EnableCarDisplay(isEngineOn);
			}
		}

		public void RemoveExtraModule<T>() where T : VehicleDataExtraModule
		{
			for (int i = 0; i < _extraModules.Count; i++)
			{
				if (_extraModules[i].GetType() == typeof(T))
				{
					_extraModules[i].Dispose();
					_extraModules.RemoveAt(i);
					i--;
				}
			}
		}
	}
}
