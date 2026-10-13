using System;
using CodeBase.Infrastructure.States;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VLB;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Infrastructure.States;

namespace _Car_Parking.Scripts.Car.PoliceLights
{
	public class PoliceLightControl : MonoBehaviour
	{
		private static readonly uint Stop_Police_Sirens = AudioWrapper.GetId("Stop_Police_Siren_1");

		private static readonly uint Play_Police_Sirens = AudioWrapper.GetId("Play_Police_Siren_1");

		public Renderer lights;

		public Renderer redLightBloom;

		public Renderer blueLightBloom;

		public Material redLightMat;

		public Material blueLightMat;

		public Material redLightBloomMat;

		public Material blueLightBloomMat;

		public float alpha;

		private int tintId;

		private int instensityId;

		private float targetAlpha;

		private float duration = 5f;

		public float startTime;

		public VolumetricLightBeam redBeam;

		public VolumetricLightBeam blueBeam;

		private float speed1 = 5f;

		public float speed = 1f;

		public bool highQuality;

		private IDisposable _turnOff;

		private IGameStateMachine _stateMachine;

		private SoundController _soundController;

		private GameObject _car;

		public bool IsSirenEnabled { get; set; }

		private bool IsSoundControllerEnabled { get; set; }

		private bool IsCarEnabled { get; set; }

		private bool IsSirenSoundEnabled { get; set; }

		private bool IsLightsEnabled { get; set; }

		private bool IsSirenShouldBeEnabled { get; set; }

		private bool WasInitialized { get; set; }

		private void Awake()
		{
			_stateMachine = new SimpleStateMachine(new PoliceLightsStates.DisabledState(this), new PoliceLightsStates.EnabledLightsOnlyState(this), new PoliceLightsStates.EnabledState(this));
			_soundController = GetComponentInParent<SoundController>();
			if (!_soundController)
			{
				return;
			}
			_car = _soundController.gameObject;
			_soundController.OnDestroyAsObservable().Subscribe((Unit _) =>
			{
				_stateMachine.Enter<PoliceLightsStates.DisabledState>();
			}).AddTo(_car);
			_car.OnEnableAsObservable().Subscribe((Unit _) =>
			{
				IsCarEnabled = true;
				UpdateState(IsSirenShouldBeEnabled);
			}).AddTo(_car);
			_car.OnDisableAsObservable().Subscribe((Unit _) =>
			{
				IsCarEnabled = false;
				UpdateState(IsSirenShouldBeEnabled);
			}).AddTo(_car);
			Observable.EveryUpdate().Subscribe((long _) =>
			{
				if (IsLightsEnabled)
				{
					UpdateSirenWhenEnabled();
				}
				if (IsSoundControllerEnabled != _soundController.enabled)
				{
					IsSoundControllerEnabled = _soundController.enabled;
					UpdateState(IsSirenShouldBeEnabled);
				}
			}).AddTo(_car);
		}

		private void OnDestroy()
		{
			StopPoliceSiren();
			CommonExtensions.DestroyMat(ref redLightMat);
			CommonExtensions.DestroyMat(ref blueLightMat);
			CommonExtensions.DestroyMat(ref redLightBloomMat);
			CommonExtensions.DestroyMat(ref blueLightBloomMat);
			_stateMachine.Enter<PoliceLightsStates.DisabledState>();
			_stateMachine = null;
		}

		public void Initialize()
		{
			tintId = Shader.PropertyToID("_TintColor");
			instensityId = Shader.PropertyToID("_Intensity");
			redLightMat = new Material(lights.materials[1].shader);
			redLightMat.CopyPropertiesFromMaterial(lights.materials[1]);
			lights.materials[1] = redLightMat;
			blueLightMat = new Material(lights.materials[2].shader);
			blueLightMat.CopyPropertiesFromMaterial(lights.materials[2]);
			lights.materials[2] = blueLightMat;
			Material[] materials = new Material[3]
			{
				lights.materials[0],
				redLightMat,
				blueLightMat
			};
			lights.materials = materials;
			redLightBloomMat = new Material(redLightBloom.material.shader);
			redLightBloomMat.CopyPropertiesFromMaterial(redLightBloom.material);
			redLightBloom.material = redLightBloomMat;
			blueLightBloomMat = new Material(blueLightBloom.material.shader);
			blueLightBloomMat.CopyPropertiesFromMaterial(blueLightBloom.material);
			blueLightBloom.material = blueLightBloomMat;
			startTime = Time.time;
			speed = UnityEngine.Random.Range(0.2f, 0.3f);
			WasInitialized = true;
			_stateMachine.Enter<PoliceLightsStates.DisabledState>();
		}

		private bool IsEventPlayingOnGameObject(uint testEventId)
		{
			uint[] array = new uint[50];
			uint io_ruNumIDs = (uint)array.Length;
			AudioWrapper.GetPlayingIDsFromGameObject(base.gameObject, ref io_ruNumIDs, array);
			for (int i = 0; i < io_ruNumIDs; i++)
			{
				if (AudioWrapper.GetEventIDFromPlayingID(array[i]) == testEventId)
				{
					return true;
				}
			}
			return false;
		}

