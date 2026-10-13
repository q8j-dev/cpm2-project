Shader "CPM/URP/Misc/HeightVertexBlendXYMAO" {
	Properties {
		_DepthBlend ("DepthBlend", Range(0.002, 0.5)) = 0.13
		[Toggle(_VERTEXAO)] _VERTEXAO ("VertexAO", Float) = 0
		[NoScaleOffset] _BaseMap ("Base Albedo", 2D) = "white" {}
		[NoScaleOffset] _BaseNormalMask ("Base NormalMask", 2D) = "grey" {}
		_Base_Tiling_And_Offset ("Base Tiling And Offset", Vector) = (1,1,0,0)
		_Base_Layer_Color ("Base Layer Color", Vector) = (1,1,1,0)
		_Base_Metalness ("Base Metalness", Range(0, 1)) = 0
		[KeywordEnum(Metalness, Height)] _BASE_MASKBLUECHANNEL ("Base MaskBlueChannel", Float) = 1
		_Base_Smoothness ("Base Smoothness", Range(0, 1)) = 1
		_Base_Occlusion ("Base Occlusion", Range(0, 1)) = 1
		[NoScaleOffset] _RedMap ("Red Albedo", 2D) = "white" {}
		[NoScaleOffset] _RedNormalMask ("Red NormalMask", 2D) = "grey" {}
		_Red_Tiling_And_Offset ("Red Tiling And Offset", Vector) = (1,1,0,0)
		_Red_Layer_Color ("Red Layer Color", Vector) = (1,1,1,0)
		_Red_Metalness ("Red Metalness", Range(0, 1)) = 0
		[KeywordEnum(Metalness, Height)] _RED_MASKBLUECHANNEL ("Red MaskBlueChannel", Float) = 1
		_Red_Smoothness ("Red Smoothness", Range(0, 1)) = 1
		_Red_Occlusion ("Red Occlusion", Range(0, 1)) = 1
		[NoScaleOffset] _GreenMap ("Green Albedo", 2D) = "white" {}
		[NoScaleOffset] _GreenNormalMask ("Green NormalMask", 2D) = "grey" {}
		_Green_Tiling_And_Offset ("Green Tiling And Offset", Vector) = (1,1,0,0)
		_Green_Layer_Color ("Green Layer Color", Vector) = (1,1,1,0)
		_Green_Metalness ("Green Metalness", Range(0, 1)) = 0
		[KeywordEnum(Metalness, Height)] _GREEN_MASKBLUECHANNEL ("Green MaskBlueChannel", Float) = 1
		_Green_Smoothness ("Green Smoothness", Range(0, 1)) = 1
		_Green_Occlusion ("Green Occlusion", Range(0, 1)) = 1
		[NoScaleOffset] _BlueMap ("Blue Albedo", 2D) = "white" {}
		[NoScaleOffset] _BlueNormalMask ("Blue NormalMask", 2D) = "grey" {}
		_Blue_Tiling_And_Offset ("Blue Tiling And Offset", Vector) = (1,1,0,0)
		_Blue_Layer_Color ("Blue Layer Color", Vector) = (1,1,1,0)
		_Blue_Metalness ("Blue Metalness", Range(0, 1)) = 0
		[KeywordEnum(Metalness, Height)] _BLUE_MASKBLUECHANNEL ("Blue MaskBlueChannel", Float) = 1
		_Blue_Smoothness ("Blue Smoothness", Range(0, 1)) = 1
		_Blue_Occlusion ("Blue Occlusion", Range(0, 1)) = 1
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