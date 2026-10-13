using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.UIElement.CircularMenu;

namespace _Car_Parking.Scripts.Scopes
{
	public class FreeDrivePartScope : LifetimeScope
	{
		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterBuildCallback((IObjectResolver container) =>
			{
				GameObject[] rootGameObjects = base.gameObject.scene.GetRootGameObjects();
				foreach (GameObject gameObject in rootGameObjects)
				{
					FreeDrivePrefabSpawner[] componentsInChildren = gameObject.GetComponentsInChildren<FreeDrivePrefabSpawner>(includeInactive: true);
					foreach (FreeDrivePrefabSpawner instance in componentsInChildren)
					{
						container.Inject(instance);
					}
					CircularMenuSpawner[] componentsInChildren2 = gameObject.GetComponentsInChildren<CircularMenuSpawner>(includeInactive: true);
					foreach (CircularMenuSpawner instance2 in componentsInChildren2)
					{
						container.Inject(instance2);
					}
					SnowParticlesMover[] componentsInChildren3 = gameObject.GetComponentsInChildren<SnowParticlesMover>(includeInactive: true);
					foreach (SnowParticlesMover instance3 in componentsInChildren3)
					{
						container.Inject(instance3);
					}
				}
			});
		}
	}
}
