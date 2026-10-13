using CarInteractionAnimationsScripts;
using UnityEngine;

namespace CarAnimationScripts
{
	public class SetterPoliceParentController : MonoBehaviour
	{
		private Transform _initialParent;

		private GameObject _policeLight;

		[field: SerializeField]
		public CarDoorType ParentType { get; private set; }

		public void SetParent(Transform parent, GameObject policeLight)
		{
			_initialParent = policeLight.transform.parent;
			_policeLight = policeLight;
			policeLight.transform.parent = parent;
		}

		public void ResetParent()
		{
			if ((bool)_policeLight && (bool)_initialParent)
			{
				_policeLight.transform.parent = _initialParent;
			}
		}
	}
}
