Shader "HighlightPlus/Geometry/Glow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Glow ("Glow", Vector) = (1,0.025,0.75,0.5)
		_Glow2 ("Glow2", Vector) = (0.01,1,0.5,0)
		_GlowColor ("Glow Color", Color) = (1,1,1,1)
		_Color ("Color", Color) = (1,1,1,1)
		_GlowDirection ("GlowDir", Vector) = (1,1,0,1)
		_Cull ("Cull Mode", Float) = 2
		_ConstantWidth ("Constant Width", Float) = 1
		_GlowZTest ("ZTest", Float) = 4
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "QUEUE" = "Transparent+102" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull [_Cull]
			ZTest [_GlowZTest]
			ZWrite Off
			Stencil { Ref 2 ReadMask 255 WriteMask 255 Comp NotEqual Pass Keep Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float3 _Glow2;
			float _ConstantWidth;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _Time : packoffset(c0);
				float4 _ProjectionParams : packoffset(c5);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixInvV : packoffset(c13);
				float4x4 unity_MatrixVP : packoffset(c17);
			}
			cbuffer Props : register(b4)
			{
				float4 _GlowColor : packoffset(c0);
				float4 _Glow : packoffset(c1);
				float4 _GlowDirection : packoffset(c2);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xyz = asuint(unity_MatrixInvV._m10_m10_m10 * unity_WorldToObject._m01_m11_m21);
				r0.xyz = asuint(unity_WorldToObject._m00_m10_m20 * unity_MatrixInvV._m00_m00_m00 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * unity_MatrixInvV._m20_m20_m20 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 * unity_MatrixInvV._m30_m30_m30 + asfloat(r0.xyz));
				r0.x = asuint(dot(asfloat(r0.xyz), i_NORMAL0.xyz));
				r1.xyz = asuint(unity_MatrixInvV._m11_m11_m11 * unity_WorldToObject._m01_m11_m21);
				r1.xyz = asuint(unity_WorldToObject._m00_m10_m20 * unity_MatrixInvV._m01_m01_m01 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_WorldToObject._m02_m12_m22 * unity_MatrixInvV._m21_m21_m21 + asfloat(r1.xyz));
				r1.xyz = asuint(unity_WorldToObject._m03_m13_m23 * unity_MatrixInvV._m31_m31_m31 + asfloat(r1.xyz));
				r0.y = asuint(dot(asfloat(r1.xyz), i_NORMAL0.xyz));
				r0.z = asuint(dot(asfloat(r0.xy), asfloat(r0.xy)));
				r0.z = asuint(rsqrt(asfloat(r0.z)));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.yz = asuint(glstate_matrix_projection._m01_m11 * asfloat(r0.yy));
				r0.xy = asuint(glstate_matrix_projection._m00_m10 * asfloat(r0.xx) + asfloat(r0.yz));
				r0.xy = asuint(_GlowDirection.xy + asfloat(r0.xy));
				r1 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r1 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r1));
				r1 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r1));
				r2 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r1.yyyy));
				r2 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r1.xxxx) + asfloat(r2));
				r2 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r1.zzzz) + asfloat(r2));
				r1 = asuint(unity_MatrixVP._m03_m13_m23_m33 * asfloat(r1.wwww) + asfloat(r2));
				r0.z = asuint(asfloat(r1.z) / _ProjectionParams.y);
				r0.z = asuint(1.0 + -asfloat(r0.z));
				r0.z = asuint(_ProjectionParams.z * asfloat(r0.z));
				r0.z = asuint(max(0.0, asfloat(r0.z)));
				r0.w = asuint(2.0 + -asfloat(r0.z));
				r0.z = asuint(glstate_matrix_projection._m33 * asfloat(r0.w) + asfloat(r0.z));
				r0.z = asuint(-2.0 + asfloat(r0.z));
				r0.z = asuint(_ConstantWidth * asfloat(r0.z) + 2.0);
				r0.w = asuint(_Time.w * _Glow2.y);
				r0.w = asuint(sin(asfloat(r0.w)));
				r0.w = asuint(asfloat(r0.w) * 0.25 + 1.0);
				r0.w = asuint(_Glow.y * asfloat(r0.w) + _Glow2.x);
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.w));
				o_SV_POSITION0.xy = asfloat(r0.xy) * asfloat(r0.zz) + asfloat(r1.xy);
				o_SV_POSITION0.zw = asfloat(r1.zw);
				o_COLOR0.xyz = _GlowColor.xyz;
				o_COLOR0.w = _Glow.x;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[2];
			float3 _Glow2;
			cbuffer Props : register(b1)
			{
				float4 _Glow : packoffset(c1);
			}

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.xy = asuint(_Glow.zz * i_SV_POSITION0.xy);
				r0.xy = asuint(floor(asfloat(r0.xy)));
				r0.xy = asuint(_Glow.ww * asfloat(r0.xy));
				r0.x = asuint(asfloat(r0.x) + asfloat(r0.y));
				r0.x = asuint(frac(asfloat(r0.x)));
				r0.x = asuint(saturate(_Glow2.z + asfloat(r0.x)));
				o_SV_Target0.w = i_COLOR0.w * asfloat(r0.x);
				o_SV_Target0.xyz = i_COLOR0.xyz;
			}
			#endif
			ENDHLSL
		}
	}
}
