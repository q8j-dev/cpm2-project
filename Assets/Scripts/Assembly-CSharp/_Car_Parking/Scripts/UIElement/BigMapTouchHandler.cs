using MVPFree.MapIcons.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.UIElement
{
	public class BigMapTouchHandler : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerUpHandler, IPointerDownHandler
	{
		private FreeDriveProvider _freeDriveProvider;

		private MapIconsModel _mapIconsModel;

		public static bool IsMapOpen { get; private set; }

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, MapIconsModel mapIconsModel)
		{
			_freeDriveProvider = freeDriveProvider;
			_mapIconsModel = mapIconsModel;
		}

		private void OnEnable()
		{
			IsMapOpen = true;
			_mapIconsModel.OpenedBigMap(enabled: true);
		}

		private void OnDisable()
		{
			IsMapOpen = false;
			_mapIconsModel.OpenedBigMap(enabled: false);
		}

		public void OnDrag(PointerEventData eventData)
		{
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_mapIconsModel.OnBigMapPointerUp(eventData);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			_mapIconsModel.OnBigMapPointerDown(eventData);
		}
	}
}
