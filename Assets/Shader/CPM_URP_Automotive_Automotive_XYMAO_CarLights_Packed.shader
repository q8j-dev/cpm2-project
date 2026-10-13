Shader "CPM/URP/Automotive/Automotive_XYMAO_CarLights_Packed" {
	Properties {
		[NoScaleOffset] _BaseMap ("Albedo", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,0)
		[Toggle(_NORMALMAP1)] _NORMALMAP1 ("HasNormal", Float) = 0
		[NoScaleOffset] _NormalMask ("NormalMask", 2D) = "grey" {}
		_Metallic ("Metalness", Range(0, 1)) = 0
		[Toggle(_METALNESS)] _METALNESS ("HasMetalness", Float) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		[Toggle(_SMOOTHNESS)] _SMOOTHNESS ("HasSmoothness", Float) = 0
		_OcclusionStrength ("Occlusion", Range(0, 1)) = 1
		[Toggle(_OCCLUSIONMAP)] _OCCLUSIONMAP ("HasAO", Float) = 0
		_Tiling_And_Offset ("Tiling And Offset", Vector) = (1,1,0,0)
		[NoScaleOffset] _EmissionArrayMap ("Emission Array Map", 2DArray) = "" {}
		_FakeBottomAO ("FakeBottomAO", Range(0, 1)) = 0
		_VertAO ("VertAO", Range(0, 1)) = 1
		_StencilRef ("_StencilRef", Float) = 1
		_XenonDir ("XenonDir", Vector) = (0,0,1,0)
		[Toggle(_SEQUENTIAL_TURN_SIGNALS)] _SEQUENTIAL_TURN_SIGNALS ("Sequential Turn Signals", Float) = 0
		_DRL_Power ("DRL Power", Range(0, 1)) = 1
		[HDR] _DRL_Color_0 ("DRL Color (0)", Vector) = (1,1,1,0)
		_LBL_Power ("LBL Power", Range(0, 1)) = 1
		[HDR] _LBL_Color_0 ("LBL Color (0)", Vector) = (1,1,1,0)
		_HBL_Power ("HBL Power", Range(0, 1)) = 1
		[HDR] _HBL_Color_0 ("HBL Color (0)", Vector) = (1,1,1,0)
		_LTL_Power_0 ("LTL Power (0)", Range(0, 1)) = 1
		[HDR] _LTL_Color_0 ("LTL Color (0)", Vector) = (1,1,1,0)
		_RTL_Power_0 ("RTL Power (0)", Range(0, 1)) = 1
		[HDR] _RTL_Color_0 ("RTL Color (0)", Vector) = (1,1,1,0)
		_PTL_Power_0 ("PTL Power (0)", Range(0, 1)) = 1
		[HDR] _PTL_Color_0 ("PTL Color (0)", Vector) = (1,1,1,0)
		_BL_Power_0 ("BL Power (0)", Range(0, 1)) = 1
		[HDR] _BL_Color_0 ("BL Color (0)", Vector) = (1,1,1,0)
		_RL_Power ("RL Power", Range(0, 1)) = 1
		[HDR] _RL_Color_0 ("RL Color (0)", Vector) = (1,1,1,0)
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