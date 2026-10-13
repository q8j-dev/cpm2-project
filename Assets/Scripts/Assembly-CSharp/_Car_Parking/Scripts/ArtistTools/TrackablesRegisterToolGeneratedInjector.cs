using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.ArtistTools
{
	[Preserve]
	internal class TrackablesRegisterToolGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:TrackablesRegisterTool cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TrackablesRegisterTool obj = (TrackablesRegisterTool)instance;
			GlobalCollectionsProvider provider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "provider", parameters);
			obj.Construct(provider);
		}
	}
}
