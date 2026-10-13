using UnityEngine;

public class TODExposureWBDriver : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void DriveExposureWB(Vector2 exposurewb)
	{
		if ((bool)CPMGraphicsSettingsManager.Instance)
		{
			CPMGraphicsSettingsManager.Instance.SetExternalExposure(exposurewb.x);
			CPMGraphicsSettingsManager.Instance.SetExternalWhiteBallance(exposurewb.y);
		}
	}
}
