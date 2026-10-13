using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService;

[CreateAssetMenu(fileName = "GaragePositionsConfig", menuName = "Scriptable/GaragePositionsConfig", order = 15)]
[Preserve]
public class GaragePositionsConfig : PriceConfig<GarageData>
{
	public GarageData[] GaragePositionsDatas;

	public readonly List<int> OrderedCarIds = new List<int>
	{
		13, 133, 132, 53, 99, 100, 102, 37, 21, 48,
		77, 74, 2, 23, 51, 163, 186, 158, 55, 39,
		181, 196, 160, 197, 47, 66, 1, 106, 76, 0,
		43, 152, 108, 82, 81, 146, 204, 147, 210, 148,
		149, 49, 112, 59, 29, 20, 88, 137, 139, 176,
		180, 179, 185, 54, 60, 85, 45, 190, 6, 57,
		209, 30, 56, 145, 156, 11, 128, 177, 129, 131,
		140, 187, 184, 134, 89, 12, 9, 31, 120, 4,
		8, 62, 17, 175, 157, 113, 86, 168, 200, 5,
		127, 117, 15, 35, 206, 3, 28, 151, 110, 87,
		103, 19, 24, 116, 121, 123, 58, 22, 205, 109,
		138, 130, 70, 170, 171, 191, 195, 101, 7, 118,
		65, 135, 211, 193, 150, 153, 104, 114, 105, 208,
		154, 126, 189, 192, 61, 107, 111, 18, 199, 40,
		166, 141, 14, 188, 136, 115, 10, 194, 172, 125,
		42, 124, 169, 142, 161, 143, 144, 164, 41, 162,
		201, 44, 159, 27, 178, 32, 198, 202, 207, 155,
		165, 182, 183, 203
	};

	public override GarageData Get(int index)
	{
		GarageData garageData = GaragePositionsDatas.First((GarageData x) => x.Index == index);
		if (garageData != null)
		{
			return garageData;
		}
		Debug.LogError("Can't find garage with index:" + index);
		return GaragePositionsDatas[0];
	}
}
