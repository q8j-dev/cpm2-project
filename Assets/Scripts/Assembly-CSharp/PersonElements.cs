using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Other.Helpers;

public class PersonElements : MonoBehaviour
{
	[Serializable]
	public enum Pack
	{
		no = 0,
		police = 1,
		noPants = 2,
		noPantsShoes = 3,
		noPantsGloves = 4
	}

	public enum AvailableType
	{
		Normal = 0,
		King = 1
	}

	[Serializable]
	public class PersonKitData
	{
		private ObscuredInt _maskValue;

		private short placeholderNumber;

		private string extraParameter;

		private int additionalIndex;

		private string reservedKey;

		private bool enableMasking;

		public ObscuredInt obscuredPrice;

		private float shadowIntensity;

		private byte checkCode;

		private char extraFlag;

		public ObscuredInt price2;

		public bool HidesBodyCompletely;

		public int MainBodyMask;

		public bool IsRewardKit;

		public int hairToSet = -1;

		public bool HideBeard;

		public Equipment.Role role;

		public Pack pack;

		public int Id;

		public PersonClothsEventType PersonClothsEventType;

		[SerializeField]
		private ObscuredInt ObscuredAvailableType;

		public AvailableType AvailableType => (AvailableType)(int)ObscuredAvailableType;

		public OneKitMenuItem.AvailableToBuyType AvailableToBuyStatus
		{
			get
			{
				if (AvailableType == AvailableType.King && UtilsRank.GetRank() < UtilsRank.RankType.King)
				{
					return OneKitMenuItem.AvailableToBuyType.Locked;
				}
				return OneKitMenuItem.AvailableToBuyType.Normal;
			}
		}
	}

	private static readonly int[] CapHideInCarMale = new int[51]
	{
		15, 29, 37, 38, 51, 63, 64, 65, 66, 67,
		68, 69, 70, 71, 72, 73, 80, 81, 82, 83,
		84, 85, 86, 87, 107, 108, 109, 110, 113, 114,
		115, 116, 117, 118, 119, 120, 121, 122, 123, 124,
		125, 126, 127, 128, 129, 130, 131, 132, 133, 140,
		141
	};

	private static readonly int[] CapHideInCarFemale = new int[25]
	{
		16, 30, 41, 42, 43, 44, 45, 46, 47, 48,
		49, 50, 51, 58, 59, 60, 61, 62, 63, 64,
		65, 86, 87, 88, 89
	};

	public bool FemaleElements;

	public static bool CapNeedHideInCar(int genderIndex, int headwearIndex)
	{
		switch (genderIndex)
		{
		case 0:
			if (CapHideInCarFemale != null)
			{
				return CapHideInCarFemale.Contains(headwearIndex);
			}
			break;
		case 1:
			if (CapHideInCarMale != null)
			{
				return CapHideInCarMale.Contains(headwearIndex);
			}
			break;
		}
		return false;
	}
}
