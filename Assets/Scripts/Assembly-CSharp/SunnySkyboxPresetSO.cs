using UnityEngine;

[CreateAssetMenu(fileName = "SunnySkyboxPreset", menuName = "Skybox Preset/Sunny Skybox")]
public class SunnySkyboxPresetSO : ScriptableObject
{
	private readonly int _tex = Shader.PropertyToID("_Tex");

	private readonly int _tint = Shader.PropertyToID("_Tint");

	[SerializeField]
	private Cubemap _cubemap = default;

	[SerializeField]
	private Color _tintColor = default;

	public void SetPreset()
	{
		Material sunnySkybox = V.CoreGameManager.GetComponent<SceneControl>().SunnySkybox;
		if ((bool)sunnySkybox)
		{
			sunnySkybox.SetTexture(_tex, _cubemap);
			sunnySkybox.SetColor(_tint, _tintColor);
		}
	}
}
