using System.Collections.Generic;

namespace _Car_Parking.Scripts.DataEntities.Car.CarEntities
{
	public static class OneCarAdapterDictionaries
	{
		public static Dictionary<string, int> floatsInOneCar = new Dictionary<string, int>();

		public static Dictionary<string, int> vectorsInOneCar = new Dictionary<string, int>();

		public static Dictionary<string, int> boughtPartsInOneCar = new Dictionary<string, int>();

		public static void InitDictionaries()
		{
			floatsInOneCar["CarPoliceBought"] = 0;
			floatsInOneCar["maxPower"] = 1;
			floatsInOneCar["maxPowerRPM"] = 2;
			floatsInOneCar["maxPowerRpm"] = 2;
			floatsInOneCar["maxTorque"] = 3;
			floatsInOneCar["maxTorqueRPM"] = 4;
			floatsInOneCar["FOffset"] = 5;
			floatsInOneCar["ROffset"] = 6;
			floatsInOneCar["FforwardGripFactor"] = 7;
			floatsInOneCar["GforwardGripFactor"] = 8;
			floatsInOneCar["wheelToSet"] = 9;
			floatsInOneCar["FsuspensionTravel"] = 10;
			floatsInOneCar["RsuspensionTravel"] = 11;
			floatsInOneCar["FsuspensionRate"] = 12;
			floatsInOneCar["RsuspensionRate"] = 13;
			floatsInOneCar["Fcamber"] = 14;
			floatsInOneCar["Rcamber"] = 15;
			floatsInOneCar["shiftTime"] = 16;
			floatsInOneCar["burble"] = 17;
			floatsInOneCar["turboShot"] = 18;
			floatsInOneCar["mileage"] = 19;
			floatsInOneCar["MaxSteeringAngle"] = 20;
			floatsInOneCar["_Shininess"] = 21;
			floatsInOneCar["_Smoothness"] = 22;
			floatsInOneCar["_WShininess"] = 23;
			floatsInOneCar["_WSmoothness"] = 24;
			floatsInOneCar["_W2Shininess"] = 25;
			floatsInOneCar["_W2Smoothness"] = 26;
			floatsInOneCar["camDistance"] = 27;
			floatsInOneCar["camHeight"] = 28;
			floatsInOneCar["camPitch"] = 29;
			floatsInOneCar["camDamp"] = 30;
			floatsInOneCar["camField"] = 31;
			floatsInOneCar["sizeWheelScale"] = 32;
			floatsInOneCar["COGx"] = 33;
			floatsInOneCar["COGy"] = 34;
			floatsInOneCar["WindowA"] = 35;
			floatsInOneCar["tireFont"] = 36;
			floatsInOneCar["tireFontStyle"] = 37;
			floatsInOneCar["rimRadius"] = 38;
			floatsInOneCar["tireHealth2_"] = 39;
			floatsInOneCar["tireHealth3_"] = 40;
			floatsInOneCar["tireHealth0_"] = 41;
			floatsInOneCar["tireHealth1_"] = 42;
			floatsInOneCar["brake"] = 43;
			vectorsInOneCar["CarColor"] = 0;
			vectorsInOneCar["CarColorReflective"] = 1;
			vectorsInOneCar["CarW"] = 2;
			vectorsInOneCar["WheelColorReflective"] = 3;
			vectorsInOneCar["CarColorWindow"] = 4;
			vectorsInOneCar["WindowSpecColor"] = 5;
			vectorsInOneCar["type0_C_"] = 6;
			vectorsInOneCar["type5_C_"] = 7;
			vectorsInOneCar["type6_C_"] = 8;
			vectorsInOneCar["Diod_C_"] = 9;
			vectorsInOneCar["Smoke_C_"] = 10;
			vectorsInOneCar["CarColorSpecular"] = 11;
			vectorsInOneCar["ColorSpecularWheel"] = 12;
			vectorsInOneCar["tireTextColor"] = 13;
			vectorsInOneCar["CarW2"] = 14;
			vectorsInOneCar["WheelColorReflective2"] = 15;
			vectorsInOneCar["ColorSpecularWheel2"] = 16;
			vectorsInOneCar["CarWB"] = 17;
			boughtPartsInOneCar["hasTurbo"] = 0;
			for (int i = 0; i < 35; i++)
			{
				boughtPartsInOneCar["isSet" + i] = i + 1;
			}
			for (int j = 0; j < 35; j++)
			{
				boughtPartsInOneCar[j + "_"] = j + 36;
			}
		}
	}
}
