using System;
using System.Collections.Generic;
using CPM.Database;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Car
{
	[CreateAssetMenu(fileName = "BadCarConfig", menuName = "Configs/BadCarConfig")]
	public class BadCarConfig : ScriptableObject
	{
		[Serializable]
		private struct WheelData
		{
			public Vector3 LocalPosition;

			public Vector3 TopPos;
		}

		private const string SourceCarResourcePath = "car132";

		public Color Color = new Color(1f, 0.31f, 0.89f);

		public Material CarMaterial;

		public Vector3 MainModelPosition;

		public int PlacesCount;

		public TargetPositionRecorder MainTPR;

		public TargetPositionRecorder[] AllTargets;

		[SerializeField]
		private WheelData[] Wheels;

		public void Apply(VehicleData vehicleData, CarInfo carInfo, int carIdToSet)
		{
			if (!vehicleData || carInfo == null)
			{
				return;
			}
			carInfo.Colors.CarColor = new CarPaintColor(Color);
			vehicleData.BlockSellThisCar = true;
			vehicleData.CarId = carIdToSet;
			vehicleData.StockMaterial = CarMaterial;
			vehicleData.CarMat = CarMaterial;
			if (vehicleData.TryGetComponent<BodyKits>(out var component))
			{
				component.BumperCountRear = 2;
				component.BumperCountFront = 2;
				component.SkirtsCount = 2;
				component.HoodCount = 0;
				component.FenderCount = 0;
				component.PaidSpoilers = new PaidExteriorPart[1]
				{
					new PaidExteriorPart
					{
						Cost = 0,
						Id = 0,
						PartType = BodyKitType.Spoiler
					}
				};
				component.PaidFrontBumpers = new PaidExteriorPart[1]
				{
					new PaidExteriorPart
					{
						Cost = 0,
						Id = 0,
						PartType = BodyKitType.FrontBumper
					}
				};
				component.PaidRearBumpers = new PaidExteriorPart[1]
				{
					new PaidExteriorPart
					{
						Cost = 0,
						Id = 0,
						PartType = BodyKitType.RearBumper
					}
				};
				component.PaidSkirts = new PaidExteriorPart[1]
				{
					new PaidExteriorPart
					{
						Cost = 0,
						Id = 0,
						PartType = BodyKitType.Skirt
					}
				};
				component.PaidHoods = Array.Empty<PaidExteriorPart>();
				component.PaidIntakeHood = new PaidExteriorPart[1]
				{
					new PaidExteriorPart
					{
						Cost = 0,
						Id = 0,
						PartType = BodyKitType.AirIntakeHood
					}
				};
				component.PaidIntakeRoof = new PaidExteriorPart[1]
				{
					new PaidExteriorPart
					{
						Cost = 0,
						Id = 0,
						PartType = BodyKitType.AirIntakeRoof
					}
				};
				component.PaidFenders = Array.Empty<PaidExteriorPart>();
				component.PaidTrims = Array.Empty<PaidExteriorPart>();
			}
			AllPersonPoints componentInChildren = vehicleData.GetComponentInChildren<AllPersonPoints>(includeInactive: true);
			Vector3 vector = vehicleData.MainModelPosition - MainModelPosition;
			if ((bool)componentInChildren)
			{
				if (MainTPR != null)
				{
					if (componentInChildren.mainTPR == null)
					{
						componentInChildren.mainTPR = new TargetPositionRecorder();
					}
					CopyRecorderValues(MainTPR, componentInChildren.mainTPR);
				}
				if (AllTargets != null)
				{
					if (componentInChildren.allTargets == null || componentInChildren.allTargets.Length != AllTargets.Length)
					{
						componentInChildren.allTargets = ResizePreserve(componentInChildren.allTargets, AllTargets.Length);
					}
					for (int i = 0; i < AllTargets.Length; i++)
					{
						if (componentInChildren.allTargets[i] == null)
						{
							componentInChildren.allTargets[i] = new TargetPositionRecorder();
						}
						CopyRecorderValues(AllTargets[i], componentInChildren.allTargets[i]);
						componentInChildren.allTargets[i].ParentPosion += vector;
					}
				}
				if (componentInChildren.allPlaces == null || componentInChildren.allPlaces.Length != PlacesCount)
				{
					componentInChildren.allPlaces = ResizePreserve(componentInChildren.allPlaces, PlacesCount);
				}
				if (componentInChildren.allTargets != null)
				{
					int num = Math.Min(componentInChildren.allTargets.Length, componentInChildren.allPlaces.Length);
					for (int j = 0; j < num; j++)
					{
						ApplyRecorderToTransforms(componentInChildren.allTargets[j], componentInChildren.allPlaces[j]);
					}
				}
				if (vehicleData.TryGetComponent<DriverController>(out var component2))
				{
					IReadOnlyList<GameObject> members = vehicleData.VehicleMembersController.Members;
					for (int k = 0; k < members.Count; k++)
					{
						GameObject gameObject = members[k];
						if ((bool)gameObject)
						{
							DriverActionController component3 = gameObject.GetComponent<DriverActionController>();
							if ((bool)component3)
							{
								component3.initDone = false;
							}
							if (k == 0 && gameObject.TryGetComponent<MainPerson>(out var component4) && (bool)component4.ik)
							{
								component2.SetHandWheelToSteeringWheel(gameObject);
							}
							if ((bool)component3)
							{
								component3.UpdateIK();
							}
						}
					}
				}
			}
			if (!vehicleData.TryGetComponent<Axles>(out var component5) || component5.AllWheels == null || Wheels == null)
			{
				return;
			}
			int num2 = Math.Min(component5.AllWheels.Count, Wheels.Length);
			for (int l = 0; l < num2; l++)
			{
				WheelProvider wheelProvider = component5.AllWheels[l];
				if ((bool)wheelProvider)
				{
					WheelData wheelData = Wheels[l];
					wheelProvider.transform.localPosition = wheelData.LocalPosition + vector;
					wheelProvider.TopPos = wheelData.TopPos + vector;
					if (wheelProvider.Wheel != null)
					{
						wheelProvider.Wheel.TopPos = wheelData.TopPos + vector;
					}
				}
			}
			for (int m = num2; m < component5.AllWheels.Count; m++)
			{
				WheelProvider wheelProvider2 = component5.AllWheels[m];
				if ((bool)wheelProvider2)
				{
					wheelProvider2.gameObject.SetActive(value: false);
					if (wheelProvider2.Wheel != null)
					{
						wheelProvider2.Wheel.Enabled = false;
					}
				}
			}
		}

		private static TargetPositionRecorder CloneRecorder(TargetPositionRecorder source)
		{
			if (source == null)
			{
				return null;
			}
			TargetPositionRecorder targetPositionRecorder = new TargetPositionRecorder();
			JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(source), targetPositionRecorder);
			return targetPositionRecorder;
		}

		private static void CopyRecorderValues(TargetPositionRecorder src, TargetPositionRecorder dst)
		{
			dst.LHandPosition = src.LHandPosition;
			dst.LHandRot = src.LHandRot;
			dst.RHandPosition = src.RHandPosition;
			dst.RHandRot = src.RHandRot;
			dst.LUpperArmPosition = src.LUpperArmPosition;
			dst.RUpperArmPosition = src.RUpperArmPosition;
			dst.mainBodyPosition = src.mainBodyPosition;
			dst.LFootPosition = src.LFootPosition;
			dst.LFootRot = src.LFootRot;
			dst.RFootPosition = src.RFootPosition;
			dst.RFootRot = src.RFootRot;
			dst.LBendGoalPosition = src.LBendGoalPosition;
			dst.RBendGoalPosition = src.RBendGoalPosition;
			dst.LLegBendGoalPosition = src.LLegBendGoalPosition;
			dst.RLegBendGoalPosition = src.RLegBendGoalPosition;
			dst.HeadPosition = src.HeadPosition;
			dst.HeadRotationEuler = src.HeadRotationEuler;
			dst.ShifterPos = src.ShifterPos;
			dst.ShifterRot = src.ShifterRot;
			dst.ParentPosion = src.ParentPosion;
			dst.ParentRot = src.ParentRot;
		}

		private static void ApplyRecorderToTransforms(TargetPositionRecorder tpr, Transform place)
		{
			if (tpr != null)
			{
				if ((bool)place)
				{
					place.localPosition = tpr.ParentPosion;
					place.localEulerAngles = tpr.ParentRot;
				}
				SetLocal(tpr.LHand, tpr.LHandPosition, tpr.LHandRot);
				SetLocal(tpr.RHand, tpr.RHandPosition, tpr.RHandRot);
				SetLocal(tpr.LUpperArm, tpr.LUpperArmPosition, Vector3.zero);
				SetLocal(tpr.RUpperArm, tpr.RUpperArmPosition, Vector3.zero);
				SetLocal(tpr.mainBody, tpr.mainBodyPosition, Vector3.zero);
				SetLocal(tpr.LFoot, tpr.LFootPosition, tpr.LFootRot);
				SetLocal(tpr.RFoot, tpr.RFootPosition, tpr.RFootRot);
				SetLocal(tpr.LBendGoal, tpr.LBendGoalPosition, Vector3.zero);
				SetLocal(tpr.RBendGoal, tpr.RBendGoalPosition, Vector3.zero);
				SetLocal(tpr.LLegBendGoal, tpr.LLegBendGoalPosition, Vector3.zero);
				SetLocal(tpr.RLegBendGoal, tpr.RLegBendGoalPosition, Vector3.zero);
				SetLocal(tpr.Head, tpr.HeadPosition, tpr.HeadRotationEuler);
				SetLocal(tpr.Shifter, tpr.ShifterPos, tpr.ShifterRot);
			}
		}

		private static void SetLocal(GameObject go, Vector3 localPos, Vector3 localEuler)
		{
			if ((bool)go)
			{
				go.transform.localPosition = localPos;
				go.transform.localEulerAngles = localEuler;
			}
		}

		private static T[] ResizePreserve<T>(T[] src, int newLength)
		{
			T[] array = new T[newLength];
			if (src != null)
			{
				int num = Math.Min(src.Length, newLength);
				for (int i = 0; i < num; i++)
				{
					array[i] = src[i];
				}
			}
			return array;
		}
	}
}
