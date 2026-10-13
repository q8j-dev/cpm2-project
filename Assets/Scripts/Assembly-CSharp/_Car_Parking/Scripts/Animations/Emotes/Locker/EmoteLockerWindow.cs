using System;
using System.Collections.Generic;
using CarInteractionAnimationsScripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Offers.Reward;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.Animations.Emotes.Locker
{
	public class EmoteLockerWindow : WindowScript, IBackableWindow
	{
		private enum Mode
		{
			Shop = 0,
			FavAdd = 1,
			FavRemove = 2,
			Emoji = 3,
			EmojiFavAdd = 4,
			EmojiFavRemove = 5
		}

		private enum MovesFilter
		{
			Favorites = 0,
			CoinAnimations = 1,
			DanceAnimations = 2,
			Poses = 3
		}

		private const string LastTabMoves = "moves";

		private const string LastTabEmoji = "emoji";

		private const string BundleNotAvailableKey = "SuitPack.PackNotAvailable";

		private const float ViewportBottomWithPackBuy = 140f;

		private const float ViewportBottomWithPreviewText = 80f;

		private const float ViewportBottomWithPackBuyAndPreviewText = 195.6f;

		private const float PackBuyRaiseWithPreviewText = 55.6f;

		private const float ScrollToSelectedDuration = 0.25f;

		private const float ScrollToSelectedMargin = 10f;

		[SerializeField]
		private Toggle MovesTabToggle;

		[SerializeField]
		private Toggle EmojiTabToggle;

		[SerializeField]
		private Toggle FavoritesFilterToggle;

		[SerializeField]
		private Toggle ClipsFilterToggle;

		[SerializeField]
		private Toggle DanceFilterToggle;

		[SerializeField]
		private Toggle PosesFilterToggle;

		[SerializeField]
		private GameObject ClipsNoticeDot;

		[SerializeField]
		private GameObject DanceNoticeDot;

		[SerializeField]
		private GameObject PosesNoticeDot;

		[SerializeField]
		private GameObject MovesFilterPanel;

		[SerializeField]
		private GameObject EmojiFilterPanel;

		[SerializeField]
		private Toggle EmojiAllFilterToggle;

		[SerializeField]
		private Toggle EmojiFavoritesFilterToggle;

		[SerializeField]
		private EmoteLockerGrid Grid;

		[SerializeField]
		private EmoteLockerGrid EmojiGrid;

		[SerializeField]
		private Button AddButton;

		[SerializeField]
		private Button RemoveButton;

		[SerializeField]
		private Button EmojiAddButton;

		[SerializeField]
		private Button EmojiRemoveButton;

		[SerializeField]
		private Button ActionButton;

		[SerializeField]
		private TextMeshProUGUI ActionButtonText;

		[SerializeField]
		private Button BackgroundCloseButton;

		[SerializeField]
		private Button PackBuyButton;

		[SerializeField]
		private GameObject PreviewText;

		[SerializeField]
		private Button CancelButton;

		[SerializeField]
		private RectTransform GridViewport;

		private EmoteCatalog _catalog;

		private EmojiCatalogSO _emojiCatalog;

		private EmoteFavoritesService _favorites;

		private EmoteNoticeTracker _notices;

		private EmoteLockerOpenRequest _openRequest;

		private CurrencyManagerController _currencyManagerController;

		private BundleOfferCatalog _bundleOfferCatalog;

		private OptionalDependency<FreeDriveProvider> _freeDriveProviderDep;

		private OptionalDependency<ChatControl> _chatControlDep;

		private EmoteLockerContext _context;

		private EmotePlaybackController _playback;

		private ChatControl _chatControl;

		private Mode _mode;

		private MovesFilter _movesFilter = MovesFilter.CoinAnimations;

		private bool _emojiFavoritesOnly;

		private bool _inCar;

		private int _gender = -1;

		private bool _wiringUi;

		private bool _closing;

		private int _selectedPickerId = -1;

		private IEmoteEntry _selectedPickerEntry;

		private EmoteLockerItem _selectedPickerCard;

		private readonly HashSet<int> _editSelection = new HashSet<int>();

		private IDisposable _previewAutoStop;

		private Action _turnOffAction;

		private float _viewportOriginalBottom;

		private bool _viewportBottomCaptured;

		private RectTransform _packBuyRect;

		private float _packBuyOriginalY;

		private ScrollRect _gridScroll;

		private Tweener _scrollToSelectedTween;

		private bool IsEmojiSection
		{
			get
			{
				if (_mode != Mode.Emoji)
				{
					return IsEmojiEdit;
				}
				return true;
			}
		}

		private bool IsMovesEdit
		{
			get
			{
				if (_mode != Mode.FavAdd)
				{
					return _mode == Mode.FavRemove;
				}
				return true;
			}
		}

		private bool IsEmojiEdit
		{
			get
			{
				if (_mode != Mode.EmojiFavAdd)
				{
					return _mode == Mode.EmojiFavRemove;
				}
				return true;
			}
		}

		private bool IsEditMode
		{
			get
			{
				if (!IsMovesEdit)
				{
					return IsEmojiEdit;
				}
				return true;
			}
		}

		protected override InputGroupsEnum WindowInputGroup
		{
			get
			{
				if (!FlagsStorage.OutOfCar)
				{
					if (!FlagsStorage.IsPassenger)
					{
						return InputGroupsEnum.Ui;
					}
					return InputGroupsEnum.CarOther;
				}
				return InputGroupsEnum.PersonOther;
			}
		}

		private EmoteLockerGrid ActiveGrid
		{
			get
			{
				if (!IsEmojiSection || !EmojiGrid)
				{
					return Grid;
				}
				return EmojiGrid;
			}
		}

		private bool IsDanceShopTab
		{
			get
			{
				if (_mode == Mode.Shop)
				{
					return _movesFilter == MovesFilter.DanceAnimations;
				}
				return false;
			}
		}

		[Inject]
		public void Construct(EmoteCatalog catalog, EmojiCatalogSO emojiCatalog, EmoteFavoritesService favorites, EmoteNoticeTracker notices, EmoteLockerOpenRequest openRequest, CurrencyManagerController currencyManagerController, BundleOfferCatalog bundleOfferCatalog, OptionalDependency<FreeDriveProvider> freeDriveProvider, OptionalDependency<ChatControl> chatControl)
		{
			_catalog = catalog;
			_emojiCatalog = emojiCatalog;
			_favorites = favorites;
			_notices = notices;
			_openRequest = openRequest;
			_currencyManagerController = currencyManagerController;
			_bundleOfferCatalog = bundleOfferCatalog;
			_freeDriveProviderDep = freeDriveProvider;
			_chatControlDep = chatControl;
		}

		protected override void Start()
		{
			_openRequest.TryConsume(out _context);
			base.Start();
			_turnOffAction = TurnOffPreview;
			if ((bool)BackgroundCloseButton)
			{
				BackgroundCloseButton.onClick.AddListener(OnBackgroundClose);
			}
			InitBackButton();
			if ((bool)PackBuyButton)
			{
				PackBuyButton.gameObject.SetActive(value: false);
				_packBuyRect = (RectTransform)PackBuyButton.transform;
				_packBuyOriginalY = _packBuyRect.anchoredPosition.y;
			}
			if ((bool)PreviewText)
			{
				PreviewText.SetActive(value: false);
			}
			if ((bool)CancelButton)
			{
				CancelButton.gameObject.SetActive(value: false);
			}
			if ((bool)GridViewport)
			{
				_viewportOriginalBottom = GridViewport.offsetMin.y;
				_viewportBottomCaptured = true;
				_gridScroll = GridViewport.GetComponentInParent<ScrollRect>();
			}
			if ((bool)Grid)
			{
				_inCar = V.Car != null || FlagsStorage.IsPassenger;
				_gender = ResolveGender();
				_playback = new EmotePlaybackController(_freeDriveProviderDep?.Value);
				_chatControl = _chatControlDep?.Value;
				if ((bool)_chatControl)
				{
					ChatControl chatControl = _chatControl;
					chatControl.OnChatOpened = (Action<RectTransform, bool>)Delegate.Combine(chatControl.OnChatOpened, new Action<RectTransform, bool>(OnChatOpened));
				}
				GlobalEventSystem.OnHudClicked?.Invoke();
				WireUi();
				RegisterNotices();
				if (!(_emojiCatalog == null))
				{
					_ = _emojiCatalog.Entries.Count;
				}
				bool flag = _context != null && _context.HideEmojiTab;
				if ((bool)EmojiTabToggle)
				{
					EmojiTabToggle.gameObject.SetActive(!flag);
				}
				if ((bool)MovesTabToggle && _inCar)
				{
					MovesTabToggle.interactable = false;
				}
				RestoreLastFilters();
				string text = PlayerPrefs.GetString("emoteLockerLastTab", "moves");
				bool emoji = !flag && (_inCar || text == "emoji");
				_ = _inCar & flag;
				SelectTab(emoji, persist: false);
				TryOpenRequestedShopTab();
			}
		}

		private void TryOpenRequestedShopTab()
		{
			EmoteLockerContext context = _context;
			if (context != null && context.OpenPickerTab.HasValue && !_inCar)
			{
				_movesFilter = ((_context.OpenPickerTab == AnimationShopTab.Poses) ? MovesFilter.Poses : MovesFilter.DanceAnimations);
				SelectTab(emoji: false, persist: false);
				FocusRequestedBundle();
			}
		}

		private void FocusRequestedBundle()
		{
			int focusBundleId = _context.FocusBundleId;
			if (focusBundleId < 0)
			{
				return;
			}
			foreach (IEmoteEntry item in _catalog.GridOrder)
			{
				if (item.BundleId != focusBundleId)
				{
					continue;
				}
				EmoteLockerItem emoteLockerItem = Grid.FindCard(item.Id);
				if ((bool)emoteLockerItem)
				{
					if (IsOwned(item))
					{
						ScrollCardIntoView(emoteLockerItem);
					}
					else
					{
						OnPickerCardTapped(emoteLockerItem);
					}
					break;
				}
			}
		}

		protected override void OnDestroy()
		{
			_scrollToSelectedTween?.Kill();
			StopCurrentPreview();
			if ((bool)_chatControl)
			{
				ChatControl chatControl = _chatControl;
				chatControl.OnChatOpened = (Action<RectTransform, bool>)Delegate.Remove(chatControl.OnChatOpened, new Action<RectTransform, bool>(OnChatOpened));
			}
			_favorites?.SaveChangesOnClose();
			_playback?.Dispose();
			base.OnDestroy();
		}

		private static int ResolveGender()
		{
			if (!V.DriverPerson)
			{
				return -1;
			}
			MainPerson component = V.DriverPerson.GetComponent<MainPerson>();
			if (!component)
			{
				return -1;
			}
			return component.Gender;
		}

		private void WireUi()
		{
			Grid.CardTapped += OnCardTapped;
			if ((bool)EmojiGrid && EmojiGrid != Grid)
			{
				EmojiGrid.CardTapped += OnCardTapped;
			}
			if ((bool)MovesTabToggle)
			{
				MovesTabToggle.onValueChanged.AddListener((bool on) =>
				{
					OnTabToggle(on, emoji: false);
				});
			}
			if ((bool)EmojiTabToggle)
			{
				EmojiTabToggle.onValueChanged.AddListener((bool on) =>
				{
					OnTabToggle(on, emoji: true);
				});
			}
			if ((bool)FavoritesFilterToggle)
			{
				FavoritesFilterToggle.onValueChanged.AddListener((bool on) =>
				{
					OnMovesFilterToggle(on, MovesFilter.Favorites);
				});
			}
			if ((bool)ClipsFilterToggle)
			{
				ClipsFilterToggle.onValueChanged.AddListener((bool on) =>
				{
					OnMovesFilterToggle(on, MovesFilter.CoinAnimations);
				});
			}
			if ((bool)DanceFilterToggle)
			{
				DanceFilterToggle.onValueChanged.AddListener((bool on) =>
				{
					OnMovesFilterToggle(on, MovesFilter.DanceAnimations);
				});
			}
			if ((bool)PosesFilterToggle)
			{
				PosesFilterToggle.onValueChanged.AddListener((bool on) =>
				{
					OnMovesFilterToggle(on, MovesFilter.Poses);
				});
			}
			if ((bool)EmojiAllFilterToggle)
			{
				EmojiAllFilterToggle.onValueChanged.AddListener((bool on) =>
				{
					OnEmojiFilterToggle(on, favoritesOnly: false);
				});
			}
			if ((bool)EmojiFavoritesFilterToggle)
			{
				EmojiFavoritesFilterToggle.onValueChanged.AddListener((bool on) =>
				{
					OnEmojiFilterToggle(on, favoritesOnly: true);
				});
			}
			if ((bool)AddButton)
			{
				AddButton.onClick.AddListener(() =>
				{
					SwitchMode(Mode.FavAdd);
				});
			}
			if ((bool)RemoveButton)
			{
				RemoveButton.onClick.AddListener(() =>
				{
					SwitchMode(Mode.FavRemove);
				});
			}
			if ((bool)EmojiAddButton)
			{
				EmojiAddButton.onClick.AddListener(() =>
				{
					SwitchMode(Mode.EmojiFavAdd);
				});
			}
			if ((bool)EmojiRemoveButton)
			{
				EmojiRemoveButton.onClick.AddListener(() =>
				{
					SwitchMode(Mode.EmojiFavRemove);
				});
			}
			if ((bool)ActionButton)
			{
				ActionButton.onClick.AddListener(OnActionButton);
			}
			if ((bool)PackBuyButton)
			{
				PackBuyButton.onClick.AddListener(OnPackBuyButton);
			}
			if ((bool)CancelButton)
			{
				CancelButton.onClick.AddListener(OnCancelButton);
			}
		}

		private void RegisterNotices()
		{
			_notices.RegisterCurrentContent(11, CollectCatalogIds(AnimationShopTab.Animations));
			_notices.RegisterCurrentContent(12, CollectCatalogIds(AnimationShopTab.Poses));
		}

		private List<int> CollectCatalogIds(AnimationShopTab tab)
		{
			List<int> list = new List<int>();
			foreach (IEmoteEntry entry in _catalog.Entries)
			{
				if (entry.Id != 0 && entry.ShopTab == tab)
				{
					list.Add(entry.Id);
				}
			}
			return list;
		}

		private void OnChatOpened(RectTransform chatRect, bool open)
		{
			if (open)
			{
				Close();
			}
		}

		private void OnTabToggle(bool on, bool emoji)
		{
			if (on && !_wiringUi && emoji != IsEmojiSection)
			{
				SelectTab(emoji, persist: true);
			}
		}

		private void SelectTab(bool emoji, bool persist)
		{
			_wiringUi = true;
			if ((bool)MovesTabToggle)
			{
				MovesTabToggle.SetIsOnWithoutNotify(!emoji);
			}
			if ((bool)EmojiTabToggle)
			{
				EmojiTabToggle.SetIsOnWithoutNotify(emoji);
			}
			SyncFilterToggles();
			_wiringUi = false;
			if (persist)
			{
				PlayerPrefs.SetString("emoteLockerLastTab", emoji ? "emoji" : "moves");
				PlayerPrefs.Save();
			}
			if (!emoji)
			{
				MarkFilterRead(_movesFilter);
			}
			SwitchMode(emoji ? Mode.Emoji : Mode.Shop);
		}

		private void SyncFilterToggles()
		{
			if ((bool)FavoritesFilterToggle)
			{
				FavoritesFilterToggle.SetIsOnWithoutNotify(_movesFilter == MovesFilter.Favorites);
			}
			if ((bool)ClipsFilterToggle)
			{
				ClipsFilterToggle.SetIsOnWithoutNotify(_movesFilter == MovesFilter.CoinAnimations);
			}
			if ((bool)DanceFilterToggle)
			{
				DanceFilterToggle.SetIsOnWithoutNotify(_movesFilter == MovesFilter.DanceAnimations);
			}
			if ((bool)PosesFilterToggle)
			{
				PosesFilterToggle.SetIsOnWithoutNotify(_movesFilter == MovesFilter.Poses);
			}
			if ((bool)EmojiAllFilterToggle)
			{
				EmojiAllFilterToggle.SetIsOnWithoutNotify(!_emojiFavoritesOnly);
			}
			if ((bool)EmojiFavoritesFilterToggle)
			{
				EmojiFavoritesFilterToggle.SetIsOnWithoutNotify(_emojiFavoritesOnly);
			}
		}

		private void RestoreLastFilters()
		{
			int num = PlayerPrefs.GetInt("emoteLockerLastMovesFilter", 1);
			if (num >= 0 && num <= 3)
			{
				_movesFilter = (MovesFilter)num;
			}
			_emojiFavoritesOnly = PlayerPrefs.GetInt("emoteLockerLastEmojiFilter", 0) == 1;
		}

		private void OnMovesFilterToggle(bool on, MovesFilter filter)
		{
			if (on && !_wiringUi && _movesFilter != filter)
			{
				_movesFilter = filter;
				PlayerPrefs.SetInt("emoteLockerLastMovesFilter", (int)filter);
				PlayerPrefs.Save();
				ResetPickerState();
				MarkFilterRead(filter);
				Rebuild();
			}
		}

		private void OnEmojiFilterToggle(bool on, bool favoritesOnly)
		{
			if (on && !_wiringUi && _emojiFavoritesOnly != favoritesOnly)
			{
				_emojiFavoritesOnly = favoritesOnly;
				PlayerPrefs.SetInt("emoteLockerLastEmojiFilter", favoritesOnly ? 1 : 0);
				PlayerPrefs.Save();
				Rebuild();
			}
		}

		private void MarkFilterRead(MovesFilter filter)
		{
			switch (filter)
			{
			case MovesFilter.CoinAnimations:
			case MovesFilter.DanceAnimations:
				_notices.MarkRead(11, CollectCatalogIds(AnimationShopTab.Animations));
				break;
			case MovesFilter.Poses:
				_notices.MarkRead(12, CollectCatalogIds(AnimationShopTab.Poses));
				break;
			}
		}

		private void SwitchMode(Mode mode)
		{
			ResetPickerState();
			_mode = mode;
			if (IsMovesEdit && _movesFilter == MovesFilter.Favorites)
			{
				_movesFilter = MovesFilter.CoinAnimations;
				SyncFilterToggles();
			}
			Rebuild();
		}

		private void Rebuild()
		{
			if ((bool)Grid)
			{
				List<int> list = new List<int>(_favorites.FavoriteMoves);
				HashSet<int> favoriteMoves = new HashSet<int>(list);
				HashSet<int> hashSet = new HashSet<int>(_favorites.FavoriteEmojis);
				EmoteLockerGrid activeGrid = ActiveGrid;
				ApplyActiveContent(activeGrid);
				activeGrid.BeginBuild();
				switch (_mode)
				{
				case Mode.Shop:
					BuildShopCards(favoriteMoves);
					break;
				case Mode.FavAdd:
					BuildFavAddCards(favoriteMoves);
					break;
				case Mode.FavRemove:
					BuildFavRemoveCards(list, favoriteMoves);
					break;
				case Mode.Emoji:
				case Mode.EmojiFavAdd:
				case Mode.EmojiFavRemove:
					BuildEmojiCards(hashSet);
					break;
				}
				activeGrid.EndBuild();
				RefreshPanels();
				RefreshButtons(list.Count, hashSet.Count);
				RefreshNoticeDots();
			}
		}

		private void ApplyActiveContent(EmoteLockerGrid activeGrid)
		{
			if ((bool)EmojiGrid && !(EmojiGrid == Grid))
			{
				bool flag = activeGrid == EmojiGrid;
				if ((bool)Grid.ContentRoot)
				{
					Grid.ContentRoot.SetActive(!flag);
				}
				if ((bool)EmojiGrid.ContentRoot)
				{
					EmojiGrid.ContentRoot.SetActive(flag);
				}
				if ((bool)_gridScroll && (bool)activeGrid.ContentRect)
				{
					_gridScroll.content = activeGrid.ContentRect;
				}
			}
		}

		private bool PassesMovesFilter(IEmoteEntry entry, HashSet<int> favoriteMoves)
		{
			switch (_movesFilter)
			{
			case MovesFilter.Favorites:
				return favoriteMoves?.Contains(entry.Id) ?? false;
			case MovesFilter.CoinAnimations:
				if (entry.ShopTab == AnimationShopTab.Animations)
				{
					return entry.BuyFlow == EmoteBuyFlow.Coins;
				}
				return false;
			case MovesFilter.DanceAnimations:
				if (entry.ShopTab == AnimationShopTab.Animations)
				{
					return entry.BuyFlow == EmoteBuyFlow.Bundle;
				}
				return false;
			default:
				return entry.ShopTab == AnimationShopTab.Poses;
			}
		}

		private void BuildShopCards(HashSet<int> favoriteMoves)
		{
			foreach (IEmoteEntry item in _catalog.GridOrder)
			{
				if (item.Id == 0 || !PassesMovesFilter(item, favoriteMoves))
				{
					continue;
				}
				bool flag = _gender >= 0 && !PoseAudienceRules.Matches(item.Audience, _gender);
				if (!flag || item.GenderMismatchPolicy != GenderMismatchPolicy.Hide)
				{
					EmoteLockerItem emoteLockerItem = Grid.AddCard(item.Id, item.Sprite);
					if (emoteLockerItem == null)
					{
						break;
					}
					bool flag2 = IsOwned(item);
					emoteLockerItem.SetFavorite(favoriteMoves.Contains(item.Id));
					emoteLockerItem.SetOwnedBadge(flag2);
					emoteLockerItem.SetCoinBadge(!flag2 && item.BuyFlow == EmoteBuyFlow.Coins);
					TrackSelectionOnCard(emoteLockerItem, item.Id);
					if (flag)
					{
						emoteLockerItem.SetDisabled(disabled: true);
					}
				}
			}
		}

		private void BuildFavAddCards(HashSet<int> favoriteMoves)
		{
			foreach (IEmoteEntry item in _catalog.GridOrder)
			{
				if (item.Id == 0 || !IsOwned(item) || favoriteMoves.Contains(item.Id) || !PassesMovesFilter(item, favoriteMoves))
				{
					continue;
				}
				bool flag = _gender >= 0 && !PoseAudienceRules.Matches(item.Audience, _gender);
				if (!flag || item.GenderMismatchPolicy != GenderMismatchPolicy.Hide)
				{
					EmoteLockerItem emoteLockerItem = Grid.AddCard(item.Id, item.Sprite);
					if (emoteLockerItem == null)
					{
						break;
					}
					ApplyEditSelection(emoteLockerItem, item.Id);
					if (flag)
					{
						emoteLockerItem.SetDisabled(disabled: true);
					}
				}
			}
		}

		private void BuildFavRemoveCards(List<int> favoriteMovesList, HashSet<int> favoriteMoves)
		{
			foreach (int favoriteMoves2 in favoriteMovesList)
			{
				IEmoteEntry byId = _catalog.GetById(favoriteMoves2);
				if (byId != null && PassesMovesFilter(byId, favoriteMoves))
				{
					EmoteLockerItem emoteLockerItem = Grid.AddCard(byId.Id, byId.Sprite);
					if (emoteLockerItem == null)
					{
						break;
					}
					emoteLockerItem.SetFavorite(value: true);
					ApplyEditSelection(emoteLockerItem, byId.Id);
				}
			}
		}

		private void TrackSelectionOnCard(EmoteLockerItem card, int id)
		{
			bool flag = id == _selectedPickerId;
			card.SetSelected(flag);
			if (flag)
			{
				_selectedPickerCard = card;
			}
		}

		private void ApplyEditSelection(EmoteLockerItem card, int id)
		{
			card.SetSelected(_editSelection.Contains(id));
			if (id == _selectedPickerId)
			{
				_selectedPickerCard = card;
			}
		}

		private void BuildEmojiCards(HashSet<int> favoriteEmojis)
		{
			if (_emojiCatalog == null)
			{
				return;
			}
			foreach (EmojiCatalogSO.EmojiEntry entry in _emojiCatalog.Entries)
			{
				if (entry == null)
				{
					continue;
				}
				bool flag = favoriteEmojis.Contains(entry.Id);
				if (_mode switch
				{
					Mode.EmojiFavAdd => !flag, 
					Mode.EmojiFavRemove => flag, 
					_ => !_emojiFavoritesOnly | flag, 
				})
				{
					EmoteLockerItem emoteLockerItem = ActiveGrid.AddCard(entry.Id, entry.Sprite);
					if (emoteLockerItem == null)
					{
						break;
					}
					emoteLockerItem.SetFavorite(flag);
					if (IsEmojiEdit)
					{
						ApplyEditSelection(emoteLockerItem, entry.Id);
					}
				}
			}
		}

		private static bool IsOwned(IEmoteEntry entry)
		{
			if (!entry.IsOwned)
			{
				if (entry.BuyFlow == EmoteBuyFlow.Coins)
				{
					return entry.Price == 0;
				}
				return false;
			}
			return true;
		}

		private IEmoteEntry FindUnownedDanceEntry()
		{
			if (_catalog == null)
			{
				return null;
			}
			IEmoteEntry emoteEntry = null;
			foreach (IEmoteEntry item in _catalog.GridOrder)
			{
				if (item.Id != 0 && item.ShopTab == AnimationShopTab.Animations && item.BuyFlow == EmoteBuyFlow.Bundle && item.BundleId >= 0 && !IsOwned(item) && (_gender < 0 || PoseAudienceRules.Matches(item.Audience, _gender) || item.GenderMismatchPolicy != GenderMismatchPolicy.Hide))
				{
					if (_bundleOfferCatalog != null && _bundleOfferCatalog.TryGetActiveOfferForAnimationBundle(item.BundleId, out var _))
					{
						return item;
					}
					if (emoteEntry == null)
					{
						emoteEntry = item;
					}
				}
			}
			return emoteEntry;
		}

		private void RefreshPanels()
		{
			bool isEmojiSection = IsEmojiSection;
			if ((bool)MovesFilterPanel)
			{
				MovesFilterPanel.SetActive(!isEmojiSection);
			}
			if ((bool)EmojiFilterPanel)
			{
				EmojiFilterPanel.SetActive(_mode == Mode.Emoji);
			}
			if ((bool)FavoritesFilterToggle)
			{
				FavoritesFilterToggle.gameObject.SetActive(!IsMovesEdit);
			}
		}

		private void RefreshButtons(int favoriteMovesCount, int favoriteEmojisCount)
		{
			bool flag = _mode == Mode.Shop && _movesFilter == MovesFilter.Favorites;
			if ((bool)AddButton)
			{
				AddButton.gameObject.SetActive(flag);
				if (flag)
				{
					AddButton.transform.SetAsLastSibling();
				}
			}
			if ((bool)RemoveButton)
			{
				bool flag2 = flag && favoriteMovesCount > 0;
				RemoveButton.gameObject.SetActive(flag2);
				if (flag2)
				{
					RemoveButton.transform.SetAsLastSibling();
				}
			}
			bool flag3 = _mode == Mode.Emoji && _emojiFavoritesOnly;
			if ((bool)EmojiAddButton)
			{
				EmojiAddButton.gameObject.SetActive(flag3);
				if (flag3)
				{
					EmojiAddButton.transform.SetAsLastSibling();
				}
			}
			if ((bool)EmojiRemoveButton)
			{
				bool flag4 = flag3 && favoriteEmojisCount > 0;
				EmojiRemoveButton.gameObject.SetActive(flag4);
				if (flag4)
				{
					EmojiRemoveButton.transform.SetAsLastSibling();
				}
			}
			RefreshActionButton();
		}

		private void RefreshActionButton()
		{
			bool flag = _mode == Mode.Shop && _selectedPickerEntry != null && !IsOwned(_selectedPickerEntry) && _selectedPickerEntry.BuyFlow == EmoteBuyFlow.Bundle;
			if (!flag && IsDanceShopTab)
			{
				flag = FindUnownedDanceEntry() != null;
			}
			if ((bool)ActionButton)
			{
				switch (_mode)
				{
				case Mode.Shop:
					RefreshShopAction(flag);
					break;
				case Mode.FavAdd:
				case Mode.EmojiFavAdd:
					ActionButton.gameObject.SetActive(_editSelection.Count > 0);
					SetActionText(new LocalizedString("EmoteLocker.AddSelected"));
					break;
				case Mode.FavRemove:
				case Mode.EmojiFavRemove:
					ActionButton.gameObject.SetActive(_editSelection.Count > 0);
					SetActionText(new LocalizedString("EmoteLocker.RemoveSelected"));
					break;
				default:
					ActionButton.gameObject.SetActive(value: false);
					break;
				}
			}
			bool flag2 = _mode == Mode.Shop && _selectedPickerEntry != null && !IsOwned(_selectedPickerEntry);
			if ((bool)PackBuyButton)
			{
				PackBuyButton.gameObject.SetActive(flag);
			}
			if ((bool)PreviewText)
			{
				PreviewText.SetActive(flag2);
			}
			if ((bool)CancelButton)
			{
				CancelButton.gameObject.SetActive(IsEditMode && _editSelection.Count == 0);
			}
			ApplyPackBuyRaise(flag2);
			ApplyViewportBottom(flag, flag2);
			EnsureSelectedCardVisible();
		}

		private void RefreshShopAction(bool bundlePick)
		{
			IEmoteEntry selectedPickerEntry = _selectedPickerEntry;
			if ((selectedPickerEntry == null) | bundlePick)
			{
				ActionButton.gameObject.SetActive(value: false);
				return;
			}
			ActionButton.gameObject.SetActive(value: true);
			SetActionText(selectedPickerEntry.Price.ToString());
		}

		private void ApplyPackBuyRaise(bool previewTextShown)
		{
			if ((bool)_packBuyRect)
			{
				Vector2 anchoredPosition = _packBuyRect.anchoredPosition;
				anchoredPosition.y = (previewTextShown ? (_packBuyOriginalY + 55.6f) : _packBuyOriginalY);
				_packBuyRect.anchoredPosition = anchoredPosition;
			}
		}

		private void ApplyViewportBottom(bool packBuyShown, bool previewTextShown)
		{
			if ((bool)GridViewport && _viewportBottomCaptured)
			{
				float y = _viewportOriginalBottom;
				if (packBuyShown & previewTextShown)
				{
					y = 195.6f;
				}
				else if (packBuyShown)
				{
					y = 140f;
				}
				else if (previewTextShown)
				{
					y = 80f;
				}
				Vector2 offsetMin = GridViewport.offsetMin;
				offsetMin.y = y;
				GridViewport.offsetMin = offsetMin;
			}
		}

		private void EnsureSelectedCardVisible()
		{
			if ((bool)_selectedPickerCard)
			{
				ScrollCardIntoView(_selectedPickerCard);
			}
		}

		private void ScrollCardIntoView(EmoteLockerItem targetCard)
		{
			if ((bool)targetCard && (bool)GridViewport && (bool)_gridScroll && (bool)_gridScroll.content)
			{
				Canvas.ForceUpdateCanvases();
				RectTransform obj = (RectTransform)targetCard.transform;
				Vector3[] array = new Vector3[4];
				obj.GetWorldCorners(array);
				float num = float.MaxValue;
				float num2 = float.MinValue;
				for (int i = 0; i < array.Length; i++)
				{
					float y = GridViewport.InverseTransformPoint(array[i]).y;
					num = Mathf.Min(num, y);
					num2 = Mathf.Max(num2, y);
				}
				Rect rect = GridViewport.rect;
				float num3 = rect.yMin + 10f - num;
				float num4 = num2 - (rect.yMax - 10f);
				if (!(num3 <= 0f) || !(num4 <= 0f))
				{
					RectTransform content = _gridScroll.content;
					float value = content.anchoredPosition.y + ((num3 > 0f) ? num3 : (0f - num4));
					float max = Mathf.Max(0f, content.rect.height - rect.height);
					value = Mathf.Clamp(value, 0f, max);
					_gridScroll.velocity = Vector2.zero;
					_scrollToSelectedTween?.Kill();
					_scrollToSelectedTween = content.DOAnchorPosY(value, 0.25f).SetEase(Ease.OutCubic).SetUpdate(isIndependentUpdate: true);
				}
			}
		}

		private void SetActionText(string text)
		{
			if ((bool)ActionButtonText)
			{
				ActionButtonText.text = text;
			}
		}

		private void RefreshNoticeDots()
		{
			bool active = _notices.HasNotice(11);
			if ((bool)ClipsNoticeDot)
			{
				ClipsNoticeDot.SetActive(active);
			}
			if ((bool)DanceNoticeDot)
			{
				DanceNoticeDot.SetActive(active);
			}
			if ((bool)PosesNoticeDot)
			{
				PosesNoticeDot.SetActive(_notices.HasNotice(12));
			}
		}

		private void OnCardTapped(EmoteLockerItem item)
		{
			if (_closing)
			{
				return;
			}
			switch (_mode)
			{
			case Mode.Shop:
				OnShopCardTapped(item);
				break;
			case Mode.FavAdd:
			case Mode.FavRemove:
				OnMovesEditCardTapped(item);
				break;
			case Mode.Emoji:
				if (_playback != null && _playback.TryPlayEmoji(item.Id))
				{
					_closing = true;
					Close();
				}
				break;
			case Mode.EmojiFavAdd:
			case Mode.EmojiFavRemove:
				OnEmojiEditCardTapped(item);
				break;
			}
		}

		private void OnShopCardTapped(EmoteLockerItem item)
		{
			IEmoteEntry byId = _catalog.GetById(item.Id);
			if (byId == null)
			{
				return;
			}
			if (IsOwned(byId))
			{
				ResetPickerState();
				if (_playback != null && _playback.TryPlayMove(byId, this))
				{
					_closing = true;
					Close();
				}
				else
				{
					RefreshActionButton();
				}
			}
			else
			{
				OnPickerCardTapped(item);
			}
		}

		private void OnPickerCardTapped(EmoteLockerItem item)
		{
			if (_selectedPickerId == item.Id)
			{
				ResetPickerState();
				RefreshActionButton();
				return;
			}
			IEmoteEntry byId = _catalog.GetById(item.Id);
			if (byId != null)
			{
				if ((bool)_selectedPickerCard)
				{
					_selectedPickerCard.SetSelected(value: false);
				}
				item.SetSelected(value: true);
				StartPreview(byId, item);
				RefreshActionButton();
			}
		}

		private void OnMovesEditCardTapped(EmoteLockerItem item)
		{
			if (_editSelection.Remove(item.Id))
			{
				item.SetSelected(value: false);
				if (_selectedPickerId == item.Id)
				{
					StopCurrentPreview();
					ClearPreviewTracking();
				}
				RefreshActionButton();
				return;
			}
			IEmoteEntry byId = _catalog.GetById(item.Id);
			if (byId != null)
			{
				_editSelection.Add(item.Id);
				item.SetSelected(value: true);
				StartPreview(byId, item);
				RefreshActionButton();
			}
		}

		private void StartPreview(IEmoteEntry entry, EmoteLockerItem card)
		{
			_previewAutoStop?.Dispose();
			_previewAutoStop = null;
			EmotePreviewContext ctx = PreviewContext();
			if (_selectedPickerEntry != null)
			{
				_selectedPickerEntry.StopPreview(in ctx);
			}
			_selectedPickerId = entry.Id;
			_selectedPickerEntry = entry;
			_selectedPickerCard = card;
			entry.Preview(in ctx);
			float? previewAutoStopSeconds = entry.PreviewAutoStopSeconds;
			if (previewAutoStopSeconds.HasValue)
			{
				float valueOrDefault = previewAutoStopSeconds.GetValueOrDefault();
				_previewAutoStop = Observable.Timer(TimeSpan.FromSeconds(valueOrDefault)).Subscribe((long _) =>
				{
					TurnOffPreview();
				}).AddTo(this);
			}
		}

		private void OnEmojiEditCardTapped(EmoteLockerItem item)
		{
			if (_editSelection.Remove(item.Id))
			{
				item.SetSelected(value: false);
				if (_selectedPickerCard == item)
				{
					_selectedPickerCard = null;
				}
			}
			else
			{
				_editSelection.Add(item.Id);
				item.SetSelected(value: true);
				_selectedPickerCard = item;
			}
			RefreshActionButton();
		}

		private EmotePreviewContext PreviewContext()
		{
			PersonAnimationsController animations = null;
			WorldInteractionController wic = null;
			NewMoveController move = null;
			if ((bool)V.DriverPerson)
			{
				MainPerson component = V.DriverPerson.GetComponent<MainPerson>();
				animations = (component ? component.PersonAnimationsController : null);
				wic = V.DriverPerson.GetComponent<WorldInteractionController>();
				move = V.DriverPerson.GetComponent<NewMoveController>();
			}
			return new EmotePreviewContext(animations, wic, move, isMainMenu: false, FlagsStorage.IsMultiFree, RagdollCPM.IsDriverRagdolled(), _turnOffAction);
		}

		private void StopCurrentPreview()
		{
			_previewAutoStop?.Dispose();
			_previewAutoStop = null;
			if (_selectedPickerEntry != null)
			{
				EmotePreviewContext ctx = PreviewContext();
				_selectedPickerEntry.StopPreview(in ctx);
				StopGenericPreview(in ctx);
			}
		}

		private void TurnOffPreview()
		{
			EmotePreviewContext ctx = PreviewContext();
			if (_selectedPickerEntry != null)
			{
				if (_selectedPickerEntry.KeepsRunningOnTurnOff)
				{
					return;
				}
				_selectedPickerEntry.StopPreview(in ctx);
			}
			StopGenericPreview(in ctx);
		}

		private static void StopGenericPreview(in EmotePreviewContext ctx)
		{
			if ((bool)ctx.Animations)
			{
				ctx.Animations.StopSpecialAnimation();
			}
			if ((bool)ctx.Move)
			{
				ctx.Move.Input.Emotion = 0;
			}
		}

		private void ClearPickerSelection()
		{
			if ((bool)_selectedPickerCard)
			{
				_selectedPickerCard.SetSelected(value: false);
			}
			ClearPreviewTracking();
			_editSelection.Clear();
		}

		private void ClearPreviewTracking()
		{
			_selectedPickerId = -1;
			_selectedPickerEntry = null;
			_selectedPickerCard = null;
		}

		private void ResetPickerState()
		{
			_scrollToSelectedTween?.Kill();
			StopCurrentPreview();
			ClearPickerSelection();
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			if (IsMovesEdit)
			{
				SwitchMode(Mode.Shop);
				InitBackButton();
			}
			else if (IsEmojiEdit)
			{
				SwitchMode(Mode.Emoji);
				InitBackButton();
			}
			else
			{
				Close();
			}
		}

		private void OnBackgroundClose()
		{
			Close();
		}

		private void OnActionButton()
		{
			switch (_mode)
			{
			case Mode.Shop:
				if (_selectedPickerEntry != null && !IsOwned(_selectedPickerEntry) && _selectedPickerEntry.BuyFlow == EmoteBuyFlow.Coins)
				{
					ConfirmCoinsPurchase(_selectedPickerEntry);
				}
				break;
			case Mode.FavAdd:
				CommitMoveFavorite(add: true);
				break;
			case Mode.FavRemove:
				CommitMoveFavorite(add: false);
				break;
			case Mode.EmojiFavAdd:
				CommitEmojiFavorite(add: true);
				break;
			case Mode.EmojiFavRemove:
				CommitEmojiFavorite(add: false);
				break;
			case Mode.Emoji:
				break;
			}
		}

		private void CommitMoveFavorite(bool add)
		{
			if (_editSelection.Count == 0)
			{
				return;
			}
			List<int> list = new List<int>(_editSelection);
			ResetPickerState();
			foreach (int item in list)
			{
				if (_favorites.IsFavoriteMove(item) != add)
				{
					_favorites.ToggleMove(item);
				}
			}
			SwitchMode(Mode.Shop);
		}

		private void CommitEmojiFavorite(bool add)
		{
			if (_editSelection.Count == 0)
			{
				return;
			}
			List<int> list = new List<int>(_editSelection);
			ClearPickerSelection();
			foreach (int item in list)
			{
				if (_favorites.IsFavoriteEmoji(item) != add)
				{
					_favorites.ToggleEmoji(item);
				}
			}
			SwitchMode(Mode.Emoji);
		}

		private void OnPackBuyButton()
		{
			if (_mode == Mode.Shop)
			{
				IEmoteEntry emoteEntry = _selectedPickerEntry ?? (IsDanceShopTab ? FindUnownedDanceEntry() : null);
				if (emoteEntry != null)
				{
					BuySelectedViaBundle(emoteEntry);
				}
			}
		}

		private void OnCancelButton()
		{
			if (IsMovesEdit)
			{
				SwitchMode(Mode.Shop);
			}
			else if (IsEmojiEdit)
			{
				SwitchMode(Mode.Emoji);
			}
		}

		private void ConfirmCoinsPurchase(IEmoteEntry entry)
		{
			int id = entry.Id;
			int price = entry.Price;
			_managerUI.ShowConfirmWindow(() =>
			{
				BuyForCoins(id, price).Forget();
			}, new LocalizedString("Playerprofilewindow.Message"), string.Format(new LocalizedString("VisualTuningWindow.Buythisitemfor"), price)).Forget();
		}

		private async UniTaskVoid BuyForCoins(int id, int price)
		{
			await _currencyManagerController.TryCoinsSpend(SpendCoinsType.Animations, new AdvancedInt(price), () =>
			{
				AnimationShop.SetAnimationOwned(id, owned: true);
				_managerUI.ShowNotificationWindow(new LocalizedString("AnimationShop.Success"), 2f).Forget();
				if ((bool)this)
				{
					ResetPickerState();
					Rebuild();
				}
			});
		}

		private async UniTask BuySelectedViaBundle(IEmoteEntry entry)
		{
			if (!((entry.ShopTab == AnimationShopTab.Poses) ? _bundleOfferCatalog.TryGetActiveOfferForIkBundle(entry.BundleId, out var offer) : _bundleOfferCatalog.TryGetActiveOfferForAnimationBundle(entry.BundleId, out offer)))
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("SuitPack.PackNotAvailable"), 2f).Forget();
				return;
			}
			ResetPickerState();
			Func<bool> func = _context?.ExitWalkForBanner;
			if (func == null || (func.Target is UnityEngine.Object obj && !obj) || func())
			{
				await _managerUI.ShowBundleOfferWindow(offer, "EmoteLockerWindow");
			}
			else
			{
				Rebuild();
			}
		}
	}
}
