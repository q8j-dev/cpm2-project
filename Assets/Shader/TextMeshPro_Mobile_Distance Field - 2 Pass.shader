Shader "TextMeshPro/Mobile/Distance Field - 2 Pass"
{
	Properties
	{
		_FaceColor ("Face Color", Color) = (1,1,1,1)
		_FaceDilate ("Face Dilate", Range(-1, 1)) = 0
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth ("Outline Thickness", Range(0, 1)) = 0
		_OutlineSoftness ("Outline Softness", Range(0, 1)) = 0
		_UnderlayColor ("Border Color", Color) = (0,0,0,0.5)
		_UnderlayOffsetX ("Border OffsetX", Range(-1, 1)) = 0
		_UnderlayOffsetY ("Border OffsetY", Range(-1, 1)) = 0
		_UnderlayDilate ("Border Dilate", Range(-1, 1)) = 0
		_UnderlaySoftness ("Border Softness", Range(0, 1)) = 0
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
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass
		{
			Name "Outline"
			Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
			ColorMask [_ColorMask]
			Cull [_CullMode]
			ZTest [unity_GUIZTestMode]
			ZWrite Off
			Stencil { Ref [_Stencil] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] Comp [_StencilComp] Pass [_StencilOp] Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ OUTLINE_ON
			#pragma multi_compile __ UNDERLAY_ON
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ UNITY_UI_CLIP_RECT

			#if defined(SHADER_STAGE_VERTEX)
			#if (defined(OUTLINE_ON) && defined(UNDERLAY_ON) && defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)) || (defined(OUTLINE_ON) && defined(UNDERLAY_ON) && defined(UNITY_UI_ALPHACLIP)) || (defined(OUTLINE_ON) && defined(UNDERLAY_ON) && defined(UNITY_UI_CLIP_RECT)) || (defined(OUTLINE_ON) && defined(UNDERLAY_ON))
			float4 _GlobalsPad0[3];
			float4 _FaceColor;
			float _FaceDilate;
			float _OutlineSoftness;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _OutlineColor;
			float _OutlineWidth;
			float _GlobalsPad3;
			float _GlobalsPad4;
			float _GlobalsPad5;
			float4 _GlobalsPad6[12];
			float _UnderlayOffsetX;
			float _UnderlayOffsetY;
			float _UnderlayDilate;
			float _UnderlaySoftness;
			float4 _GlobalsPad7[2];
			float _GlobalsPad8;
			float _WeightNormal;
			float _WeightBold;
			float _ScaleRatioA;
			float _GlobalsPad9;
			float _ScaleRatioC;
			float _VertexOffsetX;
			float _VertexOffsetY;
			float4 _GlobalsPad10[2];
			float4 _ClipRect;
			float _MaskSoftnessX;
			float _MaskSoftnessY;
			float _TextureWidth;
			float _TextureHeight;
			float _GradientScale;
			float _ScaleX;
			float _ScaleY;
			float _PerspectiveFilter;
			float _Sharpness;
			float _UIMaskSoftnessX;
			float _UIMaskSoftnessY;
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

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float4 o_COLOR1 : COLOR1, out float4 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD1 : TEXCOORD1, out float4 o_TEXCOORD2 : TEXCOORD2, out float4 o_TEXCOORD3 : TEXCOORD3, out float2 o_TEXCOORD4 : TEXCOORD4)
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
				r3.xyz = asuint(i_COLOR0.xyz);
				r3.w = 0x3f800000u;
				r3 = asuint(_FaceColor.xyzw * asfloat(r3));
				o_COLOR0.xyz = asfloat(r3.xyz) * asfloat(r3.www);
				o_COLOR0.w = _FaceColor.w;
				o_COLOR1.xyz = _OutlineColor.xyz * _OutlineColor.www;
				o_COLOR1.w = _OutlineColor.w;
				r3 = asuint(max(float4(-2.0e+10, -2.0e+10, -2.0e+10, -2.0e+10), _ClipRect.xyzw));
				r3 = asuint(min(float4(2.0e+10, 2.0e+10, 2.0e+10, 2.0e+10), asfloat(r3)));
				r0.zw = asuint(-asfloat(r3.xy) + asfloat(r0.xy));
				r0.xy = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + -asfloat(r3.xy));
				o_TEXCOORD2.xy = asfloat(r0.xy) + -asfloat(r3.zw);
				r0.xy = asuint(asfloat(r3.zw) + -asfloat(r3.xy));
				o_TEXCOORD0.zw = asfloat(r0.zw) / asfloat(r0.xy);
				o_TEXCOORD0.xy = i_TEXCOORD0.xy;
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r1.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r1.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r1.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www));
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r0.xyz)));
				r0.yz = asuint(glstate_matrix_projection._m01_m11 * _ScreenParams.yy);
				r0.yz = asuint(glstate_matrix_projection._m00_m10 * _ScreenParams.xx + asfloat(r0.yz));
				r0.yz = asuint(float2(_ScaleX, _ScaleY) * abs(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r2.ww) / asfloat(r0.yz));
				r0.w = asuint(dot(asfloat(r0.yz), asfloat(r0.yz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.x = asuint(_GradientScale * abs(i_TEXCOORD0.w));
				r1.y = asuint(1.0 + _Sharpness);
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y));
				r1.y = asuint(asfloat(r1.x) * asfloat(r0.w));
				r1.z = asuint(1.0 + -_PerspectiveFilter);
				r1.z = asuint(abs(asfloat(r1.y)) * asfloat(r1.z));
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.x) + -asfloat(r1.z));
				r0.x = asuint(abs(asfloat(r0.x)) * asfloat(r0.w) + asfloat(r1.z));
				b0.w = (0.0 == glstate_matrix_projection._m33);
				r0.x = b0.w ? r0.x : r1.y;
				r0.w = asuint(_ScaleRatioA * _OutlineSoftness);
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.x) + 1.0);
				r1.x = asuint(asfloat(r0.x) / asfloat(r0.w));
				b0.w = (0.0 >= i_TEXCOORD0.w);
				r0.w = b0.w ? 0x3f800000u : 0u;
				r1.y = asuint(_WeightBold + -_WeightNormal);
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.y) + _WeightNormal);
				r0.w = asuint(asfloat(r0.w) * 0.25 + _FaceDilate);
				r0.w = asuint(_ScaleRatioA * asfloat(r0.w));
				r0.w = asuint(0.5 - asfloat(r0.w) * 0.5);
				r1.w = asuint(asfloat(r0.w) * asfloat(r1.x) + -0.5);
				o_TEXCOORD1.xw = asfloat(r1.xw);
				r1.y = asuint(_ScaleRatioA * _OutlineWidth);
				r1.y = asuint(0.5 * asfloat(r1.y));
				o_TEXCOORD1.y = asfloat(r1.w) - asfloat(r1.y) * asfloat(r1.x);
				o_TEXCOORD1.z = asfloat(r1.y) * asfloat(r1.x) + asfloat(r1.w);
				r1.xy = asuint(max(float2(_UIMaskSoftnessX, _UIMaskSoftnessY), float2(_MaskSoftnessX, _MaskSoftnessY)));
				r0.yz = asuint(asfloat(r1.xy) * float2(0.25, 0.25) + asfloat(r0.yz));
				o_TEXCOORD2.zw = float2(0.25, 0.25) / asfloat(r0.yz);
				o_TEXCOORD3.z = i_COLOR0.w;
				o_TEXCOORD3.w = 0.0;
				r1 = asuint(float4(_ScaleRatioC, _ScaleRatioC, _ScaleRatioC, _ScaleRatioC) * float4(_UnderlaySoftness, _UnderlayDilate, _UnderlayOffsetX, _UnderlayOffsetY));
				r0.yz = asuint(float2(_GradientScale, _GradientScale) * -asfloat(r1.zw));
				r0.yz = asuint(asfloat(r0.yz) / float2(_TextureWidth, _TextureHeight));
				o_TEXCOORD3.xy = i_TEXCOORD0.xy + asfloat(r0.yz);
				r0.y = asuint(asfloat(r1.x) * asfloat(r0.x) + 1.0);
				r0.x = asuint(asfloat(r0.x) / asfloat(r0.y));
				r0.y = asuint(0.5 * asfloat(r1.y));
				r0.z = asuint(asfloat(r0.w) * asfloat(r0.x) + -0.5);
				o_TEXCOORD4.y = asfloat(r0.z) - asfloat(r0.y) * asfloat(r0.x);
				o_TEXCOORD4.x = asfloat(r0.x);
			}
			#elif (defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)) || defined(UNITY_UI_ALPHACLIP) || defined(UNITY_UI_CLIP_RECT) || 1
			float4 _GlobalsPad0[3];
			float4 _FaceColor;
			float _FaceDilate;
			float _OutlineSoftness;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _OutlineColor;
			float _OutlineWidth;
			float _GlobalsPad3;
			float _GlobalsPad4;
			float _GlobalsPad5;
			float4 _GlobalsPad6[15];
			float _GlobalsPad7;
			float _WeightNormal;
			float _WeightBold;
			float _ScaleRatioA;
			float _GlobalsPad8;
			float _GlobalsPad9;
			float _VertexOffsetX;
			float _VertexOffsetY;
			float4 _GlobalsPad10[2];
			float4 _ClipRect;
			float _MaskSoftnessX;
			float _MaskSoftnessY;
			float _GlobalsPad11;
			float _GlobalsPad12;
			float _GradientScale;
			float _ScaleX;
			float _ScaleY;
			float _PerspectiveFilter;
			float _Sharpness;
			float _UIMaskSoftnessX;
			float _UIMaskSoftnessY;
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

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float4 o_COLOR1 : COLOR1, out float4 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD1 : TEXCOORD1, out float4 o_TEXCOORD2 : TEXCOORD2)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b1;
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
				r3 = asuint(_FaceColor.xyzw * i_COLOR0.xyzw);
				o_COLOR0.xyz = asfloat(r3.xyz) * asfloat(r3.www);
				o_COLOR0.w = asfloat(r3.w);
				r0.z = asuint(_OutlineColor.w * i_COLOR0.w);
				o_COLOR1.xyz = _OutlineColor.xyz * asfloat(r0.zzz);
				o_COLOR1.w = asfloat(r0.z);
				r3 = asuint(max(float4(-2.0e+10, -2.0e+10, -2.0e+10, -2.0e+10), _ClipRect.xyzw));
				r3 = asuint(min(float4(2.0e+10, 2.0e+10, 2.0e+10, 2.0e+10), asfloat(r3)));
				r0.zw = asuint(-asfloat(r3.xy) + asfloat(r0.xy));
				r0.xy = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + -asfloat(r3.xy));
				o_TEXCOORD2.xy = asfloat(r0.xy) + -asfloat(r3.zw);
				r0.xy = asuint(asfloat(r3.zw) + -asfloat(r3.xy));
				o_TEXCOORD0.zw = asfloat(r0.zw) / asfloat(r0.xy);
				o_TEXCOORD0.xy = i_TEXCOORD0.xy;
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r1.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r1.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r1.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www));
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r0.xyz)));
				r0.yz = asuint(glstate_matrix_projection._m01_m11 * _ScreenParams.yy);
				r0.yz = asuint(glstate_matrix_projection._m00_m10 * _ScreenParams.xx + asfloat(r0.yz));
				r0.yz = asuint(float2(_ScaleX, _ScaleY) * abs(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r2.ww) / asfloat(r0.yz));
				r0.w = asuint(dot(asfloat(r0.yz), asfloat(r0.yz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.x = asuint(_GradientScale * abs(i_TEXCOORD0.w));
				r1.y = asuint(1.0 + _Sharpness);
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y));
				r1.y = asuint(asfloat(r1.x) * asfloat(r0.w));
				r1.z = asuint(1.0 + -_PerspectiveFilter);
				r1.z = asuint(abs(asfloat(r1.y)) * asfloat(r1.z));
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.x) + -asfloat(r1.z));
				r0.x = asuint(abs(asfloat(r0.x)) * asfloat(r0.w) + asfloat(r1.z));
				b0.w = (0.0 == glstate_matrix_projection._m33);
				r0.x = b0.w ? r0.x : r1.y;
				r0.w = asuint(_ScaleRatioA * _OutlineSoftness);
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.x) + 1.0);
				r0.x = asuint(asfloat(r0.x) / asfloat(r0.w));
				b1.x = (0.0 >= i_TEXCOORD0.w);
				r1.x = b1.x ? 0x3f800000u : 0u;
				r1.y = asuint(_WeightBold + -_WeightNormal);
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y) + _WeightNormal);
				r1.x = asuint(asfloat(r1.x) * 0.25 + _FaceDilate);
				r1.x = asuint(_ScaleRatioA * asfloat(r1.x));
				r1.x = asuint(0.5 - asfloat(r1.x) * 0.5);
				r0.w = asuint(asfloat(r1.x) * asfloat(r0.x) + -0.5);
				o_TEXCOORD1.xw = asfloat(r0.xw);
				r1.x = asuint(_ScaleRatioA * _OutlineWidth);
				r1.x = asuint(0.5 * asfloat(r1.x));
				o_TEXCOORD1.y = asfloat(r0.w) - asfloat(r1.x) * asfloat(r0.x);
				o_TEXCOORD1.z = asfloat(r1.x) * asfloat(r0.x) + asfloat(r0.w);
				r0.xw = asuint(max(float2(_UIMaskSoftnessX, _UIMaskSoftnessY), float2(_MaskSoftnessX, _MaskSoftnessY)));
				r0.xy = asuint(asfloat(r0.xw) * float2(0.25, 0.25) + asfloat(r0.yz));
				o_TEXCOORD2.zw = float2(0.25, 0.25) / asfloat(r0.xy);
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(OUTLINE_ON) && defined(UNDERLAY_ON) && defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[18];
			float4 _UnderlayColor;
			float4 _GlobalsPad1[7];
			float4 _ClipRect;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_COLOR1 : COLOR1, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float2 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD3.xy));
				r0.x = asuint(saturate(asfloat(r0.w) * i_TEXCOORD4.x + -i_TEXCOORD4.y));
				r1.xyz = asuint(_UnderlayColor.xyz * _UnderlayColor.www);
				r1.w = asuint(_UnderlayColor.w);
				r0 = asuint(asfloat(r1) * asfloat(r0.xxxx));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1.x = asuint(saturate(asfloat(r1.w) * i_TEXCOORD1.x + -i_TEXCOORD1.y));
				r2 = asuint(i_COLOR1.xyzw * asfloat(r1.xxxx));
				r1.x = asuint(1.0 - i_COLOR1.w * asfloat(r1.x));
				r0 = asuint(asfloat(r0) * asfloat(r1.xxxx) + asfloat(r2));
				r1.xy = asuint(_ClipRect.zw + -_ClipRect.xy);
				r1.xy = asuint(-abs(i_TEXCOORD2.xy) + asfloat(r1.xy));
				r1.xy = asuint(saturate(i_TEXCOORD2.zw * asfloat(r1.xy)));
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y));
				r0 = asuint(asfloat(r1.xxxx) * asfloat(r0));
				r1.x = asuint(asfloat(r0.w) * i_TEXCOORD3.z + -0.00100000005);
				r0 = asuint(i_TEXCOORD3.zzzz * asfloat(r0));
				o_SV_Target0 = asfloat(r0);
				b0.x = (asfloat(r1.x) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(OUTLINE_ON) && defined(UNDERLAY_ON) && defined(UNITY_UI_ALPHACLIP)
			float4 _GlobalsPad0[18];
			float4 _UnderlayColor;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_COLOR1 : COLOR1, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float2 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD3.xy));
				r0.x = asuint(saturate(asfloat(r0.w) * i_TEXCOORD4.x + -i_TEXCOORD4.y));
				r1.xyz = asuint(_UnderlayColor.xyz * _UnderlayColor.www);
				r1.w = asuint(_UnderlayColor.w);
				r0 = asuint(asfloat(r1) * asfloat(r0.xxxx));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1.x = asuint(saturate(asfloat(r1.w) * i_TEXCOORD1.x + -i_TEXCOORD1.y));
				r2 = asuint(i_COLOR1.xyzw * asfloat(r1.xxxx));
				r1.x = asuint(1.0 - i_COLOR1.w * asfloat(r1.x));
				r0 = asuint(asfloat(r0) * asfloat(r1.xxxx) + asfloat(r2));
				r1.x = asuint(asfloat(r0.w) * i_TEXCOORD3.z + -0.00100000005);
				r0 = asuint(i_TEXCOORD3.zzzz * asfloat(r0));
				o_SV_Target0 = asfloat(r0);
				b0.x = (asfloat(r1.x) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(OUTLINE_ON) && defined(UNDERLAY_ON) && defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[18];
			float4 _UnderlayColor;
			float4 _GlobalsPad1[7];
			float4 _ClipRect;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_COLOR1 : COLOR1, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float2 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD3.xy));
				r0.x = asuint(saturate(asfloat(r0.w) * i_TEXCOORD4.x + -i_TEXCOORD4.y));
				r1.xyz = asuint(_UnderlayColor.xyz * _UnderlayColor.www);
				r1.w = asuint(_UnderlayColor.w);
				r0 = asuint(asfloat(r1) * asfloat(r0.xxxx));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1.x = asuint(saturate(asfloat(r1.w) * i_TEXCOORD1.x + -i_TEXCOORD1.y));
				r2 = asuint(i_COLOR1.xyzw * asfloat(r1.xxxx));
				r1.x = asuint(1.0 - i_COLOR1.w * asfloat(r1.x));
				r0 = asuint(asfloat(r0) * asfloat(r1.xxxx) + asfloat(r2));
				r1.xy = asuint(_ClipRect.zw + -_ClipRect.xy);
				r1.xy = asuint(-abs(i_TEXCOORD2.xy) + asfloat(r1.xy));
				r1.xy = asuint(saturate(i_TEXCOORD2.zw * asfloat(r1.xy)));
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y));
				r0 = asuint(asfloat(r1.xxxx) * asfloat(r0));
				o_SV_Target0 = i_TEXCOORD3.zzzz * asfloat(r0);
			}
			#elif defined(OUTLINE_ON) && defined(UNDERLAY_ON)
			float4 _GlobalsPad0[18];
			float4 _UnderlayColor;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_COLOR1 : COLOR1, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD3 : TEXCOORD3, float2 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD3.xy));
				r0.x = asuint(saturate(asfloat(r0.w) * i_TEXCOORD4.x + -i_TEXCOORD4.y));
				r1.xyz = asuint(_UnderlayColor.xyz * _UnderlayColor.www);
				r1.w = asuint(_UnderlayColor.w);
				r0 = asuint(asfloat(r1) * asfloat(r0.xxxx));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1.x = asuint(saturate(asfloat(r1.w) * i_TEXCOORD1.x + -i_TEXCOORD1.y));
				r2 = asuint(i_COLOR1.xyzw * asfloat(r1.xxxx));
				r1.x = asuint(1.0 - i_COLOR1.w * asfloat(r1.x));
				r0 = asuint(asfloat(r0) * asfloat(r1.xxxx) + asfloat(r2));
				o_SV_Target0 = i_TEXCOORD3.zzzz * asfloat(r0);
			}
			#elif (defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)) || defined(UNITY_UI_ALPHACLIP)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_COLOR1 : COLOR1, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				if (0xffffffffu != 0) discard;
				o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#elif defined(UNITY_UI_CLIP_RECT) || 1

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_COLOR1 : COLOR1, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#endif
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "Face"
			Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
			ColorMask [_ColorMask]
			Cull [_CullMode]
			ZTest [unity_GUIZTestMode]
			ZWrite Off
			Stencil { Ref [_Stencil] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] Comp [_StencilComp] Pass [_StencilOp] Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ UNITY_UI_CLIP_RECT

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[3];
			float4 _FaceColor;
			float _FaceDilate;
			float _OutlineSoftness;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[17];
			float _GlobalsPad4;
			float _WeightNormal;
			float _WeightBold;
			float _ScaleRatioA;
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _VertexOffsetX;
			float _VertexOffsetY;
			float4 _GlobalsPad7[2];
			float4 _ClipRect;
			float _MaskSoftnessX;
			float _MaskSoftnessY;
			float _GlobalsPad8;
			float _GlobalsPad9;
			float _GradientScale;
			float _ScaleX;
			float _ScaleY;
			float _PerspectiveFilter;
			float _Sharpness;
			float _UIMaskSoftnessX;
			float _UIMaskSoftnessY;
			int _UIVertexColorAlwaysGammaSpace;
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

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float4 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float4 o_TEXCOORD2 : TEXCOORD2)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				bool4 b4;
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
				r2.xyz = asuint(i_COLOR0.xyz * float3(0.265884995, 0.265884995, 0.265884995) + float3(0.736584008, 0.736584008, 0.736584008));
				r2.xyz = asuint(i_COLOR0.xyz * asfloat(r2.xyz) + float3(-0.00980184041, -0.00980184041, -0.00980184041));
				r2.xyz = asuint(i_COLOR0.xyz * asfloat(r2.xyz) + float3(0.00319697009, 0.00319697009, 0.00319697009));
				r3.xyz = asuint(i_COLOR0.xyz * float3(0.0849710032, 0.0849710032, 0.0849710032) + float3(-0.000163029006, -0.000163029006, -0.000163029006));
				b4.xyz = (i_COLOR0.xyz < float3(0.0725490004, 0.0725490004, 0.0725490004));
				r2.xyz = b4.xyz ? r3.xyz : r2.xyz;
				r3.xyz = (asuint(float3(asfloat(_UIVertexColorAlwaysGammaSpace), asfloat(_UIVertexColorAlwaysGammaSpace), asfloat(_UIVertexColorAlwaysGammaSpace))) != 0) ? r2.xyz : asuint(i_COLOR0.xyz);
				r3.w = asuint(i_COLOR0.w);
				r3 = asuint(_FaceColor.xyzw * asfloat(r3));
				o_COLOR0.xyz = asfloat(r3.xyz) * asfloat(r3.www);
				o_COLOR0.w = asfloat(r3.w);
				r3 = asuint(max(float4(-2.0e+10, -2.0e+10, -2.0e+10, -2.0e+10), _ClipRect.xyzw));
				r3 = asuint(min(float4(2.0e+10, 2.0e+10, 2.0e+10, 2.0e+10), asfloat(r3)));
				r0.zw = asuint(-asfloat(r3.xy) + asfloat(r0.xy));
				r0.xy = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + -asfloat(r3.xy));
				o_TEXCOORD2.xy = asfloat(r0.xy) + -asfloat(r3.zw);
				r0.xy = asuint(asfloat(r3.zw) + -asfloat(r3.xy));
				o_TEXCOORD0.zw = asfloat(r0.zw) / asfloat(r0.xy);
				o_TEXCOORD0.xy = i_TEXCOORD0.xy;
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xxx));
				r1.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r1.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r1.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www));
				r0.x = asuint(dot(asfloat(r1.xyz), asfloat(r0.xyz)));
				r0.yz = asuint(glstate_matrix_projection._m01_m11 * _ScreenParams.yy);
				r0.yz = asuint(glstate_matrix_projection._m00_m10 * _ScreenParams.xx + asfloat(r0.yz));
				r0.yz = asuint(float2(_ScaleX, _ScaleY) * abs(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r2.ww) / asfloat(r0.yz));
				r0.w = asuint(dot(asfloat(r0.yz), asfloat(r0.yz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.x = asuint(_GradientScale * abs(i_TEXCOORD0.w));
				r1.y = asuint(1.0 + _Sharpness);
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y));
				r1.y = asuint(asfloat(r1.x) * asfloat(r0.w));
				r1.z = asuint(1.0 + -_PerspectiveFilter);
				r1.z = asuint(abs(asfloat(r1.y)) * asfloat(r1.z));
				r0.w = asuint(asfloat(r1.x) * asfloat(r0.w) + -asfloat(r1.z));
				r0.x = asuint(abs(asfloat(r0.x)) * asfloat(r0.w) + asfloat(r1.z));
				b0.w = (glstate_matrix_projection._m33 == 0.0);
				r0.x = b0.w ? r0.x : r1.y;
				r0.w = asuint(_ScaleRatioA * _OutlineSoftness);
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.x) + 1.0);
				r0.x = asuint(asfloat(r0.x) / asfloat(r0.w));
				b0.w = (0.0 >= i_TEXCOORD0.w);
				r0.w = b0.w ? 0x3f800000u : 0u;
				r1.x = asuint(_WeightBold + -_WeightNormal);
				r0.w = asuint(asfloat(r0.w) * asfloat(r1.x) + _WeightNormal);
				r0.w = asuint(asfloat(r0.w) * 0.25 + _FaceDilate);
				r0.w = asuint(_ScaleRatioA * asfloat(r0.w));
				r0.w = asuint(0.5 - asfloat(r0.w) * 0.5);
				o_TEXCOORD1.y = asfloat(r0.w) * asfloat(r0.x) + -0.5;
				o_TEXCOORD1.x = asfloat(r0.x);
				r0.xw = asuint(max(float2(_UIMaskSoftnessX, _UIMaskSoftnessY), float2(_MaskSoftnessX, _MaskSoftnessY)));
				r0.xy = asuint(asfloat(r0.xw) * float2(0.25, 0.25) + asfloat(r0.yz));
				o_TEXCOORD2.zw = float2(0.25, 0.25) / asfloat(r0.xy);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[26];
			float4 _ClipRect;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.xy = asuint(_ClipRect.zw + -_ClipRect.xy);
				r0.xy = asuint(-abs(i_TEXCOORD2.xy) + asfloat(r0.xy));
				r0.xy = asuint(saturate(i_TEXCOORD2.zw * asfloat(r0.xy)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.y = asuint(saturate(asfloat(r1.w) * i_TEXCOORD1.x + -i_TEXCOORD1.y));
				r1 = asuint(i_COLOR0.xyzw * asfloat(r0.yyyy));
				r0.y = asuint(asfloat(r1.w) * asfloat(r0.x) + -0.00100000005);
				r1 = asuint(asfloat(r1) * asfloat(r0.xxxx));
				o_SV_Target0 = asfloat(r1);
				b0.x = (asfloat(r0.y) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(UNITY_UI_ALPHACLIP)
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(saturate(asfloat(r0.w) * i_TEXCOORD1.x + -i_TEXCOORD1.y));
				r0.y = asuint(i_COLOR0.w * asfloat(r0.x) + -0.00100000005);
				r1 = asuint(i_COLOR0.xyzw * asfloat(r0.xxxx));
				o_SV_Target0 = asfloat(r1);
				b0.x = (asfloat(r0.y) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[26];
			float4 _ClipRect;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(_ClipRect.zw + -_ClipRect.xy);
				r0.xy = asuint(-abs(i_TEXCOORD2.xy) + asfloat(r0.xy));
				r0.xy = asuint(saturate(i_TEXCOORD2.zw * asfloat(r0.xy)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.y = asuint(saturate(asfloat(r1.w) * i_TEXCOORD1.x + -i_TEXCOORD1.y));
				r1 = asuint(i_COLOR0.xyzw * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r1) * asfloat(r0.xxxx);
			}
			#elif 1
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(saturate(asfloat(r0.w) * i_TEXCOORD1.x + -i_TEXCOORD1.y));
				o_SV_Target0 = i_COLOR0.xyzw * asfloat(r0.xxxx);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
	CustomEditor "TMPro.EditorUtilities.TMP_SDFShaderGUI"
}
