using System.Collections.Generic;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Drag;

namespace CPMEngine
{
	[RequireComponent(typeof(Rigidbody))]
	[ExecuteInEditMode]
	public class Axles : MonoBehaviour
	{
		private bool _isTrailer;

		public bool DualAxles;

		public List<WheelProvider> AllWheels;

		public Axle FrontAxle = new Axle();

		public Axle RearAxle = new Axle();

		public Axle Rear1Axle = new Axle();

		public Axle Rear2Axle = new Axle();

		private Powertrain _powertrain;

		private Rigidbody _rigidbody;

		private VehicleEntityProvider _vehicleEntityProvider;

		private SoundController _soundController;

		private CarChildsReferences _carChildsReferences;

		public ReactiveProperty<bool> IsBrakePressed;

		public bool HasPneumaticSuspension { get; set; }

		public bool SuspensionsEditing { get; set; }

		public void GetTyreHealth(Axle axle, out float left, out float right)
		{
			left = 0f;
			right = 0f;
			if (axle != null && axle.Wheels.Count > 0)
			{
				if ((bool)axle.LeftWheel)
				{
					left = axle.LeftWheel.Wheel.TyreHealth;
				}
				if ((bool)axle.RightWheel)
				{
					right = axle.RightWheel.Wheel.TyreHealth;
				}
				else
				{
					right = left;
				}
				if (!axle.LeftWheel)
				{
					left = right;
				}
			}
		}

		public void SetTyreHealth(Axle axle, float health, TyreType tyreType)
		{
			if (axle == null || axle.Wheels.Count <= 0)
			{
				return;
			}
			foreach (WheelProvider wheel in axle.Wheels)
			{
				wheel.SetTyreHealth(health, tyreType);
				wheel.Temperature = 25f;
			}
		}

		public void SetTyreHealth(Axle axle, float[] condition, TyreType tyreType)
		{
			if (axle.Wheels == null)
			{
				return;
			}
			foreach (WheelProvider wheel in axle.Wheels)
			{
				if (wheel.Wheel != null)
				{
					wheel.SetTyreHealth(wheel.IsLeft ? condition[0] : condition[1], tyreType);
					wheel.Temperature = 25f;
				}
			}
		}

		public Axle GetAxle(WheelSidePos wheelSidePos)
		{
			if (wheelSidePos != WheelSidePos.REAR)
			{
				return FrontAxle;
			}
			return RearAxle;
		}

		private void Awake()
		{
			IsBrakePressed = new ReactiveProperty<bool>();
			TryGetComponent<Powertrain>(out _powertrain);
			TryGetComponent<Rigidbody>(out _rigidbody);
			TryGetComponent<VehicleEntityProvider>(out _vehicleEntityProvider);
			TryGetComponent<SoundController>(out _soundController);
			TryGetComponent<CarChildsReferences>(out _carChildsReferences);
		}

		private void OnEnable()
		{
			if ((bool)_powertrain)
			{
				_powertrain.OnUpdateChassisEvent += UpdateHeatingBrakes;
				_powertrain.OnUpdateChassisEvent += UpdateAirBag;
			}
			if ((bool)_soundController)
			{
				if (FrontAxle != null)
				{
					FrontAxle.OnPneumaticDown += _soundController.PneumaticFrontSuspensionDown;
					FrontAxle.OnPneumaticUp += _soundController.PneumaticFrontSuspensionUp;
				}
				if (RearAxle != null)
				{
					RearAxle.OnPneumaticDown += _soundController.PneumaticRearSuspensionDown;
					RearAxle.OnPneumaticUp += _soundController.PneumaticRearSuspensionUp;
				}
			}
		}

		private void OnDisable()
		{
			if ((bool)_powertrain)
			{
				_powertrain.OnUpdateChassisEvent -= UpdateHeatingBrakes;
				_powertrain.OnUpdateChassisEvent += UpdateAirBag;
			}
			if ((bool)_soundController)
			{
				if (FrontAxle != null)
				{
					FrontAxle.OnPneumaticDown -= _soundController.PneumaticFrontSuspensionDown;
					FrontAxle.OnPneumaticUp -= _soundController.PneumaticFrontSuspensionUp;
				}
				if (RearAxle != null)
				{
					RearAxle.OnPneumaticDown -= _soundController.PneumaticRearSuspensionDown;
					RearAxle.OnPneumaticUp -= _soundController.PneumaticRearSuspensionUp;
				}
			}
		}

		public void BrakeRPC(string value)
		{
			IsBrakePressed.Value = value == "1";
		}

		private void UpdateHeatingBrakes(Powertrain powertrain)
		{
			if (!IsAxleHeatable(FrontAxle) || !IsAxleHeatable(RearAxle))
			{
				FrontAxle.CoolingToDefault();
				RearAxle.CoolingToDefault();
			}
			else
			{
				HeatDiskFront();
				HeatDiskRear();
				UpdateTyreHealth();
			}
		}

		private static bool IsAxleHeatable(Axle axle)
		{
			if (axle.LeftWheel == null && axle.RightWheel == null)
			{
				return false;
			}
			if (axle.LeftWheel != null && !axle.LeftWheel.Wheel.Enabled)
			{
				return false;
			}
			if (axle.RightWheel != null && !axle.RightWheel.Wheel.Enabled)
			{
				return false;
			}
			return true;
		}

