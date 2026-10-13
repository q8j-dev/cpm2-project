Shader "CPM/URP/Vegetation/TreeLit" {
	Properties {
		[NoScaleOffset] _BaseMap ("BaseMap", 2D) = "white" {}
		[Toggle(_USEVERTEXCOLOR)] _USEVERTEXCOLOR ("UseVertexColor", Float) = 0
		_Color ("Color", Vector) = (0,0,0,0)
		[NoScaleOffset] [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
		_Clip ("Clip", Range(0, 1)) = 0.5
		_NormalSoften ("NormalSoften", Range(0, 1)) = 0
		_AO ("AO", Range(0, 1)) = 1
		_Smootness ("Smootness", Range(0, 1)) = 0
		[NoScaleOffset] _NoiseTex ("NoiseTex", 2D) = "white" {}
		[Toggle(_DOUBLE_SIDE_SHADING)] _DOUBLE_SIDE_SHADING ("Double Side Shading", Float) = 0
		_Float ("Float", Float) = 0
		_Axis ("Axis", Vector) = (0,1,0,0)
		_time ("time", Float) = 0.1
		_ScaleNoise ("ScaleNoise", Float) = 1
		_DisplScale ("DisplScale", Float) = 1
		_RootTime ("RootTime", Float) = 0
		_RootScaleNoise ("RootScaleNoise", Float) = 0
		_RootDispScale ("RootDispScale", Float) = 0
		[Toggle(_USE_COLOR_TINT)] _USE_COLOR_TINT ("Use Color Tint", Float) = 0
		_TintA ("TintA", Vector) = (0.5,0.5,0.5,0)
		_TintB ("TintB", Vector) = (0,0,0,0)
		_TintScale ("TintScale", Float) = 0
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