using UnityEngine;

public class waterMaterial : MonoBehaviour
{
	public Material waterMat;

	private int mainTex;

	private int NormalTex;

	public float xSpeed = 0.1f;

	public float ySpeed = 0.1f;

	private float x = 1f;

	private float y = 1f;

	private void Start()
	{
		mainTex = Shader.PropertyToID("_MainTex");
		NormalTex = Shader.PropertyToID("_BumpMap");
		if (!waterMat)
		{
			waterMat = GetComponent<Renderer>().material;
		}
		if ((bool)waterMat && !waterMat.HasProperty(mainTex))
		{
			waterMat = null;
		}
		if ((bool)waterMat)
		{
			Vector2 textureScale = waterMat.GetTextureScale(mainTex);
			x = textureScale.x;
			y = textureScale.y;
		}
	}

	private void Update()
	{
		if ((bool)waterMat)
		{
			float num = Mathf.Cos(Time.time) * xSpeed + x;
			float num2 = Mathf.Sin(Time.time) * ySpeed + y;
			waterMat.SetTextureScale(mainTex, new Vector2(num, num2));
		}
		else
		{
			base.enabled = false;
		}
	}
}
