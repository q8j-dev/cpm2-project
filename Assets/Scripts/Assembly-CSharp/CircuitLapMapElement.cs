using UnityEngine;

public class CircuitLapMapElement : MonoBehaviour
{
	private MeshRenderer _meshRenderer;

	[SerializeField]
	private Material DefaultMaterial;

	[SerializeField]
	private Material SelectedMaterial;

	private void OnValidate()
	{
		_meshRenderer = GetComponent<MeshRenderer>();
	}

	public void Select(bool state)
	{
		_meshRenderer.material = (state ? SelectedMaterial : DefaultMaterial);
	}
}
