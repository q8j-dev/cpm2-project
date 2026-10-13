using System.Collections.Generic;
using VContainer;

namespace Persistence.Systems
{
	[Preserve]
	internal class ShadersVaultConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ShadersVaultConfig shadersVaultConfig = new ShadersVaultConfig();
			Inject(shadersVaultConfig, resolver, parameters);
			return shadersVaultConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
