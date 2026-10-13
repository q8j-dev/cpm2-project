using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking;

public class LevelSceneControl : MonoBehaviour
{
	public GameObject[] highQualityObjects;

	public GameObject[] lowQualityObjects;

	public bool isLoadedMainCity;

	public bool LoadedHighway;

	public bool LoadedRoad2;

	public bool LoadedDesert;

	public GameObject exhaustParticle;

	public AudioSource[] randomSounds;

	public GameObject CarTowLoad;

	public Material skyboxDesert;

	public Material skyboxSky;

	public float rotation;

	private int IdRotation;

	public float speedRotation = 0.5f;

	private Vector3 v3ref = Vector3.zero;

	private Vector3 v3ref2 = Vector3.zero;

	private bool changingAmbient;

	private bool changingDirectionLightColor;

	private Vector3 TargetAmbientColor;

	private Vector3 TargetDirectionalColor;

	private float ambientColorChangeSpeed = 2f;

	private float dirColorChangeSpeed = 2f;

	public bool enablingFog;

	private float targetFogDensity;

	private float currentFogDensity;

	private float startTimeFogEnable;

	private float timeFogEnable = 4f;

	private IDisposable _disposableGetAfterMountainHighQualityObj;

	private IDisposable _disposablePlayAudio;

	private IDisposable _disposableGetAfterDesertHighQualityObj;

	private void Awake()
	{
		AudioSource[] array = randomSounds;
		if (array != null && array.Length > 0)
		{
			_disposablePlayAudio?.Dispose();
			_disposablePlayAudio = Observable.Timer(TimeSpan.FromSeconds(1.0)).Concat(Observable.Interval(TimeSpan.FromSeconds(7.0))).Subscribe((long _) =>
			{
				PlayAudio();
			})
				.AddTo(this);
		}
		IdRotation = Shader.PropertyToID("_Rotation");
	}

	private void Start()
	{
		if ((bool)skyboxDesert && FlagsStorage.IsMainMenu)
		{
			int nameID = Shader.PropertyToID("_Exposure");
			skyboxDesert.SetFloat(nameID, 2f);
			skyboxSky.SetFloat(nameID, 2f);
			skyboxDesert.SetFloat(IdRotation, 163f);
		}
	}

	private void Update()
	{
	}

	public void EnableHighQuality()
	{
		PlayerDatas.DisableEnableAll(isEnable: true, highQualityObjects);
		PlayerDatas.DisableEnableAll(isEnable: false, lowQualityObjects);
	}

	public void EnableLowQuality()
	{
		PlayerDatas.DisableEnableAll(isEnable: false, highQualityObjects);
		PlayerDatas.DisableEnableAll(isEnable: true, lowQualityObjects);
	}

	public void LoadMainCity()
	{
		if (!isLoadedMainCity)
		{
			PlayerDatas.LoadMainCityScene();
			isLoadedMainCity = true;
		}
	}

	public void LoadMountain()
	{
		if (LoadedHighway)
		{
			return;
		}
		LoadedHighway = true;
		_disposableGetAfterMountainHighQualityObj?.Dispose();
		AsyncOperation asyncOperation = PlayerDatas.LoadMountainScene();
		if (asyncOperation != null)
		{
			asyncOperation.completed += (AsyncOperation _) =>
			{
				if ((bool)this)
				{
					GetAfterMountainHighQualityObj();
				}
			};
		}
		else
		{
			GetAfterMountainHighQualityObj();
		}
	}

	public void LoadRoad2()
	{
		if (!LoadedRoad2)
		{
			PlayerDatas.LoadRoad2Scene();
			LoadedRoad2 = true;
		}
	}

	public void LoadDesert()
	{
		if (LoadedDesert)
		{
			return;
		}
		LoadedDesert = true;
		_disposableGetAfterDesertHighQualityObj?.Dispose();
		AsyncOperation asyncOperation = PlayerDatas.LoadDesertScene();
		if (asyncOperation != null)
		{
			asyncOperation.completed += (AsyncOperation _) =>
			{
				if ((bool)this)
				{
					GetAfterDesertHighQualityObj();
				}
			};
		}
		else
		{
			GetAfterDesertHighQualityObj();
		}
	}

	private void GetAfterMountainHighQualityObj()
	{
		GameObject[] rootGameObjects = SceneManager.GetSceneByBuildIndex(16).GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			string text = gameObject.name;
			if (!(text == "hq"))
			{
				if (text == "hide")
				{
					gameObject.SetActive(value: true);
				}
			}
			else
			{
				GameObject[] array = new GameObject[highQualityObjects.Length + 1];
				array[array.Length - 1] = gameObject;
				highQualityObjects.CopyTo(array, 0);
				highQualityObjects = array;
				gameObject.SetActive(CPMGraphicsSettingsManager.Instance.Quality > 1);
			}
		}
	}

	private void GetAfterDesertHighQualityObj()
	{
		GameObject[] rootGameObjects = SceneManager.GetSceneByBuildIndex(15).GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			string text = gameObject.name;
			if (!(text == "hq"))
			{
				if (text == "hide")
				{
					gameObject.SetActive(value: true);
				}
			}
			else
			{
				GameObject[] array = new GameObject[highQualityObjects.Length + 1];
				array[array.Length - 1] = gameObject;
				highQualityObjects.CopyTo(array, 0);
				highQualityObjects = array;
				gameObject.SetActive(CPMGraphicsSettingsManager.Instance.Quality > 1);
			}
		}
	}

	public void SetLightsParking(bool onParking)
	{
		changingAmbient = true;
		changingDirectionLightColor = true;
		startTimeFogEnable = Time.time;
	}

	private Vector3 ToVector3(Color c)
	{
		return new Vector3(c.r, c.g, c.b);
	}

	public void PlayAudio()
	{
		AudioSource[] array = randomSounds;
		foreach (AudioSource audioSource in array)
		{
			if (UnityEngine.Random.Range(1, 13) == 1)
			{
				if ((bool)audioSource)
				{
					audioSource.Play();
				}
				break;
			}
		}
	}
}
