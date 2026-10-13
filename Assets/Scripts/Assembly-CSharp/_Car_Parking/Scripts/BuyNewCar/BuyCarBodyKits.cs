using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.BuyNewCar
{
	public class BuyCarBodyKits : MonoBehaviour
	{
		private class BodyKitData
		{
			public int Index;

			public BodyKitType BodyKitType;

			public CostType PriceType;

			public int Price;

			public string TransformName;

			public BodyKitData(int index, BodyKitType bodyKitType, CostType priceType, int price, string transformName)
			{
				Index = index;
				BodyKitType = bodyKitType;
				PriceType = priceType;
				Price = price;
				TransformName = transformName;
			}
		}

		public ButtonPhysicsClickerController ButtonPhysicsClickerController;

		[SerializeField]
		private BodyKitsUpdater BodyKitsUpdater;

		private bool _initialLoadDone;

		[SerializeField]
		private BuyCarBodyKitsUI BuyCarBodyKitsUI;

		[SerializeField]
		private BodyKitsUIButtonPositionHelper UIButtonPositionHelper;

		private int _carID;

		private List<BodyKitData> _bodyKitDatas = new List<BodyKitData>();

		private BodyKits _bodyKitsOriginal;

		private IPricesService _pricesService;

		private ManagerUI _managerUI;

		private Dictionary<BodyKitType, int> _selectedBodyKits = new Dictionary<BodyKitType, int>();

		private Array AllBodyKitType => Enum.GetValues(typeof(BodyKitType));

		public int SetAmount
		{
			get
			{
				int num = 0;
				foreach (BodyKitType item in AllBodyKitType)
				{
					int bodyKitCount = GetBodyKitCount(item);
					if (bodyKitCount > 0)
					{
						num = ((num > 0) ? Mathf.Min(num, bodyKitCount) : bodyKitCount);
					}
				}
				return num;
			}
		}

		public int CurrentSetIndex
		{
			get
			{
				if (_selectedBodyKits.Count > 0)
				{
					using Dictionary<BodyKitType, int>.Enumerator enumerator = _selectedBodyKits.GetEnumerator();
					if (enumerator.MoveNext())
					{
						return enumerator.Current.Value;
					}
				}
				return 0;
			}
		}

		public bool HaveChanges
		{
			get
			{
				foreach (KeyValuePair<BodyKitType, int> selectedBodyKit in _selectedBodyKits)
				{
					if (selectedBodyKit.Value != GetDefaultKitOrdinal(selectedBodyKit.Key))
					{
						return true;
					}
				}
				return false;
			}
		}

		public event Action<BuyCarBodyKits> OnChanged;

		[Inject]
		public void Construct(IPricesService pricesService, ManagerUI managerUI)
		{
			_pricesService = pricesService;
			_managerUI = managerUI;
			if (base.isActiveAndEnabled)
			{
				_managerUI.OnCurrentDealerWindowUpdated += CurrentDealerWindowUpdated;
				CurrentDealerWindowUpdated(new ManagerUI.CurrentDealerWindowUpdatedData(_managerUI.CurrentDealerWindow, null));
			}
		}

		public void Init(int carID, BuyBodyKitsUIButtonPositionGetter bodyKitsUIButtonPositionGetter)
		{
			_carID = carID;
			_bodyKitDatas.Clear();
			_initialLoadDone = false;
			if ((bool)BodyKitsUpdater)
			{
				BodyKitsUpdater.InstantShow = true;
			}
			TrackInitialLoad(InitLoadCar(bodyKitsUIButtonPositionGetter)).Forget();
		}

		private async UniTaskVoid TrackInitialLoad(UniTask load)
		{
			try
			{
				await load;
			}
			finally
			{
				_initialLoadDone = true;
			}
		}

		public async UniTask WaitForBodyKitsAsync(CancellationToken cancellationToken)
		{
			while (!_initialLoadDone)
			{
				if (await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow())
				{
					return;
				}
			}
			if ((bool)BodyKitsUpdater)
			{
				await BodyKitsUpdater.WaitForPendingAsync(cancellationToken);
				BodyKitsUpdater.InstantShow = false;
			}
		}

		public CombinedPrice GetPrice()
		{
			CombinedPrice combinedPrice = new CombinedPrice();
			foreach (KeyValuePair<BodyKitType, int> selectedBodyKit in _selectedBodyKits)
			{
				BodyKitData bodyKit = GetBodyKit(selectedBodyKit.Key, selectedBodyKit.Value);
				if (bodyKit != null && IsPaidKitIndex(selectedBodyKit.Key, bodyKit.Index))
				{
					combinedPrice.Add(bodyKit.PriceType, bodyKit.Price);
				}
			}
			return combinedPrice;
		}

		private static bool ShouldForceNonDefault(BodyKitType type)
		{
			return BodyKitForcedDefaults.ShouldForceNonDefault(type);
		}

		private bool IsPaidKitIndex(BodyKitType type, int index)
		{
			if (!ShouldForceNonDefault(type))
			{
				return index != 0;
			}
			return index > GetDefaultKitIndex(type);
		}

		private int GetDefaultKitIndex(BodyKitType type)
		{
			if (!ShouldForceNonDefault(type))
			{
				return 0;
			}
			for (int i = 0; i < _bodyKitDatas.Count; i++)
			{
				if (_bodyKitDatas[i].BodyKitType == type && _bodyKitDatas[i].Index != 0)
				{
					return _bodyKitDatas[i].Index;
				}
			}
			return 0;
		}

		private int GetDefaultKitOrdinal(BodyKitType type)
		{
			if (!ShouldForceNonDefault(type))
			{
				return 0;
			}
			int bodyKitCount = GetBodyKitCount(type);
			for (int i = 0; i < bodyKitCount; i++)
			{
				BodyKitData bodyKit = GetBodyKit(type, i);
				if (bodyKit != null && bodyKit.Index != 0)
				{
					return i;
				}
			}
			return 0;
		}

		public void RecordBodyKitsForNewPurchasedCar(CarInfo carInfo)
		{
			foreach (KeyValuePair<BodyKitType, int> selectedBodyKit in _selectedBodyKits)
			{
				BodyKitData bodyKit = GetBodyKit(selectedBodyKit.Key, selectedBodyKit.Value);
				if (bodyKit != null)
				{
					carInfo.BoughtBodyKits.AddBoughtBodyKits(selectedBodyKit.Key, bodyKit.Index);
					carInfo.InstalledBodyKits.GetInstalledBodyKitIndexRef(selectedBodyKit.Key) = bodyKit.Index;
				}
			}
			for (int i = 0; i < carInfo.InstalledBodyKits.Trims.Length; i++)
			{
				carInfo.BoughtBodyKits.AddBoughtBodyKits(BodyKitType.Trim, carInfo.InstalledBodyKits.Trims[i]);
			}
			carInfo.Save();
		}

		public void ApplySelectedBodyKitsTo(InstalledBodyKits installed)
		{
			if (installed == null)
			{
				return;
			}
			foreach (KeyValuePair<BodyKitType, int> selectedBodyKit in _selectedBodyKits)
			{
				BodyKitData bodyKit = GetBodyKit(selectedBodyKit.Key, selectedBodyKit.Value);
				if (bodyKit != null)
				{
					installed.GetInstalledBodyKitIndexRef(selectedBodyKit.Key) = bodyKit.Index;
				}
			}
		}

		public void ResetBodyKits()
		{
			foreach (BodyKitType item in AllBodyKitType)
			{
				if (ShouldForceNonDefault(item))
				{
					ResetToFirstNonDefaultKit(item);
				}
				else
				{
					SlideBodyKit(item, 0, 1);
				}
			}
		}

		private void ResetToFirstNonDefaultKit(BodyKitType bodyKitType)
		{
			int bodyKitCount = GetBodyKitCount(bodyKitType);
			if (bodyKitCount <= 1)
			{
				return;
			}
			for (int i = 0; i < bodyKitCount; i++)
			{
				BodyKitData bodyKit = GetBodyKit(bodyKitType, i);
				if (bodyKit != null && bodyKit.Index != 0)
				{
					_selectedBodyKits.TryGetValue(bodyKitType, out var value);
					int num = i - value;
					if (num != 0)
					{
						SlideBodyKit(bodyKitType, num);
					}
					break;
				}
			}
		}

		public void SlideAllBodyKits(int addIndex)
		{
			foreach (BodyKitType item in AllBodyKitType)
			{
				SlideBodyKit(item, addIndex, SetAmount);
			}
		}

		private void OnEnable()
		{
			BuyCarBodyKitsUI.OnClicked += OnBodyKitButtonClicked;
			if (!(_managerUI == null))
			{
				_managerUI.OnCurrentDealerWindowUpdated += CurrentDealerWindowUpdated;
				CurrentDealerWindowUpdated(new ManagerUI.CurrentDealerWindowUpdatedData(_managerUI.CurrentDealerWindow, null));
			}
		}

		private void OnDisable()
		{
			BuyCarBodyKitsUI.OnClicked -= OnBodyKitButtonClicked;
			_managerUI.OnCurrentDealerWindowUpdated -= CurrentDealerWindowUpdated;
			if ((bool)_managerUI.CurrentDealerWindow)
			{
				_managerUI.CurrentDealerWindow.OnShowBodyKitsChanged -= OnShowBodyKitsChanged;
			}
		}

		private void CurrentDealerWindowUpdated(ManagerUI.CurrentDealerWindowUpdatedData currentDealerWindowUpdatedData)
		{
			if ((bool)currentDealerWindowUpdatedData.OldValue)
			{
				currentDealerWindowUpdatedData.OldValue.OnShowBodyKitsChanged -= OnShowBodyKitsChanged;
			}
			if ((bool)currentDealerWindowUpdatedData.NewValue)
			{
				currentDealerWindowUpdatedData.NewValue.OnShowBodyKitsChanged += OnShowBodyKitsChanged;
				OnShowBodyKitsChanged(currentDealerWindowUpdatedData.NewValue.ShowBodyKits);
			}
		}

		private void OnShowBodyKitsChanged(bool isShow)
		{
			BuyCarBodyKitsUI.gameObject.SetActive(isShow);
		}

		private async UniTask InitLoadCar(BuyBodyKitsUIButtonPositionGetter bodyKitsUIButtonPositionGetter)
		{
			GameObject gameObject = (await Resources.LoadAsync($"car{_carID}")) as GameObject;
			if (!this || !gameObject || !gameObject.TryGetComponent<BodyKits>(out _bodyKitsOriginal))
			{
				return;
			}
			AddBodyKits(_bodyKitsOriginal.spoilers, BodyKitType.Spoiler, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.BumperCountFront, BodyKitType.FrontBumper, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.BumperCountRear, BodyKitType.RearBumper, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.SkirtsCount, BodyKitType.Skirt, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.HoodCount, BodyKitType.Hood, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.airIntakesHood, BodyKitType.AirIntakeHood, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.airIntakeRoof, BodyKitType.AirIntakeRoof, _bodyKitsOriginal);
			AddBodyKits(_bodyKitsOriginal.FenderCount, BodyKitType.Fender, _bodyKitsOriginal);
			foreach (BodyKitType item in AllBodyKitType)
			{
				bool flag = ShouldForceNonDefault(item) && GetBodyKitCount(item) > 1;
				if (!HaveBodyKitsForBuy(item) && !flag)
				{
					continue;
				}
				string n = BodyKits.BodyKitTypeToGOName(item);
				Transform transform = base.transform.parent.FindChildContainsName(n);
				if ((bool)transform)
				{
					bodyKitValues bodyKitValues2 = transform.GetComponent<bodyKitValues>();
					if (!bodyKitValues2)
					{
						bodyKitValues2 = transform.gameObject.AddComponent<bodyKitValues>();
					}
					bodyKitValues2.BodyKitType = item;
					BodyKitsUpdater.AddBodyKit(bodyKitValues2);
				}
			}
			foreach (BodyKitType item2 in AllBodyKitType)
			{
				if (!HaveBodyKitsForBuy(item2))
				{
					continue;
				}
				Transform transform2 = base.transform.parent.FindChildContainsName(BodyKits.BodyKitTypeToGOName(item2));
				if ((bool)transform2)
				{
					BoxCollider boxCollider = transform2.gameObject.AddComponent<BoxCollider>();
					List<Vector3> list = new List<Vector3>();
					if ((bool)bodyKitsUIButtonPositionGetter)
					{
						list = bodyKitsUIButtonPositionGetter.GetPositions(item2);
					}
					if (list == null || list.Count == 0)
					{
						list = UIButtonPositionHelper.GetPositions(item2, boxCollider, base.transform);
					}
					for (int i = 0; i < list.Count; i++)
					{
						BuyCarBodyKitsUI.AddButton(list[i], item2, i);
					}
					UnityEngine.Object.Destroy(boxCollider);
				}
			}
			foreach (BodyKitType item3 in AllBodyKitType)
			{
				if (ShouldForceNonDefault(item3))
				{
					ResetToFirstNonDefaultKit(item3);
				}
			}
			OnChanged?.Invoke(this);
		}

		private void OnBodyKitButtonClicked(BodyKitType bodyKitType, int addIndex)
		{
			if (!_managerUI.CurrentDealerWindow.ConfirmPopupActive)
			{
				SlideBodyKit(bodyKitType, addIndex);
			}
		}

		private void SlideBodyKit(BodyKitType bodyKitType, int addIndex, int maxIndex = -1)
		{
			if (GetBodyKitCount(bodyKitType) <= 1)
			{
				return;
			}
			int num = 0;
			maxIndex = ((maxIndex >= 0) ? Mathf.Min(maxIndex, GetBodyKitCount(bodyKitType)) : GetBodyKitCount(bodyKitType));
			if (_selectedBodyKits.TryGetValue(bodyKitType, out var value))
			{
				num = value;
			}
			int num2 = num;
			num += addIndex;
			if (num < 0)
			{
				num = maxIndex - 1;
			}
			else if (num >= maxIndex)
			{
				num = 0;
			}
			if (ShouldForceNonDefault(bodyKitType))
			{
				int num3 = ((addIndex >= 0) ? 1 : (-1));
				int num4 = maxIndex + 1;
				while (num4-- > 0)
				{
					BodyKitData bodyKit = GetBodyKit(bodyKitType, num);
					if (bodyKit == null || bodyKit.Index != 0)
					{
						break;
					}
					num += num3;
					if (num < 0)
					{
						num = maxIndex - 1;
					}
					else if (num >= maxIndex)
					{
						num = 0;
					}
				}
			}
			if (num != num2)
			{
				_selectedBodyKits[bodyKitType] = num;
				Vector3 bodyKitPosition = Vector3.zero;
				Quaternion bodyKitRotation = Quaternion.identity;
				Vector3 bodyKitScale = Vector3.one;
				switch (bodyKitType)
				{
				case BodyKitType.Spoiler:
					GetBodyKitTransformData(_bodyKitsOriginal.spoilers, GetBodyKit(bodyKitType, num).TransformName, ref bodyKitPosition, ref bodyKitRotation, ref bodyKitScale);
					break;
				case BodyKitType.AirIntakeHood:
					GetBodyKitTransformData(_bodyKitsOriginal.airIntakesHood, GetBodyKit(bodyKitType, num).TransformName, ref bodyKitPosition, ref bodyKitRotation, ref bodyKitScale);
					break;
				case BodyKitType.AirIntakeRoof:
					GetBodyKitTransformData(_bodyKitsOriginal.airIntakeRoof, GetBodyKit(bodyKitType, num).TransformName, ref bodyKitPosition, ref bodyKitRotation, ref bodyKitScale);
					break;
				}
				BodyKitsUpdater.SetBodyKits(new BodyKitsUpdater.BodyKitsData(_carID, bodyKitType, GetBodyKit(bodyKitType, num).Index), new TransformData(bodyKitPosition, bodyKitRotation, bodyKitScale));
				OnChanged?.Invoke(this);
			}
		}

		private void AddBodyKits(Transform[] bodyKitsTransformsArray, BodyKitType bodyKitType, BodyKits bodyKits)
		{
			for (int i = 0; i < bodyKitsTransformsArray.Length; i++)
			{
				if ((bool)bodyKitsTransformsArray[i])
				{
					int num = Utils.ExtractIntFromName(bodyKitsTransformsArray[i].name);
					if (num >= 0)
					{
						AddBodyKit(num, bodyKitType, bodyKits, bodyKitsTransformsArray[i].name);
					}
				}
			}
		}

		private void AddBodyKits(int bodyKitsTransformsArray, BodyKitType bodyKitType, BodyKits bodyKits)
		{
			for (int i = 0; i < bodyKitsTransformsArray; i++)
			{
				AddBodyKit(i, bodyKitType, bodyKits, "");
			}
		}

		private void AddBodyKit(int index, BodyKitType bodyKitType, BodyKits bodyKits, string transformName)
		{
			bodyKits.GetPriceForKit(bodyKitType, index, _pricesService, out var price, out var priceType);
			_bodyKitDatas.Add(new BodyKitData(index, bodyKitType, priceType, price, transformName));
		}

		private bool HaveBodyKitsForBuy(BodyKitType bodyKitType)
		{
			if (!ShouldForceNonDefault(bodyKitType))
			{
				return GetBodyKitCount(bodyKitType) > 1;
			}
			int num = 0;
			int bodyKitCount = GetBodyKitCount(bodyKitType);
			for (int i = 0; i < bodyKitCount; i++)
			{
				BodyKitData bodyKit = GetBodyKit(bodyKitType, i);
				if (bodyKit == null || bodyKit.Index != 0)
				{
					num++;
				}
			}
			return num > 1;
		}

		private int GetBodyKitCount(BodyKitType bodyKitType)
		{
			int num = 0;
			for (int i = 0; i < _bodyKitDatas.Count; i++)
			{
				if (_bodyKitDatas[i].BodyKitType == bodyKitType)
				{
					num++;
				}
			}
			return num;
		}

		private BodyKitData GetBodyKit(BodyKitType bodyKitType, int index)
		{
			for (int i = 0; i < _bodyKitDatas.Count; i++)
			{
				if (_bodyKitDatas[i].BodyKitType == bodyKitType)
				{
					index--;
					if (index < 0)
					{
						return _bodyKitDatas[i];
					}
				}
			}
			return null;
		}

		private void GetBodyKitTransformData(Transform[] transforms, string transformName, ref Vector3 bodyKitPosition, ref Quaternion bodyKitRotation, ref Vector3 bodyKitScale)
		{
			for (int i = 0; i < transforms.Length; i++)
			{
				if ((bool)transforms[i] && transforms[i].name == transformName)
				{
					bodyKitPosition = transforms[i].localPosition;
					bodyKitRotation = transforms[i].localRotation;
					bodyKitScale = transforms[i].localScale;
				}
			}
		}

		public AdvancedInt GetMoneyKitsCount()
		{
			int num = 0;
			foreach (KeyValuePair<BodyKitType, int> selectedBodyKit in _selectedBodyKits)
			{
				BodyKitData bodyKit = GetBodyKit(selectedBodyKit.Key, selectedBodyKit.Value);
				if (bodyKit != null && IsPaidKitIndex(selectedBodyKit.Key, bodyKit.Index) && bodyKit.PriceType == CostType.Money)
				{
					num++;
				}
			}
			return new AdvancedInt(num);
		}

		public AdvancedInt GetCoinsPrice()
		{
			int num = 0;
			foreach (KeyValuePair<BodyKitType, int> selectedBodyKit in _selectedBodyKits)
			{
				BodyKitData bodyKit = GetBodyKit(selectedBodyKit.Key, selectedBodyKit.Value);
				if (bodyKit != null && IsPaidKitIndex(selectedBodyKit.Key, bodyKit.Index) && bodyKit.PriceType == CostType.Coins)
				{
					num += bodyKit.Price;
				}
			}
			return new AdvancedInt(num);
		}

		public List<int> GetBodyKitsForBuyCars()
		{
			return new List<int>
			{
				GetSelectedBodyKit(BodyKitType.Spoiler),
				GetSelectedBodyKit(BodyKitType.FrontBumper),
				GetSelectedBodyKit(BodyKitType.RearBumper),
				GetSelectedBodyKit(BodyKitType.Skirt),
				GetSelectedBodyKit(BodyKitType.Hood),
				GetSelectedBodyKit(BodyKitType.AirIntakeHood),
				GetSelectedBodyKit(BodyKitType.AirIntakeRoof),
				GetSelectedBodyKit(BodyKitType.Fender)
			};
		}

		private int GetSelectedBodyKit(BodyKitType bodyKitType)
		{
			if (_selectedBodyKits.TryGetValue(bodyKitType, out var value))
			{
				BodyKitData bodyKit = GetBodyKit(bodyKitType, value);
				if (bodyKit != null)
				{
					return bodyKit.Index;
				}
			}
			return 0;
		}
	}
}
