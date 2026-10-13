using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking;

public class FPSDisplay2 : MonoBehaviour
{
	public bool showFps;

	public bool SetQualityAuto;

	public bool AutoFarClip = true;

	public float timeLowThan13c;

	public float timeMoreThan40c;

	public float timeLowThan13;

	public float timeMoreThan40;

	public float timeMoreThan50;

	public float averageFps;

	public float updateInterval = 0.5f;

	public float accum;

	public int frames;

	private float _timeLeft;

	private float _fpsOverallAverage;

	private float _fpsAverageCount;

	private GUIStyle _style = new GUIStyle();

	private Camera _currentCamera;

	private MenuLevelParent _menuLevelParent;

	private CancellationTokenSource _timersCts;

	public float CurrentFPS { get; private set; }

	[Inject]
	public void Construct(MenuLevelParent menuLevelParent)
	{
		_menuLevelParent = menuLevelParent;
	}

	private void Awake()
	{
		_menuLevelParent.FPSDisplay2 = this;
	}

	private void Start()
	{
		averageFps = 30f;
		_timeLeft = updateInterval;
		showFps = PlayerPrefs.GetInt("ShowFps", 0) != 0;
		_currentCamera = GetComponent<Camera>();
		if (CPMGraphicsSettingsManager.Instance.Quality == -2)
		{
			SetQualityAuto = true;
			switch (PlayerPrefs.GetInt("qualityAuto", CPMGraphicsSettingsManager.Instance.Quality))
			{
			case 2:
				CPMGraphicsSettingsManager.Instance.ApplyPreset(2);
				SetQualityAuto = false;
				break;
			case 1:
				CPMGraphicsSettingsManager.Instance.ApplyPreset(1);
				SetQualityAuto = false;
				break;
			default:
				CPMGraphicsSettingsManager.Instance.ApplyPreset();
				SetQualityAuto = false;
				break;
			}
			if (_timersCts == null)
			{
				_timersCts = new CancellationTokenSource();
			}
			SetQualityLoop(_timersCts.Token).Forget();
		}
		if (FlagsStorage.IsFreeDriveMode)
		{
			if (_timersCts == null)
			{
				_timersCts = new CancellationTokenSource();
			}
			CheckQualityIOSLoop(_timersCts.Token).Forget();
		}
	}

	private async UniTaskVoid SetQualityLoop(CancellationToken ct)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		while (!ct.IsCancellationRequested)
		{
			SetQuality();
			await UniTask.Delay(TimeSpan.FromSeconds(10.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		}
	}

	private async UniTaskVoid CheckQualityIOSLoop(CancellationToken ct)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(720.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		while (!ct.IsCancellationRequested)
		{
			CheckQualityIOS();
			await UniTask.Delay(TimeSpan.FromSeconds(600.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		}
	}

	private void OnDestroy()
	{
		_timersCts?.Cancel();
		_timersCts?.Dispose();
		_timersCts = null;
	}

	private void CheckQualityIOS()
	{
		if (averageFps < 24f)
		{
			switch (CPMGraphicsSettingsManager.Instance.Quality)
			{
			case 1:
				CPMGraphicsSettingsManager.Instance.ApplyPreset();
				SetQualityAuto = false;
				break;
			case 2:
				CPMGraphicsSettingsManager.Instance.ApplyPreset(1);
				SetQualityAuto = false;
				break;
			}
		}
	}

	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		_timeLeft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)_timeLeft <= 0.0)
		{
			CurrentFPS = accum / (float)frames;
			_timeLeft = updateInterval;
			accum = 0f;
			frames = 0;
			if (_fpsAverageCount > 100f)
			{
				averageFps = _fpsOverallAverage / _fpsAverageCount;
				_fpsAverageCount = 0f;
				_fpsOverallAverage = 0f;
			}
			else if (!float.IsNaN(CurrentFPS) && !float.IsInfinity(CurrentFPS))
			{
				_fpsOverallAverage += CurrentFPS;
				_fpsAverageCount++;
			}
		}
		if (CurrentFPS < 12f)
		{
			timeLowThan13 = Time.deltaTime + timeLowThan13;
			timeLowThan13c = Time.deltaTime + timeLowThan13c;
		}
		if (CurrentFPS > 43f)
		{
			timeMoreThan50 = Time.deltaTime + timeMoreThan50;
		}
		if (CurrentFPS > 40f)
		{
			timeMoreThan40c = Time.deltaTime + timeMoreThan40c;
		}
		if (CurrentFPS > 25f)
		{
			timeMoreThan40 = Time.deltaTime + timeMoreThan40;
		}
	}

