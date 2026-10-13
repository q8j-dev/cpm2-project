using System;
using System.Collections;
using System.IO;
using AlmostEngine;
using AlmostEngine.Screenshot;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using _Car_Parking;
using _Car_Parking.Scripts.PhotoMode.UI;
using _Car_Parking.Scripts.WindowSystem;

public class PhotoModePrototype : MonoBehaviour
{
	public const int MinRamSize = 1600;

	private bool _lowMemory;

	public int AccumFrames = 64;

	public int FrameCounter;

	public float MovementSpeed = 9f;

	public ShutterSpeed m_ShutterSpeed = ShutterSpeed._1_60;

	public CameraPanning m_CameraPanning;

	public float FocalLength = 50f;

	public Aperture m_Aperture = Aperture.F5_6;

	private CPMGraphicsSettings.ColorCorrectionLut CCLut;

	private Transform FocusTransform;

	private Quaternion LookOffset;

	private Vector3 PosOffset;

	public PhotoModeState ModeState;

	public RenderTexture accumulationTex;

	public RenderTexture accumulationDoubleBufferTex;

	public RenderTexture captureRT;

	private Material AccumMat;

	private Mesh _quad;

	public Material QuadMaterial;

	public Shader BlitAdditive;

	public Shader BlitDoubleBufferAdditive;

	public Camera captureCamera;

	private UniversalAdditionalCameraData camdata;

	private Camera mainCamera;

	private UniversalAdditionalCameraData mainCamData;

	private Volume ppVolume;

	private float baseSceneExposure;

	private LensFlareSwitch FlareSwitch;

	public GameObject UiPrefab;

	public VolumeProfile ppProfile;

	[HideInInspector]
	public PhotoModeUI UiInstance;

	public PhotoModeWindow PhotoModeWindow;

	public FloatingJoystick MovingJoystick;

	public bool IsJoystickPressed;

	public bool RecordSequence;

	private bool ExitSequence;

	private int sequencecounter;

	public bool RealtimeMode;

	private bool useDoubleBuffer;

	[Header("Camera movement options")]
	[SerializeField]
	private TouchDetector TouchDetector;

	[SerializeField]
	private float RotationSpeed = 0.2f;

	[SerializeField]
	private float TiltSpeed = 0.2f;

	[SerializeField]
	private float SmoothTime = 0.1f;

	[SerializeField]
	private float VerticalMaxAngle = 85f;

	[SerializeField]
	private float VerticalMinAngle = -85f;

	[SerializeField]
	private float TouchZoomSpeed = 0.5f;

	[SerializeField]
	private float SpeedOffset = 0.04f;

	[SerializeField]
	private float DistanceBetweenCamNWall = 1f;

	private float _yaw;

	private float _pitch;

	private float _roll;

	private Touch _currentTouch;

	private int _currentTouchIndex = -1;

	private Vector3 HostPosition
	{
		get
		{
			if (FlagsStorage.OutOfCar && (bool)V.DriverPerson)
			{
				return V.DriverPerson.transform.position;
			}
			if ((bool)V.Car)
			{
				return V.Car.transform.position;
			}
			return captureCamera.transform.position;
		}
	}

	public float CameraZoom
	{
		get
		{
			if ((bool)captureCamera)
			{
				return captureCamera.fieldOfView;
			}
			return 0f;
		}
		set
		{
			ZoomCamera(value);
		}
	}

	public Aperture Aperture
	{
		get
		{
			return m_Aperture;
		}
		set
		{
			m_Aperture = value;
			SetAperture();
		}
	}

	public float CameraDistortion
	{
		get
		{
			if (ppProfile.TryGet<PaniniProjection>(out var component))
			{
				return component.distance.value;
			}
			return 0f;
		}
		set
		{
			SetCameraDistortion(value);
		}
	}

	public CPMGraphicsSettings.ColorCorrectionLut ColorCorrection
	{
		get
		{
			return CCLut;
		}
		set
		{
			CCLut = value;
			ApplyColorCorrection();
		}
	}

