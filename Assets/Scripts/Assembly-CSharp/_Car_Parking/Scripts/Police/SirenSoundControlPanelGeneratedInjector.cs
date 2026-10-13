using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Police
{
	[Preserve]
	internal class SirenSoundControlPanelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SirenSoundControlPanel cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SirenSoundControlPanel obj = (SirenSoundControlPanel)instance;
			PoliceSystemConfig policeConfig = (PoliceSystemConfig)resolver.ResolveOrParameter(typeof(PoliceSystemConfig), "policeConfig", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			IPoliceSystem policeSystem = (IPoliceSystem)resolver.ResolveOrParameter(typeof(IPoliceSystem), "policeSystem", parameters);
			obj.Construct(policeConfig, managerUI, policeSystem);
		}
	}
}
