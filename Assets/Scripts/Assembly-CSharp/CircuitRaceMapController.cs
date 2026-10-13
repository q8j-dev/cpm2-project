using UnityEngine;
using VContainer;

public class CircuitRaceMapController : MonoBehaviour
{
	private CircuitRaceControlller _circuitRaceControlller;

	[SerializeField]
	private float MapUICaneraSize = 552.4f;

	[SerializeField]
	private float DefaultMapUICaneraSize = 363.5843f;

	[SerializeField]
	private Transform MapCameraUIPlaceTransform;

	[SerializeField]
	private Transform MapCameraUIDefaultPlaceTransform;

	[SerializeField]
	private Camera MapCamera;

	[SerializeField]
	private MapCamera MapCameraScript;

	[SerializeField]
	private Transform CarTransform;

	private RenderTexture _renderTexture;

	private Texture _capturedTexture;

	public int textureWidth = 512;

	public int textureHeight = 512;

	[Inject]
	public void Construct(CircuitRaceControlller circuitRaceControlller)
	{
		_circuitRaceControlller = circuitRaceControlller;
	}

	public MapCamera GetMapCamera()
	{
		return MapCameraScript;
	}

	public Texture GetMapTexture()
	{
		return _capturedTexture;
	}

	public void OnSelectRaceTrack()
	{
	}

	public void OnMapSelected()
	{
		MapCamera.Render();
	}

	public void SetMapForUICamera(bool state)
	{
	}
}
