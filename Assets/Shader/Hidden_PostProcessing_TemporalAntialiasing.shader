Shader "Hidden/PostProcessing/TemporalAntialiasing"
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
			float4 _GlobalsPad0[26];
			float _RenderViewportScaleFactor;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[1];
			float4 _MainTex_TexelSize;
			float4 _CameraDepthTexture_TexelSize;
			float2 _Jitter;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _FinalBlendParameters;
			float _Sharpness;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _HistoryTex : register(t1);
			Texture2D<float4> _CameraDepthTexture : register(t2);
			Texture2D<float4> _CameraMotionVectorsTexture : register(t3);
			SamplerState sampler_LinearClamp : register(s0);
			SamplerState sampler_HistoryTex : register(s1);
			SamplerState sampler_CameraDepthTexture : register(s2);
			SamplerState sampler_CameraMotionVectorsTexture : register(s3);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0, out float4 o_SV_Target1 : SV_Target1)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				bool4 b0;
				r0.xy = asuint(-_CameraDepthTexture_TexelSize.xy + i_TEXCOORD1.xy);
				r0.xy = asuint(max(float2(0.0, 0.0), asfloat(r0.xy)));
				r0.xy = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r0.xy)));
				r0 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, asfloat(r0.xy)).yzxw);
				r1 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, i_TEXCOORD1.xy).yzxw);
				b0.w = (asfloat(r0.z) >= asfloat(r1.z));
				r0.w = b0.w ? 0x3f800000u : 0u;
				r0.xy = uint2(0xbf800000u, 0xbf800000u);
				r1.xy = uint2(0u, 0u);
				r0.xyz = asuint(-asfloat(r1.yyz) + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r0.www) * asfloat(r0.xyz) + asfloat(r1.xyz));
				r1.xy = uint2(0x3f800000u, 0xbf800000u);
				r2 = asuint(_CameraDepthTexture_TexelSize.xyxy * float4(1.0, -1.0, -1.0, 1.0) + i_TEXCOORD1.xyxy);
				r2 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r2)));
				r2 = asuint(min(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r2)));
				r3 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, asfloat(r2.xy)));
				r2 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, asfloat(r2.zw)).yzxw);
				r1.z = r3.x;
				b0.w = (asfloat(r3.x) >= asfloat(r0.z));
				r0.w = b0.w ? 0x3f800000u : 0u;
				r1.xyz = asuint(asfloat(r1.xyz) + -asfloat(r0.yyz));
				r0.xyz = asuint(asfloat(r0.www) * asfloat(r1.xyz) + asfloat(r0.xyz));
				r2.xy = uint2(0xbf800000u, 0x3f800000u);
				b0.w = (asfloat(r2.z) >= asfloat(r0.z));
				r0.w = b0.w ? 0x3f800000u : 0u;
				r1.xyz = asuint(asfloat(r2.xyz) + -asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r0.www) * asfloat(r1.xyz) + asfloat(r0.xyz));
				r1.xy = asuint(_CameraDepthTexture_TexelSize.xy + i_TEXCOORD1.xy);
				r1.xy = asuint(max(float2(0.0, 0.0), asfloat(r1.xy)));
				r1.xy = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r1.xy)));
				r1 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, asfloat(r1.xy)));
				b0.z = (asfloat(r1.x) >= asfloat(r0.z));
				r0.z = b0.z ? 0x3f800000u : 0u;
				r1.xy = asuint(float2(1.0, 1.0) + -asfloat(r0.xy));
				r0.xy = asuint(asfloat(r0.zz) * asfloat(r1.xy) + asfloat(r0.xy));
				r0.xy = asuint(asfloat(r0.xy) * _CameraDepthTexture_TexelSize.xy + i_TEXCOORD1.xy);
				r0 = asuint(_CameraMotionVectorsTexture.Sample(sampler_CameraMotionVectorsTexture, asfloat(r0.xy)));
				r0.z = asuint(dot(asfloat(r0.xy), asfloat(r0.xy)));
				r0.xy = asuint(i_TEXCOORD1.xy + -asfloat(r0.xy));
				r0.xy = asuint(max(float2(0.0, 0.0), asfloat(r0.xy)));
				r0.xy = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r0.xy)));
				r1 = asuint(_HistoryTex.Sample(sampler_HistoryTex, asfloat(r0.xy)));
				r0.x = asuint(sqrt(asfloat(r0.z)));
				r0.y = asuint(100.0 * asfloat(r0.x));
				r0.x = asuint(_FinalBlendParameters.z * asfloat(r0.x));
				r0.y = asuint(min(1.0, asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.y) * -3.75 + 4.0);
				r0.zw = asuint(-_Jitter.xy + i_TEXCOORD1.xy);
				r0.zw = asuint(max(float2(0.0, 0.0), asfloat(r0.zw)));
				r0.zw = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r0.zw)));
				r2.xy = asuint(asfloat(r0.zw) - _MainTex_TexelSize.xy * float2(0.5, 0.5));
				r2.xy = asuint(max(float2(0.0, 0.0), asfloat(r2.xy)));
				r2.xy = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r2.xy)));
				r2 = asuint(_MainTex.Sample(sampler_LinearClamp, asfloat(r2.xy)));
				r3.xy = asuint(_MainTex_TexelSize.xy * float2(0.5, 0.5) + asfloat(r0.zw));
				r4 = asuint(_MainTex.Sample(sampler_LinearClamp, asfloat(r0.zw)));
				r0.zw = asuint(max(float2(0.0, 0.0), asfloat(r3.xy)));
				r0.zw = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r0.zw)));
				r3 = asuint(_MainTex.Sample(sampler_LinearClamp, asfloat(r0.zw)));
				r5 = asuint(asfloat(r3) + asfloat(r2));
				r6 = asuint(asfloat(r4) + asfloat(r4));
				r5 = asuint(asfloat(r5) * float4(4.0, 4.0, 4.0, 4.0) + -asfloat(r6));
				r6 = asuint(asfloat(r4) - asfloat(r5) * float4(0.166666999, 0.166666999, 0.166666999, 0.166666999));
				r6 = asuint(float4(_Sharpness, _Sharpness, _Sharpness, _Sharpness) * asfloat(r6));
				r4 = asuint(asfloat(r6) * float4(2.71828198, 2.71828198, 2.71828198, 2.71828198) + asfloat(r4));
				r4 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r4)));
				r4 = asuint(min(float4(65472.0, 65472.0, 65472.0, 65472.0), asfloat(r4)));
				r5.xyz = asuint(asfloat(r5.xyz) + asfloat(r4.xyz));
				r5.xyz = asuint(float3(0.142857, 0.142857, 0.142857) * asfloat(r5.xyz));
				r0.z = asuint(dot(asfloat(r5.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r0.w = asuint(dot(asfloat(r4.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r0.z = asuint(asfloat(r0.z) + -asfloat(r0.w));
				r5.xyz = asuint(min(asfloat(r3.xyz), asfloat(r2.xyz)));
				r2.xyz = asuint(max(asfloat(r3.xyz), asfloat(r2.xyz)));
				r2.xyz = asuint(asfloat(r0.yyy) * abs(asfloat(r0.zzz)) + asfloat(r2.xyz));
				r0.yzw = asuint(asfloat(r5.xyz) - asfloat(r0.yyy) * abs(asfloat(r0.zzz)));
				r3.xyz = asuint(asfloat(r2.xyz) + -asfloat(r0.yzw));
				r0.yzw = asuint(asfloat(r2.xyz) + asfloat(r0.yzw));
				r2.xyz = asuint(float3(0.5, 0.5, 0.5) * asfloat(r3.xyz));
				r3.xyz = asuint(asfloat(r1.xyz) - asfloat(r0.yzw) * float3(0.5, 0.5, 0.5));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.yzw));
				r5.xyz = asuint(float3(9.99999975e-05, 9.99999975e-05, 9.99999975e-05) + asfloat(r3.xyz));
				r2.xyz = asuint(asfloat(r2.xyz) / asfloat(r5.xyz));
				r2.x = asuint(min(abs(asfloat(r2.x)), abs(asfloat(r2.y))));
				r2.x = asuint(min(asfloat(r2.x), abs(asfloat(r2.z))));
				r2.x = asuint(min(1.0, asfloat(r2.x)));
				r1.xyz = asuint(asfloat(r3.xyz) * asfloat(r2.xxx) + asfloat(r0.yzw));
				r1 = asuint(asfloat(r1) + -asfloat(r4));
				r0.y = asuint(_FinalBlendParameters.y + -_FinalBlendParameters.x);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y) + _FinalBlendParameters.x);
				r0.x = asuint(max(_FinalBlendParameters.y, asfloat(r0.x)));
				r0.x = asuint(min(_FinalBlendParameters.x, asfloat(r0.x)));
				r0 = asuint(asfloat(r0.xxxx) * asfloat(r1) + asfloat(r4));
				r0 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r0)));
				r0 = asuint(min(float4(65472.0, 65472.0, 65472.0, 65472.0), asfloat(r0)));
				o_SV_Target0 = asfloat(r0);
				o_SV_Target1 = asfloat(r0);
			}
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
			float4 _GlobalsPad0[26];
			float _RenderViewportScaleFactor;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[1];
			float4 _MainTex_TexelSize;
			float4 _GlobalsPad5[1];
			float2 _Jitter;
			float _GlobalsPad6;
			float _GlobalsPad7;
			float4 _FinalBlendParameters;
			float _Sharpness;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _HistoryTex : register(t1);
			Texture2D<float4> _CameraMotionVectorsTexture : register(t2);
			SamplerState sampler_LinearClamp : register(s0);
			SamplerState sampler_HistoryTex : register(s1);
			SamplerState sampler_CameraMotionVectorsTexture : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0, out float4 o_SV_Target1 : SV_Target1)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				r0.xy = asuint(-_Jitter.xy + i_TEXCOORD1.xy);
				r0.xy = asuint(max(float2(0.0, 0.0), asfloat(r0.xy)));
				r0.xy = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r0.xy)));
				r0.zw = asuint(asfloat(r0.xy) - _MainTex_TexelSize.xy * float2(0.5, 0.5));
				r0.zw = asuint(max(float2(0.0, 0.0), asfloat(r0.zw)));
				r0.zw = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r0.zw)));
				r1 = asuint(_MainTex.Sample(sampler_LinearClamp, asfloat(r0.zw)));
				r0.zw = asuint(_MainTex_TexelSize.xy * float2(0.5, 0.5) + asfloat(r0.xy));
				r2 = asuint(_MainTex.Sample(sampler_LinearClamp, asfloat(r0.xy)));
				r0.xy = asuint(max(float2(0.0, 0.0), asfloat(r0.zw)));
				r0.xy = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r0.xy)));
				r0 = asuint(_MainTex.Sample(sampler_LinearClamp, asfloat(r0.xy)));
				r3 = asuint(asfloat(r1) + asfloat(r0));
				r4 = asuint(asfloat(r2) + asfloat(r2));
				r3 = asuint(asfloat(r3) * float4(4.0, 4.0, 4.0, 4.0) + -asfloat(r4));
				r4 = asuint(asfloat(r2) - asfloat(r3) * float4(0.166666999, 0.166666999, 0.166666999, 0.166666999));
				r4 = asuint(float4(_Sharpness, _Sharpness, _Sharpness, _Sharpness) * asfloat(r4));
				r2 = asuint(asfloat(r4) * float4(2.71828198, 2.71828198, 2.71828198, 2.71828198) + asfloat(r2));
				r2 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r2)));
				r2 = asuint(min(float4(65472.0, 65472.0, 65472.0, 65472.0), asfloat(r2)));
				r3.xyz = asuint(asfloat(r3.xyz) + asfloat(r2.xyz));
				r3.xyz = asuint(float3(0.142857, 0.142857, 0.142857) * asfloat(r3.xyz));
				r0.w = asuint(dot(asfloat(r3.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r1.w = asuint(dot(asfloat(r2.xyz), float3(0.212672904, 0.715152204, 0.0721750036)));
				r0.w = asuint(-asfloat(r1.w) + asfloat(r0.w));
				r3.xyz = asuint(min(asfloat(r0.xyz), asfloat(r1.xyz)));
				r0.xyz = asuint(max(asfloat(r1.xyz), asfloat(r0.xyz)));
				r1 = asuint(_CameraMotionVectorsTexture.Sample(sampler_CameraMotionVectorsTexture, i_TEXCOORD1.xy));
				r1.z = asuint(dot(asfloat(r1.xy), asfloat(r1.xy)));
				r1.xy = asuint(i_TEXCOORD1.xy + -asfloat(r1.xy));
				r1.xy = asuint(max(float2(0.0, 0.0), asfloat(r1.xy)));
				r1.xy = asuint(min(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor), asfloat(r1.xy)));
				r4 = asuint(_HistoryTex.Sample(sampler_HistoryTex, asfloat(r1.xy)));
				r1.x = asuint(sqrt(asfloat(r1.z)));
				r1.y = asuint(100.0 * asfloat(r1.x));
				r1.x = asuint(_FinalBlendParameters.z * asfloat(r1.x));
				r1.y = asuint(min(1.0, asfloat(r1.y)));
				r1.y = asuint(asfloat(r1.y) * -3.75 + 4.0);
				r3.xyz = asuint(asfloat(r3.xyz) - asfloat(r1.yyy) * abs(asfloat(r0.www)));
				r0.xyz = asuint(asfloat(r1.yyy) * abs(asfloat(r0.www)) + asfloat(r0.xyz));
				r1.yzw = asuint(asfloat(r0.xyz) + -asfloat(r3.xyz));
				r0.xyz = asuint(asfloat(r0.xyz) + asfloat(r3.xyz));
				r1.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r1.yzw));
				r3.xyz = asuint(asfloat(r4.xyz) - asfloat(r0.xyz) * float3(0.5, 0.5, 0.5));
				r0.xyz = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.xyz));
				r5.xyz = asuint(float3(9.99999975e-05, 9.99999975e-05, 9.99999975e-05) + asfloat(r3.xyz));
				r1.yzw = asuint(asfloat(r1.yzw) / asfloat(r5.xyz));
				r0.w = asuint(min(abs(asfloat(r1.y)), abs(asfloat(r1.z))));
				r0.w = asuint(min(asfloat(r0.w), abs(asfloat(r1.w))));
				r0.w = asuint(min(1.0, asfloat(r0.w)));
				r4.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.www) + asfloat(r0.xyz));
				r0 = asuint(asfloat(r4) + -asfloat(r2));
				r1.y = asuint(_FinalBlendParameters.y + -_FinalBlendParameters.x);
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y) + _FinalBlendParameters.x);
				r1.x = asuint(max(_FinalBlendParameters.y, asfloat(r1.x)));
				r1.x = asuint(min(_FinalBlendParameters.x, asfloat(r1.x)));
				r0 = asuint(asfloat(r1.xxxx) * asfloat(r0) + asfloat(r2));
				r0 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r0)));
				r0 = asuint(min(float4(65472.0, 65472.0, 65472.0, 65472.0), asfloat(r0)));
				o_SV_Target0 = asfloat(r0);
				o_SV_Target1 = asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
}
