using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure
{
	public class PrefabFactory : IPrefabFactory
	{
		private readonly IObjectResolver _resolver;

		[UnityEngine.Scripting.Preserve]
		public PrefabFactory(IObjectResolver resolver)
		{
			_resolver = resolver;
		}

		public GameObject Instantiate(GameObject obj, Transform parent)
		{
			bool activeSelf = obj.activeSelf;
			obj.SetActive(value: false);
			GameObject gameObject = null;
			try
			{
				gameObject = UnityEngine.Object.Instantiate(obj, parent);
				SafeInjectGameObject(gameObject);
				return gameObject;
			}
			finally
			{
				obj.SetActive(activeSelf);
				gameObject?.SetActive(activeSelf);
			}
		}

		public T Instantiate<T>(T obj, Transform parent) where T : MonoBehaviour
		{
			bool activeSelf = obj.gameObject.activeSelf;
			obj.gameObject.SetActive(value: false);
			T val = null;
			try
			{
				val = UnityEngine.Object.Instantiate(obj, parent);
				SafeInjectGameObject(val.gameObject);
				return val;
			}
			finally
			{
				obj.gameObject.SetActive(activeSelf);
				if (val != null)
				{
					val.gameObject.SetActive(activeSelf);
				}
			}
		}

		public T Instantiate<T>(T obj, Vector3 pos, Quaternion rot, Transform parent) where T : MonoBehaviour
		{
			bool activeSelf = obj.gameObject.activeSelf;
			obj.gameObject.SetActive(value: false);
			T val = null;
			try
			{
				val = UnityEngine.Object.Instantiate(obj, pos, rot, parent);
				SafeInjectGameObject(val.gameObject);
				return val;
			}
			finally
			{
				obj.gameObject.SetActive(activeSelf);
				if (val != null)
				{
					val.gameObject.SetActive(activeSelf);
				}
			}
		}

		public GameObject Instantiate(GameObject obj, Vector3 pos, Quaternion rot)
		{
			return Instantiate(obj, pos, rot, null);
		}

		public GameObject Instantiate(GameObject obj, Vector3 pos, Quaternion rot, Transform parent)
		{
			bool activeSelf = obj.activeSelf;
			obj.SetActive(value: false);
			GameObject gameObject = null;
			try
			{
				gameObject = UnityEngine.Object.Instantiate(obj, pos, rot, parent);
				SafeInjectGameObject(gameObject);
				return gameObject;
			}
			catch (VContainerException arg)
			{
				Debug.LogError($"[PrefabFactory] Instantiate failed for '{obj.name}': {arg}");
			}
			finally
			{
				obj.SetActive(activeSelf);
				gameObject?.SetActive(activeSelf);
			}
			return null;
		}

		public GameObject Instantiate(GameObject obj)
		{
			bool activeSelf = obj.activeSelf;
			obj.SetActive(value: false);
			GameObject gameObject = null;
			try
			{
				gameObject = UnityEngine.Object.Instantiate(obj);
				SafeInjectGameObject(gameObject);
				return gameObject;
			}
			finally
			{
				obj.SetActive(activeSelf);
				gameObject?.SetActive(activeSelf);
			}
		}

		public T InstantiateComponent<T>(GameObject obj) where T : Component
		{
			T val = obj.AddComponent<T>();
			_resolver.Inject(val);
			return val;
		}

		public void Inject(GameObject obj)
		{
			SafeInjectGameObject(obj);
		}

		public T Create<T>(params object[] args)
		{
			Type typeFromHandle = typeof(T);
			ConstructorInfo constructor = typeFromHandle.GetConstructor(Type.EmptyTypes);
			if (constructor != null)
			{
				T val = (T)constructor.Invoke(null);
				_resolver.Inject(val);
				return val;
			}
			ConstructorInfo[] constructors = typeFromHandle.GetConstructors();
			ConstructorInfo constructorInfo = null;
			ConstructorInfo[] array = constructors;
			foreach (ConstructorInfo constructorInfo2 in array)
			{
				if (constructorInfo2.GetCustomAttributes(typeof(InjectAttribute), inherit: false).Length != 0)
				{
					constructorInfo = constructorInfo2;
					break;
				}
			}
			if (constructorInfo == null && constructors.Length != 0)
			{
				constructorInfo = constructors[0];
			}
			if (constructorInfo == null)
			{
				throw new InvalidOperationException("[PrefabFactory] No constructor found for " + typeFromHandle.Name + ". If running IL2CPP, add [UnityEngine.Scripting.Preserve] to the class.");
			}
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			object[] array2 = new object[parameters.Length];
			for (int j = 0; j < parameters.Length; j++)
			{
				array2[j] = _resolver.Resolve(parameters[j].ParameterType);
			}
			return (T)constructorInfo.Invoke(array2);
		}

		private void SafeInjectGameObject(GameObject gameObject)
		{
			SafeInjectRecursive(gameObject);
		}

		private void SafeInjectRecursive(GameObject current)
		{
			if (current == null)
			{
				return;
			}
			MonoBehaviour[] components = current.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour monoBehaviour in components)
			{
				if (!(monoBehaviour == null))
				{
					try
					{
						_resolver.Inject(monoBehaviour);
					}
					catch (VContainerException ex)
					{
						Debug.LogError("[PrefabFactory] Skipped injection for " + monoBehaviour.GetType().Name + " on '" + current.name + "': " + ex.Message);
					}
				}
			}
			Transform transform = current.transform;
			for (int j = 0; j < transform.childCount; j++)
			{
				SafeInjectRecursive(transform.GetChild(j).gameObject);
			}
		}
	}
}
