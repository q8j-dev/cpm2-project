using VContainer.Unity;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.Car.Physics
{
	public class MozaPollService : ITickable
	{
		private readonly RegistryHandle<GearVariable> _gearHandle = new RegistryHandle<GearVariable>(EventRegistryIDs.Gear);

		public void Tick()
		{
		}

		private GearEnum GetGear(int gear)
		{
			return gear switch
			{
				-1 => GearEnum.R, 
				0 => GearEnum.N, 
				1 => GearEnum.G1, 
				2 => GearEnum.G2, 
				3 => GearEnum.G3, 
				4 => GearEnum.G4, 
				5 => GearEnum.G5, 
				6 => GearEnum.G6, 
				7 => GearEnum.G7, 
				_ => GearEnum.N, 
			};
		}
	}
}
