using UnityEngine;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class RemoveParent : MonoBehaviour
	{
		public void Awake()
		{
			base.transform.SetParent(null);
		}
	}
}
