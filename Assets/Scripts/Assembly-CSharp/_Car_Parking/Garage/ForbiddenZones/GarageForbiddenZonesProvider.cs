using UnityEngine.Scripting;

namespace _Car_Parking.Garage.ForbiddenZones
{
	[Preserve]
	public class GarageForbiddenZonesProvider : IForbiddenZonesProvider, IForbiddenZonesRegistrar
	{
		private IGarageForbiddenZones _current;

		public IGarageForbiddenZones Current => _current;

		public void Register(IGarageForbiddenZones provider)
		{
			_current = provider;
		}

		public void Unregister(IGarageForbiddenZones provider)
		{
			if (_current == provider)
			{
				_current = null;
			}
		}
	}
}
