using System;
using UnityEngine;
using _Car_Parking.Scripts.EventSystem;

namespace _Car_Parking.Scripts
{
	[CreateAssetMenu(menuName = "CarParking/PatchNotes/PatchNotesData")]
	public class PatchNotesData : ScriptableObject
	{
		[Serializable]
		public class CarItem
		{
			public string Text;

			public int Index = -1;

			public bool IsSpecial;

			public PatchNoteCarType PatchNoteCarType;

			public _Car_Parking.Scripts.EventSystem.EventType EventType;
		}

		public enum PatchNoteCarType
		{
			None = 0,
			DailyReward = 1,
			Event = 2
		}

		[TextArea]
		public string[] Features;

		[TextArea]
		public string[] Items;

		[TextArea]
		public string[] Fix;

		public CarItem[] Cars;
	}
}
