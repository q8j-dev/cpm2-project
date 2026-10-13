using UnityEngine;
using VContainer;

public class BirdPoint : MonoBehaviour
{
	private LevelScript _levelScript;

	[Inject]
	public void Construct(LevelScript levelScript)
	{
		_levelScript = levelScript;
	}

	private void Awake()
	{
		_levelScript.Birds = base.gameObject;
	}
}
