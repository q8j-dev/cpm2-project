using System;
using System.Threading;
using CPM.Scripts.Garage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.View;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement
{
	public class BigOfferWindowBase : MonoBehaviour, IBigBannerView
	{
		[SerializeField]
		private Button ButtonClose;

		[SerializeField]
		private Button ButtonBuy;

		[SerializeField]
		protected Button ButtonViewInShop;

		[SerializeField]
		private bool IsDefaultBanner;

		[SerializeField]
		protected OfferBigBannerView OfferBigBannerView;

		private int _offerID = -1;

		protected GarageController _garage;

		private OptionalDependency<GarageController> _garageDep;

		protected CoreGameManager _coreGameManager;

		protected IPurchaseSystem _purchaseSystem;

		protected ManagerUI _managerUI;

		private CancellationTokenSource _cancellationTokenSourceCheckOfferTime;

		protected IOfferSystem _offerSystem;

		[Inject]
		public void Construct(CoreGameManager coreGameManager, IOfferSystem offerSystem, OptionalDependency<GarageController> garage, IPurchaseSystem purchaseSystem, ManagerUI managerUI)
		{
			_offerSystem = offerSystem;
			_garageDep = garage;
			_coreGameManager = coreGameManager;
			_purchaseSystem = purchaseSystem;
			_managerUI = managerUI;
		}

		protected virtual void Awake()
		{
			_garage = _garageDep?.Value;
		}

		public void InitBanner(IBigBannerView.BannerData bannerData)
		{
			if (bannerData != null && bannerData.OfferSkinModel != null)
			{
				_offerID = bannerData.OfferID;
			}
		}

		public bool IsCorrectPrefab(IBigBannerView.BannerData bannerData)
		{
			if (bannerData == null || bannerData.OfferSkinModel == null)
			{
				return false;
			}
			if (_offerID >= 0 && bannerData.OfferID >= 0)
			{
				return _offerID == bannerData.OfferID;
			}
			if (string.IsNullOrEmpty(bannerData.OfferSkinModel.BannerPrefab))
			{
				return IsDefaultBanner;
			}
			return bannerData.OfferSkinModel.BannerPrefab == base.gameObject.name;
		}

		public void Shown()
		{
			_offerSystem.MarkShownBigBanner(_offerID);
		}

		private void Start()
		{
			OfferModel offerModel = null;
			if (_offerID >= 0)
			{
				offerModel = _offerSystem.GetOffer(_offerID);
			}
			if (offerModel == null)
			{
				offerModel = _offerSystem.GetOfferWithBiggestPriority();
			}
			OfferBigBannerView.Init(offerModel);
			OnOfferLoaded(offerModel);
			_cancellationTokenSourceCheckOfferTime = new CancellationTokenSource();
			CheckOfferTimeInitItemsAsync(_cancellationTokenSourceCheckOfferTime.Token).Forget();
		}

		private void OnEnable()
		{
			AddCallbacks();
		}

		private void OnDisable()
		{
			RemoveCallbacks();
		}

		private void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheckOfferTime);
		}

		protected void PrepareToClose()
		{
			RemoveCallbacks();
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheckOfferTime);
		}

		protected virtual void AddCallbacks()
		{
			ButtonClose.onClick.AddListener(ButtonCloseClickCallback);
			ButtonBuy.onClick.AddListener(ButtonBuyClickCallback);
			ButtonViewInShop.onClick.AddListener(ButtonViewInShopClickCallback);
		}

		protected virtual void RemoveCallbacks()
		{
			ButtonClose.onClick.RemoveListener(ButtonCloseClickCallback);
			ButtonBuy.onClick.RemoveListener(ButtonBuyClickCallback);
			ButtonViewInShop.onClick.RemoveListener(ButtonViewInShopClickCallback);
		}

		protected void ButtonCloseClickCallback()
		{
			PrepareToClose();
			EventView.Closed?.Invoke();
		}

		protected virtual void ButtonBuyClickCallback()
		{
		}

		protected virtual void ButtonViewInShopClickCallback()
		{
		}

		protected virtual void OnOfferLoaded(OfferModel offerModel)
		{
		}

		private async UniTaskVoid CheckOfferTimeInitItemsAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				if ((bool)OfferBigBannerView && OfferBigBannerView.CurrentOfferModel != null && !OfferBigBannerView.CurrentOfferModel.IsValid)
				{
					ButtonCloseClickCallback();
					break;
				}
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
		}
	}
}
