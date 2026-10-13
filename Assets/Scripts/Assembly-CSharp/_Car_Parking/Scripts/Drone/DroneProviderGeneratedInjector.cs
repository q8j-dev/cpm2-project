using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.Chat;

namespace _Car_Parking.Scripts.Drone
{
	[Preserve]
	internal class DroneProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DroneProvider droneProvider = new DroneProvider();
			Inject(droneProvider, resolver, parameters);
			return droneProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DroneProvider obj = (DroneProvider)instance;
			ChatControl chatControl = (ChatControl)resolver.ResolveOrParameter(typeof(ChatControl), "chatControl", parameters);
			obj.Construct(chatControl);
		}
	}
}
