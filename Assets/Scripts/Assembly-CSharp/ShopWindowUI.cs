using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CarSlots;

public class ShopWindowUI : MonoBehaviour
{
	public GameObject oneProductCarPrefab;

	public GameObject oneProductOtherPrefab;

	public Transform carProductParent;

	public Transform personProductParent;

	public oneProductShopData[] allProductCars;

	public oneProductShopData[] allProductPersons;

	public GameObject oneProductSelectedWindow;

	public GameObject allParent;

	public oneProductShopData lastSelected;

	public List<int> allHashes;

	public int forCarCount;

	public int forPersonCount;

	private int counterAllProduct;

	private int counerPurchasedProduct;

	public int prevMenuIndex;

	private float lastCarScrollValue;

	private float lastOtherScrollValue;

	private AudioSource source;

	private MultiPlayerMenu multiPlayerMenu;

	private CharactersService _charactersService;

	private PersonKitConfig _malePersonKitConfig;

	private PersonKitConfig _femalePersonKitConfig;

	private CurrencyManagerController _currencyManagerController;

	private ISlotService _slotService;

	[Inject]
	public void Construct(MultiPlayerMenu multiPlayerMenu, CharactersService charactersService, PersonKitConfigs personKitConfigs, CurrencyManagerController currencyManagerController, ISlotService slotService)
	{
		_slotService = slotService;
		_currencyManagerController = currencyManagerController;
		_charactersService = charactersService;
		this.multiPlayerMenu = multiPlayerMenu;
		_malePersonKitConfig = personKitConfigs.Male;
		_femalePersonKitConfig = personKitConfigs.Female;
	}

	private void Awake()
	{
		source = GetComponent<AudioSource>();
		allHashes = new List<int>();
		oneProductShopData[] array = allProductCars;
		foreach (oneProductShopData oneProductShopData2 in array)
		{
			string idPrefs = oneProductShopData2.idPrefs;
			ObscuredInt coinPrice = oneProductShopData2.coinPrice;
			string text = idPrefs + coinPrice.ToString() + "1122";
			allHashes.Add(text.GetHashCode());
		}
		array = allProductPersons;
		foreach (oneProductShopData oneProductShopData3 in array)
		{
			string idPrefs2 = oneProductShopData3.idPrefs;
			ObscuredInt coinPrice = oneProductShopData3.coinPrice;
			string text2 = idPrefs2 + coinPrice.ToString() + "1122";
			allHashes.Add(text2.GetHashCode());
		}
		int num = 0;
		PersonElements.PersonKitData[][] allKits = _malePersonKitConfig.allKits;
		foreach (PersonElements.PersonKitData[] obj in allKits)
		{
			int num2 = -1;
			PersonElements.PersonKitData[] array2 = obj;
			foreach (PersonElements.PersonKitData personKitData in array2)
			{
				num2++;
				if ((int)personKitData.obscuredPrice != 0)
				{
					ObscuredInt coinPrice = personKitData.obscuredPrice;
					string text3 = coinPrice.ToString() + "1122";
					allHashes.Add(text3.GetHashCode());
				}
			}
			num2 = -1;
			array2 = _femalePersonKitConfig.allKits[num];
			foreach (PersonElements.PersonKitData personKitData2 in array2)
			{
				num2++;
				if ((int)personKitData2.obscuredPrice != 0)
				{
					ObscuredInt coinPrice = personKitData2.obscuredPrice;
					string text4 = coinPrice.ToString() + "1122";
					allHashes.Add(text4.GetHashCode());
				}
			}
			num++;
		}
	}

