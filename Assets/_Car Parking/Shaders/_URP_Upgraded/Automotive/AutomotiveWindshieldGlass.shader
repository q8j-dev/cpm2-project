Shader "CPM/URP/Automotive/AutomotiveWindshieldGlass" {
	Properties {
		_Color ("Color", Vector) = (0,0.4823529,0.3686275,0.09411765)
		_MainTex ("Vinyl Texture", 2D) = "black" {}
		_FakeBottomAO ("FakeBottomAO", Range(0, 1)) = 0
		_VertAO ("VertAO", Range(0, 1)) = 1
		_MaskAddTex ("Dirt Albedo", 2D) = "white" {}
		_MaskColor ("Dirt color", Vector) = (0.9058824,0.7529412,0.5176471,1)
		_MaskValue ("Dirt Amount", Range(0, 1)) = 0
		[Normal] _CracksNormal ("CracksNormal", 2D) = "bump" {}
		_Shattered ("Shattered", Range(0, 1)) = 0
		_StencilRef ("_StencilRef", Float) = 1
		[NoScaleOffset] _ShatterTexture ("ShatterTexture", 2D) = "white" {}
		_ShatterTileOffset ("ShatterTileOffset", Vector) = (1,1,0,0)
		_ShatterHole ("ShatterHole", Range(0, 1)) = 0
		_WipersValue ("WipersValue", Range(0, 1)) = 0
		_WipersScale ("WipersScale", Float) = 1.5
		_RainOverlayValue ("RainOverlayValue", Range(0, 1)) = 0
		_InvertWipers ("InvertWipers", Float) = 0
		_WipersTilingOffset ("Wipers Transform", Vector) = (0,-1.5,0.2,0.7)
		[Toggle(_WIPERSVERTICALINVERT)] _WIPERSVERTICALINVERT ("_WipersVerticalInvert", Float) = 0
		[Toggle(_WIPERSHORIZONTALINVERT)] _WIPERSHORIZONTALINVERT ("_WipersHorizontalInvert", Float) = 0
		_FPRainBlend ("FPRainBlend", Float) = 1
		[NoScaleOffset] _SampleTexture2D_a7ed63fef56043cbb0ef6d5e37f4a45b_Texture_1_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _AutomotiveRainGatedCustomFunction_c53087ed45864de2bfb6e53caabc2edd_StillDropsTex_12_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _AutomotiveRainGatedCustomFunction_c53087ed45864de2bfb6e53caabc2edd_RainDropsTex_13_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _AutomotiveRainGatedCustomFunction_c53087ed45864de2bfb6e53caabc2edd_RainStreaksTex_14_Texture2D ("Texture2D", 2D) = "white" {}
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