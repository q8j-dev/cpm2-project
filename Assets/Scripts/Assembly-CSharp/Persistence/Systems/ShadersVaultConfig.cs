using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace Persistence.Systems
{
	[CreateAssetMenu(menuName = "Configs/ShadersVault", fileName = "ShadersVault")]
	public class ShadersVaultConfig : ScriptableObject
	{
		public const string ResourceShadersPath = "Shaders";

		public Shader CarPaintShader;

		public Shader CarWindowShader;

		public Shader CarLightsShader;

		public Shader CarSimpleShader;

		public Shader LitLiteShader;

		public Shader BenchmarkShader;

		private readonly Dictionary<string, Shader> _resourceShaders = new Dictionary<string, Shader>();

		public void Init()
		{
			GlobalEventSystem.OnShaderRequired += RequireShaderHandler;
		}

		private void OnDestroy()
		{
			GlobalEventSystem.OnShaderRequired -= RequireShaderHandler;
		}

		private Shader RequireShaderHandler(string shaderName)
		{
			Shader shader = Shader.Find(shaderName);
			if ((bool)shader)
			{
				return shader;
			}
			Shader shader2 = shaderName switch
			{
				"CPM/URP/Automotive/CarPaint" => CarPaintShader, 
				"CPM/URP/Automotive/AutomotiveWindshieldGlass" => CarWindowShader, 
				"CPM/URP/Automotive/AutomotiveLights" => CarLightsShader, 
				"CPM/URP/Automotive/AutomotiveSimple" => CarSimpleShader, 
				"CPM/URP/LitLite" => LitLiteShader, 
				_ => null, 
			};
			if ((bool)shader2)
			{
				return shader2;
			}
			if (_resourceShaders.TryGetValue(shaderName, out var value))
			{
				return value;
			}
			Shader shader3 = Resources.Load<Shader>("Shaders/" + shaderName);
			if (!shader3)
			{
				return null;
			}
			_resourceShaders[shaderName] = shader3;
			return shader3;
		}

		public void Clear()
		{
			_resourceShaders.Clear();
		}
	}
}
