using System.Collections.Generic;
using VContainer;

[Preserve]
internal class PersonKitConfigsGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		PersonKitConfigs personKitConfigs = new PersonKitConfigs((PersonKitConfig)resolver.ResolveOrParameter(typeof(PersonKitConfig), "male", parameters), (PersonKitConfig)resolver.ResolveOrParameter(typeof(PersonKitConfig), "female", parameters));
		Inject(personKitConfigs, resolver, parameters);
		return personKitConfigs;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
	}
}
