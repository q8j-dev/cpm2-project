using System;
using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.MenuUI.VisualTuning;

[Preserve]
internal class CalipersTuningPanelGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CalipersTuningPanel cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CalipersTuningPanel obj = (CalipersTuningPanel)instance;
		ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
		BrakeTuningConfig brakeTuningConfig = (BrakeTuningConfig)resolver.ResolveOrParameter(typeof(BrakeTuningConfig), "brakeTuningConfig", parameters);
		IVisualTuningViewModelComplexOne visualTuningViewModelComplexOne = (IVisualTuningViewModelComplexOne)resolver.ResolveOrParameter(typeof(IVisualTuningViewModelComplexOne), "visualTuningViewModelComplexOne", parameters);
		IWheelsDataService wheelsDataService = (IWheelsDataService)resolver.ResolveOrParameter(typeof(IWheelsDataService), "wheelsDataService", parameters);
		IVisualTuningViewModelComplexParams visualTuningViewModelComplexParams = (IVisualTuningViewModelComplexParams)resolver.ResolveOrParameter(typeof(IVisualTuningViewModelComplexParams), "visualTuningViewModelComplexParams", parameters);
		obj.Construct(carInfoService, brakeTuningConfig, visualTuningViewModelComplexOne, wheelsDataService, visualTuningViewModelComplexParams);
	}
}
