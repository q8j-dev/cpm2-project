Shader "Hidden/Rain/DropsCRT" {
	Properties {
		_RippleTexture ("Ripple Texture (RGBA as in Seb)", 2D) = "black" {}
		_RippleTiling ("Ripple Tiling", Float) = 5
		_PhaseMul ("Phase Mult (default 9)", Float) = 9
		_ClampMax ("Clamp Max (default 3)", Float) = 3
		_Amplitude ("XY Amplitude", Float) = 0.35
		_Fade ("Global Fade by _FilmWetness", Float) = 1
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
}