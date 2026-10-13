using DG.Tweening;
using Services.PriceStyleService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.UIElement
{
	public class ApartmentPlaceUIItem : PlaceUIItem<HomesConfig, HomesConfig.HomeDataItem>
	{
		[SerializeField]
		private TextMeshProUGUI PriceText;

		[SerializeField]
		private TextMeshProUGUI LocationNameText;

		[SerializeField]
		private TextMeshProUGUI NameText;

		[SerializeField]
		private GameObject BlockObject;

		[SerializeField]
		private Image MoneyFrame;

		[SerializeField]
		private Image MoneyImage;

		[SerializeField]
		private Button Button;

		[SerializeField]
		private RawImageLoadAsync MainImage;

		[SerializeField]
		private Image SelectedItemPlace;

		private PriceStyleConfig _priceStyleConfig;

		[Inject]
		public void Construct(PriceStyleConfig priceStyleConfig)
		{
			_priceStyleConfig = priceStyleConfig;
		}

		protected void OnEnable()
		{
			Button.onClick.AddListener(OnClick);
		}

		protected void OnDisable()
		{
			Button.onClick.RemoveListener(OnClick);
		}

		public override void Init()
		{
			base.Init();
			PriceStyleData style = GetStyle();
			float a = MoneyFrame.color.a;
			MoneyFrame.color = style.PriceColor;
			MoneyFrame.DOFade(a, 0f);
			MoneyImage.sprite = style.PriceSprite;
			PriceText.text = $"{base.DataItem.GetPrice:n0}";
			NameText.text = new LocalizedString(base.DataItem.Name);
			LocationNameText.text = new LocalizedString("PropertyWindow." + base.DataItem.PlaceToSpawn);
			MainImage.LoadFromAddressables(base.DataItem.SecondaryImageAddressablePath);
		}

		private PriceStyleData GetStyle()
		{
			return _priceStyleConfig.GetByType(base.DataItem.PriceType);
		}

		protected override HomesConfig.HomeDataItem GetData(int index)
		{
			return config.Get(index);
		}

		public void ChangeActiveState(bool state)
		{
			BlockObject.SetActive(state);
		}

		private void OnClick()
		{
			if (!base.IsChosen)
			{
				base.IsChosen = true;
			}
		}

		protected override void OnChosenChanged()
		{
			SetSelectedItemPlaceActive(base.IsChosen);
		}

		private void SetSelectedItemPlaceActive(bool active)
		{
			SelectedItemPlace.DOKill();
			if (active)
			{
				SelectedItemPlace.gameObject.SetActive(value: true);
				SelectedItemPlace.DOFade(1f, 0.3f);
			}
			else
			{
				SelectedItemPlace.DOFade(0f, 0.3f).OnComplete(() =>
				{
					SelectedItemPlace.gameObject.SetActive(value: false);
				});
			}
		}
	}
}
