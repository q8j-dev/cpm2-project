using UnityEngine;

namespace _Car_Parking.Scripts.FirstGameSetup
{
	[CreateAssetMenu(fileName = "Character equipment presets", menuName = "Scriptables/Character equipment presets")]
	public class CharacterEquipmentPresetsSo : ScriptableObject
	{
		[field: SerializeField]
		public CharacterEquipmentPreset[] Presets { get; private set; }
	}
}
