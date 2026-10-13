using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Drag;

public class CircuitFinishSplashWindow : WindowScript
{
	private const string PlayFinishingSoundEffect = "Play_Finishing_Effect";

	protected override void OnEnable()
	{
		AudioWrapper.PostEvent("Play_Finishing_Effect", base.gameObject);
		base.OnEnable();
	}
}
