using System.Collections.Generic;
using Flags;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class FlagsVisualScopeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FlagsVisualScope flagsVisualScope = new FlagsVisualScope();
			Inject(flagsVisualScope, resolver, parameters);
			return flagsVisualScope;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FlagsVisualScope obj = (FlagsVisualScope)instance;
			CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
			FlagsDataSO flagsDataSo = (FlagsDataSO)resolver.ResolveOrParameter(typeof(FlagsDataSO), "flagsDataSo", parameters);
			CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
			MenuControl menuControl = (MenuControl)resolver.ResolveOrParameter(typeof(MenuControl), "menuControl", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(currencyManagerController, flagsDataSo, cameraManager, menuControl, managerUI);
		}
	}
}
