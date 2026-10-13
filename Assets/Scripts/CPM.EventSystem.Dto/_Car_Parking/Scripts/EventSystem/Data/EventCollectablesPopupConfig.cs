using System;
using UnityEngine;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[CreateAssetMenu(menuName = "Configs/EventCollectablesPopupConfig")]
	public class EventCollectablesPopupConfig : ScriptableObject
	{
		[Serializable]
		public class PopupData
		{
			public EventType EventType;

			public float DelayToShow;

			public string PopupKey = "";

			public string AudioKey = "";
		}

		[SerializeField]
		private PopupData[] PopupDatas;

		public PopupData GetPopupData(EventType eventType)
		{
			for (int i = 0; i < PopupDatas.Length; i++)
			{
				if (PopupDatas[i].EventType == eventType)
				{
					return PopupDatas[i];
				}
			}
			return null;
		}
	}
}
