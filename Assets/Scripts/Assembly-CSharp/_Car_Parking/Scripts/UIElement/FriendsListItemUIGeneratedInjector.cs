using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class FriendsListItemUIGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:FriendsListItemUI cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FriendsListItemUI obj = (FriendsListItemUI)instance;
			PlayerDatas.FriendsUtility friendsUtility = (PlayerDatas.FriendsUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.FriendsUtility), "friendsUtility", parameters);
			obj.Construct(friendsUtility);
		}
	}
}
