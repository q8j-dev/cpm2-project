Shader "CPM/URP/Automotive/Automotive_XYMAO" {
	Properties {
		[NoScaleOffset] _BaseMap ("Albedo", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,0)
		[NoScaleOffset] _NormalMask ("NormalMask", 2D) = "grey" {}
		[Toggle(_NORMALMAP1)] _NORMALMAP1 ("HasNormal", Float) = 0
		_Metallic ("Metalness", Range(0, 1)) = 0
		[Toggle(_METALNESS)] _METALNESS ("HasMetalness", Float) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		[Toggle(_SMOOTHNESS)] _SMOOTHNESS ("HasSmoothness", Float) = 0
		_OcclusionStrength ("Occlusion", Range(0, 1)) = 1
		[Toggle(_OCCLUSIONMAP)] _OCCLUSIONMAP ("HasAO", Float) = 0
		_Tiling_And_Offset ("Tiling And Offset", Vector) = (1,1,0,0)
		[Toggle(_EMISSION)] _EMISSION ("Emission", Float) = 0
		[NoScaleOffset] _EmissionMap ("Emission Map", 2D) = "white" {}
		[HDR] _EmissionColor ("Emission Color", Vector) = (0,0,0,0)
		[Toggle(_USEDIRT)] _USEDIRT ("UseDirt", Float) = 0
		_FakeBottomAO ("FakeBottomAO", Range(0, 1)) = 0
		_VertAO ("VertAO", Range(0, 1)) = 1
		_MaskValue ("Dirt Amount", Range(0, 1)) = 0
		_MaskColor ("Dirt color", Vector) = (0.9058824,0.7529412,0.5176471,1)
		_MaskAddTex ("Dirt Albedo", 2D) = "white" {}
		_StencilRef ("_StencilRef", Float) = 1
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