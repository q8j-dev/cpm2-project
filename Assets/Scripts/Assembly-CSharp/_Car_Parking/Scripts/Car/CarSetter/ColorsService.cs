using CPM.Database;
using CPMEngine;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Car.CarSetter
{
	public class ColorsService : IInitializable
	{
		private ICarInfoService _carInfoService;

		private CarMatType _carMatType;

		private DefaultMaterialsConfig _defaultMaterialsConfig;

		private CarDirtConfig _dirtConfig;

		public static Material LowWheelMaterial;

		private static readonly int Color1 = Shader.PropertyToID("_Color");

		private static readonly int MaskAddTex = Shader.PropertyToID("_MaskAddTex");

		private static readonly int CracksNormal = Shader.PropertyToID("_CracksNormal");

		private static readonly int ShatterTileOffset = Shader.PropertyToID("_ShatterTileOffset");

		[Inject]
		public void Construct(ICarInfoService carInfoService, DefaultMaterialsConfig defaultMaterialsConfig, CarDirtConfig carDirtConfig)
		{
			_defaultMaterialsConfig = defaultMaterialsConfig;
			_carInfoService = carInfoService;
			LowWheelMaterial = _defaultMaterialsConfig.LowWheelMaterial;
			_dirtConfig = carDirtConfig;
		}

		public void Initialize()
		{
		}

		private Material CreateCarMaterial(CarInfo info)
		{
			Material material = new Material(_defaultMaterialsConfig.CarBody);
			if (info != null)
			{
				material.SetColor(ShaderPropertyIDs.DirtColor, info.Colors.DirtColor);
				material.SetFloat(ShaderPropertyIDs.DirtAmount, info.Colors.DirtAmount);
				SetCarPaintColor(material, info.Colors.CarColor);
			}
			return material;
		}

		public void SetDirtTexture(int carId, Material carMat, int dirtMaskId)
		{
			Texture dirtTexture = _dirtConfig.GetDirtTexture(carId);
			if ((bool)dirtTexture)
			{
				carMat.SetTexture(dirtMaskId, dirtTexture);
			}
		}

		public void CreateFirstMat(VehicleData vehicleData)
		{
			if (!vehicleData.IsFirstMatCreated)
			{
				vehicleData.IsFirstMatCreated = true;
				vehicleData.CarMat = CreateCarMaterial(vehicleData.mppc.isOnlineCar ? null : vehicleData.carInfo);
				vehicleData.CarMat.name = (vehicleData.StockMaterial ? vehicleData.StockMaterial.name : vehicleData.CarMat.name) + " (Instance)";
				int dirtMaskId = Shader.PropertyToID("_MaskTex");
				SetDirtTexture(vehicleData.CarId, vehicleData.CarMat, dirtMaskId);
				vehicleData.BodyLoadingComposition.UpdateMaterial();
				if (!vehicleData.isTraffiCar)
				{
					vehicleData.DirtComposition.GetCarDirt();
				}
				if (((bool)vehicleData.LevelsProvider.CurrentLevelData && vehicleData.LevelsProvider.CurrentLevelData.Type == LevelType.Chasing) || !vehicleData.IsRacingCar)
				{
					vehicleData.OnCarColorChanged(vehicleData.CarMat.color);
					return;
				}
				Color color = Random.ColorHSV();
				vehicleData.CarMat.color = color;
				vehicleData.CarMat.SetColor(Color1, color);
				vehicleData.OnCarColorChanged(vehicleData.CarMat.color);
			}
		}

		public static void SetCarPaintColor(Material mat, CarPaintColor lastColorComplex)
		{
			if (mat == null)
			{
				Debug.LogWarning("No material");
				return;
			}
			mat.color = lastColorComplex.Color;
			lastColorComplex.CarMatType.ApplyTypeOnMat(mat);
			mat.SetInt(ShaderPropertyIDs.PaintType, (int)lastColorComplex.CarMatType);
			mat.SetFloat(ShaderPropertyIDs.IridescenceThickness, lastColorComplex.IridescenceThickness);
			mat.SetFloat(ShaderPropertyIDs.IridescencePower, lastColorComplex.IridescencePower);
		}

		public Material CreateWindowMaterial(Vector4 holeParameters, Vector4 wipersTilingOffset, float wipersScale, bool wipersVerticalInvert, bool wipersHorizontalInvert)
		{
			Shader shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveWindshieldGlass");
			Material material = new Material(_defaultMaterialsConfig.Window);
			material.name = "WindowMat";
			material.EnableKeyword(new LocalKeyword(shader, "_USEDIRT"));
			material.SetKeyword(new LocalKeyword(shader, "_WIPERSVERTICALINVERT"), wipersVerticalInvert);
			material.SetKeyword(new LocalKeyword(shader, "_WIPERSHORIZONTALINVERT"), wipersHorizontalInvert);
			material.SetTextureScale(MaskAddTex, new Vector2(10f, 10f));
			material.SetTextureScale(CracksNormal, new Vector2(15f, 15f));
			material.SetVector(ShatterTileOffset, holeParameters);
			material.SetVector(ShaderPropertyIDs.WipersTilingOffset, wipersTilingOffset);
			material.SetFloat(ShaderPropertyIDs.WipersScale, wipersScale);
			material.SetFloat(ShaderPropertyIDs.WipersValue, 0f);
			material.SetFloat(ShaderPropertyIDs.RainOverlayValue, 0f);
			material.SetFloat(ShaderPropertyIDs.InvertWipers, 0f);
			return material;
		}
	}
}
