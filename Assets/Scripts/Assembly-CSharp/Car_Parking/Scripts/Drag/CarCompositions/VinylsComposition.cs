using System;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using UnityEngine;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class VinylsComposition : IVinylDrawPerformer
	{
		private readonly VehicleData _vehicle;

		private readonly VinylDrawer _vinylDrawer;

		private readonly PlayerDatas.VinylsDataUtility _vinylsDataUtility;

		public RenderTexture RenderTexture { get; set; }

		public OneVinylsPreset RacingCarVinylPreset { get; private set; }

		public MonoBehaviour ComponentToCheck => _vehicle;

		public VinylsComposition(VehicleData vehicle, VinylDrawer vinylDrawer, PlayerDatas.VinylsDataUtility vinylsDataUtility)
		{
			_vinylsDataUtility = vinylsDataUtility;
			_vinylDrawer = vinylDrawer;
			_vehicle = vehicle;
		}

		public void TryLoadRacingPreset(int carId)
		{
			if (RacingCarVinylPreset != null)
			{
				return;
			}
			AllVinylPreset allVinylPreset = Resources.Load<AllVinylPreset>($"LiveryPresets/PresetLiveryCar{carId}");
			if (!(allVinylPreset == null))
			{
				int count = allVinylPreset.allPresetList.Count;
				if (count != 0)
				{
					int index = UnityEngine.Random.Range(0, count);
					RacingCarVinylPreset = allVinylPreset.allPresetList[index];
				}
			}
		}

		public int GetVinylsCount(VinylsType type, bool isFrontPlate)
		{
			if (_vehicle.IsRacingCar)
			{
				if (RacingCarVinylPreset == null)
				{
					return 0;
				}
				return type switch
				{
					VinylsType.CarVinyls => (RacingCarVinylPreset.vynilOneCarList?.allVynils?.Count).GetValueOrDefault(), 
					VinylsType.WindowVinyls => (RacingCarVinylPreset.windowVynilOneCarList?.allVynils?.Count).GetValueOrDefault(), 
					_ => 0, 
				};
			}
			if ((bool)_vehicle.mppc && _vehicle.mppc.isOnlineCar)
			{
				return _vehicle.mppc.GetVinylsCount(type, isFrontPlate);
			}
			ES3File file;
			return VinylsUtility.GetVinylsCount(_vehicle.carInfo, out file, type, isFrontPlate);
		}

		public void RenderVinyls(VinylsType type, bool isFrontPlate)
		{
			try
			{
				switch (type)
				{
				case VinylsType.CarVinyls:
				{
					RenderTexture renderTexture2 = _vinylDrawer.GetRenderTexture(VinylsType.CarVinyls);
					RenderTexture = ReplaceRenderTexture(RenderTexture, renderTexture2);
					_vehicle.CarMat.mainTexture = renderTexture2;
					break;
				}
				case VinylsType.PlateVinyls:
				{
					RenderTexture renderTexture3 = _vinylDrawer.GetRenderTexture(VinylsType.PlateVinyls);
					PlatesComposition platesComposition = _vehicle.PlatesComposition;
					bool flag = platesComposition.FrontPlateRT == platesComposition.RearPlateRT;
					if (isFrontPlate)
					{
						platesComposition.FrontPlateRT = (flag ? renderTexture3 : ReplaceRenderTexture(platesComposition.FrontPlateRT, renderTexture3));
						platesComposition.FrontPlateMat.mainTexture = renderTexture3;
					}
					else
					{
						platesComposition.RearPlateRT = (flag ? renderTexture3 : ReplaceRenderTexture(platesComposition.RearPlateRT, renderTexture3));
						platesComposition.RearPlateMat.mainTexture = renderTexture3;
					}
					break;
				}
				case VinylsType.WindowVinyls:
					if ((bool)_vehicle.WindowComposition.WindowMat)
					{
						RenderTexture renderTexture = _vinylDrawer.GetRenderTexture(VinylsType.WindowVinyls);
						_vehicle.WindowComposition.WindowRT = ReplaceRenderTexture(_vehicle.WindowComposition.WindowRT, renderTexture);
						_vehicle.WindowComposition.WindowMat.mainTexture = renderTexture;
					}
					break;
				}
				if (_vehicle.IsRacingCar)
				{
					VynilOneCar oneVynil = ((type == VinylsType.WindowVinyls) ? RacingCarVinylPreset.windowVynilOneCarList : RacingCarVinylPreset.vynilOneCarList);
					_vinylsDataUtility.LoadAllVinyls(oneVynil, type);
				}
				else
				{
					_vinylsDataUtility.LoadAllVinyls(_vehicle.carInfo, type, isFrontPlate);
				}
				_vinylDrawer.FocusOnVinyls(type);
				_vinylDrawer.TakeSnapshot();
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			finally
			{
				_vinylDrawer.DeleteVinylContext(type);
			}
		}

		public void RenderNewPreset()
		{
			_ = "Assets/Resources/LiveryPresets/PresetLiveryCar" + _vehicle.CarId + ".asset";
			AllVinylPreset allVinylPreset = Resources.Load<AllVinylPreset>("LiveryPresets/PresetLiveryCar" + _vehicle.CarId);
			if (!(allVinylPreset == null))
			{
				OneVinylsPreset oneVinylsPreset = allVinylPreset.allPresetList[0];
				VynilOneCar vynilOneCarList = oneVinylsPreset.vynilOneCarList;
				if (vynilOneCarList?.allVynils != null && vynilOneCarList.allVynils.Count > 0)
				{
					RenderTexture renderTexture = _vinylDrawer.GetRenderTexture(VinylsType.CarVinyls);
					RenderTexture = ReplaceRenderTexture(RenderTexture, renderTexture);
					_vehicle.CarMat.mainTexture = renderTexture;
					_vinylsDataUtility.LoadAllVinyls(vynilOneCarList, VinylsType.CarVinyls);
					_vinylDrawer.FocusOnVinyls(VinylsType.CarVinyls);
					_vinylDrawer.TakeSnapshot();
					_vinylDrawer.DeleteVinylContext(VinylsType.CarVinyls);
				}
				VynilOneCar windowVynilOneCarList = oneVinylsPreset.windowVynilOneCarList;
				if (windowVynilOneCarList?.allVynils != null && windowVynilOneCarList.allVynils.Count > 0 && _vehicle.WindowComposition.WindowMat != null)
				{
					RenderTexture renderTexture2 = _vinylDrawer.GetRenderTexture(VinylsType.WindowVinyls);
					_vehicle.WindowComposition.WindowRT = ReplaceRenderTexture(_vehicle.WindowComposition.WindowRT, renderTexture2);
					_vehicle.WindowComposition.WindowMat.mainTexture = renderTexture2;
					_vinylsDataUtility.LoadAllVinyls(windowVynilOneCarList, VinylsType.WindowVinyls);
					_vinylDrawer.FocusOnVinyls(VinylsType.WindowVinyls);
					_vinylDrawer.TakeSnapshot();
					_vinylDrawer.DeleteVinylContext(VinylsType.WindowVinyls);
				}
			}
		}

		public void DeleteFrontPlate()
		{
			PlatesComposition platesComposition = _vehicle.PlatesComposition;
			UnityEngine.Object.Destroy(platesComposition.Plates[0]);
			platesComposition.Plates[0] = null;
			UnityEngine.Object.Destroy(platesComposition.FrontPlateMat);
			platesComposition.FrontPlateMat = null;
			DeleteRenderTexture(VinylsType.PlateVinyls, isFrontPlate: true);
		}

		public void DeleteRearPlate()
		{
			PlatesComposition platesComposition = _vehicle.PlatesComposition;
			UnityEngine.Object.Destroy(platesComposition.Plates[1]);
			platesComposition.Plates[1] = null;
			UnityEngine.Object.Destroy(platesComposition.RearPlateMat);
			platesComposition.RearPlateMat = null;
			DeleteRenderTexture(VinylsType.PlateVinyls);
		}

		public void DeletePlates()
		{
			PlatesComposition platesComposition = _vehicle.PlatesComposition;
			UnityEngine.Object.Destroy(platesComposition.Plates[0]);
			platesComposition.Plates[0] = null;
			UnityEngine.Object.Destroy(platesComposition.Plates[1]);
			platesComposition.Plates[1] = null;
			UnityEngine.Object.Destroy(platesComposition.FrontPlateMat);
			platesComposition.FrontPlateMat = null;
			UnityEngine.Object.Destroy(platesComposition.RearPlateMat);
			platesComposition.RearPlateMat = null;
			_vinylDrawer.DeleteVinylContext(VinylsType.PlateVinyls);
			DeleteRenderTexture(VinylsType.PlateVinyls, isFrontPlate: true);
			DeleteRenderTexture(VinylsType.PlateVinyls);
		}

		public void DestroyAllRenderTextures()
		{
			CommonExtensions.DeleteRenderTex(RenderTexture);
			RenderTexture = null;
			_vehicle.WindowComposition.DestroyRenderTexture();
			_vehicle.PlatesComposition.DestroyRenderTextures();
		}

		private static RenderTexture ReplaceRenderTexture(RenderTexture currentRt, RenderTexture newRt)
		{
			if ((bool)currentRt && currentRt != newRt)
			{
				CommonExtensions.DeleteRenderTex(currentRt);
			}
			return newRt;
		}

		public void DeleteRenderTexture(VinylsType rtType, bool isFrontPlate = false)
		{
			switch (rtType)
			{
			case VinylsType.CarVinyls:
				CommonExtensions.DeleteRenderTex(RenderTexture);
				RenderTexture = null;
				break;
			case VinylsType.PlateVinyls:
				_vehicle.PlatesComposition.DeletePlateRenderTexture(isFrontPlate);
				break;
			case VinylsType.WindowVinyls:
				_vehicle.WindowComposition.DestroyRenderTexture();
				break;
			case VinylsType.FlagVinyls:
				break;
			}
		}
	}
}
