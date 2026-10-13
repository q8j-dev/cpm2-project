using UnityEngine;

namespace _Car_Parking.Scripts.Person
{
	public class SwitchHeadController : MonoBehaviour
	{
		private const float DistanceToDisableHead = 0.2f;

		private readonly Vector3 _hiddenHeadScale = new Vector3(0.4f, 0f, 0f);

		[field: SerializeField]
		public Transform Head { get; private set; }

		public bool IsHeadHidden { get; private set; }

		public void SwitchHead(bool value)
		{
			Head.localScale = (value ? Vector3.one : _hiddenHeadScale);
			IsHeadHidden = !value;
		}
	}
}
