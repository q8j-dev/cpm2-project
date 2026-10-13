namespace _Car_Parking.Scripts.Police
{
	public class EnablePoliceButton : EnableDisablePoliceButton
	{
		protected override void ButtonOnClick()
		{
			base.ButtonOnClick();
			UpdateSystem(value: true);
		}
	}
}
