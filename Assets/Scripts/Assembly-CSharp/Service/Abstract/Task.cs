using System;
using UnityEngine;

namespace Service.Abstract
{
	[Serializable]
	public abstract class Task : ITask
	{
		[SerializeField]
		protected TASK_STATE state;

		public TYPE Type;

		[SerializeField]
		private uint collision;

		protected virtual TASK_STATE taskState
		{
			get
			{
				return state;
			}
			set
			{
				state = value;
			}
		}

		public int CurrentItemsCount { get; set; }

		public int TotalItemsCount { get; set; }

		public Order Order { get; protected set; }

		public Action OnItemsCountChanged { get; set; }

		public virtual void AddCollision()
		{
			collision++;
		}

		public virtual uint GetCollisionCount()
		{
			return collision;
		}

		public virtual void ResetAnimation()
		{
		}

		public virtual void SetState(TASK_STATE state)
		{
			taskState = state;
		}

		public virtual TASK_STATE GetState()
		{
			return state;
		}

		public virtual void Init(Order order)
		{
			collision = 0u;
			Order = order;
			taskState = TASK_STATE.NONE;
		}
	}
}
