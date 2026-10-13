using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI.Person
{
	[Preserve]
	internal class AnimationShopOpenRequestGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AnimationShopOpenRequest animationShopOpenRequest = new AnimationShopOpenRequest();
			Inject(animationShopOpenRequest, resolver, parameters);
			return animationShopOpenRequest;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
