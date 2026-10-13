using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	public class MonoBehaviourUtilities : MonoBehaviour
	{
		private static GameObject _parent;

		public static T GetOrCreateSceneInstance<T>(string path, bool addEditorTag) where T : MonoBehaviour
		{
			T[] array = FindObjectsByType<T>();
			if (array.Length > 1)
			{
				Debug.LogError($"Multiple components of type {typeof(T)} exists in scene. Just one is required, delete extra components before continuing.");
				for (int i = 0; i < array.Length; i++)
				{
					Debug.LogWarning($"{typeof(T)} component exists on: {array[i].name}", array[i]);
				}
			}
			if (array.Length == 0)
			{
				return GetOrCreateGameObject(path, addEditorTag).AddComponent<T>();
			}
			return array[0];
		}

		public static T GetOrCreateObjectScript<T>(string holderName, bool addEditorTag) where T : MonoBehaviour
		{
			return GetOrCreateObjectScript<T>(GetOrCreateGameObject(holderName, addEditorTag));
		}

		public static T GetOrCreateObjectScript<T>(GameObject holder) where T : MonoBehaviour
		{
			if (TryGetObjectScript(holder, out ExecutionResult<T> result))
			{
				return result.Value;
			}
			return holder.AddComponent<T>();
		}

		public static GameObject GetOrCreateGameObject(string path, bool addEditorTag)
		{
			if (_parent == null)
			{
				_parent = GameObject.Find("Gley");
				if (_parent == null)
				{
					_parent = new GameObject("Gley");
					Debug.Log(_parent.name + " was generated");
				}
			}
			string[] array = path.Split("/");
			Transform transform = _parent.transform;
			for (int i = 0; i < array.Length; i++)
			{
				transform = FindOrCreate(transform, array[i], addEditorTag);
			}
			return transform.gameObject;
		}

		public static GameObject CreateGameObject(string name, Transform parent, Vector3 position, bool addEditorTag)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			gameObject.transform.position = position;
			if (addEditorTag)
			{
				gameObject.tag = "EditorOnly";
			}
			return gameObject;
		}

		public static bool TryGetSceneScript<T>(out ExecutionResult<T> result) where T : MonoBehaviour
		{
			T[] array = FindObjectsByType<T>();
			string error = null;
			T value = null;
			if (array.Length == 0)
			{
				error = UrbanSystemErrors.TypeNotFound<T>();
				result = new ExecutionResult<T>(value, error);
				return false;
			}
			if (array.Length > 1)
			{
				error = UrbanSystemErrors.MultipleComponentsOfTypeFound<T>();
				T[] array2 = array;
				foreach (T val in array2)
				{
					Debug.LogWarning(UrbanSystemErrors.ComponentAlreadyExistsOn<T>(val.name), val);
				}
				result = new ExecutionResult<T>(value, error);
				return false;
			}
			value = array[0];
			result = new ExecutionResult<T>(value, error);
			return true;
		}

		public static bool TryGetObjectScript<T>(string holderName, out ExecutionResult<T> result) where T : MonoBehaviour
		{
			if (TryGetObject(holderName, out var result2))
			{
				return TryGetObjectScript(result2.Value, out result);
			}
			result = new ExecutionResult<T>(null, result2.Error);
			return false;
		}

		public static bool TryGetObjectScript<T>(GameObject holder, out ExecutionResult<T> result) where T : MonoBehaviour
		{
			T component = holder.GetComponent<T>();
			if (component == null)
			{
				result = new ExecutionResult<T>(null, UrbanSystemErrors.TypeNotFound<T>());
				return false;
			}
			result = new ExecutionResult<T>(component, null);
			return true;
		}

		public static bool TryGetObject(string path, out ExecutionResult<GameObject> result)
		{
			GameObject gameObject = GameObject.Find(path);
			if (gameObject == null || !GetFullPath(gameObject).Contains(path))
			{
				result = new ExecutionResult<GameObject>(null, UrbanSystemErrors.ObjectNotFound(path));
				return false;
			}
			result = new ExecutionResult<GameObject>(gameObject, null);
			return true;
		}

		private static string GetFullPath(GameObject obj)
		{
			string text = obj.name;
			Transform parent = obj.transform;
			while (parent.parent != null)
			{
				parent = parent.parent;
				text = parent.name + "/" + text;
			}
			return text;
		}

		public static T[] FindObjectsByType<T>() where T : MonoBehaviour
		{
			return Object.FindObjectsByType<T>(FindObjectsSortMode.None);
		}

		private static Transform FindOrCreate(Transform parent, string gameObjectName, bool editor)
		{
			Transform transform = parent.transform.Find(gameObjectName);
			if (transform == null)
			{
				transform = CreateGameObject(gameObjectName, parent, parent.position, editor).transform;
				Debug.Log(transform.name + " was generated");
			}
			return transform;
		}
	}
}
