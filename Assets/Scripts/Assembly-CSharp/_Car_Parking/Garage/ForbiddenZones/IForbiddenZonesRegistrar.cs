namespace _Car_Parking.Garage.ForbiddenZones
{
	public interface IForbiddenZonesRegistrar
	{
		void Register(IGarageForbiddenZones provider);

		void Unregister(IGarageForbiddenZones provider);
	}
}
