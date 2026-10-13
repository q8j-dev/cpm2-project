using CPM.Database;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class SlotItemContent : ItemContentDraggableContent
	{
		public class ContentData
		{
			public Texture Texture;

			public string CarInstanceId;

			public ContentData(Texture texture, string carInstanceId)
			{
				Texture = texture;
				CarInstanceId = carInstanceId;
			}
		}

		[SerializeField]
		private TMP_Text[] IndexesText;

		[SerializeField]
		private RawImageLoadAsync Icon;

		[SerializeField]
		private ItemContentDraggableAnimation ItemContentDraggableAnimation;

		[SerializeField]
		private GameObjectsActiveBySlotColumnPosition GameObjectsActiveBySlotColumnPosition;

		private string _carInstanceId = "";

		private ISlotService _slotService;

		private ICarInfoService _carInfoService;

		public bool IsActive { get; private set; } = true;

		public string CarInstanceId => _carInstanceId;

		[Inject]
		public void Construct(ISlotService slotService, ICarInfoService carInfoService)
		{
			_slotService = slotService;
			_carInfoService = carInfoService;
		}

		private void Awake()
		{
			ItemContentDraggableAnimation.SelectVisibleUpdate(visible: false);
		}

		public void ClearCarInstanceId()
		{
			_carInstanceId = "";
		}

		public override void DestroyAnimation()
		{
			IsActive = false;
			base.DestroyAnimation();
		}

		public void Init(int garageIndex, int slotIndex, SlotsViewColumn.PositionType positionType)
		{
			Init(slotIndex, positionType);
			Icon.ImageActive = false;
			SlotCollection slotCollection = _slotService.GetSlotCollection(garageIndex);
			if (slotCollection[slotIndex] != null && !slotCollection[slotIndex].IsEmpty)
			{
				if (_carInfoService.TryGetFromSlot(slotCollection[slotIndex].ID, out var carInfo))
				{
					Icon.Load(PathGetterHelper.GetCarPreviewPath(carInfo.CarID));
				}
				ItemContentDraggableAnimation.EmptyVisibleUpdate(visible: false);
			}
			else
			{
				ItemContentDraggableAnimation.EmptyVisibleUpdate(visible: true);
			}
			if (slotCollection[slotIndex] != null)
			{
				_carInstanceId = slotCollection[slotIndex].CarFilePath;
			}
		}

		public void Init(int slotIndex, SlotsViewColumn.PositionType positionType)
		{
			for (int i = 0; i < IndexesText.Length; i++)
			{
				IndexesText[i].text = $"{slotIndex + 1}";
			}
			GameObjectsActiveBySlotColumnPosition.UpdateState(positionType);
		}

		public override void CopyContent(ItemContentDraggableContent content)
		{
			SlotItemContent slotItemContent = content as SlotItemContent;
			if ((bool)slotItemContent)
			{
				SetContentData(slotItemContent.GetContentData());
				Icon.ImageActive = true;
				slotItemContent._carInstanceId = "";
			}
		}

		public override object GetContentData()
		{
			return new ContentData(Icon.Texture, _carInstanceId);
		}

		public override void SetContentData(object contentDataObject)
		{
			ContentData contentData = (ContentData)contentDataObject;
			Icon.Texture = contentData.Texture;
			_carInstanceId = contentData.CarInstanceId;
		}
	}
}
