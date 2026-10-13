using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class qualitySet : MonoBehaviour
{
	public int qualityLOD = 2;

	private GlobalCollectionsProvider _provider;

	[Inject]
	public void Construct(GlobalCollectionsProvider provider)
	{
		_provider = provider;
	}

	private void Awake()
	{
		_provider?.RegisterQualitySet(this);
	}

	public void Start()
	{
		base.gameObject.SetActive(CPMGraphicsSettingsManager.Instance.Quality >= qualityLOD);
	}

	private void OnDestroy()
	{
		_provider?.UnregisterQualitySet(this);
	}
}
