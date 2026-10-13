using UnityEngine;

public class arrowOffsetMove : MonoBehaviour
{
	public Material arrowMat;

	private int mainTex;

	public float xSpeed = 0.1f;

	private readonly MaterialAssetStateGuard _materialGuard = new MaterialAssetStateGuard();

	private float scaleX;

	private void Start()
	{
		mainTex = Shader.PropertyToID("_BaseMap");
		_materialGuard.Capture(arrowMat);
	}

	private void OnDestroy()
	{
		_materialGuard.Restore();
	}

	private void Update()
	{
		if (Time.timeScale != 0f)
		{
			scaleX += xSpeed;
			if (Mathf.Abs(scaleX) > 500f)
			{
				scaleX = 0f;
			}
			arrowMat.SetTextureOffset(mainTex, new Vector2(scaleX, 0f));
		}
	}
}
