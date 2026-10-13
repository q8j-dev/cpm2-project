using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Characters
{
	[Preserve]
	internal class CharactersSuitInventoryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CharactersSuitInventory charactersSuitInventory = new CharactersSuitInventory((CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters));
			Inject(charactersSuitInventory, resolver, parameters);
			return charactersSuitInventory;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
