Shader "CPM/URP/Misc/PBR_Cutout_Packed_XYSAO" {
	Properties {
		[NoScaleOffset] _Albedo ("Albedo", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		[NoScaleOffset] _NormalMask ("NormalMask", 2D) = "white" {}
		_Metalness ("Metalness", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 1
		_Tiling_And_Offset ("Tiling And Offset", Vector) = (1,1,0,0)
		[Toggle(_TRANSLUCENCY)] _TRANSLUCENCY ("Translucency", Float) = 0
		_TranslucencyPower ("TranslucencyPower", Range(0, 1)) = 0.5
		_TranslucencyFalloff ("TranslucencyFalloff", Range(0.001, 50)) = 1
		[NoScaleOffset] _NoiseTex ("NoiseTex", 2D) = "white" {}
		[Toggle(_USE_COLOR_TINT)] _USE_COLOR_TINT ("Use Color Tint", Float) = 0
		_TintA ("TintA", Vector) = (0.5,0.5,0.5,0)
		_TintB ("TintB", Vector) = (0,0,0,0)
		_TintScale ("TintScale", Float) = 0
		[NoScaleOffset] _TintMask ("TintMask (R)", 2D) = "white" {}
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
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

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}