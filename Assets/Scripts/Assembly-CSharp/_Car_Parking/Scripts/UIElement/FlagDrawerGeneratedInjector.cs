using System;
using System.Collections.Generic;
using Flags;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class FlagDrawerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FlagDrawer cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FlagDrawer obj = (FlagDrawer)instance;
			FlagsDataSO flagsDataSo = (FlagsDataSO)resolver.ResolveOrParameter(typeof(FlagsDataSO), "flagsDataSo", parameters);
			obj.Construct(flagsDataSo);
		}
	}
}
