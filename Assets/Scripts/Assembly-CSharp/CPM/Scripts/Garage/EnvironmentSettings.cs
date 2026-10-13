using UnityEngine;
using UnityEngine.Rendering;

namespace CPM.Scripts.Garage
{
	public class EnvironmentSettings
	{
		public Material Skybox;

		public Light Sun;

		public Color SubtractiveShadowColor;

		public AmbientMode AmbientMode;

		public Color AmbientSkyColor;

		public Color AmbientEquatorColor;

		public Color AmbientGroundColor;

		public DefaultReflectionMode ReflectionMode;

		public Cubemap CustomReflection;

		public float ReflectionIntensity;

		public int ReflectionBounces;

		public bool Fog;

		public static EnvironmentSettings FromCurrent()
		{
			return new EnvironmentSettings
			{
				Skybox = RenderSettings.skybox,
				Sun = RenderSettings.sun,
				SubtractiveShadowColor = RenderSettings.subtractiveShadowColor,
				AmbientMode = RenderSettings.ambientMode,
				AmbientSkyColor = RenderSettings.ambientSkyColor,
				AmbientEquatorColor = RenderSettings.ambientEquatorColor,
				AmbientGroundColor = RenderSettings.ambientGroundColor,
				ReflectionMode = RenderSettings.defaultReflectionMode,
				CustomReflection = RenderSettings.customReflection,
				ReflectionIntensity = RenderSettings.reflectionIntensity,
				ReflectionBounces = RenderSettings.reflectionBounces,
				Fog = RenderSettings.fog
			};
		}

		public void Apply()
		{
			RenderSettings.skybox = Skybox;
			RenderSettings.sun = Sun;
			RenderSettings.subtractiveShadowColor = SubtractiveShadowColor;
			RenderSettings.ambientMode = AmbientMode;
			RenderSettings.ambientSkyColor = AmbientSkyColor;
			RenderSettings.ambientEquatorColor = AmbientEquatorColor;
			RenderSettings.ambientGroundColor = AmbientGroundColor;
			RenderSettings.defaultReflectionMode = ReflectionMode;
			RenderSettings.customReflection = CustomReflection;
			RenderSettings.reflectionIntensity = ReflectionIntensity;
			RenderSettings.reflectionBounces = ReflectionBounces;
			RenderSettings.fog = Fog;
		}
	}
}
