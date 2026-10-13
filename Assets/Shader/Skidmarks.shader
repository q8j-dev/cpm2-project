Shader "Skidmarks"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "QUEUE" = "Geometry+5" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull Back
			ZTest LEqual
			ZWrite Off
			Offset -4, -4
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
			float4 _Color;
			cbuffer UnityFog : register(b1)
			{
				float4 unity_FogColor : packoffset(c0);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float i_TEXCOORD2 : TEXCOORD2, float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.w = asuint(i_COLOR0.w * asfloat(r0.w));
				r0.xyz = asuint(_Color.xyz * asfloat(r0.xyz) + -unity_FogColor.xyz);
				o_SV_Target0.xyz = float3(i_TEXCOORD2, i_TEXCOORD2, i_TEXCOORD2) * asfloat(r0.xyz) + unity_FogColor.xyz;
				r0.x = asuint(_Color.w * asfloat(r0.w));
				b0.y = (0.0 >= asfloat(r0.x));
				o_SV_Target0.w = asfloat(r0.x);
				if (b0.y) discard;
			}
			#elif 1
			float4 _GlobalsPad0[3];
			float4 _Color;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.w = asuint(i_COLOR0.w * asfloat(r0.w));
				r0 = asuint(_Color.xyzw * asfloat(r0));
				b1.x = (0.0 >= asfloat(r0.w));
				o_SV_Target0 = asfloat(r0);
				if (b1.x) discard;
			}
			#endif
			#endif
			ENDHLSL
		}
	}
	Fallback "Transparent/VertexLit"
}
