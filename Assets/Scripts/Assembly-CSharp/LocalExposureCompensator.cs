using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SunAndSkyController))]
public class LocalExposureCompensator : MonoBehaviour
{
	private SunAndSkyController m_SunAndSky;

	[SerializeField]
	public List<ExposureElement> ExposureElements = new List<ExposureElement>();

	private float exposureVal;

	private CPMGraphicsSettingsManager manager;

	private float PrevHorizon;

	private void Start()
	{
		m_SunAndSky = GetComponent<SunAndSkyController>();
	}

	private void Update()
	{
		if (Mathf.Abs(m_SunAndSky.HorizonAngle - PrevHorizon) < 0.02f)
		{
			return;
		}
		PrevHorizon = m_SunAndSky.HorizonAngle;
		if (!manager)
		{
			manager = CPMGraphicsSettingsManager.Instance;
		}
		if (manager == null || manager._graphicsSettings == null)
		{
			return;
		}
		foreach (ExposureElement exposureElement in ExposureElements)
		{
			UpdateExposure(exposureElement.m_exposureProfile, (exposureElement.m_exposureCurve != null) ? exposureElement.m_exposureCurve.Evaluate(Mathf.Clamp01(m_SunAndSky.HorizonAngle * 0.5f + 0.5f)) : 1f);
		}
	}

	private void UpdateExposure(VolumeProfile volprofile, float exposureVal)
	{
		if ((bool)volprofile && volprofile.TryGet<ColorAdjustments>(out var component))
		{
			component.active = manager._graphicsSettings.m_Tonemapping != TonemappingMode.None;
			component.postExposure.value = exposureVal + manager.AcesAdjustment;
		}
	}
}
