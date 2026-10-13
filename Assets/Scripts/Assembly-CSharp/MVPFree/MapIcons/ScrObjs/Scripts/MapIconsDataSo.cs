using System.Collections.Generic;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Views;
using UnityEngine;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace MVPFree.MapIcons.ScrObjs.Scripts
{
	[CreateAssetMenu(fileName = "MapIconsData", menuName = "Scriptable/MapIconsData")]
	public class MapIconsDataSo : ScriptableObject
	{
		private Dictionary<IconType, IconData> _iconsDataDic;

		[field: SerializeField]
		public SelectIconData SelectIcon { get; private set; }

		[field: SerializeField]
		public Color DescriptionSelectColor { get; private set; }

		[field: SerializeField]
		public float NotSelectedAlpha { get; private set; }

		[field: SerializeField]
		public Sprite IconHide { get; private set; }

		[field: SerializeField]
		public Sprite IconShow { get; private set; }

		[field: SerializeField]
		public float DistanceToCheckNearIcons { get; private set; }

		[field: SerializeField]
		public float TimeIntervalCheckNearIcons { get; private set; }

		[field: SerializeField]
		public IconMapView MapIconPrefab { get; private set; }

		[field: SerializeField]
		public IconDescriptionItemView UIMapDescriptionItemPrefab { get; private set; }

		[field: SerializeField]
		public Vector3 InitIconRotation { get; private set; }

		[field: SerializeField]
		public Vector3 InitIconPositionY { get; private set; }

		[field: SerializeField]
		public Vector3 RotationImageArrowDown { get; private set; }

		[field: SerializeField]
		public Vector3 RotationImageArrowUp { get; private set; }

		[field: SerializeField]
		public List<IconData> MapIconsData { get; private set; }

		[field: SerializeField]
		public List<IconData> MapIconsDatasForEvents { get; private set; }

		[field: SerializeField]
		public List<_Car_Parking.Scripts.EventSystem.EventType> CurrentEvents { get; private set; }

		[field: SerializeField]
		public List<IconType> IconsToIgnoreToAddDescriptionList { get; private set; }

		[field: SerializeField]
		public List<IconType> IconsToDisableWhenZoomCam { get; private set; }

		[field: SerializeField]
		public List<IconType> IconsDescriptionsToIgnoreCreateInSinglePlayer { get; private set; }

		public void Init()
		{
			if (_iconsDataDic == null)
			{
				_iconsDataDic = new Dictionary<IconType, IconData>();
				InitIcons(MapIconsData);
				InitIcons(MapIconsDatasForEvents);
			}
		}

		private void InitIcons(List<IconData> iconsDatas)
		{
			foreach (IconData iconsData in iconsDatas)
			{
				if (iconsData.IconType != IconType.None && !_iconsDataDic.ContainsKey(iconsData.IconType))
				{
					_iconsDataDic.Add(iconsData.IconType, iconsData);
				}
			}
		}

		public IconData GetIconData(IconType iconType)
		{
			if (_iconsDataDic.IsNullOrEmpty() || !_iconsDataDic.TryGetValue(iconType, out var value))
			{
				return null;
			}
			return value;
		}

		public List<IconData> GetCurrentEventIcons(IEventService eventService)
		{
			List<IconData> list = new List<IconData>();
			foreach (IconData mapIconsDatasForEvent in MapIconsDatasForEvents)
			{
				if (eventService?.GetEventByName(mapIconsDatasForEvent.EventType) != null)
				{
					list.Add(mapIconsDatasForEvent);
				}
			}
			return list;
		}
	}
}
