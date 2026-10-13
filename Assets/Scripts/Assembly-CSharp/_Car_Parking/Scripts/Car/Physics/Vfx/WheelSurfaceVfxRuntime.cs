using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.Physics.Vfx
{
	public class WheelSurfaceVfxRuntime
	{
		public class RuntimeEntry
		{
			public WheelSurfaceVfxEntry Source;

			public ParticleSystem Instance;

			public Color TintColor;

			public float StartSize;
		}

		public RuntimeEntry[][] BySurface { get; private set; }

		public static WheelSurfaceVfxRuntime Build(WheelSurfaceVfxLibrary library, Transform parent)
		{
			int num = MaxSurfaceIndex() + 1;
			WheelSurfaceVfxRuntime wheelSurfaceVfxRuntime = new WheelSurfaceVfxRuntime
			{
				BySurface = new RuntimeEntry[num][]
			};
			if (!library || library.entries == null || library.entries.Count == 0)
			{
				return wheelSurfaceVfxRuntime;
			}
			Dictionary<int, List<RuntimeEntry>> dictionary = new Dictionary<int, List<RuntimeEntry>>();
			for (int i = 0; i < library.entries.Count; i++)
			{
				WheelSurfaceVfxEntry wheelSurfaceVfxEntry = library.entries[i];
				if (wheelSurfaceVfxEntry == null)
				{
					continue;
				}
				if (!string.IsNullOrEmpty(wheelSurfaceVfxEntry.ID))
				{
					_ = wheelSurfaceVfxEntry.ID;
				}
				else
				{
					_ = $"#{i}";
				}
				if (!wheelSurfaceVfxEntry.Prefab || wheelSurfaceVfxEntry.Surfaces == null || wheelSurfaceVfxEntry.Surfaces.Length == 0)
				{
					continue;
				}
				ParticleSystem particleSystem = UnityEngine.Object.Instantiate(wheelSurfaceVfxEntry.Prefab, parent);
				particleSystem.gameObject.name = (string.IsNullOrEmpty(wheelSurfaceVfxEntry.ID) ? wheelSurfaceVfxEntry.Prefab.name : wheelSurfaceVfxEntry.ID);
				_ = particleSystem.main.simulationSpace;
				_ = 1;
				for (int j = 0; j < wheelSurfaceVfxEntry.Surfaces.Length; j++)
				{
					int surface = (int)wheelSurfaceVfxEntry.Surfaces[j].Surface;
					if (surface >= 0 && surface < num)
					{
						RuntimeEntry item = new RuntimeEntry
						{
							Source = wheelSurfaceVfxEntry,
							Instance = particleSystem,
							TintColor = wheelSurfaceVfxEntry.Surfaces[j].Color,
							StartSize = wheelSurfaceVfxEntry.Surfaces[j].Size
						};
						if (!dictionary.TryGetValue(surface, out var value))
						{
							value = (dictionary[surface] = new List<RuntimeEntry>());
						}
						value.Add(item);
					}
				}
			}
			foreach (KeyValuePair<int, List<RuntimeEntry>> item2 in dictionary)
			{
				wheelSurfaceVfxRuntime.BySurface[item2.Key] = item2.Value.ToArray();
			}
			return wheelSurfaceVfxRuntime;
		}

		public void Dispose()
		{
			if (BySurface == null)
			{
				return;
			}
			HashSet<ParticleSystem> hashSet = new HashSet<ParticleSystem>();
			for (int i = 0; i < BySurface.Length; i++)
			{
				RuntimeEntry[] array = BySurface[i];
				if (array == null)
				{
					continue;
				}
				for (int j = 0; j < array.Length; j++)
				{
					ParticleSystem instance = array[j].Instance;
					if ((bool)instance && hashSet.Add(instance))
					{
						UnityEngine.Object.Destroy(instance.gameObject);
					}
				}
			}
			BySurface = null;
		}

		private static int MaxSurfaceIndex()
		{
			int num = 0;
			foreach (Surface value in Enum.GetValues(typeof(Surface)))
			{
				if ((int)value > num)
				{
					num = (int)value;
				}
			}
			return num;
		}
	}
}
