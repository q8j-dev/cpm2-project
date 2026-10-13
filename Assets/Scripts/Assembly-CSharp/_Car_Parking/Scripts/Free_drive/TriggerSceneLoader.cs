using System;
using UniRx;
using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Free_drive
{
	public class TriggerSceneLoader : MonoBehaviour
	{
		private const float Delay = 0f;

		public int[] SceneIndexesToUnload;

		public bool LoadsScenes;

		public int FrontSceneIndexToLoad = -1;

		public int RearSceneIndexToLoad = -1;

		private SceneControl _sceneControl;

		private IDisposable _enterDisposable;

		private IDisposable _exitDisposable;

		public bool Japan;

		public bool CityMain;

		[Inject]
		public void Construct(SceneControl sceneControl)
		{
			_sceneControl = sceneControl;
		}

		private void OnTriggerEnter(Collider other)
		{
			_enterDisposable?.Dispose();
			_enterDisposable = Observable.Timer(TimeSpan.FromSeconds(0.0)).Subscribe((long _) =>
			{
				TriggerEnterAfterDelay(other);
			}).AddTo(this);
			if (Japan)
			{
				_sceneControl.TryLoadJapanFreePart();
			}
			else if (CityMain)
			{
				_sceneControl.TryLoadCityMainRaceTrackFreePart();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (LoadsScenes)
			{
				_exitDisposable?.Dispose();
				_exitDisposable = Observable.Timer(TimeSpan.FromSeconds(0.0)).Subscribe((long _) =>
				{
					OnExit(other);
				}).AddTo(this);
				if (Japan)
				{
					_sceneControl.TryLoadJapanFreePart();
				}
				else if (CityMain)
				{
					_sceneControl.TryLoadCityMainRaceTrackFreePart();
				}
			}
		}

		private void TriggerEnterAfterDelay(Collider other)
		{
			if (other.CompareTag("DriverPerson") && other.gameObject == V.DriverPerson)
			{
				if (Japan)
				{
					_sceneControl.TryLoadJapanFreePart();
				}
				else if (CityMain)
				{
					UnLoadScenes();
					_sceneControl.TryLoadCityMainRaceTrackFreePart();
				}
			}
			else
			{
				if (!other.CompareTag("Car") && !other.CompareTag("OnlineCar"))
				{
					return;
				}
				VehicleData componentInParent = other.GetComponentInParent<VehicleData>();
				if ((bool)componentInParent && componentInParent.gameObject == V.Car)
				{
					if (Japan)
					{
						_sceneControl.TryLoadJapanFreePart();
					}
					else if (CityMain)
					{
						UnLoadScenes();
						_sceneControl.TryLoadCityMainRaceTrackFreePart();
					}
				}
			}
		}

		private void UnLoadScenes()
		{
			for (int i = 0; i < SceneIndexesToUnload.Length; i++)
			{
				_sceneControl.DestroyEnvironmentByIndex(SceneIndexesToUnload[i]);
			}
		}

		private void LoadScene(GameObject obj)
		{
			int index = ((Vector3.Dot(base.transform.forward, obj.transform.position - base.transform.position) > 0f) ? FrontSceneIndexToLoad : RearSceneIndexToLoad);
			Debug.Log("Index to load:" + index);
			_sceneControl.LoadEnvironmentByIndex(index);
		}

		private void OnExit(Collider other)
		{
			if (other.CompareTag("DriverPerson") && other.gameObject == V.DriverPerson)
			{
				LoadScene(other.gameObject);
			}
			else if (other.CompareTag("Car") || other.CompareTag("OnlineCar"))
			{
				VehicleData componentInParent = other.GetComponentInParent<VehicleData>();
				if ((bool)componentInParent && componentInParent.gameObject == V.Car)
				{
					LoadScene(componentInParent.gameObject);
				}
			}
		}
	}
}
