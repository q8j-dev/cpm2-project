using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VContainer;

public sealed class FinishFlashController : MonoBehaviour
{
	private static readonly int SID_Intensity = Shader.PropertyToID("_FinishFlash_Intensity");

	[SerializeField]
	private FinishFlashFeature featureInRenderer = default;

	[SerializeField]
	private Volume volume;

	private Camera _camera;

	[Min(0f)]
	public float defaultRise = 0.015f;

	[Min(0f)]
	public float defaultHold = 0.01f;

	[Min(0f)]
	public float defaultFall = 0.4f;

	[Range(0f, 1.5f)]
	public float defaultPeak = 1f;

	private float _t0;

	private float _rise;

	private float _hold;

	private float _fall;

	private float _peak;

	private bool _active;

	private Bloom _bloom;

	private float _bloomCached;

	private bool _bloomOverridden;

	[Inject]
	public void Construct(Camera camera)
	{
		_camera = camera;
	}

	private static float Smooth01(float x)
	{
		x = Mathf.Clamp01(x);
		return x * x * (3f - 2f * x);
	}

	private void Start()
	{
		volume = _camera.GetComponent<Volume>();
		if (featureInRenderer != null)
		{
			featureInRenderer.ApplyStaticUniforms();
		}
		Shader.DisableKeyword("_FINISH_FLASH_ON");
		Shader.SetGlobalFloat(SID_Intensity, 0f);
		ResolveBloom();
		ClearBloomOverride();
	}

	private void OnDisable()
	{
		Shader.DisableKeyword("_FINISH_FLASH_ON");
		Shader.SetGlobalFloat(SID_Intensity, 0f);
		RestoreBloomIfNeeded();
	}

	private void LateUpdate()
	{
		if (!_active)
		{
			RestoreBloomIfNeeded();
			return;
		}
		float num = Time.unscaledTime - _t0;
		float num2;
		if (num <= _rise)
		{
			float x = ((_rise > 1E-05f) ? (num / _rise) : 1f);
			num2 = _peak * Smooth01(x);
		}
		else if (num <= _rise + _hold)
		{
			num2 = _peak;
		}
		else if (num <= _rise + _hold + _fall)
		{
			float x2 = (num - _rise - _hold) / Mathf.Max(_fall, 1E-05f);
			num2 = _peak * (1f - Smooth01(x2));
		}
		else
		{
			num2 = 0f;
			_active = false;
		}
		Shader.SetGlobalFloat(SID_Intensity, num2);
		if (num2 > 0f)
		{
			Shader.EnableKeyword("_FINISH_FLASH_ON");
			ApplyBloomByIntensity(num2);
		}
		else
		{
			Shader.DisableKeyword("_FINISH_FLASH_ON");
			RestoreBloomIfNeeded();
		}
	}

	public void TriggerFinishFlash()
	{
		Trigger(defaultPeak, defaultRise, defaultHold, defaultFall);
	}

	public void Trigger(float peak, float rise, float hold, float fall)
	{
		_peak = Mathf.Max(0f, peak);
		_rise = Mathf.Max(0f, rise);
		_hold = Mathf.Max(0f, hold);
		_fall = Mathf.Max(0f, fall);
		_t0 = Time.unscaledTime;
		if (_peak > 0f && _rise + _hold + _fall > 0f)
		{
			_active = true;
			Shader.EnableKeyword("_FINISH_FLASH_ON");
			ApplyBloomOverride();
			Shader.SetGlobalFloat(SID_Intensity, 0f);
		}
	}

	private void ResolveBloom()
	{
		_bloom = null;
		if (!(volume == null))
		{
			VolumeProfile volumeProfile = (volume.HasInstantiatedProfile() ? volume.profile : volume.sharedProfile);
			if (volumeProfile != null)
			{
				volumeProfile.TryGet<Bloom>(out _bloom);
			}
		}
	}

	private void ApplyBloomOverride()
	{
		if (!_bloomOverridden)
		{
			if (_bloom == null)
			{
				ResolveBloom();
			}
			if (!(_bloom == null))
			{
				_bloomCached = _bloom.intensity.value;
				_bloomOverridden = true;
			}
		}
	}

	private void ApplyBloomByIntensity(float flashIntensity01)
	{
		if (!_bloomOverridden)
		{
			return;
		}
		if (_bloom == null)
		{
			ResolveBloom();
		}
		if (!(_bloom == null))
		{
			float num = Mathf.Clamp01(flashIntensity01);
			if (num <= 0f)
			{
				RestoreBloomIfNeeded();
				return;
			}
			float b = ((featureInRenderer != null) ? featureInRenderer.BloomAtPeak : 2f);
			_bloom.intensity.value = Mathf.Lerp(_bloomCached, b, num);
		}
	}

	private void RestoreBloomIfNeeded()
	{
		if (_bloomOverridden)
		{
			if (_bloom == null)
			{
				ResolveBloom();
			}
			if (_bloom != null)
			{
				_bloom.intensity.value = _bloomCached;
			}
			_bloomOverridden = false;
		}
	}

	private void ClearBloomOverride()
	{
		_bloomOverridden = false;
	}
}
