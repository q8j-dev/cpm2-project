using System;
using Service.Abstract;

namespace Service.Cargo
{
	[Serializable]
	public class Task : Service.Abstract.Task
	{
		public Task(Service.Abstract.Order order)
		{
			Init(order);
			Type = TYPE.CARGO;
		}
	}
}
