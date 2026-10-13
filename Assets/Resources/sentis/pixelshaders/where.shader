Shader "Hidden/Sentis/Where"
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
			#pragma multi_compile __ WhereFloat
			#pragma multi_compile __ WhereInt

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
			#if defined(WhereFloat)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftA;
			int WidthMaskA;
			int WidthShiftB;
			int WidthMaskB;
			int WidthShiftX;
			int WidthMaskX;
			int DimO[8];
			float _GlobalsPad2;
			float _GlobalsPad3;
			float _GlobalsPad4;
			int StridesA[8];
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			int StridesB[8];
			float _GlobalsPad8;
			float _GlobalsPad9;
			float _GlobalsPad10;
			int StridesX[8];
			int DimAxisA;
			int DimAxisB;
			int DimAxisX;
			Texture2D<float4> Aptr : register(t0);
			Texture2D<float4> Bptr : register(t1);
			Texture2D<int4> Xptr : register(t2);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				uint4 r7;
				uint4 r8;
				bool4 b1;
				r0.zw = uint2(0u, 0u);
				r1.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r1.xy = uint2(asfloat(r1.xy));
				r1.y = asuint(asint(r1.y) << asint(asfloat(WidthShiftO)));
				r1.x = asuint(asint(r1.y) + asint(r1.x));
				r1.x = r1.x / asuint(asfloat(DimO[0]));
				r2.x = r1.x % asuint(asfloat(DimO[0]));
				r1.x = r1.x / asuint(asfloat(DimO[1]));
				r3.x = r1.x % asuint(asfloat(DimO[1]));
				r1.y = asuint(asint(r3.x) * asint(asfloat(StridesA[1])));
				r1.y = asuint(asint(r2.x) * asint(asfloat(StridesA[0])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[2]));
				r4.x = r1.x % asuint(asfloat(DimO[2]));
				r1.y = asuint(asint(r4.x) * asint(asfloat(StridesA[2])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[3]));
				r5.x = r1.x % asuint(asfloat(DimO[3]));
				r1.y = asuint(asint(r5.x) * asint(asfloat(StridesA[3])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[4]));
				r6.x = r1.x % asuint(asfloat(DimO[4]));
				r1.y = asuint(asint(r6.x) * asint(asfloat(StridesA[4])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[5]));
				r7.x = r1.x % asuint(asfloat(DimO[5]));
				r1.y = asuint(asint(r7.x) * asint(asfloat(StridesA[5])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[6]));
				r8.x = r1.x % asuint(asfloat(DimO[6]));
				r1.y = asuint(asint(r8.x) * asint(asfloat(StridesA[6])) + asint(r1.y));
				r1.x = r1.x % asuint(asfloat(DimO[7]));
				r1.y = asuint(asint(r1.x) * asint(asfloat(StridesA[7])) + asint(r1.y));
				r0.x = asuint(asfloat(WidthMaskA)) & r1.y;
				r0.y = r1.y >> asuint(asfloat(WidthShiftA));
				r0 = asuint(Aptr.Load(int3(asint(r0.xy), asint(r0.w))));
				b1.yzw = (int3(1, 1, 1) == asint(float3(asfloat(DimAxisA), asfloat(DimAxisB), asfloat(DimAxisX))));
				r0 = b1.yyyy ? r0.xxxx : r0;
				r1.y = asuint(asint(r3.x) * asint(asfloat(StridesB[1])));
				r2.y = asuint(asint(r3.x) * asint(asfloat(StridesX[1])));
				r2.y = asuint(asint(r2.x) * asint(asfloat(StridesX[0])) + asint(r2.y));
				r1.y = asuint(asint(r2.x) * asint(asfloat(StridesB[0])) + asint(r1.y));
				r1.y = asuint(asint(r4.x) * asint(asfloat(StridesB[2])) + asint(r1.y));
				r2.x = asuint(asint(r4.x) * asint(asfloat(StridesX[2])) + asint(r2.y));
				r2.x = asuint(asint(r5.x) * asint(asfloat(StridesX[3])) + asint(r2.x));
				r1.y = asuint(asint(r5.x) * asint(asfloat(StridesB[3])) + asint(r1.y));
				r1.y = asuint(asint(r6.x) * asint(asfloat(StridesB[4])) + asint(r1.y));
				r2.x = asuint(asint(r6.x) * asint(asfloat(StridesX[4])) + asint(r2.x));
				r2.x = asuint(asint(r7.x) * asint(asfloat(StridesX[5])) + asint(r2.x));
				r1.y = asuint(asint(r7.x) * asint(asfloat(StridesB[5])) + asint(r1.y));
				r1.y = asuint(asint(r8.x) * asint(asfloat(StridesB[6])) + asint(r1.y));
				r2.x = asuint(asint(r8.x) * asint(asfloat(StridesX[6])) + asint(r2.x));
				r2.x = asuint(asint(r1.x) * asint(asfloat(StridesX[7])) + asint(r2.x));
				r1.x = asuint(asint(r1.x) * asint(asfloat(StridesB[7])) + asint(r1.y));
				r3.x = asuint(asfloat(WidthMaskB)) & r1.x;
				r3.y = r1.x >> asuint(asfloat(WidthShiftB));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Bptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = b1.zzzz ? r3.xxxx : r3;
				r4.x = asuint(asfloat(WidthMaskX)) & r2.x;
				r4.y = r2.x >> asuint(asfloat(WidthShiftX));
				r4.zw = uint2(0u, 0u);
				r2 = Xptr.Load(int3(asint(r4.xy), asint(r4.w)));
				r1 = b1.wwww ? r2.xxxx : r2;
				o_SV_Target0 = (r1 != 0) ? asfloat(r0) : asfloat(r3);
			}
			#elif defined(WhereInt)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftA;
			int WidthMaskA;
			int WidthShiftB;
			int WidthMaskB;
			int WidthShiftX;
			int WidthMaskX;
			int DimO[8];
			float _GlobalsPad2;
			float _GlobalsPad3;
			float _GlobalsPad4;
			int StridesA[8];
			float _GlobalsPad5;
			float _GlobalsPad6;
			float _GlobalsPad7;
			int StridesB[8];
			float _GlobalsPad8;
			float _GlobalsPad9;
			float _GlobalsPad10;
			int StridesX[8];
			int DimAxisA;
			int DimAxisB;
			int DimAxisX;
			Texture2D<int4> Aptr : register(t0);
			Texture2D<int4> Bptr : register(t1);
			Texture2D<int4> Xptr : register(t2);

			void frag(float4 i_SV_Position0 : SV_Position, out int4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				uint4 r7;
				uint4 r8;
				bool4 b1;
				r0.zw = uint2(0u, 0u);
				r1.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r1.xy = uint2(asfloat(r1.xy));
				r1.y = asuint(asint(r1.y) << asint(asfloat(WidthShiftO)));
				r1.x = asuint(asint(r1.y) + asint(r1.x));
				r1.x = r1.x / asuint(asfloat(DimO[0]));
				r2.x = r1.x % asuint(asfloat(DimO[0]));
				r1.x = r1.x / asuint(asfloat(DimO[1]));
				r3.x = r1.x % asuint(asfloat(DimO[1]));
				r1.y = asuint(asint(r3.x) * asint(asfloat(StridesA[1])));
				r1.y = asuint(asint(r2.x) * asint(asfloat(StridesA[0])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[2]));
				r4.x = r1.x % asuint(asfloat(DimO[2]));
				r1.y = asuint(asint(r4.x) * asint(asfloat(StridesA[2])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[3]));
				r5.x = r1.x % asuint(asfloat(DimO[3]));
				r1.y = asuint(asint(r5.x) * asint(asfloat(StridesA[3])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[4]));
				r6.x = r1.x % asuint(asfloat(DimO[4]));
				r1.y = asuint(asint(r6.x) * asint(asfloat(StridesA[4])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[5]));
				r7.x = r1.x % asuint(asfloat(DimO[5]));
				r1.y = asuint(asint(r7.x) * asint(asfloat(StridesA[5])) + asint(r1.y));
				r1.x = r1.x / asuint(asfloat(DimO[6]));
				r8.x = r1.x % asuint(asfloat(DimO[6]));
				r1.y = asuint(asint(r8.x) * asint(asfloat(StridesA[6])) + asint(r1.y));
				r1.x = r1.x % asuint(asfloat(DimO[7]));
				r1.y = asuint(asint(r1.x) * asint(asfloat(StridesA[7])) + asint(r1.y));
				r0.x = asuint(asfloat(WidthMaskA)) & r1.y;
				r0.y = r1.y >> asuint(asfloat(WidthShiftA));
				r0 = Aptr.Load(int3(asint(r0.xy), asint(r0.w)));
				b1.yzw = (int3(1, 1, 1) == asint(float3(asfloat(DimAxisA), asfloat(DimAxisB), asfloat(DimAxisX))));
				r0 = b1.yyyy ? r0.xxxx : r0;
				r1.y = asuint(asint(r3.x) * asint(asfloat(StridesB[1])));
				r2.y = asuint(asint(r3.x) * asint(asfloat(StridesX[1])));
				r2.y = asuint(asint(r2.x) * asint(asfloat(StridesX[0])) + asint(r2.y));
				r1.y = asuint(asint(r2.x) * asint(asfloat(StridesB[0])) + asint(r1.y));
				r1.y = asuint(asint(r4.x) * asint(asfloat(StridesB[2])) + asint(r1.y));
				r2.x = asuint(asint(r4.x) * asint(asfloat(StridesX[2])) + asint(r2.y));
				r2.x = asuint(asint(r5.x) * asint(asfloat(StridesX[3])) + asint(r2.x));
				r1.y = asuint(asint(r5.x) * asint(asfloat(StridesB[3])) + asint(r1.y));
				r1.y = asuint(asint(r6.x) * asint(asfloat(StridesB[4])) + asint(r1.y));
				r2.x = asuint(asint(r6.x) * asint(asfloat(StridesX[4])) + asint(r2.x));
				r2.x = asuint(asint(r7.x) * asint(asfloat(StridesX[5])) + asint(r2.x));
				r1.y = asuint(asint(r7.x) * asint(asfloat(StridesB[5])) + asint(r1.y));
				r1.y = asuint(asint(r8.x) * asint(asfloat(StridesB[6])) + asint(r1.y));
				r2.x = asuint(asint(r8.x) * asint(asfloat(StridesX[6])) + asint(r2.x));
				r2.x = asuint(asint(r1.x) * asint(asfloat(StridesX[7])) + asint(r2.x));
				r1.x = asuint(asint(r1.x) * asint(asfloat(StridesB[7])) + asint(r1.y));
				r3.x = asuint(asfloat(WidthMaskB)) & r1.x;
				r3.y = r1.x >> asuint(asfloat(WidthShiftB));
				r3.zw = uint2(0u, 0u);
				r3 = Bptr.Load(int3(asint(r3.xy), asint(r3.w)));
				r3 = b1.zzzz ? r3.xxxx : r3;
				r4.x = asuint(asfloat(WidthMaskX)) & r2.x;
				r4.y = r2.x >> asuint(asfloat(WidthShiftX));
				r4.zw = uint2(0u, 0u);
				r2 = Xptr.Load(int3(asint(r4.xy), asint(r4.w)));
				r1 = b1.wwww ? r2.xxxx : r2;
				o_SV_Target0 = asint((r1 != 0) ? asfloat(r0) : asfloat(r3));
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
