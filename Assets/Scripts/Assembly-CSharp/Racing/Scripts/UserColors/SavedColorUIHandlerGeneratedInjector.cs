using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace Racing.Scripts.UserColors
{
	[Preserve]
	internal class SavedColorUIHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SavedColorUIHandler cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SavedColorUIHandler obj = (SavedColorUIHandler)instance;
			SavedColorCollection savedColorCollection = (SavedColorCollection)resolver.ResolveOrParameter(typeof(SavedColorCollection), "savedColorCollection", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			obj.Construct(savedColorCollection, prefabFactory);
		}
	}
}
