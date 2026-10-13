using System;
using Service.Abstract;
using Service.Taxi;
using UnityEngine;

namespace Service.Delivery
{
	[Serializable]
	public class Task : Service.Abstract.Task
	{
		[SerializeField]
		private float collisionTime = 3f;

		private float time
		{
			get
			{
				return collisionTime;
			}
			set
			{
				collisionTime = value;
				if (value < 0f)
				{
					Global.TaskManager.OnCancelReward += V.DriverPerson.GetComponent<TaxiController>().CancelExtraReward;
				}
			}
		}

		public Task(Service.Abstract.Order order)
		{
			Init(order);
			Type = TYPE.DELIVERY;
		}
	}
}
