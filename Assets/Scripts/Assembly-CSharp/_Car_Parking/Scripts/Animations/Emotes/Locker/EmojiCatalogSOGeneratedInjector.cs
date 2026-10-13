using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	[Preserve]
	internal class EmojiCatalogSOGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EmojiCatalogSO emojiCatalogSO = new EmojiCatalogSO();
			Inject(emojiCatalogSO, resolver, parameters);
			return emojiCatalogSO;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
