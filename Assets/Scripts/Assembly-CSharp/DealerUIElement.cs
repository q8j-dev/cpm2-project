using System.Collections.Generic;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

public class DealerUIElement : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private RawImage carImage;

	[SerializeField]
	private Image BackgroundClassImage;

	[SerializeField]
	private TextMeshProUGUI ClassText;

	[SerializeField]
	private TextMeshProUGUI PerformancePointsText;

	[SerializeField]
	private CombinedPriceView CombinedPriceView;

	[SerializeField]
	private float DefaultCarImageScale = 1f;

	[SerializeField]
	private float SelectedCarImageScale = 1.2f;

	[SerializeField]
	private GameObject BundleMark;

	[SerializeField]
	private Button BundleButton;

	private Tween _tween;

	private int _index;

	private DealerWindow _dealerWindow;

	private CarPriceHelperV2 _carPriceHelper;

	private NewCarsMarkController _newCarsMarkController;

	private ObscuredInt _validate;

	private IOfferSystem _offerSystem;

	private ManagerUI _managerUI;

	[field: SerializeField]
	public TextMeshProUGUI PriceText { get; private set; }

	[field: SerializeField]
	public Image PayImage { get; private set; }

	[field: SerializeField]
	public GameObject NewCarMark { get; private set; }

	[field: SerializeField]
	public Image SideIcon { get; private set; }

	[field: SerializeField]
	public GameObject PriceRoot { get; private set; }

	public int CarId => _validate;

	public GameObject SelectableObject
	{
		get
		{
			if (!button)
			{
				return null;
			}
			return button.gameObject;
		}
	}

	[Inject]
	public void Construct(CarPriceHelperV2 carPriceHelperV2, NewCarsMarkController newCarsMarkController, IOfferSystem offerSystem, ManagerUI managerUI)
	{
		_carPriceHelper = carPriceHelperV2;
		_newCarsMarkController = newCarsMarkController;
		_offerSystem = offerSystem;
		_managerUI = managerUI;
	}

	public void Init(DealerWindow dealerWindow, NewVehicleData newVehicleData)
	{
		newVehicleData.Id.Get(out _index);
		_validate = newVehicleData.Data.Serial;
		_dealerWindow = dealerWindow;
		PayImage.sprite = newVehicleData.MoneySprite;
		PriceText.text = $"{newVehicleData.Data.GetPriceUI:n0}";
		PriceText.color = newVehicleData.MoneyColor;
		carImage.texture = newVehicleData.CarTexture;
		carImage.enabled = carImage.texture;
		BackgroundClassImage.color = Utils.GetClassColor(newVehicleData.Data.GetClass);
		ClassText.text = newVehicleData.Data.GetClassName;
		PerformancePointsText.text = newVehicleData.Data.GetPerformance.ToString();
		button.onClick.RemoveListener(OnButtonClick);
		button.onClick.AddListener(OnButtonClick);
		if (_newCarsMarkController != null)
		{
			NewCarMark.SetActive(_newCarsMarkController.IsCarNew(newVehicleData.Data.Serial));
		}
		BundleMarkUpdated();
		UpdatePrice(newVehicleData.Data.Serial);
	}

	public void SetCarTexture(Texture texture)
	{
		carImage.texture = texture;
		carImage.enabled = carImage.texture;
	}

	public void UpdatePrice(ObscuredInt val)
	{
		CombinedPrice carPrice = _carPriceHelper.GetCarPrice(val);
		if ((bool)CombinedPriceView && carPrice != null)
		{
			CombinedPriceView.Init(carPrice);
		}
	}

	private void OnButtonClick()
	{
		_dealerWindow.SnapToCar(_index).Forget();
	}

	public void OnFocus(bool state)
	{
		_tween = base.transform.DOScale(state ? SelectedCarImageScale : DefaultCarImageScale, 0.5f);
	}

	private void OnEnable()
	{
		if (_newCarsMarkController != null)
		{
			_newCarsMarkController.OnOldCarUpdatedEvent += OldCarUpdatedCallback;
		}
		BundleButton.onClick.AddListener(OnBundleButtonClick);
		_offerSystem.OnDataUpdatedEvent += OnOffersDataUpdated;
	}

	private void OnDisable()
	{
		if (_newCarsMarkController != null)
		{
			_newCarsMarkController.OnOldCarUpdatedEvent -= OldCarUpdatedCallback;
		}
		BundleButton.onClick.RemoveListener(OnBundleButtonClick);
		_offerSystem.OnDataUpdatedEvent -= OnOffersDataUpdated;
		_tween.Kill();
	}

	private void OnOffersDataUpdated()
	{
		BundleMarkUpdated();
	}

	private void OldCarUpdatedCallback(int carID)
	{
		if (carID == (int)_validate)
		{
			NewCarMark.SetActive(value: false);
		}
	}

	private void BundleMarkUpdated()
	{
		List<OfferModel> offersWithCarIdAndHaveSuit = _offerSystem.GetOffersWithCarIdAndHaveSuit(_validate);
		BundleMark.SetActive(offersWithCarIdAndHaveSuit.Count > 0);
	}

	private void OnBundleButtonClick()
	{
		List<OfferModel> offersWithCarIdAndHaveSuit = _offerSystem.GetOffersWithCarIdAndHaveSuit(_validate);
		if (offersWithCarIdAndHaveSuit.Count != 0)
		{
			EventViewWindow.AdditionalData additionalData = new EventViewWindow.AdditionalData(new IBigBannerView.BannerData(offersWithCarIdAndHaveSuit[0]));
			additionalData.PreviousWindow = "NewCarsWindow";
			WindowScript window = _managerUI.GetWindow("NewCarsWindow");
			if ((bool)window && window.TryGetComponent<TweenerCanvas>(out var component))
			{
				component.Hide();
			}
			_managerUI.ShowWindow("EventViewWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false, JsonConvert.SerializeObject(additionalData)).Forget();
		}
	}
}
