using System;
using System.Collections.Generic;
using CPM.Database.Constructors;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using UniRx;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class WheelComposition
	{
		private readonly VehicleData _vehicle;

		private CurrencyManagerController _currencyManagerController;

		private bool _burntTireMessageShowed;

		private FreeDriveProvider _freeDriveProvider;

		public Material TireMat;

		public Material WheelMatLod;

		public Material WheelMatFront1;

		public Material WheelMatFront2;

		public Material WheelMatRear1;

		public Material WheelMatRear2;

		private readonly List<Material> _runtimeWheelMaterials = new List<Material>();

		private readonly Dictionary<AxleType, Material> _extraRearMat1 = new Dictionary<AxleType, Material>();

		private readonly Dictionary<AxleType, Material> _extraRearMat2 = new Dictionary<AxleType, Material>();

		public RenderTexture TireRenderTexture;

		public RenderTexture TireRenderNormalTexture;

		public float LastTireHealthRendered;

		public IReadOnlyList<Material> RuntimeWheelMaterials => _runtimeWheelMaterials;

		public IEnumerable<Material> ExtraRearMaterials1 => _extraRearMat1.Values;

		public IEnumerable<Material> ExtraRearMaterials2 => _extraRearMat2.Values;

		public GameObject SkidMark { get; set; }

		public ParticleSystem SkidSmoke { get; set; }

		public void RegisterRuntimeWheelMaterial(Material mat)
		{
			if ((bool)mat && !_runtimeWheelMaterials.Contains(mat))
			{
				_runtimeWheelMaterials.Add(mat);
			}
		}

		public void SetExtraRearMaterials(AxleType axleType, Material mat1, Material mat2)
		{
			if ((bool)mat1)
			{
				_extraRearMat1[axleType] = mat1;
			}
			else
			{
				_extraRearMat1.Remove(axleType);
			}
			if ((bool)mat2)
			{
				_extraRearMat2[axleType] = mat2;
			}
			else
			{
				_extraRearMat2.Remove(axleType);
			}
		}

		public WheelComposition(VehicleData vehicle, CurrencyManagerController currencyManagerController, FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
			_vehicle = vehicle;
			_currencyManagerController = currencyManagerController;
		}

		public void TryInitWheelComposition(bool isMineCar)
		{
			if (isMineCar)
			{
				Observable.Interval(TimeSpan.FromSeconds(1.0)).Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(2, 5))).Subscribe((long _) =>
				{
					CheckTireHealth();
				})
					.AddTo(_vehicle.gameObject);
			}
		}

		public void SetNewTire()
		{
			int num = 3000;
			if ((int)CurrencyManagerController.Money < num)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("WheelComposition.NotEnoughMoney"), 3f);
				_freeDriveProvider.FreeDriveControl.ShowActionButton(4, new LocalizedString("FreeDrive.GetMoney!"));
				return;
			}
			float num2 = 1f;
			foreach (WheelProvider allWheel in _vehicle.Axles.AllWheels)
			{
				num2 = Mathf.Min(num2, allWheel.Wheel.TyreHealth);
			}
			if (num2 < 0.5f)
			{
				++_freeDriveProvider.FreeDriveDB.tireBurnt;
			}
			foreach (WheelProvider allWheel2 in _vehicle.Axles.AllWheels)
			{
				allWheel2.SetTyreHealth(1f, allWheel2.TyreType);
				allWheel2.Temperature = 25f;
				allWheel2.DirtAmount = 0f;
			}
			ObscuredPrefs.SetFloat($"tireHealth0_{_vehicle.CarId}", 1f);
			ObscuredPrefs.SetFloat($"tireHealth1_{_vehicle.CarId}", 1f);
			ObscuredPrefs.SetFloat($"tireHealth2_{_vehicle.CarId}", 1f);
			ObscuredPrefs.SetFloat($"tireHealth3_{_vehicle.CarId}", 1f);
			LastTireHealthRendered = 1f;
			V.CoreGameManager.SetMessage(string.Concat(new LocalizedString("WheelComposition.NewTires"), $"\n-{num}"), 3f);
			AdvancedInt price = new AdvancedInt(num);
			_currencyManagerController.TryMoneySpend(out var _, price);
			_burntTireMessageShowed = false;
		}

		public void Dispose()
		{
			foreach (Material runtimeWheelMaterial in _runtimeWheelMaterials)
			{
				if ((bool)runtimeWheelMaterial)
				{
					UnityEngine.Object.Destroy(runtimeWheelMaterial);
				}
			}
			_runtimeWheelMaterials.Clear();
			CommonExtensions.DestroyMat(ref WheelMatLod);
			CommonExtensions.DestroyMat(ref WheelMatFront1);
			CommonExtensions.DestroyMat(ref WheelMatFront2);
			CommonExtensions.DestroyMat(ref WheelMatRear1);
			CommonExtensions.DestroyMat(ref WheelMatRear2);
			_extraRearMat1.Clear();
			_extraRearMat2.Clear();
			CommonExtensions.DestroyMat(ref TireMat);
			CommonExtensions.DeleteRenderTex(TireRenderTexture);
			TireRenderTexture = null;
			CommonExtensions.DeleteRenderTex(TireRenderNormalTexture);
			TireRenderNormalTexture = null;
		}

		private void UpdateHealthTire(float health)
		{
			if ((bool)TireRenderTexture)
			{
				WheelConstructor.DynamicCanvasTire.updateHealth(_vehicle.gameObject, health);
			}
			LastTireHealthRendered = health;
		}

		public void CheckTireHealth()
		{
			if (_vehicle.gameObject.activeSelf && !_vehicle.sellingWorldCar)
			{
				float num = 1f;
				List<WheelProvider> allWheels = _vehicle.Axles.AllWheels;
				for (int i = 0; i < allWheels.Count; i++)
				{
					num = Mathf.Min(num, allWheels[i].Wheel.TyreHealth);
				}
				if (num == 0f && LastTireHealthRendered != 0f)
				{
					UpdateHealthTire(num);
				}
				else if (Math.Abs(LastTireHealthRendered - num) > 0.05f)
				{
					UpdateHealthTire(num);
				}
				if (!FlagsStorage.IsMainMenu)
				{
					TryShowMessageAboutTireBurnt();
				}
			}
		}

		private bool TryShowMessageAboutTireBurnt()
		{
			if (_vehicle.gameObject != V.Car)
			{
				return false;
			}
			if (_burntTireMessageShowed)
			{
				return false;
			}
			foreach (WheelProvider allWheel in _vehicle.Axles.AllWheels)
			{
				if (allWheel.Wheel.TyreHealth < 0.01f)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("WheelComposition.ChangeTires"), 3f);
					_burntTireMessageShowed = true;
					return true;
				}
			}
			return false;
		}
	}
}
