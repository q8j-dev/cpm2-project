Shader "Hidden/Universal Render Pipeline/Sampling"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			Name "BoxDownsample"
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[127];
			float4 _BlitScaleBias;

			void vert(uint i_SV_VertexID0 : SV_VertexID, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				r0.x = asuint(asint(asfloat(i_SV_VertexID0)) << 1);
				r0.x = 2u & r0.x;
				r0.w = 2u & asuint(asfloat(i_SV_VertexID0));
				r0.xy = asuint(float2(r0.xw));
				o_SV_POSITION0.xy = asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0);
				r0.z = asuint(1.0 + -asfloat(r0.y));
				o_TEXCOORD0 = asfloat(r0.xz) * _BlitScaleBias.xy + _BlitScaleBias.zw;
				o_SV_POSITION0.zw = float2(1.0, 1.0);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[126];
			float4 _BlitTexture_TexelSize;
			float _SampleOffset;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_BlitTexture : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(float4(-1.0, -1.0, 1.0, 1.0) * float4(_SampleOffset, _SampleOffset, _SampleOffset, _SampleOffset));
				r1 = asuint(_BlitTexture_TexelSize.xyxy * asfloat(r0.xyzy) + i_TEXCOORD0.xyxy);
				r0 = asuint(_BlitTexture_TexelSize.xyxy * asfloat(r0.xwzw) + i_TEXCOORD0.xyxy);
				r2 = asuint(_BlitTexture.SampleBias(sampler_BlitTexture, asfloat(r1.xy), _GlobalMipBias.x));
				r1 = asuint(_BlitTexture.SampleBias(sampler_BlitTexture, asfloat(r1.zw), _GlobalMipBias.x));
				r1 = asuint(asfloat(r2) + asfloat(r1));
				r2 = asuint(_BlitTexture.SampleBias(sampler_BlitTexture, asfloat(r0.xy), _GlobalMipBias.x));
				r0 = asuint(_BlitTexture.SampleBias(sampler_BlitTexture, asfloat(r0.zw), _GlobalMipBias.x));
				r1 = asuint(asfloat(r2) + asfloat(r1));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				o_SV_Target0 = float4(0.25, 0.25, 0.25, 0.25) * asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
}
