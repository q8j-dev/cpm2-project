using System;
using CPM.Database;
using DataEntities.Vinyl;
using Persistence.Systems;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.CommonUtility;

namespace CPM.Scripts.VinylDrawer
{
	public static class VinylsUtility
	{
		public const int VinylsInstantiationDataIndex = 6;

		public static int GetVinylsCount(CarInfo carInfo, out ES3File file, VinylsType type = VinylsType.CarVinyls, bool isFrontPlate = false)
		{
			int num = ((type == VinylsType.PlateVinyls) ? PlateDataUtility.TryGetPlateIdForCar(carInfo.CarInstanceId, isFrontPlate) : 0);
			if (!EasySaveDataHandler.TryGetFile(type switch
			{
				VinylsType.CarVinyls => DirectoryHelper.GetCarVinylsPath(carInfo.CarInstanceId), 
				VinylsType.PlateVinyls => DirectoryHelper.GetPlateVinylsPath(num.ToString()), 
				VinylsType.WindowVinyls => DirectoryHelper.GetWindowVinylsPath(carInfo.CarInstanceId), 
				_ => throw new ArgumentOutOfRangeException("type", type, null), 
			}, out file))
			{
				return 0;
			}
			return file.GetKeys().Length;
		}

		public static void CalculateFillVinylsInstantiationData(object[] photonInstantiationData, CarInfo carInfo, int vinylsInstantiationDataIndex)
		{
			object[] array = (object[])(photonInstantiationData[vinylsInstantiationDataIndex] = new object[4]);
			array[0] = GetVinylsInstantiationData(carInfo, VinylsType.CarVinyls, isFrontPlate: false);
			array[1] = GetVinylsInstantiationData(carInfo, VinylsType.PlateVinyls, isFrontPlate: true);
			array[2] = GetVinylsInstantiationData(carInfo, VinylsType.PlateVinyls, isFrontPlate: false);
			array[3] = GetVinylsInstantiationData(carInfo, VinylsType.WindowVinyls, isFrontPlate: false);
		}

		public static object[] GetVinylsInstantiationData(CarInfo carInfo, VinylsType type, bool isFrontPlate)
		{
			int vinylsCount = GetVinylsCount(carInfo, out var file, type, isFrontPlate);
			if (vinylsCount <= 0)
			{
				return null;
			}
			object[] array = new object[vinylsCount];
			string[] keys = file.GetKeys();
			for (int i = 0; i < vinylsCount; i++)
			{
				object[] array2 = new object[6];
				PopulateVinylInstantiationData(keys[i], file, array2);
				array[i] = array2;
			}
			return array;
		}

		private static void PopulateVinylInstantiationData(string vinylKey, ES3File saveFile, object[] vinylInstantiationData)
		{
			VinylData vinylData = saveFile.Load<VinylData>(vinylKey);
			string text = vinylData.text;
			if (text != null && text.Length > 500)
			{
				text = text.Substring(0, 400);
			}
			Color color = BitMaskColorPacker.UnpackColor(vinylData.color);
			float[] array = new float[4] { color.r, color.g, color.b, color.a };
			vinylInstantiationData[0] = vinylData.position;
			vinylInstantiationData[1] = vinylData.scaleRotation;
			vinylInstantiationData[2] = vinylData.iconPosition;
			vinylInstantiationData[3] = text;
			vinylInstantiationData[4] = array;
			vinylInstantiationData[5] = vinylData.packedData;
		}

		public static VinylData UnpackVinylInstantiationData(object[] vinylInstantiationData)
		{
			float[] array = (float[])vinylInstantiationData[4];
			Color color = new Color(array[0], array[1], array[2], array[3]);
			return new VinylData
			{
				position = (Vector3)vinylInstantiationData[0],
				scaleRotation = (Vector3)vinylInstantiationData[1],
				iconPosition = (Vector3)vinylInstantiationData[2],
				text = (string)vinylInstantiationData[3],
				color = BitMaskColorPacker.PackColor(color),
				packedData = (long)vinylInstantiationData[5]
			};
		}

		internal static VinylData ParseVinylToVinylData(StickerItem stickerItem, bool resetBackColor)
		{
			ParseVinylToStickerItem(stickerItem, resetBackColor);
			return GetVinylDataFromSticker(stickerItem);
		}

