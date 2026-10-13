Shader "Hidden/Sentis/Split"
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
			#pragma multi_compile __ BLOCKWISE
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
			#if defined(BLOCKWISE) && defined(INT)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxis;
			int DimAxisX;
			int SplitStart;
			int SplitLength;
			Texture2D<int4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out int4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(StrideAxis));
				r1.x = r0.x % asuint(asfloat(StrideAxis));
				r0.x = r0.x / asuint(asfloat(SplitLength));
				r2.x = r0.x % asuint(asfloat(SplitLength));
				r0.y = asuint(asint(asfloat(SplitStart)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(DimAxisX)) * asint(r0.x) + asint(r0.y));
				r0.x = asuint(asint(asfloat(StrideAxis)) * asint(r0.x) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = asint(asfloat(Xptr.Load(int3(asint(r1.xy), asint(r1.w)))));
			}
			#elif defined(INT)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxis;
			int DimAxisX;
			int SplitStart;
			int SplitLength;
			Texture2D<int4> Xptr : register(t0);

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
				bool4 b2;
				bool4 b4;
				bool4 b5;
				bool4 b8;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(StrideAxis));
				r1.x = r0.x % asuint(asfloat(StrideAxis));
				r0.x = r0.x / asuint(asfloat(SplitLength));
				r2.x = r0.x % asuint(asfloat(SplitLength));
				int idx0 = asint(r2.x);
				r0.y = asuint(idx0 << 2);
				r2 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r2 = asuint(asint(float4(asfloat(SplitStart), asfloat(SplitStart), asfloat(SplitStart), asfloat(SplitStart))) + asint(r2));
				r3 = r2 >> 2;
				r2 = uint4(3u, 3u, 3u, 3u) & r2;
				r0 = asuint(asint(float4(asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX))) * asint(r0.xxxx) + asint(r3));
				r0 = asuint(asint(float4(asfloat(StrideAxis), asfloat(StrideAxis), asfloat(StrideAxis), asfloat(StrideAxis))) * asint(r0) + asint(r1.xxxx));
				r1 = asuint(float4(asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX))) & r0.zxyw;
				r0 = r0 >> asuint(float4(asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX)));
				r3.x = r1.w;
				r3.y = r0.w;
				r3.zw = uint2(0u, 0u);
				r3 = Xptr.Load(int3(asint(r3.xy), asint(r3.w)));
				b4 = (r2.zwww < uint4(3u, 1u, 2u, 3u));
				r6 = asuint(-asint(r2));
				r5.y = b4.z ? r6.w : 0u;
				r7 = asuint(int4(-3, -3, -3, -3) + asint(r2));
				r5.z = b4.z ? 0u : r7.w;
				r4.x = b4.x ? 0xffffffffu : 0u;
				r4.w = b4.w ? 0xffffffffu : 0u;
				b4.xw = (int2(0, 0) == asint(r4.wx));
				r4.x = b4.x ? 0xffffffffu : 0u;
				r4.y = b4.y ? 0xffffffffu : 0u;
				r5.xw = r4.yx;
				r3 = r5 & r3;
				r3.xy = r3.xz | r3.yw;
				o_SV_Target0.w = asint(asfloat(r3.x | r3.y));
				r3.x = r1.y;
				r3.y = r0.x;
				r3.zw = uint2(0u, 0u);
				r3 = Xptr.Load(int3(asint(r3.xy), asint(r3.w)));
				b5 = (r2.yxxx < uint4(1u, 1u, 2u, 3u));
				b2 = (r2.yyzz < uint4(2u, 3u, 1u, 2u));
				r8.y = b5.z ? r6.x : 0u;
				r6.xy = b2.xw ? r6.yz : uint2(0u, 0u);
				r8.z = b5.z ? 0u : r7.x;
				r2.xw = b2.xw ? uint2(0u, 0u) : r7.yz;
				r5.w = b5.w ? 0xffffffffu : 0u;
				b8.w = (0 == asint(r5.w));
				r5.y = b5.y ? 0xffffffffu : 0u;
				r8.x = r5.y;
				r8.w = b8.w ? 0xffffffffu : 0u;
				r3 = r8 & r3;
				r3.xy = r3.xz | r3.yw;
				o_SV_Target0.x = asint(asfloat(r3.x | r3.y));
				r0.x = r1.z;
				r1.y = r0.z;
				r1.zw = uint2(0u, 0u);
				r1 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
				r2.y = b2.y ? 0xffffffffu : 0u;
				b5.w = (0 == asint(r2.y));
				r4.y = r6.y;
				r5.y = r6.x;
				r2.z = b2.z ? 0xffffffffu : 0u;
				r4.xz = r2.zw;
				r5.z = r2.x;
				r4.w = b4.w ? 0xffffffffu : 0u;
				r1 = r1 & r4;
				r1.xy = r1.xz | r1.yw;
				o_SV_Target0.z = asint(asfloat(r1.x | r1.y));
				r0.zw = uint2(0u, 0u);
				r0 = Xptr.Load(int3(asint(r0.xy), asint(r0.w)));
				r5.x = b5.x ? 0xffffffffu : 0u;
				r5.w = b5.w ? 0xffffffffu : 0u;
				r0 = r0 & r5;
				r0.xy = r0.xz | r0.yw;
				o_SV_Target0.y = asint(asfloat(r0.x | r0.y));
			}
			#elif defined(BLOCKWISE)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxis;
			int DimAxisX;
			int SplitStart;
			int SplitLength;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(StrideAxis));
				r1.x = r0.x % asuint(asfloat(StrideAxis));
				r0.x = r0.x / asuint(asfloat(SplitLength));
				r2.x = r0.x % asuint(asfloat(SplitLength));
				r0.y = asuint(asint(asfloat(SplitStart)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(DimAxisX)) * asint(r0.x) + asint(r0.y));
				r0.x = asuint(asint(asfloat(StrideAxis)) * asint(r0.x) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif 1
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxis;
			int DimAxisX;
			int SplitStart;
			int SplitLength;
			Texture2D<float4> Xptr : register(t0);
			static const float4 icb[4] =
			{
				float4(1.0, 0.0, 0.0, 0.0),
				float4(0.0, 1.0, 0.0, 0.0),
				float4(0.0, 0.0, 1.0, 0.0),
				float4(0.0, 0.0, 0.0, 1.0)
			};

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
				r0.x = r0.x / asuint(asfloat(StrideAxis));
				r1.x = r0.x % asuint(asfloat(StrideAxis));
				r0.x = r0.x / asuint(asfloat(SplitLength));
				r2.x = r0.x % asuint(asfloat(SplitLength));
				int idx0 = asint(r2.x);
				r0.y = asuint(idx0 << 2);
				r2 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r2 = asuint(asint(float4(asfloat(SplitStart), asfloat(SplitStart), asfloat(SplitStart), asfloat(SplitStart))) + asint(r2));
				r3 = r2 >> 2;
				r2 = uint4(3u, 3u, 3u, 3u) & r2;
				r0 = asuint(asint(float4(asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX))) * asint(r0.xxxx) + asint(r3));
				r0 = asuint(asint(float4(asfloat(StrideAxis), asfloat(StrideAxis), asfloat(StrideAxis), asfloat(StrideAxis))) * asint(r0) + asint(r1.xxxx));
				r1 = asuint(float4(asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX))) & r0.zxyw;
				r0 = r0.xwyz >> asuint(float4(asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX)));
				r3.x = r1.y;
				r3.y = r0.x;
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				o_SV_Target0.x = dot(asfloat(r3), icb[asint(r2.x)]);
				r3.x = r1.z;
				r3.y = r0.z;
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				o_SV_Target0.y = dot(asfloat(r3), icb[asint(r2.y)]);
				r0.x = r1.w;
				r1.y = r0.w;
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				o_SV_Target0.z = dot(asfloat(r1), icb[asint(r2.z)]);
				r0.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r0.xy), asint(r0.w))));
				o_SV_Target0.w = dot(asfloat(r0), icb[asint(r2.w)]);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
