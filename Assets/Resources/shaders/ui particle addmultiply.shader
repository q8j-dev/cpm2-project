Shader "UI Extensions/Particles/~Additive-Multiply"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}
	SubShader
	{
		Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass
		{
			Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
			ColorMask 14
			Cull Off
			ZTest [unity_GUIZTestMode]
			ZWrite Off
			Stencil { Ref [_Stencil] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] Comp [_StencilComp] Pass [_StencilOp] Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ FOG_EXP
			#pragma multi_compile __ SOFTPARTICLES_ON
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			#if defined(SHADER_STAGE_VERTEX)
			#if (defined(FOG_EXP) && defined(SOFTPARTICLES_ON) && defined(UNITY_UI_ALPHACLIP)) || (defined(FOG_EXP) && defined(SOFTPARTICLES_ON))
			float4 _GlobalsPad0[3];
			float4 _MainTex_ST;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ProjectionParams : packoffset(c5);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 unity_MatrixV : packoffset(c9);
				float4x4 unity_MatrixVP : packoffset(c17);
			}
			cbuffer UnityFog : register(b4)
			{
				float4 unity_FogParams : packoffset(c1);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float o_TEXCOORD1 : TEXCOORD1, out float4 o_TEXCOORD2 : TEXCOORD2)
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
				o_SV_POSITION0 = asfloat(r0);
				o_COLOR0 = i_COLOR0.xyzw;
				r1.x = asuint(asfloat(r0.z) / _ProjectionParams.y);
				r1.x = asuint(1.0 + -asfloat(r1.x));
				r1.x = asuint(_ProjectionParams.z * asfloat(r1.x));
				r1.x = asuint(max(0.0, asfloat(r1.x)));
				r1.x = asuint(unity_FogParams.y * asfloat(r1.x));
				o_TEXCOORD1 = exp2(-asfloat(r1.x));
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				r1 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r1));
				r0.z = asuint(unity_MatrixV._m21 * asfloat(r1.y));
				r0.z = asuint(unity_MatrixV._m20 * asfloat(r1.x) + asfloat(r0.z));
				r0.z = asuint(unity_MatrixV._m22 * asfloat(r1.z) + asfloat(r0.z));
				r0.z = asuint(unity_MatrixV._m23 * asfloat(r1.w) + asfloat(r0.z));
				o_TEXCOORD2.z = -asfloat(r0.z);
				r0.y = asuint(_ProjectionParams.x * asfloat(r0.y));
				r1.xzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.xwy));
				o_TEXCOORD2.w = asfloat(r0.w);
				o_TEXCOORD2.xy = asfloat(r1.xw) + asfloat(r1.zz);
			}
			#elif (defined(FOG_EXP) && defined(UNITY_UI_ALPHACLIP)) || defined(FOG_EXP)
			float4 _GlobalsPad0[3];
			float4 _MainTex_ST;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ProjectionParams : packoffset(c5);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}
			cbuffer UnityFog : register(b4)
			{
				float4 unity_FogParams : packoffset(c1);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float o_TEXCOORD1 : TEXCOORD1)
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
				o_SV_POSITION0 = asfloat(r0);
				r0.x = asuint(asfloat(r0.z) / _ProjectionParams.y);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0.x = asuint(_ProjectionParams.z * asfloat(r0.x));
				r0.x = asuint(max(0.0, asfloat(r0.x)));
				r0.x = asuint(unity_FogParams.y * asfloat(r0.x));
				o_TEXCOORD1 = exp2(-asfloat(r0.x));
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			}
			#elif (defined(SOFTPARTICLES_ON) && defined(UNITY_UI_ALPHACLIP)) || defined(SOFTPARTICLES_ON)
			float4 _GlobalsPad0[3];
			float4 _MainTex_ST;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ProjectionParams : packoffset(c5);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 unity_MatrixV : packoffset(c9);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD2 : TEXCOORD2)
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
				o_SV_POSITION0 = asfloat(r0);
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				r1 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r1));
				r0.z = asuint(unity_MatrixV._m21 * asfloat(r1.y));
				r0.z = asuint(unity_MatrixV._m20 * asfloat(r1.x) + asfloat(r0.z));
				r0.z = asuint(unity_MatrixV._m22 * asfloat(r1.z) + asfloat(r0.z));
				r0.z = asuint(unity_MatrixV._m23 * asfloat(r1.w) + asfloat(r0.z));
				o_TEXCOORD2.z = -asfloat(r0.z);
				r0.y = asuint(_ProjectionParams.x * asfloat(r0.y));
				r1.xzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.xwy));
				o_TEXCOORD2.w = asfloat(r0.w);
				o_TEXCOORD2.xy = asfloat(r1.xw) + asfloat(r1.zz);
			}
			#elif defined(UNITY_UI_ALPHACLIP) || 1
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

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0)
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
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if (defined(FOG_EXP) && defined(SOFTPARTICLES_ON) && defined(UNITY_UI_ALPHACLIP)) || (defined(FOG_EXP) && defined(SOFTPARTICLES_ON))
			float4 _GlobalsPad0[2];
			float4 _TintColor;
			float4 _GlobalsPad1[1];
			float _InvFade;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ZBufferParams : packoffset(c7);
			}
			Texture2D<float4> _CameraDepthTexture : register(t0);
			Texture2D<float4> _MainTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(i_TEXCOORD2.xy / i_TEXCOORD2.ww);
				r0 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, asfloat(r0.xy)));
				r0.x = asuint(_ZBufferParams.z * asfloat(r0.x) + _ZBufferParams.w);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.x = asuint(-i_TEXCOORD2.z + asfloat(r0.x));
				r0.x = asuint(saturate(_InvFade * asfloat(r0.x)));
				r0 = asuint(i_COLOR0.xyzw * asfloat(r0.xxxx));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1.xyz = asuint(_TintColor.xyz * asfloat(r1.xyz));
				r1.w = asuint(1.0 + -asfloat(r1.w));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xyz));
				r0.w = asuint(dot(_TintColor.ww, asfloat(r0.ww)));
				o_SV_Target0.w = asfloat(r1.w) * asfloat(r0.w);
				r0.xyz = asuint(asfloat(r0.xyz) + asfloat(r0.xyz));
				r0.w = asuint(saturate(i_TEXCOORD1));
				o_SV_Target0.xyz = asfloat(r0.www) * asfloat(r0.xyz);
			}
			#elif (defined(FOG_EXP) && defined(UNITY_UI_ALPHACLIP)) || defined(FOG_EXP)
			float4 _GlobalsPad0[2];
			float4 _TintColor;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.xyz = asuint(_TintColor.xyz * asfloat(r0.xyz));
				r0.w = asuint(1.0 + -asfloat(r0.w));
				r0.xyz = asuint(i_COLOR0.xyz * asfloat(r0.xyz));
				r0.xyz = asuint(asfloat(r0.xyz) + asfloat(r0.xyz));
				r1.x = asuint(saturate(i_TEXCOORD1));
				o_SV_Target0.xyz = asfloat(r1.xxx) * asfloat(r0.xyz);
				r0.x = asuint(dot(_TintColor.ww, i_COLOR0.ww));
				o_SV_Target0.w = asfloat(r0.w) * asfloat(r0.x);
			}
			#elif (defined(SOFTPARTICLES_ON) && defined(UNITY_UI_ALPHACLIP)) || defined(SOFTPARTICLES_ON)
			float4 _GlobalsPad0[2];
			float4 _TintColor;
			float4 _GlobalsPad1[1];
			float _InvFade;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ZBufferParams : packoffset(c7);
			}
			Texture2D<float4> _CameraDepthTexture : register(t0);
			Texture2D<float4> _MainTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(i_TEXCOORD2.xy / i_TEXCOORD2.ww);
				r0 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, asfloat(r0.xy)));
				r0.x = asuint(_ZBufferParams.z * asfloat(r0.x) + _ZBufferParams.w);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.x = asuint(-i_TEXCOORD2.z + asfloat(r0.x));
				r0.x = asuint(saturate(_InvFade * asfloat(r0.x)));
				r0 = asuint(i_COLOR0.xyzw * asfloat(r0.xxxx));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1.xyz = asuint(_TintColor.xyz * asfloat(r1.xyz));
				r1.w = asuint(1.0 + -asfloat(r1.w));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xyz));
				r0.w = asuint(dot(_TintColor.ww, asfloat(r0.ww)));
				o_SV_Target0.w = asfloat(r1.w) * asfloat(r0.w);
				o_SV_Target0.xyz = asfloat(r0.xyz) + asfloat(r0.xyz);
			}
			#elif defined(UNITY_UI_ALPHACLIP) || 1
			float4 _GlobalsPad0[2];
			float4 _TintColor;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.xyz = asuint(_TintColor.xyz * asfloat(r0.xyz));
				r0.w = asuint(1.0 + -asfloat(r0.w));
				r0.xyz = asuint(i_COLOR0.xyz * asfloat(r0.xyz));
				o_SV_Target0.xyz = asfloat(r0.xyz) + asfloat(r0.xyz);
				r0.x = asuint(dot(_TintColor.ww, i_COLOR0.ww));
				o_SV_Target0.w = asfloat(r0.w) * asfloat(r0.x);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
