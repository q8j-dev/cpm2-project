namespace _Car_Parking.Scripts.Audio.Dummy
{
	public class BankDummy : BaseTypeDummy
	{
		public object WwiseObjectReference;

		public override object ObjectReference { get; set; }

		public override object WwiseObjectType { get; }

		public void Load(bool decodeBank = false, bool saveDecodedBank = false)
		{
		}

		public void LoadAsync(object callback = null)
		{
		}

		public void Unload()
		{
		}
	}
}
