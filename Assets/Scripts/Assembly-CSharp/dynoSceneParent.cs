using UnityEngine;
using VContainer;

public class dynoSceneParent : MonoBehaviour
{
	public Transform[] frontAxles;

	public Transform[] rearAxles;

	public DynoGraphFill graphFill;

	public Transform point;

	[Inject]
	public void Construct(DynoRunController dynoRunController)
	{
		dynoRunController.dynoSceneParent = this;
	}
}
