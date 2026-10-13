using System;
using Car_Parking.Scripts.Racing;
using MVPFree.MapIcons.Enum;
using _Car_Parking.Scripts.EventSystem;

namespace MVPFree.MapIcons.ScrObjs.Scripts
{
	[Serializable]
	public class TimeAttackData
	{
		public EventType EventType;

		public string Name;

		public string Description;

		public IconType IconType;

		public int TrackIndex;

		public TimeAttackMode TimeAttackMode;

		public Surface[] RequiredSurfaces;
	}
}
