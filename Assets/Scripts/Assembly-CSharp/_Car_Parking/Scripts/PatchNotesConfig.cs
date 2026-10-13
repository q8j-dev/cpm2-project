using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts
{
	[Preserve]
	[CreateAssetMenu(menuName = "CarParking/PatchNotes/PatchNotesConfig")]
	public class PatchNotesConfig : ScriptableObject
	{
		public PatchNotesData[] PatchNotesDatas;

		public int HashData
		{
			get
			{
				string text = "";
				if (PatchNotesDatas != null)
				{
					for (int i = 0; i < PatchNotesDatas.Length; i++)
					{
						if ((bool)PatchNotesDatas[i])
						{
							text += PatchNotesDatas[i].name;
						}
					}
				}
				return text.GetHashCode();
			}
		}

		[Preserve]
		public PatchNotesConfig()
		{
		}
	}
}
