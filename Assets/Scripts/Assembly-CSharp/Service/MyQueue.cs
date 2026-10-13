using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Service
{
	public sealed class MyQueue<T> where T : IPopUpWindow
	{
		private ushort status;

		private ushort isBlocked;

		private Queue<T> queue;

		public MyQueue()
		{
			status = 0;
			queue = new Queue<T>();
		}

		public void Enqueue(T item)
		{
			queue.Enqueue(item);
			if (status == 0 && queue.Count == 1)
			{
				Dequeue().Forget();
				status = 1;
			}
		}

		public async UniTaskVoid Dequeue()
		{
			if (queue.Count == 0)
			{
				status = 0;
				return;
			}
			while (isBlocked == 1)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(100.0));
			}
			queue.Dequeue().Create().Forget();
		}

		public void Lock()
		{
			isBlocked = 1;
		}

		public void Unlock()
		{
			isBlocked = 0;
		}
	}
}
