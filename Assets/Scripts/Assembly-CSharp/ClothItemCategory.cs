using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.Offers.Reward;

public class ClothItemCategory : MonoBehaviour
{
	[SerializeField]
	private GameObject NoticeObject;

	public Toggle Toggle;

	private ICharacterEquipmentDataService _characterEquipmentDataService;

	private PersonKitConfig _maleKitConfig;

	private PersonKitConfig _femaleKitConfig;

	private INotificationService _notificationService;

	private BundleOfferCatalog _bundleOfferCatalog;

	[field: SerializeField]
	public KitType KitType { get; private set; }

	[field: SerializeField]
	public PersonClothsEventType PersonClothsEventType { get; private set; }

	public event Action<KitType, PersonClothsEventType> OnSelectedClothType;

	[Inject]
	public void Construct(PersonKitConfigs personKitConfigs, ICharacterEquipmentDataService characterEquipmentDataService, INotificationService notificationService, BundleOfferCatalog bundleOfferCatalog)
	{
		_characterEquipmentDataService = characterEquipmentDataService;
		_femaleKitConfig = personKitConfigs.Female;
		_maleKitConfig = personKitConfigs.Male;
		_notificationService = notificationService;
		_bundleOfferCatalog = bundleOfferCatalog;
	}

	public void Init()
	{
		if (KitType == KitType.None)
		{
			Toggle.interactable = false;
			return;
		}
		Toggle.onValueChanged.AddListener(OnSelected);
		CheckState();
	}

	public void OnSelected(bool state)
	{
		if (state)
		{
			OnSelectedClothType?.Invoke(KitType, PersonClothsEventType);
		}
	}

	private void CheckState()
	{
		bool flag = _characterEquipmentDataService.Gender == 0;
		int num = (flag ? PlayerPrefs.GetInt("femaleClothCount_" + (int)KitType, 0) : PlayerPrefs.GetInt("maleClothCount_" + (int)KitType, 0));
		int num2 = (flag ? _femaleKitConfig.GetKitsByTypeId((int)KitType).Length : _maleKitConfig.GetKitsByTypeId((int)KitType).Length);
		if (num2 > num)
		{
			PlayerPrefs.SetInt(flag ? ("femaleClothCount_" + (int)KitType) : ("maleClothCount_" + (int)KitType), num2);
		}
	}

	private void OnEnable()
	{
		SetNotice();
		_notificationService.NoticeByTypeChanged += OnNoticeByTypeChanged;
	}

	private void OnDisable()
	{
		_notificationService.NoticeByTypeChanged -= OnNoticeByTypeChanged;
		Toggle.isOn = false;
	}

	private void OnNoticeByTypeChanged(int type, int gender)
	{
		if (type == (int)KitType && gender == _characterEquipmentDataService.Gender)
		{
			SetNotice();
		}
	}

	private void SetNotice()
	{
		int gender = _characterEquipmentDataService.Gender;
		bool active = _notificationService.AnyNotice(gender, (EquipmentNotice notice) => notice.Type == (int)KitType && !_bundleOfferCatalog.IsPackItem(gender, notice.Type, notice.Index));
		NoticeObject.SetActive(active);
	}

	private void OnDestroy()
	{
		Toggle.onValueChanged.RemoveAllListeners();
		OnSelectedClothType = null;
	}
}
