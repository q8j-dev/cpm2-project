Shader "HighlightPlus/Geometry/SeeThroughInner"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SeeThrough ("See Through", Range(0, 1)) = 0.8
		_SeeThroughTintColor ("See Through Tint Color", Color) = (1,0,0,0.8)
		_SeeThroughNoise ("Noise", Float) = 1
		_Color ("Color", Color) = (1,1,1,1)
		_CutOff ("CutOff", Float) = 0.5
		_SeeThroughBorderWidth ("Outline Offset", Float) = 0.01
		_SeeThroughBorderConstantWidth ("Constant Width", Float) = 1
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "QUEUE" = "Transparent+201" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Back
			ZTest LEqual
			ZWrite On
			Stencil { Ref 2 ReadMask 3 WriteMask 255 Comp Greater Pass Keep Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ HP_ALPHACLIP

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _MainTex_ST;
			float4 _GlobalsPad1[4];
			float _SeeThroughBorderWidth;
			float _SeeThroughBorderConstantWidth;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ProjectionParams : packoffset(c5);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixInvV : packoffset(c13);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xyz = asuint(unity_MatrixInvV._m11_m11_m11 * unity_WorldToObject._m01_m11_m21);
				r0.xyz = asuint(unity_WorldToObject._m00_m10_m20 * unity_MatrixInvV._m01_m01_m01 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * unity_MatrixInvV._m21_m21_m21 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 * unity_MatrixInvV._m31_m31_m31 + asfloat(r0.xyz));
				r0.x = asuint(dot(asfloat(r0.xyz), i_NORMAL0.xyz));
				r0.xy = asuint(glstate_matrix_projection._m01_m11 * asfloat(r0.xx));
				r1.xyz = asuint(unity_MatrixInvV._m10_m10_m10 * unity_WorldToObject._m01_m11_m21);
				r1.xyz = asuint(unity_WorldToObject._m00_m10_m20 * unity_MatrixInvV._m00_m00_m00 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_WorldToObject._m02_m12_m22 * unity_MatrixInvV._m20_m20_m20 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_WorldToObject._m03_m13_m23 * unity_MatrixInvV._m30_m30_m30 + asfloat(r1.xyz));
				r0.z = asuint(dot(asfloat(r1.xyz), i_NORMAL0.xyz));
				r0.xy = asuint(glstate_matrix_projection._m00_m10 * asfloat(r0.zz) + asfloat(r0.xy));
				r1 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r1 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r1));
				r2 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r1.yyyy));
				r2 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r1.xxxx) + asfloat(r2));
				r2 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r1.zzzz) + asfloat(r2));
				r1 = asuint(unity_MatrixVP._m03_m13_m23_m33 * asfloat(r1.wwww) + asfloat(r2));
				r0.z = asuint(asfloat(r1.z) / _ProjectionParams.y);
				r0.z = asuint(1.0 + -asfloat(r0.z));
				r0.z = asuint(_ProjectionParams.z * asfloat(r0.z));
				r0.z = asuint(max(0.0, asfloat(r0.z)));
				r0.w = asuint(2.0 + -asfloat(r0.z));
				r0.z = asuint(glstate_matrix_projection._m33 * asfloat(r0.w) + asfloat(r0.z));
				r0.z = asuint(-2.0 + asfloat(r0.z));
				r0.z = asuint(_SeeThroughBorderConstantWidth * asfloat(r0.z) + 2.0);
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				o_SV_POSITION0.xy = asfloat(r1.xy) - asfloat(r0.xy) * float2(_SeeThroughBorderWidth, _SeeThroughBorderWidth);
				r0.x = asuint(asfloat(r1.w) + -asfloat(r1.z));
				o_SV_POSITION0.z = asfloat(r1.w) - asfloat(r0.x) * 0.00100000005;
				o_SV_POSITION0.w = asfloat(r1.w);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(HP_ALPHACLIP)
			float4 _GlobalsPad0[3];
			float _SeeThrough;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _SeeThroughTintColor;
			float _CutOff;
			float _SeeThroughNoise;
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
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.w = asuint(-_CutOff + asfloat(r0.w));
				b0.w = (asfloat(r0.w) < 0.0);
				if (b0.w) discard;
				r1.xyz = asuint(_SeeThroughTintColor.xyz + -asfloat(r0.xyz));
				r0.xyz = asuint(_SeeThroughTintColor.www * asfloat(r1.xyz) + asfloat(r0.xyz));
				r0.w = asuint(_Time.w * i_SV_POSITION0.y);
				r0.w = asuint(frac(asfloat(r0.w)));
				r0.w = asuint(0.100000001 * asfloat(r0.w));
				o_SV_Target0.xyz = float3(_SeeThroughNoise, _SeeThroughNoise, _SeeThroughNoise) * asfloat(r0.www) + asfloat(r0.xyz);
				r0.x = asuint(i_SV_POSITION0.y + i_SV_POSITION0.y);
				b0.x = (asfloat(r0.x) >= -asfloat(r0.x));
				r0.xy = b0.xx ? uint2(0x40000000u, 0x3f000000u) : uint2(0xc0000000u, 0xbf000000u);
				r0.y = asuint(i_SV_POSITION0.y * asfloat(r0.y));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y) + -1.0);
				r0.x = asuint(_SeeThrough * asfloat(r0.x) + -_SeeThrough);
				o_SV_Target0.w = _SeeThroughNoise * asfloat(r0.x) + _SeeThrough;
			}
			#elif 1
			float4 _GlobalsPad0[3];
			float _SeeThrough;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _SeeThroughTintColor;
			float _GlobalsPad4;
			float _SeeThroughNoise;
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
				bool4 b0;
				r0.x = asuint(i_SV_POSITION0.y + i_SV_POSITION0.y);
				b0.x = (asfloat(r0.x) >= -asfloat(r0.x));
				r0.xy = b0.xx ? uint2(0x40000000u, 0x3f000000u) : uint2(0xc0000000u, 0xbf000000u);
				r0.y = asuint(i_SV_POSITION0.y * asfloat(r0.y));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y) + -1.0);
				r0.x = asuint(_SeeThrough * asfloat(r0.x) + -_SeeThrough);
				o_SV_Target0.w = _SeeThroughNoise * asfloat(r0.x) + _SeeThrough;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1.xyz = asuint(_SeeThroughTintColor.xyz + -asfloat(r0.xyz));
				r0.xyz = asuint(_SeeThroughTintColor.www * asfloat(r1.xyz) + asfloat(r0.xyz));
				r0.w = asuint(_Time.w * i_SV_POSITION0.y);
				r0.w = asuint(frac(asfloat(r0.w)));
				r0.w = asuint(0.100000001 * asfloat(r0.w));
				o_SV_Target0.xyz = float3(_SeeThroughNoise, _SeeThroughNoise, _SeeThroughNoise) * asfloat(r0.www) + asfloat(r0.xyz);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
