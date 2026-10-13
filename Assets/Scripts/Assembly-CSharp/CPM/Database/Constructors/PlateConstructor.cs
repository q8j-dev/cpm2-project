using CarAnimationScripts;
using Car_Parking.Scripts.Drag.CarCompositions;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace CPM.Database.Constructors
{
	public class PlateConstructor
	{
		public static void SetPlates(VehicleData vehicleData, CarInfo carInfo)
		{
			vehicleData.VinylsComposition.DeletePlates();
			int num = PlateDataUtility.TryGetPlateIdForCar(carInfo.CarInstanceId, isFrontPlate: true);
			if (num != 0)
			{
				InstantiatePlate(isFront: true, num, vehicleData);
			}
			int num2 = PlateDataUtility.TryGetPlateIdForCar(carInfo.CarInstanceId);
			if (num2 != 0)
			{
				InstantiatePlate(isFront: false, num2, vehicleData);
			}
		}

		public static void InstantiatePlate(bool isFront, int plateId, VehicleData vehicleData)
		{
			PlatesComposition platesComposition = vehicleData.PlatesComposition;
			if (isFront)
			{
				if (platesComposition.FrontPlatePoint == null)
				{
					return;
				}
				GlobalEventSystem.InvokePlateCreated(plateId, vehicleData, isFront: true);
				platesComposition.Plates[0] = Object.Instantiate(Resources.Load("Other/Plate_" + plateId % 2, typeof(GameObject)), platesComposition.FrontPlatePoint) as GameObject;
				if ((bool)platesComposition.FrontPlatePos)
				{
					platesComposition.Plates[0].transform.position = platesComposition.FrontPlatePos.position;
				}
				platesComposition.Plates[0].SetActive(value: true);
				Object.Destroy(platesComposition.Plates[0].GetComponent<MeshCollider>());
				platesComposition.FrontPlateMat = platesComposition.Plates[0].GetComponent<Renderer>().material;
			}
			else
			{
				if (platesComposition.RearPlatePoint == null)
				{
					return;
				}
				GlobalEventSystem.InvokePlateCreated(plateId, vehicleData, isFront: false);
				platesComposition.Plates[1] = Object.Instantiate(Resources.Load("Other/Plate_" + plateId % 2, typeof(GameObject)), platesComposition.RearPlatePoint) as GameObject;
				if ((bool)platesComposition.RearPlatePos)
				{
					platesComposition.Plates[1].transform.position = platesComposition.RearPlatePos.position;
				}
				platesComposition.Plates[1].SetActive(value: true);
				Object.Destroy(platesComposition.Plates[1].GetComponent<MeshCollider>());
				platesComposition.RearPlateMat = platesComposition.Plates[1].GetComponent<Renderer>().material;
			}
			Transform plateTransform = (isFront ? platesComposition.FrontPlatePoint : platesComposition.RearPlatePoint);
			if (!vehicleData.gameObject.TryGetComponent<CarAnimationsController>(out var carAnimationsController))
			{
				return;
			}
			if (vehicleData.CarId == 197)
			{
				if (isFront)
				{
					return;
				}
				isFront = true;
			}
			if ((bool)carAnimationsController.CarAnimationReferences)
			{
				carAnimationsController.CarAnimationReferences.SetInitialParentForPlate(plateTransform, isFront);
				return;
			}
			(from _ in Observable.EveryUpdate()
				where carAnimationsController.CarAnimationReferences
				select _).First().Subscribe((long _) =>
			{
				carAnimationsController.CarAnimationReferences.SetInitialParentForPlate(plateTransform, isFront);
			}).AddTo(carAnimationsController);
		}
	}
}
