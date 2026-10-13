using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Steam.Graphics
{
	[Preserve]
	internal class FpvShadowCascadeServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FpvShadowCascadeService fpvShadowCascadeService = new FpvShadowCascadeService();
			Inject(fpvShadowCascadeService, resolver, parameters);
			return fpvShadowCascadeService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
