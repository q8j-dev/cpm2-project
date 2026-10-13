using UnityEngine;

namespace _Car_Parking.Scripts.Audio.Dummy
{
	public class SwitchDummy : BaseGroupTypeDummy
	{
		public object WwiseObjectReference;

		public override object ObjectReference
		{
			get
			{
				return WwiseObjectReference;
			}
			set
			{
				WwiseObjectReference = value;
			}
		}

		public override object WwiseObjectType => null;

		public override object WwiseObjectGroupType => null;

		public void SetValue(GameObject gameObject)
		{
		}
	}
}
