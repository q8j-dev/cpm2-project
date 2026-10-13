using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace CPM.NightLighting
{
	public class DecalLight : MonoBehaviour
	{
		private Renderer _renderer;

		private SunAndSkyController SunController;

		public Vector3 Center => _renderer.bounds.center;

		private void Awake()
		{
			_renderer = GetComponent<Renderer>();
		}

		private void Start()
		{
			InitStart();
		}

		private void InitStart()
		{
			SunController = GlobalEventSystem.RequestSunController();
			if (SunController == null)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				SunController.SubscribeDecalLight(this);
			}
		}

		private void OnDestroy()
		{
			if (SunController != null)
			{
				SunController.UnsubscribeDecalLight(this);
			}
		}
	}
}
