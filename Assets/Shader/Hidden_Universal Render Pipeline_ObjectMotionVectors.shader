Shader "Hidden/Universal Render Pipeline/ObjectMotionVectors"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			Name "Object Motion Vectors"
			Tags { "LIGHTMODE" = "MOTIONVECTORS" }
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			float4 _GlobalsPad1[7];
			float4x4 _PrevViewProjMatrix;
			float4x4 _NonJitteredViewProjMatrix;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_MatrixPreviousM : packoffset(c34);
				float4 unity_MotionVectorsParams : packoffset(c42);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_TEXCOORD4 : TEXCOORD4, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0));
				o_SV_POSITION0.z = asfloat(r0.z) - unity_MotionVectorsParams.z * asfloat(r0.w);
				o_SV_POSITION0.xyw = asfloat(r0.xyw);
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 * i_POSITION0.wwww + asfloat(r0));
				r1 = asuint(_NonJitteredViewProjMatrix._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(_NonJitteredViewProjMatrix._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(_NonJitteredViewProjMatrix._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_TEXCOORD0 = _NonJitteredViewProjMatrix._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				b0.x = (unity_MotionVectorsParams.x == 1.0);
				r1.xyz = asuint(i_TEXCOORD4.xyz);
				r1.w = 0x3f800000u;
				r0 = b0.xxxx ? r1 : asuint(i_POSITION0.xyzw);
				r1 = asuint(unity_MatrixPreviousM._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixPreviousM._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixPreviousM._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(unity_MatrixPreviousM._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1));
				r1 = asuint(_PrevViewProjMatrix._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(_PrevViewProjMatrix._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(_PrevViewProjMatrix._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_TEXCOORD1 = _PrevViewProjMatrix._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			cbuffer UnityPerDraw : register(b0)
			{
				float4 unity_MotionVectorsParams : packoffset(c42);
			}

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				bool4 b0;
				b0.x = (unity_MotionVectorsParams.y == 0.0);
				[branch] if (b0.x)
				{
					o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
					return;
				}
				r0.x = asuint(1.0 / i_TEXCOORD0.w);
				r0.y = asuint(1.0 / i_TEXCOORD1.w);
				r0.yz = asuint(i_TEXCOORD1.xy * asfloat(r0.yy));
				r0.xy = asuint(i_TEXCOORD0.xy * asfloat(r0.xx) + -asfloat(r0.yz));
				o_SV_Target0.xy = float2(0.5, -0.5) * asfloat(r0.xy);
				o_SV_Target0.zw = float2(0.0, 0.0);
			}
			#endif
			ENDHLSL
		}
	}
}
