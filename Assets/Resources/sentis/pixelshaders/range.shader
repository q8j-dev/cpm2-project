Shader "Hidden/Sentis/Range"
{
	Properties
	{
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
			#pragma multi_compile __ INT

			#if defined(SHADER_STAGE_VERTEX)
			cbuffer UnityPerDraw : register(b0)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b1)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION)
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
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(INT)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int rangeStartInt;
			int rangeDeltaInt;

			void frag(float4 i_SV_Position0 : SV_Position, out int4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				int idx0 = asint(r0.x);
				r0.x = asuint(idx0 << 2);
				r0 = asuint(int4(0, 1, 2, 3) + asint(r0.xxxx));
				o_SV_Target0 = asint(asfloat(asint(float4(asfloat(rangeDeltaInt), asfloat(rangeDeltaInt), asfloat(rangeDeltaInt), asfloat(rangeDeltaInt))) * asint(r0) + asint(float4(asfloat(rangeStartInt), asfloat(rangeStartInt), asfloat(rangeStartInt), asfloat(rangeStartInt)))));
			}
			#elif 1
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			float rangeStartFloat;
			float rangeDeltaFloat;

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				int idx0 = asint(r0.x);
				r0.x = asuint(idx0 << 2);
				r0 = asuint(int4(0, 1, 2, 3) + asint(r0.xxxx));
				r0 = asuint(float4(r0));
				o_SV_Target0 = float4(rangeDeltaFloat, rangeDeltaFloat, rangeDeltaFloat, rangeDeltaFloat) * asfloat(r0) + float4(rangeStartFloat, rangeStartFloat, rangeStartFloat, rangeStartFloat);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
