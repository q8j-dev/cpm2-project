using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Serialization;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Other.Helpers;

[Serializable]
public class GarageData : IPriced
{
	public string Name;

	public string NameLocalizedKey;

	[FormerlySerializedAs("Location")]
	public string LocationKey;

	public int Index;

	public int SceneIndex;

	public string SceneName;

	public int DefaultCarSpotCount = 3;

	public bool ForSale;

	public ObscuredInt MoneyCost;

	public ObscuredInt CoinCost;

	public int MaxCarSpotCount;

	[SerializeField]
	private GlobalCost _price;

	public PositionsAndRotation FreeDriverPlace;

	public PositionsAndRotation SellCarPoint;

	public PositionsAndRotation CarPaintbox;

	public Vector3 RWDLocalPosDynoStand;

	public Vector3 FWDLocalPosDynoStand;

	public Vector3 SWDLocalPosDynoStand;

	public float PersonShopCameraXAxis;

	public PositionsAndRotation DynoChecker;

	public PositionsAndRotation VisualTuningChecker;

	public PositionsAndRotation PersonCustomizationPosition;

	public string GarageIconAddressablePath;

	public PositionsAndRotation StartPlaceToMovePlayerToShop;

	public Vector3 PositionTargetForCamera;

	public PositionsAndRotation SellCarPointForCustomMoveCam;

	public CombinedPrice SaleCost => new CombinedPrice(CoinCost, MoneyCost);

	public CostType PriceType => _price.PriceType;

	public ObscuredInt GetPrice
	{
		get
		{
			ObscuredInt price = _price.Price;
			price.RandomizeCryptoKey();
			return price;
		}
	}
}
