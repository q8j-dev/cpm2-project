Shader "CPM/URP/Misc/MaskBlend" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,0)
		[NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
		[NoScaleOffset] _FirstTex ("First Texture", 2D) = "white" {}
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		[NoScaleOffset] [Normal] _BumpMap ("NormalMap", 2D) = "bump" {}
		[Toggle(USE_MASK_BLEND)] USE_MASK_BLEND ("Use mask blending", Float) = 0
		[ToggleUI] _VERTEXAO ("VertexAO", Float) = 0
		[NoScaleOffset] _SecondTex ("Second Texture", 2D) = "white" {}
		[NoScaleOffset] _MaskTex ("MaskTex", 2D) = "white" {}
		[KeywordEnum(Main, First, Second)] _REDTEX ("RedTex", Float) = 0
		_RTile ("R channel tile", Vector) = (1,1,0,0)
		_ColorR ("R Channel color", Vector) = (1,1,1,0)
		_SmoothnessR ("SmoothnessR", Range(0, 1)) = 0.5
		[Toggle(USE_G_CHANNEL)] USE_G_CHANNEL ("Use G-channel", Float) = 0
		[KeywordEnum(Main, First, Second)] _GREENTEX ("GreenTex", Float) = 0
		_GTile ("G channel tile", Vector) = (1,1,0,0)
		_ColorG ("G Channel color", Vector) = (1,1,1,1)
		_SmoothnessG ("SmoothnessG", Range(0, 1)) = 0.5
		[Toggle(USE_B_CHANNEL)] USE_B_CHANNEL ("Use B-channel", Float) = 0
		[KeywordEnum(Main, First, Second)] _BLUETEX ("BlueTex", Float) = 0
		_BTile ("B channel tile", Vector) = (1,1,0,0)
		_ColorB ("B Channel color", Vector) = (1,1,1,0)
		_SmoothnessB ("SmoothnessB", Range(0, 1)) = 0.5
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
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}