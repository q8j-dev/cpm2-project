using CPM.Database.Constructors;
using CPMEngine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Car.CarSetter
{
	public static class TrafficCarsService
	{
		public static GameObject LoadTrafficCar(int i, Vector3 pos, Vector3 dir, LevelsProvider levelsProvider)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("traffic"), pos, Quaternion.identity);
			if ((bool)gameObject)
			{
				Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
				gameObject.transform.rotation = rotation;
				GameObject gameObject2 = Object.Instantiate(Resources.Load<GameObject>("Trigger"), pos, Quaternion.identity, gameObject.transform);
				float num = 0f;
				if (SceneManager.GetActiveScene().buildIndex == 13 || levelsProvider.CurrentLevelData.ID == 31)
				{
					num = 6f;
				}
				gameObject2.transform.SetLocalPositionAndRotation(new Vector3(0f, 0f, 5.5f + num / 2f), Quaternion.identity);
				gameObject2.transform.localScale = Vector3.one;
				if (gameObject2.transform.TryGetComponent<BoxCollider>(out var component))
				{
					component.size = new Vector3(2.2f, 2f, 14f + num);
				}
				TriggerScriptTrafficCar component2 = gameObject2.GetComponent<TriggerScriptTrafficCar>();
				component2.CarAIController = gameObject.GetComponent<CarAIController>();
				gameObject.AddComponent<TrafficCarTriger>().triggerScriptTrafficCar = component2;
				VehicleData component3 = gameObject.GetComponent<VehicleData>();
				component3.trigerScriptTraffic = component2;
				component3.LevelsProvider = levelsProvider;
				SetBodyAndWheelCar(gameObject, i, waypointCar: false);
				component3.GetPowertrain.GetDynoData(out var desc);
				component3.GetPowertrain.TrafficMode = true;
				desc.Aero.Cx = 0.36f;
				desc.Aero.FrontDownforce = 3f;
				desc.Aero.RearDownforce = 3f;
				desc.Chassis.Mass = 1600f;
				desc.Chassis.FrontPercent = 55f;
				desc.Chassis.UpPercent = 0f;
				desc.Chassis.Length = 4.7f;
				desc.Chassis.Width = 1.8f;
				desc.Chassis.Height = 1.5f;
				desc.Engine.IdleRpm = 900f;
				desc.Engine.RedlineRpm = 7000f;
				desc.Engine.MaxRpm = 8000f;
				desc.Engine.InertiaRatio = 0.21f;
				desc.Engine.MaxTorque = UnityEngine.Random.Range(300, 600);
				desc.Engine.MaxTorqueRpm = UnityEngine.Random.Range(3000, 5600);
				desc.Transmission.GearType = GearboxType.GearRwd;
				desc.Transmission.ClutchMaxTorque = 700f;
				desc.Engine.TorqueCurveSamples = Utils.GenerateTorqueCurve(desc.Engine.MaxTorque, desc.Engine.MaxTorqueRpm, 0, desc.Engine.RedlineRpm, desc.Engine.MaxRpm, 0f, 0f);
				desc.Lsd.RearLSD.Type = LSDType.Torsen;
				desc.Lsd.RearLSD.LockPercent = 50f;
				desc.GearBox.GearShiftingDelay = 0.25f;
				desc.GearBox.FinalDrive = 4.44f;
				if (gameObject.TryGetComponent<Powertrain>(out var component4))
				{
					component4.Automatic = true;
					component4.IsAutomaticGearbox = true;
					component4.NumGears = 5;
				}
				desc.Chassis.BrakeBalance = 60;
				desc.FrontSuspension.BrakeDiskTorque = 6600f;
				desc.FrontSuspension.BrakeDiscType = BrakeDiscType.VENTED;
				desc.RearSuspension.BrakeDiskTorque = 6000f;
				desc.RearSuspension.BrakeDiscType = BrakeDiscType.VENTED;
				desc.FrontSuspension.MaxSteerAngle = 36f;
				desc.FrontSuspension.Ackerman = 75f;
				desc.FrontSuspension.Caster = 6f;
				desc.FrontSuspension.Camber = -0.5f;
				desc.FrontSuspension.ToeIn = 0.05f;
				desc.RearSuspension.Camber = -0.5f;
				desc.RearSuspension.ToeIn = 0f;
				desc.FrontTyre.Pressure = 240000f;
				desc.RearTyre.Pressure = 240000f;
				desc.FrontSuspension.SpringLength = 0.35f;
				desc.FrontSuspension.Stiffness = 52000f;
				desc.FrontSuspension.Bump = 2800f;
				desc.FrontSuspension.Rebound = 5200f;
				desc.RearSuspension.SpringLength = 0.35f;
				desc.RearSuspension.Stiffness = 52000f;
				desc.RearSuspension.Bump = 2800f;
				desc.RearSuspension.Rebound = 5200f;
				component3.GetPowertrain.SetDynoData(desc, apply: true);
			}
			return gameObject;
		}

		public static GameObject LoadTC(int index, Vector3 pos)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("traffic"), pos, Quaternion.Euler(Vector3.zero));
			if ((bool)gameObject)
			{
				gameObject.AddComponent<TrafficCarTriger>();
				SetBodyAndWheelCar(gameObject, index, waypointCar: false);
			}
			return gameObject;
		}

		public static GameObject LoadTrafficCarForWaypointMove(int i, Vector3 pos)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("trafficWaypoint"), pos, Quaternion.Euler(Vector3.zero));
			if ((bool)gameObject)
			{
				GameObject gameObject2 = Object.Instantiate(Resources.Load<GameObject>("Trigger2"), pos, Quaternion.identity, gameObject.transform);
				gameObject.GetComponent<TrafficWaypointControl>().colliders[4] = gameObject2.GetComponent<BoxCollider>();
				gameObject.GetComponent<TrafficWaypointControl>().tstcw = gameObject2.GetComponent<TriggerScriptTrafficCarWaypoint>();
				float num = 0f;
				if (SceneManager.GetActiveScene().buildIndex == 13)
				{
					num = 6f;
				}
				gameObject2.transform.localPosition = new Vector3(0f, 0f, 5.5f + num / 2f);
				gameObject2.transform.localEulerAngles = Vector3.zero;
				gameObject2.transform.localScale = new Vector3(2f, 6f, 6f + num);
				gameObject2.GetComponent<TriggerScriptTrafficCarWaypoint>().twc = gameObject.GetComponent<TrafficWaypointControl>();
				SetBodyAndWheelCar(gameObject, i, waypointCar: true);
			}
			return gameObject;
		}

		private static void SetBodyAndWheelCar(GameObject car, int i, bool waypointCar)
		{
			i = math.clamp(i, 1, 10);
			Powertrain component = car.GetComponent<Powertrain>();
			string path = $"LQTraffic/body{i}";
			Transform transform = null;
			Transform transform2 = null;
			Transform transform3 = null;
			Transform transform4 = null;
			GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>(path), Vector3.zero, Quaternion.Euler(Vector3.zero), car.transform);
			Axles component3;
			if (waypointCar)
			{
				if (car.TryGetComponent<TrafficWaypointControl>(out var component2))
				{
					transform = component2.wheels[0];
					transform2 = component2.wheels[1];
					transform4 = component2.wheels[2];
					transform3 = component2.wheels[3];
				}
				component2.wheelParent.transform.localPosition = Vector3.zero;
				component2.trafficCarDataBody = gameObject.GetComponent<TraffiCarDataBody>();
				component2.colliders[5] = gameObject.GetComponent<BoxCollider>();
			}
			else if (car.TryGetComponent<Axles>(out component3))
			{
				transform = component3.FrontAxle.LeftWheel.transform;
				transform2 = component3.FrontAxle.RightWheel.transform;
				transform4 = component3.RearAxle.LeftWheel.transform;
				transform3 = component3.RearAxle.RightWheel.transform;
			}
			gameObject.transform.SetSiblingIndex(0);
			float z = 0f;
			float z2 = 0f;
			float num = 0f;
			float s = 1f;
			float y = 0f;
			float y2 = 0.64f;
			float z3 = 0f;
			WheelData wheelData = new WheelData
			{
				wheelSize = WheelDiameter.R17,
				rimWidth = RimWidth.W6,
				tyreWidth = TyreWidth.W225,
				tyreProfile = TyreProfile.P60
			};
			switch (i)
			{
			case 1:
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.4f;
				}
				else
				{
					y = -1f;
					y2 = -0.34f;
				}
				z = 1.82f;
				z2 = -1.1f;
				num = 0.77f;
				s = 1.1f;
				wheelData.wheelSize = WheelDiameter.R18;
				wheelData.rimWidth = RimWidth.W8;
				wheelData.tyreWidth = TyreWidth.W265;
				wheelData.tyreProfile = TyreProfile.P60;
				break;
			case 2:
				z3 = -1.273f;
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.6f;
				}
				else
				{
					y = -1.15f;
					y2 = -0.4f;
				}
				z = 0.84f;
				z2 = -3.54f;
				num = 1.06f;
				s = 1.7f;
				wheelData.wheelSize = WheelDiameter.R22;
				wheelData.rimWidth = RimWidth.W11;
				wheelData.tyreWidth = TyreWidth.W385;
				wheelData.tyreProfile = TyreProfile.P80;
				break;
			case 3:
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.33f;
				}
				else
				{
					y = -0.93f;
					y2 = -0.4f;
				}
				z = 1.93f;
				z2 = -1.93f;
				num = 0.89f;
				s = 1f;
				break;
			case 4:
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.34f;
				}
				else
				{
					y = -0.93f;
					y2 = -0.38f;
				}
				z = 1.62f;
				z2 = -1.46f;
				num = 0.81f;
				s = 1.05f;
				wheelData.wheelSize = WheelDiameter.R17;
				wheelData.rimWidth = RimWidth.W7;
				wheelData.tyreWidth = TyreWidth.W235;
				wheelData.tyreProfile = TyreProfile.P65;
				break;
			case 5:
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.34f;
				}
				else
				{
					y = -0.93f;
					y2 = -0.36f;
				}
				z = 1.98f;
				z2 = -1.87f;
				num = 0.89f;
				s = 1f;
				break;
			case 6:
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.36f;
				}
				else
				{
					y = -0.93f;
					y2 = -0.4f;
				}
				z = 1.37f;
				z2 = -1.35f;
				num = 0.81f;
				s = 1.05f;
				wheelData.wheelSize = WheelDiameter.R17;
				wheelData.rimWidth = RimWidth.W7;
				wheelData.tyreWidth = TyreWidth.W235;
				wheelData.tyreProfile = TyreProfile.P65;
				break;
			case 7:
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.34f;
				}
				else
				{
					y = -0.93f;
					y2 = -0.4f;
				}
				z = 1.573f;
				z2 = -1.338f;
				num = 0.81f;
				s = 1.05f;
				wheelData.wheelSize = WheelDiameter.R17;
				wheelData.rimWidth = RimWidth.W7;
				wheelData.tyreWidth = TyreWidth.W235;
				wheelData.tyreProfile = TyreProfile.P65;
				break;
			case 8:
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.33f;
				}
				else
				{
					y = -0.93f;
					y2 = -0.32f;
				}
				z = 1.36f;
				z2 = -1.28f;
				num = 0.79f;
				s = 1f;
				break;
			case 9:
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.37f;
				}
				else
				{
					y = -1f;
					y2 = -0.42f;
				}
				z = 1.45f;
				z2 = -1.54f;
				num = 0.84f;
				s = 1.1f;
				wheelData.wheelSize = WheelDiameter.R18;
				wheelData.rimWidth = RimWidth.W8;
				wheelData.tyreWidth = TyreWidth.W265;
				wheelData.tyreProfile = TyreProfile.P60;
				break;
			case 10:
				if (waypointCar)
				{
					y = 0f;
					y2 = 0.37f;
				}
				else
				{
					y = -1f;
					y2 = -0.4f;
				}
				z = 1.459f;
				z2 = -1.22f;
				num = 0.81f;
				s = 1.1f;
				wheelData.wheelSize = WheelDiameter.R18;
				wheelData.rimWidth = RimWidth.W8;
				wheelData.tyreWidth = TyreWidth.W265;
				wheelData.tyreProfile = TyreProfile.P60;
				break;
			}
			Vector3 localPosition = new Vector3(0f, y, z3);
			if ((bool)transform)
			{
				transform.localPosition = new Vector3(0f - num, y2, z);
			}
			if ((bool)transform2)
			{
				transform2.localPosition = new Vector3(num, y2, z);
			}
			if ((bool)transform4)
			{
				transform4.localPosition = new Vector3(0f - num, y2, z2);
			}
			if ((bool)transform3)
			{
				transform3.localPosition = new Vector3(num, y2, z2);
			}
			if (!waypointCar)
			{
				component.SetCollider(gameObject.GetComponent<BoxCollider>());
				if ((bool)car.GetComponent<TrafficCarTriger>())
				{
					car.GetComponent<TrafficCarTriger>().trafficCarDataBody = gameObject.GetComponent<TraffiCarDataBody>();
				}
			}
			if (car.TryGetComponent<Axles>(out var component4))
			{
				foreach (WheelProvider allWheel in component4.AllWheels)
				{
					if ((bool)allWheel)
					{
						allWheel.Model = allWheel.GetComponentInChildren<TrafficWheel>().gameObject;
					}
				}
				component.InitWheel(WheelIndex.FrontLeft, component4.FrontAxle.LeftWheel);
				component.InitWheel(WheelIndex.FrontRight, component4.FrontAxle.RightWheel);
				component.InitWheel(WheelIndex.RearLeft, component4.RearAxle.LeftWheel);
				component.InitWheel(WheelIndex.RearRight, component4.RearAxle.RightWheel);
				component.SetWheelParams(AxleType.Front, wheelData);
				component.SetWheelParams(AxleType.Rear, wheelData);
			}
			WheelConstructor.SetWheelScaleTrafficCar(car, s);
			gameObject.transform.SetLocalPositionAndRotation(localPosition, Quaternion.identity);
			gameObject.name = "body";
		}

		public static void RemoveOldBodyAndSet(GameObject car, int i, bool WaypointCar)
		{
			GameObject gameObject = car.transform.Find("body").gameObject;
			if ((bool)gameObject)
			{
				Object.Destroy(gameObject);
				PlayerDatas.countToUnload++;
				if (PlayerDatas.countToUnload > 20)
				{
					PlayerDatas.countToUnload = 0;
				}
			}
			SetBodyAndWheelCar(car, i, WaypointCar);
		}
	}
}
