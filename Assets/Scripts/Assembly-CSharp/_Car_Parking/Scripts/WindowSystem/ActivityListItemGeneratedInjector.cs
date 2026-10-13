using System;
using System.Collections.Generic;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class ActivityListItemGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ActivityListItem cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ActivityListItem obj = (ActivityListItem)instance;
			MapIconsDataSo icons = (MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "icons", parameters);
			obj.Construct(icons);
		}
	}
}
