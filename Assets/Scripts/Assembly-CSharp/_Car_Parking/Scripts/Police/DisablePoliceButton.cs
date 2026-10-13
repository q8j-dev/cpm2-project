namespace _Car_Parking.Scripts.Police
{
	public class DisablePoliceButton : EnableDisablePoliceButton
	{
		protected override void ButtonOnClick()
		{
			base.ButtonOnClick();
			UpdateSystem(value: false);
		}
	}
}
