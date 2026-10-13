Shader "Sky/CloudHemisphere" {
	Properties {
		[Header(Noise Textures)] _BaseNoiseTex ("Base Noise (R=PerlinWorley GBA=Worley, 128^3)", 3D) = "white" {}
		_DetailNoiseTex ("Detail Noise (RGB Worley)", 3D) = "white" {}
		_CoverageTex ("Coverage (R=cov G=precip B=type)", 2D) = "grey" {}
		_BlueNoise ("Blue Noise", 2D) = "black" {}
		_BackBuffer ("BackBuffer", 2D) = "black" {}
		[Header(Atmosphere)] _EarthRadius ("Earth Radius (m)", Float) = 6371000
		_CloudLayerBottom ("Cloud Layer Bottom (m)", Float) = 1500
		_CloudLayerTop ("Cloud Layer Top (m)", Float) = 4000
		_PlanetCenter ("Planet Centre (m)", Vector) = (0,-6371000,0,0)
		[Header(Shape and Coverage)] _BaseNoiseScale ("Base Noise Scale", Float) = 3E-05
		_DetailNoiseScale ("Detail Noise Scale", Float) = 0.0004
		_DetailStrength ("Detail Strength", Range(0, 1)) = 0.35
		_CoverageScale ("Coverage Scale", Float) = 1.5E-05
		_CoverageOffset ("Coverage Offset", Vector) = (0,0,0,0)
		_CoverageScrollSpeed ("Coverage Scroll Speed", Float) = 0.2
		_CoverageMultiplier ("Coverage Multiplier", Float) = 1
		_CloudTypeBias ("Cloud Type Bias", Range(-1, 1)) = 0
		_DensityScale ("Density Scale", Float) = 1
		_PrecipDensityBoost ("Precip Density Boost", Float) = 2.5
		[Header(Wind)] _WindDirection ("Wind Direction", Vector) = (1,0,0,0)
		_CloudSpeed ("Cloud Speed (m/s)", Float) = 10
		_CloudTopOffset ("Cloud Top Skew (m)", Float) = 500
		[Header(Lighting)] _CloudExtinction ("Cloud Extinction (1/m)", Float) = 0.04
		_CloudAlbedo ("Cloud Albedo", Range(0, 1)) = 0.9
		_PhaseForward ("Phase Forward g", Range(0, 1)) = 0.8
		_PhaseBackward ("Phase Backward g", Range(-1, 0)) = -0.15
		_PhaseBlend ("Phase Blend", Range(0, 1)) = 0.5
		_MSAttenuation ("MS Attenuation (a)", Range(0, 1)) = 0.5
		_MSContribution ("MS Contribution (b)", Range(0, 1)) = 0.5
		_MSEccentricity ("MS Eccentricity (c)", Range(0, 1)) = 0.5
		_PowderStrength ("Powder Strength", Range(0, 1)) = 0.5
		_AmbientGround ("Ambient at Base", Range(0, 1)) = 0.4
		_LightStepLength ("Sun Step Length (m)", Float) = 200
		_LightConeRadius ("Sun Cone Radius (m)", Float) = 60
		_LongShadowStepMul ("Long Shadow Step Mul", Float) = 8
		[Header(March)] _MaxRayDistance ("Max Ray Distance (m)", Float) = 100000
		_MaxSteps ("Max Steps", Float) = 128
		_MinSteps ("Min Steps", Float) = 64
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
}