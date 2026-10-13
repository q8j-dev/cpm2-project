using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public interface IPersonActivation
	{
		bool IsActiveDriver(GameObject self);
	}
}
