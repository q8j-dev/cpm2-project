using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class CarClassFilterVisibleController : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] GameObjects = default;

		private NewCarWindowConfig _config;

		[Inject]
		public void Construct(NewCarWindowConfig config)
		{
			_config = config;
		}

		private void OnEnable()
		{
			for (int i = 0; i < GameObjects.Length; i++)
			{
				if ((bool)GameObjects[i])
				{
					GameObjects[i].SetActive(_config.IsFilterClassesWorking);
				}
			}
		}
	}
}
