using System;
using Cysharp.Threading.Tasks;
using Service.Abstract;
using Service.Manager;
using UnityEngine;

namespace Service
{
	public static class Global
	{
		public static Service.Manager.TaskManager TaskManager;

		public static readonly MyQueue<IPopUpWindow> PopUpWindows = new MyQueue<IPopUpWindow>();

		private static GameObject _popUpWindow;

		public static void GetTrailer(out ClientController trailer)
		{
			trailer = null;
			if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MultiPersonControl>(out var component))
			{
				GameObject currentCar = component.currentCar;
				if ((bool)currentCar)
				{
					ConfigurableJoint component2 = currentCar.GetComponent<ConfigurableJoint>();
					trailer = (((bool)component2 && (bool)component2.connectedBody) ? component2.connectedBody.GetComponent<ClientController>() : null);
				}
			}
		}

		public static async UniTask<GameObject> GetPopUpWindow()
		{
			if (_popUpWindow == null)
			{
				ResourceRequest rr = Resources.LoadAsync("BattlePass/Prefabs/PopUpWindow");
				while (!rr.isDone)
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
				}
				Debug.Log("loading popup");
				_popUpWindow = rr.asset as GameObject;
			}
			return _popUpWindow;
		}
	}
}
