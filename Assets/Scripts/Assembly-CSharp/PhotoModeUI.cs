using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PhotoModeUI : MonoBehaviour
{
	public PhotoModePrototype m_PhotoMode;

	public RawImage ResultDisplay;

	public Image LoadingBar;

	public CanvasGroup[] CanvasGroups;

	public Text ShutterSpeedDisplay;

	public Text ApertureDisplay;

	private static readonly Aperture[] kApertures = new Aperture[11]
	{
		Aperture.F1_0,
		Aperture.F1_4,
		Aperture.F2_0,
		Aperture.F2_8,
		Aperture.F4_0,
		Aperture.F5_6,
		Aperture.F8_0,
		Aperture.F11,
		Aperture.F16,
		Aperture.F22,
		Aperture.F32
	};

	private static readonly string[] sApertures = new string[11]
	{
		"f/1", "f/1.4", "f/2", "f/2.8", "f/4", "f/5.6", "f/8", "f/11", "f/16", "f/22",
		"f/32"
	};

	private static readonly ShutterSpeed[] kSpeeds = new ShutterSpeed[12]
	{
		ShutterSpeed.Sec2,
		ShutterSpeed.Sec1,
		ShutterSpeed._1_2,
		ShutterSpeed._1_4,
		ShutterSpeed._1_8,
		ShutterSpeed._1_15,
		ShutterSpeed._1_30,
		ShutterSpeed._1_60,
		ShutterSpeed._1_125,
		ShutterSpeed._1_250,
		ShutterSpeed._1_500,
		ShutterSpeed._1_1000
	};

	private static readonly string[] sSpeeds = new string[16]
	{
		"30\"", "15\"", "8\"", "4\"", "2\"", "1\"", "1/2", "1/4", "1/8", "1/15",
		"1/30", "1/60", "1/125", "1/250", "1/500", "1/1000"
	};

	public void SetDisplayTexture(Texture tex)
	{
		ResultDisplay.texture = tex;
	}

	public void SetDisplayColor(Color col)
	{
		ResultDisplay.material.SetColor("_Color", col);
	}

	public void Init()
	{
		SetShutterSpeed(15f);
		SetAperture(5f);
		SetPanningMode(0);
		ZoomCamera(60f);
		SetExposure(0f);
		SetLensDistortion(1f);
		ResultDisplay.color = Color.white;
	}

	private void LateUpdate()
	{
		LoadingBar.fillAmount = 1f / (float)m_PhotoMode.AccumFrames * (float)m_PhotoMode.FrameCounter;
	}

	public void OpenScreen(int index)
	{
		for (int i = 0; i < CanvasGroups.Length; i++)
		{
			CanvasGroups[i].DOKill();
			bool flag = i == index;
			CanvasGroups[i].DOFade(flag ? 1f : 0f, 0.25f).SetUpdate(UpdateType.Late, isIndependentUpdate: true);
			CanvasGroups[i].blocksRaycasts = flag;
			CanvasGroups[i].interactable = flag;
		}
	}

	public void OnImageDrag(Vector2 pos)
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			m_PhotoMode.PanCamera(pos);
		}
	}

	public void OnImageClick(Vector2 pos)
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			m_PhotoMode.CastFocusPoint(pos);
		}
	}

	public void CaptureButton()
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			m_PhotoMode.Capture();
		}
	}

	public void OnJoysticDrag(Vector2 pos)
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			m_PhotoMode.MoveCamera(pos);
		}
	}

	public void TiltCamera(float val)
	{
	}

	public void ZoomCamera(float val)
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			m_PhotoMode.ZoomCamera(val);
		}
	}

	public void SetAperture(float val)
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			int num = Mathf.RoundToInt(val);
			m_PhotoMode.m_Aperture = kApertures[num];
			m_PhotoMode.SetAperture();
			ApertureDisplay.text = "Aperture: " + sApertures[num];
		}
	}

	public void SetShutterSpeed(float val)
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			int num = Mathf.RoundToInt(val);
			m_PhotoMode.m_ShutterSpeed = kSpeeds[num];
			ShutterSpeedDisplay.text = "Shutter Speed: " + sSpeeds[num];
		}
	}

	public void SetLensDistortion(float val)
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			m_PhotoMode.SetCameraDistortion(val);
		}
	}

	public void SetExposure(float val)
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			m_PhotoMode.SetCameraExposure(val);
		}
	}

	public void SetPanningMode(int val)
	{
		m_PhotoMode.m_CameraPanning = (CameraPanning)val;
	}

	public void Save()
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PostCapture)
		{
			m_PhotoMode.SaveScreenshot();
		}
	}

	public void Exit()
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PostCapture || m_PhotoMode.ModeState == PhotoModeState.PauseAndFrame)
		{
			m_PhotoMode.ExitPhotoMode();
		}
	}

	public void Back()
	{
		if (m_PhotoMode.ModeState == PhotoModeState.PostCapture)
		{
			m_PhotoMode.EnterPhotoMode();
		}
	}
}
