using UnityEngine;
using _Car_Parking.Scripts.Other.ProceduralDoors;

namespace _Car_Parking.Scripts
{
	public class EnableIfPlayerStaysInCollider : MonoBehaviour
	{
		public bool Invert;

		public Collider Collider;

		private void Awake()
		{
			if ((bool)Collider)
			{
				Collider.isTrigger = true;
				Collider.gameObject.EnsureComponent<StayInColliderLink>(out var _).Init(this);
				base.gameObject.SetActive(Invert);
			}
		}

		public void OnEnter()
		{
			if (base.gameObject.activeSelf == Invert)
			{
				base.gameObject.SetActive(!Invert);
				Debug.Log("[Enable If Player stays] Entered collider", base.gameObject);
			}
		}

		public void OnExit()
		{
			if (base.gameObject.activeSelf == !Invert)
			{
				base.gameObject.SetActive(Invert);
				Debug.Log("[Enable If Player stays] Exited collider", base.gameObject);
			}
		}
	}
}
