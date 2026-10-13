Shader "HighlightPlus/Geometry/SolidColor"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_CutOff ("CutOff", Float) = 0.5
		_Cull ("Cull Mode", Float) = 2
		_ZTest ("ZTest", Float) = 4
	}
	SubShader
	{
		Tags { "QUEUE" = "Transparent+100" "RenderType" = "Transparent" }
		Pass
		{
			Cull [_Cull]
			ZTest [_ZTest]
			ZWrite Off
			Offset -1, -1
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ HP_ALPHACLIP
			#pragma multi_compile __ HP_DEPTHCLIP

			#if defined(SHADER_STAGE_VERTEX)
			#if (defined(HP_ALPHACLIP) && defined(HP_DEPTHCLIP)) || defined(HP_DEPTHCLIP)
			float4 _GlobalsPad0[2];
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

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float o_TEXCOORD2 : TEXCOORD2, out float4 o_TEXCOORD1 : TEXCOORD1)
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
				o_SV_POSITION0 = asfloat(r1);
				r0.y = asuint(unity_MatrixV._m21 * asfloat(r0.y));
				r0.x = asuint(unity_MatrixV._m20 * asfloat(r0.x) + asfloat(r0.y));
				r0.x = asuint(unity_MatrixV._m22 * asfloat(r0.z) + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 * asfloat(r0.w) + asfloat(r0.x));
				r0.x = asuint(_ProjectionParams.w * asfloat(r0.x));
				o_TEXCOORD2 = -asfloat(r0.x);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				r0.x = asuint(_ProjectionParams.x * asfloat(r1.y));
				r0.w = asuint(0.5 * asfloat(r0.x));
				r0.xz = asuint(float2(0.5, 0.5) * asfloat(r1.xw));
				o_TEXCOORD1.zw = asfloat(r1.zw);
				o_TEXCOORD1.xy = asfloat(r0.xw) + asfloat(r0.zz);
			}
			#elif defined(HP_ALPHACLIP) || 1
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
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(HP_ALPHACLIP) && defined(HP_DEPTHCLIP)
			float4 _GlobalsPad0[3];
			float _CutOff;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ZBufferParams : packoffset(c7);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _CameraDepthTexture : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_CameraDepthTexture : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(-_CutOff + asfloat(r0.w));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				r0.xy = asuint(i_TEXCOORD1.xy / i_TEXCOORD1.ww);
				r0 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, asfloat(r0.xy)));
				r0.x = asuint(_ZBufferParams.x * asfloat(r0.x) + _ZBufferParams.y);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) - i_TEXCOORD2 * 0.999000013);
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				o_SV_Target0 = float4(1.0, 1.0, 1.0, 1.0);
			}
			#elif defined(HP_DEPTHCLIP)
			cbuffer UnityPerCamera : register(b0)
			{
				float4 _ZBufferParams : packoffset(c7);
			}
			Texture2D<float4> _CameraDepthTexture : register(t0);
			SamplerState sampler_CameraDepthTexture : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float i_TEXCOORD2 : TEXCOORD2, float4 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD1.xy / i_TEXCOORD1.ww);
				r0 = asuint(_CameraDepthTexture.Sample(sampler_CameraDepthTexture, asfloat(r0.xy)));
				r0.x = asuint(_ZBufferParams.x * asfloat(r0.x) + _ZBufferParams.y);
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.x = asuint(asfloat(r0.x) - i_TEXCOORD2 * 0.999000013);
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				o_SV_Target0 = float4(1.0, 1.0, 1.0, 1.0);
			}
			#elif defined(HP_ALPHACLIP)
			float4 _GlobalsPad0[3];
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
				o_SV_Target0 = float4(1.0, 1.0, 1.0, 1.0);
			}
			#elif 1

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				o_SV_Target0 = float4(1.0, 1.0, 1.0, 1.0);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
