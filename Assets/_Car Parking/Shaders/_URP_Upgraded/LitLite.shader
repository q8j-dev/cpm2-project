Shader "CPM/URP/LitLite" {
	Properties {
		_BaseColor ("Color", Vector) = (1,1,1,0)
		[KeywordEnum(Off, RGB, RGBA)] _USEVERTEXCOLOR ("UseVertexColor", Float) = 0
		_Cutoff ("Alpha Cutoff", Float) = 0.5
		[NoScaleOffset] _BaseMap ("Albedo", 2D) = "white" {}
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic", Range(0, 1)) = 0
		[NoScaleOffset] _MetallicGlossMap ("MaskMap", 2D) = "white" {}
		[ToggleUI] _VERTEXAO ("VertexAO", Float) = 0
		_OcclusionStrength ("OcclusionStrength", Range(0, 1)) = 0
		_FakeOcclusion ("FakeOcclusion", Range(0, 1)) = 0
		[NoScaleOffset] [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
		[Toggle(_EMISSION)] _EMISSION ("Emission", Float) = 0
		[HDR] _EmissionColor ("Emission Color", Vector) = (0,0,0,0)
		[NoScaleOffset] _EmissionMap ("Emission", 2D) = "white" {}
		_BaseMap_ST ("TilingAndOffset", Vector) = (1,1,0,0)
		_StencilRef ("_StencilRef", Float) = 0
		_RainVisibility ("RainVisibility", Range(0, 1)) = 1
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
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}