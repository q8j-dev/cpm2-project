Shader "UI/CircularWaves"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[HDR] _WaveColor ("Wave Color", Color) = (1,0.85,0.4,1)
		[HDR] _BaseColor ("Base Fill", Color) = (0.35,0.28,0.1,0)
		_Center ("Center (UV)", Vector) = (0.5,0.5,0,0)
		_MinRadius ("Min Radius", Range(0, 1)) = 0.217
		_MaxRadius ("Max Radius", Range(0, 1)) = 0.627
		_Feather ("Range Feather", Range(0.0005, 0.5)) = 0.065
		_Frequency ("Rings Per UV", Range(0.5, 64)) = 9.5
		_RingWidth ("Ring Width", Range(0.002, 1)) = 0.595
		_InnerRingFeather ("Inner Ring Feather", Range(0.0005, 1)) = 0.672
		_Speed ("Speed", Range(-10, 10)) = 2
		_Phase ("Phase Offset", Range(0, 1)) = 0.109
		_OuterFade ("Outer Fade", Range(0, 4)) = 4
		_TimeScale ("Time Scale", Range(0, 4)) = 1
		_UseGlobalTime ("Use Global _Time.y", Float) = 1
		_OutsideAlpha ("Outside Alpha", Range(0, 1)) = 0
		_BaseFillAlpha ("Base Alpha", Range(0, 1)) = 0
		_RingAlpha ("Ring Alpha", Range(0, 1)) = 0.7
		[HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector] _Stencil ("Stencil ID", Float) = 0
		[HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
		[HideInInspector] _ColorMask ("Color Mask", Float) = 15
		[HideInInspector] _ClipRect ("Clip Rect", Vector) = (0,0,1,1)
		[HideInInspector] _MaskSoftnessX ("Mask Softness X", Float) = 0
		[HideInInspector] _MaskSoftnessY ("Mask Softness Y", Float) = 0
	}
	SubShader
	{
		Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" }
		Pass
		{
			Name "UI-CircularWaves"
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ColorMask [_ColorMask]
			Cull Off
			ZTest LEqual
			ZWrite Off
			Stencil { Ref [_Stencil] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] Comp [_StencilComp] Pass [_StencilOp] Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ UNITY_UI_CLIP_RECT

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _MainTex_ST;
			float4 _Color;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_COLOR0 : COLOR0, out float4 o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o_COLOR0 = _Color.xyzw * i_COLOR0.xyzw;
				o_TEXCOORD1 = i_POSITION0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[4];
			float4 _WaveColor;
			float4 _BaseColor;
			float4 _Center;
			float _MinRadius;
			float _MaxRadius;
			float _Feather;
			float _Frequency;
			float _RingWidth;
			float _InnerRingFeather;
			float _Speed;
			float _Phase;
			float _OuterFade;
			float _TimeScale;
			float _UseGlobalTime;
			float _OutsideAlpha;
			float _BaseFillAlpha;
			float _RingAlpha;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _ClipRect;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b2;
				r0.xy = asuint(saturate(float2(_RingWidth, _InnerRingFeather)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0.y = asuint(asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(1.0 / asfloat(r0.x));
				b0.z = (0.5 < _UseGlobalTime);
				r0.w = asuint(_Time.y * _TimeScale);
				r1.x = asuint(_TimeScale * _Phase);
				r0.z = b0.z ? r0.w : r1.x;
				r0.z = asuint(asfloat(r0.z) * _Speed + _Phase);
				r1.xy = asuint(-_Center.xy + i_TEXCOORD0.xy);
				r0.w = asuint(dot(asfloat(r1.xy), asfloat(r1.xy)));
				r0.w = asuint(sqrt(asfloat(r0.w)));
				r0.z = asuint(asfloat(r0.w) * _Frequency + -asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.y = asuint(asfloat(r0.z) + -asfloat(r0.y));
				b0.z = (1.0 >= asfloat(r0.z));
				r0.z = b0.z ? 0x3f800000u : 0u;
				r0.x = asuint(saturate(asfloat(r0.y) * asfloat(r0.x)));
				r0.y = asuint(asfloat(r0.x) * -2.0 + 3.0);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.x));
				r0.x = asuint(asfloat(r0.y) * asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				r0.x = asuint(min(1.0, asfloat(r0.x)));
				r0.yz = asuint(float2(_MaxRadius, _MaxRadius) + -float2(_Feather, _MinRadius));
				r1.x = asuint(asfloat(r0.w) + -asfloat(r0.y));
				r0.w = asuint(-_MinRadius + asfloat(r0.w));
				r0.y = asuint(_MaxRadius + -asfloat(r0.y));
				r0.z = asuint(max(9.99999975e-05, asfloat(r0.z)));
				r0.z = asuint(saturate(asfloat(r0.w) / asfloat(r0.z)));
				r0.z = asuint(1.0 + -asfloat(r0.z));
				r0.z = asuint(log2(asfloat(r0.z)));
				r0.z = asuint(_OuterFade * asfloat(r0.z));
				r0.z = asuint(exp2(asfloat(r0.z)));
				r0.y = asuint(1.0 / asfloat(r0.y));
				r0.y = asuint(saturate(asfloat(r1.x) * asfloat(r0.y)));
				r1.x = asuint(asfloat(r0.y) * -2.0 + 3.0);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.y));
				r0.y = asuint(1.0 - asfloat(r1.x) * asfloat(r0.y));
				r1.x = asuint(1.0 / _Feather);
				r0.w = asuint(saturate(asfloat(r1.x) * asfloat(r0.w)));
				r1.x = asuint(asfloat(r0.w) * -2.0 + 3.0);
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				r0.w = asuint(asfloat(r1.x) * asfloat(r0.w));
				r0.y = asuint(saturate(asfloat(r0.w) * asfloat(r0.y)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				r0.z = asuint(1.0 + -asfloat(r0.y));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.w = asuint(i_COLOR0.w * asfloat(r1.w));
				r1.x = asuint(_BaseFillAlpha * asfloat(r0.w));
				r0.w = asuint(_OutsideAlpha * asfloat(r0.w));
				r1.x = asuint(asfloat(r1.x) * asfloat(r0.y));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z) + asfloat(r1.x));
				r0.z = asuint(saturate(_RingAlpha * asfloat(r0.x) + asfloat(r0.z)));
				r0.x = asuint(_WaveColor.w * asfloat(r0.x));
				r1.xyz = asuint(_WaveColor.xyz * asfloat(r0.xxx));
				b0.xw = (i_TEXCOORD1.xy >= _ClipRect.xy);
				r0.xw = b0.xw ? uint2(0x3f800000u, 0x3f800000u) : uint2(0u, 0u);
				b2.xy = (_ClipRect.zw >= i_TEXCOORD1.xy);
				r2.xy = b2.xy ? uint2(0x3f800000u, 0x3f800000u) : uint2(0u, 0u);
				r0.xw = asuint(asfloat(r2.xy) * asfloat(r0.xw));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.w));
				r0.w = asuint(asfloat(r0.z) * asfloat(r0.x) + -0.00100000005);
				r0.x = asuint(asfloat(r0.z) * asfloat(r0.x));
				o_SV_Target0.w = asfloat(r0.x);
				b0.x = (asfloat(r0.w) < 0.0);
				if (b0.x) discard;
				r0.x = asuint(saturate(_BaseColor.w));
				r0.xzw = asuint(_BaseColor.xyz * asfloat(r0.xxx));
				o_SV_Target0.xyz = asfloat(r0.xzw) * asfloat(r0.yyy) + asfloat(r1.xyz);
			}
			#elif defined(UNITY_UI_ALPHACLIP)
			float4 _GlobalsPad0[4];
			float4 _WaveColor;
			float4 _BaseColor;
			float4 _Center;
			float _MinRadius;
			float _MaxRadius;
			float _Feather;
			float _Frequency;
			float _RingWidth;
			float _InnerRingFeather;
			float _Speed;
			float _Phase;
			float _OuterFade;
			float _TimeScale;
			float _UseGlobalTime;
			float _OutsideAlpha;
			float _BaseFillAlpha;
			float _RingAlpha;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.xy = asuint(saturate(float2(_RingWidth, _InnerRingFeather)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0.y = asuint(asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(1.0 / asfloat(r0.x));
				b0.z = (0.5 < _UseGlobalTime);
				r0.w = asuint(_Time.y * _TimeScale);
				r1.x = asuint(_TimeScale * _Phase);
				r0.z = b0.z ? r0.w : r1.x;
				r0.z = asuint(asfloat(r0.z) * _Speed + _Phase);
				r1.xy = asuint(-_Center.xy + i_TEXCOORD0.xy);
				r0.w = asuint(dot(asfloat(r1.xy), asfloat(r1.xy)));
				r0.w = asuint(sqrt(asfloat(r0.w)));
				r0.z = asuint(asfloat(r0.w) * _Frequency + -asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.y = asuint(asfloat(r0.z) + -asfloat(r0.y));
				b0.z = (1.0 >= asfloat(r0.z));
				r0.z = b0.z ? 0x3f800000u : 0u;
				r0.x = asuint(saturate(asfloat(r0.y) * asfloat(r0.x)));
				r0.y = asuint(asfloat(r0.x) * -2.0 + 3.0);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.x));
				r0.x = asuint(asfloat(r0.y) * asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				r0.x = asuint(min(1.0, asfloat(r0.x)));
				r0.yz = asuint(float2(_MaxRadius, _MaxRadius) + -float2(_Feather, _MinRadius));
				r1.x = asuint(asfloat(r0.w) + -asfloat(r0.y));
				r0.w = asuint(-_MinRadius + asfloat(r0.w));
				r0.y = asuint(_MaxRadius + -asfloat(r0.y));
				r0.z = asuint(max(9.99999975e-05, asfloat(r0.z)));
				r0.z = asuint(saturate(asfloat(r0.w) / asfloat(r0.z)));
				r0.z = asuint(1.0 + -asfloat(r0.z));
				r0.z = asuint(log2(asfloat(r0.z)));
				r0.z = asuint(_OuterFade * asfloat(r0.z));
				r0.z = asuint(exp2(asfloat(r0.z)));
				r0.y = asuint(1.0 / asfloat(r0.y));
				r0.y = asuint(saturate(asfloat(r1.x) * asfloat(r0.y)));
				r1.x = asuint(asfloat(r0.y) * -2.0 + 3.0);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.y));
				r0.y = asuint(1.0 - asfloat(r1.x) * asfloat(r0.y));
				r1.x = asuint(1.0 / _Feather);
				r0.w = asuint(saturate(asfloat(r1.x) * asfloat(r0.w)));
				r1.x = asuint(asfloat(r0.w) * -2.0 + 3.0);
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				r0.w = asuint(asfloat(r1.x) * asfloat(r0.w));
				r0.y = asuint(saturate(asfloat(r0.w) * asfloat(r0.y)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				r0.z = asuint(1.0 + -asfloat(r0.y));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.w = asuint(i_COLOR0.w * asfloat(r1.w));
				r1.x = asuint(_BaseFillAlpha * asfloat(r0.w));
				r0.w = asuint(_OutsideAlpha * asfloat(r0.w));
				r1.x = asuint(asfloat(r1.x) * asfloat(r0.y));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z) + asfloat(r1.x));
				r0.z = asuint(saturate(_RingAlpha * asfloat(r0.x) + asfloat(r0.z)));
				r0.x = asuint(_WaveColor.w * asfloat(r0.x));
				r1.xyz = asuint(_WaveColor.xyz * asfloat(r0.xxx));
				r0.x = asuint(-0.00100000005 + asfloat(r0.z));
				o_SV_Target0.w = asfloat(r0.z);
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				r0.x = asuint(saturate(_BaseColor.w));
				r0.xzw = asuint(_BaseColor.xyz * asfloat(r0.xxx));
				o_SV_Target0.xyz = asfloat(r0.xzw) * asfloat(r0.yyy) + asfloat(r1.xyz);
			}
			#elif defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[4];
			float4 _WaveColor;
			float4 _BaseColor;
			float4 _Center;
			float _MinRadius;
			float _MaxRadius;
			float _Feather;
			float _Frequency;
			float _RingWidth;
			float _InnerRingFeather;
			float _Speed;
			float _Phase;
			float _OuterFade;
			float _TimeScale;
			float _UseGlobalTime;
			float _OutsideAlpha;
			float _BaseFillAlpha;
			float _RingAlpha;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _ClipRect;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b2;
				r0.xy = asuint(saturate(float2(_RingWidth, _InnerRingFeather)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0.y = asuint(asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(1.0 / asfloat(r0.x));
				b0.z = (0.5 < _UseGlobalTime);
				r0.w = asuint(_Time.y * _TimeScale);
				r1.x = asuint(_TimeScale * _Phase);
				r0.z = b0.z ? r0.w : r1.x;
				r0.z = asuint(asfloat(r0.z) * _Speed + _Phase);
				r1.xy = asuint(-_Center.xy + i_TEXCOORD0.xy);
				r0.w = asuint(dot(asfloat(r1.xy), asfloat(r1.xy)));
				r0.w = asuint(sqrt(asfloat(r0.w)));
				r0.z = asuint(asfloat(r0.w) * _Frequency + -asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.y = asuint(asfloat(r0.z) + -asfloat(r0.y));
				b0.z = (1.0 >= asfloat(r0.z));
				r0.z = b0.z ? 0x3f800000u : 0u;
				r0.x = asuint(saturate(asfloat(r0.y) * asfloat(r0.x)));
				r0.y = asuint(asfloat(r0.x) * -2.0 + 3.0);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.x));
				r0.x = asuint(asfloat(r0.y) * asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				r0.x = asuint(min(1.0, asfloat(r0.x)));
				r0.yz = asuint(float2(_MaxRadius, _MaxRadius) + -float2(_Feather, _MinRadius));
				r1.x = asuint(asfloat(r0.w) + -asfloat(r0.y));
				r0.w = asuint(-_MinRadius + asfloat(r0.w));
				r0.y = asuint(_MaxRadius + -asfloat(r0.y));
				r0.z = asuint(max(9.99999975e-05, asfloat(r0.z)));
				r0.z = asuint(saturate(asfloat(r0.w) / asfloat(r0.z)));
				r0.z = asuint(1.0 + -asfloat(r0.z));
				r0.z = asuint(log2(asfloat(r0.z)));
				r0.z = asuint(_OuterFade * asfloat(r0.z));
				r0.z = asuint(exp2(asfloat(r0.z)));
				r0.y = asuint(1.0 / asfloat(r0.y));
				r0.y = asuint(saturate(asfloat(r1.x) * asfloat(r0.y)));
				r1.x = asuint(asfloat(r0.y) * -2.0 + 3.0);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.y));
				r0.y = asuint(1.0 - asfloat(r1.x) * asfloat(r0.y));
				r1.x = asuint(1.0 / _Feather);
				r0.w = asuint(saturate(asfloat(r1.x) * asfloat(r0.w)));
				r1.x = asuint(asfloat(r0.w) * -2.0 + 3.0);
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				r0.w = asuint(asfloat(r1.x) * asfloat(r0.w));
				r0.y = asuint(saturate(asfloat(r0.w) * asfloat(r0.y)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				r0.z = asuint(1.0 + -asfloat(r0.y));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.w = asuint(i_COLOR0.w * asfloat(r1.w));
				r1.x = asuint(_BaseFillAlpha * asfloat(r0.w));
				r0.w = asuint(_OutsideAlpha * asfloat(r0.w));
				r1.x = asuint(asfloat(r1.x) * asfloat(r0.y));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z) + asfloat(r1.x));
				r0.z = asuint(saturate(_RingAlpha * asfloat(r0.x) + asfloat(r0.z)));
				r0.x = asuint(_WaveColor.w * asfloat(r0.x));
				r1.xyz = asuint(_WaveColor.xyz * asfloat(r0.xxx));
				b0.xw = (i_TEXCOORD1.xy >= _ClipRect.xy);
				r0.xw = b0.xw ? uint2(0x3f800000u, 0x3f800000u) : uint2(0u, 0u);
				b2.xy = (_ClipRect.zw >= i_TEXCOORD1.xy);
				r2.xy = b2.xy ? uint2(0x3f800000u, 0x3f800000u) : uint2(0u, 0u);
				r0.xw = asuint(asfloat(r2.xy) * asfloat(r0.xw));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.w));
				o_SV_Target0.w = asfloat(r0.z) * asfloat(r0.x);
				r0.x = asuint(saturate(_BaseColor.w));
				r0.xzw = asuint(_BaseColor.xyz * asfloat(r0.xxx));
				o_SV_Target0.xyz = asfloat(r0.xzw) * asfloat(r0.yyy) + asfloat(r1.xyz);
			}
			#elif 1
			float4 _GlobalsPad0[4];
			float4 _WaveColor;
			float4 _BaseColor;
			float4 _Center;
			float _MinRadius;
			float _MaxRadius;
			float _Feather;
			float _Frequency;
			float _RingWidth;
			float _InnerRingFeather;
			float _Speed;
			float _Phase;
			float _OuterFade;
			float _TimeScale;
			float _UseGlobalTime;
			float _OutsideAlpha;
			float _BaseFillAlpha;
			float _RingAlpha;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.xy = asuint(saturate(float2(_RingWidth, _InnerRingFeather)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0.y = asuint(asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(1.0 / asfloat(r0.x));
				b0.z = (0.5 < _UseGlobalTime);
				r0.w = asuint(_Time.y * _TimeScale);
				r1.x = asuint(_TimeScale * _Phase);
				r0.z = b0.z ? r0.w : r1.x;
				r0.z = asuint(asfloat(r0.z) * _Speed + _Phase);
				r1.xy = asuint(-_Center.xy + i_TEXCOORD0.xy);
				r0.w = asuint(dot(asfloat(r1.xy), asfloat(r1.xy)));
				r0.w = asuint(sqrt(asfloat(r0.w)));
				r0.z = asuint(asfloat(r0.w) * _Frequency + -asfloat(r0.z));
				r0.z = asuint(frac(asfloat(r0.z)));
				r0.y = asuint(asfloat(r0.z) + -asfloat(r0.y));
				b0.z = (1.0 >= asfloat(r0.z));
				r0.z = b0.z ? 0x3f800000u : 0u;
				r0.x = asuint(saturate(asfloat(r0.y) * asfloat(r0.x)));
				r0.y = asuint(asfloat(r0.x) * -2.0 + 3.0);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.x));
				r0.x = asuint(asfloat(r0.y) * asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				r0.x = asuint(min(1.0, asfloat(r0.x)));
				r0.yz = asuint(float2(_MaxRadius, _MaxRadius) + -float2(_Feather, _MinRadius));
				r1.x = asuint(asfloat(r0.w) + -asfloat(r0.y));
				r0.w = asuint(-_MinRadius + asfloat(r0.w));
				r0.y = asuint(_MaxRadius + -asfloat(r0.y));
				r0.z = asuint(max(9.99999975e-05, asfloat(r0.z)));
				r0.z = asuint(saturate(asfloat(r0.w) / asfloat(r0.z)));
				r0.z = asuint(1.0 + -asfloat(r0.z));
				r0.z = asuint(log2(asfloat(r0.z)));
				r0.z = asuint(_OuterFade * asfloat(r0.z));
				r0.z = asuint(exp2(asfloat(r0.z)));
				r0.y = asuint(1.0 / asfloat(r0.y));
				r0.y = asuint(saturate(asfloat(r1.x) * asfloat(r0.y)));
				r1.x = asuint(asfloat(r0.y) * -2.0 + 3.0);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.y));
				r0.y = asuint(1.0 - asfloat(r1.x) * asfloat(r0.y));
				r1.x = asuint(1.0 / _Feather);
				r0.w = asuint(saturate(asfloat(r1.x) * asfloat(r0.w)));
				r1.x = asuint(asfloat(r0.w) * -2.0 + 3.0);
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				r0.w = asuint(asfloat(r1.x) * asfloat(r0.w));
				r0.y = asuint(saturate(asfloat(r0.w) * asfloat(r0.y)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z));
				r0.z = asuint(1.0 + -asfloat(r0.y));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.w = asuint(i_COLOR0.w * asfloat(r1.w));
				r1.x = asuint(_BaseFillAlpha * asfloat(r0.w));
				r0.w = asuint(_OutsideAlpha * asfloat(r0.w));
				r1.x = asuint(asfloat(r1.x) * asfloat(r0.y));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.z) + asfloat(r1.x));
				o_SV_Target0.w = saturate(_RingAlpha * asfloat(r0.x) + asfloat(r0.z));
				r0.x = asuint(_WaveColor.w * asfloat(r0.x));
				r0.xzw = asuint(_WaveColor.xyz * asfloat(r0.xxx));
				r1.x = asuint(saturate(_BaseColor.w));
				r1.xyz = asuint(_BaseColor.xyz * asfloat(r1.xxx));
				o_SV_Target0.xyz = asfloat(r1.xyz) * asfloat(r0.yyy) + asfloat(r0.xzw);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
