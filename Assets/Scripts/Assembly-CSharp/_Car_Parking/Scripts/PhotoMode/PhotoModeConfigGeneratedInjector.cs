using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.PhotoMode
{
	[Preserve]
	internal class PhotoModeConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhotoModeConfig photoModeConfig = new PhotoModeConfig();
			Inject(photoModeConfig, resolver, parameters);
			return photoModeConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
