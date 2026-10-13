using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Connection
{
	[Preserve]
	internal class TokenServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TokenService tokenService = new TokenService();
			Inject(tokenService, resolver, parameters);
			return tokenService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
