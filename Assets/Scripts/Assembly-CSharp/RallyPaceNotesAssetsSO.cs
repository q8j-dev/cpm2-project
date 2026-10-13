using System.Linq;
using UnityEngine;
using _Car_Parking.Scripts.Rally.Data;

[CreateAssetMenu(menuName = "Rally/PaceNoteIconsSO")]
public class RallyPaceNotesAssetsSO : ScriptableObject
{
	[SerializeField]
	private Sprite[] TurnIcons;

	[SerializeField]
	private Sprite[] LinkIcons;

	[SerializeField]
	private RallyMarkIconData[] MarkIcons;

	public Sprite GetTurnIcon(RallySequenceTurnType type)
	{
		return TurnIcons[(int)type];
	}

	public Sprite GetLinkIcon(RallySequenceLink link)
	{
		return LinkIcons[(int)link];
	}

	public Sprite GetMarkIcon(RallySequenceMark mark)
	{
		return MarkIcons.FirstOrDefault((RallyMarkIconData m) => m.SequenceMark == mark)?.Icon;
	}
}
