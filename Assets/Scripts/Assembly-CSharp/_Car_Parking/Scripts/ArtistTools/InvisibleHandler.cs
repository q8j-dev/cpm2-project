using UnityEngine;

namespace _Car_Parking.Scripts.ArtistTools
{
	public class InvisibleHandler : MonoBehaviour
	{
		private IInvisibleRegister _handler;

		public void Init(IInvisibleRegister handler)
		{
			_handler = handler;
		}

		public void OnBecameInvisible()
		{
			if (_handler != null && _handler.IsTriggerActivated)
			{
				_handler.UnregisterInvisible(this);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
