using System.IO;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts.DataEntities;
using _Car_Parking.Scripts.DataEntities.Car;

namespace _Car_Parking.Scripts.Offers
{
	public class OfferCarVisualDataSaver : MonoBehaviour
	{
		private const string PathSave = "OfferCars";

		public static void SaveData(string fileName)
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				CarInfoForDatabase carInfoForDatabase = new CarInfoForDatabase(component.carInfo);
				carInfoForDatabase.TryGetCarVinyls(VinylsType.CarVinyls, out carInfoForDatabase.Vinyls);
				carInfoForDatabase.TryGetCarVinyls(VinylsType.WindowVinyls, out carInfoForDatabase.Window);
				NewCarVisualData newCarVisualData = new NewCarVisualData();
				newCarVisualData.VynilOneCar = new VynilOneCar();
				newCarVisualData.VynilOneCar.allVynils = carInfoForDatabase.Vinyls;
				newCarVisualData.VynilOneCar.CarID = component.CarId;
				newCarVisualData.VynilOneCar.CarInstanceID = null;
				newCarVisualData.CarPaintColor = carInfoForDatabase.Colors.CarColor;
				string text = Path.Combine(Application.persistentDataPath, "OfferCars");
				string text2 = Path.Combine(text, fileName + ".json");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				File.WriteAllText(text2, JsonConvert.SerializeObject(newCarVisualData, JsonConvertCustomSettingsForDatabase.JsonSettingsForDataBase));
				Debug.Log("saved in: " + text2);
			}
		}
	}
}
