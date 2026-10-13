using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Multiplayer.Chat;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	[Preserve]
	internal class ActiveChatPlayerInfoGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ActiveChatPlayerInfo cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ActiveChatPlayerInfo obj = (ActiveChatPlayerInfo)instance;
			ChatControl chatControl = (ChatControl)resolver.ResolveOrParameter(typeof(ChatControl), "chatControl", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(chatControl, managerUI);
		}
	}
}
