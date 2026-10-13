using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ConfirmBuyWindowBase : WindowScript, IBackableWindow
	{
		[SerializeField]
		private Button CancelButton = default;

		[SerializeField]
		private Button BuyButton = default;

		[SerializeField]
		private Image BuyButtonGroup = default;

		[SerializeField]
		private RawImageLoadAsync MainIconRawImage = default;

		[SerializeField]
		private Image MainIcon = default;

		[SerializeField]
		private TextMeshProUGUI NameText = default;

		[SerializeField]
		private Image PriceIcon = default;

		[SerializeField]
		private Text PriceText = default;

		[SerializeField]
		private TextMeshProUGUI PriceTextMesh = default;

		private ParkingMenuUI _parkingMenuUI;

		private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

		private ChatControl _chatControl;

		private OptionalDependency<ChatControl> _chatControlDep;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(OptionalDependency<ParkingMenuUI> parkingMenuUI, OptionalDependency<ChatControl> chatControl)
		{
			_chatControlDep = chatControl;
			_parkingMenuUIDep = parkingMenuUI;
		}

		protected override void Awake()
		{
			base.Awake();
			_parkingMenuUI = _parkingMenuUIDep?.Value;
			_chatControl = _chatControlDep?.Value;
		}

		public void Back()
		{
			OnCancel();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: false);
			_managerUI.SetCurrentWindow(this);
		}

		protected override void OnEnable()
		{
			CancelButton?.onClick.AddListener(OnCancel);
			BuyButton?.onClick.AddListener(OnBuy);
			if (ScenesHelper.IsFreeDriveSceneActive())
			{
				_parkingMenuUI?.gameObject.SetActive(value: false);
				if ((bool)_chatControl)
				{
					_chatControl.HideAllChat();
				}
			}
			base.OnEnable();
			InitBackButton();
		}

		protected virtual void OnDisable()
		{
			CancelButton?.onClick.RemoveListener(OnCancel);
			BuyButton?.onClick.RemoveListener(OnBuy);
			if (ScenesHelper.IsFreeDriveSceneActive())
			{
				_parkingMenuUI?.gameObject.SetActive(value: true);
				if ((bool)_chatControl)
				{
					_chatControl.ShowPartiallyChat();
				}
			}
		}

		protected void SetMainView(Sprite mainIcon, string name)
		{
			if ((bool)MainIcon)
			{
				MainIcon.sprite = mainIcon;
			}
			if ((bool)NameText)
			{
				NameText.text = name;
			}
		}

		protected void SetMainView(string mainIconAddressablePath, string name)
		{
			if ((bool)MainIconRawImage)
			{
				MainIconRawImage.LoadFromAddressables(mainIconAddressablePath);
			}
			if ((bool)NameText)
			{
				NameText.text = name;
			}
		}

		protected void SetPrice(Sprite priceIcon, string priceText, bool showPriceIcon = true)
		{
			if ((bool)PriceIcon)
			{
				PriceIcon.sprite = priceIcon;
			}
			PriceIcon?.gameObject.SetActive(showPriceIcon);
			if ((bool)PriceTextMesh)
			{
				PriceTextMesh.SetActive(showPriceIcon);
				if (showPriceIcon)
				{
					PriceTextMesh.text = priceText;
				}
			}
			if ((bool)PriceText)
			{
				PriceText.SetActive(!showPriceIcon);
				if (!showPriceIcon)
				{
					PriceText.text = priceText;
				}
			}
		}

		protected void SetBuyBlock()
		{
			if ((bool)BuyButton)
			{
				BuyButton.interactable = false;
			}
			if ((bool)BuyButtonGroup)
			{
				BuyButtonGroup.DOFade(0.2f, 0f);
			}
		}

		protected virtual void OnCancel()
		{
		}

		protected virtual void OnBuy()
		{
		}
	}
}
