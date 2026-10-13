using UnityEngine;
using _Car_Parking.Scripts.GameActivity;

namespace _Car_Parking.Scripts.WindowSystem
{
	public abstract class GameActivityDetailView : MonoBehaviour
	{
		protected CanvasNavigator Navigator;

		[SerializeField]
		protected DetailEventView DetailEventView;

		public abstract GameActivityType Type { get; }

		public void Render(GameActivityEntry entry, CanvasNavigator navigator)
		{
			Navigator = navigator;
			if ((bool)DetailEventView)
			{
				DetailEventView.gameObject.SetActive(value: false);
				DetailEventView.Init(entry);
			}
			OnRender(entry);
		}

		protected virtual void OnRender(GameActivityEntry entry)
		{
		}
	}
}
