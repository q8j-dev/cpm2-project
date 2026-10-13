using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Free_drive
{
	[Preserve]
	internal class FreeDrivePlayerPropertyCarSitListenerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeDrivePlayerPropertyCarSitListener freeDrivePlayerPropertyCarSitListener = new FreeDrivePlayerPropertyCarSitListener();
			Inject(freeDrivePlayerPropertyCarSitListener, resolver, parameters);
			return freeDrivePlayerPropertyCarSitListener;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
