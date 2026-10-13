Shader "HighlightPlus/Geometry/BlurGlow"
{
	Properties
	{
		_MainTex ("Texture", any) = "white" {}
		_Color ("Color", Color) = (1,1,0,1)
		_BlurScale ("Blur Scale", Float) = 2
		_Speed ("Speed", Float) = 1
		_StereoRendering ("Stereo Rendering Correction", Float) = 1
	}
	SubShader
	{
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _MainTex_TexelSize;
			float4 _GlobalsPad1[1];
			float _BlurScale;
			float _Speed;
			float _StereoRendering;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float2 o_TEXCOORD2 : TEXCOORD2, out float2 o_TEXCOORD3 : TEXCOORD3, out float2 o_TEXCOORD4 : TEXCOORD4)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				r0.x = asuint(_Time.w * _Speed);
				r0.x = asuint(sin(asfloat(r0.x)));
				r0.x = asuint(asfloat(r0.x) * 0.25 + 1.0);
				r0.x = asuint(_BlurScale * asfloat(r0.x));
				r0.x = asuint(_MainTex_TexelSize.x * asfloat(r0.x));
				r0.x = asuint(_StereoRendering * asfloat(r0.x));
				r1.x = asuint(i_TEXCOORD0.x);
				o_TEXCOORD1.x = asfloat(r1.x) - asfloat(r0.x) * 1.38461542;
				r0.xz = asuint(float2(1.38461542, 3.23076916) * asfloat(r0.xx));
				b0.y = (_MainTex_TexelSize.y < 0.0);
				r1.z = asuint(1.0 + -i_TEXCOORD0.y);
				r1.yw = b0.yy ? r1.zz : asuint(i_TEXCOORD0.yy);
				float3 out0 = asfloat(r1.xyw);
				o_TEXCOORD0 = out0.xy;
				o_TEXCOORD1.y = out0.z;
				r0.w = 0u;
				o_TEXCOORD2 = asfloat(r1.xw) + asfloat(r0.xw);
				o_TEXCOORD3 = asfloat(r1.xw) + -asfloat(r0.zw);
				o_TEXCOORD4 = asfloat(r1.xw) + asfloat(r0.zw);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float2 i_TEXCOORD2 : TEXCOORD2, float2 i_TEXCOORD3 : TEXCOORD3, float2 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD2.xy));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				r0 = asuint(float4(0.31621623, 0.31621623, 0.31621623, 0.31621623) * asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0 = asuint(asfloat(r1) * float4(0.227027029, 0.227027029, 0.227027029, 0.227027029) + asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD3.xy));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD4.xy));
				r1 = asuint(asfloat(r2) + asfloat(r1));
				o_SV_Target0 = asfloat(r1) * float4(0.0702702701, 0.0702702701, 0.0702702701, 0.0702702701) + asfloat(r0);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _MainTex_TexelSize;
			float4 _GlobalsPad1[1];
			float _BlurScale;
			float _Speed;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float2 o_TEXCOORD2 : TEXCOORD2, out float2 o_TEXCOORD3 : TEXCOORD3, out float2 o_TEXCOORD4 : TEXCOORD4)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				r0.x = asuint(_Time.w * _Speed);
				r0.x = asuint(sin(asfloat(r0.x)));
				r0.x = asuint(asfloat(r0.x) * 0.25 + 1.0);
				r0.x = asuint(_BlurScale * asfloat(r0.x));
				r0.x = asuint(_MainTex_TexelSize.y * asfloat(r0.x));
				b0.y = (_MainTex_TexelSize.y < 0.0);
				r0.z = asuint(1.0 + -i_TEXCOORD0.y);
				r1.x = b0.y ? r0.z : asuint(i_TEXCOORD0.y);
				r1.w = asuint(asfloat(r1.x) - asfloat(r0.x) * 1.38461542);
				r0.yw = asuint(float2(1.38461542, 3.23076916) * asfloat(r0.xx));
				r1.y = asuint(i_TEXCOORD0.x);
				float4 out0 = asfloat(r1.yxyw);
				o_TEXCOORD0 = out0.xy;
				o_TEXCOORD1 = out0.zw;
				r0.z = 0u;
				o_TEXCOORD2 = asfloat(r1.yx) + asfloat(r0.zy);
				o_TEXCOORD3 = asfloat(r1.yx) + -asfloat(r0.zw);
				o_TEXCOORD4 = asfloat(r1.yx) + asfloat(r0.zw);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float2 i_TEXCOORD2 : TEXCOORD2, float2 i_TEXCOORD3 : TEXCOORD3, float2 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD2.xy));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				r0 = asuint(float4(0.31621623, 0.31621623, 0.31621623, 0.31621623) * asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0 = asuint(asfloat(r1) * float4(0.227027029, 0.227027029, 0.227027029, 0.227027029) + asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD3.xy));
				r2 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD4.xy));
				r1 = asuint(asfloat(r2) + asfloat(r1));
				o_SV_Target0 = asfloat(r1) * float4(0.0702702701, 0.0702702701, 0.0702702701, 0.0702702701) + asfloat(r0);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _MainTex_TexelSize;
			float4 _GlobalsPad1[1];
			float _GlobalsPad2;
			float _GlobalsPad3;
			float _StereoRendering;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float2 o_TEXCOORD2 : TEXCOORD2, out float2 o_TEXCOORD3 : TEXCOORD3, out float2 o_TEXCOORD4 : TEXCOORD4)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				r0.x = 0x3f800000u;
				r0.z = asuint(_StereoRendering);
				r0.xy = asuint(_MainTex_TexelSize.xx * asfloat(r0.xz));
				r1.x = asuint(_StereoRendering);
				r1.z = 0xbf800000u;
				r0.xz = asuint(asfloat(r1.xz) * asfloat(r0.xy));
				b1.x = (_MainTex_TexelSize.y < 0.0);
				r1.y = asuint(1.0 + -i_TEXCOORD0.y);
				r1.y = b1.x ? r1.y : asuint(i_TEXCOORD0.y);
				r0.yw = asuint(_MainTex_TexelSize.yy);
				r1.x = asuint(i_TEXCOORD0.x);
				o_TEXCOORD1 = asfloat(r1.xy) + -asfloat(r0.xy);
				o_TEXCOORD0 = asfloat(r1.xy);
				o_TEXCOORD2 = asfloat(r1.xy) + -asfloat(r0.zw);
				o_TEXCOORD3 = asfloat(r1.xy) + asfloat(r0.zw);
				o_TEXCOORD4 = asfloat(r1.xy) + asfloat(r0.xw);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float2 i_TEXCOORD2 : TEXCOORD2, float2 i_TEXCOORD3 : TEXCOORD3, float2 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD1.xy));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD2.xy));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD3.xy));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD4.xy));
				r0 = asuint(asfloat(r1) + asfloat(r0));
				o_SV_Target0 = float4(0.25, 0.25, 0.25, 0.25) * asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
}
