using System.Collections.Generic;
using VContainer;

namespace MVPFree.MapIcons.ScrObjs.Scripts
{
	[Preserve]
	internal class TimeAttackDataSOGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TimeAttackDataSO timeAttackDataSO = new TimeAttackDataSO();
			Inject(timeAttackDataSO, resolver, parameters);
			return timeAttackDataSO;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
