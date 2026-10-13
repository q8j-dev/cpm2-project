using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	public class VisualTuningFavouriteUI : MonoBehaviour
	{
		[SerializeField]
		protected VisualTuningFavouriteControllerBase VisualTuningFavouriteController;

		[SerializeField]
		protected ToggleSwitchHandler AddFavouriteSwitchHandler;

		[SerializeField]
		protected Toggle AddRemoveToFavouriteToggle;

		[SerializeField]
		private Toggle ShowFavouritesViewToggle = default;

		[SerializeField]
		private Toggle ShowAllViewToggle = default;

		[SerializeField]
		private TextMeshProUGUI CurrentFavouriteCountLabel = default;

		private EventRegistry _eventRegistry;

		private RegistryHandle<ScriptableEvent> _addFavoriteHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.AddFavorite);

		private RegistryHandle<ScriptableEvent> _toggleFavoritesHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleFavorites);

		[Inject]
		public void Construct(EventRegistry eventRegistry)
		{
			_eventRegistry = eventRegistry;
		}

		private void OnEnable()
		{
			_addFavoriteHandle.Get(_eventRegistry).Subscribe(ToggleSelectedFavorite);
			_toggleFavoritesHandle.Get(_eventRegistry).Subscribe(ToggleFavoritesViewContainer);
		}

		private void OnDisable()
		{
			_addFavoriteHandle.Get(_eventRegistry)?.Unsubscribe(ToggleSelectedFavorite);
			_toggleFavoritesHandle.Get(_eventRegistry)?.Unsubscribe(ToggleFavoritesViewContainer);
		}

		private void ToggleSelectedFavorite()
		{
			AddRemoveToFavouriteToggle.isOn = !AddRemoveToFavouriteToggle.isOn;
		}

		private void ToggleFavoritesViewContainer()
		{
			bool isOn = ShowFavouritesViewToggle.isOn;
			VisualTuningFavouriteController.SwitchViewContainers(isOn);
			ShowFavouritesViewToggle.SetIsOnWithoutNotify(!isOn);
			ShowAllViewToggle.SetIsOnWithoutNotify(isOn);
		}

		private void Awake()
		{
			RegisterEvents();
		}

		private void OnDestroy()
		{
			UnregisterEvents();
		}

		private void SetOnShowAllViewToggle()
		{
			ShowAllViewToggle.isOn = true;
		}

		private void AddRemoveItemToFavourite(bool isOn)
		{
			bool flag = VisualTuningFavouriteController.AddRemoveItemToFavourite(isOn);
			if (isOn && !flag)
			{
				AddRemoveToFavouriteToggle.SetIsOnWithoutNotify(value: false);
				Observable.NextFrame().Subscribe((Unit _) =>
				{
					AddFavouriteSwitchHandler.Switch(isOn: false);
				}).AddTo(this);
			}
		}

		protected virtual void RegisterEvents()
		{
			AddRemoveToFavouriteToggle.onValueChanged.AddListener(AddRemoveItemToFavourite);
			ShowFavouritesViewToggle.onValueChanged.AddListener(OnShowFavouriteItems);
			ShowAllViewToggle.onValueChanged.AddListener(OnShowAllItems);
			VisualTuningFavouriteController.OnFavouriteCountChanged += SetFavouriteCountText;
			VisualTuningFavouriteController.OnItemSelected += OnSelectItem;
			VisualTuningFavouriteController.OnResetToAllView += SetOnShowAllViewToggle;
		}

		protected virtual void UnregisterEvents()
		{
			AddRemoveToFavouriteToggle.onValueChanged.RemoveListener(AddRemoveItemToFavourite);
			ShowFavouritesViewToggle.onValueChanged.RemoveListener(OnShowFavouriteItems);
			ShowAllViewToggle.onValueChanged.RemoveListener(OnShowAllItems);
			VisualTuningFavouriteController.OnFavouriteCountChanged -= SetFavouriteCountText;
			VisualTuningFavouriteController.OnItemSelected -= OnSelectItem;
			VisualTuningFavouriteController.OnResetToAllView -= SetOnShowAllViewToggle;
		}

		private void OnShowFavouriteItems(bool isOn)
		{
			if (isOn)
			{
				VisualTuningFavouriteController.SwitchViewContainers(showAllItems: false);
			}
		}

		private void OnShowAllItems(bool isOn)
		{
			if (isOn)
			{
				VisualTuningFavouriteController.SwitchViewContainers(showAllItems: true);
			}
		}

		private void SetFavouriteCountText(int count)
		{
			CurrentFavouriteCountLabel.text = $"{Mathf.Clamp(count, 0, 10)} / {10}";
		}

		private void OnSelectItem(bool isFavourite)
		{
			AddRemoveToFavouriteToggle.SetIsOnWithoutNotify(isFavourite);
			AddFavouriteSwitchHandler.Switch(isFavourite);
		}
	}
}
