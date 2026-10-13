using System.Collections.Generic;
using CPM.Database;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class BodyKitsCartData
	{
		public BodyKitType BodyKitType = BodyKitType.None;

		public int BodyKitIndex = -1;

		public List<int> Trims = new List<int>();

		private CarInfo _carInfo;

		private readonly BodyKitsTrimConfigs _trimConfigs;

		private CartDataExtraController _extraController;

		public bool IsFilled
		{
			get
			{
				if (BodyKitType != BodyKitType.None && BodyKitType != BodyKitType.Trim && BodyKitIndex >= 0)
				{
					return true;
				}
				if (_carInfo != null && _carInfo.InstalledBodyKits != null && _carInfo.InstalledBodyKits.Trims != null)
				{
					if (_carInfo.InstalledBodyKits.Trims.Length != Trims.Count)
					{
						return true;
					}
					for (int i = 0; i < Trims.Count; i++)
					{
						if (!_carInfo.InstalledBodyKits.Trims.Contains(Trims[i]))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public BodyKitsCartData(BodyKitsTrimConfigs trimConfigs)
		{
			_trimConfigs = trimConfigs;
		}

		public void Init(CarInfo carInfo)
		{
			_carInfo = carInfo;
			if (carInfo.CarID == 220)
			{
				_extraController = new CartDataExtraController220(_trimConfigs);
			}
			else
			{
				_extraController = null;
			}
			BodyKitType = BodyKitType.None;
			SetDefaultTrims();
		}

		public void AddSmart(BodyKitType bodyKitType, int index)
		{
			if (bodyKitType == BodyKitType.Trim)
			{
				BodyKitsTrimConfigs.TrimType trimType = _trimConfigs.GetTrimType(index);
				if (trimType != BodyKitsTrimConfigs.TrimType.None)
				{
					for (int i = 0; i < Trims.Count; i++)
					{
						if (_trimConfigs.GetTrimType(Trims[i]) == trimType && Trims[i] != index)
						{
							Trims.RemoveAt(i);
							i--;
						}
					}
				}
				if (Trims.Contains(index))
				{
					BodyKitsTrimConfigs.TrimType trimType2 = _trimConfigs.GetTrimType(index);
					if (!_trimConfigs.ThisTrimRequiredToInstall(trimType2) || GetTrimAmount(trimType2) > 1)
					{
						Trims.Remove(index);
					}
				}
				else
				{
					Trims.Add(index);
				}
				BodyKitType = BodyKitType.None;
				BodyKitIndex = -1;
			}
			else
			{
				SetDefaultTrims();
				BodyKitType = bodyKitType;
				BodyKitIndex = index;
			}
			_extraController?.AddSmart(this, BodyKitType, index);
		}

		public bool IsHaveItem(BodyKitUIElementData bodyKitUIElementData)
		{
			if (bodyKitUIElementData.BodyKitType == BodyKitType.Trim)
			{
				return Trims.Contains(bodyKitUIElementData.Index);
			}
			if (bodyKitUIElementData.BodyKitType == BodyKitType)
			{
				return bodyKitUIElementData.Index == BodyKitIndex;
			}
			return false;
		}

		public void Clear()
		{
			BodyKitType = BodyKitType.None;
			BodyKitIndex = -1;
			SetDefaultTrims();
		}

		public CombinedPrice GetPrice(IPricesService pricesService)
		{
			CombinedPrice priceBodyKit = GetPriceBodyKit(BodyKitType, BodyKitIndex, pricesService);
			foreach (int trim in Trims)
			{
				priceBodyKit += GetPriceBodyKit(BodyKitType.Trim, trim, pricesService);
			}
			return priceBodyKit;
		}

		private void SetDefaultTrims()
		{
			Trims.Clear();
			int[] installedBodyKitIds = _carInfo.InstalledBodyKits.GetInstalledBodyKitIds(BodyKitType.Trim);
			foreach (int item in installedBodyKitIds)
			{
				Trims.Add(item);
			}
		}

		private CombinedPrice GetPriceBodyKit(BodyKitType bodyKitType, int bodyKitIndex, IPricesService pricesService)
		{
			if (bodyKitType == BodyKitType.None)
			{
				return new CombinedPrice();
			}
			CombinedPrice combinedPrice = BodyKitsService.GetCombinedPrice(bodyKitType, bodyKitIndex, pricesService);
			if (_carInfo.BoughtBodyKits.GetBodyKitPaidStatus(bodyKitType, bodyKitIndex))
			{
				combinedPrice.Coin = 0;
				combinedPrice.Money = 0;
			}
			return combinedPrice;
		}

		private int GetTrimAmount(BodyKitsTrimConfigs.TrimType trimType)
		{
			int num = 0;
			foreach (int trim in Trims)
			{
				if (_trimConfigs.GetTrimType(trim) == trimType)
				{
					num++;
				}
			}
			return num;
		}
	}
}
