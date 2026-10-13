Shader "Shader Graphs/RadialBlur" {
	Properties {
		_BlurAmount ("Blur Amount", Float) = 0.001
		_RotationAxis ("Rotation Axis", Vector) = (1,0,0,0)
		[NoScaleOffset] _BueNoise ("BueNoise", 2D) = "white" {}
		[Toggle(_FRONT_FILTER)] _FRONT_FILTER ("Front Filter", Float) = 0
		_Offset_Front_Filter ("Offset Front Filter", Float) = 0
		[Toggle(_BACK_FILTER)] _BACK_FILTER ("Back Filter", Float) = 0
		_Back_Filter_Offset ("Back Filter Offset", Float) = 1
		_Back_Filter_Falloff ("Back Filter Falloff", Float) = 0.001
		[Toggle(_MULTISAMPLEBLUR)] _MULTISAMPLEBLUR ("MultisampleBlur", Float) = 0
		_StencilRef ("_StencilRef", Float) = 1
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