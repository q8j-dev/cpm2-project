Shader "Hidden/Universal/CoreBlitColorAndDepth"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" }
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DISABLE_TEXTURE2D_X_ARRAY

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
			float4 _GlobalsPad0[128];
			float _BlitMipLevel;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_LinearClamp : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				o_SV_Target0 = _BlitTexture.SampleLevel(sampler_LinearClamp, i_TEXCOORD0.xy, _BlitMipLevel);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DISABLE_TEXTURE2D_X_ARRAY

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
			float4 _GlobalsPad0[128];
			float _BlitMipLevel;
			Texture2D<float4> _BlitTexture : register(t0);
			Texture2D<float4> _InputDepthTexture : register(t1);
			SamplerState sampler_PointClamp : register(s0);
			SamplerState sampler_LinearClamp : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0, out float o_SV_Depth0 : SV_Depth)
			{
				uint4 r0;
				o_SV_Target0 = _BlitTexture.SampleLevel(sampler_LinearClamp, i_TEXCOORD0.xy, _BlitMipLevel);
				r0 = asuint(_InputDepthTexture.SampleLevel(sampler_PointClamp, i_TEXCOORD0.xy, _BlitMipLevel));
				o_SV_Depth0 = asfloat(r0.x);
			}
			#endif
			ENDHLSL
		}
	}
}