	private void OnStart()
	{
		counerPurchasedProduct = 0;
		counterAllProduct = 0;
		forCarCount = 0;
		forPersonCount = 0;
		foreach (Transform item in carProductParent.transform)
		{
			if ((bool)item)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		foreach (Transform item2 in personProductParent.transform)
		{
			if ((bool)item2)
			{
				UnityEngine.Object.Destroy(item2.gameObject);
			}
		}
		oneProductShopData[] array = allProductCars;
		foreach (oneProductShopData one in array)
		{
			try
			{
				CreateOneCarProduct(one);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
		array = allProductPersons;
		foreach (oneProductShopData one2 in array)
		{
			try
			{
				CreateOneOtherProduct(one2);
			}
			catch (Exception message2)
			{
				Debug.LogError(message2);
			}
		}
		if (forPersonCount != 0)
		{
			forPersonCount--;
		}
		loadPersonKitForSale();
		CloseOnFocus();
		base.transform.GetChild(0).gameObject.SetActive(forCarCount == 0);
		base.transform.GetChild(1).gameObject.SetActive(forPersonCount == 0);
	}

	private void OnEnable()
	{
		if (V.CoreGameManager.menuControl.prevMenuIndex != 27)
		{
			prevMenuIndex = V.CoreGameManager.menuControl.prevMenuIndex;
		}
		OnStart();
	}

	public void CloseShopWindow()
	{
		V.CoreGameManager.menuControl.prevMenuIndex = prevMenuIndex;
	}

	public void onProductSelected(oneProductShopData toFocus)
	{
		oneProductSelectedWindow.SetActive(value: true);
		allParent.SetActive(value: false);
		lastSelected = toFocus;
		GameObject gameObject = oneProductSelectedWindow.transform.GetChild(2).gameObject;
		if (toFocus.ProductID.Trim() == "")
		{
			gameObject.SetActive(value: true);
		}
		else
		{
			gameObject.SetActive(value: false);
		}
		if (lastSelected.isHorn)
		{
			int num = int.Parse(Regex.Replace(lastSelected.idPrefs, "[^0-9]", ""));
			if (lastSelected.uiCard.clip == null)
			{
				lastSelected.uiCard.clip = Resources.Load<AudioClip>("horns/HORN" + num);
			}
			source.clip = lastSelected.uiCard.clip;
			source.Play();
		}
	}

	public async void PurchaseSelectedProduct()
	{
	}

	public void OnPurchaseDone()
	{
		MonoBehaviour.print("OnPurchaseDone");
		if (lastSelected != null && lastSelected.ProductID != "" && lastSelected.carID != 0 && ObscuredPrefs.GetInt("car" + lastSelected.carID) == 1)
		{
			ObscuredPrefs.SetInt("LastCarId", lastSelected.carID);
		}
		if (V.StoreController == null)
		{
			CloseOnFocus();
		}
	}

	public void CloseOnFocus()
	{
		lastSelected = null;
		oneProductSelectedWindow.SetActive(value: false);
		allParent.SetActive(value: true);
	}

	private bool alreadyOwn(oneProductShopData one)
	{
		EquipmentsData equipmentsData = _charactersService.GetEquipmentsData(one.gender);
		if (one.isForPerson)
		{
			if (equipmentsData.IsItemBought(one.type, one.idEquipment))
			{
				return true;
			}
		}
		else if (one.carID != 0)
		{
			if (ObscuredPrefs.GetInt("car" + one.carID) == 1)
			{
				return true;
			}
		}
		else if (ObscuredPrefs.GetInt(one.idPrefs) == 1)
		{
			return true;
		}
		return false;
	}

	public void CreateOneCarProduct(oneProductShopData one)
	{
		counterAllProduct++;
		if (alreadyOwn(one))
		{
			counerPurchasedProduct++;
			return;
		}
		Transform obj = UnityEngine.Object.Instantiate(oneProductCarPrefab, carProductParent, worldPositionStays: true).transform;
		obj.GetComponent<OneProductShopWindowUI>().SetUIData(one);
		obj.GetComponent<OneProductShopWindowUI>().shopWindowUI = this;
		obj.SetParent(carProductParent);
		obj.transform.localPosition = new Vector3(forCarCount * 215, 0f, 0f);
		obj.gameObject.SetActive(value: true);
		obj.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
		forCarCount++;
	}

	public void CreateOneOtherProduct(oneProductShopData one)
	{
		counterAllProduct++;
		if (alreadyOwn(one))
		{
			counerPurchasedProduct++;
			return;
		}
		Transform obj = UnityEngine.Object.Instantiate(oneProductOtherPrefab, personProductParent, worldPositionStays: true).transform;
		obj.GetComponent<OneProductShopWindowUI>().SetUIData(one);
		obj.GetComponent<OneProductShopWindowUI>().shopWindowUI = this;
		obj.SetParent(personProductParent);
		obj.transform.localPosition = new Vector3(forPersonCount * 215, 0f, 0f);
		obj.gameObject.SetActive(value: true);
		obj.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
		forPersonCount++;
	}

	public void CreateOnePersonProduct(PersonElements.PersonKitData k, int typeID, int kitID, int gender)
	{
		Texture texture = Resources.Load(CharactersService.GetPathForKit(gender, typeID) + "icons/" + kitID + "_icon", typeof(Texture)) as Texture;
		oneProductShopData oneProductShopData2 = new oneProductShopData
		{
			texture = texture,
			coinPrice = k.obscuredPrice,
			isForPerson = true,
			inAppID = "",
			width = 300,
			height = 300,
			gender = gender,
			type = typeID,
			idEquipment = kitID
		};
		counterAllProduct++;
		if (alreadyOwn(oneProductShopData2) || texture == null)
		{
			counerPurchasedProduct++;
			return;
		}
		Transform obj = UnityEngine.Object.Instantiate(oneProductOtherPrefab, personProductParent, worldPositionStays: true).transform;
		forPersonCount++;
		obj.GetComponent<OneProductShopWindowUI>().SetUIData(oneProductShopData2);
		obj.GetComponent<OneProductShopWindowUI>().shopWindowUI = this;
		obj.SetParent(personProductParent);
		obj.transform.localPosition = new Vector3(forPersonCount * 215, 0f, 0f);
		obj.gameObject.SetActive(value: true);
		obj.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
	}

	private void loadPersonKitForSale()
	{
		int num = 0;
		PersonElements.PersonKitData[][] allKits = _femalePersonKitConfig.allKits;
		foreach (PersonElements.PersonKitData[] obj in allKits)
		{
			int num2 = -1;
			PersonElements.PersonKitData[] array = obj;
			foreach (PersonElements.PersonKitData personKitData in array)
			{
				num2++;
				if ((int)personKitData.obscuredPrice != 0)
				{
					CreateOnePersonProduct(personKitData, num, num2, 1);
				}
			}
			num2 = -1;
			array = _malePersonKitConfig.allKits[num];
			foreach (PersonElements.PersonKitData personKitData2 in array)
			{
				num2++;
				if ((int)personKitData2.obscuredPrice != 0)
				{
					CreateOnePersonProduct(personKitData2, num, num2, 0);
				}
			}
			num++;
		}
	}

	private void Update()
	{
	}
}
