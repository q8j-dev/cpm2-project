using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	[Preserve]
	internal class ObstacleRaceSpectatorPanelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ObstacleRaceSpectatorPanel cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ObstacleRaceSpectatorPanel obj = (ObstacleRaceSpectatorPanel)instance;
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			InputMapSetEvent inputMapSetEvent = (InputMapSetEvent)resolver.ResolveOrParameter(typeof(InputMapSetEvent), "inputMapSetEvent", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			OptionalDependency<DesktopControlsSettingsSO> controlsSettingsDep = (OptionalDependency<DesktopControlsSettingsSO>)resolver.ResolveOrParameter(typeof(OptionalDependency<DesktopControlsSettingsSO>), "controlsSettingsDep", parameters);
			obj.Construct(eventRegistry, inputMapSetEvent, managerUI, controlsSettingsDep);
		}
	}
}