		public static VinylData GetVinylDataFromSticker(StickerItem stickerItem)
		{
			return new VinylData
			{
				position = new Vector3(stickerItem.position.x, stickerItem.position.y, stickerItem.Order),
				scaleRotation = new Vector3(stickerItem.scale.x, stickerItem.scale.y, stickerItem.euler.z),
				iconPosition = stickerItem.positionIcon,
				text = stickerItem.text,
				color = BitMaskColorPacker.PackColor(stickerItem.color),
				packedData = BitMaskPacker.Pack(stickerItem.isText, stickerItem.italic, stickerItem.bold, stickerItem.flipX == 1, stickerItem.fontIndex, stickerItem.stckerIndex, (int)stickerItem.xAngle, (int)stickerItem.yAngle)
			};
		}

		internal static void ParseVinylDataToStickerItem(VinylData vinylData, StickerItem stickerData, int order, GameObject vinyl)
		{
			Vector3 position = new Vector3(vinylData.position.x, vinylData.position.y, -0.01f * (float)order);
			Vector3 iconPosition = vinylData.iconPosition;
			Vector3 euler = new Vector3(0f, 0f, vinylData.scaleRotation.z);
			Vector3 scale = new Vector3(vinylData.scaleRotation.x, vinylData.scaleRotation.y, 1f);
			BitMaskPacker.Unpack(vinylData.packedData, out var isText, out var italic, out var bold, out var flipX, out var fontIndex, out var stickerIndex, out var xAngle, out var yAngle);
			string text = vinylData.text;
			Color color = BitMaskColorPacker.UnpackColor(vinylData.color);
			if (isText)
			{
				stickerData.isText = true;
				stickerData.text = text;
				stickerIndex = 1;
			}
			else
			{
				stickerData.isText = false;
				stickerIndex = ((stickerIndex >= 0 && stickerIndex < V.AllVinyls.Length && stickerIndex != 1) ? stickerIndex : 0);
			}
			stickerData.position = position;
			stickerData.Order = order;
			stickerData.positionIcon = iconPosition;
			stickerData.euler = euler;
			stickerData.scale = scale;
			stickerData.color = color;
			stickerData.ThisVinyl = vinyl.transform;
			stickerData.stckerIndex = stickerIndex;
			stickerData.xAngle = xAngle;
			stickerData.yAngle = yAngle;
			stickerData.sprite.sprite = V.AllVinyls[stickerIndex].sprite;
			Color color2 = stickerData.color;
			color2.a = 1f;
			stickerData.sprite.color = color2;
			stickerData.flipX = (flipX ? 1 : 0);
			stickerData.fontIndex = fontIndex;
			stickerData.italic = italic;
			stickerData.bold = bold;
			stickerData.oldIndex = stickerIndex;
			stickerData.oldColor = color;
		}

		internal static void ParseVinylToStickerItem(StickerItem sticker, bool resetBackColor)
		{
			bool flag = sticker.stckerIndex == 1;
			Transform thisVinyl = sticker.ThisVinyl;
			SpriteRenderer component = thisVinyl.GetComponent<SpriteRenderer>();
			TextMesh component2 = thisVinyl.GetChild(0).GetComponent<TextMesh>();
			Color color = (flag ? component2.color : component.color);
			FontStyle fontStyle = component2.fontStyle;
			sticker.gameObject.SetActive(value: true);
			sticker.sprite.sprite = V.AllVinyls[flag ? 1 : sticker.stckerIndex].sprite;
			sticker.sprite.color = color;
			sticker.color = color;
			if (resetBackColor)
			{
				sticker.backround.color = sticker.InitialColorBackround;
				sticker.checkMark.SetActive(value: false);
			}
			sticker.position = thisVinyl.localPosition;
			sticker.euler = thisVinyl.localEulerAngles;
			sticker.scale = thisVinyl.localScale;
			sticker.isText = flag;
			sticker.fontIndex = (component2.font ? Array.IndexOf(V.Fonts, component2.font) : 0);
			sticker.italic = fontStyle == FontStyle.Italic || fontStyle == FontStyle.BoldAndItalic;
			sticker.bold = fontStyle == FontStyle.Bold || fontStyle == FontStyle.BoldAndItalic;
			sticker.flipX = (component.flipX ? 1 : 0);
			sticker.oldIndex = sticker.stckerIndex;
			sticker.oldColor = color;
			sticker.UpdateOrder();
			sticker.text = (flag ? component2.text : "");
			if (sticker.text.Length > 400)
			{
				sticker.text = sticker.text.Substring(0, 400);
			}
		}
	}
}
