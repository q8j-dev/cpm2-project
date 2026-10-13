Shader "Hidden/Universal Render Pipeline/PaniniProjection"
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
			Name "Panini Projection"
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ _GENERIC
			#pragma multi_compile __ _UNIT_DISTANCE

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
			#if defined(_GENERIC)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[126];
			float4 _Params;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_LinearClamp : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.xy = asuint(_Params.xy * asfloat(r0.xy));
				r0.xy = asuint(_Params.ww * asfloat(r0.xy));
				r0.z = asuint(1.0 + _Params.z);
				r0.w = asuint(asfloat(r0.z) * asfloat(r0.z));
				r0.w = asuint(asfloat(r0.x) * asfloat(r0.x) + asfloat(r0.w));
				r1.x = asuint(_Params.z * asfloat(r0.x));
				r1.y = asuint(asfloat(r0.w) - asfloat(r1.x) * asfloat(r1.x));
				r1.y = asuint(sqrt(asfloat(r1.y)));
				r1.y = asuint(asfloat(r1.y) * asfloat(r0.z));
				r1.x = asuint(asfloat(r1.y) - asfloat(r1.x) * asfloat(r0.x));
				r0.w = asuint(asfloat(r1.x) / asfloat(r0.w));
				r1.x = asuint(_Params.z + asfloat(r0.w));
				r0.z = asuint(asfloat(r1.x) / asfloat(r0.z));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.ww));
				r0.xy = asuint(asfloat(r0.xy) / _Params.xy);
				r0.xy = asuint(asfloat(r0.xy) * float2(0.5, 0.5) + float2(0.5, 0.5));
				o_SV_Target0 = _BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r0.xy), _GlobalMipBias.x);
			}
			#elif defined(_UNIT_DISTANCE)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[126];
			float4 _Params;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_LinearClamp : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.xy = asuint(i_TEXCOORD0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0));
				r0.xy = asuint(_Params.xy * asfloat(r0.xy));
				r0.xy = asuint(_Params.ww * asfloat(r0.xy));
				r0.z = asuint(asfloat(r0.x) * asfloat(r0.x));
				r0.w = asuint(asfloat(r0.x) * asfloat(r0.x) + 4.0);
				r0.w = asuint(sqrt(asfloat(r0.w)));
				r0.z = asuint(asfloat(r0.z) / asfloat(r0.w));
				r0.z = asuint(asfloat(r0.w) + -asfloat(r0.z));
				r0.z = asuint(asfloat(r0.z) / asfloat(r0.w));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.z = asuint(asfloat(r0.z) * 2.0 + -1.0);
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				r0.xy = asuint(asfloat(r0.xy) / _Params.xy);
				r0.xy = asuint(asfloat(r0.xy) * float2(0.5, 0.5) + float2(0.5, 0.5));
				o_SV_Target0 = _BlitTexture.SampleBias(sampler_LinearClamp, asfloat(r0.xy), _GlobalMipBias.x);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
