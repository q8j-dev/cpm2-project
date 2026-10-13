Shader "Hidden/Universal Render Pipeline/Blit"
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
			Name "Blit"
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ _LINEAR_TO_SRGB_CONVERSION

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
			#if defined(_LINEAR_TO_SRGB_CONVERSION)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_BlitTexture : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b2;
				r0 = asuint(_BlitTexture.SampleBias(sampler_BlitTexture, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r1.xyz = asuint(log2(abs(asfloat(r0.xyz))));
				r1.xyz = asuint(float3(0.416666657, 0.416666657, 0.416666657) * asfloat(r1.xyz));
				r1.xyz = asuint(exp2(asfloat(r1.xyz)));
				r1.xyz = asuint(asfloat(r1.xyz) * float3(1.05499995, 1.05499995, 1.05499995) + float3(-0.0549999997, -0.0549999997, -0.0549999997));
				b2.xyz = (float3(0.00313080009, 0.00313080009, 0.00313080009) >= asfloat(r0.xyz));
				r0.xyz = asuint(float3(12.9232101, 12.9232101, 12.9232101) * asfloat(r0.xyz));
				o_SV_Target0.w = asfloat(r0.w);
				o_SV_Target0.xyz = b2.xyz ? asfloat(r0.xyz) : asfloat(r1.xyz);
			}
			#elif 1
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_BlitTexture : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				o_SV_Target0 = _BlitTexture.SampleBias(sampler_BlitTexture, i_TEXCOORD0.xy, _GlobalMipBias.x);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
