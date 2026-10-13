using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class PhotoModeObserverGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhotoModeObserver photoModeObserver = new PhotoModeObserver();
			Inject(photoModeObserver, resolver, parameters);
			return photoModeObserver;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
