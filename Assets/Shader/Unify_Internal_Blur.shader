Shader "Unify/Internal/Blur"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		Pass
		{
			Name "Kawase"
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
			float4 _GlobalsPad0[129];
			float _BlitMipLevel;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[2];
			int _Iteration;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			float4 _BlurParams;
			float2 _BlitTexture_TexelSize;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_LinearClamp : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				r0.x = asuint(float(asint(asfloat(_Iteration))));
				r0.y = asuint(1.0 / _BlurParams.z);
				r0.yz = asuint(_BlitTexture_TexelSize.yx * asfloat(r0.yy));
				r0.xyz = asuint(_BlurParams.wxx * asfloat(r0.xyz));
				r1.xy = asuint(float2(0.5, 0.5) * asfloat(r0.yz));
				r0.xy = asuint(asfloat(r0.yz) * asfloat(r0.xx) + asfloat(r1.xy));
				r1.zw = asuint(i_TEXCOORD0.yx + asfloat(r0.xy));
				r1.xy = asuint(i_TEXCOORD0.xy + -asfloat(r0.yx));
				r0 = asuint(_BlitTexture.SampleLevel(sampler_LinearClamp, asfloat(r1.wz), _BlitMipLevel));
				r2 = asuint(_BlitTexture.SampleLevel(sampler_LinearClamp, asfloat(r1.xz), _BlitMipLevel));
				r3 = asuint(_BlitTexture.SampleLevel(sampler_LinearClamp, asfloat(r1.wy), _BlitMipLevel));
				r1 = asuint(_BlitTexture.SampleLevel(sampler_LinearClamp, asfloat(r1.xy), _BlitMipLevel));
				r0 = asuint(asfloat(r2) + asfloat(r0));
				r0 = asuint(asfloat(r0) + asfloat(r3));
				r0 = asuint(asfloat(r0) + asfloat(r1));
				o_SV_Target0 = float4(0.25, 0.25, 0.25, 0.25) * asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
}
