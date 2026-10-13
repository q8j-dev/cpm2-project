using System.Globalization;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.UIElement;

public class BuyCarWorldWindow : MonoBehaviour
{
	public const string JoystickUITag = "JoystickUI";

	[SerializeField]
	private TextMeshProUGUI TextPrice;

	[SerializeField]
	private TextMeshProUGUI TextCarData;

	[SerializeField]
	private TextMeshProUGUI TextPlayerName;

	[SerializeField]
	private RectTransform TopRect;

	private SellCarTrigger _sellCarTrigger;

	private MenuLevelParent _menuLevelParent;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private IAccountDataProvider _accountDataProvider;

	[Inject]
	public void Construct(MenuLevelParent menuLevelParent, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveProvider freeDriveProvider, IAccountDataProvider accountDataProvider)
	{
		_menuLevelParent = menuLevelParent;
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_freeDriveProvider = freeDriveProvider;
		_accountDataProvider = accountDataProvider;
	}

	private void OnEnable()
	{
		UIHelper.DelayedRebuildLayout(TopRect).Forget();
	}

	public void Init(string carData, int price, SellCarTrigger sellCarTrigger)
	{
		NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
		numberFormatInfo.NumberGroupSeparator = " ";
		TextPrice.text = price.ToString("N0", numberFormatInfo);
		TextCarData.text = carData;
		_sellCarTrigger = sellCarTrigger;
		TextPlayerName.text = "";
		if ((bool)sellCarTrigger && (bool)sellCarTrigger.SellingCar)
		{
			MultiPersonControl playerWithCar = _onlineCollectionsProvider.GetPlayerWithCar(sellCarTrigger.SellingCar);
			if ((bool)playerWithCar)
			{
				TextPlayerName.text = playerWithCar.ThisPlayerName;
			}
			else if (_freeDriveProvider.FirstCarMultiPlayer == sellCarTrigger.SellingCar)
			{
				TextPlayerName.text = _accountDataProvider.Name;
			}
		}
		UIHelper.DelayedRebuildLayout(TopRect).Forget();
	}

	public void Close()
	{
		if ((bool)base.gameObject)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void TryBuyFromAim()
	{
		if (!_menuLevelParent.isDroneEnabled)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.AcceptSwitch, base.gameObject);
			_sellCarTrigger.BuyThisCar();
		}
	}
}
