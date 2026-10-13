using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;

[Preserve]
internal class EmojiGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:Emoji cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		Emoji obj = (Emoji)instance;
		OnlineCollectionsProvider onlineCollectionsProvider = (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "onlineCollectionsProvider", parameters);
		obj.Construct(onlineCollectionsProvider);
	}
}
