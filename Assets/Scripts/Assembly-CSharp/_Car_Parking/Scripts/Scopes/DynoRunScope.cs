using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Scopes
{
	public class DynoRunScope : LifetimeScope
	{
		[SerializeField]
		private DynoRunController DynoRunController;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(DynoRunController);
		}
	}
}
