Shader "Hidden/PostProcessing/Bloom"
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
			float4 _GlobalsPad5[2];
			float4 _Threshold;
			float4 _Params;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _AutoExposureTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_AutoExposureTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				r0 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-0.5, -0.5, 0.5, -0.5) + i_TEXCOORD0.xyxy));
				r0 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.xy)));
				r0 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.zw)));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				r1 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-0.5, 0.5, 0.5, 0.5) + i_TEXCOORD0.xyxy));
				r1 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.zw)));
				r0 = asuint(asfloat(r2) + asfloat(r0));
				r0 = asuint(asfloat(r0) + asfloat(r1));
				r1.xy = asuint(saturate(-_MainTex_TexelSize.xy + i_TEXCOORD0.xy));
				r1.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r2 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(0.0, -1.0, 1.0, -1.0) + i_TEXCOORD0.xyxy));
				r2 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r2));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r2.xy)));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r2.zw)));
				r2 = asuint(asfloat(r3) + asfloat(r2));
				r1 = asuint(asfloat(r3) + asfloat(r1));
				r3.xy = asuint(saturate(i_TEXCOORD0.xy));
				r3.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r3.xy));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r3.xy)));
				r1 = asuint(asfloat(r3) + asfloat(r1));
				r4 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-1.0, 0.0, 1.0, 0.0) + i_TEXCOORD0.xyxy));
				r4 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r4));
				r5 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.xy)));
				r4 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.zw)));
				r1 = asuint(asfloat(r5) + asfloat(r1));
				r5 = asuint(asfloat(r5) + asfloat(r3));
				r1 = asuint(float4(0.03125, 0.03125, 0.03125, 0.03125) * asfloat(r1));
				r0 = asuint(asfloat(r0) * float4(0.125, 0.125, 0.125, 0.125) + asfloat(r1));
				r1 = asuint(asfloat(r4) + asfloat(r2));
				r2 = asuint(asfloat(r4) + asfloat(r3));
				r1 = asuint(asfloat(r1) + asfloat(r3));
				r0 = asuint(asfloat(r1) * float4(0.03125, 0.03125, 0.03125, 0.03125) + asfloat(r0));
				r1 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-1.0, 1.0, 0.0, 1.0) + i_TEXCOORD0.xyxy));
				r1 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.zw)));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r4 = asuint(asfloat(r5) + asfloat(r3));
				r1 = asuint(asfloat(r4) + asfloat(r1));
				r0 = asuint(asfloat(r1) * float4(0.03125, 0.03125, 0.03125, 0.03125) + asfloat(r0));
				r1.xy = asuint(saturate(_MainTex_TexelSize.xy + i_TEXCOORD0.xy));
				r1.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r1 = asuint(asfloat(r2) + asfloat(r1));
				r1 = asuint(asfloat(r1) + asfloat(r3));
				r0 = asuint(asfloat(r1) * float4(0.03125, 0.03125, 0.03125, 0.03125) + asfloat(r0));
				r0 = asuint(min(float4(65504.0, 65504.0, 65504.0, 65504.0), asfloat(r0)));
				r1 = asuint(_AutoExposureTex.Sample(sampler_AutoExposureTex, i_TEXCOORD0.xy));
				r0 = asuint(asfloat(r1.xxxx) * asfloat(r0));
				r0 = asuint(min(_Params.xxxx, asfloat(r0)));
				r1.x = asuint(max(asfloat(r0.x), asfloat(r0.y)));
				r1.x = asuint(max(asfloat(r1.x), asfloat(r0.z)));
				r1.yz = asuint(-_Threshold.yx + asfloat(r1.xx));
				r1.xy = asuint(max(float2(9.99999975e-05, 0.0), asfloat(r1.xy)));
				r1.y = asuint(min(_Threshold.z, asfloat(r1.y)));
				r1.w = asuint(_Threshold.w * asfloat(r1.y));
				r1.y = asuint(asfloat(r1.w) * asfloat(r1.y));
				r1.y = asuint(max(asfloat(r1.y), asfloat(r1.z)));
				r1.x = asuint(asfloat(r1.y) / asfloat(r1.x));
				o_SV_Target0 = asfloat(r1.xxxx) * asfloat(r0);
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
			float4 _GlobalsPad5[2];
			float4 _Threshold;
			float4 _Params;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _AutoExposureTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_AutoExposureTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-1.0, -1.0, 1.0, -1.0) + i_TEXCOORD0.xyxy));
				r0 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.xy)));
				r0 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.zw)));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				r1 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-1.0, 1.0, 1.0, 1.0) + i_TEXCOORD0.xyxy));
				r1 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.zw)));
				r0 = asuint(asfloat(r2) + asfloat(r0));
				r0 = asuint(asfloat(r0) + asfloat(r1));
				r0 = asuint(float4(0.25, 0.25, 0.25, 0.25) * asfloat(r0));
				r0 = asuint(min(float4(65504.0, 65504.0, 65504.0, 65504.0), asfloat(r0)));
				r1 = asuint(_AutoExposureTex.Sample(sampler_AutoExposureTex, i_TEXCOORD0.xy));
				r0 = asuint(asfloat(r1.xxxx) * asfloat(r0));
				r0 = asuint(min(_Params.xxxx, asfloat(r0)));
				r1.x = asuint(max(asfloat(r0.x), asfloat(r0.y)));
				r1.x = asuint(max(asfloat(r1.x), asfloat(r0.z)));
				r1.yz = asuint(-_Threshold.yx + asfloat(r1.xx));
				r1.xy = asuint(max(float2(9.99999975e-05, 0.0), asfloat(r1.xy)));
				r1.y = asuint(min(_Threshold.z, asfloat(r1.y)));
				r1.w = asuint(_Threshold.w * asfloat(r1.y));
				r1.y = asuint(asfloat(r1.w) * asfloat(r1.y));
				r1.y = asuint(max(asfloat(r1.y), asfloat(r1.z)));
				r1.x = asuint(asfloat(r1.y) / asfloat(r1.x));
				o_SV_Target0 = asfloat(r1.xxxx) * asfloat(r0);
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
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				r0 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-0.5, -0.5, 0.5, -0.5) + i_TEXCOORD0.xyxy));
				r0 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.xy)));
				r0 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.zw)));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				r1 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-0.5, 0.5, 0.5, 0.5) + i_TEXCOORD0.xyxy));
				r1 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.zw)));
				r0 = asuint(asfloat(r2) + asfloat(r0));
				r0 = asuint(asfloat(r0) + asfloat(r1));
				r1.xy = asuint(saturate(-_MainTex_TexelSize.xy + i_TEXCOORD0.xy));
				r1.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r2 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(0.0, -1.0, 1.0, -1.0) + i_TEXCOORD0.xyxy));
				r2 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r2));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r2.xy)));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r2.zw)));
				r2 = asuint(asfloat(r3) + asfloat(r2));
				r1 = asuint(asfloat(r3) + asfloat(r1));
				r3.xy = asuint(saturate(i_TEXCOORD0.xy));
				r3.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r3.xy));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r3.xy)));
				r1 = asuint(asfloat(r3) + asfloat(r1));
				r4 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-1.0, 0.0, 1.0, 0.0) + i_TEXCOORD0.xyxy));
				r4 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r4));
				r5 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.xy)));
				r4 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.zw)));
				r1 = asuint(asfloat(r5) + asfloat(r1));
				r5 = asuint(asfloat(r5) + asfloat(r3));
				r1 = asuint(float4(0.03125, 0.03125, 0.03125, 0.03125) * asfloat(r1));
				r0 = asuint(asfloat(r0) * float4(0.125, 0.125, 0.125, 0.125) + asfloat(r1));
				r1 = asuint(asfloat(r4) + asfloat(r2));
				r2 = asuint(asfloat(r4) + asfloat(r3));
				r1 = asuint(asfloat(r1) + asfloat(r3));
				r0 = asuint(asfloat(r1) * float4(0.03125, 0.03125, 0.03125, 0.03125) + asfloat(r0));
				r1 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-1.0, 1.0, 0.0, 1.0) + i_TEXCOORD0.xyxy));
				r1 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.zw)));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r4 = asuint(asfloat(r5) + asfloat(r3));
				r1 = asuint(asfloat(r4) + asfloat(r1));
				r0 = asuint(asfloat(r1) * float4(0.03125, 0.03125, 0.03125, 0.03125) + asfloat(r0));
				r1.xy = asuint(saturate(_MainTex_TexelSize.xy + i_TEXCOORD0.xy));
				r1.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r1 = asuint(asfloat(r2) + asfloat(r1));
				r1 = asuint(asfloat(r1) + asfloat(r3));
				o_SV_Target0 = asfloat(r1) * float4(0.03125, 0.03125, 0.03125, 0.03125) + asfloat(r0);
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
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-1.0, -1.0, 1.0, -1.0) + i_TEXCOORD0.xyxy));
				r0 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.xy)));
				r0 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.zw)));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				r1 = asuint(saturate(_MainTex_TexelSize.xyxy * float4(-1.0, 1.0, 1.0, 1.0) + i_TEXCOORD0.xyxy));
				r1 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.zw)));
				r0 = asuint(asfloat(r2) + asfloat(r0));
				r0 = asuint(asfloat(r0) + asfloat(r1));
				o_SV_Target0 = float4(0.25, 0.25, 0.25, 0.25) * asfloat(r0);
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
			float _SampleScale;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _BloomTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_BloomTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				r0.xy = asuint(saturate(i_TEXCOORD0.xy));
				r0.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0.xy));
				r0 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.xy)));
				r1.x = 0x3f800000u;
				r1.z = asuint(_SampleScale);
				r1 = asuint(_MainTex_TexelSize.xyxy * asfloat(r1.xxzz));
				r2.zw = uint2(0xbf800000u, 0u);
				r2.x = asuint(_SampleScale);
				r3 = asuint(saturate(i_TEXCOORD0.xyxy - asfloat(r1.xywy) * asfloat(r2.xxwx)));
				r3 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r3));
				r4 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r3.xy)));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r3.zw)));
				r3 = asuint(asfloat(r3) * float4(2.0, 2.0, 2.0, 2.0) + asfloat(r4));
				r4.xy = asuint(saturate(i_TEXCOORD0.xy - asfloat(r1.zy) * asfloat(r2.zx)));
				r4.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r4.xy));
				r4 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.xy)));
				r3 = asuint(asfloat(r4) + asfloat(r3));
				r4 = asuint(saturate(asfloat(r1.zwxw) * asfloat(r2.zwxw) + i_TEXCOORD0.xyxy));
				r5 = asuint(saturate(asfloat(r1.zywy) * asfloat(r2.zxwx) + i_TEXCOORD0.xyxy));
				r1.xy = asuint(saturate(asfloat(r1.xy) * asfloat(r2.xx) + i_TEXCOORD0.xy));
				r1.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r2 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r5));
				r4 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r4));
				r5 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.xy)));
				r4 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.zw)));
				r3 = asuint(asfloat(r5) * float4(2.0, 2.0, 2.0, 2.0) + asfloat(r3));
				r0 = asuint(asfloat(r0) * float4(4.0, 4.0, 4.0, 4.0) + asfloat(r3));
				r0 = asuint(asfloat(r4) * float4(2.0, 2.0, 2.0, 2.0) + asfloat(r0));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r2.xy)));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r2.zw)));
				r0 = asuint(asfloat(r3) + asfloat(r0));
				r0 = asuint(asfloat(r2) * float4(2.0, 2.0, 2.0, 2.0) + asfloat(r0));
				r0 = asuint(asfloat(r0) + asfloat(r1));
				r1 = asuint(_BloomTex.Sample(sampler_BloomTex, i_TEXCOORD1.xy));
				o_SV_Target0 = asfloat(r0) * float4(0.0625, 0.0625, 0.0625, 0.0625) + asfloat(r1);
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
			float _SampleScale;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _BloomTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_BloomTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(float4(-1.0, -1.0, 1.0, 1.0) * _MainTex_TexelSize.xyxy);
				r1.x = asuint(0.5 * _SampleScale);
				r2 = asuint(saturate(asfloat(r0.xyzy) * asfloat(r1.xxxx) + i_TEXCOORD0.xyxy));
				r0 = asuint(saturate(asfloat(r0.xwzw) * asfloat(r1.xxxx) + i_TEXCOORD0.xyxy));
				r0 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0));
				r1 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r2));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.zw)));
				r1 = asuint(asfloat(r2) + asfloat(r1));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.xy)));
				r0 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.zw)));
				r1 = asuint(asfloat(r2) + asfloat(r1));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				r1 = asuint(_BloomTex.Sample(sampler_BloomTex, i_TEXCOORD1.xy));
				o_SV_Target0 = asfloat(r0) * float4(0.25, 0.25, 0.25, 0.25) + asfloat(r1);
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
			float4 _GlobalsPad0[31];
			float4 _Threshold;
			float4 _Params;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _AutoExposureTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_AutoExposureTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r0.xyz = asuint(min(float3(65504.0, 65504.0, 65504.0), asfloat(r0.xyz)));
				r1 = asuint(_AutoExposureTex.Sample(sampler_AutoExposureTex, i_TEXCOORD0.xy));
				r0.xyz = asuint(asfloat(r1.xxx) * asfloat(r0.xyz));
				r0.xyz = asuint(min(_Params.xxx, asfloat(r0.xyz)));
				r0.w = asuint(max(asfloat(r0.x), asfloat(r0.y)));
				r0.w = asuint(max(asfloat(r0.w), asfloat(r0.z)));
				r1.xy = asuint(-_Threshold.yx + asfloat(r0.ww));
				r0.w = asuint(max(9.99999975e-05, asfloat(r0.w)));
				r1.x = asuint(max(0.0, asfloat(r1.x)));
				r1.x = asuint(min(_Threshold.z, asfloat(r1.x)));
				r1.z = asuint(_Threshold.w * asfloat(r1.x));
				r1.x = asuint(asfloat(r1.z) * asfloat(r1.x));
				r1.x = asuint(max(asfloat(r1.x), asfloat(r1.y)));
				r0.w = asuint(asfloat(r1.x) / asfloat(r0.w));
				o_SV_Target0.xyz = asfloat(r0.xyz) * asfloat(r0.www);
				o_SV_Target0.w = 1.0;
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
			float _SampleScale;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			float4 _ColorIntensity;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				r0.xy = asuint(saturate(i_TEXCOORD0.xy));
				r0.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0.xy));
				r0 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.xy)));
				r1.x = 0x3f800000u;
				r1.z = asuint(_SampleScale);
				r1 = asuint(_MainTex_TexelSize.xyxy * asfloat(r1.xxzz));
				r2.zw = uint2(0xbf800000u, 0u);
				r2.x = asuint(_SampleScale);
				r3 = asuint(saturate(i_TEXCOORD0.xyxy - asfloat(r1.xywy) * asfloat(r2.xxwx)));
				r3 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r3));
				r4 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r3.xy)));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r3.zw)));
				r3.xyz = asuint(asfloat(r3.xyz) * float3(2.0, 2.0, 2.0) + asfloat(r4.xyz));
				r4.xy = asuint(saturate(i_TEXCOORD0.xy - asfloat(r1.zy) * asfloat(r2.zx)));
				r4.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r4.xy));
				r4 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.xy)));
				r3.xyz = asuint(asfloat(r4.xyz) + asfloat(r3.xyz));
				r4 = asuint(saturate(asfloat(r1.zwxw) * asfloat(r2.zwxw) + i_TEXCOORD0.xyxy));
				r5 = asuint(saturate(asfloat(r1.zywy) * asfloat(r2.zxwx) + i_TEXCOORD0.xyxy));
				r1.xy = asuint(saturate(asfloat(r1.xy) * asfloat(r2.xx) + i_TEXCOORD0.xy));
				r1.xy = asuint(float2(_RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r1.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r2 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r5));
				r4 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r4));
				r5 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.xy)));
				r4 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r4.zw)));
				r3.xyz = asuint(asfloat(r5.xyz) * float3(2.0, 2.0, 2.0) + asfloat(r3.xyz));
				r0.xyz = asuint(asfloat(r0.xyz) * float3(4.0, 4.0, 4.0) + asfloat(r3.xyz));
				r0.xyz = asuint(asfloat(r4.xyz) * float3(2.0, 2.0, 2.0) + asfloat(r0.xyz));
				r3 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r2.xy)));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r2.zw)));
				r0.xyz = asuint(asfloat(r3.xyz) + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r2.xyz) * float3(2.0, 2.0, 2.0) + asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r0.xyz) + asfloat(r1.xyz));
				r0.xyz = asuint(float3(0.0625, 0.0625, 0.0625) * asfloat(r0.xyz));
				r0.xyz = asuint(_ColorIntensity.www * asfloat(r0.xyz));
				o_SV_Target0.xyz = _ColorIntensity.xyz * asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
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
			float _SampleScale;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			float4 _ColorIntensity;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(float4(-1.0, -1.0, 1.0, 1.0) * _MainTex_TexelSize.xyxy);
				r1.x = asuint(0.5 * _SampleScale);
				r2 = asuint(saturate(asfloat(r0.xyzy) * asfloat(r1.xxxx) + i_TEXCOORD0.xyxy));
				r0 = asuint(saturate(asfloat(r0.xwzw) * asfloat(r1.xxxx) + i_TEXCOORD0.xyxy));
				r0 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r0));
				r1 = asuint(float4(_RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor, _RenderViewportScaleFactor) * asfloat(r2));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.xy)));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r1.zw)));
				r1.xyz = asuint(asfloat(r2.xyz) + asfloat(r1.xyz));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.xy)));
				r0 = asuint(_MainTex.Sample(sampler_MainTex, asfloat(r0.zw)));
				r1.xyz = asuint(asfloat(r2.xyz) + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) + asfloat(r0.xyz));
				r0.xyz = asuint(float3(0.25, 0.25, 0.25) * asfloat(r0.xyz));
				r0.xyz = asuint(_ColorIntensity.www * asfloat(r0.xyz));
				o_SV_Target0.xyz = _ColorIntensity.xyz * asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#endif
			ENDHLSL
		}
	}
}
