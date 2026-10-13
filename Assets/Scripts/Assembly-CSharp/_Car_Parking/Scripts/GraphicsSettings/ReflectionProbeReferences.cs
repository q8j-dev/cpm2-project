using UnityEngine;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class ReflectionProbeReferences : MonoBehaviour
	{
		[SerializeField]
		private ReflectionProbe[] Probes = default;

		private void Start()
		{
			if (Probes.Length != 0)
			{
				CPMGraphicsSettingsManager.Instance.ReflectionProbes.AddRange(Probes);
				CPMGraphicsSettingsManager.Instance.ApplyStaticReflections();
			}
		}

		private void OnDestroy()
		{
			ReflectionProbe[] probes = Probes;
			foreach (ReflectionProbe item in probes)
			{
				if (CPMGraphicsSettingsManager.Instance.ReflectionProbes.Contains(item))
				{
					CPMGraphicsSettingsManager.Instance.ReflectionProbes.Remove(item);
				}
			}
		}
	}
}
