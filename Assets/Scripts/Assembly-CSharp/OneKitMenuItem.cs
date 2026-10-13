using System;
using System.Threading;
using CPM.Database;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;

public class OneKitMenuItem : MonoBehaviour
{
	public enum AvailableToBuyType
	{
		Normal = 0,
		Locked = 1,
		EventEnded = 2
	}

	public RawImage kitTexture;

	public Image edge;

	public TextMeshProUGUI price;

	public Toggle SelectToggle;

	public GameObject ownedMark;

	[FormerlySerializedAs("ClothType")]
	public KitType kitType;

	public int id;

	public GameObject equipment;

	public Image CoinImage;

	[SerializeField]
	private PersonAvailableGameObjectActive PersonAvailableGameObjectActive;

	[SerializeField]
	private ClothItemAvailableGameObjectActive ClothItemAvailableGameObjectActive;

	[SerializeField]
	private ForceRebuildLayoutImmediate BottomForceRebuildLayoutImmediate;

	[SerializeField]
	private GameObject NoticeObject;

	[SerializeField]
	private GameObject SaleNoticeObject;

	[SerializeField]
	private Sprite SelectedSprite;

	[SerializeField]
	private Sprite DeselectedSprite;

	private MainPerson _mainPerson;

	private CharactersService _charactersService;

	private PersonElements.PersonKitData _personKitData;

	private ICharacterEquipmentDataService _characterEquipmentDataService;

	private PersonKitConfig _maleKitConfig;

	private PersonKitConfig _femaleKitConfig;

	private INotificationService _notificationService;

	private PersonClothesEventChecker _personClothesEventChecker;

	private PersonClothsEventType _personClothsEventType;

	private int _packId = -1;

	private bool _isPackItem;

	private CancellationTokenSource _checkEventCancellationTokenSource = new CancellationTokenSource();

	public PersonElements.PersonKitData PersonKitData => _personKitData;

	public int PackId => _packId;

	public event Action<OneKitMenuItem> OnSelectedOwnedKitItem;

	public event Action<OneKitMenuItem> OnSelected;

	[Inject]
	public void Construct(CharactersService charactersService, ICharacterEquipmentDataService characterEquipmentDataService, PersonKitConfigs personKitConfigs, INotificationService notificationService, PersonClothesEventChecker personClothesEventChecker)
	{
		_charactersService = charactersService;
		_characterEquipmentDataService = characterEquipmentDataService;
		_maleKitConfig = personKitConfigs.Male;
		_femaleKitConfig = personKitConfigs.Female;
		_notificationService = notificationService;
		_personClothesEventChecker = personClothesEventChecker;
	}

	private void Start()
	{
		SelectToggle = GetComponent<Toggle>();
		SelectToggle.onValueChanged.AddListener(OnPersonKitToggleValueChanged);
	}

	private void OnEnable()
	{
		if ((bool)_mainPerson)
		{
			checkState(_mainPerson.Gender);
		}
	}

	private void OnPersonKitToggleValueChanged(bool state)
	{
		if (state)
		{
			OnSelected?.Invoke(this);
			edge.sprite = SelectedSprite;
			_notificationService.MarkAsRead((int)kitType, id, _characterEquipmentDataService.Gender);
			SetNotice();
		}
		else
		{
			edge.sprite = DeselectedSprite;
		}
	}

	public void Init(PersonKitDataWrapper personKitDataWrapper, ToggleGroup toggleGroup, bool selected, MainPerson mainPerson, int id)
	{
		_mainPerson = mainPerson;
		_personKitData = personKitDataWrapper.PersonKitData;
		kitTexture.texture = null;
		SelectToggle.group = toggleGroup;
		edge.sprite = (selected ? SelectedSprite : DeselectedSprite);
		_personClothsEventType = personKitDataWrapper.PersonClothsEventType;
		_packId = personKitDataWrapper.PackId;
		_isPackItem = _packId >= 1;
		this.id = id;
		kitType = personKitDataWrapper.KitType;
		price.text = personKitDataWrapper.PersonKitData.obscuredPrice.ToString();
		Texture texture = Resources.Load<Texture>(CharactersService.GetIconPathForKit(_characterEquipmentDataService.Gender, (int)kitType, id));
		if ((bool)texture)
		{
			kitTexture.texture = texture;
		}
		checkState(mainPerson.Gender);
		SetNotice();
	}

