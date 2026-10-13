using System;
using System.Collections.Generic;
using CPM.Database;
using UnityEngine;
using UnityEngine.Rendering;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Car.Visual;
using _Car_Parking.Scripts.Drag;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class WindowComposition
	{
		private sealed class FenderWindow
		{
			public BodyKitReferences Refs;

			public Renderer[] Renderers;

			public Material Mat;

			public Vector2 Range;

			public Vector4 TilingOffset;

			public float Scale;

			public Vector4 ShatterTileOffset;
		}

		public readonly struct WiperSettings
		{
			public readonly float WipeDuration;

			public readonly float DwellDuration;

			public readonly float DwellJitter;

			public readonly float RefillDuration;

			public readonly float RefillMin;

			public readonly float WipePower;

			public readonly float RefillPower;

			public WiperSettings(float wipeDuration, float dwellDuration, float dwellJitter, float refillDuration, float refillMin, float wipePower, float refillPower)
			{
				WipeDuration = wipeDuration;
				DwellDuration = dwellDuration;
				DwellJitter = dwellJitter;
				RefillDuration = refillDuration;
				RefillMin = refillMin;
				WipePower = wipePower;
				RefillPower = refillPower;
			}
		}

		private readonly VehicleData _vehicle;

		private readonly ColorsService _colorsService;

		private readonly WindshieldBrakeService _windshieldBrakeService;

		private bool _isWindowMatSpawned;

		private bool _windowBroken;

		public Material WindowMat;

		public RenderTexture WindowRT;

		public GameObject WindowBody;

		private bool _wipersActive;

		private float _wipeElapsed;

		private float _dwellElapsed;

		private float _dwellTarget;

		private int _wiperInvert;

		private bool _wiping = true;

		private int _wiperMode = -1;

		private int _wipePhase;

		private bool _wiperStopping;

		private bool _wiperParked;

		private float _lastWipe = float.NaN;

		private float _lastOverlay = float.NaN;

		private int _lastWiperInvert = -1;

		private float _lastCull = -1f;

		private float _lastFPRainBlend = -1f;

		private float _lastPhysicalWiperValue;

		private bool _hasPhysicalWiperValue;

		private Material _wipersNeutralOn;

		private GameObject _wiperAudioEmitter;

		private readonly List<IPhysicalWiperSource> _wiperSources = new List<IPhysicalWiperSource>();

		private bool _wiperSourcesDirty = true;

		private bool _hasPhysicalWipersCached;

		private int _wiperModeCached;

		private static readonly int ShatterTileOffsetId = Shader.PropertyToID("_ShatterTileOffset");

		private readonly List<FenderWindow> _fenderWindows = new List<FenderWindow>();

		private readonly List<BodyKitReferences> _customWindowKits = new List<BodyKitReferences>();

		public bool HasPhysicalWipers
		{
			get
			{
				EnsureWiperSources();
				return _hasPhysicalWipersCached;
			}
		}

		public WindowComposition(VehicleData vehicle, ColorsService colorsService, WindshieldBrakeService windshieldBrakeService)
		{
			_vehicle = vehicle;
			_colorsService = colorsService;
			_windshieldBrakeService = windshieldBrakeService;
		}

		public void SubscribeToGraphicLoaded()
		{
			_vehicle.OnCarGraphicLoaded += SetWindowColor;
		}

		private void SetWindowColor()
		{
			_vehicle.OnCarGraphicLoaded -= SetWindowColor;
			if (!(V.Car != _vehicle.gameObject) && (bool)WindowMat)
			{
				WindowMat.color = PlayerDatas.GetColor($"CarColorWindow{_vehicle.CarId}", WindowMat.color);
				Color c = WindowMat.color;
				ForEachFenderMat((Material m) =>
				{
					m.color = c;
				});
			}
		}

		public void CreateWindowMat(GameObject go)
		{
			Debug.Log("[CarData] CreateWindowMat" + _vehicle.gameObject.name + " " + go.name);
			if (!go)
			{
				return;
			}
			Renderer component = go.GetComponent<Renderer>();
			if (!WindowMat)
			{
				go.name = "window";
				if (!component.sharedMaterial)
				{
					return;
				}
				CreateWindowMat();
				WindowBody = go;
				_isWindowMatSpawned = true;
			}
			component.material = WindowMat;
		}

		public void CreateWindowMat()
		{
			if ((bool)WindowMat)
			{
				return;
			}
			Vector4 holeParameters = new Vector4(_vehicle.WindowShatterHoleSize.x, _vehicle.WindowShatterHoleSize.y, _vehicle.WindowShatterHolePosition.x, _vehicle.WindowShatterHolePosition.y);
			Material material = (WindowMat = _colorsService.CreateWindowMaterial(holeParameters, _vehicle.WindowWipersTilingOffset, _vehicle.WindowWipersScale, _vehicle.WindowWipersVerticalInvert, _vehicle.WindowWipersHorizontalInvert));
			_isWindowMatSpawned = true;
			if (_vehicle.mppc.isOnlineCar)
			{
				if (_vehicle.TryGetComponent<CarInfoContainer>(out var component))
				{
					CarInfo carInfo = component.CarInfo;
					if (carInfo != null && carInfo.Colors != null)
					{
						material.color = component.CarInfo.Colors.WindowColor;
					}
				}
			}
			else if (_vehicle.carInfo != null)
			{
				material.color = _vehicle.carInfo.Colors.WindowColor;
			}
		}

		public void SetWindowShatterHoleActive(bool isBreak)
		{
			if (!WindowMat)
			{
				return;
			}
			if (isBreak)
			{
				WindowMat.SetFloat(ShaderPropertyIDs.ShatterHole, 1f);
				WindowMat.SetFloat(ShaderPropertyIDs.Shattered, 1f);
				ForEachFenderMat((Material m) =>
				{
					m.SetFloat(ShaderPropertyIDs.ShatterHole, 1f);
					m.SetFloat(ShaderPropertyIDs.Shattered, 1f);
				});
				if (_windshieldBrakeService != null && !_windowBroken)
				{
					if ((bool)_vehicle && _vehicle.gameObject.activeInHierarchy)
					{
						AudioWrapper.PostEvent("Play_Car_window_crash", _vehicle.gameObject);
					}
					if ((bool)WindowBody)
					{
						_windshieldBrakeService.ShowBrakeVfx(WindowBody.transform).Forget();
					}
					_windowBroken = true;
				}
			}
			else
			{
				WindowMat.SetFloat(ShaderPropertyIDs.ShatterHole, 0f);
				WindowMat.SetFloat(ShaderPropertyIDs.Shattered, 0f);
				ForEachFenderMat((Material m) =>
				{
					m.SetFloat(ShaderPropertyIDs.ShatterHole, 0f);
					m.SetFloat(ShaderPropertyIDs.Shattered, 0f);
				});
				_windowBroken = false;
			}
		}

		public void RefreshWiperSources()
		{
			_wiperSourcesDirty = true;
			EnsureWiperSources();
		}

		private void EnsureWiperSources()
		{
			if (_wiperSourcesDirty)
			{
				_wiperSourcesDirty = false;
				_wiperSources.Clear();
				if (_vehicle.CarBodyReferences != null)
				{
					_wiperSources.Add(_vehicle.CarBodyReferences);
				}
				if (_vehicle.BodyKits != null)
				{
					_vehicle.BodyKits.CollectPhysicalWiperSources(_wiperSources);
				}
				RebuildFenderWindows();
				_hasPhysicalWipersCached = ComputeHasPhysicalWipers();
				_wiperModeCached = ComputeWiperMode();
			}
		}

		private void RebuildFenderWindows()
		{
			for (int i = 0; i < _fenderWindows.Count; i++)
			{
				FenderWindow fenderWindow = _fenderWindows[i];
				if ((bool)fenderWindow.Mat)
				{
					UnbindFenderMat(fenderWindow);
					UnityEngine.Object.Destroy(fenderWindow.Mat);
				}
			}
			_fenderWindows.Clear();
			if (_vehicle.BodyKits == null)
			{
				return;
			}
			_customWindowKits.Clear();
			_vehicle.BodyKits.CollectCustomWindowKits(_customWindowKits);
			for (int j = 0; j < _customWindowKits.Count; j++)
			{
				BodyKitReferences bodyKitReferences = _customWindowKits[j];
				if ((bool)bodyKitReferences)
				{
					Renderer[] array = ResolveKitGlassRenderers(bodyKitReferences);
					if (array.Length != 0)
					{
						FenderWindow fenderWindow2 = new FenderWindow
						{
							Refs = bodyKitReferences,
							Renderers = array,
							Range = bodyKitReferences.WiperShaderRange,
							TilingOffset = bodyKitReferences.WindowWipersTilingOffset,
							Scale = bodyKitReferences.WindowWipersScale,
							ShatterTileOffset = new Vector4(bodyKitReferences.WindowShatterHoleSize.x, bodyKitReferences.WindowShatterHoleSize.y, bodyKitReferences.WindowShatterHolePosition.x, bodyKitReferences.WindowShatterHolePosition.y)
						};
						_fenderWindows.Add(fenderWindow2);
						PushFenderWindow(fenderWindow2, 0f);
					}
				}
			}
			ApplySharedInvertFromActiveWindow();
		}

		private void ApplySharedInvertFromActiveWindow()
		{
			if ((bool)WindowMat)
			{
				bool windowWipersVerticalInvert;
				bool windowWipersHorizontalInvert;
				if (_fenderWindows.Count > 0 && (bool)_fenderWindows[0].Refs)
				{
					windowWipersVerticalInvert = _fenderWindows[0].Refs.WindowWipersVerticalInvert;
					windowWipersHorizontalInvert = _fenderWindows[0].Refs.WindowWipersHorizontalInvert;
				}
				else
				{
					windowWipersVerticalInvert = _vehicle.WindowWipersVerticalInvert;
					windowWipersHorizontalInvert = _vehicle.WindowWipersHorizontalInvert;
				}
				Shader shader = WindowMat.shader;
				WindowMat.SetKeyword(new LocalKeyword(shader, "_WIPERSVERTICALINVERT"), windowWipersVerticalInvert);
				WindowMat.SetKeyword(new LocalKeyword(shader, "_WIPERSHORIZONTALINVERT"), windowWipersHorizontalInvert);
			}
		}

		private void UnbindFenderMat(FenderWindow fender)
		{
			if (!WindowMat)
			{
				return;
			}
			for (int i = 0; i < fender.Renderers.Length; i++)
			{
				if ((bool)fender.Renderers[i])
				{
					fender.Renderers[i].sharedMaterial = WindowMat;
				}
			}
		}

		private static Renderer[] ResolveKitGlassRenderers(BodyKitReferences refs)
		{
			if (!refs.TryGetComponent<bodyKitValues>(out var component) || component.GlassWindows == null)
			{
				return Array.Empty<Renderer>();
			}
			List<Renderer> list = new List<Renderer>(component.GlassWindows.Length);
			GameObject[] glassWindows = component.GlassWindows;
			foreach (GameObject gameObject in glassWindows)
			{
				if ((bool)gameObject && gameObject.TryGetComponent<Renderer>(out var component2))
				{
					list.Add(component2);
				}
			}
			return list.ToArray();
		}

		private void PushFenderWindow(FenderWindow fender, float wipersValue)
		{
			if (EnsureFenderMat(fender))
			{
				fender.Mat.SetFloat(ShaderPropertyIDs.WipersValue, wipersValue);
				fender.Mat.SetFloat(ShaderPropertyIDs.RainOverlayValue, 0f);
				fender.Mat.SetFloat(ShaderPropertyIDs.InvertWipers, 0f);
			}
		}

		private bool EnsureFenderMat(FenderWindow fender)
		{
			if ((bool)fender.Mat)
			{
				return true;
			}
			if (!WindowMat)
			{
				return false;
			}
			Material material = new Material(WindowMat)
			{
				name = WindowMat.name + "_kit"
			};
			material.SetVector(ShaderPropertyIDs.WipersTilingOffset, fender.TilingOffset);
			material.SetFloat(ShaderPropertyIDs.WipersScale, fender.Scale);
			material.SetVector(ShatterTileOffsetId, fender.ShatterTileOffset);
			if ((bool)fender.Refs)
			{
				material.SetKeyword(new LocalKeyword(material.shader, "_WIPERSVERTICALINVERT"), fender.Refs.WindowWipersVerticalInvert);
				material.SetKeyword(new LocalKeyword(material.shader, "_WIPERSHORIZONTALINVERT"), fender.Refs.WindowWipersHorizontalInvert);
			}
			fender.Mat = material;
			for (int i = 0; i < fender.Renderers.Length; i++)
			{
				Renderer renderer = fender.Renderers[i];
				if ((bool)renderer)
				{
					renderer.SetPropertyBlock(null);
					renderer.sharedMaterial = material;
				}
			}
			return true;
		}

		private void ForEachFenderMat(Action<Material> apply)
		{
			for (int i = 0; i < _fenderWindows.Count; i++)
			{
				if ((bool)_fenderWindows[i].Mat)
				{
					apply(_fenderWindows[i].Mat);
				}
			}
		}

		private int ResolveWiperModeInt()
		{
			EnsureWiperSources();
			return _wiperModeCached;
		}

		private int ComputeWiperMode()
		{
			for (int i = 0; i < _fenderWindows.Count; i++)
			{
				BodyKitReferences refs = _fenderWindows[i].Refs;
				if ((bool)refs)
				{
					if (refs.WiperMode != CarBodyReferences.WiperAnimationMode.WipeAndReturn)
					{
						return 0;
					}
					return 1;
				}
			}
			CarBodyReferences carBodyReferences = _vehicle.CarBodyReferences;
			if (!(carBodyReferences != null) || carBodyReferences.WiperMode != CarBodyReferences.WiperAnimationMode.WipeAndReturn)
			{
				return 0;
			}
			return 1;
		}

		private bool ComputeHasPhysicalWipers()
		{
			for (int i = 0; i < _wiperSources.Count; i++)
			{
				IPhysicalWiperSource physicalWiperSource = _wiperSources[i];
				if (physicalWiperSource is UnityEngine.Object obj && (bool)obj && physicalWiperSource.HasPhysicalWipers)
				{
					return true;
				}
			}
			return false;
		}

		private void PosePhysicalWipers(float value)
		{
			EnsureWiperSources();
			for (int i = 0; i < _wiperSources.Count; i++)
			{
				IPhysicalWiperSource physicalWiperSource = _wiperSources[i];
				if (physicalWiperSource is UnityEngine.Object obj && (bool)obj)
				{
					physicalWiperSource.ApplyWiperPose(value);
				}
			}
		}

		public void TickWipers(float dt, in WiperSettings s)
		{
			_wiperStopping = false;
			_wiperParked = false;
			AdvanceWipers(dt, in s);
		}

		public bool TickWiperStop(float dt, in WiperSettings s)
		{
			if (!_wipersActive || _wiperParked)
			{
				return true;
			}
			if (!WindowMat)
			{
				StopWipers();
				return true;
			}
			_wiperStopping = true;
			AdvanceWipers(dt, in s);
			return _wiperParked;
		}

		public void ClearWipers()
		{
			if (_wipersActive)
			{
				StopWipers();
				_wipersNeutralOn = WindowMat;
			}
			else
			{
				if (!WindowMat || _wipersNeutralOn == WindowMat)
				{
					return;
				}
				_wipersNeutralOn = WindowMat;
				WindowMat.SetFloat(ShaderPropertyIDs.WipersValue, 0f);
				WindowMat.SetFloat(ShaderPropertyIDs.RainOverlayValue, 0f);
				WindowMat.SetFloat(ShaderPropertyIDs.InvertWipers, 0f);
				for (int i = 0; i < _fenderWindows.Count; i++)
				{
					if ((bool)_fenderWindows[i].Refs)
					{
						PushFenderWindow(_fenderWindows[i], 0f);
					}
				}
			}
		}

		private void AdvanceWipers(float dt, in WiperSettings s)
		{
			if ((bool)WindowMat)
			{
				_wipersNeutralOn = null;
				int num = ResolveWiperModeInt();
				if (!_wipersActive || num != _wiperMode)
				{
					_wipersActive = true;
					_wiperMode = num;
					ResetWiperCycle(in s);
				}
				if (num == 1)
				{
					TickWipeAndReturn(dt, in s);
				}
				else
				{
					TickMirroredStrokes(dt, in s);
				}
			}
		}

		private void TickMirroredStrokes(float dt, in WiperSettings s)
		{
			if (_wiping)
			{
				_wipeElapsed += dt;
				float num = ((s.WipeDuration <= 0f) ? 1f : Mathf.Clamp01(_wipeElapsed / s.WipeDuration));
				float num2 = EaseInOut(num, s.WipePower);
				ApplyWipers(num2, (_wiperInvert == 1) ? (1f - num2) : num2, s.RefillMin, _wiperInvert);
				if (num >= 1f)
				{
					_dwellElapsed = 0f;
					_dwellTarget = Mathf.Max(0f, s.DwellDuration + UnityEngine.Random.Range(0f - s.DwellJitter, s.DwellJitter));
					_wiping = false;
					if (_wiperStopping && _wiperInvert == 1)
					{
						_wiperParked = true;
					}
				}
				return;
			}
			_dwellElapsed += dt;
			float t = ((s.RefillDuration <= 0f) ? 1f : Mathf.Clamp01(_dwellElapsed / s.RefillDuration));
			float overlay = Mathf.Lerp(s.RefillMin, 1f, EaseOut(t, s.RefillPower));
			ApplyWipers(1f, (_wiperInvert == 1) ? 0f : 1f, overlay, _wiperInvert);
			if (_dwellElapsed >= _dwellTarget)
			{
				_wiperInvert = ((_wiperInvert == 0) ? 1 : 0);
				_wipeElapsed = 0f;
				_wiping = true;
				ApplyWipers(0f, (_wiperInvert == 1) ? 1f : 0f, s.RefillMin, _wiperInvert);
				if (_wiperInvert == 0)
				{
					PlayWiperRaiseSound();
				}
			}
		}

		private void TickWipeAndReturn(float dt, in WiperSettings s)
		{
			if (_wipePhase == 0)
			{
				_wipeElapsed += dt;
				float num = ((s.WipeDuration <= 0f) ? 1f : Mathf.Clamp01(_wipeElapsed / s.WipeDuration));
				float num2 = EaseInOut(num, s.WipePower);
				ApplyWipers(num2, num2, s.RefillMin, 0);
				if (num >= 1f)
				{
					_wipePhase = 1;
					_wipeElapsed = 0f;
				}
			}
			else if (_wipePhase == 1)
			{
				_wipeElapsed += dt;
				float num3 = ((s.WipeDuration <= 0f) ? 1f : Mathf.Clamp01(_wipeElapsed / s.WipeDuration));
				ApplyWipers(1f, 1f - EaseInOut(num3, s.WipePower), s.RefillMin, 0);
				if (num3 >= 1f)
				{
					_wipePhase = 2;
					_dwellElapsed = 0f;
					_dwellTarget = Mathf.Max(0f, s.DwellDuration + UnityEngine.Random.Range(0f - s.DwellJitter, s.DwellJitter));
					if (_wiperStopping)
					{
						_wiperParked = true;
					}
				}
			}
			else
			{
				_dwellElapsed += dt;
				float t = ((s.RefillDuration <= 0f) ? 1f : Mathf.Clamp01(_dwellElapsed / s.RefillDuration));
				float overlay = Mathf.Lerp(s.RefillMin, 1f, EaseOut(t, s.RefillPower));
				ApplyWipers(1f, 0f, overlay, 0);
				if (_dwellElapsed >= _dwellTarget)
				{
					_wipePhase = 0;
					_wipeElapsed = 0f;
					ApplyWipers(0f, 0f, s.RefillMin, 0);
					PlayWiperRaiseSound();
				}
			}
		}

		public void SetGlassDoubleSided(bool doubleSided)
		{
			if (!WindowMat)
			{
				return;
			}
			float cull = (doubleSided ? 0f : 2f);
			if (cull != _lastCull)
			{
				_lastCull = cull;
				WindowMat.SetFloat(ShaderPropertyIDs.Cull, cull);
				ForEachFenderMat((Material m) =>
				{
					m.SetFloat(ShaderPropertyIDs.Cull, cull);
				});
			}
		}

		public void SetFirstPersonRainBlend(float blend)
		{
			if ((bool)WindowMat && !Mathf.Approximately(blend, _lastFPRainBlend))
			{
				_lastFPRainBlend = blend;
				WindowMat.SetFloat(ShaderPropertyIDs.FPRainBlend, blend);
				ForEachFenderMat((Material m) =>
				{
					m.SetFloat(ShaderPropertyIDs.FPRainBlend, blend);
				});
			}
		}

		public void StopWipers()
		{
			if (!_wipersActive)
			{
				return;
			}
			_wipersActive = false;
			if ((bool)WindowMat)
			{
				WindowMat.SetFloat(ShaderPropertyIDs.WipersValue, 0f);
				WindowMat.SetFloat(ShaderPropertyIDs.RainOverlayValue, 0f);
				WindowMat.SetFloat(ShaderPropertyIDs.InvertWipers, 0f);
			}
			for (int i = 0; i < _fenderWindows.Count; i++)
			{
				FenderWindow fenderWindow = _fenderWindows[i];
				if ((bool)fenderWindow.Refs)
				{
					PushFenderWindow(fenderWindow, 0f);
				}
			}
			PosePhysicalWipers(0f);
			_lastWipe = (_lastOverlay = float.NaN);
			_lastWiperInvert = -1;
			_hasPhysicalWiperValue = false;
			_wiperStopping = false;
			_wiperParked = false;
		}

		private void ResetWiperCycle(in WiperSettings s)
		{
			_wipeElapsed = 0f;
			_dwellElapsed = 0f;
			_dwellTarget = s.DwellDuration;
			_wiperInvert = 0;
			_wiping = true;
			_wipePhase = 0;
			_lastWipe = (_lastOverlay = float.NaN);
			_lastWiperInvert = -1;
			_hasPhysicalWiperValue = false;
			_wiperParked = false;
			ApplyWipers(0f, 0f, s.RefillMin, 0);
			PlayWiperRaiseSound();
		}

		private void PlayWiperRaiseSound()
		{
			if (HasPhysicalWipers)
			{
				GameObject gameObject = ResolveWiperAudioEmitter();
				if ((bool)gameObject)
				{
					AudioWrapper.PostEvent("Play_car_wipers", gameObject);
				}
			}
		}

		private GameObject ResolveWiperAudioEmitter()
		{
			if ((bool)_wiperAudioEmitter)
			{
				return _wiperAudioEmitter;
			}
			Transform transform = (_vehicle ? _vehicle.transform : null);
			if (!transform)
			{
				return null;
			}
			_wiperAudioEmitter = new GameObject("WiperAudioEmitter");
			Transform transform2 = _wiperAudioEmitter.transform;
			transform2.SetParent(transform, worldPositionStays: false);
			transform2.position = ResolveWiperEmitterPosition(transform);
			WiseHelper.SetAudioWiseComponents(_wiperAudioEmitter, addColliderTrigger: false);
			return _wiperAudioEmitter;
		}

		private Vector3 ResolveWiperEmitterPosition(Transform carRoot)
		{
			if (TryGetRightWiperPosition(carRoot, out var position))
			{
				return position;
			}
			return carRoot.position;
		}

		private bool TryGetRightWiperPosition(Transform carRoot, out Vector3 position)
		{
			position = Vector3.zero;
			EnsureWiperSources();
			Transform transform = null;
			float num = 0f;
			for (int i = 0; i < _wiperSources.Count; i++)
			{
				CarBodyReferences.WiperBlade[] array = _wiperSources[i]?.WiperBlades;
				if (array == null)
				{
					continue;
				}
				foreach (CarBodyReferences.WiperBlade wiperBlade in array)
				{
					if (wiperBlade != null && (bool)wiperBlade.Pivot)
					{
						float x = carRoot.InverseTransformPoint(wiperBlade.Pivot.position).x;
						if (transform == null || x > num)
						{
							num = x;
							transform = wiperBlade.Pivot;
						}
					}
				}
			}
			if (transform == null)
			{
				return false;
			}
			position = transform.position;
			return true;
		}

		private void ApplyWipers(float wipe, float physical, float overlay, int invert)
		{
			if (invert != _lastWiperInvert || Mathf.Abs(wipe - _lastWipe) >= 0.001f || Mathf.Abs(overlay - _lastOverlay) >= 0.001f)
			{
				_lastWipe = wipe;
				_lastOverlay = overlay;
				_lastWiperInvert = invert;
				float num = 0f;
				float num2 = 1f;
				if (_vehicle.CarBodyReferences != null)
				{
					Vector2 wiperShaderRange = _vehicle.CarBodyReferences.WiperShaderRange;
					num = wiperShaderRange.x;
					num2 = wiperShaderRange.y;
				}
				if (invert == 1)
				{
					float num3 = 1f - num2;
					num2 = 1f - num;
					num = num3;
				}
				WindowMat.SetFloat(ShaderPropertyIDs.WipersValue, Mathf.Lerp(num, num2, wipe));
				WindowMat.SetFloat(ShaderPropertyIDs.RainOverlayValue, overlay);
				WindowMat.SetFloat(ShaderPropertyIDs.InvertWipers, invert);
				for (int i = 0; i < _fenderWindows.Count; i++)
				{
					FenderWindow fenderWindow = _fenderWindows[i];
					if ((bool)fenderWindow.Refs && EnsureFenderMat(fenderWindow))
					{
						float num4 = fenderWindow.Range.x;
						float num5 = fenderWindow.Range.y;
						if (invert == 1)
						{
							float num6 = 1f - num5;
							num5 = 1f - num4;
							num4 = num6;
						}
						fenderWindow.Mat.SetFloat(ShaderPropertyIDs.WipersValue, Mathf.Lerp(num4, num5, wipe));
						fenderWindow.Mat.SetFloat(ShaderPropertyIDs.RainOverlayValue, overlay);
						fenderWindow.Mat.SetFloat(ShaderPropertyIDs.InvertWipers, invert);
					}
				}
			}
			if (!_hasPhysicalWiperValue || Mathf.Abs(physical - _lastPhysicalWiperValue) >= 0.0005f)
			{
				_lastPhysicalWiperValue = physical;
				_hasPhysicalWiperValue = true;
				PosePhysicalWipers(physical);
			}
		}

		private static float EaseInOut(float t, float p)
		{
			float num = Mathf.Pow(t, p);
			float num2 = Mathf.Pow(1f - t, p);
			return num / (num + num2);
		}

		private static float EaseOut(float t, float p)
		{
			return 1f - Mathf.Pow(1f - t, p);
		}

		public void DestroyMat()
		{
			if ((bool)_wiperAudioEmitter)
			{
				UnityEngine.Object.Destroy(_wiperAudioEmitter);
				_wiperAudioEmitter = null;
			}
			for (int i = 0; i < _fenderWindows.Count; i++)
			{
				if ((bool)_fenderWindows[i].Mat)
				{
					UnityEngine.Object.Destroy(_fenderWindows[i].Mat);
				}
			}
			_fenderWindows.Clear();
			if (_isWindowMatSpawned)
			{
				CommonExtensions.DestroyMat(ref WindowMat);
			}
		}

		public void DestroyRenderTexture()
		{
			CommonExtensions.DeleteRenderTex(WindowRT);
			WindowRT = null;
		}
	}
}
