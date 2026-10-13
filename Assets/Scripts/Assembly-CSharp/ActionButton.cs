using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.UIElement;

public class ActionButton : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI Text;

	[SerializeField]
	private Button ActionBtn;

	[SerializeField]
	private ButtonSpiteUpdater ButtonSpiteUpdater;

	[SerializeField]
	private ButtonSpiteUpdater.SpriteData CoinSpriteData;

	[SerializeField]
	private ButtonSpiteUpdater.SpriteData MoneySpriteData;

	[SerializeField]
	private Image CoinImage;

	[SerializeField]
	private Sprite CoinSprite;

	[SerializeField]
	private Sprite MoneySprite;

	private bool _isPurchase;

	private FreeDriveProvider _freeDriveProvider;

	private EventRegistry _eventRegistry;

	private RegistryHandle<ScriptableEvent> _interactHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Interact);

	[Inject]
	public void Construct(EventRegistry eventRegistry, FreeDriveProvider freeDriveProvider)
	{
		_eventRegistry = eventRegistry;
		_freeDriveProvider = freeDriveProvider;
	}

	public void Init(string textToSet, bool purchase = false, CostType costType = CostType.Money)
	{
		Text.text = textToSet;
		if ((bool)CoinImage && (bool)CoinSprite && (bool)MoneySprite && (bool)ButtonSpiteUpdater)
		{
			switch (costType)
			{
			case CostType.Money:
				CoinImage.sprite = MoneySprite;
				ButtonSpiteUpdater.UpdateSprite(MoneySpriteData);
				break;
			case CostType.Coins:
				CoinImage.sprite = CoinSprite;
				ButtonSpiteUpdater.UpdateSprite(CoinSpriteData);
				break;
			}
		}
		CoinImage.gameObject.SetActive(purchase);
		if (purchase)
		{
			DestroyAfter().Forget();
		}
	}

	private void OnEnable()
	{
		_interactHandle.Get(_eventRegistry).Subscribe(AcceptDelegate);
	}

	private void OnDisable()
	{
		_interactHandle.Get(_eventRegistry)?.Unsubscribe(AcceptDelegate);
		ActionBtn.onClick.RemoveAllListeners();
	}

	private void AcceptDelegate()
	{
		ActionBtn.onClick.Invoke();
	}

	private async UniTask DestroyAfter()
	{
		await UniTask.Delay(5000);
		if ((bool)this && (bool)base.gameObject)
		{
			Object.Destroy(base.gameObject);
		}
		if ((bool)V.Car && V.Car.GetComponent<VehicleData>().CarOnLift && (bool)_freeDriveProvider.FreeDriveControl)
		{
			_freeDriveProvider.FreeDriveControl.ShowActionButton(1);
		}
	}

	public Button GetActionButton()
	{
		return ActionBtn;
	}
}
