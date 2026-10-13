Shader "Hidden/Sentis/Softmax"
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
			#pragma multi_compile __ LOGSOFTMAXEND
			#pragma multi_compile __ SOFTMAXEND

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
			#if defined(SOFTMAXEND)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int WidthShiftB;
			int WidthMaskB;
			int StrideAxisX;
			int DimAxisX;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);
			Texture2D<float4> Bptr : register(t2);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisX));
				r2.x = r0.x % asuint(asfloat(StrideAxisX));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.x = r0.x / asuint(asfloat(DimAxisX));
				r0.x = asuint(asint(asfloat(StrideAxisX)) * asint(r0.x) + asint(r2.x));
				r2.y = r0.x >> asuint(asfloat(WidthShiftB));
				r3.xy = asuint(float2(asfloat(WidthMaskS), asfloat(WidthMaskB))) & r0.xx;
				r2.x = r0.x >> asuint(asfloat(WidthShiftS));
				r3.z = r2.x;
				r3.w = 0u;
				r2.zw = r3.yw;
				r0 = asuint(Sptr.Load(int3(asint(r3.xz), asint(r3.w))));
				r2 = asuint(Bptr.Load(int3(asint(r2.zy), asint(r2.w))));
				r1 = asuint(-asfloat(r2) + asfloat(r1));
				r1 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r1));
				r1 = asuint(exp2(asfloat(r1)));
				r1 = asuint(asfloat(r1) / asfloat(r0));
				b0 = (float4(0.0, 0.0, 0.0, 0.0) == asfloat(r0));
				o_SV_Target0 = b0 ? float4(0.0, 0.0, 0.0, 0.0) : asfloat(r1);
			}
			#elif defined(LOGSOFTMAXEND)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int WidthShiftB;
			int WidthMaskB;
			int StrideAxisX;
			int DimAxisX;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);
			Texture2D<float4> Bptr : register(t2);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisX));
				r2.x = r0.x % asuint(asfloat(StrideAxisX));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.x = r0.x / asuint(asfloat(DimAxisX));
				r0.x = asuint(asint(asfloat(StrideAxisX)) * asint(r0.x) + asint(r2.x));
				r2.y = r0.x >> asuint(asfloat(WidthShiftB));
				r3.xy = asuint(float2(asfloat(WidthMaskS), asfloat(WidthMaskB))) & r0.xx;
				r2.x = r0.x >> asuint(asfloat(WidthShiftS));
				r3.z = r2.x;
				r3.w = 0u;
				r2.zw = r3.yw;
				r0 = asuint(Sptr.Load(int3(asint(r3.xz), asint(r3.w))));
				r0 = asuint(log2(asfloat(r0)));
				r2 = asuint(Bptr.Load(int3(asint(r2.zy), asint(r2.w))));
				r1 = asuint(-asfloat(r2) + asfloat(r1));
				o_SV_Target0 = asfloat(r1) - asfloat(r0) * float4(0.693147182, 0.693147182, 0.693147182, 0.693147182);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
