using System;
using Service.Abstract;
using UnityEngine;

namespace Service.Taxi
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

		protected override TASK_STATE taskState
		{
			get
			{
				return base.taskState;
			}
			set
			{
				base.taskState = value;
				if (value == TASK_STATE.START)
				{
					Global.TaskManager.TaxiManager.OnCounter += ((Task)Global.TaskManager.Task).Counter;
				}
			}
		}

		public Task(Service.Abstract.Order order)
		{
			Init(order);
			Type = TYPE.TAXI;
		}

		public void Counter()
		{
			if (time >= 0f)
			{
				time -= Time.deltaTime;
			}
		}

		public override void AddCollision()
		{
			if (time < 0f)
			{
				base.AddCollision();
			}
			time = 3f;
		}
	}
}
