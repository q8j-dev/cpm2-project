using UnityEngine;

namespace _Car_Parking.Scripts.Police
{
	public abstract class PoliceCarBodyKit : MonoBehaviour
	{
		[SerializeField]
		private PoliceCarLampsController[] Lamps;

		public abstract PoliceBodyKitType BodyKitType { get; }

		public int Index { get; set; } = -1;

		public PoliceCarLampsController[] PoliceCarLamps => Lamps;

		public void StartLampAnimation()
		{
			for (int i = 0; i < Lamps.Length; i++)
			{
				if (Lamps[i].gameObject.activeInHierarchy)
				{
					Lamps[i].StartAnimation();
				}
			}
		}

		public void StopLampAnimation()
		{
			for (int i = 0; i < Lamps.Length; i++)
			{
				if (Lamps[i].gameObject.activeInHierarchy)
				{
					Lamps[i].StopAnimation();
				}
			}
		}
	}
}
