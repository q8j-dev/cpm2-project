using UnityEngine;

public class moveTrain : MonoBehaviour
{
	public Transform[] tweens;

	public Transform[] trainParts;

	private void Update()
	{
		for (int i = 0; i < 7; i++)
		{
			trainParts[i].position = tweens[i].position + V.GlobalOffset;
			trainParts[i].rotation = tweens[i].rotation;
		}
	}
}
