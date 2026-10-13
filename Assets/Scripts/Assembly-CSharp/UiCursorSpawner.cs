using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

public class UiCursorSpawner : MonoBehaviour
{
	[Header("Circular Menu")]
	[SerializeField]
	private GameObject cursorPrefab;

	private IPrefabFactory _prefabFactory;

	[Inject]
	public void Construct(IPrefabFactory prefabFactory)
	{
		_prefabFactory = prefabFactory;
	}

	private void Start()
	{
		if (_prefabFactory != null && (bool)cursorPrefab)
		{
			_prefabFactory.Instantiate(cursorPrefab);
		}
	}
}
