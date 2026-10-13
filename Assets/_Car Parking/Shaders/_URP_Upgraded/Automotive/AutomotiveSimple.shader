Shader "CPM/URP/Automotive/AutomotiveSimple" {
	Properties {
		[NoScaleOffset] _BaseMap ("Albedo", 2D) = "white" {}
		[Toggle(_ALPHATHICKNESS)] _ALPHATHICKNESS ("AlphaThickness", Float) = 0
		_Thickness ("Thickness", Range(0, 1)) = 0
		_Color ("Color", Vector) = (1,1,1,0)
		[Toggle(_NORMALMAP)] _NORMALMAP ("NormalMap", Float) = 0
		[NoScaleOffset] [Normal] _BumpMap ("Normal map", 2D) = "bump" {}
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		_Metalness ("Metalness", Range(0, 1)) = 0
		[HDR] _EmissionColor ("Emission Color", Vector) = (0,0,0,0)
		[NoScaleOffset] _EmissionMap ("Emission Map", 2D) = "white" {}
		_FakeBottomAO ("FakeBottomAO", Range(0, 1)) = 0
		_VertAO ("VertAO", Range(0, 1)) = 1
		[ToggleUI] _USEDIRT ("UseDirt", Float) = 0
		_MaskAddTex ("Dirt Albedo", 2D) = "white" {}
		_MaskColor ("Dirt color", Vector) = (0.9058824,0.7529412,0.5176471,1)
		_MaskValue ("Dirt Amount", Range(0, 1)) = 0
		_StencilRef ("_StencilRef", Float) = 1
		_RainStreaksPower ("RainStreaksPower", Range(0, 1)) = 1
		_RainPower ("RainPower", Range(0, 1)) = 1
		[NoScaleOffset] _AutomotiveRainGatedCustomFunction_c53087ed45864de2bfb6e53caabc2edd_StillDropsTex_12_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _AutomotiveRainGatedCustomFunction_c53087ed45864de2bfb6e53caabc2edd_RainDropsTex_13_Texture2D ("Texture2D", 2D) = "white" {}
		[NoScaleOffset] _AutomotiveRainGatedCustomFunction_c53087ed45864de2bfb6e53caabc2edd_RainStreaksTex_14_Texture2D ("Texture2D", 2D) = "white" {}
		[HideInInspector] _WorkflowMode ("_WorkflowMode", Float) = 1
		[HideInInspector] _CastShadows ("_CastShadows", Float) = 1
		[HideInInspector] _ReceiveShadows ("_ReceiveShadows", Float) = 1
		[HideInInspector] _Surface ("_Surface", Float) = 0
		[HideInInspector] _Blend ("_Blend", Float) = 0
		[HideInInspector] _AlphaClip ("_AlphaClip", Float) = 0
		[HideInInspector] _BlendModePreserveSpecular ("_BlendModePreserveSpecular", Float) = 1
		[HideInInspector] _SrcBlend ("_SrcBlend", Float) = 1
		[HideInInspector] _DstBlend ("_DstBlend", Float) = 0
		[ToggleUI] [HideInInspector] _ZWrite ("_ZWrite", Float) = 1
		[HideInInspector] _ZWriteControl ("_ZWriteControl", Float) = 0
		[HideInInspector] _ZTest ("_ZTest", Float) = 4
		[HideInInspector] _Cull ("_Cull", Float) = 2
		[HideInInspector] _AlphaToMask ("_AlphaToMask", Float) = 0
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