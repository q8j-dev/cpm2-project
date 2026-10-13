using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class LOD0Disabler : MonoBehaviour
	{
		private LODGroup _lod;

		private LOD[] _startingLods;

		private LOD[] _disabledLods;

		private void Start()
		{
			if (!TryGetComponent<LODGroup>(out _lod) || !CPMGraphicsSettingsManager.Instance)
			{
				Object.Destroy(this);
				return;
			}
			_startingLods = _lod.GetLODs();
			_disabledLods = _startingLods.Skip(1).ToArray();
			UpdateLOD0State(CPMGraphicsSettingsManager.Instance.Quality);
			CPMGraphicsSettingsManager.Instance.OnQualityChanged += UpdateLOD0State;
		}

		private void OnDestroy()
		{
			if ((bool)CPMGraphicsSettingsManager.Instance)
			{
				CPMGraphicsSettingsManager.Instance.OnQualityChanged -= UpdateLOD0State;
			}
		}

		private void UpdateLOD0State(int quality)
		{
			_lod.SetLODs((quality == 0) ? _startingLods : _disabledLods);
		}
	}
}
