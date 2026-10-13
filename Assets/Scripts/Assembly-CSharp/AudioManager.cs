using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	public AudioSource ambientSound;

	public AudioSource serviceInsideSound;

	private void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance == this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void EnterGarage()
	{
	}

	public void EnterCarService()
	{
	}

	public void GoOutside()
	{
	}

	public void TurnOffAll()
	{
	}
}
