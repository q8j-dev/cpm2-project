using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PhotoMode;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class CPMGraphicsSettingsManagerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CPMGraphicsSettingsManager cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CPMGraphicsSettingsManager obj = (CPMGraphicsSettingsManager)instance;
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		PhotoModeConfig config = (PhotoModeConfig)resolver.ResolveOrParameter(typeof(PhotoModeConfig), "config", parameters);
		obj.Construct(levelsProvider, config);
	}
}
