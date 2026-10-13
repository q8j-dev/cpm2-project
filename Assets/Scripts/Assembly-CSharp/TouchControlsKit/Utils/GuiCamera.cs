using UnityEngine;

namespace TouchControlsKit.Utils
{
	[RequireComponent(typeof(Camera))]
	public sealed class GuiCamera : MonoBehaviour
	{
		private static Camera myCamera;

		private static Transform myTransform;

		public static Camera guiCamera
		{
			get
			{
				if (!myCamera)
				{
					myCamera = Object.FindObjectOfType<GuiCamera>().GetComponent<Camera>();
				}
				return myCamera;
			}
		}

		public static Transform guiCameraTransform
		{
			get
			{
				if (!myTransform)
				{
					myTransform = Object.FindObjectOfType<GuiCamera>().transform;
				}
				return myTransform;
			}
		}

		public static Vector2 ScreenToWorldPoint(Vector2 position)
		{
			return guiCamera.ScreenToWorldPoint(position);
		}
	}
}
