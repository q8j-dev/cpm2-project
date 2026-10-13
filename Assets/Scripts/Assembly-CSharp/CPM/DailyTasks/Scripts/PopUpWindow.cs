using CPM.DailyTasks.Daily;
using Cysharp.Threading.Tasks;
using Service;
using Service.Abstract;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CPM.DailyTasks.Scripts
{
	public class PopUpWindow : Service.Abstract.PopUpWindow
	{
		private DailyTaskWindowManager dailyTaskWindowManager;

		public override void Init()
		{
		}

		public override void Show()
		{
			dailyTaskWindowManager.Open();
		}

		public override void Destroy(GameObject gameObject)
		{
			Object.Destroy(gameObject);
			Global.PopUpWindows.Dequeue().Forget();
		}

		public override async UniTaskVoid Create()
		{
			if (!V.CoreGameManager.MenuLevelParent)
			{
				return;
			}
			GameObject gameObject = await Global.GetPopUpWindow();
			gameObject.transform.SetParent(V.CoreGameManager.MenuLevelParent.transform);
			Transform tr = gameObject.transform.parent;
			Vector3 leftPos = Vector3.one;
			Vector3 rightPos = Vector3.one;
			_ = Vector3.one;
			int coef = 1;
			Vector3 localScale;
			if (SceneManager.GetActiveScene().buildIndex == 13)
			{
				leftPos = new Vector3(1200f, 650f, 0f);
				rightPos = new Vector3(2200f, 650f, 0f);
				localScale = Vector3.one / 5f * 4f;
				coef = 2;
			}
			else
			{
				leftPos = new Vector3(730f, 250f, 0f);
				rightPos = new Vector3(1400f, 250f, 0f);
				localScale = Vector3.one / 2f;
				coef = 1;
			}
			tr.localScale = localScale;
			tr.localPosition = rightPos;
			action = (float x) =>
			{
				if (x > 0f || tr.localPosition.x > leftPos.x)
				{
					tr.localPosition += new Vector3(x * (float)coef, 0f, 0f);
				}
				if (x > 0f && tr.localPosition.x > rightPos.x)
				{
					Destroy(tr.gameObject);
				}
			};
		}

		public PopUpWindow(DailyTaskWindowManager windowManager, Texture texture, string title, Prize prize)
		{
			base.texture = texture;
			base.title = title;
			base.prize = prize;
			dailyTaskWindowManager = windowManager;
		}
	}
}
