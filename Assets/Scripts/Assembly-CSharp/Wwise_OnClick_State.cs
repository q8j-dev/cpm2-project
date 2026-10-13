using AK.Wwise;
using UnityEngine;

public class Wwise_OnClick_State : MonoBehaviour
{
	public State[] FilterOffState;

	public void WiseFilterOff()
	{
		State[] filterOffState = FilterOffState;
		for (int i = 0; i < filterOffState.Length; i++)
		{
			filterOffState[i].SetValue();
		}
	}
}
