using UnityEngine;

namespace Gley.UrbanSystem.Internal
{
	public class VisibilityScript : MonoBehaviour
	{
		private bool _readyToRemove;

		private bool _neverBeenVisible;

		public void Reset()
		{
			_neverBeenVisible = true;
			_readyToRemove = false;
		}

		private void OnBecameVisible()
		{
			_neverBeenVisible = false;
			_readyToRemove = false;
		}

		private void OnBecameInvisible()
		{
			_readyToRemove = true;
		}

		public bool IsNotInView()
		{
			if (_neverBeenVisible)
			{
				return true;
			}
			return _readyToRemove;
		}
	}
}
