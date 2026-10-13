using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.MenuUI
{
	[Preserve]
	internal class LockCarButtonReferenceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:LockCarButtonReference cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LockCarButtonReference obj = (LockCarButtonReference)instance;
			LockCarButtonController lockCarButtonController = (LockCarButtonController)resolver.ResolveOrParameter(typeof(LockCarButtonController), "lockCarButtonController", parameters);
			obj.Construct(lockCarButtonController);
		}
	}
}
