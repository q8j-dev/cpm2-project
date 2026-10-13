using UnityEngine;

public class PitStopController : MonoBehaviour
{
	[SerializeField]
	private PitStopTrigger[] PitStopTriggers = default;

	public PitStopTrigger GetAvaliblePitStopTrigger()
	{
		PitStopTrigger[] pitStopTriggers = PitStopTriggers;
		foreach (PitStopTrigger pitStopTrigger in pitStopTriggers)
		{
			if (!pitStopTrigger.IsBusy)
			{
				return pitStopTrigger;
			}
		}
		return null;
	}
}
