using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class ProceduralDoorFrontCollider : MonoBehaviour
	{
		public Action Exit { get; private set; }

		public Action Enter { get; private set; }

		public ProceduralDoor Door { get; private set; }

		public void Init(ProceduralDoor door)
		{
			base.gameObject.name = "ProceduralDoorFrontCollider";
			Enter = door.CreateCallback(isFront: true, entering: true);
			Exit = door.CreateCallback(isFront: true, entering: false);
			Door = door;
		}
	}
}
