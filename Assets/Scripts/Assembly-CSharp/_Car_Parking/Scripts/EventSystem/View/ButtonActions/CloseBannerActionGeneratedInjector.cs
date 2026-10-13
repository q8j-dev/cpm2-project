using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.EventSystem.View.ButtonActions
{
	[Preserve]
	internal class CloseBannerActionGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CloseBannerAction closeBannerAction = new CloseBannerAction();
			Inject(closeBannerAction, resolver, parameters);
			return closeBannerAction;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
