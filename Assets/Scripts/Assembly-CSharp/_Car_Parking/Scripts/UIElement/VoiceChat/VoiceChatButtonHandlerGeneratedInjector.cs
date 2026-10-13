using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.UIElement.HUD;

namespace _Car_Parking.Scripts.UIElement.VoiceChat
{
	[Preserve]
	internal class VoiceChatButtonHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:VoiceChatButtonHandler cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VoiceChatButtonHandler obj = (VoiceChatButtonHandler)instance;
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			HudColorChanger hudColorChanger = (HudColorChanger)resolver.ResolveOrParameter(typeof(HudColorChanger), "hudColorChanger", parameters);
			EventRegistry eventRegistry = (EventRegistry)resolver.ResolveOrParameter(typeof(EventRegistry), "eventRegistry", parameters);
			obj.Construct(menuLevelParent, hudColorChanger, eventRegistry);
		}
	}
}
