using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class loadCityParts : MonoBehaviour
{
	public int partNumber;

	public bool loadingNow;

	public new Collider collider;

	private AsyncOperationHandle<GameObject> _loadingHandle;

	private bool _isReleased;

	private void Start()
	{
		collider = GetComponent<Collider>();
	}
}
