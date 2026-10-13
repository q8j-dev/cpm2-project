Shader "KEI RainFX/Backend/RainDrop CRT" {
	Properties {
		_TorusTex ("Torus Normals Texture", 2D) = "grey" {}
		_DistortonPower ("Distoprtion Power", Range(-1, 1)) = 0
		_DistortonScale ("Distoprtion Scaler", Range(0, 10)) = 1
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		_RainDropTex ("RainDrops", 2D) = "white" {}
		[Toggle] _JustDistort ("JustDistort", Float) = 1
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
}