	private void OnGUI()
	{
		if (!showFps)
		{
			return;
		}
		int width = Screen.width;
		int height = Screen.height;
		Rect position = new Rect(0f, 0f, width, height * 2 / 100);
		_style.alignment = TextAnchor.UpperLeft;
		_style.fontSize = height * 3 / 100;
		_style.normal.textColor = Color.red;
		string text = "               " + CurrentFPS.ToString("F1");
		if (Time.timeScale == 0f)
		{
			text = "               fps";
			for (int i = 0; (float)i < Mathf.Floor(Time.unscaledTime * 3f % 4f); i++)
			{
				text += ".";
			}
		}
		if (_menuLevelParent.RoundEdgeIphone)
		{
			text = "           " + text;
		}
		GUI.Label(position, text, _style);
	}

	public void SetQuality()
	{
		if (timeLowThan13 >= 9f)
		{
			if (CPMGraphicsSettingsManager.Instance.Quality == 2 && SetQualityAuto)
			{
				CPMGraphicsSettingsManager.Instance.ApplyPreset(1);
				SetQualityAuto = false;
				PlayerPrefs.SetInt("qualityAuto", CPMGraphicsSettingsManager.Instance.Quality);
			}
			if (CPMGraphicsSettingsManager.Instance.Quality == 1 && SetQualityAuto)
			{
				CPMGraphicsSettingsManager.Instance.ApplyPreset();
				SetQualityAuto = false;
				PlayerPrefs.SetInt("qualityAuto", CPMGraphicsSettingsManager.Instance.Quality);
			}
		}
		else if (timeMoreThan40 >= 6f && CPMGraphicsSettingsManager.Instance.Quality == 0)
		{
			if (CPMGraphicsSettingsManager.Instance.Quality == 0 && SetQualityAuto)
			{
				CPMGraphicsSettingsManager.Instance.ApplyPreset(1);
				SetQualityAuto = false;
				PlayerPrefs.SetInt("qualityAuto", CPMGraphicsSettingsManager.Instance.Quality);
			}
		}
		else if (timeMoreThan50 >= 7f && CPMGraphicsSettingsManager.Instance.Quality == 1 && CPMGraphicsSettingsManager.Instance.Quality == 1 && SetQualityAuto && Screen.width > 2000)
		{
			CPMGraphicsSettingsManager.Instance.ApplyPreset(2);
			SetQualityAuto = false;
			PlayerPrefs.SetInt("qualityAuto", CPMGraphicsSettingsManager.Instance.Quality);
		}
		timeLowThan13 = 0f;
		timeMoreThan50 = 0f;
		timeMoreThan40 = 0f;
	}

	public void AdoptDistance()
	{
		if (!AutoFarClip)
		{
			return;
		}
		if (timeLowThan13c >= 1.9f && _currentCamera.farClipPlane > 110f)
		{
			_currentCamera.farClipPlane -= 5f;
			if (V.CoreGameManager.Fog)
			{
				RenderSettings.fogEndDistance = _currentCamera.farClipPlane;
				RenderSettings.fogStartDistance = _currentCamera.farClipPlane - _currentCamera.farClipPlane * 0.1f;
			}
		}
		if (timeMoreThan40c > 1.9f && _currentCamera.farClipPlane < 200f)
		{
			_currentCamera.farClipPlane += 10f;
			if (V.CoreGameManager.Fog)
			{
				RenderSettings.fogEndDistance = _currentCamera.farClipPlane;
				RenderSettings.fogStartDistance = _currentCamera.farClipPlane - _currentCamera.farClipPlane * 0.1f;
			}
		}
		timeLowThan13c = 0f;
		timeMoreThan40c = 0f;
	}
}
