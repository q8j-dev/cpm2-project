using System;
using CPM.DailyTasks.Daily;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Service.Abstract
{
	public abstract class PopUpWindow : IPopUpWindow
	{
		public Texture texture;

		public string title;

		public Prize prize;

		public Action<float> action;

		public abstract UniTaskVoid Create();

		public abstract void Destroy(GameObject gameObject);

		public abstract void Init();

		public abstract void Show();
	}
}
