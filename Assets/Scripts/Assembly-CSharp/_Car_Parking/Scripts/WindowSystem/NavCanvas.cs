using UnityEngine;

namespace _Car_Parking.Scripts.WindowSystem
{
	public abstract class NavCanvas : MonoBehaviour
	{
		public abstract string Id { get; }

		public virtual string Title => "Title";

		protected CanvasNavigator Navigator { get; private set; }

		public void Bind(CanvasNavigator navigator)
		{
			Navigator = navigator;
		}

		public virtual void Open(object data)
		{
		}

		public void Show()
		{
			base.gameObject.SetActive(value: true);
		}

		public void Hide()
		{
			base.gameObject.SetActive(value: false);
		}

		protected virtual void OnEnable()
		{
		}

		protected virtual void OnDisable()
		{
		}

		protected virtual void Start()
		{
		}
	}
}
