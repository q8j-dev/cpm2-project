using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	[Preserve]
	internal class ChatExtraDataControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ChatExtraDataController chatExtraDataController = new ChatExtraDataController();
			Inject(chatExtraDataController, resolver, parameters);
			return chatExtraDataController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
