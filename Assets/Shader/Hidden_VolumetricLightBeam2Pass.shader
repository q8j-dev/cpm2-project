Shader "Hidden/VolumetricLightBeam2Pass" {
	Properties {
		_ConeSlopeCosSin ("Cone Slope Cos Sin", Vector) = (0,0,0,0)
		_ConeRadius ("Cone Radius", Vector) = (0,0,0,0)
		_ConeApexOffsetZ ("Cone Apex Offset Z", Float) = 0
		[HDR] _ColorFlat ("Color", Vector) = (1,1,1,1)
		_AlphaInside ("Alpha Inside", Range(0, 1)) = 1
		_AlphaOutside ("Alpha Outside", Range(0, 1)) = 1
		_DistanceFadeStart ("Distance Fade Start", Float) = 0
		_DistanceFadeEnd ("Distance Fade End", Float) = 1
		_DistanceCamClipping ("Camera Clipping Distance", Float) = 0.5
		_FadeOutFactor ("FadeOutFactor", Float) = 1
		_AttenuationLerpLinearQuad ("Lerp between attenuation linear and quad", Float) = 0.5
		_DepthBlendDistance ("Depth Blend Distance", Float) = 2
		_FresnelPow ("Fresnel Pow", Range(0, 15)) = 1
		_GlareFrontal ("Glare Frontal", Range(0, 1)) = 0.5
		_GlareBehind ("Glare from Behind", Range(0, 1)) = 0.5
		_DrawCap ("Draw Cap", Float) = 1
		_NoiseLocal ("Noise Local", Vector) = (0,0,0,0)
		_NoiseParam ("Noise Param", Vector) = (0,0,0,0)
		_CameraParams ("Camera Params", Vector) = (0,0,0,0)
		_CameraPosObjectSpace ("Camera Position Object Space", Vector) = (0,0,0,0)
		_ClippingPlaneWS ("Clipping Plane WS", Vector) = (0,0,0,0)
		_BlendSrcFactor ("BlendSrcFactor", Float) = 1
		_BlendDstFactor ("BlendDstFactor", Float) = 1
		_CameraBufferSizeSRP ("Camera Buffer Size SRP", Vector) = (1,1,0,0)
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