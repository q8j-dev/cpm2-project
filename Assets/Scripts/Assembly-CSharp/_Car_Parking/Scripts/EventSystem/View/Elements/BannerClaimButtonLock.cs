using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.EventSystem.View.Elements
{
	[RequireComponent(typeof(Button))]
	public class BannerClaimButtonLock : MonoBehaviour, IBannerElement
	{
		private Button _button;

		public void Bind(BannerContext context)
		{
			if (!_button)
			{
				TryGetComponent<Button>(out _button);
			}
			if ((bool)_button)
			{
				_button.interactable = BannerClaimStateElement.Resolve(context) != BannerClaimStateElement.ClaimState.Claimed;
			}
		}
	}
}
