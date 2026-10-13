using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.WwisePrefabs.WiseSoundScripts
{
	[Preserve]
	internal class MoneyChangedSoundGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:MoneyChangedSound cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
