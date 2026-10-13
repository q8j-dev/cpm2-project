using TouchControlsKit;
using UnityEngine;

public class LevelLoader2 : MonoBehaviour
{
	[SerializeField]
	private bool isMainMenu = default;

	private void Update()
	{
		if (isMainMenu)
		{
			if (TCKInput.GetButtonDown("btnFps"))
			{
				Application.LoadLevel("FirstPerson");
			}
			if (TCKInput.GetButtonDown("btnPlatf"))
			{
				Application.LoadLevel("2DPlatformer");
			}
			if (TCKInput.GetButtonDown("btnBal"))
			{
				Application.LoadLevel("TiltBallDemo");
			}
			if (TCKInput.GetButtonDown("btnCar"))
			{
				Application.LoadLevel("WheelCarDemo");
			}
		}
		else if (TCKInput.GetButtonUp("mButton"))
		{
			Application.LoadLevel("mainMenu");
		}
	}
}
