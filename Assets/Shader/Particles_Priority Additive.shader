Shader "Particles/Priority Additive"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01, 3)) = 1
	}
	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent+1" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha One, SrcAlpha One
			ColorMask 14
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ SOFTPARTICLES_ON

			#if defined(SHADER_STAGE_VERTEX)
			#if defined(SOFTPARTICLES_ON)
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

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Position0 : SV_Position, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD1 : TEXCOORD1)
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
				r1 = asuint(unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1));
				o_SV_Position0 = asfloat(r1);
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				r0.y = asuint(unity_MatrixV._m21 * asfloat(r0.y));
				r0.x = asuint(unity_MatrixV._m20 * asfloat(r0.x) + asfloat(r0.y));
				r0.x = asuint(unity_MatrixV._m22 * asfloat(r0.z) + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 * asfloat(r0.w) + asfloat(r0.x));
				o_TEXCOORD1.z = -asfloat(r0.x);
				r0.x = asuint(_ProjectionParams.x * asfloat(r1.y));
				r0.w = asuint(0.5 * asfloat(r0.x));
				r0.xz = asuint(float2(0.5, 0.5) * asfloat(r1.xw));
				o_TEXCOORD1.w = asfloat(r1.w);
				o_TEXCOORD1.xy = asfloat(r0.xw) + asfloat(r0.zz);
			}
			#elif 1
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

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Position0 : SV_Position, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0)
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
				o_SV_Position0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(SOFTPARTICLES_ON)
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

			void frag(float4 i_SV_Position0 : SV_Position, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(i_TEXCOORD1.xy / i_TEXCOORD1.ww);
				r0 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, asfloat(r0.xy)));
				r0.x = asuint(_ZBufferParams.z * asfloat(r0.x) + _ZBufferParams.w);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.x = asuint(-i_TEXCOORD1.z + asfloat(r0.x));
				r0.x = asuint(saturate(_InvFade * asfloat(r0.x)));
				r0.w = asuint(i_COLOR0.w * asfloat(r0.x));
				r0.xyz = asuint(i_COLOR0.xyz);
				r0 = asuint(asfloat(r0) + asfloat(r0));
				r0 = asuint(_TintColor.xyzw * asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				o_SV_Target0 = asfloat(r1) * asfloat(r0);
			}
			#elif 1
			float4 _GlobalsPad0[2];
			float4 _TintColor;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_Position0 : SV_Position, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(i_COLOR0.xyzw + i_COLOR0.xyzw);
				r0 = asuint(_TintColor.xyzw * asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				o_SV_Target0 = asfloat(r1) * asfloat(r0);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent+1" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha One, SrcAlpha One
			ColorMask 14
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ FOG_EXP

			#if defined(SHADER_STAGE_VERTEX)
			#if defined(FOG_EXP)
			float4 _GlobalsPad0[2];
			float4 _MainTex_ST;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixV : packoffset(c9);
				float4x4 unity_MatrixVP : packoffset(c17);
			}
			cbuffer UnityFog : register(b3)
			{
				float4 unity_FogParams : packoffset(c1);
			}

			void vert(float3 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float3 i_TEXCOORD0 : TEXCOORD0, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float o_TEXCOORD2 : TEXCOORD2, out float4 o_SV_POSITION0 : SV_POSITION)
			{
				uint4 r0;
				uint4 r1;
				o_COLOR0 = saturate(i_COLOR0.xyzw);
				float4 out0 = i_TEXCOORD0.xyxy * _MainTex_ST.xyxy + _MainTex_ST.zwzw;
				o_TEXCOORD0 = out0.xy;
				o_TEXCOORD1 = out0.zw;
				r0.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m11_m11_m11);
				r0.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m01_m01_m01 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m21_m21_m21 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m31_m31_m31 + asfloat(r0.xyz));
				r0.xyz = asuint(i_POSITION0.yyy * asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m10_m10_m10);
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m00_m00_m00 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m20_m20_m20 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m30_m30_m30 + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) * i_POSITION0.xxx + asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m12_m12_m12);
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m02_m02_m02 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m22_m22_m22 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m32_m32_m32 + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) * i_POSITION0.zzz + asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m13_m13_m13);
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m03_m03_m03 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m23_m23_m23 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m33_m33_m33 + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) + asfloat(r0.xyz));
				r0.x = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(unity_FogParams.y * asfloat(r0.x));
				r0.x = asuint(exp2(-asfloat(r0.x)));
				o_TEXCOORD2 = min(1.0, asfloat(r0.x));
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
			}
			#elif 1
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

			void vert(float3 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float3 i_TEXCOORD0 : TEXCOORD0, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float4 o_SV_POSITION0 : SV_POSITION)
			{
				uint4 r0;
				uint4 r1;
				o_COLOR0 = saturate(i_COLOR0.xyzw);
				float4 out0 = i_TEXCOORD0.xyxy * _MainTex_ST.xyxy + _MainTex_ST.zwzw;
				o_TEXCOORD0 = out0.xy;
				o_TEXCOORD1 = out0.zw;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(FOG_EXP)
			float4 _GlobalsPad0[3];
			float4 _TintColor;
			cbuffer UnityFog : register(b1)
			{
				float4 unity_FogColor : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float i_TEXCOORD2 : TEXCOORD2, float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0 = asuint(_TintColor.xyzw * i_COLOR0.xyzw);
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r0 = asuint(asfloat(r1) * asfloat(r0));
				b1.x = (0.00499999989 >= asfloat(r0.w));
				if (b1.x) discard;
				r0.xyz = asuint(asfloat(r0.xyz) * float3(2.0, 2.0, 2.0) + -unity_FogColor.xyz);
				r0.w = asuint(asfloat(r0.w) + asfloat(r0.w));
				o_SV_Target0.w = asfloat(r0.w);
				o_SV_Target0.xyz = float3(i_TEXCOORD2, i_TEXCOORD2, i_TEXCOORD2) * asfloat(r0.xyz) + unity_FogColor.xyz;
			}
			#elif 1
			float4 _GlobalsPad0[3];
			float4 _TintColor;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0 = asuint(_TintColor.xyzw * i_COLOR0.xyzw);
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r0 = asuint(asfloat(r1) * asfloat(r0));
				b1.x = (0.00499999989 >= asfloat(r0.w));
				o_SV_Target0 = asfloat(r0) + asfloat(r0);
				if (b1.x) discard;
			}
			#endif
			#endif
			ENDHLSL
		}
	}
	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent+1" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha One, SrcAlpha One
			ColorMask 14
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ FOG_EXP

			#if defined(SHADER_STAGE_VERTEX)
			#if defined(FOG_EXP)
			float4 _GlobalsPad0[2];
			float4 _MainTex_ST;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixV : packoffset(c9);
				float4x4 unity_MatrixVP : packoffset(c17);
			}
			cbuffer UnityFog : register(b3)
			{
				float4 unity_FogParams : packoffset(c1);
			}

			void vert(float3 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float3 i_TEXCOORD0 : TEXCOORD0, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float o_TEXCOORD1 : TEXCOORD1, out float4 o_SV_POSITION0 : SV_POSITION)
			{
				uint4 r0;
				uint4 r1;
				o_COLOR0 = saturate(i_COLOR0.xyzw);
				r0.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m11_m11_m11);
				r0.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m01_m01_m01 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m21_m21_m21 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m31_m31_m31 + asfloat(r0.xyz));
				r0.xyz = asuint(i_POSITION0.yyy * asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m10_m10_m10);
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m00_m00_m00 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m20_m20_m20 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m30_m30_m30 + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) * i_POSITION0.xxx + asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m12_m12_m12);
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m02_m02_m02 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m22_m22_m22 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m32_m32_m32 + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) * i_POSITION0.zzz + asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * unity_ObjectToWorld._m13_m13_m13);
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * unity_ObjectToWorld._m03_m03_m03 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m02_m12_m22 * unity_ObjectToWorld._m23_m23_m23 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_MatrixV._m03_m13_m23 * unity_ObjectToWorld._m33_m33_m33 + asfloat(r1.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) + asfloat(r0.xyz));
				r0.x = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(unity_FogParams.y * asfloat(r0.x));
				r0.x = asuint(exp2(-asfloat(r0.x)));
				o_TEXCOORD1 = min(1.0, asfloat(r0.x));
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
			}
			#elif 1
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

			void vert(float3 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float3 i_TEXCOORD0 : TEXCOORD0, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION)
			{
				uint4 r0;
				uint4 r1;
				o_COLOR0 = saturate(i_COLOR0.xyzw);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(FOG_EXP)
			cbuffer UnityFog : register(b0)
			{
				float4 unity_FogColor : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float i_TEXCOORD1 : TEXCOORD1, float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.w = asuint(i_COLOR0.w * asfloat(r0.w));
				r0.xyz = asuint(asfloat(r0.xyz) * i_COLOR0.xyz + -unity_FogColor.xyz);
				o_SV_Target0.xyz = float3(i_TEXCOORD1, i_TEXCOORD1, i_TEXCOORD1) * asfloat(r0.xyz) + unity_FogColor.xyz;
				b0.x = (0.00999999978 >= asfloat(r0.w));
				o_SV_Target0.w = asfloat(r0.w);
				if (b0.x) discard;
			}
			#elif 1
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0 = asuint(i_COLOR0.xyzw * asfloat(r0));
				b1.x = (0.00999999978 >= asfloat(r0.w));
				o_SV_Target0 = asfloat(r0);
				if (b1.x) discard;
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
