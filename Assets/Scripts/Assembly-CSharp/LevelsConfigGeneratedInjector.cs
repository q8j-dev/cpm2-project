using System.Collections.Generic;
using VContainer;

[Preserve]
internal class LevelsConfigGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		LevelsConfig levelsConfig = new LevelsConfig();
		Inject(levelsConfig, resolver, parameters);
		return levelsConfig;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
