Shader "Hidden/Sentis/Trilu"
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
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			int width;
			int height;
			int direction;
			int offset;
			Texture2D<int4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out int4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.x = r1.x / asuint(asfloat(DimBlockedO));
				r3.x = r1.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r1 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r1.xxxx) + asint(r3));
				r1 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r1) + asint(r2.xxxx));
				r1 = r1 / asuint(float4(asfloat(width), asfloat(width), asfloat(width), asfloat(width)));
				r2 = r1 % asuint(float4(asfloat(width), asfloat(width), asfloat(width), asfloat(width)));
				r1 = r1 % asuint(float4(asfloat(height), asfloat(height), asfloat(height), asfloat(height)));
				r1 = asuint(asint(r2) + -asint(r1));
				r1 = asuint(asint(r1) * asint(float4(asfloat(direction), asfloat(direction), asfloat(direction), asfloat(direction))));
				r0.y = asuint(asint(asfloat(offset)) * asint(asfloat(direction)));
				b1 = (asint(r1) >= asint(r0.yyyy));
				r1 = b1 ? uint4(1u, 1u, 1u, 1u) : uint4(0u, 0u, 0u, 0u);
				r2.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r2.y = r0.x >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r0 = Xptr.Load(int3(asint(r2.xy), asint(r2.w)));
				o_SV_Target0 = asint(asfloat(asint(r0) * asint(r1)));
			}
			#elif 1
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			int width;
			int height;
			int direction;
			int offset;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.x = r1.x / asuint(asfloat(DimBlockedO));
				r3.x = r1.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r1 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r1.xxxx) + asint(r3));
				r1 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r1) + asint(r2.xxxx));
				r1 = r1 / asuint(float4(asfloat(width), asfloat(width), asfloat(width), asfloat(width)));
				r2 = r1 % asuint(float4(asfloat(width), asfloat(width), asfloat(width), asfloat(width)));
				r1 = r1 % asuint(float4(asfloat(height), asfloat(height), asfloat(height), asfloat(height)));
				r1 = asuint(asint(r2) + -asint(r1));
				r1 = asuint(asint(r1) * asint(float4(asfloat(direction), asfloat(direction), asfloat(direction), asfloat(direction))));
				r0.y = asuint(asint(asfloat(offset)) * asint(asfloat(direction)));
				b1 = (asint(r1) >= asint(r0.yyyy));
				r1 = b1 ? uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u) : uint4(0u, 0u, 0u, 0u);
				r2.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r2.y = r0.x >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				o_SV_Target0 = asfloat(r1) * asfloat(r0);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
