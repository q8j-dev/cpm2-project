Shader "HighlightPlus/Geometry/Outline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth ("Outline Offset", Float) = 0.01
		_OutlineDirection ("Outline Direction", Vector) = (0,0,0,1)
		_Color ("Color", Color) = (1,1,1,1)
		_Cull ("Cull Mode", Float) = 2
		_ConstantWidth ("Constant Width", Float) = 1
		_OutlineZTest ("ZTest", Float) = 4
	}
	SubShader
	{
		Tags { "DisableBatching" = "true" "QUEUE" = "Transparent+120" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			Cull [_Cull]
			ZTest [_OutlineZTest]
			ZWrite On
			Stencil { Ref 2 ReadMask 255 WriteMask 255 Comp NotEqual Pass Replace Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[3];
			float _OutlineWidth;
			float _ConstantWidth;
			cbuffer UnityPerCamera : register(b1)
			{
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
				float4 _OutlineDirection : packoffset(c0);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, out float4 o_SV_POSITION0 : SV_POSITION)
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
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.zw = asuint(_OutlineDirection.xy * asfloat(r0.zz));
				r0.xy = asuint(asfloat(r0.xy) * float2(_OutlineWidth, _OutlineWidth) + asfloat(r0.zw));
				o_SV_POSITION0.xy = asfloat(r1.xy) + asfloat(r0.xy);
				o_SV_POSITION0.zw = asfloat(r1.zw);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[2];
			float4 _OutlineColor;

			void frag(float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0)
			{
				o_SV_Target0 = _OutlineColor.xyzw;
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			ColorMask 0
			Cull [_Cull]
			ZTest Always
			ZWrite Off
			Stencil { Ref 2 ReadMask 255 WriteMask 255 Comp Always Pass Zero Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[3];
			float _OutlineWidth;
			float2 _OutlineDirection;
			float _ConstantWidth;
			cbuffer UnityPerCamera : register(b1)
			{
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

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, out float4 o_SV_POSITION0 : SV_POSITION)
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
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zz));
				r0.zw = asuint(_OutlineDirection.xy * asfloat(r0.zz));
				r0.xy = asuint(asfloat(r0.xy) * float2(_OutlineWidth, _OutlineWidth) + asfloat(r0.zw));
				o_SV_POSITION0.xy = asfloat(r1.xy) + asfloat(r0.xy);
				o_SV_POSITION0.zw = asfloat(r1.zw);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, out float4 o_SV_Target0 : SV_Target0)
			{
				o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#endif
			ENDHLSL
		}
	}
}