	public float WhiteBallanceTint
	{
		get
		{
			if (ppProfile.TryGet<WhiteBalance>(out var component))
			{
				return component.tint.value;
			}
			return 0f;
		}
		set
		{
			SetWhiteBallanceTint(value);
		}
	}

	public float WhiteBallance
	{
		get
		{
			if (ppProfile.TryGet<WhiteBalance>(out var component))
			{
				return component.temperature.value;
			}
			return 0f;
		}
		set
		{
			SetWhiteBallance(value);
		}
	}

	public float CameraExposure
	{
		get
		{
			if (ppProfile.TryGet<ColorAdjustments>(out var component))
			{
				return component.postExposure.value - baseSceneExposure;
			}
			return 0f;
		}
		set
		{
			SetCameraExposure(value);
		}
	}

	public event Action<float> OnZoomByTouch;

	public event Action OnCaptureCompleted;

	public void InitCamera()
	{
		mainCamera = Camera.main;
		mainCamData = mainCamera.GetUniversalAdditionalCameraData();
	}

	public void EnterPhotoMode()
	{
		CaptureBaseSceneExposure();
		_lowMemory = IsLowRam();
		if (!FlareSwitch)
		{
			FlareSwitch = UnityEngine.Object.FindObjectOfType<LensFlareSwitch>();
		}
		if ((bool)FlareSwitch)
		{
			FlareSwitch.ReplaceFlare(original: false);
		}
		useDoubleBuffer = true;
		if (!_quad)
		{
			_quad = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
		}
		ExitSequence = false;
		sequencecounter = 0;
		if (!RealtimeMode)
		{
			Time.timeScale = 0f;
			Time.fixedDeltaTime = 0.02f;
		}
		if (!captureRT)
		{
			captureRT = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGBHalf);
		}
		if (!accumulationTex)
		{
			accumulationTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf);
			accumulationTex.name = "AccumulationTex";
		}
		if (useDoubleBuffer && !accumulationDoubleBufferTex)
		{
			accumulationDoubleBufferTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf);
			accumulationDoubleBufferTex.name = "AccumulationDoubleBufferTex";
		}
		int num = LayerMask.NameToLayer("Water");
		if (!captureCamera)
		{
			GameObject gameObject = new GameObject("CaptureCamera");
			gameObject.transform.position = mainCamera.transform.position;
			gameObject.transform.rotation = mainCamera.transform.rotation;
			captureCamera = gameObject.AddComponent<Camera>();
			captureCamera.enabled = true;
			captureCamera.CopyFrom(mainCamera);
			captureCamera.targetTexture = captureRT;
			captureCamera.allowDynamicResolution = false;
			camdata = captureCamera.GetUniversalAdditionalCameraData();
			camdata.SetRenderer(4);
			camdata.renderShadows = mainCamera.GetUniversalAdditionalCameraData().renderShadows;
			ppVolume = captureCamera.gameObject.AddComponent<Volume>();
			ppVolume.priority = 10f;
			ppVolume.profile = ppProfile;
			camdata.renderPostProcessing = true;
			camdata.volumeLayerMask = LayerMaskFromLayer(num);
			camdata.stopNaN = true;
			gameObject.gameObject.layer = num;
		}
		captureCamera.cullingMask = mainCamera.cullingMask;
		camdata.antialiasing = mainCamData.antialiasing;
		ToggleColorCorrection(active: true);
		ToggleExposure(active: true);
		ToggleInCameraEffects(active: true);
		mainCamera.enabled = false;
		if (!FocusTransform)
		{
			CastFocusPoint(new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f));
		}
		if (!AccumMat)
		{
			AccumMat = new Material(useDoubleBuffer ? BlitDoubleBufferAdditive : BlitAdditive);
		}
		ModeState = PhotoModeState.PauseAndFrame;
		PhotoModeWindow.ResetResultDisplay();
		PhotoModeWindow.SetDisplayColor(Color.white);
		PhotoModeWindow.SetDisplayTexture(captureRT);
		QuadMaterial.mainTexture = accumulationTex;
		InitCameraRotAngle();
		V.PlayerPointLOD = captureCamera.transform;
		CPMGraphicsSettingsManager.Instance.ApplyRenderScale();
	}

	public void ExitPhotoMode()
	{
		if ((bool)FlareSwitch)
		{
			FlareSwitch.ReplaceFlare();
		}
		if ((bool)FocusTransform)
		{
			UnityEngine.Object.Destroy(FocusTransform.gameObject);
		}
		if ((bool)captureRT)
		{
			CommonExtensions.DeleteRenderTex(captureRT);
		}
		if ((bool)accumulationTex)
		{
			CommonExtensions.DeleteRenderTex(accumulationTex);
		}
		if ((bool)accumulationDoubleBufferTex)
		{
			CommonExtensions.DeleteRenderTex(accumulationDoubleBufferTex);
		}
		if ((bool)AccumMat)
		{
			UnityEngine.Object.Destroy(AccumMat);
		}
		if ((bool)mainCamera)
		{
			mainCamera.enabled = true;
		}
		if ((bool)captureCamera && (bool)captureCamera.gameObject)
		{
			UnityEngine.Object.Destroy(captureCamera.gameObject);
		}
		if (!RealtimeMode)
		{
			Time.timeScale = 1f;
		}
		CPMGraphicsSettingsManager.Instance.ApplyRenderScale();
		ModeState = PhotoModeState.StandBy;
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MainPerson>(out var component))
		{
			component.RestoreHeadAfterPhotoMode();
		}
	}

	public void ResetCameraSettings()
	{
		ToggleColorCorrection(active: true);
		ToggleExposure(active: true);
		ToggleInCameraEffects(active: true);
		ModeState = PhotoModeState.PauseAndFrame;
		CPMGraphicsSettingsManager.Instance.ApplyRenderScale();
	}

	public void Capture()
	{
		if (!_lowMemory)
		{
			UnityGraphicsBullshit.RenderScale = 1f;
		}
		captureCamera.transform.SetParent((m_CameraPanning == CameraPanning.AttachedToTarget) ? FocusTransform : null);
		captureCamera.cullingMask = mainCamera.cullingMask;
		if (!RecordSequence || sequencecounter == 0)
		{
			LookOffset = GetLookOffset(captureCamera.transform, FocusTransform, Vector3.up);
			PosOffset = captureCamera.transform.position - FocusTransform.transform.position;
		}
		FrameCounter = 0;
		RenderTexture active = RenderTexture.active;
		Graphics.SetRenderTarget(accumulationTex);
		GL.Clear(clearDepth: false, clearColor: true, Color.black);
		if ((bool)accumulationDoubleBufferTex)
		{
			Graphics.SetRenderTarget(accumulationDoubleBufferTex);
			GL.Clear(clearDepth: false, clearColor: true, Color.black);
		}
		RenderTexture.active = active;
		camdata.antialiasing = AntialiasingMode.TemporalAntiAliasing;
		camdata.antialiasingQuality = (_lowMemory ? AntialiasingQuality.Medium : AntialiasingQuality.High);
		ToggleColorCorrection(active: false);
		ToggleInCameraEffects(active: true);
		PhotoModeWindow.SetDisplayTexture(accumulationTex);
		float value = 1f / (float)AccumFrames;
		AccumMat.SetFloat("_Weight", value);
		if (!RealtimeMode)
		{
			Time.timeScale = 1f;
			Time.captureFramerate = Mathf.RoundToInt((float)AccumFrames / Seconds(m_ShutterSpeed));
			Time.fixedDeltaTime = Mathf.Min(0.02f, 1f / (float)Time.captureFramerate);
		}
		ModeState = PhotoModeState.Capture;
	}

	private void PostCapture()
	{
		UnityGraphicsBullshit.RenderScale = 1f;
		if (!RealtimeMode)
		{
			Time.timeScale = 0f;
			Time.fixedDeltaTime = 0.02f;
			Time.captureFramerate = 0;
		}
		captureCamera.transform.SetParent(null);
		camdata.antialiasing = AntialiasingMode.None;
		PhotoModeWindow.SetDisplayTexture(captureRT);
		PhotoModeWindow.SetDisplayColor(Color.white);
		ToggleColorCorrection(active: true);
		ToggleExposure(active: false);
		ToggleInCameraEffects(active: false);
		if (RecordSequence && !ExitSequence)
		{
			StartCoroutine(NextSequence());
			return;
		}
		ModeState = PhotoModeState.PostCapture;
		OnCaptureCompleted?.Invoke();
	}

	public IEnumerator NextSequence()
	{
		ModeState = PhotoModeState.PostCapture;
		yield return null;
		yield return new WaitForEndOfFrame();
		OnCaptureCompleted?.Invoke();
		SaveScreenshot();
		Capture();
		yield return null;
	}

	private void ToggleColorCorrection(bool active)
	{
		if (ppProfile.TryGet<Tonemapping>(out var component))
		{
			component.active = active;
		}
		if (ppProfile.TryGet<ColorAdjustments>(out var component2))
		{
			component2.contrast.overrideState = active;
		}
		if (ppProfile.TryGet<LiftGammaGain>(out var component3))
		{
			component3.active = active;
		}
		if (ppProfile.TryGet<ColorLookup>(out var component4))
		{
			component4.active = active;
		}
		if (ppProfile.TryGet<WhiteBalance>(out var component5))
		{
			component5.active = active;
		}
	}

	private void ToggleExposure(bool active)
	{
		if (ppProfile.TryGet<ColorAdjustments>(out var component))
		{
			component.postExposure.overrideState = active;
		}
	}

	private void ToggleInCameraEffects(bool active)
	{
		if (ppProfile.TryGet<Bloom>(out var component))
		{
			component.active = active;
		}
		if (ppProfile.TryGet<DepthOfField>(out var component2))
		{
			component2.active = active;
		}
		if (ppProfile.TryGet<PaniniProjection>(out var component3))
		{
			component3.active = active;
		}
	}

	public void PanCamera(Vector2 val)
	{
		float num = 89f;
		float num2 = captureCamera.fieldOfView * ((float)Math.PI / 180f);
		float num3 = 2f * Mathf.Atan(Mathf.Tan(num2 * 0.5f) * captureCamera.aspect);
		float num4 = val.x / (float)Screen.width * num3;
		float num5 = (0f - val.y) / (float)Screen.height * num2;
		captureCamera.transform.Rotate(Vector3.up, 57.29578f * num4, Space.World);
		captureCamera.transform.Rotate(Vector3.right, 57.29578f * num5, Space.Self);
		Vector3 eulerAngles = captureCamera.transform.eulerAngles;
		eulerAngles.x = Normalize180(eulerAngles.x);
		eulerAngles.x = Mathf.Clamp(eulerAngles.x, 0f - num, num);
		captureCamera.transform.eulerAngles = eulerAngles;
	}

	private float Normalize180(float angle)
	{
		angle = Mathf.Repeat(angle + 180f, 360f) - 180f;
		return angle;
	}

	public void MoveCamera(Vector2 val)
	{
		Vector2 vector = val * MovementSpeed;
		MoveCamera(new Vector3(vector.x, 0f, vector.y));
	}

	private void MoveCamera()
	{
		if ((bool)MovingJoystick)
		{
			Vector3 zero = Vector3.zero;
			zero += Vector3.forward * (MovementSpeed * MovingJoystick.Vertical);
			zero += Vector3.right * (MovementSpeed * MovingJoystick.Horizontal);
			MoveCamera(zero);
		}
	}

	public void RaiseCamera(float height)
	{
		Vector3 direction = Vector3.up * (MovementSpeed * height);
		MoveCamera(direction);
	}

	public void TiltCamera(float cameraTilt)
	{
		Vector3 eulerAngles = captureCamera.transform.eulerAngles;
		captureCamera.transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, cameraTilt);
		UpdateRotAngle();
	}

	private void MoveCamera(Vector3 direction)
	{
		Vector3 position = captureCamera.transform.position;
		direction = CheckForWall(direction);
		if (!(direction == Vector3.zero))
		{
			MoveCameraWorldSpace(direction);
			float num = Extensions.DistanceSquare(captureCamera.transform.position, HostPosition);
			if (num > 4900f && num > Extensions.DistanceSquare(position, HostPosition))
			{
				captureCamera.transform.position = position;
			}
		}
	}

	private void MoveCameraWorldSpace(Vector3 direction)
	{
		Vector3 position = captureCamera.transform.position;
		float y = direction.y;
		Quaternion quaternion = Quaternion.Euler(0f, captureCamera.transform.eulerAngles.y, 0f);
		Vector3 vector = new Vector3(direction.x, 0f, direction.z);
		Vector3 vector2 = quaternion * vector;
		Vector3 position2 = position + vector2 * Time.unscaledDeltaTime;
		position2.y = position.y + y * Time.unscaledDeltaTime;
		captureCamera.transform.position = position2;
	}

	private Vector3 CheckForWall(Vector3 direction)
	{
		if (direction == Vector3.zero)
		{
			return direction;
		}
		Vector3 position = captureCamera.transform.position;
		float maxDistance = Time.unscaledDeltaTime + DistanceBetweenCamNWall;
		Quaternion quaternion = Quaternion.Euler(0f, captureCamera.transform.eulerAngles.y, 0f);
		Vector3 result = direction;
		if (direction.x != 0f || direction.z != 0f)
		{
			if (direction.x != 0f)
			{
				Vector3 vector = new Vector3(direction.x, 0f, 0f);
				if (Physics.Raycast(position, (quaternion * vector).normalized, maxDistance, -1, QueryTriggerInteraction.Ignore))
				{
					result.x = 0f;
				}
			}
			if (direction.z != 0f)
			{
				Vector3 vector2 = new Vector3(0f, 0f, direction.z);
				if (Physics.Raycast(position, (quaternion * vector2).normalized, maxDistance, -1, QueryTriggerInteraction.Ignore))
				{
					result.z = 0f;
				}
			}
		}
		if (direction.y != 0f && Physics.Raycast(position, new Vector3(0f, direction.y, 0f).normalized, maxDistance, -1, QueryTriggerInteraction.Ignore))
		{
			result.y = 0f;
		}
		return result;
	}

	private void ZoomByTouch()
	{
		if (TouchDetector.IsZoom)
		{
			Touch touch = Input.GetTouch(0);
			Touch touch2 = Input.GetTouch(1);
			Vector2 vector = touch.position - touch.deltaPosition;
			Vector2 vector2 = touch2.position - touch2.deltaPosition;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (touch.position - touch2.position).magnitude;
			float num = magnitude - magnitude2;
			float obj = Mathf.Clamp(CameraZoom + num * TouchZoomSpeed, 6f, 100f);
			OnZoomByTouch?.Invoke(obj);
		}
	}

	public void ZoomCamera(float val)
	{
		if ((bool)captureCamera)
		{
			captureCamera.fieldOfView = val;
			CalculateCamRotationSpeed();
			if (ppProfile.TryGet<DepthOfField>(out var component))
			{
				component.focalLength.value = GetFocalLength(captureCamera);
			}
		}
	}

	private void CalculateCamRotationSpeed()
	{
		float num = Mathf.InverseLerp(0f, 100f, captureCamera.fieldOfView);
		RotationSpeed = Mathf.Clamp(RotationSpeed * num + SpeedOffset, 0.01f, 0.2f);
		TiltSpeed = Mathf.Clamp(TiltSpeed * num + SpeedOffset, 0.01f, 0.2f);
	}

	private void InitCameraRotAngle()
	{
		Vector3 eulerAngles = captureCamera.transform.eulerAngles;
		_yaw = eulerAngles.y;
		_pitch = eulerAngles.x;
		_roll = eulerAngles.z;
		if (_pitch > 180f)
		{
			_pitch -= 360f;
		}
	}

	public void ApplyLookDelta(Vector2 delta)
	{
		float f = _roll * ((float)Math.PI / 180f);
		float num = delta.x * Mathf.Cos(f) - delta.y * Mathf.Sin(f);
		float num2 = delta.x * Mathf.Sin(f) + delta.y * Mathf.Cos(f);
		_yaw += num * RotationSpeed;
		_pitch -= num2 * TiltSpeed;
		_pitch = Mathf.Clamp(_pitch, VerticalMinAngle, VerticalMaxAngle);
		Quaternion b = Quaternion.AngleAxis(_yaw, Vector3.up) * Quaternion.AngleAxis(_pitch, Vector3.right) * Quaternion.AngleAxis(_roll, Vector3.forward);
		captureCamera.transform.rotation = Quaternion.Slerp(captureCamera.transform.rotation, b, Time.unscaledDeltaTime / SmoothTime);
	}

	public void TiltDelta(float delta)
	{
		_roll += delta;
		TiltCamera(_roll);
	}

	private void UpdateRotAngle()
	{
		Vector3 eulerAngles = captureCamera.transform.rotation.eulerAngles;
		_pitch = ((eulerAngles.x > 180f) ? (eulerAngles.x - 360f) : eulerAngles.x);
		_yaw = eulerAngles.y;
		_roll = eulerAngles.z;
	}

	private void CameraRotateAround()
	{
		if (TouchDetector.IsPressed && !TouchDetector.IsZoom && Input.touchCount > 0 && TouchDetector.TouchId != -1 && Input.touchCount > TouchDetector.TouchId)
		{
			_currentTouch = Input.GetTouch(TouchDetector.TouchId);
			Vector2 deltaPosition = _currentTouch.deltaPosition;
			float f = _roll * ((float)Math.PI / 180f);
			float num = deltaPosition.x * Mathf.Cos(f) - deltaPosition.y * Mathf.Sin(f);
			float num2 = deltaPosition.x * Mathf.Sin(f) + deltaPosition.y * Mathf.Cos(f);
			_yaw += num * RotationSpeed;
			_pitch -= num2 * TiltSpeed;
			_pitch = Mathf.Clamp(_pitch, VerticalMinAngle, VerticalMaxAngle);
			Quaternion b = Quaternion.Euler(_pitch, _yaw, _roll);
			captureCamera.transform.rotation = Quaternion.Slerp(captureCamera.transform.rotation, b, Time.unscaledDeltaTime / SmoothTime);
		}
	}

	public void CastFocusPoint(Vector2 val)
	{
		if ((bool)captureCamera)
		{
			Ray ray = captureCamera.ScreenPointToRay(val);
			if (Physics.Raycast(ray.origin, ray.direction, out var hitInfo, float.PositiveInfinity, -1, QueryTriggerInteraction.Ignore))
			{
				SetFocusTransform(hitInfo.point, hitInfo.collider.transform);
				SetPPFocalPoint(Vector3.Distance(ray.origin, hitInfo.point));
			}
			else
			{
				SetFocusTransform(ray.origin + ray.direction * captureCamera.farClipPlane);
				SetPPFocalPoint(captureCamera.farClipPlane);
			}
		}
	}

	private void SetFocusTransform(Vector3 pos, Transform parent = null)
	{
		if (!FocusTransform)
		{
			FocusTransform = new GameObject("Camera Focus Point").transform;
		}
		FocusTransform.transform.position = pos;
		FocusTransform.SetParent(parent);
		FocusTransform.SetAsLastSibling();
	}

	public void SetAperture()
	{
		if (ppProfile.TryGet<DepthOfField>(out var component))
		{
			component.aperture.value = (float)m_Aperture * 0.1f;
		}
	}

	private void SetPPFocalPoint(float dist)
	{
		if (ppProfile.TryGet<DepthOfField>(out var component))
		{
			component.focusDistance.value = dist;
		}
	}

	public void SetCameraDistortion(float val)
	{
		if (ppProfile.TryGet<PaniniProjection>(out var component))
		{
			component.distance.value = val;
		}
	}

	public void ApplyColorCorrection()
	{
		if (ppProfile.TryGet<ColorLookup>(out var component))
		{
			if (CCLut != CPMGraphicsSettings.ColorCorrectionLut.Off)
			{
				component.contribution.value = 1f;
				component.texture.value = CPMGraphicsSettingsManager.Instance.ColorCorrectionLUTS[(int)(CCLut - 1)];
			}
			else
			{
				component.texture.value = null;
				component.contribution.value = 0f;
			}
		}
	}

	public void SetWhiteBallanceTint(float val)
	{
		if (ppProfile.TryGet<WhiteBalance>(out var component))
		{
			component.tint.value = val;
		}
	}

	public void SetWhiteBallance(float val)
	{
		if (ppProfile.TryGet<WhiteBalance>(out var component))
		{
			component.temperature.value = val;
		}
	}

	private void CaptureBaseSceneExposure()
	{
		baseSceneExposure = 0f;
		if ((bool)mainCamera && (bool)mainCamData)
		{
			Transform trigger = (mainCamData.volumeTrigger ? mainCamData.volumeTrigger : mainCamera.transform);
			VolumeManager.instance.Update(trigger, mainCamData.volumeLayerMask);
			ColorAdjustments component = VolumeManager.instance.stack.GetComponent<ColorAdjustments>();
			if (component != null)
			{
				baseSceneExposure = component.postExposure.value;
			}
		}
	}

	public void SetCameraExposure(float val)
	{
		if (ppProfile.TryGet<ColorAdjustments>(out var component))
		{
			component.postExposure.value = baseSceneExposure + val;
		}
	}

	public void SaveScreenshot()
	{
		SaveRenderTexturePNG(captureRT);
	}

	private void SaveRenderTexturePNG(RenderTexture linearRT)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(linearRT.width, linearRT.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
		Graphics.Blit(linearRT, temporary);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = temporary;
		Texture2D texture2D = new Texture2D(temporary.width, temporary.height, TextureFormat.RGB24, mipChain: false, linear: false);
		texture2D.ReadPixels(new Rect(0f, 0f, texture2D.width, texture2D.height), 0, 0);
		texture2D.Apply();
		string screenShotPath = GetScreenShotPath();
		TextureExporter.ExportToFile(texture2D, screenShotPath, TextureExporter.ImageFileFormat.PNG, 100);
		UnityEngine.Object.DestroyImmediate(texture2D);
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(temporary);
	}

	private string GetScreenShotPath()
	{
		string text = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
		if (RecordSequence)
		{
			text = sequencecounter.ToString("0000");
			sequencecounter++;
		}
		string path = text + ".png";
		string text2 = ScreenshotNameParser.ParsePath(ScreenshotNameParser.DestinationFolder.PICTURES_FOLDER, "CPM2/");
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		text2 = Path.Combine(text2, path);
		return PathUtils.PreventOverwrite(text2);
	}

	private void LateUpdate()
	{
		switch (ModeState)
		{
		case PhotoModeState.PauseAndFrame:
			HandleCameraControl();
			break;
		case PhotoModeState.Capture:
			if (m_CameraPanning != CameraPanning.AttachedToTarget)
			{
				captureCamera.transform.rotation = LookRotationWithOffset(captureCamera.transform, FocusTransform, LookOffset, Vector3.up);
			}
			if (m_CameraPanning == CameraPanning.FollowTarget)
			{
				captureCamera.transform.position = FocusTransform.transform.position + PosOffset;
			}
			if (RealtimeMode)
			{
				captureCamera.Render();
				Graphics.Blit(captureRT, accumulationTex);
				PostCapture();
			}
			else if (FrameCounter < AccumFrames)
			{
				if (FrameCounter > 0)
				{
					captureCamera.Render();
					if (useDoubleBuffer)
					{
						bool flag = FrameCounter % 2 == 0;
						AccumMat.SetTexture("_BackgroundTex", flag ? accumulationDoubleBufferTex : accumulationTex);
						Graphics.Blit(captureRT, flag ? accumulationTex : accumulationDoubleBufferTex, AccumMat);
						PhotoModeWindow.SetDisplayTexture(flag ? accumulationTex : accumulationDoubleBufferTex);
					}
					else
					{
						Graphics.Blit(captureRT, accumulationTex, AccumMat);
					}
				}
				FrameCounter++;
				float num4 = 1f / (1f / (float)AccumFrames * (float)FrameCounter);
				PhotoModeWindow.SetDisplayColor(new Color(num4, num4, num4, 1f));
			}
			else
			{
				PostCapture();
			}
			break;
		case PhotoModeState.PostCapture:
			if ((bool)QuadMaterial && (bool)_quad)
			{
				float num = 0.01f;
				float num2;
				float x;
				if (captureCamera.orthographic)
				{
					num2 = captureCamera.orthographicSize * 2f;
					x = num2 * captureCamera.aspect;
				}
				else
				{
					float num3 = captureCamera.nearClipPlane + num;
					num2 = 2f * num3 * Mathf.Tan(captureCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f));
					x = num2 * captureCamera.aspect;
				}
				Vector3 pos = captureCamera.transform.position + captureCamera.transform.forward * (captureCamera.nearClipPlane + num);
				Quaternion rotation = captureCamera.transform.rotation;
				Vector3 s = new Vector3(x, num2, 1f);
				Matrix4x4 matrix = Matrix4x4.TRS(pos, rotation, s);
				Graphics.DrawMesh(_quad, matrix, QuadMaterial, 0, captureCamera);
			}
			break;
		case PhotoModeState.StandBy:
			break;
		}
	}

	private void HandleCameraControl()
	{
		if (IsJoystickPressed)
		{
			MoveCamera();
		}
		CameraRotateAround();
		if (Input.touchCount > 1)
		{
			ZoomByTouch();
		}
	}

	public Quaternion GetLookOffset(Transform cam, Transform target, Vector3 up = default(Vector3))
	{
		if (up == default(Vector3))
		{
			up = Vector3.up;
		}
		Vector3 forward = target.position - cam.position;
		if (forward.sqrMagnitude < 1E-06f)
		{
			return Quaternion.identity;
		}
		return Quaternion.Inverse(Quaternion.LookRotation(forward, up)) * cam.rotation;
	}

	public Quaternion LookRotationWithOffset(Transform cam, Transform target, Quaternion cachedOffset, Vector3 up = default(Vector3))
	{
		if (up == default(Vector3))
		{
			up = Vector3.up;
		}
		Vector3 forward = target.position - cam.position;
		if (forward.sqrMagnitude < 1E-06f)
		{
			return cam.rotation;
		}
		return Quaternion.LookRotation(forward, up) * cachedOffset;
	}

	private float Seconds(ShutterSpeed speed)
	{
		if (speed >= (ShutterSpeed)0)
		{
			return 1f / (float)speed;
		}
		return 0 - speed;
	}

	private float GetFocalLength(Camera cam)
	{
		float num = (cam.usePhysicalProperties ? cam.sensorSize.y : 24f);
		return 0.5f * num / Mathf.Tan(cam.fieldOfView * 0.5f * ((float)Math.PI / 180f));
	}

	private LayerMask LayerMaskFromLayer(int layerIndex)
	{
		return 1 << layerIndex;
	}

	private bool IsLowRam()
	{
		return SystemInfo.systemMemorySize <= 1600;
	}
}