		private void UpdateTyreHealth()
		{
			if ((bool)_vehicleEntityProvider && !_powertrain.IsRaceCar && (!_vehicleEntityProvider || !_vehicleEntityProvider.IsMultiplayerCar))
			{
				float[] frontTyreHealth = _vehicleEntityProvider.CarEntity.MaintenanceInfo.FrontTyreHealth;
				if (frontTyreHealth != null && frontTyreHealth.Length > 1)
				{
					FrontAxle?.UpdateTyreHealth(out _vehicleEntityProvider.CarEntity.MaintenanceInfo.FrontTyreHealth[0], out _vehicleEntityProvider.CarEntity.MaintenanceInfo.FrontTyreHealth[1]);
				}
				frontTyreHealth = _vehicleEntityProvider.CarEntity.MaintenanceInfo.RearTyreHealth;
				if (frontTyreHealth != null && frontTyreHealth.Length > 1)
				{
					RearAxle?.UpdateTyreHealth(out _vehicleEntityProvider.CarEntity.MaintenanceInfo.RearTyreHealth[0], out _vehicleEntityProvider.CarEntity.MaintenanceInfo.RearTyreHealth[1]);
				}
			}
		}

		private void UpdateAirBag(Powertrain powertrain)
		{
			if (HasPneumaticSuspension && (bool)_vehicleEntityProvider)
			{
				FrontAxle?.UpdateSuspension(_rigidbody);
				RearAxle?.UpdateSuspension(_rigidbody);
				if (Rear1Axle.Wheels.Count > 0)
				{
					Rear1Axle?.UpdateSuspension(_rigidbody);
				}
				if (Rear2Axle.Wheels.Count > 0)
				{
					Rear2Axle?.UpdateSuspension(_rigidbody);
				}
			}
		}

		private void HeatDiskFront()
		{
			float getAvrWheelsSpeed = FrontAxle.GetAvrWheelsSpeed;
			if (_powertrain.OnLineCar)
			{
				_powertrain.Brake = (IsBrakePressed.Value ? 1 : 0);
			}
			if (IsBrakePressed.Value && getAvrWheelsSpeed > 3f)
			{
				FrontAxle?.ApplyBrakingForce(getAvrWheelsSpeed, _powertrain.BrakeFrontBalance, _powertrain.Brake);
			}
			else
			{
				FrontAxle?.CoolDownBrake(getAvrWheelsSpeed);
			}
			if (FrontAxle == null)
			{
				return;
			}
			foreach (WheelProvider wheel in FrontAxle.Wheels)
			{
				if ((bool)wheel.WheelModel && (bool)wheel.WheelModel.BrakeDiskMaterial)
				{
					wheel.WheelModel.BrakeDiskMaterial.SetColor(ShaderPropertyIDs.EmissionColor, FrontAxle.GetBrakeDiskHeatColor);
				}
			}
		}

		private void HeatDiskRear()
		{
			float getAvrWheelsSpeed = RearAxle.GetAvrWheelsSpeed;
			if (_powertrain.OnLineCar)
			{
				_powertrain.Brake = (IsBrakePressed.Value ? 1 : 0);
			}
			if (IsBrakePressed.Value && getAvrWheelsSpeed > 3f)
			{
				float brakeBalance = 1f - _powertrain.BrakeFrontBalance;
				RearAxle?.ApplyBrakingForce(getAvrWheelsSpeed, brakeBalance, _powertrain.Brake);
				if (Rear1Axle.Wheels.Count > 0)
				{
					Rear1Axle.ApplyBrakingForce(getAvrWheelsSpeed, brakeBalance, _powertrain.Brake);
				}
				if (Rear2Axle.Wheels.Count > 0)
				{
					Rear2Axle.ApplyBrakingForce(getAvrWheelsSpeed, brakeBalance, _powertrain.Brake);
				}
			}
			else
			{
				RearAxle?.CoolDownBrake(getAvrWheelsSpeed);
				if (Rear1Axle.Wheels.Count > 0)
				{
					Rear1Axle?.CoolDownBrake(getAvrWheelsSpeed);
				}
				if (Rear2Axle.Wheels.Count > 0)
				{
					Rear2Axle?.CoolDownBrake(getAvrWheelsSpeed);
				}
			}
			if (RearAxle != null)
			{
				foreach (WheelProvider wheel in RearAxle.Wheels)
				{
					if ((bool)wheel.WheelModel && (bool)wheel.WheelModel.BrakeDiskMaterial)
					{
						wheel.WheelModel.BrakeDiskMaterial.SetColor(ShaderPropertyIDs.EmissionColor, RearAxle.GetBrakeDiskHeatColor);
					}
				}
			}
			if (Rear1Axle != null)
			{
				foreach (WheelProvider wheel2 in Rear1Axle.Wheels)
				{
					if ((bool)wheel2.WheelModel && (bool)wheel2.WheelModel.BrakeDiskMaterial)
					{
						wheel2.WheelModel.BrakeDiskMaterial.SetColor(ShaderPropertyIDs.EmissionColor, Rear1Axle.GetBrakeDiskHeatColor);
					}
				}
			}
			if (Rear2Axle == null)
			{
				return;
			}
			foreach (WheelProvider wheel3 in Rear2Axle.Wheels)
			{
				if ((bool)wheel3.WheelModel && (bool)wheel3.WheelModel.BrakeDiskMaterial)
				{
					wheel3.WheelModel.BrakeDiskMaterial.SetColor(ShaderPropertyIDs.EmissionColor, Rear2Axle.GetBrakeDiskHeatColor);
				}
			}
		}
	}
}
