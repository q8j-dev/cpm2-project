using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class ProceduralDoorBackCollider : MonoBehaviour
	{
		public Action Exit { get; private set; }

		public Action Enter { get; private set; }

		public ProceduralDoor Door { get; private set; }

		public void Init(ProceduralDoor door)
		{
			base.gameObject.name = "ProceduralDoorBackCollider";
			Enter = door.CreateCallback(isFront: false, entering: true);
			Exit = door.CreateCallback(isFront: false, entering: false);
			Door = door;
		}
	}
}
