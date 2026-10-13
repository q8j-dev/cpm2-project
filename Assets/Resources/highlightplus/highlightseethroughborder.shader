Shader "HighlightPlus/Geometry/SeeThroughBorder"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SeeThroughBorderColor ("Outline Color", Color) = (0,0,0,1)
		_Color ("Color", Color) = (1,1,1,1)
		_CutOff ("CutOff", Float) = 0.5
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "QUEUE" = "Transparent+201" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Back
			ZTest LEqual
			ZWrite Off
			Stencil { Ref 2 ReadMask 3 WriteMask 255 Comp Greater Pass Keep Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ HP_ALPHACLIP

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
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
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1));
				r0.z = asuint(asfloat(r0.w) + -asfloat(r0.z));
				r0.z = asuint(asfloat(r0.w) - asfloat(r0.z) * 0.00100000005);
				o_SV_POSITION0.xyw = asfloat(r0.xyw);
				o_SV_POSITION0.z = -9.99999975e-05 + asfloat(r0.z);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(HP_ALPHACLIP)
			float4 _GlobalsPad0[3];
			float4 _SeeThroughBorderColor;
			float _CutOff;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(-_CutOff + asfloat(r0.w));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				o_SV_Target0 = _SeeThroughBorderColor.xyzw;
			}
			#elif 1
			float4 _GlobalsPad0[3];
			float4 _SeeThroughBorderColor;

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				o_SV_Target0 = _SeeThroughBorderColor.xyzw;
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
