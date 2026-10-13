using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TouchControlsKit
{
	public abstract class ControllerBase : MonoBehaviour
	{
		[Serializable]
		public class TCKEvent : UnityEvent<float, float>
		{
		}

		public RectTransform touchzoneRect;

		public Image touchzoneImage;

		public float sensitivity = 1f;

		[SerializeField]
		protected string myName = "NONAME_Controller";

		[SerializeField]
		protected bool showTouchZone = true;

		[SerializeField]
		protected Color32 nativeColor;

		public Axes axisX = new Axes("Horizontal");

		public Axes axisY = new Axes("Vertical");

		public bool broadcast;

		protected int touchId = -1;

		protected bool touchDown;

		[SerializeField]
		public TCKEvent downEvent;

		[SerializeField]
		public TCKEvent pressEvent;

		[SerializeField]
		public TCKEvent upEvent;

		[SerializeField]
		public TCKEvent clickEvent;

		protected Vector2 defaultPosition;

		protected Vector2 currentPosition;

		protected Vector2 currentDirection;

		public string MyName
		{
			get
			{
				return myName;
			}
			set
			{
				if (!(myName == value))
				{
					if (value == string.Empty)
					{
						Debug.LogError("ERROR: Controller name for " + base.gameObject.name + " cannot be empty");
						return;
					}
					myName = value;
					base.gameObject.name = myName;
				}
			}
		}

		public bool ShowTouchZone
		{
			get
			{
				return showTouchZone;
			}
			set
			{
				if (showTouchZone != value)
				{
					showTouchZone = value;
					ShowHideTouchZone();
				}
			}
		}

		public virtual void ControlAwake()
		{
			touchzoneRect = base.gameObject.GetComponent<RectTransform>();
			touchzoneImage = base.gameObject.GetComponent<Image>();
		}

		internal abstract void UpdatePosition(Vector2 touchPos);

		internal virtual void ControlReset()
		{
			touchId = -1;
			touchDown = false;
			SetAxis(0f, 0f);
		}

		protected virtual void ShowHideTouchZone()
		{
			if (showTouchZone)
			{
				touchzoneImage.color = nativeColor;
				return;
			}
			nativeColor = touchzoneImage.color;
			Color32 color = nativeColor;
			color.a = 0;
			touchzoneImage.color = color;
		}

		protected void SetAxis(float x, float y)
		{
			axisX.SetValue(x);
			axisY.SetValue(y);
		}

		protected void DownHandler()
		{
			if (broadcast)
			{
				downEvent.Invoke(axisX.value, axisY.value);
			}
		}

		internal void PressHandler()
		{
			if (broadcast && touchDown)
			{
				pressEvent.Invoke(axisX.value, axisY.value);
			}
		}

		protected void UpHandler()
		{
			if (broadcast)
			{
				upEvent.Invoke(axisX.value, axisY.value);
			}
		}

		protected void ClickHandler()
		{
			if (broadcast)
			{
				clickEvent.Invoke(axisX.value, axisY.value);
			}
		}
	}
}
