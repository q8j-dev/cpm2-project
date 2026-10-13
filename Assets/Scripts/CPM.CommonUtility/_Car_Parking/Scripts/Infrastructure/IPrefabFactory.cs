using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	public interface IPrefabFactory
	{
		GameObject Instantiate(GameObject obj, Transform parent);

		GameObject Instantiate(GameObject obj, Vector3 pos, Quaternion rot);

		GameObject Instantiate(GameObject obj, Vector3 pos, Quaternion rot, Transform parent);

		GameObject Instantiate(GameObject obj);

		T Instantiate<T>(T obj, Transform parent) where T : MonoBehaviour;

		T Instantiate<T>(T obj, Vector3 pos, Quaternion rot, Transform parent) where T : MonoBehaviour;

		T InstantiateComponent<T>(GameObject obj) where T : Component;

		void Inject(GameObject obj);

		T Create<T>(params object[] args);
	}
}
