Shader "HighlightPlus/Geometry/ComposeOutline"
{
	Properties
	{
		_MainTex ("Texture", any) = "black" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Cull ("Cull Mode", Float) = 2
		_ZTest ("ZTest Mode", Float) = 0
		_Flip ("Flip", Vector) = (0,1,0,1)
		_Debug ("Debug Color", Color) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "QUEUE" = "Transparent+120" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZTest Always
			ZWrite Off
			Stencil { Ref 2 ReadMask 255 WriteMask 255 Comp NotEqual Pass Keep Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[3];
			float3 _Flip;
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

			void vert(float4 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0)
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
				r0.y = asuint(_ProjectionParams.x * asfloat(r0.y));
				r1.xzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.xwy));
				r0.xy = asuint(asfloat(r1.xw) + asfloat(r1.zz));
				r0.y = asuint(_Flip.y * asfloat(r0.y));
				o_TEXCOORD0.y = asfloat(r0.w) * _Flip.x + asfloat(r0.y);
				o_TEXCOORD0.xzw = asfloat(r0.xzw);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[2];
			float4 _Color;
			float4 _GlobalsPad1[1];
			float4 _Debug;
			Texture2D<float4> _HPComposeOutlineFinal : register(t0);
			SamplerState sampler_HPComposeOutlineFinal : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.xy = asuint(i_TEXCOORD0.xy / i_TEXCOORD0.ww);
				r0 = asuint(_HPComposeOutlineFinal.Sample(sampler_HPComposeOutlineFinal, asfloat(r0.xy)));
				r0.w = asuint(saturate(_Color.w * asfloat(r0.w)));
				r0.xyz = asuint(saturate(_Color.xyz));
				o_SV_Target0 = _Debug.xyzw + asfloat(r0);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZTest Always
			ZWrite Off
			Stencil { Ref 2 ReadMask 255 WriteMask 255 Comp NotEqual Pass Keep Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _MainTex_ST;
			float4 _GlobalsPad1[1];
			float3 _Flip;
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
				r0.xy = asuint(i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw);
				r0.z = asuint(asfloat(r0.y) * _Flip.y + _Flip.x);
				o_TEXCOORD0 = asfloat(r0.xz);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[3];
			float4 _Color;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				o_SV_Target0.w = saturate(_Color.w * asfloat(r0.w));
				o_SV_Target0.xyz = saturate(_Color.xyz);
			}
			#endif
			ENDHLSL
		}
	}
}
