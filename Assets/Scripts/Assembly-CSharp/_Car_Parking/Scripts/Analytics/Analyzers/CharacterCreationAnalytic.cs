using System.Text;
using CPM.Database;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class CharacterCreationAnalytic : AnalyticProviderBase
	{
		public CharacterCreationAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogCharacterCreation(CharacterCreationType characterCreationType)
		{
			ICharacterEquipmentDataService characterEquipmentDataService = AnalyticsDependencies.Get<ICharacterEquipmentDataService>();
			if (characterEquipmentDataService != null)
			{
				base.AnalyticEventFactory.CreateEvent<CharacterCreationEvent, CharacterCreationContext>(new CharacterCreationContext
				{
					Status = characterCreationType.ToString(),
					PresetId = GetPreset(characterEquipmentDataService)
				}).Send();
			}
		}

		private string GetPreset(ICharacterEquipmentDataService characterEquipmentDataService)
		{
			int gender = characterEquipmentDataService.Gender;
			EquipmentsData equipmentsData = ((gender == 1) ? characterEquipmentDataService.Male : characterEquipmentDataService.Female);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(gender);
			if (equipmentsData.SelectedEquipments == null)
			{
				return stringBuilder.ToString();
			}
			foreach (int selectedEquipment in equipmentsData.SelectedEquipments)
			{
				stringBuilder.Append(".");
				stringBuilder.Append(selectedEquipment);
			}
			return stringBuilder.ToString();
		}
	}
}
