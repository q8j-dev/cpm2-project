using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Free_drive
{
	public class City2PartsCollection : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] _parts = default;

		[SerializeField]
		private GameObject[] _objects = default;

		private void Awake()
		{
			GlobalEventSystem.OnCity2PartActivated += ActivatePart;
		}

		private void OnDestroy()
		{
			GlobalEventSystem.OnCity2PartActivated -= ActivatePart;
		}

		private void ActivatePart(bool isPart, int id, bool show)
		{
			if (isPart)
			{
				if (id >= 0 && id < _parts.Length)
				{
					_parts[id].SetActive(show);
				}
			}
			else if (id >= 0 && id < _objects.Length && (bool)_objects[id])
			{
				_objects[id].SetActive(show);
			}
		}
	}
}
