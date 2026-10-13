using Cysharp.Threading.Tasks;
using DG.Tweening;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.ScrObjs.Scripts;
using Services.PriceStyleService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem.PropertyPopupWindow
{
	public abstract class PropertyPopupWindow : WindowScript
	{
		[SerializeField]
		private RectTransform TableRootTransform;

		[SerializeField]
		private Transform MainInformationContainer;

		[SerializeField]
		private TextMeshProUGUI NameText;

		[SerializeField]
		private TextMeshProUGUI LocationText;

		[SerializeField]
		private TextMeshProUGUI CostText;

		[SerializeField]
		private Image CostImage;

		[SerializeField]
		private RealCurrencyView RealMoneyCostView;

		[SerializeField]
		private Image PropertyTypeImage;

		private PriceStyleConfig _priceStyleConfig;

		private FreeDriveProvider _freeDriveProvider;

		private MapIconsDataSo _mapIconsDataSo;

		private FreeDriveControl _freeDriveControl;

		private bool _isClosing;

		private Sequence _appearSequence;

		private EventRegistry _eventRegistry;

		private RegistryHandle<BoolEvent> _onMapMoveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnMapMove);

		[field: SerializeField]
		protected Transform CostContainer { get; private set; }

		[Inject]
		public void Construct(PriceStyleConfig priceStyleConfig, FreeDriveProvider freeDriveProvider, MapIconsDataSo mapIconsDataSo, EventRegistry eventRegistry, FreeDriveControl freeDriveControl)
		{
			_priceStyleConfig = priceStyleConfig;
			_freeDriveProvider = freeDriveProvider;
			_mapIconsDataSo = mapIconsDataSo;
			_freeDriveControl = freeDriveControl;
			_eventRegistry = eventRegistry;
		}

		protected override void OnEnable()
		{
			PlayAppearAnimation();
			_freeDriveControl.OnBigMapHide += Close;
			base.OnEnable();
			_onMapMoveHandle.Get(_eventRegistry).Subscribe(OnMapMove);
		}

		private void OnMapMove(bool isStarted)
		{
			if (isStarted)
			{
				PlayCloseAnimation();
			}
		}

		private void OnDisable()
		{
			_freeDriveControl.OnBigMapHide -= Close;
			_appearSequence?.Kill();
			if (TableRootTransform != null)
			{
				TableRootTransform.DOKill();
			}
			_onMapMoveHandle.Get(_eventRegistry)?.Unsubscribe(OnMapMove);
		}

		protected void Init(string name, string location, IconType iconType, GlobalCost cost)
		{
			CostText.text = cost.Price.ToString();
			CostImage.sprite = _priceStyleConfig.GetByType(cost.PriceType).PriceSprite;
			SetUIView(name, location, iconType);
		}

		protected void InitRealCost(string name, string location, IconType iconType, string cost)
		{
			CostText.SetActive(active: false);
			CostImage.SetActive(active: false);
			RealMoneyCostView?.UpdateText(cost);
			SetUIView(name, location, iconType);
		}

		protected void PlayAppearAnimation()
		{
			TableRootTransform.DOKill();
			Vector2 zero = Vector2.zero;
			float width = TableRootTransform.rect.width;
			TableRootTransform.anchoredPosition = zero + Vector2.right * width;
			TableRootTransform.localScale = Vector3.zero;
			_appearSequence?.Kill();
			_appearSequence = DOTween.Sequence().SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
			_appearSequence.Append(TableRootTransform.DOAnchorPos(zero, 0.2f).SetEase(Ease.OutCubic));
			_appearSequence.Join(TableRootTransform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
		}

		private void Update()
		{
			if (_freeDriveProvider.MapCamera.beganTouching || !_freeDriveProvider.MapCamera.ShowingBigMap)
			{
				PlayCloseAnimation();
			}
		}

		private void SetUIView(string name, string location, IconType iconType)
		{
			NameText.text = name;
			LocationText.text = location;
			PropertyTypeImage.sprite = _mapIconsDataSo.GetIconData(iconType).SpriteIcon;
			MainInformationContainer?.Rebuild().Forget();
		}

		private void PlayCloseAnimation()
		{
			if (!_isClosing)
			{
				_isClosing = true;
				TableRootTransform.DOKill();
				TableRootTransform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).SetLink(base.gameObject, LinkBehaviour.KillOnDestroy)
					.OnComplete(Close);
			}
		}
	}
}
