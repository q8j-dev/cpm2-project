using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.CommonUtility.Contracts
{
	[Preserve]
	internal class PhotonCarValidatorGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhotonCarValidator photonCarValidator = new PhotonCarValidator();
			Inject(photonCarValidator, resolver, parameters);
			return photonCarValidator;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
