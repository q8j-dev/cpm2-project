using Service;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Scopes
{
	public class FreeDriveConfigScope : LifetimeScope
	{
		[SerializeField]
		private NamesScriptables namesScriptables = default;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterInstance(namesScriptables);
		}
	}
}
