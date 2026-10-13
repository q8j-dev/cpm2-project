namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct ComboOnlySlots
	{
		public readonly int Header;

		public readonly int Modifier;

		public readonly int Main;

		public ComboOnlySlots(int header, int modifier, int main)
		{
			Header = header;
			Modifier = modifier;
			Main = main;
		}
	}
}
