using UnityEngine;
using UnityEngine.Rendering;
using VacuumShaders.TerrainToMesh;

[AddComponentMenu("VacuumShaders/Terrain To Mesh/Example/Runtime Converter")]
public class RunTime_Terrain_Convertion : MonoBehaviour
{
	public Terrain sourceTerrain;

	public TerrainConvertInfo convertInfo;

	public bool generateBasemap;

	public bool attachMeshCollider;

	private void Start()
	{
		if (!(sourceTerrain != null))
		{
			return;
		}
		Mesh[] array = TerrainToMeshConverter.Convert(sourceTerrain, convertInfo, _normalizeUV: false);
		if (array == null)
		{
			return;
		}
		Material material = null;
		material = ((!generateBasemap) ? GenerateMaterial_Splatmap() : GenerateMaterial_Basemap());
		if (array.Length == 1)
		{
			MeshFilter meshFilter = base.gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				meshFilter = base.gameObject.AddComponent<MeshFilter>();
			}
			meshFilter.sharedMesh = array[0];
			MeshRenderer meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
			if (meshRenderer == null)
			{
				meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			}
			meshRenderer.sharedMaterial = material;
			if (attachMeshCollider)
			{
				base.gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
			}
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = new GameObject(array[i].name);
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			MeshFilter meshFilter2 = gameObject.AddComponent<MeshFilter>();
			meshFilter2.sharedMesh = array[i];
			gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
			if (attachMeshCollider)
			{
				gameObject.AddComponent<MeshCollider>().sharedMesh = meshFilter2.sharedMesh;
			}
		}
	}

	private Material GenerateMaterial_Basemap()
	{
		Texture2D _diffuseMap = null;
		Texture2D _normalMap = null;
		bool sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
		TerrainToMeshConverter.ExtractBasemap(sourceTerrain, out _diffuseMap, out _normalMap, 1024, 1024, sRGB);
		Material material = null;
		if (GraphicsSettings.defaultRenderPipeline == null)
		{
			material = new Material(Shader.Find((_normalMap != null) ? "Legacy Shaders/Bumped Diffuse" : "Legacy Shaders/Diffuse"));
			material.SetTexture("_MainTex", _diffuseMap);
			material.SetColor("_Color", Color.white);
			if (_normalMap != null)
			{
				material.SetTexture("_BumpMap", _normalMap);
			}
		}
		else
		{
			Shader shader = Shader.Find("Lightweight Render Pipeline/Lit");
			if (shader == null)
			{
				shader = Shader.Find("VacuumShaders/Terrain To Mesh/SRP Default");
			}
			material = new Material(shader);
			if (material.HasProperty("_MainTex"))
			{
				material.SetTexture("_MainTex", _diffuseMap);
			}
			if (material.HasProperty("_BaseMap"))
			{
				material.SetTexture("_BaseMap", _diffuseMap);
			}
			material.SetColor("_Color", Color.white);
			if (_normalMap != null)
			{
				material.SetTexture("_BumpMap", _normalMap);
			}
		}
		return material;
	}

	private Material GenerateMaterial_Splatmap()
	{
		Material material = null;
		Texture2D[] array = TerrainToMeshConverter.ExtractSplatmaps(sourceTerrain);
		if (array == null || array.Length == 0)
		{
			return material;
		}
		int num = TerrainToMeshConverter.ExtractTexturesInfo(sourceTerrain, out var _diffuseTextures, out var _normalTextures, out var _uvScale, out var _uvOffset, out var _metalic, out var _smoothness);
		if (num == 0 || _diffuseTextures == null)
		{
			Debug.LogWarning("usedTexturesCount == 0");
			return material;
		}
		if (num == 1)
		{
			Shader shader = null;
			if (GraphicsSettings.defaultRenderPipeline == null)
			{
				shader = Shader.Find("Legacy Shaders/Diffuse");
				if (shader != null)
				{
					material = new Material(shader);
					material.SetTexture("_MainTex", _diffuseTextures[0]);
					material.SetTextureScale("_MainTex", _uvScale[0]);
					material.SetTextureOffset("_MainTex", _uvOffset[0]);
				}
			}
			else
			{
				shader = Shader.Find("Lightweight Render Pipeline/Lit");
				if (shader != null)
				{
					material = new Material(shader);
					material.SetTexture("_BaseMap", _diffuseTextures[0]);
					material.SetTextureScale("_BaseMap", _uvScale[0]);
					material.SetTextureOffset("_BaseMap", _uvOffset[0]);
				}
			}
			return material;
		}
		num = Mathf.Clamp(num, 2, 8);
		bool flag = false;
		if (_normalTextures != null && num < 5)
		{
			flag = true;
		}
		string text = ((!(GraphicsSettings.defaultRenderPipeline == null)) ? $"VacuumShaders/Terrain To Mesh/Lightweight Render Pipeline/Lit/{num} Textures" : string.Format("VacuumShaders/Terrain To Mesh/Standard/" + (flag ? "Bumped" : "Diffuse") + "/{0} Textures", num));
		Shader shader2 = Shader.Find(text);
		if (shader2 == null)
		{
			if (GraphicsSettings.defaultRenderPipeline == null)
			{
				Debug.LogWarning("Shader not found: " + text);
			}
			else
			{
				Debug.LogWarning("Shader not found: '" + text + "'.\nLightweight Render Pipeline shaders (http://u3d.as/1jFw) are not installed.\n");
			}
			return material;
		}
		material = new Material(shader2);
		if (array.Length == 1)
		{
			material.SetTexture("_V_T2M_Control", array[0]);
		}
		else
		{
			if (array.Length > 2)
			{
				Debug.Log("TerrainToMesh shaders support max 2 control textures. Current terrain uses " + array.Length);
			}
			material.SetTexture("_V_T2M_Control", array[0]);
			material.SetTexture("_V_T2M_Control2", array[1]);
		}
		for (int i = 0; i < num; i++)
		{
			material.SetTexture($"_V_T2M_Splat{i + 1}", _diffuseTextures[i]);
			material.SetFloat($"_V_T2M_Splat{i + 1}_uvScale", _uvScale[i].x);
			material.SetFloat($"_V_T2M_Splat{i + 1}_Metallic", _metalic[i]);
			material.SetFloat($"_V_T2M_Splat{i + 1}_Glossiness", _smoothness[i]);
			if (flag)
			{
				material.SetTexture($"_V_T2M_Splat{i + 1}_bumpMap", _normalTextures[i]);
			}
		}
		return material;
	}
}
