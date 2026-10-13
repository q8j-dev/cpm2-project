Shader "TextMeshPro/Distance Field Overlay"
{
	Properties
	{
		_FaceTex ("Face Texture", 2D) = "white" {}
		_FaceUVSpeedX ("Face UV Speed X", Range(-5, 5)) = 0
		_FaceUVSpeedY ("Face UV Speed Y", Range(-5, 5)) = 0
		[HDR] _FaceColor ("Face Color", Color) = (1,1,1,1)
		_FaceDilate ("Face Dilate", Range(-1, 1)) = 0
		[HDR] _OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineTex ("Outline Texture", 2D) = "white" {}
		_OutlineUVSpeedX ("Outline UV Speed X", Range(-5, 5)) = 0
		_OutlineUVSpeedY ("Outline UV Speed Y", Range(-5, 5)) = 0
		_OutlineWidth ("Outline Thickness", Range(0, 1)) = 0
		_OutlineSoftness ("Outline Softness", Range(0, 1)) = 0
		_Bevel ("Bevel", Range(0, 1)) = 0.5
		_BevelOffset ("Bevel Offset", Range(-0.5, 0.5)) = 0
		_BevelWidth ("Bevel Width", Range(-0.5, 0.5)) = 0
		_BevelClamp ("Bevel Clamp", Range(0, 1)) = 0
		_BevelRoundness ("Bevel Roundness", Range(0, 1)) = 0
		_LightAngle ("Light Angle", Range(0, 6.2831855)) = 3.1416
		[HDR] _SpecularColor ("Specular", Color) = (1,1,1,1)
		_SpecularPower ("Specular", Range(0, 4)) = 2
		_Reflectivity ("Reflectivity", Range(5, 15)) = 10
		_Diffuse ("Diffuse", Range(0, 1)) = 0.5
		_Ambient ("Ambient", Range(1, 0)) = 0.5
		_BumpMap ("Normal map", 2D) = "bump" {}
		_BumpOutline ("Bump Outline", Range(0, 1)) = 0
		_BumpFace ("Bump Face", Range(0, 1)) = 0
		_ReflectFaceColor ("Reflection Color", Color) = (0,0,0,1)
		_ReflectOutlineColor ("Reflection Color", Color) = (0,0,0,1)
		_Cube ("Reflection Cubemap", Cube) = "black" {}
		_EnvMatrixRotation ("Texture Rotation", Vector) = (0,0,0,0)
		[HDR] _UnderlayColor ("Border Color", Color) = (0,0,0,0.5)
		_UnderlayOffsetX ("Border OffsetX", Range(-1, 1)) = 0
		_UnderlayOffsetY ("Border OffsetY", Range(-1, 1)) = 0
		_UnderlayDilate ("Border Dilate", Range(-1, 1)) = 0
		_UnderlaySoftness ("Border Softness", Range(0, 1)) = 0
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
		_MaskCoord ("Mask Coordinates", Vector) = (0,0,32767,32767)
		_ClipRect ("Clip Rect", Vector) = (-32767,-32767,32767,32767)
		_MaskSoftnessX ("Mask SoftnessX", Float) = 0
		_MaskSoftnessY ("Mask SoftnessY", Float) = 0
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_CullMode ("Cull Mode", Float) = 0
		_ColorMask ("Color Mask", Float) = 15
	}
	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Overlay" "RenderType" = "Transparent" }
		Pass
		{
			Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
			ColorMask [_ColorMask]
			Cull [_CullMode]
			ZTest Always
			ZWrite Off
			Stencil { Ref [_Stencil] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] Comp [_StencilComp] Pass [_StencilOp] Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ UNITY_UI_CLIP_RECT

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[4];
			float _FaceDilate;
			float _OutlineSoftness;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[1];
			float _OutlineWidth;
			float _GlobalsPad4;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _GlobalsPad7[4];
			float4x4 _EnvMatrix;
			float4 _GlobalsPad8[7];
			float _GlobalsPad9;
			float _WeightNormal;
			float _WeightBold;
			float _ScaleRatioA;
			float _GlobalsPad10;
			float _GlobalsPad11;
			float _VertexOffsetX;
			float _VertexOffsetY;
			float4 _GlobalsPad12[2];
			float4 _ClipRect;
			float _MaskSoftnessX;
			float _MaskSoftnessY;
			float _GlobalsPad13;
			float _GlobalsPad14;
			float _GradientScale;
			float _ScaleX;
			float _ScaleY;
			float _PerspectiveFilter;
			float _Sharpness;
			float _GlobalsPad15;
			float _GlobalsPad16;
			float _GlobalsPad17;
			float4 _FaceTex_ST;
			float4 _OutlineTex_ST;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
				float4 _ScreenParams : packoffset(c6);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD1 : TEXCOORD1, out float4 o_TEXCOORD2 : TEXCOORD2, out float3 o_TEXCOORD3 : TEXCOORD3, out float4 o_TEXCOORD5 : TEXCOORD5)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				r0.xy = asuint(float2(_VertexOffsetX, _VertexOffsetY) + i_POSITION0.xy);
				r1 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r1));
				r2 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r1));
				r1.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 * i_POSITION0.www + asfloat(r1.xyz));
				r1.xyz = asuint(_WorldSpaceCameraPos.xyz + -asfloat(r1.xyz));
				r3 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r2.yyyy));
				r3 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r2.xxxx) + asfloat(r3));
				r3 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r2.zzzz) + asfloat(r3));
				r2 = asuint(unity_MatrixVP._m03_m13_m23_m33 * asfloat(r2.wwww) + asfloat(r3));
				o_SV_POSITION0 = asfloat(r2);
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD0 = i_TEXCOORD0.xy;
				r0.zw = asuint(glstate_matrix_projection._m01_m11 * _ScreenParams.yy);
				r0.zw = asuint(glstate_matrix_projection._m00_m10 * _ScreenParams.xx + asfloat(r0.zw));
				r0.zw = asuint(float2(_ScaleX, _ScaleY) * abs(asfloat(r0.zw)));
				r0.zw = asuint(asfloat(r2.ww) / asfloat(r0.zw));
				r1.w = asuint(dot(asfloat(r0.zw), asfloat(r0.zw)));
				r0.zw = asuint(float2(_MaskSoftnessX, _MaskSoftnessY) * float2(0.25, 0.25) + asfloat(r0.zw));
				o_TEXCOORD2.zw = float2(0.25, 0.25) / asfloat(r0.zw);
				r0.z = asuint(rsqrt(asfloat(r1.w)));
				r0.w = asuint(_GradientScale * abs(i_TEXCOORD1.y));
				r1.w = asuint(1.0 + _Sharpness);
				r0.w = asuint(asfloat(r1.w) * asfloat(r0.w));
				r1.w = asuint(asfloat(r0.z) * asfloat(r0.w));
				r2.x = asuint(1.0 + -_PerspectiveFilter);
				r2.x = asuint(asfloat(r2.x) * abs(asfloat(r1.w)));
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.w) + -asfloat(r2.x));
				r3.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r3.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r3.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r3.xyz), asfloat(r3.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r2.yzw = asuint(asfloat(r3.xyz) * asfloat(r0.www));
				r0.w = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r3.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www));
				r0.w = asuint(dot(asfloat(r2.yzw), asfloat(r3.xyz)));
				r0.z = asuint(abs(asfloat(r0.w)) * asfloat(r0.z) + asfloat(r2.x));
				b0.w = (0.0 == glstate_matrix_projection._m33);
				r2.y = b0.w ? r0.z : r1.w;
				b0.z = (0.0 >= i_TEXCOORD1.y);
				r0.z = b0.z ? 0x3f800000u : 0u;
				r0.w = asuint(_WeightBold + -_WeightNormal);
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.w) + _WeightNormal);
				r0.z = asuint(asfloat(r0.z) * 0.25 + _FaceDilate);
				r0.z = asuint(_ScaleRatioA * asfloat(r0.z));
				r2.w = asuint(0.5 * asfloat(r0.z));
				o_TEXCOORD1.yw = asfloat(r2.yw);
				r0.w = asuint(0.5 / asfloat(r2.y));
				r1.w = asuint(1.0 - _OutlineWidth * _ScaleRatioA);
				r1.w = asuint(asfloat(r1.w) - _OutlineSoftness * _ScaleRatioA);
				r1.w = asuint(asfloat(r1.w) * 0.5 + -asfloat(r0.w));
				o_TEXCOORD1.x = asfloat(r1.w) - asfloat(r0.z) * 0.5;
				r0.z = asuint(0.5 - asfloat(r0.z) * 0.5);
				o_TEXCOORD1.z = asfloat(r0.z) + asfloat(r0.w);
				r2 = asuint(max(float4(-2.0e+10, -2.0e+10, -2.0e+10, -2.0e+10), _ClipRect.xyzw));
				r2 = asuint(min(float4(2.0e+10, 2.0e+10, 2.0e+10, 2.0e+10), asfloat(r2)));
				r0.xy = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + -asfloat(r2.xy));
				o_TEXCOORD2.xy = asfloat(r0.xy) + -asfloat(r2.zw);
				r0.xyz = asuint(_EnvMatrix._m01_m11_m21 * asfloat(r1.yyy));
				r0.xyz = asuint(_EnvMatrix._m00_m10_m20 * asfloat(r1.xxx) + asfloat(r0.xyz));
				o_TEXCOORD3 = _EnvMatrix._m02_m12_m22 * asfloat(r1.zzz) + asfloat(r0.xyz);
				r0.x = asuint(0.000244140625 * i_TEXCOORD1.x);
				r0.z = asuint(floor(asfloat(r0.x)));
				r0.w = asuint(i_TEXCOORD1.x - asfloat(r0.z) * 4096.0);
				r0.xy = asuint(float2(0.001953125, 0.001953125) * asfloat(r0.zw));
				o_TEXCOORD5.xy = asfloat(r0.xy) * _FaceTex_ST.xy + _FaceTex_ST.zw;
				o_TEXCOORD5.zw = asfloat(r0.xy) * _OutlineTex_ST.xy + _OutlineTex_ST.zw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[2];
			float _FaceUVSpeedX;
			float _FaceUVSpeedY;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _FaceColor;
			float _GlobalsPad3;
			float _OutlineSoftness;
			float _OutlineUVSpeedX;
			float _OutlineUVSpeedY;
			float4 _OutlineColor;
			float _OutlineWidth;
			float _GlobalsPad4;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _GlobalsPad7[15];
			float _GlobalsPad8;
			float _GlobalsPad9;
			float _GlobalsPad10;
			float _ScaleRatioA;
			float4 _GlobalsPad11[3];
			float4 _ClipRect;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _FaceTex : register(t1);
			Texture2D<float4> _OutlineTex : register(t2);
			SamplerState sampler_FaceTex : register(s0);
			SamplerState sampler_OutlineTex : register(s1);
			SamplerState sampler_MainTex : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float4 i_TEXCOORD5 : TEXCOORD5, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(-i_TEXCOORD1.x + asfloat(r0.w));
				r0.y = asuint(i_TEXCOORD1.z + -asfloat(r0.w));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				r0.x = asuint(_ScaleRatioA * _OutlineWidth);
				r0.x = asuint(i_TEXCOORD1.y * asfloat(r0.x));
				r0.z = asuint(min(1.0, asfloat(r0.x)));
				r0.x = asuint(0.5 * asfloat(r0.x));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.w = asuint(saturate(asfloat(r0.y) * i_TEXCOORD1.y + asfloat(r0.x)));
				r0.x = asuint(asfloat(r0.y) * i_TEXCOORD1.y + -asfloat(r0.x));
				r0.y = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.zw = asuint(float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + i_TEXCOORD5.zw);
				r1 = asuint(_OutlineTex.Sample(sampler_OutlineTex, asfloat(r0.zw)));
				r1 = asuint(_OutlineColor.xyzw * asfloat(r1));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r1.www));
				r2.xyz = asuint(_FaceColor.xyz * i_COLOR0.xyz);
				r0.zw = asuint(float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + i_TEXCOORD5.xy);
				r3 = asuint(_FaceTex.Sample(sampler_FaceTex, asfloat(r0.zw)));
				r2.xyz = asuint(asfloat(r3.xyz) * asfloat(r2.xyz));
				r3.w = asuint(_FaceColor.w * asfloat(r3.w));
				r3.xyz = asuint(asfloat(r3.www) * asfloat(r2.xyz));
				r1 = asuint(-asfloat(r3) + asfloat(r1));
				r1 = asuint(asfloat(r0.yyyy) * asfloat(r1) + asfloat(r3));
				r0.y = asuint(_ScaleRatioA * _OutlineSoftness);
				r0.z = asuint(i_TEXCOORD1.y * asfloat(r0.y));
				r0.y = asuint(asfloat(r0.y) * i_TEXCOORD1.y + 1.0);
				r0.x = asuint(asfloat(r0.z) * 0.5 + asfloat(r0.x));
				r0.x = asuint(saturate(asfloat(r0.x) / asfloat(r0.y)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0 = asuint(asfloat(r1) * asfloat(r0.xxxx));
				r1.xy = asuint(_ClipRect.zw + -_ClipRect.xy);
				r1.xy = asuint(-abs(i_TEXCOORD2.xy) + asfloat(r1.xy));
				r1.xy = asuint(saturate(i_TEXCOORD2.zw * asfloat(r1.xy)));
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y));
				r1.y = asuint(asfloat(r0.w) * asfloat(r1.x) + -0.00100000005);
				r0 = asuint(asfloat(r1.xxxx) * asfloat(r0));
				o_SV_Target0 = i_COLOR0.wwww * asfloat(r0);
				b0.x = (asfloat(r1.y) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(UNITY_UI_ALPHACLIP)
			float4 _GlobalsPad0[2];
			float _FaceUVSpeedX;
			float _FaceUVSpeedY;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _FaceColor;
			float _GlobalsPad3;
			float _OutlineSoftness;
			float _OutlineUVSpeedX;
			float _OutlineUVSpeedY;
			float4 _OutlineColor;
			float _OutlineWidth;
			float _GlobalsPad4;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _GlobalsPad7[15];
			float _GlobalsPad8;
			float _GlobalsPad9;
			float _GlobalsPad10;
			float _ScaleRatioA;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _FaceTex : register(t1);
			Texture2D<float4> _OutlineTex : register(t2);
			SamplerState sampler_FaceTex : register(s0);
			SamplerState sampler_OutlineTex : register(s1);
			SamplerState sampler_MainTex : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float4 i_TEXCOORD5 : TEXCOORD5, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(-i_TEXCOORD1.x + asfloat(r0.w));
				r0.y = asuint(i_TEXCOORD1.z + -asfloat(r0.w));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				r0.x = asuint(_ScaleRatioA * _OutlineWidth);
				r0.x = asuint(i_TEXCOORD1.y * asfloat(r0.x));
				r0.z = asuint(min(1.0, asfloat(r0.x)));
				r0.x = asuint(0.5 * asfloat(r0.x));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.w = asuint(saturate(asfloat(r0.y) * i_TEXCOORD1.y + asfloat(r0.x)));
				r0.x = asuint(asfloat(r0.y) * i_TEXCOORD1.y + -asfloat(r0.x));
				r0.y = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.zw = asuint(float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + i_TEXCOORD5.zw);
				r1 = asuint(_OutlineTex.Sample(sampler_OutlineTex, asfloat(r0.zw)));
				r1 = asuint(_OutlineColor.xyzw * asfloat(r1));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r1.www));
				r2.xyz = asuint(_FaceColor.xyz * i_COLOR0.xyz);
				r0.zw = asuint(float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + i_TEXCOORD5.xy);
				r3 = asuint(_FaceTex.Sample(sampler_FaceTex, asfloat(r0.zw)));
				r2.xyz = asuint(asfloat(r3.xyz) * asfloat(r2.xyz));
				r3.w = asuint(_FaceColor.w * asfloat(r3.w));
				r3.xyz = asuint(asfloat(r3.www) * asfloat(r2.xyz));
				r1 = asuint(-asfloat(r3) + asfloat(r1));
				r1 = asuint(asfloat(r0.yyyy) * asfloat(r1) + asfloat(r3));
				r0.y = asuint(_ScaleRatioA * _OutlineSoftness);
				r0.z = asuint(i_TEXCOORD1.y * asfloat(r0.y));
				r0.y = asuint(asfloat(r0.y) * i_TEXCOORD1.y + 1.0);
				r0.x = asuint(asfloat(r0.z) * 0.5 + asfloat(r0.x));
				r0.x = asuint(saturate(asfloat(r0.x) / asfloat(r0.y)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0.y = asuint(asfloat(r1.w) * asfloat(r0.x) + -0.00100000005);
				r1 = asuint(asfloat(r1) * asfloat(r0.xxxx));
				o_SV_Target0 = i_COLOR0.wwww * asfloat(r1);
				b0.x = (asfloat(r0.y) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[2];
			float _FaceUVSpeedX;
			float _FaceUVSpeedY;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _FaceColor;
			float _GlobalsPad3;
			float _OutlineSoftness;
			float _OutlineUVSpeedX;
			float _OutlineUVSpeedY;
			float4 _OutlineColor;
			float _OutlineWidth;
			float _GlobalsPad4;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _GlobalsPad7[15];
			float _GlobalsPad8;
			float _GlobalsPad9;
			float _GlobalsPad10;
			float _ScaleRatioA;
			float4 _GlobalsPad11[3];
			float4 _ClipRect;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _FaceTex : register(t1);
			Texture2D<float4> _OutlineTex : register(t2);
			SamplerState sampler_FaceTex : register(s0);
			SamplerState sampler_OutlineTex : register(s1);
			SamplerState sampler_MainTex : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float4 i_TEXCOORD5 : TEXCOORD5, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(-i_TEXCOORD1.x + asfloat(r0.w));
				r0.y = asuint(i_TEXCOORD1.z + -asfloat(r0.w));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				r0.x = asuint(_ScaleRatioA * _OutlineWidth);
				r0.x = asuint(i_TEXCOORD1.y * asfloat(r0.x));
				r0.z = asuint(min(1.0, asfloat(r0.x)));
				r0.x = asuint(0.5 * asfloat(r0.x));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.w = asuint(saturate(asfloat(r0.y) * i_TEXCOORD1.y + asfloat(r0.x)));
				r0.x = asuint(asfloat(r0.y) * i_TEXCOORD1.y + -asfloat(r0.x));
				r0.y = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.zw = asuint(float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + i_TEXCOORD5.zw);
				r1 = asuint(_OutlineTex.Sample(sampler_OutlineTex, asfloat(r0.zw)));
				r1 = asuint(_OutlineColor.xyzw * asfloat(r1));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r1.www));
				r2.xyz = asuint(_FaceColor.xyz * i_COLOR0.xyz);
				r0.zw = asuint(float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + i_TEXCOORD5.xy);
				r3 = asuint(_FaceTex.Sample(sampler_FaceTex, asfloat(r0.zw)));
				r2.xyz = asuint(asfloat(r3.xyz) * asfloat(r2.xyz));
				r3.w = asuint(_FaceColor.w * asfloat(r3.w));
				r3.xyz = asuint(asfloat(r3.www) * asfloat(r2.xyz));
				r1 = asuint(-asfloat(r3) + asfloat(r1));
				r1 = asuint(asfloat(r0.yyyy) * asfloat(r1) + asfloat(r3));
				r0.y = asuint(_ScaleRatioA * _OutlineSoftness);
				r0.z = asuint(i_TEXCOORD1.y * asfloat(r0.y));
				r0.y = asuint(asfloat(r0.y) * i_TEXCOORD1.y + 1.0);
				r0.x = asuint(asfloat(r0.z) * 0.5 + asfloat(r0.x));
				r0.x = asuint(saturate(asfloat(r0.x) / asfloat(r0.y)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0 = asuint(asfloat(r1) * asfloat(r0.xxxx));
				r1.xy = asuint(_ClipRect.zw + -_ClipRect.xy);
				r1.xy = asuint(-abs(i_TEXCOORD2.xy) + asfloat(r1.xy));
				r1.xy = asuint(saturate(i_TEXCOORD2.zw * asfloat(r1.xy)));
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y));
				r0 = asuint(asfloat(r1.xxxx) * asfloat(r0));
				o_SV_Target0 = i_COLOR0.wwww * asfloat(r0);
			}
			#elif 1
			float4 _GlobalsPad0[2];
			float _FaceUVSpeedX;
			float _FaceUVSpeedY;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _FaceColor;
			float _GlobalsPad3;
			float _OutlineSoftness;
			float _OutlineUVSpeedX;
			float _OutlineUVSpeedY;
			float4 _OutlineColor;
			float _OutlineWidth;
			float _GlobalsPad4;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float4 _GlobalsPad7[15];
			float _GlobalsPad8;
			float _GlobalsPad9;
			float _GlobalsPad10;
			float _ScaleRatioA;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _FaceTex : register(t1);
			Texture2D<float4> _OutlineTex : register(t2);
			SamplerState sampler_FaceTex : register(s0);
			SamplerState sampler_OutlineTex : register(s1);
			SamplerState sampler_MainTex : register(s2);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float3 i_TEXCOORD3 : TEXCOORD3, float4 i_TEXCOORD5 : TEXCOORD5, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(-i_TEXCOORD1.x + asfloat(r0.w));
				r0.y = asuint(i_TEXCOORD1.z + -asfloat(r0.w));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				r0.x = asuint(_ScaleRatioA * _OutlineWidth);
				r0.x = asuint(i_TEXCOORD1.y * asfloat(r0.x));
				r0.z = asuint(min(1.0, asfloat(r0.x)));
				r0.x = asuint(0.5 * asfloat(r0.x));
				r0.z = asuint(sqrt(asfloat(r0.z)));
				r0.w = asuint(saturate(asfloat(r0.y) * i_TEXCOORD1.y + asfloat(r0.x)));
				r0.x = asuint(asfloat(r0.y) * i_TEXCOORD1.y + -asfloat(r0.x));
				r0.y = asuint(asfloat(r0.w) * asfloat(r0.z));
				r0.zw = asuint(float2(_OutlineUVSpeedX, _OutlineUVSpeedY) * _Time.yy + i_TEXCOORD5.zw);
				r1 = asuint(_OutlineTex.Sample(sampler_OutlineTex, asfloat(r0.zw)));
				r1 = asuint(_OutlineColor.xyzw * asfloat(r1));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r1.www));
				r2.xyz = asuint(_FaceColor.xyz * i_COLOR0.xyz);
				r0.zw = asuint(float2(_FaceUVSpeedX, _FaceUVSpeedY) * _Time.yy + i_TEXCOORD5.xy);
				r3 = asuint(_FaceTex.Sample(sampler_FaceTex, asfloat(r0.zw)));
				r2.xyz = asuint(asfloat(r3.xyz) * asfloat(r2.xyz));
				r3.w = asuint(_FaceColor.w * asfloat(r3.w));
				r3.xyz = asuint(asfloat(r3.www) * asfloat(r2.xyz));
				r1 = asuint(-asfloat(r3) + asfloat(r1));
				r1 = asuint(asfloat(r0.yyyy) * asfloat(r1) + asfloat(r3));
				r0.y = asuint(_ScaleRatioA * _OutlineSoftness);
				r0.z = asuint(i_TEXCOORD1.y * asfloat(r0.y));
				r0.y = asuint(asfloat(r0.y) * i_TEXCOORD1.y + 1.0);
				r0.x = asuint(asfloat(r0.z) * 0.5 + asfloat(r0.x));
				r0.x = asuint(saturate(asfloat(r0.x) / asfloat(r0.y)));
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0 = asuint(asfloat(r1) * asfloat(r0.xxxx));
				o_SV_Target0 = i_COLOR0.wwww * asfloat(r0);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
	Fallback "TextMeshPro/Mobile/Distance Field"
	CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
