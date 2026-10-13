Shader "TextMeshPro/Mobile/Distance Field (Surface)"
{
	Properties
	{
		_FaceTex ("Fill Texture", 2D) = "white" {}
		[HDR] _FaceColor ("Fill Color", Color) = (1,1,1,1)
		_FaceDilate ("Face Dilate", Range(-1, 1)) = 0
		[HDR] _OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineTex ("Outline Texture", 2D) = "white" {}
		_OutlineWidth ("Outline Thickness", Range(0, 1)) = 0
		_OutlineSoftness ("Outline Softness", Range(0, 1)) = 0
		[HDR] _GlowColor ("Color", Color) = (0,1,0,0.5)
		_GlowOffset ("Offset", Range(-1, 1)) = 0
		_GlowInner ("Inner", Range(0, 1)) = 0.05
		_GlowOuter ("Outer", Range(0, 1)) = 0.05
		_GlowPower ("Falloff", Range(1, 0)) = 0.75
		_WeightNormal ("Weight Normal", Float) = 0
		_WeightBold ("Weight Bold", Float) = 0.5
		_ShaderFlags ("Flags", Float) = 0
		_ScaleRatioA ("Scale RatioA", Float) = 1
		_ScaleRatioB ("Scale RatioB", Float) = 1
		_ScaleRatioC ("Scale RatioC", Float) = 1
		_MainTex ("Font Atlas", 2D) = "white" {}
		_TextureWidth ("Texture Width", Float) = 512
		_TextureHeight ("Texture Height", Float) = 512
		_GradientScale ("Gradient Scale", Float) = 5
		_ScaleX ("Scale X", Float) = 1
		_ScaleY ("Scale Y", Float) = 1
		_PerspectiveFilter ("Perspective Correction", Range(0, 1)) = 0.875
		_Sharpness ("Sharpness", Range(-1, 1)) = 0
		_VertexOffsetX ("Vertex OffsetX", Float) = 0
		_VertexOffsetY ("Vertex OffsetY", Float) = 0
		_CullMode ("Cull Mode", Float) = 0
	}
	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		LOD 300
		Pass
		{
			Name "FORWARD"
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ColorMask 14
			Cull [_CullMode]
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DIRECTIONAL
			#pragma multi_compile __ FOG_EXP
			#pragma multi_compile __ LIGHTPROBE_SH
			#pragma multi_compile __ VERTEXLIGHT_ON

			#if defined(SHADER_STAGE_VERTEX)
			#if defined(DIRECTIONAL) && defined(FOG_EXP) && defined(LIGHTPROBE_SH) && defined(VERTEXLIGHT_ON)
			float4 _GlobalsPad0[6];
			float _FaceDilate;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[6];
			float4x4 _EnvMatrix;
			float4 _GlobalsPad5[7];
			float _GlobalsPad6;
			float _WeightNormal;
			float _WeightBold;
			float _ScaleRatioA;
			float _GlobalsPad7;
			float _GlobalsPad8;
			float _VertexOffsetX;
			float _VertexOffsetY;
			float4 _GlobalsPad9[4];
			float _GradientScale;
			float _ScaleX;
			float _ScaleY;
			float _PerspectiveFilter;
			float _Sharpness;
			float _GlobalsPad10;
			float _GlobalsPad11;
			float _GlobalsPad12;
			float4 _MainTex_ST;
			float4 _FaceTex_ST;
			float4 _OutlineTex_ST;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
				float4 _ScreenParams : packoffset(c6);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 unity_4LightPosX0 : packoffset(c3);
				float4 unity_4LightPosY0 : packoffset(c4);
				float4 unity_4LightPosZ0 : packoffset(c5);
				float4 unity_4LightAtten0 : packoffset(c6);
				float4 unity_LightColor[8] : packoffset(c7);
				float4 unity_SHBr : packoffset(c42);
				float4 unity_SHBg : packoffset(c43);
				float4 unity_SHBb : packoffset(c44);
				float4 unity_SHC : packoffset(c45);
			}
			cbuffer UnityPerDraw : register(b3)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
				float4 unity_WorldTransformParams : packoffset(c9);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_TANGENT0 : TANGENT0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float2 o_TEXCOORD5 : TEXCOORD5, out float4 o_TEXCOORD2 : TEXCOORD2, out float4 o_TEXCOORD3 : TEXCOORD3, out float4 o_TEXCOORD4 : TEXCOORD4, out float4 o_COLOR0 : COLOR0, out float3 o_TEXCOORD6 : TEXCOORD6, out float o_TEXCOORD8 : TEXCOORD8, out float3 o_TEXCOORD7 : TEXCOORD7)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				r0.xy = asuint(float2(_VertexOffsetX, _VertexOffsetY) + i_POSITION0.xy);
				r1 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r1));
				r2 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r1));
				r1.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 * i_POSITION0.www + asfloat(r1.xyz));
				r3 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r2.yyyy));
				r3 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r2.xxxx) + asfloat(r3));
				r3 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r2.zzzz) + asfloat(r3));
				r3 = asuint(unity_MatrixVP._m03_m13_m23_m33 * asfloat(r2.wwww) + asfloat(r3));
				o_SV_POSITION0 = asfloat(r3);
				o_TEXCOORD8 = asfloat(r3.z);
				r0.w = asuint(0.000244140625 * i_TEXCOORD1.x);
				r3.x = asuint(floor(asfloat(r0.w)));
				r3.y = asuint(i_TEXCOORD1.x - asfloat(r3.x) * 4096.0);
				r3.xy = asuint(float2(0.001953125, 0.001953125) * asfloat(r3.xy));
				o_TEXCOORD0.zw = asfloat(r3.xy) * _FaceTex_ST.xy + _FaceTex_ST.zw;
				o_TEXCOORD1 = asfloat(r3.xy) * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
				o_TEXCOORD0.xy = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				b0.w = (0.0 >= i_TEXCOORD1.y);
				r0.w = b0.w ? 0x3f800000u : 0u;
				r1.w = asuint(_WeightBold + -_WeightNormal);
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.w) + _WeightNormal);
				r0.w = asuint(asfloat(r0.w) * 0.25 + _FaceDilate);
				r0.w = asuint(_ScaleRatioA * asfloat(r0.w));
				o_TEXCOORD5.x = 0.5 * asfloat(r0.w);
				r0.w = asuint(unity_MatrixVP._m31 * asfloat(r2.y));
				r0.w = asuint(unity_MatrixVP._m30 * asfloat(r2.x) + asfloat(r0.w));
				r0.w = asuint(unity_MatrixVP._m32 * asfloat(r2.z) + asfloat(r0.w));
				r0.w = asuint(unity_MatrixVP._m33 * asfloat(r2.w) + asfloat(r0.w));
				r2.xy = asuint(glstate_matrix_projection._m01_m11 * _ScreenParams.yy);
				r2.xy = asuint(glstate_matrix_projection._m00_m10 * _ScreenParams.xx + asfloat(r2.xy));
				r2.xy = asuint(float2(_ScaleX, _ScaleY) * asfloat(r2.xy));
				r2.xy = asuint(asfloat(r0.ww) / asfloat(r2.xy));
				r0.w = asuint(dot(asfloat(r2.xy), asfloat(r2.xy)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.w = asuint(_GradientScale * abs(i_TEXCOORD1.y));
				r2.x = asuint(1.0 + _Sharpness);
				r1.w = asuint(asfloat(r2.x) * asfloat(r1.w));
				r2.x = asuint(asfloat(r1.w) * asfloat(r0.w));
				r2.y = asuint(1.0 + -_PerspectiveFilter);
				r2.x = asuint(asfloat(r2.x) * asfloat(r2.y));
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.w) + -asfloat(r2.x));
				r2.yzw = asuint(unity_WorldToObject._m01_m11_m21 * _WorldSpaceCameraPos.yyy);
				r2.yzw = asuint(unity_WorldToObject._m00_m10_m20 * _WorldSpaceCameraPos.xxx + asfloat(r2.yzw));
				r2.yzw = asuint(unity_WorldToObject._m02_m12_m22 * _WorldSpaceCameraPos.zzz + asfloat(r2.yzw));
				r2.yzw = asuint(unity_WorldToObject._m03_m13_m23 + asfloat(r2.yzw));
				r0.z = asuint(i_POSITION0.z);
				r0.xyz = asuint(asfloat(r2.yzw) + -asfloat(r0.xyz));
				r0.x = asuint(dot(i_NORMAL0.xyz, asfloat(r0.xyz)));
				b0.y = (0.0 < asfloat(r0.x));
				b0.x = (asfloat(r0.x) < 0.0);
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.x = asuint(asint(r0.x) + -asint(r0.y));
				r0.x = asuint(float(asint(r0.x)));
				r0.xyz = asuint(i_NORMAL0.xyz * asfloat(r0.xxx));
				r3.x = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m00_m10_m20));
				r3.y = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m01_m11_m21));
				r3.z = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m02_m12_m22));
				r0.x = asuint(dot(asfloat(r3.xyz), asfloat(r3.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r3 = asuint(asfloat(r3.xyzz) * asfloat(r0.xxxx));
				r0.xyz = asuint(_WorldSpaceCameraPos.xyz + -asfloat(r1.xyz));
				r1.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r1.w = asuint(rsqrt(asfloat(r1.w)));
				r2.yzw = asuint(asfloat(r1.www) * asfloat(r0.xyz));
				r1.w = asuint(dot(asfloat(r3.xyw), asfloat(r2.yzw)));
				o_TEXCOORD5.y = abs(asfloat(r1.w)) * asfloat(r0.w) + asfloat(r2.x);
				r2.xyz = asuint(unity_ObjectToWorld._m11_m21_m01 * i_TANGENT0.yyy);
				r2.xyz = asuint(unity_ObjectToWorld._m10_m20_m00 * i_TANGENT0.xxx + asfloat(r2.xyz));
				r2.xyz = asuint(unity_ObjectToWorld._m12_m22_m02 * i_TANGENT0.zzz + asfloat(r2.xyz));
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.www));
				r4.xyz = asuint(asfloat(r3.wxy) * asfloat(r2.xyz));
				r4.xyz = asuint(asfloat(r3.ywx) * asfloat(r2.yzx) + -asfloat(r4.xyz));
				r0.w = asuint(unity_WorldTransformParams.w * i_TANGENT0.w);
				r4.xyz = asuint(asfloat(r4.xyz) * asfloat(r0.www));
				o_TEXCOORD2.y = asfloat(r4.x);
				o_TEXCOORD2.z = asfloat(r3.x);
				o_TEXCOORD2.x = asfloat(r2.z);
				o_TEXCOORD2.w = asfloat(r1.x);
				o_TEXCOORD3.x = asfloat(r2.x);
				o_TEXCOORD4.x = asfloat(r2.y);
				o_TEXCOORD3.z = asfloat(r3.y);
				o_TEXCOORD3.y = asfloat(r4.y);
				o_TEXCOORD4.y = asfloat(r4.z);
				o_TEXCOORD3.w = asfloat(r1.y);
				o_TEXCOORD4.z = asfloat(r3.w);
				o_TEXCOORD4.w = asfloat(r1.z);
				o_COLOR0 = i_COLOR0.xyzw;
				r2.xyz = asuint(_EnvMatrix._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(_EnvMatrix._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r2.xyz));
				o_TEXCOORD6 = _EnvMatrix._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw);
				r0 = asuint(unity_4LightPosY0.xyzw + -asfloat(r1.yyyy));
				r2 = asuint(asfloat(r0) * asfloat(r3.yyyy));
				r0 = asuint(asfloat(r0) * asfloat(r0));
				r4 = asuint(unity_4LightPosX0.xyzw + -asfloat(r1.xxxx));
				r1 = asuint(unity_4LightPosZ0.xyzw + -asfloat(r1.zzzz));
				r2 = asuint(asfloat(r4) * asfloat(r3.xxxx) + asfloat(r2));
				r0 = asuint(asfloat(r4) * asfloat(r4) + asfloat(r0));
				r0 = asuint(asfloat(r1) * asfloat(r1) + asfloat(r0));
				r1 = asuint(asfloat(r1) * asfloat(r3.wwzw) + asfloat(r2));
				r0 = asuint(max(float4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07), asfloat(r0)));
				r2 = asuint(rsqrt(asfloat(r0)));
				r0 = asuint(asfloat(r0) * unity_4LightAtten0.xyzw + float4(1.0, 1.0, 1.0, 1.0));
				r0 = asuint(float4(1.0, 1.0, 1.0, 1.0) / asfloat(r0));
				r1 = asuint(asfloat(r2) * asfloat(r1));
				r1 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r1)));
				r0 = asuint(asfloat(r1) * asfloat(r0));
				r1.xyz = asuint(unity_LightColor[1].xyz * asfloat(r0.yyy));
				r1.xyz = asuint(unity_LightColor[0].xyz * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_LightColor[2].xyz * asfloat(r0.zzz) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_LightColor[3].xyz * asfloat(r0.www) + asfloat(r0.xyz));
				r0.w = asuint(asfloat(r3.y) * asfloat(r3.y));
				r0.w = asuint(asfloat(r3.x) * asfloat(r3.x) + -asfloat(r0.w));
				r1 = asuint(asfloat(r3) * asfloat(r3.ywzx));
				r2.x = asuint(dot(unity_SHBr.xyzw, asfloat(r1)));
				r2.y = asuint(dot(unity_SHBg.xyzw, asfloat(r1)));
				r2.z = asuint(dot(unity_SHBb.xyzw, asfloat(r1)));
				r1.xyz = asuint(unity_SHC.xyz * asfloat(r0.www) + asfloat(r2.xyz));
				o_TEXCOORD7 = asfloat(r1.xyz) + asfloat(r0.xyz);
			}
			#elif defined(DIRECTIONAL) && defined(LIGHTPROBE_SH) && defined(VERTEXLIGHT_ON)
			float4 _GlobalsPad0[6];
			float _FaceDilate;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[6];
			float4x4 _EnvMatrix;
			float4 _GlobalsPad5[7];
			float _GlobalsPad6;
			float _WeightNormal;
			float _WeightBold;
			float _ScaleRatioA;
			float _GlobalsPad7;
			float _GlobalsPad8;
			float _VertexOffsetX;
			float _VertexOffsetY;
			float4 _GlobalsPad9[4];
			float _GradientScale;
			float _ScaleX;
			float _ScaleY;
			float _PerspectiveFilter;
			float _Sharpness;
			float _GlobalsPad10;
			float _GlobalsPad11;
			float _GlobalsPad12;
			float4 _MainTex_ST;
			float4 _FaceTex_ST;
			float4 _OutlineTex_ST;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
				float4 _ScreenParams : packoffset(c6);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 unity_4LightPosX0 : packoffset(c3);
				float4 unity_4LightPosY0 : packoffset(c4);
				float4 unity_4LightPosZ0 : packoffset(c5);
				float4 unity_4LightAtten0 : packoffset(c6);
				float4 unity_LightColor[8] : packoffset(c7);
				float4 unity_SHBr : packoffset(c42);
				float4 unity_SHBg : packoffset(c43);
				float4 unity_SHBb : packoffset(c44);
				float4 unity_SHC : packoffset(c45);
			}
			cbuffer UnityPerDraw : register(b3)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
				float4 unity_WorldTransformParams : packoffset(c9);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_TANGENT0 : TANGENT0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float2 o_TEXCOORD5 : TEXCOORD5, out float4 o_TEXCOORD2 : TEXCOORD2, out float4 o_TEXCOORD3 : TEXCOORD3, out float4 o_TEXCOORD4 : TEXCOORD4, out float4 o_COLOR0 : COLOR0, out float3 o_TEXCOORD6 : TEXCOORD6, out float3 o_TEXCOORD7 : TEXCOORD7)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				r0.xy = asuint(float2(_VertexOffsetX, _VertexOffsetY) + i_POSITION0.xy);
				r1 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r1));
				r2 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r1));
				r1.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 * i_POSITION0.www + asfloat(r1.xyz));
				r3 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r2.yyyy));
				r3 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r2.xxxx) + asfloat(r3));
				r3 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r2.zzzz) + asfloat(r3));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r2.wwww) + asfloat(r3);
				r0.w = asuint(0.000244140625 * i_TEXCOORD1.x);
				r3.x = asuint(floor(asfloat(r0.w)));
				r3.y = asuint(i_TEXCOORD1.x - asfloat(r3.x) * 4096.0);
				r3.xy = asuint(float2(0.001953125, 0.001953125) * asfloat(r3.xy));
				o_TEXCOORD0.zw = asfloat(r3.xy) * _FaceTex_ST.xy + _FaceTex_ST.zw;
				o_TEXCOORD1 = asfloat(r3.xy) * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
				o_TEXCOORD0.xy = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				b0.w = (0.0 >= i_TEXCOORD1.y);
				r0.w = b0.w ? 0x3f800000u : 0u;
				r1.w = asuint(_WeightBold + -_WeightNormal);
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.w) + _WeightNormal);
				r0.w = asuint(asfloat(r0.w) * 0.25 + _FaceDilate);
				r0.w = asuint(_ScaleRatioA * asfloat(r0.w));
				o_TEXCOORD5.x = 0.5 * asfloat(r0.w);
				r0.w = asuint(unity_MatrixVP._m31 * asfloat(r2.y));
				r0.w = asuint(unity_MatrixVP._m30 * asfloat(r2.x) + asfloat(r0.w));
				r0.w = asuint(unity_MatrixVP._m32 * asfloat(r2.z) + asfloat(r0.w));
				r0.w = asuint(unity_MatrixVP._m33 * asfloat(r2.w) + asfloat(r0.w));
				r2.xy = asuint(glstate_matrix_projection._m01_m11 * _ScreenParams.yy);
				r2.xy = asuint(glstate_matrix_projection._m00_m10 * _ScreenParams.xx + asfloat(r2.xy));
				r2.xy = asuint(float2(_ScaleX, _ScaleY) * asfloat(r2.xy));
				r2.xy = asuint(asfloat(r0.ww) / asfloat(r2.xy));
				r0.w = asuint(dot(asfloat(r2.xy), asfloat(r2.xy)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.w = asuint(_GradientScale * abs(i_TEXCOORD1.y));
				r2.x = asuint(1.0 + _Sharpness);
				r1.w = asuint(asfloat(r2.x) * asfloat(r1.w));
				r2.x = asuint(asfloat(r1.w) * asfloat(r0.w));
				r2.y = asuint(1.0 + -_PerspectiveFilter);
				r2.x = asuint(asfloat(r2.x) * asfloat(r2.y));
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.w) + -asfloat(r2.x));
				r2.yzw = asuint(unity_WorldToObject._m01_m11_m21 * _WorldSpaceCameraPos.yyy);
				r2.yzw = asuint(unity_WorldToObject._m00_m10_m20 * _WorldSpaceCameraPos.xxx + asfloat(r2.yzw));
				r2.yzw = asuint(unity_WorldToObject._m02_m12_m22 * _WorldSpaceCameraPos.zzz + asfloat(r2.yzw));
				r2.yzw = asuint(unity_WorldToObject._m03_m13_m23 + asfloat(r2.yzw));
				r0.z = asuint(i_POSITION0.z);
				r0.xyz = asuint(asfloat(r2.yzw) + -asfloat(r0.xyz));
				r0.x = asuint(dot(i_NORMAL0.xyz, asfloat(r0.xyz)));
				b0.y = (0.0 < asfloat(r0.x));
				b0.x = (asfloat(r0.x) < 0.0);
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.x = asuint(asint(r0.x) + -asint(r0.y));
				r0.x = asuint(float(asint(r0.x)));
				r0.xyz = asuint(i_NORMAL0.xyz * asfloat(r0.xxx));
				r3.x = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m00_m10_m20));
				r3.y = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m01_m11_m21));
				r3.z = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m02_m12_m22));
				r0.x = asuint(dot(asfloat(r3.xyz), asfloat(r3.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r3 = asuint(asfloat(r3.xyzz) * asfloat(r0.xxxx));
				r0.xyz = asuint(_WorldSpaceCameraPos.xyz + -asfloat(r1.xyz));
				r1.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r1.w = asuint(rsqrt(asfloat(r1.w)));
				r2.yzw = asuint(asfloat(r1.www) * asfloat(r0.xyz));
				r1.w = asuint(dot(asfloat(r3.xyw), asfloat(r2.yzw)));
				o_TEXCOORD5.y = abs(asfloat(r1.w)) * asfloat(r0.w) + asfloat(r2.x);
				r2.xyz = asuint(unity_ObjectToWorld._m11_m21_m01 * i_TANGENT0.yyy);
				r2.xyz = asuint(unity_ObjectToWorld._m10_m20_m00 * i_TANGENT0.xxx + asfloat(r2.xyz));
				r2.xyz = asuint(unity_ObjectToWorld._m12_m22_m02 * i_TANGENT0.zzz + asfloat(r2.xyz));
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.www));
				r4.xyz = asuint(asfloat(r3.wxy) * asfloat(r2.xyz));
				r4.xyz = asuint(asfloat(r3.ywx) * asfloat(r2.yzx) + -asfloat(r4.xyz));
				r0.w = asuint(unity_WorldTransformParams.w * i_TANGENT0.w);
				r4.xyz = asuint(asfloat(r4.xyz) * asfloat(r0.www));
				o_TEXCOORD2.y = asfloat(r4.x);
				o_TEXCOORD2.z = asfloat(r3.x);
				o_TEXCOORD2.x = asfloat(r2.z);
				o_TEXCOORD2.w = asfloat(r1.x);
				o_TEXCOORD3.x = asfloat(r2.x);
				o_TEXCOORD4.x = asfloat(r2.y);
				o_TEXCOORD3.z = asfloat(r3.y);
				o_TEXCOORD3.y = asfloat(r4.y);
				o_TEXCOORD4.y = asfloat(r4.z);
				o_TEXCOORD3.w = asfloat(r1.y);
				o_TEXCOORD4.z = asfloat(r3.w);
				o_TEXCOORD4.w = asfloat(r1.z);
				o_COLOR0 = i_COLOR0.xyzw;
				r2.xyz = asuint(_EnvMatrix._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(_EnvMatrix._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r2.xyz));
				o_TEXCOORD6 = _EnvMatrix._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw);
				r0 = asuint(unity_4LightPosY0.xyzw + -asfloat(r1.yyyy));
				r2 = asuint(asfloat(r0) * asfloat(r3.yyyy));
				r0 = asuint(asfloat(r0) * asfloat(r0));
				r4 = asuint(unity_4LightPosX0.xyzw + -asfloat(r1.xxxx));
				r1 = asuint(unity_4LightPosZ0.xyzw + -asfloat(r1.zzzz));
				r2 = asuint(asfloat(r4) * asfloat(r3.xxxx) + asfloat(r2));
				r0 = asuint(asfloat(r4) * asfloat(r4) + asfloat(r0));
				r0 = asuint(asfloat(r1) * asfloat(r1) + asfloat(r0));
				r1 = asuint(asfloat(r1) * asfloat(r3.wwzw) + asfloat(r2));
				r0 = asuint(max(float4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07), asfloat(r0)));
				r2 = asuint(rsqrt(asfloat(r0)));
				r0 = asuint(asfloat(r0) * unity_4LightAtten0.xyzw + float4(1.0, 1.0, 1.0, 1.0));
				r0 = asuint(float4(1.0, 1.0, 1.0, 1.0) / asfloat(r0));
				r1 = asuint(asfloat(r2) * asfloat(r1));
				r1 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r1)));
				r0 = asuint(asfloat(r1) * asfloat(r0));
				r1.xyz = asuint(unity_LightColor[1].xyz * asfloat(r0.yyy));
				r1.xyz = asuint(unity_LightColor[0].xyz * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_LightColor[2].xyz * asfloat(r0.zzz) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_LightColor[3].xyz * asfloat(r0.www) + asfloat(r0.xyz));
				r0.w = asuint(asfloat(r3.y) * asfloat(r3.y));
				r0.w = asuint(asfloat(r3.x) * asfloat(r3.x) + -asfloat(r0.w));
				r1 = asuint(asfloat(r3) * asfloat(r3.ywzx));
				r2.x = asuint(dot(unity_SHBr.xyzw, asfloat(r1)));
				r2.y = asuint(dot(unity_SHBg.xyzw, asfloat(r1)));
				r2.z = asuint(dot(unity_SHBb.xyzw, asfloat(r1)));
				r1.xyz = asuint(unity_SHC.xyz * asfloat(r0.www) + asfloat(r2.xyz));
				o_TEXCOORD7 = asfloat(r1.xyz) + asfloat(r0.xyz);
			}
			#elif (defined(DIRECTIONAL) && defined(FOG_EXP) && defined(LIGHTPROBE_SH)) || (defined(DIRECTIONAL) && defined(FOG_EXP))
			float4 _GlobalsPad0[6];
			float _FaceDilate;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[6];
			float4x4 _EnvMatrix;
			float4 _GlobalsPad5[7];
			float _GlobalsPad6;
			float _WeightNormal;
			float _WeightBold;
			float _ScaleRatioA;
			float _GlobalsPad7;
			float _GlobalsPad8;
			float _VertexOffsetX;
			float _VertexOffsetY;
			float4 _GlobalsPad9[4];
			float _GradientScale;
			float _ScaleX;
			float _ScaleY;
			float _PerspectiveFilter;
			float _Sharpness;
			float _GlobalsPad10;
			float _GlobalsPad11;
			float _GlobalsPad12;
			float4 _MainTex_ST;
			float4 _FaceTex_ST;
			float4 _OutlineTex_ST;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
				float4 _ScreenParams : packoffset(c6);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 unity_SHBr : packoffset(c42);
				float4 unity_SHBg : packoffset(c43);
				float4 unity_SHBb : packoffset(c44);
				float4 unity_SHC : packoffset(c45);
			}
			cbuffer UnityPerDraw : register(b3)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
				float4 unity_WorldTransformParams : packoffset(c9);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_TANGENT0 : TANGENT0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float2 o_TEXCOORD5 : TEXCOORD5, out float4 o_TEXCOORD2 : TEXCOORD2, out float4 o_TEXCOORD3 : TEXCOORD3, out float4 o_TEXCOORD4 : TEXCOORD4, out float4 o_COLOR0 : COLOR0, out float3 o_TEXCOORD6 : TEXCOORD6, out float o_TEXCOORD8 : TEXCOORD8, out float3 o_TEXCOORD7 : TEXCOORD7)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				r0.xy = asuint(float2(_VertexOffsetX, _VertexOffsetY) + i_POSITION0.xy);
				r1 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r1));
				r2 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r1));
				r1.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 * i_POSITION0.www + asfloat(r1.xyz));
				r3 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r2.yyyy));
				r3 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r2.xxxx) + asfloat(r3));
				r3 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r2.zzzz) + asfloat(r3));
				r3 = asuint(unity_MatrixVP._m03_m13_m23_m33 * asfloat(r2.wwww) + asfloat(r3));
				o_SV_POSITION0 = asfloat(r3);
				o_TEXCOORD8 = asfloat(r3.z);
				r0.w = asuint(0.000244140625 * i_TEXCOORD1.x);
				r3.x = asuint(floor(asfloat(r0.w)));
				r3.y = asuint(i_TEXCOORD1.x - asfloat(r3.x) * 4096.0);
				r3.xy = asuint(float2(0.001953125, 0.001953125) * asfloat(r3.xy));
				o_TEXCOORD0.zw = asfloat(r3.xy) * _FaceTex_ST.xy + _FaceTex_ST.zw;
				o_TEXCOORD1 = asfloat(r3.xy) * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
				o_TEXCOORD0.xy = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				b0.w = (0.0 >= i_TEXCOORD1.y);
				r0.w = b0.w ? 0x3f800000u : 0u;
				r1.w = asuint(_WeightBold + -_WeightNormal);
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.w) + _WeightNormal);
				r0.w = asuint(asfloat(r0.w) * 0.25 + _FaceDilate);
				r0.w = asuint(_ScaleRatioA * asfloat(r0.w));
				o_TEXCOORD5.x = 0.5 * asfloat(r0.w);
				r0.w = asuint(unity_MatrixVP._m31 * asfloat(r2.y));
				r0.w = asuint(unity_MatrixVP._m30 * asfloat(r2.x) + asfloat(r0.w));
				r0.w = asuint(unity_MatrixVP._m32 * asfloat(r2.z) + asfloat(r0.w));
				r0.w = asuint(unity_MatrixVP._m33 * asfloat(r2.w) + asfloat(r0.w));
				r2.xy = asuint(glstate_matrix_projection._m01_m11 * _ScreenParams.yy);
				r2.xy = asuint(glstate_matrix_projection._m00_m10 * _ScreenParams.xx + asfloat(r2.xy));
				r2.xy = asuint(float2(_ScaleX, _ScaleY) * asfloat(r2.xy));
				r2.xy = asuint(asfloat(r0.ww) / asfloat(r2.xy));
				r0.w = asuint(dot(asfloat(r2.xy), asfloat(r2.xy)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.w = asuint(_GradientScale * abs(i_TEXCOORD1.y));
				r2.x = asuint(1.0 + _Sharpness);
				r1.w = asuint(asfloat(r2.x) * asfloat(r1.w));
				r2.x = asuint(asfloat(r1.w) * asfloat(r0.w));
				r2.y = asuint(1.0 + -_PerspectiveFilter);
				r2.x = asuint(asfloat(r2.x) * asfloat(r2.y));
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.w) + -asfloat(r2.x));
				r2.yzw = asuint(unity_WorldToObject._m01_m11_m21 * _WorldSpaceCameraPos.yyy);
				r2.yzw = asuint(unity_WorldToObject._m00_m10_m20 * _WorldSpaceCameraPos.xxx + asfloat(r2.yzw));
				r2.yzw = asuint(unity_WorldToObject._m02_m12_m22 * _WorldSpaceCameraPos.zzz + asfloat(r2.yzw));
				r2.yzw = asuint(unity_WorldToObject._m03_m13_m23 + asfloat(r2.yzw));
				r0.z = asuint(i_POSITION0.z);
				r0.xyz = asuint(asfloat(r2.yzw) + -asfloat(r0.xyz));
				r0.x = asuint(dot(i_NORMAL0.xyz, asfloat(r0.xyz)));
				b0.y = (0.0 < asfloat(r0.x));
				b0.x = (asfloat(r0.x) < 0.0);
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.x = asuint(asint(r0.x) + -asint(r0.y));
				r0.x = asuint(float(asint(r0.x)));
				r0.xyz = asuint(i_NORMAL0.xyz * asfloat(r0.xxx));
				r3.x = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m00_m10_m20));
				r3.y = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m01_m11_m21));
				r3.z = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m02_m12_m22));
				r0.x = asuint(dot(asfloat(r3.xyz), asfloat(r3.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r3 = asuint(asfloat(r3.xyzz) * asfloat(r0.xxxx));
				r0.xyz = asuint(_WorldSpaceCameraPos.xyz + -asfloat(r1.xyz));
				r1.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r1.w = asuint(rsqrt(asfloat(r1.w)));
				r2.yzw = asuint(asfloat(r1.www) * asfloat(r0.xyz));
				r1.w = asuint(dot(asfloat(r3.xyw), asfloat(r2.yzw)));
				o_TEXCOORD5.y = abs(asfloat(r1.w)) * asfloat(r0.w) + asfloat(r2.x);
				o_TEXCOORD2.w = asfloat(r1.x);
				r2.xyz = asuint(unity_ObjectToWorld._m11_m21_m01 * i_TANGENT0.yyy);
				r2.xyz = asuint(unity_ObjectToWorld._m10_m20_m00 * i_TANGENT0.xxx + asfloat(r2.xyz));
				r2.xyz = asuint(unity_ObjectToWorld._m12_m22_m02 * i_TANGENT0.zzz + asfloat(r2.xyz));
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.www));
				r4.xyz = asuint(asfloat(r3.wxy) * asfloat(r2.xyz));
				r4.xyz = asuint(asfloat(r3.ywx) * asfloat(r2.yzx) + -asfloat(r4.xyz));
				r0.w = asuint(unity_WorldTransformParams.w * i_TANGENT0.w);
				r4.xyz = asuint(asfloat(r4.xyz) * asfloat(r0.www));
				o_TEXCOORD2.y = asfloat(r4.x);
				o_TEXCOORD2.z = asfloat(r3.x);
				o_TEXCOORD2.x = asfloat(r2.z);
				o_TEXCOORD3.x = asfloat(r2.x);
				o_TEXCOORD4.x = asfloat(r2.y);
				o_TEXCOORD3.w = asfloat(r1.y);
				o_TEXCOORD4.w = asfloat(r1.z);
				o_TEXCOORD3.z = asfloat(r3.y);
				o_TEXCOORD3.y = asfloat(r4.y);
				o_TEXCOORD4.y = asfloat(r4.z);
				o_TEXCOORD4.z = asfloat(r3.w);
				o_COLOR0 = i_COLOR0.xyzw;
				r1.xyz = asuint(_EnvMatrix._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(_EnvMatrix._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				o_TEXCOORD6 = _EnvMatrix._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw);
				r0.x = asuint(asfloat(r3.y) * asfloat(r3.y));
				r0.x = asuint(asfloat(r3.x) * asfloat(r3.x) + -asfloat(r0.x));
				r1 = asuint(asfloat(r3) * asfloat(r3.ywzx));
				r2.x = asuint(dot(unity_SHBr.xyzw, asfloat(r1)));
				r2.y = asuint(dot(unity_SHBg.xyzw, asfloat(r1)));
				r2.z = asuint(dot(unity_SHBb.xyzw, asfloat(r1)));
				o_TEXCOORD7 = unity_SHC.xyz * asfloat(r0.xxx) + asfloat(r2.xyz);
			}
			#elif (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH)) || defined(DIRECTIONAL)
			float4 _GlobalsPad0[6];
			float _FaceDilate;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[6];
			float4x4 _EnvMatrix;
			float4 _GlobalsPad5[7];
			float _GlobalsPad6;
			float _WeightNormal;
			float _WeightBold;
			float _ScaleRatioA;
			float _GlobalsPad7;
			float _GlobalsPad8;
			float _VertexOffsetX;
			float _VertexOffsetY;
			float4 _GlobalsPad9[4];
			float _GradientScale;
			float _ScaleX;
			float _ScaleY;
			float _PerspectiveFilter;
			float _Sharpness;
			float _GlobalsPad10;
			float _GlobalsPad11;
			float _GlobalsPad12;
			float4 _MainTex_ST;
			float4 _FaceTex_ST;
			float4 _OutlineTex_ST;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
				float4 _ScreenParams : packoffset(c6);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 unity_SHBr : packoffset(c42);
				float4 unity_SHBg : packoffset(c43);
				float4 unity_SHBb : packoffset(c44);
				float4 unity_SHC : packoffset(c45);
			}
			cbuffer UnityPerDraw : register(b3)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
				float4 unity_WorldTransformParams : packoffset(c9);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_TANGENT0 : TANGENT0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float2 o_TEXCOORD5 : TEXCOORD5, out float4 o_TEXCOORD2 : TEXCOORD2, out float4 o_TEXCOORD3 : TEXCOORD3, out float4 o_TEXCOORD4 : TEXCOORD4, out float4 o_COLOR0 : COLOR0, out float3 o_TEXCOORD6 : TEXCOORD6, out float3 o_TEXCOORD7 : TEXCOORD7)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				r0.xy = asuint(float2(_VertexOffsetX, _VertexOffsetY) + i_POSITION0.xy);
				r1 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r1));
				r2 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r1));
				r1.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 * i_POSITION0.www + asfloat(r1.xyz));
				r3 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r2.yyyy));
				r3 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r2.xxxx) + asfloat(r3));
				r3 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r2.zzzz) + asfloat(r3));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r2.wwww) + asfloat(r3);
				r0.w = asuint(0.000244140625 * i_TEXCOORD1.x);
				r3.x = asuint(floor(asfloat(r0.w)));
				r3.y = asuint(i_TEXCOORD1.x - asfloat(r3.x) * 4096.0);
				r3.xy = asuint(float2(0.001953125, 0.001953125) * asfloat(r3.xy));
				o_TEXCOORD0.zw = asfloat(r3.xy) * _FaceTex_ST.xy + _FaceTex_ST.zw;
				o_TEXCOORD1 = asfloat(r3.xy) * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
				o_TEXCOORD0.xy = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				b0.w = (0.0 >= i_TEXCOORD1.y);
				r0.w = b0.w ? 0x3f800000u : 0u;
				r1.w = asuint(_WeightBold + -_WeightNormal);
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.w) + _WeightNormal);
				r0.w = asuint(asfloat(r0.w) * 0.25 + _FaceDilate);
				r0.w = asuint(_ScaleRatioA * asfloat(r0.w));
				o_TEXCOORD5.x = 0.5 * asfloat(r0.w);
				r0.w = asuint(unity_MatrixVP._m31 * asfloat(r2.y));
				r0.w = asuint(unity_MatrixVP._m30 * asfloat(r2.x) + asfloat(r0.w));
				r0.w = asuint(unity_MatrixVP._m32 * asfloat(r2.z) + asfloat(r0.w));
				r0.w = asuint(unity_MatrixVP._m33 * asfloat(r2.w) + asfloat(r0.w));
				r2.xy = asuint(glstate_matrix_projection._m01_m11 * _ScreenParams.yy);
				r2.xy = asuint(glstate_matrix_projection._m00_m10 * _ScreenParams.xx + asfloat(r2.xy));
				r2.xy = asuint(float2(_ScaleX, _ScaleY) * asfloat(r2.xy));
				r2.xy = asuint(asfloat(r0.ww) / asfloat(r2.xy));
				r0.w = asuint(dot(asfloat(r2.xy), asfloat(r2.xy)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.w = asuint(_GradientScale * abs(i_TEXCOORD1.y));
				r2.x = asuint(1.0 + _Sharpness);
				r1.w = asuint(asfloat(r2.x) * asfloat(r1.w));
				r2.x = asuint(asfloat(r1.w) * asfloat(r0.w));
				r2.y = asuint(1.0 + -_PerspectiveFilter);
				r2.x = asuint(asfloat(r2.x) * asfloat(r2.y));
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.w) + -asfloat(r2.x));
				r2.yzw = asuint(unity_WorldToObject._m01_m11_m21 * _WorldSpaceCameraPos.yyy);
				r2.yzw = asuint(unity_WorldToObject._m00_m10_m20 * _WorldSpaceCameraPos.xxx + asfloat(r2.yzw));
				r2.yzw = asuint(unity_WorldToObject._m02_m12_m22 * _WorldSpaceCameraPos.zzz + asfloat(r2.yzw));
				r2.yzw = asuint(unity_WorldToObject._m03_m13_m23 + asfloat(r2.yzw));
				r0.z = asuint(i_POSITION0.z);
				r0.xyz = asuint(asfloat(r2.yzw) + -asfloat(r0.xyz));
				r0.x = asuint(dot(i_NORMAL0.xyz, asfloat(r0.xyz)));
				b0.y = (0.0 < asfloat(r0.x));
				b0.x = (asfloat(r0.x) < 0.0);
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.x = asuint(asint(r0.x) + -asint(r0.y));
				r0.x = asuint(float(asint(r0.x)));
				r0.xyz = asuint(i_NORMAL0.xyz * asfloat(r0.xxx));
				r3.x = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m00_m10_m20));
				r3.y = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m01_m11_m21));
				r3.z = asuint(dot(asfloat(r0.xyz), unity_WorldToObject._m02_m12_m22));
				r0.x = asuint(dot(asfloat(r3.xyz), asfloat(r3.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r3 = asuint(asfloat(r3.xyzz) * asfloat(r0.xxxx));
				r0.xyz = asuint(_WorldSpaceCameraPos.xyz + -asfloat(r1.xyz));
				r1.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r1.w = asuint(rsqrt(asfloat(r1.w)));
				r2.yzw = asuint(asfloat(r1.www) * asfloat(r0.xyz));
				r1.w = asuint(dot(asfloat(r3.xyw), asfloat(r2.yzw)));
				o_TEXCOORD5.y = abs(asfloat(r1.w)) * asfloat(r0.w) + asfloat(r2.x);
				o_TEXCOORD2.w = asfloat(r1.x);
				r2.xyz = asuint(unity_ObjectToWorld._m11_m21_m01 * i_TANGENT0.yyy);
				r2.xyz = asuint(unity_ObjectToWorld._m10_m20_m00 * i_TANGENT0.xxx + asfloat(r2.xyz));
				r2.xyz = asuint(unity_ObjectToWorld._m12_m22_m02 * i_TANGENT0.zzz + asfloat(r2.xyz));
				r0.w = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r0.www));
				r4.xyz = asuint(asfloat(r3.wxy) * asfloat(r2.xyz));
				r4.xyz = asuint(asfloat(r3.ywx) * asfloat(r2.yzx) + -asfloat(r4.xyz));
				r0.w = asuint(unity_WorldTransformParams.w * i_TANGENT0.w);
				r4.xyz = asuint(asfloat(r4.xyz) * asfloat(r0.www));
				o_TEXCOORD2.y = asfloat(r4.x);
				o_TEXCOORD2.z = asfloat(r3.x);
				o_TEXCOORD2.x = asfloat(r2.z);
				o_TEXCOORD3.x = asfloat(r2.x);
				o_TEXCOORD4.x = asfloat(r2.y);
				o_TEXCOORD3.w = asfloat(r1.y);
				o_TEXCOORD4.w = asfloat(r1.z);
				o_TEXCOORD3.z = asfloat(r3.y);
				o_TEXCOORD3.y = asfloat(r4.y);
				o_TEXCOORD4.y = asfloat(r4.z);
				o_TEXCOORD4.z = asfloat(r3.w);
				o_COLOR0 = i_COLOR0.xyzw;
				r1.xyz = asuint(_EnvMatrix._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(_EnvMatrix._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				o_TEXCOORD6 = _EnvMatrix._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw);
				r0.x = asuint(asfloat(r3.y) * asfloat(r3.y));
				r0.x = asuint(asfloat(r3.x) * asfloat(r3.x) + -asfloat(r0.x));
				r1 = asuint(asfloat(r3) * asfloat(r3.ywzx));
				r2.x = asuint(dot(unity_SHBr.xyzw, asfloat(r1)));
				r2.y = asuint(dot(unity_SHBg.xyzw, asfloat(r1)));
				r2.z = asuint(dot(unity_SHBb.xyzw, asfloat(r1)));
				o_TEXCOORD7 = unity_SHC.xyz * asfloat(r0.xxx) + asfloat(r2.xyz);
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if (defined(DIRECTIONAL) && defined(FOG_EXP) && defined(LIGHTPROBE_SH)) || (defined(DIRECTIONAL) && defined(FOG_EXP))
			float4 _GlobalsPad0[2];
			float4 _LightColor0;
			float4 _GlobalsPad1[1];
			float _FaceUVSpeedX;
			float _FaceUVSpeedY;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _FaceColor;
			float _GlobalsPad4;
			float _OutlineSoftness;
			float _OutlineUVSpeedX;
			float _OutlineUVSpeedY;
			float4 _OutlineColor;
			float _OutlineWidth;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			float4 _GlobalsPad8[15];
			float _GlobalsPad9;
			float _GlobalsPad10;
			float _GlobalsPad11;
			float _ScaleRatioA;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
				float4 _ProjectionParams : packoffset(c5);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
				float4 unity_SHAr : packoffset(c39);
				float4 unity_SHAg : packoffset(c40);
				float4 unity_SHAb : packoffset(c41);
				float4 unity_OcclusionMaskSelector : packoffset(c46);
			}
			cbuffer UnityFog : register(b3)
			{
				float4 unity_FogColor : packoffset(c0);
				float4 unity_FogParams : packoffset(c1);
			}
			cbuffer UnityProbeVolume : register(b4)
			{
				float4 unity_ProbeVolumeParams : packoffset(c0);
				float4x4 unity_ProbeVolumeWorldToObject : packoffset(c1);
				float3 unity_ProbeVolumeSizeInv : packoffset(c5);
				float3 unity_ProbeVolumeMin : packoffset(c6);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _FaceTex : register(t1);
			Texture2D<float4> _OutlineTex : register(t2);
			Texture3D<float4> unity_ProbeVolumeSH : register(t3);
			SamplerState samplerunity_ProbeVolumeSH : register(s0);
			SamplerState sampler_FaceTex : register(s1);
			SamplerState sampler_OutlineTex : register(s2);
			SamplerState sampler_MainTex : register(s3);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float2 i_TEXCOORD5 : TEXCOORD5, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float4 i_TEXCOORD4 : TEXCOORD4, float4 i_COLOR0 : COLOR0, float3 i_TEXCOORD6 : TEXCOORD6, float i_TEXCOORD8 : TEXCOORD8, float3 i_TEXCOORD7 : TEXCOORD7, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(0.5 + -asfloat(r0.w));
				r0.x = asuint(-i_TEXCOORD5.x + asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) * i_TEXCOORD5.y + 0.5);
				r0.y = asuint(_ScaleRatioA * _OutlineWidth);
				r0.z = asuint(_ScaleRatioA * _OutlineSoftness);
				r0.yw = asuint(i_TEXCOORD5.yy * asfloat(r0.yz));
				r1 = asuint(_FaceColor.xyzw * i_COLOR0.xyzw);
				r2.x = asuint(_OutlineColor.w * i_COLOR0.w);
				r2.yz = asuint(float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + i_TEXCOORD0.zw);
				r3 = asuint(_FaceTex.Sample(sampler_FaceTex, asfloat(r2.yz)));
				r1 = asuint(asfloat(r3) * asfloat(r1));
				r2.yz = asuint(float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + i_TEXCOORD1.xy);
				r3 = asuint(_OutlineTex.Sample(sampler_OutlineTex, asfloat(r2.yz)));
				r2.yzw = asuint(_OutlineColor.xyz * asfloat(r3.xyz));
				r3.w = asuint(asfloat(r3.w) * asfloat(r2.x));
				r2.x = asuint(asfloat(r0.x) - asfloat(r0.y) * 0.5);
				r0.w = asuint(asfloat(r0.w) * 0.5 + asfloat(r2.x));
				r0.z = asuint(asfloat(r0.z) * i_TEXCOORD5.y + 1.0);
				r0.z = asuint(saturate(asfloat(r0.w) / asfloat(r0.z)));
				r0.z = asuint(1.0 + -asfloat(r0.z));
				r0.x = asuint(saturate(asfloat(r0.y) * 0.5 + asfloat(r0.x)));
				r0.y = asuint(min(1.0, asfloat(r0.y)));
				r0.y = asuint(sqrt(asfloat(r0.y)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r1.www));
				r3.xyz = asuint(asfloat(r3.www) * asfloat(r2.yzw));
				r2 = asuint(asfloat(r3) + -asfloat(r1));
				r1 = asuint(asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1));
				r0 = asuint(asfloat(r1) * asfloat(r0.zzzz));
				r1.x = asuint(max(9.99999975e-05, asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) / asfloat(r1.xxx));
				b1.x = (1.0 == unity_ProbeVolumeParams.x);
				r1.x = b1.x ? 0xffffffffu : 0u;
				[branch] if (b1.x)
				{
					b1.y = (1.0 == unity_ProbeVolumeParams.y);
					r2.xyz = asuint(unity_ProbeVolumeWorldToObject._m01_m11_m21 * i_TEXCOORD3.www);
					r2.xyz = asuint(unity_ProbeVolumeWorldToObject._m00_m10_m20 * i_TEXCOORD2.www + asfloat(r2.xyz));
					r2.xyz = asuint(unity_ProbeVolumeWorldToObject._m02_m12_m22 * i_TEXCOORD4.www + asfloat(r2.xyz));
					r2.xyz = asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23 + asfloat(r2.xyz));
					r3.y = asuint(i_TEXCOORD2.w);
					r3.z = asuint(i_TEXCOORD3.w);
					r3.w = asuint(i_TEXCOORD4.w);
					r1.yzw = b1.yyy ? r2.xyz : r3.yzw;
					r1.yzw = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r1.yzw));
					r2.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r1.yzw));
					r1.y = asuint(asfloat(r2.y) * 0.25 + 0.75);
					r1.z = asuint(unity_ProbeVolumeParams.z * 0.5 + 0.75);
					r2.x = asuint(max(asfloat(r1.y), asfloat(r1.z)));
					r2 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r2.xzw)));
				}
				else
				{
					r2 = uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u);
				}
				r1.y = asuint(saturate(dot(asfloat(r2), unity_OcclusionMaskSelector.xyzw)));
				r2.x = asuint(i_TEXCOORD2.z);
				r2.y = asuint(i_TEXCOORD3.z);
				r2.z = asuint(i_TEXCOORD4.z);
				r1.z = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r1.z = asuint(rsqrt(asfloat(r1.z)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r1.zzz));
				r1.yzw = asuint(_LightColor0.xyz * asfloat(r1.yyy));
				[branch] if (r1.x != 0)
				{
					b1.x = (1.0 == unity_ProbeVolumeParams.y);
					r3.xyz = asuint(unity_ProbeVolumeWorldToObject._m01_m11_m21 * i_TEXCOORD3.www);
					r3.xyz = asuint(unity_ProbeVolumeWorldToObject._m00_m10_m20 * i_TEXCOORD2.www + asfloat(r3.xyz));
					r3.xyz = asuint(unity_ProbeVolumeWorldToObject._m02_m12_m22 * i_TEXCOORD4.www + asfloat(r3.xyz));
					r3.xyz = asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23 + asfloat(r3.xyz));
					r4.y = asuint(i_TEXCOORD2.w);
					r4.z = asuint(i_TEXCOORD3.w);
					r4.w = asuint(i_TEXCOORD4.w);
					r3.xyz = b1.xxx ? r3.xyz : r4.yzw;
					r3.xyz = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r3.xyz));
					r3.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r3.xyz));
					r1.x = asuint(0.25 * asfloat(r3.y));
					r3.y = asuint(0.5 * unity_ProbeVolumeParams.z);
					r4.x = asuint(0.25 - unity_ProbeVolumeParams.z * 0.5);
					r1.x = asuint(max(asfloat(r3.y), asfloat(r1.x)));
					r3.x = asuint(min(asfloat(r1.x), asfloat(r4.x)));
					r4 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r3.xzw)));
					r5.xyz = asuint(float3(0.25, 0.0, 0.0) + asfloat(r3.xzw));
					r5 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r5.xyz)));
					r3.xyz = asuint(float3(0.5, 0.0, 0.0) + asfloat(r3.xzw));
					r3 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r3.xyz)));
					r2.w = 0x3f800000u;
					r4.x = asuint(dot(asfloat(r4), asfloat(r2)));
					r4.y = asuint(dot(asfloat(r5), asfloat(r2)));
					r4.z = asuint(dot(asfloat(r3), asfloat(r2)));
				}
				else
				{
					r2.w = 0x3f800000u;
					r4.x = asuint(dot(unity_SHAr.xyzw, asfloat(r2)));
					r4.y = asuint(dot(unity_SHAg.xyzw, asfloat(r2)));
					r4.z = asuint(dot(unity_SHAb.xyzw, asfloat(r2)));
				}
				r3.xyz = asuint(i_TEXCOORD7.xyz + asfloat(r4.xyz));
				r3.xyz = asuint(max(float3(0.0, 0.0, 0.0), asfloat(r3.xyz)));
				r1.x = asuint(dot(asfloat(r2.xyz), _WorldSpaceLightPos0.xyz));
				r1.x = asuint(max(0.0, asfloat(r1.x)));
				r1.yzw = asuint(asfloat(r1.yzw) * asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r1.yzw) * asfloat(r1.xxx) + asfloat(r0.xyz));
				r1.x = asuint(i_TEXCOORD8 / _ProjectionParams.y);
				r1.x = asuint(1.0 + -asfloat(r1.x));
				r1.x = asuint(_ProjectionParams.z * asfloat(r1.x));
				r1.x = asuint(max(0.0, asfloat(r1.x)));
				r1.x = asuint(unity_FogParams.y * asfloat(r1.x));
				r1.x = asuint(exp2(-asfloat(r1.x)));
				r1.x = asuint(min(1.0, asfloat(r1.x)));
				r0.xyz = asuint(-unity_FogColor.xyz + asfloat(r0.xyz));
				o_SV_Target0.xyz = asfloat(r1.xxx) * asfloat(r0.xyz) + unity_FogColor.xyz;
				o_SV_Target0.w = asfloat(r0.w);
			}
			#elif (defined(DIRECTIONAL) && defined(LIGHTPROBE_SH)) || defined(DIRECTIONAL)
			float4 _GlobalsPad0[2];
			float4 _LightColor0;
			float4 _GlobalsPad1[1];
			float _FaceUVSpeedX;
			float _FaceUVSpeedY;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _FaceColor;
			float _GlobalsPad4;
			float _OutlineSoftness;
			float _OutlineUVSpeedX;
			float _OutlineUVSpeedY;
			float4 _OutlineColor;
			float _OutlineWidth;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			float4 _GlobalsPad8[15];
			float _GlobalsPad9;
			float _GlobalsPad10;
			float _GlobalsPad11;
			float _ScaleRatioA;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
				float4 unity_SHAr : packoffset(c39);
				float4 unity_SHAg : packoffset(c40);
				float4 unity_SHAb : packoffset(c41);
				float4 unity_OcclusionMaskSelector : packoffset(c46);
			}
			cbuffer UnityProbeVolume : register(b3)
			{
				float4 unity_ProbeVolumeParams : packoffset(c0);
				float4x4 unity_ProbeVolumeWorldToObject : packoffset(c1);
				float3 unity_ProbeVolumeSizeInv : packoffset(c5);
				float3 unity_ProbeVolumeMin : packoffset(c6);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _FaceTex : register(t1);
			Texture2D<float4> _OutlineTex : register(t2);
			Texture3D<float4> unity_ProbeVolumeSH : register(t3);
			SamplerState samplerunity_ProbeVolumeSH : register(s0);
			SamplerState sampler_FaceTex : register(s1);
			SamplerState sampler_OutlineTex : register(s2);
			SamplerState sampler_MainTex : register(s3);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float2 i_TEXCOORD5 : TEXCOORD5, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float4 i_TEXCOORD4 : TEXCOORD4, float4 i_COLOR0 : COLOR0, float3 i_TEXCOORD6 : TEXCOORD6, float3 i_TEXCOORD7 : TEXCOORD7, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(0.5 + -asfloat(r0.w));
				r0.x = asuint(-i_TEXCOORD5.x + asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) * i_TEXCOORD5.y + 0.5);
				r0.y = asuint(_ScaleRatioA * _OutlineWidth);
				r0.z = asuint(_ScaleRatioA * _OutlineSoftness);
				r0.yw = asuint(i_TEXCOORD5.yy * asfloat(r0.yz));
				r1 = asuint(_FaceColor.xyzw * i_COLOR0.xyzw);
				r2.x = asuint(_OutlineColor.w * i_COLOR0.w);
				r2.yz = asuint(float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + i_TEXCOORD0.zw);
				r3 = asuint(_FaceTex.Sample(sampler_FaceTex, asfloat(r2.yz)));
				r1 = asuint(asfloat(r3) * asfloat(r1));
				r2.yz = asuint(float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + i_TEXCOORD1.xy);
				r3 = asuint(_OutlineTex.Sample(sampler_OutlineTex, asfloat(r2.yz)));
				r2.yzw = asuint(_OutlineColor.xyz * asfloat(r3.xyz));
				r3.w = asuint(asfloat(r3.w) * asfloat(r2.x));
				r2.x = asuint(asfloat(r0.x) - asfloat(r0.y) * 0.5);
				r0.w = asuint(asfloat(r0.w) * 0.5 + asfloat(r2.x));
				r0.z = asuint(asfloat(r0.z) * i_TEXCOORD5.y + 1.0);
				r0.z = asuint(saturate(asfloat(r0.w) / asfloat(r0.z)));
				r0.z = asuint(1.0 + -asfloat(r0.z));
				r0.x = asuint(saturate(asfloat(r0.y) * 0.5 + asfloat(r0.x)));
				r0.y = asuint(min(1.0, asfloat(r0.y)));
				r0.y = asuint(sqrt(asfloat(r0.y)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r1.www));
				r3.xyz = asuint(asfloat(r3.www) * asfloat(r2.yzw));
				r2 = asuint(asfloat(r3) + -asfloat(r1));
				r1 = asuint(asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1));
				r0 = asuint(asfloat(r1) * asfloat(r0.zzzz));
				r1.x = asuint(max(9.99999975e-05, asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) / asfloat(r1.xxx));
				b1.x = (1.0 == unity_ProbeVolumeParams.x);
				r1.x = b1.x ? 0xffffffffu : 0u;
				[branch] if (b1.x)
				{
					b1.y = (1.0 == unity_ProbeVolumeParams.y);
					r2.xyz = asuint(unity_ProbeVolumeWorldToObject._m01_m11_m21 * i_TEXCOORD3.www);
					r2.xyz = asuint(unity_ProbeVolumeWorldToObject._m00_m10_m20 * i_TEXCOORD2.www + asfloat(r2.xyz));
					r2.xyz = asuint(unity_ProbeVolumeWorldToObject._m02_m12_m22 * i_TEXCOORD4.www + asfloat(r2.xyz));
					r2.xyz = asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23 + asfloat(r2.xyz));
					r3.y = asuint(i_TEXCOORD2.w);
					r3.z = asuint(i_TEXCOORD3.w);
					r3.w = asuint(i_TEXCOORD4.w);
					r1.yzw = b1.yyy ? r2.xyz : r3.yzw;
					r1.yzw = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r1.yzw));
					r2.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r1.yzw));
					r1.y = asuint(asfloat(r2.y) * 0.25 + 0.75);
					r1.z = asuint(unity_ProbeVolumeParams.z * 0.5 + 0.75);
					r2.x = asuint(max(asfloat(r1.y), asfloat(r1.z)));
					r2 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r2.xzw)));
				}
				else
				{
					r2 = uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u);
				}
				r1.y = asuint(saturate(dot(asfloat(r2), unity_OcclusionMaskSelector.xyzw)));
				r2.x = asuint(i_TEXCOORD2.z);
				r2.y = asuint(i_TEXCOORD3.z);
				r2.z = asuint(i_TEXCOORD4.z);
				r1.z = asuint(dot(asfloat(r2.xyz), asfloat(r2.xyz)));
				r1.z = asuint(rsqrt(asfloat(r1.z)));
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r1.zzz));
				r1.yzw = asuint(_LightColor0.xyz * asfloat(r1.yyy));
				[branch] if (r1.x != 0)
				{
					b1.x = (1.0 == unity_ProbeVolumeParams.y);
					r3.xyz = asuint(unity_ProbeVolumeWorldToObject._m01_m11_m21 * i_TEXCOORD3.www);
					r3.xyz = asuint(unity_ProbeVolumeWorldToObject._m00_m10_m20 * i_TEXCOORD2.www + asfloat(r3.xyz));
					r3.xyz = asuint(unity_ProbeVolumeWorldToObject._m02_m12_m22 * i_TEXCOORD4.www + asfloat(r3.xyz));
					r3.xyz = asuint(unity_ProbeVolumeWorldToObject._m03_m13_m23 + asfloat(r3.xyz));
					r4.y = asuint(i_TEXCOORD2.w);
					r4.z = asuint(i_TEXCOORD3.w);
					r4.w = asuint(i_TEXCOORD4.w);
					r3.xyz = b1.xxx ? r3.xyz : r4.yzw;
					r3.xyz = asuint(-unity_ProbeVolumeMin.xyz + asfloat(r3.xyz));
					r3.yzw = asuint(unity_ProbeVolumeSizeInv.xyz * asfloat(r3.xyz));
					r1.x = asuint(0.25 * asfloat(r3.y));
					r3.y = asuint(0.5 * unity_ProbeVolumeParams.z);
					r4.x = asuint(0.25 - unity_ProbeVolumeParams.z * 0.5);
					r1.x = asuint(max(asfloat(r3.y), asfloat(r1.x)));
					r3.x = asuint(min(asfloat(r1.x), asfloat(r4.x)));
					r4 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r3.xzw)));
					r5.xyz = asuint(float3(0.25, 0.0, 0.0) + asfloat(r3.xzw));
					r5 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r5.xyz)));
					r3.xyz = asuint(float3(0.5, 0.0, 0.0) + asfloat(r3.xzw));
					r3 = asuint(unity_ProbeVolumeSH.Sample(samplerunity_ProbeVolumeSH, asfloat(r3.xyz)));
					r2.w = 0x3f800000u;
					r4.x = asuint(dot(asfloat(r4), asfloat(r2)));
					r4.y = asuint(dot(asfloat(r5), asfloat(r2)));
					r4.z = asuint(dot(asfloat(r3), asfloat(r2)));
				}
				else
				{
					r2.w = 0x3f800000u;
					r4.x = asuint(dot(unity_SHAr.xyzw, asfloat(r2)));
					r4.y = asuint(dot(unity_SHAg.xyzw, asfloat(r2)));
					r4.z = asuint(dot(unity_SHAb.xyzw, asfloat(r2)));
				}
				r3.xyz = asuint(i_TEXCOORD7.xyz + asfloat(r4.xyz));
				r3.xyz = asuint(max(float3(0.0, 0.0, 0.0), asfloat(r3.xyz)));
				r1.x = asuint(dot(asfloat(r2.xyz), _WorldSpaceLightPos0.xyz));
				r1.x = asuint(max(0.0, asfloat(r1.x)));
				r1.yzw = asuint(asfloat(r1.yzw) * asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r3.xyz) * asfloat(r0.xyz));
				o_SV_Target0.xyz = asfloat(r1.yzw) * asfloat(r1.xxx) + asfloat(r0.xyz);
				o_SV_Target0.w = asfloat(r0.w);
			}
			#endif
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "Caster"
			Tags { "LIGHTMODE" = "SHADOWCASTER" "SHADOWSUPPORT" = "true" }
			ColorMask 14
			Cull Off
			ZTest LEqual
			ZWrite On
			Offset 1, 1
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ SHADOWS_CUBE
			#pragma multi_compile __ SHADOWS_DEPTH

			#if defined(SHADER_STAGE_VERTEX)
			#if defined(SHADOWS_DEPTH)
			float4 _GlobalsPad0[2];
			float4 _MainTex_ST;
			float4 _OutlineTex_ST;
			float _OutlineWidth;
			float _FaceDilate;
			float _ScaleRatioA;
			cbuffer UnityShadows : register(b1)
			{
				float4 unity_LightShadowBias : packoffset(c5);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD1 : TEXCOORD1, out float2 o_TEXCOORD3 : TEXCOORD3, out float o_TEXCOORD2 : TEXCOORD2)
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
				r1.x = asuint(unity_LightShadowBias.x / asfloat(r0.w));
				r1.x = asuint(min(0.0, asfloat(r1.x)));
				r1.x = asuint(max(-1.0, asfloat(r1.x)));
				r0.z = asuint(asfloat(r1.x) + asfloat(r0.z));
				r1.x = asuint(min(asfloat(r0.z), asfloat(r0.w)));
				o_SV_POSITION0.xyw = asfloat(r0.xyw);
				r0.x = asuint(asfloat(r1.x) + -asfloat(r0.z));
				o_SV_POSITION0.z = unity_LightShadowBias.y * asfloat(r0.x) + asfloat(r0.z);
				o_TEXCOORD1 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o_TEXCOORD3 = i_TEXCOORD0.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
				r0.x = asuint(1.0 - _OutlineWidth * _ScaleRatioA);
				r0.x = asuint(asfloat(r0.x) - _FaceDilate * _ScaleRatioA);
				o_TEXCOORD2 = 0.5 * asfloat(r0.x);
			}
			#elif defined(SHADOWS_CUBE)
			float4 _GlobalsPad0[2];
			float4 _MainTex_ST;
			float4 _OutlineTex_ST;
			float _OutlineWidth;
			float _FaceDilate;
			float _ScaleRatioA;
			cbuffer UnityShadows : register(b1)
			{
				float4 unity_LightShadowBias : packoffset(c5);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD1 : TEXCOORD1, out float2 o_TEXCOORD3 : TEXCOORD3, out float o_TEXCOORD2 : TEXCOORD2)
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
				r1.x = asuint(min(asfloat(r0.z), asfloat(r0.w)));
				r1.x = asuint(asfloat(r1.x) + -asfloat(r0.z));
				o_SV_POSITION0.z = unity_LightShadowBias.y * asfloat(r1.x) + asfloat(r0.z);
				o_SV_POSITION0.xyw = asfloat(r0.xyw);
				o_TEXCOORD1 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o_TEXCOORD3 = i_TEXCOORD0.xy * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
				r0.x = asuint(1.0 - _OutlineWidth * _ScaleRatioA);
				r0.x = asuint(asfloat(r0.x) - _FaceDilate * _ScaleRatioA);
				o_TEXCOORD2 = 0.5 * asfloat(r0.x);
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD1 : TEXCOORD1, float2 i_TEXCOORD3 : TEXCOORD3, float i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r0.x = asuint(-i_TEXCOORD2 + asfloat(r0.w));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#endif
			ENDHLSL
		}
	}
	CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
