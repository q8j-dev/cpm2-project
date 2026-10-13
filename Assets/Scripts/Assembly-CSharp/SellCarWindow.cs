using System.Globalization;
using CPM.Common;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

public class SellCarWindow : WindowScriptAsyncResult<SellCarWindow.CarPriceData>, IBackableWindow
{
	public class CarPriceData
	{
		public string CarModel;

		public int CarPrice;

		public CarPriceData(string carModel, int carPrice)
		{
			CarModel = carModel;
			CarPrice = carPrice;
		}
	}

	public static ObscuredInt MaxPrice = 50000000;

	[SerializeField]
	private TMP_InputField InputCarModel = default;

	[SerializeField]
	private TMP_InputField InputPrice = default;

	[SerializeField]
	private TMP_Text FakePriceText = default;

	[SerializeField]
	private Button ButtonCancel = default;

	[SerializeField]
	private Button ButtonSubmit = default;

	private InputFieldStateController _modelInputFieldState;

	private InputFieldStateController _priceInputFieldState;

	private IChatFilterUtility _chatFilterUtility;

	private FreeDriveDB _freeDriveDB;

	private ParkingMenuUI _parkingMenuUI;

	private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

	private Tween _priceShakeTween;

	private const int MaxSymbolsInputPrice = 8;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(IChatFilterUtility chatFilterUtility, FreeDriveDB freeDriveDB, OptionalDependency<ParkingMenuUI> parkingMenuUI)
	{
		_parkingMenuUIDep = parkingMenuUI;
		_freeDriveDB = freeDriveDB;
		_chatFilterUtility = chatFilterUtility;
	}

	protected override void Awake()
	{
		base.Awake();
		_parkingMenuUI = _parkingMenuUIDep?.Value;
	}

	public void InitBackButton()
	{
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		Close();
	}

	protected override void Start()
	{
		base.Start();
		if ((bool)_managerUI.CircleNotification)
		{
			_managerUI.CircleNotification.RejectAndClose();
		}
		if ((bool)_managerUI.RequestWindowTemplate)
		{
			_managerUI.RequestWindowTemplate.Close();
		}
		_modelInputFieldState = InputCarModel.GetComponent<InputFieldStateController>();
		_priceInputFieldState = InputPrice.GetComponent<InputFieldStateController>();
		InputPrice.characterLimit = 8;
		InputPrice.onValueChanged.AddListener(OnInputPriceValueChanged);
		ButtonCancel.onClick.AddListener(Close);
		ButtonSubmit.onClick.AddListener(Submit);
		if ((bool)_parkingMenuUI)
		{
			_parkingMenuUI.Hide();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_priceShakeTween?.Kill();
		InputPrice.onValueChanged.RemoveListener(OnInputPriceValueChanged);
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			base.Close();
			if ((bool)_parkingMenuUI)
			{
				_parkingMenuUI.Show();
			}
		}
	}

	private void OnInputPriceValueChanged(string text)
	{
		string text2 = text.TrimStart('0');
		if (text2 != text)
		{
			InputPrice.SetTextWithoutNotify(text2);
			InputPrice.caretPosition = text2.Length;
		}
		if (int.TryParse(InputPrice.text, out var result))
		{
			NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
			numberFormatInfo.NumberGroupSeparator = " ";
			FakePriceText.text = result.ToString("N0", numberFormatInfo);
		}
		else
		{
			FakePriceText.text = "";
		}
	}

	private void ShowPriceError()
	{
		_priceInputFieldState.SetError();
		_priceShakeTween?.Kill(complete: true);
		_priceShakeTween = InputPrice.transform.DOShakePosition(0.5f, 10f);
	}

	private void Submit()
	{
		int result;
		if (string.IsNullOrEmpty(InputPrice.text))
		{
			ShowPriceError();
		}
		else if (!int.TryParse(InputPrice.text, out result))
		{
			ShowPriceError();
			_managerUI.ShowNotificationWindow(new LocalizedString("SellCarWindow.InvalidPrice"), 3f).Forget();
		}
		else if (result < 1)
		{
			ShowPriceError();
		}
		else if (result > (int)MaxPrice)
		{
			ShowPriceError();
			string text = new LocalizedString("SellCarWindow.PriceShouldBeLessThanMaxprice");
			_managerUI.ShowNotificationWindow(text.Replace("{MaxPrice}", MaxPrice.ToString()), 3f).Forget();
		}
		else
		{
			InputCarModel.text = PlayerDatas.CheckCarModelText(InputCarModel.text);
			InputCarModel.text = _chatFilterUtility.Filter(InputCarModel.text);
			TrySetResult(new ResultData(CodeType.Success, new CarPriceData(InputCarModel.text, result)));
			Close();
		}
	}
}
