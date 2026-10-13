using System;
using Service.Abstract;
using UnityEngine;

namespace Service.Taxi
{
	[Serializable]
	public class Order : Service.Abstract.Order
	{
		public override void Init(Vector3 departure, Vector3 destination, uint price, float distance, string name, string location)
		{
			base.Init(departure, destination, price, distance, name, location);
		}

		public Order(Vector3 departure, Vector3 destination, uint price, float distance, string name, string location)
		{
			Init(departure, destination, price, distance, name, location);
		}
	}
}
