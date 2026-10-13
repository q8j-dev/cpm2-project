using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HDHeadlighsController : MonoBehaviour
{
	public List<Material> CarLightsMaterials;

	private Dictionary<string, Material> _shaderNameToCarMat;

	private Material[] carmats;

	private List<CarLightGroup> CarLights = new List<CarLightGroup>();

	[Tooltip("Daytime Running Lights")]
	public float DRLSpeed = 0.3f;

	[Tooltip("Low Beam Lights")]
	public float LBLSpeed = 0.3f;

	[Tooltip("High Beam Lights")]
	public float HBLSpeed = 0.3f;

	[Tooltip("Position Tail Lights")]
	public float PTLSpeed = 0.3f;

	[Tooltip("Brake Lights")]
	public float BLSpeed = 0.3f;

	[Tooltip("Reverse Light")]
	public float RLSpeed = 0.3f;

	[Tooltip("Turn Signals")]
	public float TurnSignalSpeed = 0.4f;

	public bool UseTurnSignalAnimationCurve;

	public bool TurnSignalRestart;

	public AnimationCurve TurnSignalCurve;

	[Tooltip("Do Turn signals turn sequentially?")]
	public bool SequentialTurnSignals;

	[Tooltip("Time offset for sequential turn signals")]
	public float SequentialTimeOffset;

	[Tooltip("Do turn signals turn off tail lights?")]
	public bool TurnSignalsTurnOffTailLights;

	[Tooltip("Do turn signals turn off brake lights?")]
	public bool TurnSigbalsTurnOffBrakeLights;

	[Tooltip("Do Daytime Running Lights turn on Position Tail Lights?")]
	public bool DTLTurnOnTailLights;

	public bool DebugButtons;

	public bool isInitialized;

	private HDHeadLightFlare[] Flares;

	public void Init()
	{
		if (!isInitialized)
		{
			BuildProperties();
			InitializeMaterial();
			ResetLights();
			SetupRenderers();
			isInitialized = true;
		}
	}

	private void BuildProperties()
	{
		CarLights = new List<CarLightGroup>();
		CarLights.Add(BuildCarLightPropertires(new string[1] { "_DRL_Power" }, DRLSpeed));
		CarLights.Add(BuildCarLightPropertires(new string[1] { "_LBL_Power" }, LBLSpeed));
		CarLights.Add(BuildCarLightPropertires(new string[1] { "_HBL_Power" }, HBLSpeed));
		CarLights.Add(BuildCarLightPropertires(new string[4] { "_LTL_Power_0", "_LTL_Power_1", "_LTL_Power_2", "_LTL_Power_3" }, TurnSignalSpeed, repeat: true, SequentialTurnSignals, SequentialTimeOffset));
		CarLights.Add(BuildCarLightPropertires(new string[4] { "_RTL_Power_0", "_RTL_Power_1", "_RTL_Power_2", "_RTL_Power_3" }, TurnSignalSpeed, repeat: true, SequentialTurnSignals, SequentialTimeOffset));
		CarLights.Add(BuildCarLightPropertires(new string[3] { "_PTL_Power_0", "_PTL_Power_1", "_PTL_Power_2" }, PTLSpeed));
		CarLights.Add(BuildCarLightPropertires(new string[3] { "_BL_Power_0", "_BL_Power_1", "_BL_Power_1" }, BLSpeed));
		CarLights.Add(BuildCarLightPropertires(new string[1] { "_RL_Power" }, RLSpeed));
	}

	private CarLightGroup BuildCarLightPropertires(string[] properties, float duration, bool repeat = false, bool seq = false, float seqOffset = 0f)
	{
		CarLightGroup carLightGroup = new CarLightGroup();
		carLightGroup.Values = new List<CarLightValue>();
		for (int i = 0; i < properties.Length; i++)
		{
			CarLightValue carLightValue = new CarLightValue();
			carLightValue.LightValue = 0f;
			carLightValue.LightProperty = Shader.PropertyToID(properties[i]);
			carLightValue.Duration = duration;
			carLightValue.Repeat = repeat;
			carLightValue.Delay = (seq ? Mathf.Max(0f, ((float)i - 1f) * seqOffset) : 0f);
			carLightGroup.Values.Add(carLightValue);
		}
		return carLightGroup;
	}

	private void InitializeMaterial()
	{
		List<Shader> list = new List<Shader>();
		carmats = new Material[CarLightsMaterials.Count];
		for (int i = 0; i < CarLightsMaterials.Count; i++)
		{
			list.Add(CarLightsMaterials[i].shader);
			carmats[i] = Object.Instantiate(CarLightsMaterials[i]);
		}
		_shaderNameToCarMat = new Dictionary<string, Material>();
		for (int j = 0; j < list.Count && j < carmats.Length; j++)
		{
			_shaderNameToCarMat[list[j].name] = carmats[j];
		}
	}

	public void SetupRenderers()
	{
		if (_shaderNameToCarMat == null)
		{
			return;
		}
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			Material[] sharedMaterials = meshRenderer.sharedMaterials;
			for (int j = 0; j < sharedMaterials.Length; j++)
			{
				if ((bool)sharedMaterials[j] && _shaderNameToCarMat.TryGetValue(sharedMaterials[j].shader.name, out var value))
				{
					sharedMaterials[j] = value;
				}
			}
			meshRenderer.sharedMaterials = sharedMaterials;
		}
		GetFlares();
	}

	private void GetFlares()
	{
		Flares = GetComponentsInChildren<HDHeadLightFlare>();
	}

	private void NotifyFlares(HDHeadLightFlareType type, bool isOn, float time)
	{
		for (int i = 0; i < Flares.Length; i++)
		{
			if (Flares[i].FlareType == type)
			{
				Flares[i].Switch(isOn, time);
			}
		}
	}

	private void ResetLights()
	{
		foreach (CarLightGroup carLight in CarLights)
		{
			foreach (CarLightValue value in carLight.Values)
			{
				SetMats(value.LightProperty, value.LightValue);
			}
		}
	}

	public void SwitchLeftLights()
	{
		if (GetHazardLights())
		{
			SwitchRightTurnSignal(!GetRightTurnSignal());
		}
		else
		{
			SwitchLeftTurnSignal(!GetLeftTurnSignal());
		}
	}

	public void SwitchRightLights()
	{
		if (GetHazardLights())
		{
			SwitchLeftTurnSignal(!GetLeftTurnSignal());
		}
		else
		{
			SwitchRightTurnSignal(!GetRightTurnSignal());
		}
	}

	private void KillAndResetLightGroup(CarLightGroup lights)
	{
		if (lights == null)
		{
			return;
		}
		lights.isOn = false;
		if (lights.Values == null)
		{
			return;
		}
		foreach (CarLightValue value in lights.Values)
		{
			value.isOn = false;
			if (value.mainTweener != null)
			{
				value.mainTweener.Kill();
			}
			value.LightValue = 0f;
			SetMats(value.LightProperty, value.LightValue);
		}
	}

	private void SwitchLightGroup(CarLightGroup lights, bool isOn)
	{
		if (lights == null || isOn == lights.isOn)
		{
			return;
		}
		lights.isOn = isOn;
		if (lights.Values == null)
		{
			return;
		}
		foreach (CarLightValue value in lights.Values)
		{
			SwitchLight(value, isOn);
		}
	}

	private void SwitchLight(CarLightValue value, bool isOn)
	{
		value.isOn = isOn;
		float num = (isOn ? 1f : 0f);
		if (value.mainTweener != null)
		{
			value.mainTweener.Kill();
		}
		value.LightValue = ((isOn && value.Repeat) ? 0f : value.LightValue);
		if (value.Repeat && UseTurnSignalAnimationCurve)
		{
			value.mainTweener = DOTween.To(() => value.LightValue, (float x) =>
			{
				value.LightValue = x;
			}, num, Mathf.Abs(num - value.LightValue) * value.Duration).SetEase(TurnSignalCurve).SetLoops(value.isOn ? (value.Repeat ? (-1) : 0) : 0, (!TurnSignalRestart) ? LoopType.Yoyo : LoopType.Restart)
				.SetDelay(value.Delay)
				.OnUpdate(() =>
				{
					SetMats(value.LightProperty, value.LightValue);
				})
				.OnComplete(() =>
				{
					SetMats(value.LightProperty, value.LightValue);
				})
				.SetLink(base.gameObject);
		}
		else
		{
			value.mainTweener = DOTween.To(() => value.LightValue, (float x) =>
			{
				value.LightValue = x;
			}, num, Mathf.Abs(num - value.LightValue) * value.Duration).SetEase(Ease.InOutQuad).SetLoops(value.isOn ? (value.Repeat ? (-1) : 0) : 0, LoopType.Yoyo)
				.SetDelay(value.Delay)
				.OnUpdate(() =>
				{
					SetMats(value.LightProperty, value.LightValue);
				})
				.OnComplete(() =>
				{
					SetMats(value.LightProperty, value.LightValue);
				})
				.SetLink(base.gameObject);
		}
	}

	private void SetMats(int id, float value)
	{
		Material[] array = carmats;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFloat(id, value);
		}
	}

	public bool GetDaytimeRunningLights()
	{
		List<CarLightGroup> carLights = CarLights;
		if (carLights != null && carLights.Count > 0 && CarLights[0] != null)
		{
			return CarLights[0].isOn;
		}
		return false;
	}

	public void SwitchDaytimeRunningLights(bool isOn)
	{
		SwitchLightGroup(GetCarLightGroup(0), isOn);
		if (DTLTurnOnTailLights && (isOn || (!GetLowBeamLights() && !GetHighBeamLights())))
		{
			SwicthPositionTailLights(isOn);
		}
	}

	private bool GetLowBeamLights()
	{
		List<CarLightGroup> carLights = CarLights;
		if (carLights != null && carLights.Count > 1 && CarLights[1] != null)
		{
			return CarLights[1].isOn;
		}
		return false;
	}

	public void SwitchLowBeamLights(bool isOn)
	{
		SwitchLightGroup(GetCarLightGroup(1), isOn);
		NotifyFlares(HDHeadLightFlareType.LowBeam, isOn, LBLSpeed);
		if (isOn || ((!DTLTurnOnTailLights || !GetDaytimeRunningLights()) && !GetHighBeamLights() && GetPositionTailLights()))
		{
			SwicthPositionTailLights(isOn);
		}
	}

	private bool GetHighBeamLights()
	{
		List<CarLightGroup> carLights = CarLights;
		if (carLights != null && carLights.Count > 2 && CarLights[2] != null)
		{
			return CarLights[2].isOn;
		}
		return false;
	}

	public void SwitchHightBeamLights(bool isOn)
	{
		SwitchLightGroup(GetCarLightGroup(2), isOn);
		NotifyFlares(HDHeadLightFlareType.HighBeam, isOn, HBLSpeed);
		if (isOn || ((!DTLTurnOnTailLights || !GetDaytimeRunningLights()) && !GetLowBeamLights() && GetPositionTailLights()))
		{
			SwicthPositionTailLights(isOn);
		}
	}

	private bool GetLeftTurnSignal()
	{
		return GetCarLightGroup(3)?.isOn ?? false;
	}

	public void SwitchLeftTurnSignal(bool isOn)
	{
		SwitchLightGroup(GetCarLightGroup(3), isOn);
		if (isOn)
		{
			SwitchLightGroup(GetCarLightGroup(4), isOn: false);
		}
	}

	private bool GetRightTurnSignal()
	{
		List<CarLightGroup> carLights = CarLights;
		if (carLights != null && carLights.Count > 4 && CarLights[4] != null)
		{
			return CarLights[4].isOn;
		}
		return false;
	}

	public void SwitchRightTurnSignal(bool isOn)
	{
		SwitchLightGroup(GetCarLightGroup(4), isOn);
		if (isOn)
		{
			SwitchLightGroup(GetCarLightGroup(3), isOn: false);
		}
	}

	public bool GetHazardLights()
	{
		List<CarLightGroup> carLights = CarLights;
		if (carLights != null && carLights.Count > 4 && CarLights[3] != null && CarLights[4] != null && CarLights[3].isOn)
		{
			return CarLights[4].isOn;
		}
		return false;
	}

	public void SwitchHazardLights(bool isOn)
	{
		if (isOn)
		{
			KillAndResetLightGroup(CarLights[3]);
			KillAndResetLightGroup(CarLights[4]);
		}
		SwitchLightGroup(CarLights[3], isOn);
		SwitchLightGroup(CarLights[4], isOn);
	}

	public bool GetBrakeLights()
	{
		List<CarLightGroup> carLights = CarLights;
		if (carLights != null && carLights.Count > 6 && CarLights[6] != null)
		{
			return CarLights[6].isOn;
		}
		return false;
	}

	public void SwitchBrakeLights(bool isOn)
	{
		SwitchLightGroup(CarLights[6], isOn);
	}

	private bool GetReverseLight()
	{
		List<CarLightGroup> carLights = CarLights;
		if (carLights != null && carLights.Count > 7 && CarLights[7] != null)
		{
			return CarLights[7].isOn;
		}
		return false;
	}

	public void SwitchReverseLight(bool isOn)
	{
		SwitchLightGroup(GetCarLightGroup(7), isOn);
	}

	private bool GetPositionTailLights()
	{
		List<CarLightGroup> carLights = CarLights;
		if (carLights != null && carLights.Count > 5 && CarLights[5] != null)
		{
			return CarLights[5].isOn;
		}
		return false;
	}

	private void SwicthPositionTailLights(bool isOn)
	{
		SwitchLightGroup(GetCarLightGroup(5), isOn);
	}

	private CarLightGroup GetCarLightGroup(int index)
	{
		if (CarLights == null)
		{
			return null;
		}
		if (index < 0 || index >= CarLights.Count)
		{
			return null;
		}
		return CarLights[index];
	}

	private void OnDestroy()
	{
		if (carmats != null)
		{
			for (int i = 0; i < carmats.Length; i++)
			{
				CommonExtensions.DestroyMat(ref carmats[i]);
			}
			carmats = null;
		}
	}
}
