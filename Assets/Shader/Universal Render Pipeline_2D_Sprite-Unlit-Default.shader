Shader "Universal Render Pipeline/2D/Sprite-Unlit-Default"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		[HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
		[HideInInspector] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[HideInInspector] _AlphaTex ("External Alpha", 2D) = "white" {}
		[HideInInspector] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
	}
	SubShader
	{
		Tags { "QUEUE" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" }
		Pass
		{
			Tags { "LIGHTMODE" = "Universal2D" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			float4 _GlobalsPad1[47];
			float4 _MainTex_ST;
			float4 _Color;
			float4 _RendererColor;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}

			void vert(float3 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0)
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
				r0 = asuint(_Color.xyzw * i_COLOR0.xyzw);
				o_COLOR0 = _RendererColor.xyzw * asfloat(r0);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0 = asuint(_MainTex.SampleBias(sampler_MainTex, i_TEXCOORD0.xy, _GlobalMipBias.x));
				o_SV_Target0 = i_COLOR0.xyzw * asfloat(r0);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Tags { "LIGHTMODE" = "UniversalForward" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			float4 _GlobalsPad1[47];
			float4 _MainTex_ST;
			float4 _Color;
			float4 _RendererColor;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}

			void vert(float3 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0)
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
				r0 = asuint(_Color.xyzw * i_COLOR0.xyzw);
				o_COLOR0 = _RendererColor.xyzw * asfloat(r0);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0 = asuint(_MainTex.SampleBias(sampler_MainTex, i_TEXCOORD0.xy, _GlobalMipBias.x));
				o_SV_Target0 = i_COLOR0.xyzw * asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Sprites/Default"
}
