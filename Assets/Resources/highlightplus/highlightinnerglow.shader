Shader "HighlightPlus/Geometry/InnerGlow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Width ("Width", Float) = 1
		_CutOff ("CutOff", Float) = 0.5
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "QUEUE" = "Transparent+122" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha One, SrcAlpha One
			Cull Back
			ZTest [_InnerGlowZTest]
			ZWrite Off
			Offset -1, -1
			Stencil { Ref 2 ReadMask 255 WriteMask 255 Comp Equal Pass Keep Fail Keep ZFail Keep }
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
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, float3 i_NORMAL0 : NORMAL0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float3 o_TEXCOORD1 : TEXCOORD1, out float3 o_NORMAL0 : NORMAL0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r1 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				o_TEXCOORD1 = unity_ObjectToWorld._m03_m13_m23 * i_POSITION0.www + asfloat(r0.xyz);
				r0 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r1.yyyy));
				r0 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r1.xxxx) + asfloat(r0));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r1.zzzz) + asfloat(r0));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r1.wwww) + asfloat(r0);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_NORMAL0 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(HP_ALPHACLIP)
			float4 _GlobalsPad0[3];
			float _CutOff;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _Color;
			float _Width;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_NORMAL0 : NORMAL0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.x = asuint(-_CutOff + asfloat(r0.w));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				r0.xyz = asuint(-_WorldSpaceCameraPos.xyz + i_TEXCOORD1.xyz);
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				r0.w = asuint(dot(i_NORMAL0.xyz, i_NORMAL0.xyz));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.xyz = asuint(i_NORMAL0.xyz * asfloat(r0.www));
				r0.x = asuint(dot(asfloat(r0.xyz), asfloat(r1.xyz)));
				r0.x = asuint(saturate(_Width + -abs(asfloat(r0.x))));
				r0.x = asuint(asfloat(r0.x) / _Width);
				o_SV_Target0 = _Color.xyzw * asfloat(r0.xxxx);
			}
			#elif 1
			float4 _GlobalsPad0[4];
			float4 _Color;
			float _Width;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
			}

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float3 i_NORMAL0 : NORMAL0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(-_WorldSpaceCameraPos.xyz + i_TEXCOORD1.xyz);
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				r0.w = asuint(dot(i_NORMAL0.xyz, i_NORMAL0.xyz));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.xyz = asuint(i_NORMAL0.xyz * asfloat(r0.www));
				r0.x = asuint(dot(asfloat(r0.xyz), asfloat(r1.xyz)));
				r0.x = asuint(saturate(_Width + -abs(asfloat(r0.x))));
				r0.x = asuint(asfloat(r0.x) / _Width);
				o_SV_Target0 = _Color.xyzw * asfloat(r0.xxxx);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