		public void StopPoliceSiren()
		{
			if (IsSirenSoundEnabled)
			{
				AudioWrapper.PostEvent(Stop_Police_Sirens, base.gameObject);
				IsSirenSoundEnabled = false;
			}
		}

		private void TryPlayPoliceSound()
		{
			if (!IsSirenSoundEnabled)
			{
				AudioWrapper.PostEvent(Play_Police_Sirens, base.gameObject);
				IsSirenSoundEnabled = true;
			}
		}

		public bool ToggleSirenByButton()
		{
			IsSirenShouldBeEnabled = !IsSirenShouldBeEnabled;
			return EnableDisableMyCar(IsSirenShouldBeEnabled);
		}

		private bool EnableDisableMyCar(bool enable)
		{
			if (enable)
			{
				_stateMachine.Enter<PoliceLightsStates.EnabledState>();
			}
			else
			{
				_stateMachine.Enter<PoliceLightsStates.DisabledState>();
			}
			return IsSirenEnabled;
		}

		public void UpdateStateSiren()
		{
			TurnOnOffLights(IsSirenEnabled);
		}

		public void TurnOnOffLights(bool enable)
		{
			IsSirenShouldBeEnabled = enable;
			UpdateState(enable);
		}

		private void UpdateState(bool enable)
		{
			if (!WasInitialized)
			{
				return;
			}
			if (enable)
			{
				if (IsCarEnabled)
				{
					if (IsSoundControllerEnabled)
					{
						_stateMachine.Enter<PoliceLightsStates.EnabledState>();
					}
					else
					{
						_stateMachine.Enter<PoliceLightsStates.EnabledLightsOnlyState>();
					}
				}
				else
				{
					_stateMachine.Enter<PoliceLightsStates.DisabledState>();
				}
			}
			else
			{
				_stateMachine.Enter<PoliceLightsStates.DisabledState>();
			}
		}

		public void ToggleSound(bool enable)
		{
			if (enable)
			{
				TryPlayPoliceSound();
			}
			else
			{
				StopPoliceSiren();
			}
		}

		public void TurnOnLightsShort()
		{
			_turnOff?.Dispose();
			EnableDisableMyCar(enable: true);
			_turnOff = Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
			{
				EnableDisableMyCar(enable: false);
			}).AddTo(this);
		}

		public void ToggleLights(bool enable)
		{
			IsLightsEnabled = enable;
			blueLightBloom.gameObject.SetActive(enable);
			redLightBloom.gameObject.SetActive(enable);
			if (!enable)
			{
				blueLightMat.SetFloat(instensityId, 0.1f);
				redLightMat.SetFloat(instensityId, 0.1f);
			}
			if (enable)
			{
				if (CPMGraphicsSettingsManager.Instance.Quality > 1)
				{
					blueLightBloom.gameObject.SetActive(value: false);
					redLightBloom.gameObject.SetActive(value: false);
					blueBeam.gameObject.SetActive(value: true);
					redBeam.gameObject.SetActive(value: true);
					highQuality = true;
				}
				else
				{
					blueLightBloom.gameObject.SetActive(value: true);
					redLightBloom.gameObject.SetActive(value: true);
					blueBeam.gameObject.SetActive(value: false);
					redBeam.gameObject.SetActive(value: false);
					highQuality = false;
				}
			}
			else
			{
				blueLightBloom.gameObject.SetActive(enable);
				redLightBloom.gameObject.SetActive(enable);
				blueBeam.gameObject.SetActive(enable);
				redBeam.gameObject.SetActive(enable);
			}
			_turnOff?.Dispose();
		}

		private void UpdateSirenWhenEnabled()
		{
			Color color = blueLightBloomMat.GetColor(tintId);
			Color color2 = redLightBloomMat.GetColor(tintId);
			color = blueBeam.color;
			color2 = redBeam.color;
			float t = (Time.time - startTime) / speed;
			alpha = Mathf.Lerp(alpha, targetAlpha, t);
			float num = 4f;
			if (!highQuality)
			{
				num = 0.4f;
			}
			if (alpha > num)
			{
				targetAlpha = 0f;
				startTime = Time.time;
			}
			else if (alpha < 0.01f)
			{
				targetAlpha = num + 0.1f;
				startTime = Time.time;
			}
			if (highQuality)
			{
				blueBeam.intensityGlobal = alpha;
				redBeam.intensityGlobal = 4f - alpha;
			}
			else
			{
				color.a = alpha;
				color2.a = 0.4f - alpha;
				blueLightBloomMat.SetColor(tintId, color);
				redLightBloomMat.SetColor(tintId, color2);
			}
			blueLightMat.SetFloat(instensityId, alpha);
			redLightMat.SetFloat(instensityId, num - alpha);
			redBeam.UpdateAfterManualPropertyChange();
			blueBeam.UpdateAfterManualPropertyChange();
		}
	}
}
