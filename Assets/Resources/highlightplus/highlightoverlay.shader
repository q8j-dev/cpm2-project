Shader "HighlightPlus/Geometry/Overlay"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_OverlayBackColor ("Overlay Back Color", Color) = (1,1,1,1)
		_OverlayData ("Overlay Data", Vector) = (1,0.5,1,1)
		_CutOff ("CutOff", Float) = 0.5
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "QUEUE" = "Transparent+121" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Back
			ZTest LEqual
			ZWrite Off
			Offset -1, -1
			Stencil { Ref 2 ReadMask 255 WriteMask 255 Comp Equal Pass Keep Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ HP_ALPHACLIP

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[3];
			float4 _MainTex_ST;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0)
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
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(HP_ALPHACLIP)
			float4 _GlobalsPad0[2];
			float4 _Color;
			float4 _GlobalsPad1[1];
			float4 _OverlayBackColor;
			float3 _OverlayData;
			float _CutOff;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1.x = asuint(-_CutOff + asfloat(r0.w));
				r0 = asuint(_OverlayBackColor.xyzw * asfloat(r0));
				r0 = asuint(asfloat(r0) * _Color.xyzw + -_Color.xyzw);
				r0 = asuint(_OverlayData.zzzz * asfloat(r0) + _Color.xyzw);
				b1.x = (asfloat(r1.x) < 0.0);
				if (b1.x) discard;
				r1.x = asuint(_Time.y * _OverlayData.x);
				r1.x = asuint(frac(asfloat(r1.x)));
				r1.x = asuint(0.5 + -asfloat(r1.x));
				r1.y = asuint(1.0 + -_OverlayData.y);
				r1.y = asuint(asfloat(r1.y) + asfloat(r1.y));
				r1.x = asuint(asfloat(r1.y) * abs(asfloat(r1.x)) + _OverlayData.y);
				o_SV_Target0.w = asfloat(r1.x) * asfloat(r0.w);
				o_SV_Target0.xyz = asfloat(r0.xyz);
			}
			#elif 1
			float4 _GlobalsPad0[2];
			float4 _Color;
			float4 _GlobalsPad1[1];
			float4 _OverlayBackColor;
			float3 _OverlayData;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(_Time.y * _OverlayData.x);
				r0.x = asuint(frac(asfloat(r0.x)));
				r0.x = asuint(0.5 + -asfloat(r0.x));
				r0.y = asuint(1.0 + -_OverlayData.y);
				r0.y = asuint(asfloat(r0.y) + asfloat(r0.y));
				r0.x = asuint(asfloat(r0.y) * abs(asfloat(r0.x)) + _OverlayData.y);
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1 = asuint(_OverlayBackColor.xyzw * asfloat(r1));
				r1 = asuint(asfloat(r1) * _Color.xyzw + -_Color.xyzw);
				r1 = asuint(_OverlayData.zzzz * asfloat(r1) + _Color.xyzw);
				o_SV_Target0.w = asfloat(r1.w) * asfloat(r0.x);
				o_SV_Target0.xyz = asfloat(r1.xyz);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
