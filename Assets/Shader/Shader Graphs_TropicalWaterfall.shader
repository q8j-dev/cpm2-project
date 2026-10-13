Shader "Shader Graphs/TropicalWaterfall" {
	Properties {
		[NoScaleOffset] _FoamTexture ("FoamTexture", 2D) = "white" {}
		_WaterColor ("WaterColor", Vector) = (0.2156863,0.6055423,0.7254902,0.5019608)
		[NoScaleOffset] _MainNormal ("MainNormal", 2D) = "white" {}
		_MainNormalTiling ("MainNormalTiling", Vector) = (1,1,0,0)
		[NoScaleOffset] _SecondNormal ("SecondNormal", 2D) = "white" {}
		_SecondNormalTiling ("SecondNormalTiling", Vector) = (1,1,0,0)
		_Smoothness ("Smoothness", Range(0, 1)) = 1
		_NormalStrength ("NormalStrength", Float) = 1
		_WaterFlowDir ("WaterFlowDir", Vector) = (1,1,0,0)
		_WaterFlowSpeed ("WaterFlowSpeed", Float) = 0
		_refraction ("refraction", Float) = 0.01
		_FoamTop ("FoamTop", Float) = 0.28
		_FoamBottom ("FoamBottom", Float) = 1.22
		_FoamSpeed ("FoamSpeed", Float) = -0.4
		_FoamColor ("FoamColor", Vector) = (2,2,2,0.003921569)
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