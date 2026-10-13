using System;
using Service.Abstract;
using UnityEngine;

namespace Service.Delivery
{
	[Serializable]
	public class Order : Service.Abstract.Order
	{
		[HideInInspector]
		public int id;

		public Order(int id, Vector3 departure, Vector3 destination, uint price, float distance, string orderName, string destinationName)
		{
			this.id = id;
			Init(departure, destination, price, distance, orderName, destinationName);
		}
	}
}
