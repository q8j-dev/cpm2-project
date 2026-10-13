using CPM.Database;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public abstract class ColorsScopeController
	{
		protected VehicleData vehicleData;

		protected CarInfo carInfo;

		protected ManagerUI managerUI;

		public abstract ColorsScope.ColorsType ColorsType { get; }

		public ColorsScopeController(ManagerUI managerUI)
		{
			this.managerUI = managerUI;
		}

		public virtual void Enable(VehicleData vehicleData, CarInfo carInfo)
		{
			this.vehicleData = vehicleData;
			this.carInfo = carInfo;
		}

		public abstract void Disable();

		public abstract void SetColor(CarPaintColor carPaintColor);

		public abstract void Apply(CarPaintColor carPaintColor);

		public abstract void Revert();

		public abstract void Discard();

		public abstract void SetDefaultColor();
	}
}
