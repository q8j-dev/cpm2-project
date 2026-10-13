Shader "Hidden/PostProcessing/DeferredFog"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ FOG_EXP

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[26];
			float _RenderViewportScaleFactor;

			void vert(float3 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				o_SV_POSITION0.xy = i_POSITION0.xy;
				o_SV_POSITION0.zw = float2(0.0, 1.0);
				r0.xy = asuint(float2(1.0, 1.0) + i_POSITION0.xy);
				r0.xy = asuint(asfloat(r0.xy) * float2(0.5, -0.5) + float2(0.0, 1.0));
				o_TEXCOORD1 = float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0.xy);
				o_TEXCOORD0 = i_POSITION0.xy * float2(0.5, -0.5) + float2(0.5, 0.5);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(FOG_EXP)
			float4 _GlobalsPad0[17];
			float4 _ProjectionParams;
			float4 _GlobalsPad1[2];
			float4 unity_OrthoParams;
			float4 _ZBufferParams;
			float4 _GlobalsPad2[6];
			float4 _FogColor;
			float3 _FogParams;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.x = asuint(1.0 + -unity_OrthoParams.w);
				r1 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, i_TEXCOORD1.xy));
				r0.y = asuint(_ZBufferParams.x * asfloat(r1.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y) + _ZBufferParams.y);
				r0.y = asuint(1.0 - unity_OrthoParams.w * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.y) / asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) * _ProjectionParams.z + -_ProjectionParams.y);
				r0.x = asuint(-_FogParams.x * asfloat(r0.x));
				r0.x = asuint(exp2(asfloat(r0.x)));
				r0.x = asuint(min(1.0, asfloat(r0.x)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r2 = asuint(_FogColor.xyzw + -asfloat(r1));
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#elif 1
			float4 _GlobalsPad0[17];
			float4 _ProjectionParams;
			float4 _GlobalsPad1[2];
			float4 unity_OrthoParams;
			float4 _ZBufferParams;
			float4 _GlobalsPad2[6];
			float4 _FogColor;
			float3 _FogParams;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.x = asuint(1.0 + -unity_OrthoParams.w);
				r1 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, i_TEXCOORD1.xy));
				r0.y = asuint(_ZBufferParams.x * asfloat(r1.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y) + _ZBufferParams.y);
				r0.y = asuint(1.0 - unity_OrthoParams.w * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.y) / asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) * _ProjectionParams.z + -_ProjectionParams.y);
				r0.x = asuint(_FogParams.x * asfloat(r0.x));
				r0.x = asuint(-asfloat(r0.x) * asfloat(r0.x));
				r0.x = asuint(exp2(asfloat(r0.x)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r2 = asuint(_FogColor.xyzw + -asfloat(r1));
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#endif
			#endif
			ENDHLSL
		}
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ FOG_EXP

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[26];
			float _RenderViewportScaleFactor;

			void vert(float3 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				o_SV_POSITION0.xy = i_POSITION0.xy;
				o_SV_POSITION0.zw = float2(0.0, 1.0);
				r0.xy = asuint(float2(1.0, 1.0) + i_POSITION0.xy);
				r0.xy = asuint(asfloat(r0.xy) * float2(0.5, -0.5) + float2(0.0, 1.0));
				o_TEXCOORD1 = float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0.xy);
				o_TEXCOORD0 = i_POSITION0.xy * float2(0.5, -0.5) + float2(0.5, 0.5);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(FOG_EXP)
			float4 _GlobalsPad0[17];
			float4 _ProjectionParams;
			float4 _GlobalsPad1[2];
			float4 unity_OrthoParams;
			float4 _ZBufferParams;
			float4 _GlobalsPad2[6];
			float4 _FogColor;
			float3 _FogParams;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0.x = asuint(1.0 + -unity_OrthoParams.w);
				r1 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, i_TEXCOORD1.xy));
				r0.y = asuint(_ZBufferParams.x * asfloat(r1.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y) + _ZBufferParams.y);
				r0.y = asuint(1.0 - unity_OrthoParams.w * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.y) / asfloat(r0.x));
				r0.y = asuint(asfloat(r0.x) * _ProjectionParams.z + -_ProjectionParams.y);
				b0.x = (asfloat(r0.x) < 0.999899983);
				r0.x = b0.x ? 0x3f800000u : 0u;
				r0.y = asuint(-_FogParams.x * asfloat(r0.y));
				r0.y = asuint(exp2(asfloat(r0.y)));
				r0.y = asuint(min(1.0, asfloat(r0.y)));
				r0.y = asuint(1.0 + -asfloat(r0.y));
				r0.x = asuint(asfloat(r0.y) * asfloat(r0.x));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r2 = asuint(_FogColor.xyzw + -asfloat(r1));
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#elif 1
			float4 _GlobalsPad0[17];
			float4 _ProjectionParams;
			float4 _GlobalsPad1[2];
			float4 unity_OrthoParams;
			float4 _ZBufferParams;
			float4 _GlobalsPad2[6];
			float4 _FogColor;
			float3 _FogParams;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0.x = asuint(1.0 + -unity_OrthoParams.w);
				r1 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, i_TEXCOORD1.xy));
				r0.y = asuint(_ZBufferParams.x * asfloat(r1.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y) + _ZBufferParams.y);
				r0.y = asuint(1.0 - unity_OrthoParams.w * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.y) / asfloat(r0.x));
				r0.y = asuint(asfloat(r0.x) * _ProjectionParams.z + -_ProjectionParams.y);
				b0.x = (asfloat(r0.x) < 0.999899983);
				r0.x = b0.x ? 0x3f800000u : 0u;
				r0.y = asuint(_FogParams.x * asfloat(r0.y));
				r0.y = asuint(-asfloat(r0.y) * asfloat(r0.y));
				r0.y = asuint(exp2(asfloat(r0.y)));
				r0.y = asuint(1.0 + -asfloat(r0.y));
				r0.x = asuint(asfloat(r0.y) * asfloat(r0.x));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r2 = asuint(_FogColor.xyzw + -asfloat(r1));
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
