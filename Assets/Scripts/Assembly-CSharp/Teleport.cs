using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class Teleport : MonoBehaviour
{
	public bool isEntering = true;

	public bool toScene = true;

	public int sceneIndex = 33;

	public bool onlyEnter;

	public Transform positionA;

	public Transform positionB;

	private GameObject root;

	private FreeDriveProvider _freeDriveProvider;

	[Inject]
	public void Construct(FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
	}

	private void Start()
	{
		if (toScene)
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
	}

	private void OnDestroy()
	{
		if (toScene)
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
	}

	public void Teleported()
	{
		if (!onlyEnter)
		{
			_freeDriveProvider.FreeDriveControl.toRoadButton.SetActive(!isEntering);
		}
		if (toScene && sceneIndex == 30)
		{
			if (!root)
			{
				TryResolveRoot();
			}
			if (!root)
			{
				V.SceneControl.LoadHome3(immediately: false);
				return;
			}
			root.SetActive(isEntering);
		}
		if (isEntering)
		{
			V.DriverPerson.transform.position = positionA.position;
		}
		else if (!onlyEnter)
		{
			V.DriverPerson.transform.position = positionB.position;
		}
		isEntering = !isEntering;
	}

	private void TryResolveRoot()
	{
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(30);
		if (sceneByBuildIndex.isLoaded)
		{
			GameObject[] rootGameObjects = sceneByBuildIndex.GetRootGameObjects();
			if (rootGameObjects.Length != 0)
			{
				root = rootGameObjects[0];
			}
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		OnSceneLoadedAsync(scene).Forget();
	}

	private async UniTaskVoid OnSceneLoadedAsync(Scene scene)
	{
		await UniTask.DelayFrame(1);
		if (scene.buildIndex == 30)
		{
			root = SceneManager.GetSceneByBuildIndex(30).GetRootGameObjects()[0];
			if ((bool)V.DriverPerson)
			{
				V.DriverPerson.transform.position = positionA.position;
			}
			isEntering = false;
		}
	}
}
