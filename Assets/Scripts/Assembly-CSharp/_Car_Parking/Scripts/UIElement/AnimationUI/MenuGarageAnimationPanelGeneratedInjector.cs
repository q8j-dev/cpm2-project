using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Animations.Emotes.Locker;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.MenuUI.Person;

namespace _Car_Parking.Scripts.UIElement.AnimationUI
{
	[Preserve]
	internal class MenuGarageAnimationPanelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:MenuGarageAnimationPanel cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MenuGarageAnimationPanel obj = (MenuGarageAnimationPanel)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
			AnimationShopOpenRequest openRequest = (AnimationShopOpenRequest)resolver.ResolveOrParameter(typeof(AnimationShopOpenRequest), "openRequest", parameters);
			EmoteLockerOpenRequest lockerOpenRequest = (EmoteLockerOpenRequest)resolver.ResolveOrParameter(typeof(EmoteLockerOpenRequest), "lockerOpenRequest", parameters);
			obj.Construct(managerUI, coreGameManager, openRequest, lockerOpenRequest);
		}
	}
}
