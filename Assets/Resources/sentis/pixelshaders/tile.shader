Shader "Hidden/Sentis/Tile"
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
			int DimO[8];
			float _GlobalsPad2;
			float _GlobalsPad3;
			float _GlobalsPad4;
			int DimX[8];
			Texture2D<int4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out int4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(DimO[0]));
				r1.x = r0.x % asuint(asfloat(DimO[0]));
				r0.y = r1.x % asuint(asfloat(DimX[0]));
				r0.x = r0.x / asuint(asfloat(DimO[1]));
				r1.x = r0.x % asuint(asfloat(DimO[1]));
				r0.z = r1.x % asuint(asfloat(DimX[1]));
				r0.x = r0.x / asuint(asfloat(DimO[2]));
				r1.x = r0.x % asuint(asfloat(DimO[2]));
				r0.y = asuint(asint(r0.z) * asint(asfloat(DimX[0])) + asint(r0.y));
				r0.z = r1.x % asuint(asfloat(DimX[2]));
				r0.x = r0.x / asuint(asfloat(DimO[3]));
				r1.x = r0.x % asuint(asfloat(DimO[3]));
				r0.w = asuint(asint(asfloat(DimX[0])) * asint(asfloat(DimX[1])));
				r0.y = asuint(asint(r0.z) * asint(r0.w) + asint(r0.y));
				r0.z = asuint(asint(r0.w) * asint(asfloat(DimX[2])));
				r0.w = r1.x % asuint(asfloat(DimX[3]));
				r0.x = r0.x / asuint(asfloat(DimO[4]));
				r1.x = r0.x % asuint(asfloat(DimO[4]));
				r0.y = asuint(asint(r0.w) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(r0.z) * asint(asfloat(DimX[3])));
				r0.w = r1.x % asuint(asfloat(DimX[4]));
				r0.x = r0.x / asuint(asfloat(DimO[5]));
				r1.x = r0.x % asuint(asfloat(DimO[5]));
				r0.y = asuint(asint(r0.w) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(r0.z) * asint(asfloat(DimX[4])));
				r0.w = r1.x % asuint(asfloat(DimX[5]));
				r0.x = r0.x / asuint(asfloat(DimO[6]));
				r1.x = r0.x % asuint(asfloat(DimO[6]));
				r0.y = asuint(asint(r0.w) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(r0.z) * asint(asfloat(DimX[5])));
				r0.w = r1.x % asuint(asfloat(DimX[6]));
				r0.x = r0.x % asuint(asfloat(DimO[7]));
				r0.x = r0.x % asuint(asfloat(DimX[7]));
				r0.y = asuint(asint(r0.w) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(r0.z) * asint(asfloat(DimX[6])));
				r0.x = asuint(asint(r0.x) * asint(r0.z) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = asint(asfloat(Xptr.Load(int3(asint(r1.xy), asint(r1.w)))));
			}
			#elif 1
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int DimO[8];
			float _GlobalsPad2;
			float _GlobalsPad3;
			float _GlobalsPad4;
			int DimX[8];
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(DimO[0]));
				r1.x = r0.x % asuint(asfloat(DimO[0]));
				r0.y = r1.x % asuint(asfloat(DimX[0]));
				r0.x = r0.x / asuint(asfloat(DimO[1]));
				r1.x = r0.x % asuint(asfloat(DimO[1]));
				r0.z = r1.x % asuint(asfloat(DimX[1]));
				r0.x = r0.x / asuint(asfloat(DimO[2]));
				r1.x = r0.x % asuint(asfloat(DimO[2]));
				r0.y = asuint(asint(r0.z) * asint(asfloat(DimX[0])) + asint(r0.y));
				r0.z = r1.x % asuint(asfloat(DimX[2]));
				r0.x = r0.x / asuint(asfloat(DimO[3]));
				r1.x = r0.x % asuint(asfloat(DimO[3]));
				r0.w = asuint(asint(asfloat(DimX[0])) * asint(asfloat(DimX[1])));
				r0.y = asuint(asint(r0.z) * asint(r0.w) + asint(r0.y));
				r0.z = asuint(asint(r0.w) * asint(asfloat(DimX[2])));
				r0.w = r1.x % asuint(asfloat(DimX[3]));
				r0.x = r0.x / asuint(asfloat(DimO[4]));
				r1.x = r0.x % asuint(asfloat(DimO[4]));
				r0.y = asuint(asint(r0.w) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(r0.z) * asint(asfloat(DimX[3])));
				r0.w = r1.x % asuint(asfloat(DimX[4]));
				r0.x = r0.x / asuint(asfloat(DimO[5]));
				r1.x = r0.x % asuint(asfloat(DimO[5]));
				r0.y = asuint(asint(r0.w) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(r0.z) * asint(asfloat(DimX[4])));
				r0.w = r1.x % asuint(asfloat(DimX[5]));
				r0.x = r0.x / asuint(asfloat(DimO[6]));
				r1.x = r0.x % asuint(asfloat(DimO[6]));
				r0.y = asuint(asint(r0.w) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(r0.z) * asint(asfloat(DimX[5])));
				r0.w = r1.x % asuint(asfloat(DimX[6]));
				r0.x = r0.x % asuint(asfloat(DimO[7]));
				r0.x = r0.x % asuint(asfloat(DimX[7]));
				r0.y = asuint(asint(r0.w) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(r0.z) * asint(asfloat(DimX[6])));
				r0.x = asuint(asint(r0.x) * asint(r0.z) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
