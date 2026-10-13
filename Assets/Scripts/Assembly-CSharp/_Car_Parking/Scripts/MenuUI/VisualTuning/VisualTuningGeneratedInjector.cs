using System.Collections.Generic;
using CPM.Scripts.VinylDrawer;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class VisualTuningGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VisualTuning visualTuning = new VisualTuning();
			Inject(visualTuning, resolver, parameters);
			return visualTuning;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VisualTuning obj = (VisualTuning)instance;
			WheelsTuningScope wheelsTuningScope = (WheelsTuningScope)resolver.ResolveOrParameter(typeof(WheelsTuningScope), "wheelsTuningScope", parameters);
			MainScope mainScope = (MainScope)resolver.ResolveOrParameter(typeof(MainScope), "mainScope", parameters);
			ColorsScope colorsScope = (ColorsScope)resolver.ResolveOrParameter(typeof(ColorsScope), "colorsScope", parameters);
			FlagsVisualScope flagsVisualScope = (FlagsVisualScope)resolver.ResolveOrParameter(typeof(FlagsVisualScope), "flagsVisualScope", parameters);
			BodyKitsVisualScope bodyKitsVisualScope = (BodyKitsVisualScope)resolver.ResolveOrParameter(typeof(BodyKitsVisualScope), "bodyKitsVisualScope", parameters);
			VinylDrawer vinylDrawer = (VinylDrawer)resolver.ResolveOrParameter(typeof(VinylDrawer), "vinylDrawer", parameters);
			PlayerDatas.VinylsDataUtility vinylsDataUtility = (PlayerDatas.VinylsDataUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.VinylsDataUtility), "vinylsDataUtility", parameters);
			BrakesVisualScope brakesVisualScope = (BrakesVisualScope)resolver.ResolveOrParameter(typeof(BrakesVisualScope), "brakesVisualScope", parameters);
			CalipersVisualTuningScope calipersVisualTuningScope = (CalipersVisualTuningScope)resolver.ResolveOrParameter(typeof(CalipersVisualTuningScope), "calipersVisualTuningScope", parameters);
			PoliceVisualScope policeVisualScope = (PoliceVisualScope)resolver.ResolveOrParameter(typeof(PoliceVisualScope), "policeVisualScope", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			obj.Construct(wheelsTuningScope, mainScope, colorsScope, flagsVisualScope, bodyKitsVisualScope, vinylDrawer, vinylsDataUtility, brakesVisualScope, calipersVisualTuningScope, policeVisualScope, freeDriveProvider);
		}
	}
}
