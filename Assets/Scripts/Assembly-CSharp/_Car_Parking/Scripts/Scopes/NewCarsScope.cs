using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Scopes
{
	public class NewCarsScope : LifetimeScope
	{
		[SerializeField]
		private DealerWindow DealerWindow = default;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(DealerWindow);
		}
	}
}
