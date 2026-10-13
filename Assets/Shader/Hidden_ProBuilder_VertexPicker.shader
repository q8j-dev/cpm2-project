Shader "Hidden/ProBuilder/VertexPicker"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "ALWAYS" "ProBuilderPicker" = "VertexPass" "RenderType" = "Transparent" }
		Pass
		{
			Name "Vertices"
			Cull Off
			ZTest LEqual
			ZWrite On
			Offset -1, -1
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			cbuffer UnityPerCamera : register(b0)
			{
				float4 _ScreenParams : packoffset(c6);
			}
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixV : packoffset(c9);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_COLOR0 : COLOR0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1.xyz = asuint(unity_MatrixV._m01_m11_m21 * asfloat(r0.yyy));
				r1.xyz = asuint(unity_MatrixV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_MatrixV._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_MatrixV._m03_m13_m23 * asfloat(r0.www) + asfloat(r0.xyz));
				r0.w = asuint(-glstate_matrix_projection._m33 + 1.0);
				r1.x = asuint(asfloat(r0.w) * -0.0400000215 + 0.99000001);
				r0.xyz = asuint(asfloat(r1.xxx) * asfloat(r0.xyz));
				r1 = asuint(glstate_matrix_projection._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(glstate_matrix_projection._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(glstate_matrix_projection._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r1 = asuint(glstate_matrix_projection._m03_m13_m23_m33 + asfloat(r1));
				r0.xy = asuint(asfloat(r1.xy) / asfloat(r1.ww));
				r0.xy = asuint(asfloat(r0.xy) * float2(0.5, 0.5) + float2(0.5, 0.5));
				r1.xy = asuint(float2(3.5, 3.5) * i_TEXCOORD1.xy);
				r0.xy = asuint(asfloat(r0.xy) * _ScreenParams.xy + asfloat(r1.xy));
				r0.xy = asuint(asfloat(r0.xy) / _ScreenParams.xy);
				r0.xy = asuint(float2(-0.5, -0.5) + asfloat(r0.xy));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r1.ww));
				o_SV_POSITION0.xy = asfloat(r0.xy) + asfloat(r0.xy);
				o_SV_POSITION0.z = asfloat(r1.z) - asfloat(r0.w) * 9.99999975e-05;
				o_SV_POSITION0.w = asfloat(r1.w);
				o_TEXCOORD0 = i_TEXCOORD0.xy;
				o_COLOR0 = i_COLOR0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				o_SV_Target0 = i_COLOR0.xyzw;
			}
			#endif
			ENDHLSL
		}
	}
}
