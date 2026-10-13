Shader "CPM/URP/Misc/MeshTerrain6Textures" {
	Properties {
		_Color ("Color", Vector) = (0,0,0,0)
		[NoScaleOffset] _V_T2M_Control ("Control Map (RGBA)", 2D) = "black" {}
		[NoScaleOffset] _V_T2M_Splat1 ("Layer 1 (R)", 2D) = "white" {}
		_V_T2M_Splat1_uvScale ("UvScale", Float) = 1
		[NoScaleOffset] _V_T2M_Splat2 ("Layer 2 (G)", 2D) = "white" {}
		_V_T2M_Splat2_uvScale ("UvScale (1)", Float) = 1
		[NoScaleOffset] _V_T2M_Splat3 ("Layer 3 (B)", 2D) = "white" {}
		_V_T2M_Splat3_uvScale ("UvScale (2)", Float) = 0
		[NoScaleOffset] _V_T2M_Splat4 ("Layer 4 (A)", 2D) = "white" {}
		_V_T2M_Splat4_uvScale ("UvScale (3)", Float) = 0
		[NoScaleOffset] _V_T2M_Control2 ("Control Map 2 (RGBA)", 2D) = "black" {}
		[NoScaleOffset] _V_T2M_Splat5 ("Layer 1 (R) (1)", 2D) = "white" {}
		_V_T2M_Splat5_uvScale ("UvScale (4)", Float) = 0
		[NoScaleOffset] _V_T2M_Splat6 ("Layer 2 (G) (1)", 2D) = "white" {}
		_V_T2M_Splat6_uvScale ("UvScale (5)", Float) = 0
		[NoScaleOffset] _MainTex ("BaseMap (Fallback use only!)", 2D) = "white" {}
		_Smoothness ("Smoothness", Range(0, 1)) = 0
		[Toggle(_VERTEXAO)] _VERTEXAO ("VertexAO", Float) = 0
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