using System.Collections.Generic;
using System.Globalization;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.DataEntities.Car.CarEntities
{
	public static class BuyCar
	{
		public static List<int> GetBoughtParts(string packedData, int sizesGears, int sizesBoughtParts)
		{
			List<int> list = new List<int>();
			string[] array = packedData.Split("?"[0]);
			list.Add(int.Parse(array[0]));
			list.Add(0);
			for (int i = 1 + sizesGears; i < array.Length; i++)
			{
				list.Add(int.Parse(array[i]));
			}
			int index = 12;
			list.Insert(index, 0);
			list.Insert(index, 0);
			list.Insert(index, 0);
			list.Insert(index, 0);
			list.Insert(index, 0);
			return BoughtPartsSetAndHasSync(list, sizesBoughtParts);
		}

		public static List<int> BoughtPartsSetAndHasSync(List<int> boughtPartsOld, int SizesBoughtParts)
		{
			List<int> list = new List<int>();
			int num = (SizesBoughtParts - 1) / 2;
			if (boughtPartsOld == null)
			{
				boughtPartsOld = new List<int>();
			}
			boughtPartsOld.Resize2(num + 1, 0);
			list.AddRange(boughtPartsOld);
			boughtPartsOld.RemoveAt(0);
			list.AddRange(boughtPartsOld);
			list.Resize2(SizesBoughtParts, 0);
			return list;
		}

		public static List<float> GetGears(string packedData, int sizesGears)
		{
			string[] array = packedData.Split("?"[0]);
			List<float> list = new List<float>();
			for (int i = 1; i < 1 + sizesGears; i++)
			{
				list.Add(float.Parse(array[i], CultureInfo.InvariantCulture.NumberFormat));
			}
			return list;
		}

		public static string CalculateSendData(int hasTurboDefault, IOneCarKeysToCollectionsAdapter saveCarAdapter)
		{
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(saveCarAdapter.GetBoughtPart("hasTurbo", hasTurboDefault) + "?", saveCarAdapter.GetGears(0, -10f).ToString(CultureInfo.InvariantCulture), "?"), saveCarAdapter.GetGears(1, -10f).ToString(CultureInfo.InvariantCulture), "?"), saveCarAdapter.GetGears(2, -10f).ToString(CultureInfo.InvariantCulture), "?"), saveCarAdapter.GetGears(3, -10f).ToString(CultureInfo.InvariantCulture), "?"), saveCarAdapter.GetGears(4, -10f).ToString(CultureInfo.InvariantCulture), "?"), saveCarAdapter.GetGears(5, -10f).ToString(CultureInfo.InvariantCulture), "?"), saveCarAdapter.GetGears(6, -10f).ToString(CultureInfo.InvariantCulture), "?"), saveCarAdapter.GetGears(7, -10f).ToString(CultureInfo.InvariantCulture), "?"), saveCarAdapter.GetBoughtPart("isSet" + 1).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 2).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 3).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 4).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 5).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 6).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 7).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 8).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 9).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 10).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 16).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 17).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 18).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 19).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 20).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 21).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 22).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 23).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 24).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 25).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 26).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 27).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 28).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 29).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 30).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 31).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 32).ToString(), "?"), saveCarAdapter.GetBoughtPart("isSet" + 33).ToString());
		}
	}
}