	public void checkState(int gender)
	{
		if (kitType == KitType.None)
		{
			return;
		}
		EquipmentsData equipmentsData = _charactersService.GetEquipmentsData(gender);
		bool flag = equipmentsData.SelectedEquipments[(int)kitType] == id;
		int num = ((gender == 0) ? _femaleKitConfig.allKits[(int)kitType][id].obscuredPrice : _maleKitConfig.allKits[(int)kitType][id].obscuredPrice);
		bool flag2 = equipmentsData.IsItemBought((int)kitType, id);
		bool flag3 = (num == 0) | flag2;
		if (_isPackItem)
		{
			CoinImage.SetActive(active: false);
			if (flag2)
			{
				price.gameObject.SetActive(value: true);
				price.text = new LocalizedString("OneKitMenuItem.Purchased");
				ownedMark.gameObject.SetActive(flag);
				if (flag)
				{
					OnSelectedOwnedKitItem?.Invoke(this);
				}
			}
			else
			{
				price.gameObject.SetActive(value: true);
				price.text = new LocalizedString("SuitPack.Packs");
				ownedMark.gameObject.SetActive(value: false);
			}
		}
		else if (flag3)
		{
			CoinImage.SetActive(active: false);
			price.gameObject.SetActive(value: true);
			ownedMark.gameObject.SetActive(flag);
			if (flag)
			{
				OnSelectedOwnedKitItem?.Invoke(this);
			}
			price.text = new LocalizedString("OneKitMenuItem.Purchased");
		}
		else
		{
			CoinImage.SetActive(active: true);
			price.gameObject.SetActive(value: true);
			ownedMark.gameObject.SetActive(value: false);
			price.text = num.ToString();
			if (_personKitData.IsRewardKit)
			{
				base.gameObject.SetActive(value: false);
			}
		}
		if (_personKitData != null)
		{
			if ((bool)PersonAvailableGameObjectActive)
			{
				PersonAvailableGameObjectActive.UpdateState(_personKitData.AvailableType);
			}
			if ((bool)ClothItemAvailableGameObjectActive)
			{
				AvailableToBuyType type = ((!_isPackItem) ? _personKitData.AvailableToBuyStatus : AvailableToBuyType.Normal);
				ClothItemAvailableGameObjectActive.UpdateState(type);
			}
		}
		UpdateEventState(_personClothsEventType, _isPackItem ? flag2 : flag3);
		BottomForceRebuildLayoutImmediate.RebuildLayout();
	}

	private void UpdateEventState(PersonClothsEventType personClothsEventType, bool isOwned)
	{
		if ((bool)SaleNoticeObject)
		{
			SaleNoticeObject.SetActive(value: false);
		}
		if (isOwned || personClothsEventType == PersonClothsEventType.None)
		{
			return;
		}
		if (!_personClothesEventChecker.IsEventActive(personClothsEventType))
		{
			if ((bool)ClothItemAvailableGameObjectActive)
			{
				ClothItemAvailableGameObjectActive.UpdateState(AvailableToBuyType.EventEnded);
			}
		}
		else if ((bool)SaleNoticeObject)
		{
			SaleNoticeObject.SetActive(value: true);
		}
	}

	public void Clear()
	{
		kitTexture.texture = null;
		SelectToggle.group = null;
		id = -1;
		_packId = -1;
		_isPackItem = false;
		kitType = KitType.None;
		price.text = new LocalizedString("OneKitMenuItem.empty");
		kitTexture.texture = null;
	}

	private void SetNotice()
	{
		bool active = _notificationService.HasNotice((int)kitType, id, _characterEquipmentDataService.Gender);
		NoticeObject?.SetActive(active);
	}

	private void OnDestroy()
	{
		OnSelected = null;
		OnSelectedOwnedKitItem = null;
	}
}
