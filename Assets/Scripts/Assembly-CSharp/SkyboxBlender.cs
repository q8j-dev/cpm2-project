using System;
using System.Collections.Generic;
using CPM.NightLighting;
using UnityEngine;
using VContainer;

public class SkyboxBlender : MonoBehaviour
{
	public enum DayTime
	{
		Day = 0,
		Evening = 1,
		Night = 2,
		Morning = 3
	}

	[Range(0f, 360f)]
	[SerializeField]
	public float rotation;

	[SerializeField]
	private float _timePassed;

	public readonly int dayDuration = 900;

	public DayTime dayTime;

	public bool isTurnedOn;

	private readonly float speedRotation = 100f;

	private int skipRot;

	private float rot;

	private GameObject dirLight;

	private List<DecalLight> _decalLights = new List<DecalLight>(15);

	private static readonly int Blend = Shader.PropertyToID("_Blend");

	private static readonly int IdRotation = Shader.PropertyToID("_Rotation");

	private static readonly int Tint = Shader.PropertyToID("_Tint");

	private static readonly int Exposure = Shader.PropertyToID("_Exposure");

	private static readonly int Mode = Shader.PropertyToID("_BlendMode");

	private static readonly int InvertColors = Shader.PropertyToID("_InvertColors");

	private static readonly int FrontTex = Shader.PropertyToID("_FrontTex");

	private static readonly int BackTex = Shader.PropertyToID("_BackTex");

	private static readonly int LeftTex = Shader.PropertyToID("_LeftTex");

	private static readonly int RightTex = Shader.PropertyToID("_RightTex");

	private static readonly int UpTex = Shader.PropertyToID("_UpTex");

	private static readonly int DownTex = Shader.PropertyToID("_DownTex");

	private static readonly int TintColor = Shader.PropertyToID("_TintColor");

	private SceneControl _sceneControl;

	public float RealHour
	{
		get
		{
			float num = 0f;
			float num2 = _timePassed * 24f / (float)dayDuration;
			if (num2 >= 22f)
			{
				return 1f - (num2 - 22f) / 2f;
			}
			if (num2 >= 20f)
			{
				return 1f;
			}
			if (num2 >= 18f)
			{
				return (num2 - 18f) / 2f;
			}
			return 0f;
		}
	}

	public event Action<bool> OnChangeToNight;

	[Inject]
	public void Construct(SceneControl sceneControl)
	{
		_sceneControl = sceneControl;
	}

	private void Awake()
	{
	}

	public void Start()
	{
		dirLight = V.CoreGameManager.directionalLight;
		rotation = 0f;
	}

	private void OnDisable()
	{
		foreach (DecalLight decalLight in _decalLights)
		{
			if ((bool)decalLight)
			{
				decalLight.gameObject.SetActive(value: false);
			}
		}
	}

	private void Update()
	{
	}

	private void SetDirLight(float rot)
	{
		float num = _timePassed * 24f / (float)dayDuration;
		Vector3 eulerAngles = dirLight.transform.eulerAngles;
		if (num > 9f && num < 20f)
		{
			eulerAngles.x = 5f + (20f - num) * 40f / 9f;
		}
		else if (num <= 9f)
		{
			eulerAngles.x = 22.777779f + num * 40f / 9f;
		}
		else if (num > 22f)
		{
			eulerAngles.x = 5f + (num - 22f) * 40f / 9f;
		}
		else
		{
			eulerAngles.x = 5f;
		}
		eulerAngles.y = rot;
		if (!(eulerAngles.x < 5.005f) && eulerAngles.x < 22.777779f)
		{
			Mathf.Lerp(0f, 1.5f, (eulerAngles.x - 5f) / 17.777779f);
			Mathf.Lerp(0.6f, 0.8f, (eulerAngles.x - 5f) / 17.777779f);
		}
		dirLight.transform.eulerAngles = eulerAngles;
	}

	public void TurnOnBuildingLights()
	{
		isTurnedOn = true;
		Shader.EnableKeyword("LIGHTS_ON");
		OnChangeToNight?.Invoke(obj: true);
		foreach (DecalLight decalLight in _decalLights)
		{
			decalLight.gameObject.SetActive(value: true);
		}
	}

	public void TurnOffBuildingLights()
	{
		isTurnedOn = false;
		Shader.DisableKeyword("LIGHTS_ON");
		OnChangeToNight?.Invoke(obj: false);
		foreach (DecalLight decalLight in _decalLights)
		{
			decalLight.gameObject.SetActive(value: false);
		}
	}
}
