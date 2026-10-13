Shader "Hidden/Screen/FinishFlash"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		Pass
		{
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
			float4 _FinishFlash_Tint;
			float _FinishFlash_Gain;
			float _FinishFlash_Desat;
			float _FinishFlash_Intensity;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_LinearClamp : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(saturate(_FinishFlash_Desat * _FinishFlash_Intensity));
				r1 = asuint(_BlitTexture.SampleBias(sampler_LinearClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0.y = asuint(dot(asfloat(r1.xyz), float3(0.298999995, 0.587000012, 0.114)));
				r0.yzw = asuint(asfloat(r0.yyy) + -asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r0.xxx) * asfloat(r0.yzw) + asfloat(r1.xyz));
				o_SV_Target0.w = asfloat(r1.w);
				r1.xyz = asuint(asfloat(r0.xyz) * _FinishFlash_Tint.xyz + -asfloat(r0.xyz));
				r0.xyz = asuint(float3(_FinishFlash_Intensity, _FinishFlash_Intensity, _FinishFlash_Intensity) * asfloat(r1.xyz) + asfloat(r0.xyz));
				r0.w = asuint(-1.0 + _FinishFlash_Gain);
				r0.w = asuint(_FinishFlash_Intensity * asfloat(r0.w) + 1.0);
				o_SV_Target0.xyz = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif
			ENDHLSL
		}
	}
}
