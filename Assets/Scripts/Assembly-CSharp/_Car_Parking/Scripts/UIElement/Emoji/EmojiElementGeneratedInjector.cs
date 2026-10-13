using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.UIElement.Emoji
{
	[Preserve]
	internal class EmojiElementGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EmojiElement cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EmojiElement obj = (EmojiElement)instance;
			MenuLevelParent menuLevelParent = (MenuLevelParent)resolver.ResolveOrParameter(typeof(MenuLevelParent), "menuLevelParent", parameters);
			obj.Construct(menuLevelParent);
		}
	}
}
