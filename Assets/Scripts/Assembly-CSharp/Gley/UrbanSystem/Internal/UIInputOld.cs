using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	public class UIInputOld : MonoBehaviour, IUIInput
	{
		private float horizontalInput;

		private float verticalInput;

		public UIInputOld Initialize()
		{
			GameObject gameObject = GameObject.Find("SteeringUI");
			if ((bool)gameObject)
			{
				gameObject.SetActive(value: false);
			}
			return this;
		}

		public float GetHorizontalInput()
		{
			return horizontalInput;
		}

		public float GetVerticalInput()
		{
			return verticalInput;
		}

		private void Update()
		{
			horizontalInput = Input.GetAxis("Horizontal");
			verticalInput = Input.GetAxis("Vertical");
		}
	}
}
