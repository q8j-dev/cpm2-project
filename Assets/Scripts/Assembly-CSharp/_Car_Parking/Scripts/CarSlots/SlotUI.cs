using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPM.Scripts.Garage;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Racing.Scripts.UIGrid;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.CarSlots
{
	public class SlotUI : MonoBehaviour, IGridElement<SlotUI>
	{
		[SerializeField]
		private TextMeshProUGUI CounterText;

		[SerializeField]
		private RawImage CarImage;

		[SerializeField]
		private Button _button;

		[Header("Car Class Visual")]
		[SerializeField]
		private Image CarClassGo;

		[SerializeField]
		private TextMeshProUGUI CarClassText;

		[SerializeField]
		private TextMeshProUGUI CarPpText;

		[Header("Buy slot elements")]
		[SerializeField]
		private Image PlusImage;

		[SerializeField]
		private TextMeshProUGUI BuySlotText;

		[SerializeField]
		[Header("Buy car elements")]
		private Image BuyCarImage;

		[SerializeField]
		private TextMeshProUGUI BuyCarText;

		[Header("Animation")]
		[SerializeField]
		private bool UseAnimation;

		[SerializeField]
		private GameObject AnimationTarget;

		[SerializeField]
		private float AnimationDuration;

		private Tween _tween;

		private bool _isDeactivate;

		private ICarInfoService _carInfoService;

		private ISlotService _slotService;

		private SlotCollectionUIHandler _slotCollectionUIHandler;

		private GarageController _garageController;

		private ManagerUI _managerUI;

		private IGarageEntity _currentGarage;

		private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

		public int ID { get; set; }

		public RawImage CarRawImage => CarImage;

		[Inject]
		public void Construct(ICarInfoService carInfoService, ManagerUI managerUI, ISlotService slotService, GarageController garageController, IGarageEntity garageEntity, VirtualPurchaseAnalytic virtualPurchaseAnalytic)
		{
			_managerUI = managerUI;
			_garageController = garageController;
			_carInfoService = carInfoService;
			_slotService = slotService;
			_currentGarage = garageEntity;
			_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
		}

		public void Init(GridController<SlotUI> gridController)
		{
			_slotCollectionUIHandler = (SlotCollectionUIHandler)gridController;
		}

		public void Populate(int elementId, bool isInteractable = true, bool ignoreEmptySlots = false)
		{
			base.name += elementId;
			_button.onClick.RemoveAllListeners();
			CounterText.text = (elementId + 1).ToString();
			if ((bool)PlusImage)
			{
				PlusImage.SetActive(active: false);
			}
			if ((bool)BuySlotText)
			{
				BuySlotText.SetActive(active: false);
			}
			if ((bool)BuyCarImage)
			{
				BuyCarImage.SetActive(active: false);
			}
			if ((bool)BuyCarText)
			{
				BuyCarText.SetActive(active: false);
			}
			CarSlot slot = (ignoreEmptySlots ? GetOccupiedCarSlotByIndex(elementId) : _slotCollectionUIHandler[elementId]);
			ID = slot.ID;
			if (ID == _slotService.SlotCollection.LastSlotId)
			{
				_button.Select();
				_slotCollectionUIHandler.OnElementSelect(this);
				GarageMenuWindow.SelectedSlotId = ID;
			}
			if (!string.IsNullOrEmpty(slot.CarFilePath))
			{
				int carID = _carInfoService.GetFromSlot(slot.ID).CarID;
				int expectedSlotId = slot.ID;
				CarImage.texture = null;
				CarImage.color = Color.clear;
				ResourceRequest imageRequest = Resources.LoadAsync<Texture>("UI/CarImages/b" + carID);
				imageRequest.completed += (AsyncOperation _) =>
				{
					if ((bool)this && (bool)CarImage && ID == expectedSlotId)
					{
						CarImage.texture = imageRequest.asset as Texture;
						if ((bool)CarImage.texture)
						{
							CarImage.color = Color.white;
						}
					}
				};
				_button.interactable = isInteractable;
				CarImage.gameObject.SetActive(value: true);
				VehicleData carBySpot = _currentGarage.GetCarBySpot(ID);
				if (carBySpot != null)
				{
					VehicleClass classVehicle = Utils.GetClassVehicle(carBySpot.GetVehicleEntityMono.CarEntity.PerformancePoints, carBySpot.CarId);
					CarClassText.text = classVehicle.ToString();
				}
				if (isInteractable)
				{
					_button.onClick.AddListener(() =>
					{
						OnClickButton(slot);
					});
				}
				return;
			}
			CarImage.gameObject.SetActive(value: false);
			if (elementId == 0 && _slotService.SlotCollection.IsAllSlotsEmpty())
			{
				BuyCarImage.SetActive(active: true);
				BuyCarText.SetActive(active: true);
				if (isInteractable)
				{
					_button.onClick.AddListener(BuySlot);
					return;
				}
			}
			_button.interactable = false;
			void BuySlot()
			{
				_managerUI.ShowNewCarsWindow();
				V.CoreGameManager.OpenSellCars();
			}
		}

		public void Deactivate()
		{
			GetComponent<CanvasGroup>().alpha = 0.3f;
			_isDeactivate = true;
		}

		private void OnClickButton(CarSlot slot)
		{
			if (_isDeactivate)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("EventViewBigBanner.SelectClass"), 2f).Forget();
				return;
			}
			_garageController.FocusOnCarInMenu(slot.ID);
			_slotCollectionUIHandler.OnElementSelect(this);
		}

		private CarSlot GetOccupiedCarSlotByIndex(int needIndex)
		{
			List<int> currentGarageOccupiedSlotIds = _slotService.GetCurrentGarageOccupiedSlotIds();
			if (currentGarageOccupiedSlotIds.Count <= needIndex)
			{
				return _slotCollectionUIHandler[needIndex];
			}
			return _slotService.GetSlot(currentGarageOccupiedSlotIds[needIndex]);
		}

		public void InitAddButton()
		{
			BuyCarImage.SetActive(active: false);
			BuyCarText.SetActive(active: false);
			CarImage.gameObject.SetActive(value: false);
			BuySlotText.SetActive(active: true);
			PlusImage.SetActive(active: true);
			Color color = _button.image.color;
			color.a = 0.6f;
			_button.image.color = color;
			CounterText.text = "";
			string confirmContentText = new LocalizedString("SlotUI.DoYouWantToBuyANewSlotForSlots");
			string price = VehicleSlotPaymentService.Price.ToString();
			_button.onClick.AddListener(() =>
			{
				_slotCollectionUIHandler.OnSlotAddOperationFinish?.Invoke(arg0: false);
				ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(new CombinedPrice(VehicleSlotPaymentService.Price, 0));
				List<int> allSlots = _slotService.GetAllSlots();
				if (allSlots.Count == 0)
				{
					_managerUI.ShowNotificationWindow(new LocalizedString("DebugAccountData.SomethingWentWrong"), 3f, NotificationType.Error);
				}
				else
				{
					int slotId = allSlots.Last() + 2;
					_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, PurchaseStatus.PURCHASE_CLICKED);
					_managerUI.ShowConfirmWindow(() =>
					{
						BuySlot();
					}, new LocalizedString("SlotUI.SlotPurchase"), confirmContentText.Replace("{0}", price), () =>
					{
						OnCancel();
						_slotCollectionUIHandler.OnSlotAddOperationFinish?.Invoke(arg0: true);
					}, "Confirm", null, additionalData);
				}
			});
			_button.interactable = true;
		}

		private async UniTask BuySlot()
		{
			int slotId = _slotService.GetAllSlots().Last() + 2;
			_slotCollectionUIHandler.OnSlotAddOperationFinish?.Invoke(arg0: true);
			_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, PurchaseStatus.PURCHASE_CONFIRMED);
			bool flag = await _slotService.TryBuySlot();
			_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, flag ? PurchaseStatus.PURCHASE_COMPLETED : PurchaseStatus.INSUFFICIENT_FUNDS);
		}

		private void OnCancel()
		{
			int slotId = _slotService.GetAllSlots().Last() + 2;
			_virtualPurchaseAnalytic.LogPurchaseVirtual_CarSlot(VehicleSlotPaymentService.Price, slotId, PurchaseStatus.PURCHASE_CANCELED_BY_USER);
		}

		public void SetSelected(bool isSelected)
		{
			Color color = CounterText.color;
			color.a = (isSelected ? 0.9f : 0.4f);
			CounterText.color = color;
			Color color2 = _button.image.color;
			color2.a = (isSelected ? 0.6f : 0.06f);
			_button.image.color = color2;
			if (UseAnimation)
			{
				if (isSelected)
				{
					_tween?.Kill();
					_tween = AnimationTarget.transform.DOScale(1.2f, AnimationDuration);
				}
				else
				{
					_tween?.Kill();
					AnimationTarget.transform.localScale = Vector3.one;
				}
			}
		}

		public void DeleteElement()
		{
			BuySlotText.SetActive(active: false);
			PlusImage.SetActive(active: false);
			CounterText.text = ID.ToString();
			CarImage.color = Color.clear;
			_button.onClick.RemoveAllListeners();
		}
	}
}
