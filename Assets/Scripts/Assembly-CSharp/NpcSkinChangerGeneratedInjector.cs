using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;

[Preserve]
internal class NpcSkinChangerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:NpcSkinChanger cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		NpcSkinChanger obj = (NpcSkinChanger)instance;
		CharactersService charactersService = (CharactersService)resolver.ResolveOrParameter(typeof(CharactersService), "charactersService", parameters);
		obj.Construct(charactersService);
	}
}
