using UnityEngine;

public class waterAnimation : MonoBehaviour
{
	public float scrollSpeed = 0.5f;

	private float offset;

	public Material m;

	private int text1;

	private int text2;

	private int BumpMap1;

	private int BumpMap2;

	private int _DistortionMap1;

	public int skip;

	private void Start()
	{
		m = GetComponent<MeshRenderer>().material;
		text1 = Shader.PropertyToID("_Texture1");
		text2 = Shader.PropertyToID("_Texture2");
		BumpMap1 = Shader.PropertyToID("_BumpMap1");
		BumpMap2 = Shader.PropertyToID("_BumpMap2");
	}
}
