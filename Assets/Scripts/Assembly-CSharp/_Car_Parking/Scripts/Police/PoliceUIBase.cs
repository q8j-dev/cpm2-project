using UnityEngine;
using VContainer;

namespace _Car_Parking.Scripts.Police
{
	public abstract class PoliceUIBase : MonoBehaviour
	{
		protected IPoliceSystem policeSystem;

		[Inject]
		protected void ConstructBase(IPoliceSystem policeSystem)
		{
			this.policeSystem = policeSystem;
		}
	}
}
