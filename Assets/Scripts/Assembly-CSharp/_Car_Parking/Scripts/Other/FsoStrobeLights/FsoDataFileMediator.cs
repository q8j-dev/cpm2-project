using CPM.Database;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	public class FsoDataFileMediator
	{
		private CarInfo _carInfo;

		public FsoDataFileMediator(CarInfo carInfo)
		{
			_carInfo = carInfo;
		}

		public int GetFsoIndex()
		{
			return _carInfo.AdditionalData.Fso.ModelIndex;
		}

		public void SetFsoIndex(int value)
		{
			_carInfo.AdditionalData.Fso.ModelIndex = value;
			Save();
		}

		public int GetFsoPosition()
		{
			return _carInfo.AdditionalData.Fso.PositionIndex;
		}

		public void SetFsoPosition(int value)
		{
			_carInfo.AdditionalData.Fso.PositionIndex = value;
			Save();
		}

		public Color GetFsoColorLeft()
		{
			return _carInfo.AdditionalData.Fso.LeftColor;
		}

		public Color GetFsoColorRight()
		{
			return _carInfo.AdditionalData.Fso.RightColor;
		}

		public void SetFsoColor(Color colorLeft, Color colorRight)
		{
			_carInfo.AdditionalData.Fso.LeftColor = colorLeft;
			_carInfo.AdditionalData.Fso.RightColor = colorRight;
		}

		public void ResetCarPrefs()
		{
			_carInfo.AdditionalData.Fso.ModelIndex = -1;
			_carInfo.AdditionalData.Fso.PositionIndex = 0;
			_carInfo.AdditionalData.Fso.LeftColor = Color.white;
			_carInfo.AdditionalData.Fso.RightColor = Color.white;
			Save();
		}

		private void Save()
		{
			_carInfo.Save();
		}
	}
}
