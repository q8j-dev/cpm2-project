using System.Collections.Generic;
using CPM.Scripts.Garage.GarageBuyer;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	[Preserve]
	internal class UpdateIconPresenterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			UpdateIconPresenter updateIconPresenter = new UpdateIconPresenter((SceneControl)resolver.ResolveOrParameter(typeof(SceneControl), "sceneControl", parameters), (FreeDriveControl)resolver.ResolveOrParameter(typeof(FreeDriveControl), "freeDriveControl", parameters), (MapIconsModel)resolver.ResolveOrParameter(typeof(MapIconsModel), "mapIconsModel", parameters), (MapIconsDataSo)resolver.ResolveOrParameter(typeof(MapIconsDataSo), "mapIconsDataSo", parameters), (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters));
			Inject(updateIconPresenter, resolver, parameters);
			return updateIconPresenter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
