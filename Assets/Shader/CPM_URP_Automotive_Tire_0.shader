Shader "CPM/URP/Automotive/Tire" {
	Properties {
		[NoScaleOffset] _NewAlbedoSmoothness ("NewAlbedoSmoothness", 2D) = "white" {}
		[NoScaleOffset] _NewNormalCavAo ("NewNormalCavAo", 2D) = "white" {}
		[NoScaleOffset] _OldAlbedoSmoothness ("OldAlbedoSmoothness", 2D) = "white" {}
		[NoScaleOffset] _OldNormalCavAO ("OldNormalCavAO", 2D) = "white" {}
		_InnerD ("InnerD", Float) = 0
		_OuterD ("OuterD", Float) = 1
		_Width ("Width", Float) = 1
		_Wear ("Wear", Range(0, 1)) = 0
		[NoScaleOffset] _DirtAlbSmoothness ("DirtAlbSmoothness", 2D) = "white" {}
		[NoScaleOffset] _DirtNormalHeighAo ("DirtNormalHeighAo", 2D) = "white" {}
		_DirtAmount ("DirtAmount", Range(0, 1)) = 0
		_SandTiling ("SandTiling", Vector) = (1,1,0,0)
		_TopAO ("TopAO", Range(0, 1)) = 1
		_BottomAo ("BottomAo", Range(0, 1)) = 1
		_PlanePostition ("PlanePostition", Vector) = (0,-500,0,0)
		_PlaneNormal ("PlaneNormal", Vector) = (0,1,0,0)
		_StencilRef ("_StencilRef", Float) = 1
		_MaxDeflect ("MaxDeflect", Float) = 0.04
		_Bulge ("Bulge", Float) = 0.3
		_LatDeform ("LatDeform", Vector) = (0,0,0,0)
		_LongDeform ("LongDeform", Vector) = (0,0,0,0)
		_CarcassR ("CarcassR", Float) = 0.31
		_LugStiffness ("LugStiffness", Float) = 0.2
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