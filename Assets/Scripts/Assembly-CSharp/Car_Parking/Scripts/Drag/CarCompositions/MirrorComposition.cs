using System;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting;
using _Car_Parking;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Steam.Graphics;
using _Car_Parking.Scripts.Steam.Perf;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	[Preserve]
	public class MirrorComposition : MonoBehaviour
	{
		[SerializeField]
		private Camera Camera;

		[SerializeField]
		private Camera LCamera;

		[SerializeField]
		private Camera RCamera;

		[SerializeField]
		private Transform MirrorTransform;

		[SerializeField]
		private Transform LMirrorTransform;

		[SerializeField]
		private Transform RMirrorTransform;

		private RenderTexture _leftMirrorTexture;

		private RenderTexture _rightMirrorTexture;

		private RenderTexture _middleMirrorTexture;

		private Material _leftMirrorMaterial;

		private Material _rightMirrorMaterial;

		private Material _middleMirrorMaterial;

		private float _leftMirrorView;

		private float _rightMirrorView;

		private float _middleMirrorView;

		private VehicleData _currentVehicle;

		private bool _isRendering;

		private int _mirrorGeneration;

		private UniversalAdditionalCameraData _additionalCameraData;

		private UniversalAdditionalCameraData _lCameraData;

		private UniversalAdditionalCameraData _rCameraData;

		private UniversalAdditionalCameraData _mCameraData;

		private bool _steamShadowsForcedOff;

		private float _steamFarL;

		private float _steamFarR;

		private float _steamFarM;

		private float _steamNextL;

		private float _steamNextR;

		private float _steamNextM;

		private bool _steamTimedActive;

		private bool _steamWasInteriorView;

		private bool _steamWasVisibleL;

		private bool _steamWasVisibleR;

		private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

		private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");

		private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

		private static readonly int MatSmooth = Shader.PropertyToID("_Smoothness");

		private static readonly int MatMetal = Shader.PropertyToID("_Metalness");

		private static readonly string[] ExcludedMirrorLayers = new string[9] { "Water", "WheelCollider", "Tunnel", "Map", "UI", "map", "VinylCamera", "short distance", "clickPlane" };

		private const float SlowSpeedMps = 1.5f;

		private const float MidSpeedMps = 8f;

		private const int SlowFrameSkip = 4;

		private const int MidFrameSkip = 2;

		private const int FastFrameSkip = 1;

		private void Start()
		{
			Init();
		}

		private void Init()
		{
			InitCamera(LCamera);
			InitCamera(RCamera);
			InitCamera(Camera);
			InitMaterials();
			CPMGraphicsSettingsManager.Instance.OnQualityChanged += OnQualityChanged;
			_lCameraData = (LCamera ? LCamera.GetUniversalAdditionalCameraData() : null);
			_rCameraData = (RCamera ? RCamera.GetUniversalAdditionalCameraData() : null);
			_mCameraData = (Camera ? Camera.GetUniversalAdditionalCameraData() : null);
			_steamFarL = (LCamera ? LCamera.farClipPlane : 0f);
			_steamFarR = (RCamera ? RCamera.farClipPlane : 0f);
			_steamFarM = (Camera ? Camera.farClipPlane : 0f);
			SteamMirrorTuning.RegisterCamera(LCamera, interior: false);
			SteamMirrorTuning.RegisterCamera(RCamera, interior: false);
			SteamMirrorTuning.RegisterCamera(Camera, interior: true);
		}

		private void ApplySteamMirrorShadows()
		{
			bool flag = SteamPerfFlags.IsOn("mirrorShadows");
			if (flag || _steamShadowsForcedOff)
			{
				_steamShadowsForcedOff = flag;
				if ((bool)_lCameraData)
				{
					_lCameraData.renderShadows = !flag && CPMGraphicsSettingsManager.Instance.Quality >= 2;
				}
				if ((bool)_rCameraData)
				{
					_rCameraData.renderShadows = !flag;
				}
				if ((bool)_mCameraData)
				{
					_mCameraData.renderShadows = false;
				}
			}
		}

		private void ApplySteamMirrorClip()
		{
			bool clipEnabled = SteamMirrorTuning.ClipEnabled;
			float b = 150f;
			if ((bool)LCamera)
			{
				LCamera.farClipPlane = (clipEnabled ? Mathf.Min(_steamFarL, b) : _steamFarL);
			}
			if ((bool)RCamera)
			{
				RCamera.farClipPlane = (clipEnabled ? Mathf.Min(_steamFarR, b) : _steamFarR);
			}
			if ((bool)Camera)
			{
				Camera.farClipPlane = (clipEnabled ? Mathf.Min(_steamFarM, b) : _steamFarM);
			}
		}

		private void SteamTimedMirrorStep()
		{
			float unscaledTime = Time.unscaledTime;
			float num = SteamMirrorRate();
			if (!_steamTimedActive)
			{
				_steamTimedActive = true;
				_steamNextL = unscaledTime;
				_steamNextR = unscaledTime + 1f / (3f * num);
				_steamNextM = unscaledTime + 2f / (3f * num);
			}
			bool flag = IsSteamInteriorView();
			float rate = num;
			if (SteamMirrorTuning.InteriorEnabled && !flag)
			{
				rate = 5f;
			}
			if (flag && !_steamWasInteriorView)
			{
				_steamNextM = unscaledTime;
				_steamNextL = unscaledTime;
				_steamNextR = unscaledTime;
			}
			_steamWasInteriorView = flag;
			CarChildsReferences carChildsReferences = _currentVehicle.CarChildsReferences;
			float rate2 = SteamSideMirrorRate(carChildsReferences.LCam, num, flag, ref _steamWasVisibleL, ref _steamNextL, unscaledTime);
			float rate3 = SteamSideMirrorRate(carChildsReferences.RCam, num, flag, ref _steamWasVisibleR, ref _steamNextR, unscaledTime);
			SteamStepMirror(LCamera, LMirrorTransform, carChildsReferences.LCam, _leftMirrorTexture, isMiddle: false, rate2, ref _steamNextL, unscaledTime);
			SteamStepMirror(RCamera, RMirrorTransform, carChildsReferences.RCam, _rightMirrorTexture, isMiddle: false, rate3, ref _steamNextR, unscaledTime);
			SteamStepMirror(Camera, MirrorTransform, carChildsReferences.MCam, _middleMirrorTexture, isMiddle: true, rate, ref _steamNextM, unscaledTime);
		}

		private void SteamStepMirror(Camera camera, Transform mirrorTransform, Transform mirror, RenderTexture renderTexture, bool isMiddle, float rate, ref float next, float now)
		{
			if ((bool)camera && (camera.enabled = (bool)mirror && now >= next))
			{
				float num = 1f / rate;
				next = ((next + num < now) ? (now + num) : (next + num));
				mirrorTransform.SetPositionAndRotation(mirror.position, mirror.rotation);
				camera.targetTexture = renderTexture;
				camera.fieldOfView = (isMiddle ? _currentVehicle.CarMirrorController.MiddleMirrorView : _currentVehicle.CarMirrorController.SideMirrorView);
				camera.rect = new Rect(0f, 0f, isMiddle ? 2f : 1f, 1f);
			}
		}

		private float SteamMirrorRate()
		{
			Powertrain powertrain = (_currentVehicle ? _currentVehicle.GetPowertrain : null);
			float num = (powertrain ? Mathf.Abs(powertrain.SpeedMps) : 0f);
			if (num < 1.5f)
			{
				return 15f;
			}
			if (!(num < 8f))
			{
				return 60f;
			}
			return 30f;
		}

		private float SteamSideMirrorRate(Transform anchor, float rate, bool interiorView, ref bool wasVisible, ref float next, float now)
		{
			if ((!SteamMirrorTuning.ExteriorEnabled | interiorView) || !anchor)
			{
				wasVisible = true;
				return rate;
			}
			bool flag = IsSteamMirrorVisible(anchor);
			if (flag && !wasVisible)
			{
				next = now;
			}
			wasVisible = flag;
			if (!flag)
			{
				return 0.1f;
			}
			return 5f;
		}

		private static bool IsSteamMirrorVisible(Transform anchor)
		{
			Camera main = Camera.main;
			if (!main)
			{
				return true;
			}
			Vector3 rhs = main.transform.position - anchor.position;
			if (Vector3.Dot(anchor.forward, rhs) <= 0f)
			{
				return false;
			}
			Vector3 vector = main.WorldToViewportPoint(anchor.position);
			if (vector.z > 0f && vector.x > -0.1f && vector.x < 1.1f && vector.y > -0.1f)
			{
				return vector.y < 1.1f;
			}
			return false;
		}

		private bool IsSteamInteriorView()
		{
			Camera main = Camera.main;
			Transform mCam = _currentVehicle.CarChildsReferences.MCam;
			if (!main || !mCam)
			{
				return false;
			}
			float num = 2.5f;
			return (main.transform.position - mCam.position).sqrMagnitude < num * num;
		}

		private void OnDestroy()
		{
			CPMGraphicsSettingsManager.Instance.OnQualityChanged -= OnQualityChanged;
			SteamMirrorTuning.UnregisterCamera(LCamera);
			SteamMirrorTuning.UnregisterCamera(RCamera);
			SteamMirrorTuning.UnregisterCamera(Camera);
		}

		private void OnQualityChanged(int quality)
		{
			if (!_additionalCameraData)
			{
				_additionalCameraData = Camera.GetUniversalAdditionalCameraData();
			}
			_additionalCameraData.renderShadows = quality >= 2;
		}

		private static int BuildMirrorCullingMask()
		{
			int num = 0;
			string[] excludedMirrorLayers = ExcludedMirrorLayers;
			foreach (string text in excludedMirrorLayers)
			{
				int num2 = LayerMask.NameToLayer(text);
				if (num2 < 0)
				{
					Debug.LogWarning("[MirrorComposition] слой '" + text + "' не найден — в зеркале он останется");
				}
				else
				{
					num |= 1 << num2;
				}
			}
			return ~num;
		}

		private void InitCamera(Camera camera)
		{
			camera.enabled = false;
			camera.cullingMask = BuildMirrorCullingMask();
			if (!_additionalCameraData)
			{
				_additionalCameraData = camera.GetUniversalAdditionalCameraData();
			}
			int renderer = 3;
			if ((bool)_additionalCameraData)
			{
				_additionalCameraData.SetRenderer(renderer);
			}
			else
			{
				Debug.LogError("No UniversalAdditionalCameraData found on mirror camera!");
			}
		}

		private void InitMaterials()
		{
			_leftMirrorTexture = UnityEngine.Object.Instantiate(Resources.Load("CP/L Texture", typeof(RenderTexture))) as RenderTexture;
			_rightMirrorTexture = UnityEngine.Object.Instantiate(Resources.Load("CP/R Texture", typeof(RenderTexture))) as RenderTexture;
			_middleMirrorTexture = UnityEngine.Object.Instantiate(Resources.Load("CP/M Texture", typeof(RenderTexture))) as RenderTexture;
			_leftMirrorMaterial = CreateMaterial(_leftMirrorTexture);
			_rightMirrorMaterial = CreateMaterial(_rightMirrorTexture);
			_middleMirrorMaterial = CreateMaterial(_middleMirrorTexture);
		}

		private Material CreateMaterial(RenderTexture renderTexture)
		{
			Material material = new Material(Shader.Find("CPM/URP/Automotive/AutomotiveSimple"));
			material.color = Color.black;
			material.SetColor(EmissionColor, Color.white);
			material.SetTexture(EmissionMap, renderTexture);
			material.SetFloat(MatSmooth, 0f);
			material.SetFloat(MatMetal, 1f);
			return material;
		}

		public async UniTaskVoid ActivateForCar(VehicleData vehicle)
		{
			_currentVehicle = vehicle;
			if ((bool)vehicle.CarMirrorController.lMirror)
			{
				vehicle.CarMirrorController.lMirror.GetComponent<MeshRenderer>().sharedMaterial = _leftMirrorMaterial;
			}
			if ((bool)vehicle.CarMirrorController.rMirror)
			{
				vehicle.CarMirrorController.rMirror.GetComponent<MeshRenderer>().sharedMaterial = _rightMirrorMaterial;
			}
			if ((bool)vehicle.CarMirrorController.mMirror)
			{
				vehicle.CarMirrorController.mMirror.GetComponent<MeshRenderer>().sharedMaterial = _middleMirrorMaterial;
			}
			await UniTask.Yield();
			_currentVehicle = vehicle;
			if (!_isRendering)
			{
				_isRendering = true;
				Camera.enabled = true;
				if ((bool)RCamera)
				{
					RCamera.enabled = true;
				}
				if ((bool)LCamera)
				{
					LCamera.enabled = true;
				}
				UpdateMirror(++_mirrorGeneration).Forget();
			}
		}

		public void Deactivate(VehicleData vehicle)
		{
			if (!(vehicle != _currentVehicle))
			{
				Camera.enabled = false;
				_isRendering = false;
				_currentVehicle = null;
				if ((bool)RCamera)
				{
					RCamera.enabled = false;
				}
				if ((bool)LCamera)
				{
					LCamera.enabled = false;
				}
			}
		}

		private async UniTaskVoid UpdateMirror(int generation)
		{
			_additionalCameraData.renderShadows = CPMGraphicsSettingsManager.Instance.Quality >= 2;
			try
			{
				while (FlagsStorage.ShowCarMirror && _isRendering && (bool)_currentVehicle && generation == _mirrorGeneration)
				{
					ApplySteamMirrorShadows();
					ApplySteamMirrorClip();
					if (SteamMirrorTuning.RateEnabled)
					{
						SteamTimedMirrorStep();
						await UniTask.DelayFrame(1);
						if (!_isRendering || !_currentVehicle)
						{
							break;
						}
						continue;
					}
					if (_steamTimedActive)
					{
						_steamTimedActive = false;
						if ((bool)LCamera)
						{
							LCamera.enabled = true;
						}
						if ((bool)RCamera)
						{
							RCamera.enabled = true;
						}
						if ((bool)Camera)
						{
							Camera.enabled = true;
						}
					}
					await UniTask.WhenAll(Render(LCamera, LMirrorTransform, _currentVehicle.CarChildsReferences.LCam, _leftMirrorTexture, isMiddle: false), Render(RCamera, RMirrorTransform, _currentVehicle.CarChildsReferences.RCam, _rightMirrorTexture, isMiddle: false), Render(Camera, MirrorTransform, _currentVehicle.CarChildsReferences.MCam, _middleMirrorTexture, isMiddle: true));
					if (!_isRendering || !_currentVehicle)
					{
						break;
					}
				}
			}
			catch (Exception arg)
			{
				Debug.LogError($"[MirrorComposition] цикл рендера зеркал упал: {arg}");
			}
			finally
			{
				if (generation == _mirrorGeneration)
				{
					_isRendering = false;
					_currentVehicle = null;
					if ((bool)Camera)
					{
						Camera.enabled = false;
					}
					if ((bool)RCamera)
					{
						RCamera.enabled = false;
					}
					if ((bool)LCamera)
					{
						LCamera.enabled = false;
					}
				}
			}
		}

		private int FrameSkipForSpeed()
		{
			Powertrain powertrain = (_currentVehicle ? _currentVehicle.GetPowertrain : null);
			if (!powertrain)
			{
				return 4;
			}
			float num = Mathf.Abs(powertrain.SpeedMps);
			if (num < 1.5f)
			{
				return 4;
			}
			if (!(num < 8f))
			{
				return 1;
			}
			return 2;
		}

		private async UniTask Render(Camera camera, Transform mirrorTransform, Transform mirror, RenderTexture renderTexture, bool isMiddle)
		{
			if (!mirror)
			{
				return;
			}
			Transform transform = mirror.transform;
			mirrorTransform.SetPositionAndRotation(transform.position, transform.rotation);
			camera.targetTexture = renderTexture;
			camera.fieldOfView = (isMiddle ? _currentVehicle.CarMirrorController.MiddleMirrorView : _currentVehicle.CarMirrorController.SideMirrorView);
			camera.rect = new Rect(0f, 0f, isMiddle ? 2f : 1f, 1f);
			int num = FrameSkipForSpeed();
			if (num > 1)
			{
				camera.enabled = false;
				await UniTask.DelayFrame(num - 1);
				if (!_isRendering || !_currentVehicle || !mirror)
				{
					return;
				}
				camera.enabled = true;
			}
			await UniTask.DelayFrame(1);
		}
	}
}
