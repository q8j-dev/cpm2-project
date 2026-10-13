using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;

namespace _Car_Parking.Scripts.Notification
{
	[Preserve]
	internal class EquipmentNotificationServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EquipmentNotificationService equipmentNotificationService = new EquipmentNotificationService((PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters));
			Inject(equipmentNotificationService, resolver, parameters);
			return equipmentNotificationService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
