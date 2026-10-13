using UnityEngine;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class PlatesComposition
	{
		private readonly VehicleData _vehicle;

		public RenderTexture FrontPlateRT;

		public RenderTexture RearPlateRT;

		public Transform FrontPlatePoint;

		public Transform RearPlatePoint;

		public Material FrontPlateMat;

		public Material RearPlateMat;

		public GameObject[] Plates = new GameObject[2];

		public Transform FrontPlatePos { get; set; }

		public Transform RearPlatePos { get; set; }

		public PlatesComposition(VehicleData vehicle, Transform frontPlatePoint, Transform rearPlatePoint)
		{
			_vehicle = vehicle;
			FrontPlatePoint = frontPlatePoint;
			RearPlatePoint = rearPlatePoint;
		}

		public void DeletePlateRenderTexture(bool isFrontPlate)
		{
			if (isFrontPlate)
			{
				CommonExtensions.DeleteRenderTex(FrontPlateRT);
				FrontPlateRT = null;
				if (!FrontPlatePoint)
				{
					return;
				}
				{
					foreach (Transform item in FrontPlatePoint)
					{
						Object.Destroy(item.gameObject);
					}
					return;
				}
			}
			CommonExtensions.DeleteRenderTex(RearPlateRT);
			RearPlateRT = null;
			if (!RearPlatePoint)
			{
				return;
			}
			foreach (Transform item2 in RearPlatePoint)
			{
				Object.Destroy(item2.gameObject);
			}
		}

		public void DestroyMats()
		{
			CommonExtensions.DestroyMat(ref FrontPlateMat);
			CommonExtensions.DestroyMat(ref RearPlateMat);
		}

		public void DestroyRenderTextures()
		{
			CommonExtensions.DeleteRenderTex(FrontPlateRT);
			FrontPlateRT = null;
			CommonExtensions.DeleteRenderTex(RearPlateRT);
			RearPlateRT = null;
		}
	}
}
