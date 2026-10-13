Shader "Hidden/Sentis/Gather"
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
			#pragma multi_compile __ GatherInt

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
			#if defined(GatherInt)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftB;
			int WidthMaskB;
			int StrideAxisX;
			int DimAxisX;
			int DimBlockedX;
			int StrideAxisB;
			int DimAxisB;
			int DimBlockedB;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			int endLength;
			int indicesLength;
			int axisDim;
			Texture2D<int4> Xptr : register(t0);
			Texture2D<int4> Bptr : register(t1);

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
				uint4 r9;
				uint4 r10;
				uint4 r11;
				bool4 b1;
				bool4 b2;
				bool4 b4;
				bool4 b5;
				bool4 b6;
				bool4 b8;
				bool4 b10;
				bool4 b11;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r1.x = r0.x % asuint(asfloat(StrideAxisO));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r2.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r2.x);
				r0.y = asuint(idx0 << 2);
				r2 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r2));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r1.xxxx));
				r1 = r0 / asuint(float4(asfloat(endLength), asfloat(endLength), asfloat(endLength), asfloat(endLength)));
				r2 = r0 % asuint(float4(asfloat(endLength), asfloat(endLength), asfloat(endLength), asfloat(endLength)));
				r1 = r1 % asuint(float4(asfloat(indicesLength), asfloat(indicesLength), asfloat(indicesLength), asfloat(indicesLength)));
				r1 = r1 / asuint(float4(asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB)));
				r3 = r1 % asuint(float4(asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB)));
				r1 = r1 / asuint(float4(asfloat(DimAxisB), asfloat(DimAxisB), asfloat(DimAxisB), asfloat(DimAxisB)));
				r4 = r1 % asuint(float4(asfloat(DimAxisB), asfloat(DimAxisB), asfloat(DimAxisB), asfloat(DimAxisB)));
				r5 = r4 >> 2;
				r1 = asuint(asint(float4(asfloat(DimBlockedB), asfloat(DimBlockedB), asfloat(DimBlockedB), asfloat(DimBlockedB))) * asint(r1) + asint(r5));
				r4 = uint4(3u, 3u, 3u, 3u) & r4;
				r1 = asuint(asint(float4(asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB))) * asint(r1) + asint(r3));
				r3 = asuint(float4(asfloat(WidthMaskB), asfloat(WidthMaskB), asfloat(WidthMaskB), asfloat(WidthMaskB))) & r1.zxyw;
				r1 = r1 >> asuint(float4(asfloat(WidthShiftB), asfloat(WidthShiftB), asfloat(WidthShiftB), asfloat(WidthShiftB)));
				r5.x = r3.w;
				r5.y = r1.w;
				r5.zw = uint2(0u, 0u);
				r5 = Bptr.Load(int3(asint(r5.xy), asint(r5.w)));
				b6 = (r4.zwww < uint4(3u, 1u, 2u, 3u));
				r8 = asuint(-asint(r4));
				r7.y = b6.z ? r8.w : 0u;
				r9 = asuint(int4(-3, -3, -3, -3) + asint(r4));
				r7.z = b6.z ? 0u : r9.w;
				r6.x = b6.x ? 0xffffffffu : 0u;
				r6.w = b6.w ? 0xffffffffu : 0u;
				b6.xw = (int2(0, 0) == asint(r6.wx));
				r6.x = b6.x ? 0xffffffffu : 0u;
				r6.y = b6.y ? 0xffffffffu : 0u;
				r7.xw = r6.yx;
				r5 = r7 & r5;
				r5.xy = r5.xz | r5.yw;
				r5.w = r5.x | r5.y;
				r7.x = r3.y;
				r7.y = r1.x;
				r7.zw = uint2(0u, 0u);
				r7 = Bptr.Load(int3(asint(r7.xy), asint(r7.w)));
				b10 = (r4.yxxx < uint4(1u, 1u, 2u, 3u));
				b4 = (r4.yyzz < uint4(2u, 3u, 1u, 2u));
				r11.y = b10.z ? r8.x : 0u;
				r8.xy = b4.xw ? r8.yz : uint2(0u, 0u);
				r11.z = b10.z ? 0u : r9.x;
				r4.xw = b4.xw ? uint2(0u, 0u) : r9.yz;
				r10.w = b10.w ? 0xffffffffu : 0u;
				b11.w = (0 == asint(r10.w));
				r10.y = b10.y ? 0xffffffffu : 0u;
				r11.x = r10.y;
				r11.w = b11.w ? 0xffffffffu : 0u;
				r7 = r11 & r7;
				r7.xy = r7.xz | r7.yw;
				r5.x = r7.x | r7.y;
				r1.x = r3.z;
				r3.y = r1.z;
				r3.zw = uint2(0u, 0u);
				r3 = Bptr.Load(int3(asint(r3.xy), asint(r3.w)));
				r4.y = b4.y ? 0xffffffffu : 0u;
				b10.w = (0 == asint(r4.y));
				r6.y = r8.y;
				r10.y = r8.x;
				r4.z = b4.z ? 0xffffffffu : 0u;
				r6.xz = r4.zw;
				r10.z = r4.x;
				r6.w = b6.w ? 0xffffffffu : 0u;
				r3 = r3 & r6;
				r3.xy = r3.xz | r3.yw;
				r5.z = r3.x | r3.y;
				r1.zw = uint2(0u, 0u);
				r1 = Bptr.Load(int3(asint(r1.xy), asint(r1.w)));
				r10.x = b10.x ? 0xffffffffu : 0u;
				r10.w = b10.w ? 0xffffffffu : 0u;
				r1 = r1 & r10;
				r1.xy = r1.xz | r1.yw;
				r5.y = r1.x | r1.y;
				b1 = (asint(r5) < int4(0, 0, 0, 0));
				r3 = asuint(asint(float4(asfloat(axisDim), asfloat(axisDim), asfloat(axisDim), asfloat(axisDim))) + asint(r5));
				r1 = b1 ? r3 : r5;
				r1 = asuint(asint(r1) * asint(float4(asfloat(endLength), asfloat(endLength), asfloat(endLength), asfloat(endLength))));
				r3.x = asuint(asint(asfloat(endLength)) * asint(asfloat(indicesLength)));
				r0 = r0 / r3.xxxx;
				r0 = asuint(asint(r0) * asint(float4(asfloat(endLength), asfloat(endLength), asfloat(endLength), asfloat(endLength))));
				r0 = asuint(asint(r0) * asint(float4(asfloat(axisDim), asfloat(axisDim), asfloat(axisDim), asfloat(axisDim))) + asint(r1));
				r0 = asuint(asint(r0) + asint(r2));
				r0 = r0 / asuint(float4(asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX)));
				r1 = r0 % asuint(float4(asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX)));
				r0 = r0 / asuint(float4(asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX)));
				r2 = r0 % asuint(float4(asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX)));
				r3 = r2 >> 2;
				r0 = asuint(asint(float4(asfloat(DimBlockedX), asfloat(DimBlockedX), asfloat(DimBlockedX), asfloat(DimBlockedX))) * asint(r0) + asint(r3));
				r2 = uint4(3u, 3u, 3u, 3u) & r2;
				r0 = asuint(asint(float4(asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX))) * asint(r0) + asint(r1));
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
			#elif 1
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftB;
			int WidthMaskB;
			int StrideAxisX;
			int DimAxisX;
			int DimBlockedX;
			int StrideAxisB;
			int DimAxisB;
			int DimBlockedB;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			int endLength;
			int indicesLength;
			int axisDim;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<int4> Bptr : register(t1);
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
				uint4 r4;
				uint4 r5;
				uint4 r6;
				uint4 r7;
				uint4 r8;
				uint4 r9;
				uint4 r10;
				uint4 r11;
				bool4 b1;
				bool4 b4;
				bool4 b6;
				bool4 b10;
				bool4 b11;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r1.x = r0.x % asuint(asfloat(StrideAxisO));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r2.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r2.x);
				r0.y = asuint(idx0 << 2);
				r2 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r2));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r1.xxxx));
				r1 = r0 / asuint(float4(asfloat(endLength), asfloat(endLength), asfloat(endLength), asfloat(endLength)));
				r2 = r0 % asuint(float4(asfloat(endLength), asfloat(endLength), asfloat(endLength), asfloat(endLength)));
				r1 = r1 % asuint(float4(asfloat(indicesLength), asfloat(indicesLength), asfloat(indicesLength), asfloat(indicesLength)));
				r1 = r1 / asuint(float4(asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB)));
				r3 = r1 % asuint(float4(asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB)));
				r1 = r1 / asuint(float4(asfloat(DimAxisB), asfloat(DimAxisB), asfloat(DimAxisB), asfloat(DimAxisB)));
				r4 = r1 % asuint(float4(asfloat(DimAxisB), asfloat(DimAxisB), asfloat(DimAxisB), asfloat(DimAxisB)));
				r5 = r4 >> 2;
				r1 = asuint(asint(float4(asfloat(DimBlockedB), asfloat(DimBlockedB), asfloat(DimBlockedB), asfloat(DimBlockedB))) * asint(r1) + asint(r5));
				r4 = uint4(3u, 3u, 3u, 3u) & r4;
				r1 = asuint(asint(float4(asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB), asfloat(StrideAxisB))) * asint(r1) + asint(r3));
				r3 = asuint(float4(asfloat(WidthMaskB), asfloat(WidthMaskB), asfloat(WidthMaskB), asfloat(WidthMaskB))) & r1.zxyw;
				r1 = r1 >> asuint(float4(asfloat(WidthShiftB), asfloat(WidthShiftB), asfloat(WidthShiftB), asfloat(WidthShiftB)));
				r5.x = r3.w;
				r5.y = r1.w;
				r5.zw = uint2(0u, 0u);
				r5 = Bptr.Load(int3(asint(r5.xy), asint(r5.w)));
				b6 = (r4.zwww < uint4(3u, 1u, 2u, 3u));
				r8 = asuint(-asint(r4));
				r7.y = b6.z ? r8.w : 0u;
				r9 = asuint(int4(-3, -3, -3, -3) + asint(r4));
				r7.z = b6.z ? 0u : r9.w;
				r6.x = b6.x ? 0xffffffffu : 0u;
				r6.w = b6.w ? 0xffffffffu : 0u;
				b6.xw = (int2(0, 0) == asint(r6.wx));
				r6.x = b6.x ? 0xffffffffu : 0u;
				r6.y = b6.y ? 0xffffffffu : 0u;
				r7.xw = r6.yx;
				r5 = r7 & r5;
				r5.xy = r5.xz | r5.yw;
				r5.w = r5.x | r5.y;
				r7.x = r3.y;
				r7.y = r1.x;
				r7.zw = uint2(0u, 0u);
				r7 = Bptr.Load(int3(asint(r7.xy), asint(r7.w)));
				b10 = (r4.yxxx < uint4(1u, 1u, 2u, 3u));
				b4 = (r4.yyzz < uint4(2u, 3u, 1u, 2u));
				r11.y = b10.z ? r8.x : 0u;
				r8.xy = b4.xw ? r8.yz : uint2(0u, 0u);
				r11.z = b10.z ? 0u : r9.x;
				r4.xw = b4.xw ? uint2(0u, 0u) : r9.yz;
				r10.w = b10.w ? 0xffffffffu : 0u;
				b11.w = (0 == asint(r10.w));
				r10.y = b10.y ? 0xffffffffu : 0u;
				r11.x = r10.y;
				r11.w = b11.w ? 0xffffffffu : 0u;
				r7 = r11 & r7;
				r7.xy = r7.xz | r7.yw;
				r5.x = r7.x | r7.y;
				r1.x = r3.z;
				r3.y = r1.z;
				r3.zw = uint2(0u, 0u);
				r3 = Bptr.Load(int3(asint(r3.xy), asint(r3.w)));
				r4.y = b4.y ? 0xffffffffu : 0u;
				b10.w = (0 == asint(r4.y));
				r6.y = r8.y;
				r10.y = r8.x;
				r4.z = b4.z ? 0xffffffffu : 0u;
				r6.xz = r4.zw;
				r10.z = r4.x;
				r6.w = b6.w ? 0xffffffffu : 0u;
				r3 = r3 & r6;
				r3.xy = r3.xz | r3.yw;
				r5.z = r3.x | r3.y;
				r1.zw = uint2(0u, 0u);
				r1 = Bptr.Load(int3(asint(r1.xy), asint(r1.w)));
				r10.x = b10.x ? 0xffffffffu : 0u;
				r10.w = b10.w ? 0xffffffffu : 0u;
				r1 = r1 & r10;
				r1.xy = r1.xz | r1.yw;
				r5.y = r1.x | r1.y;
				b1 = (asint(r5) < int4(0, 0, 0, 0));
				r3 = asuint(asint(float4(asfloat(axisDim), asfloat(axisDim), asfloat(axisDim), asfloat(axisDim))) + asint(r5));
				r1 = b1 ? r3 : r5;
				r1 = asuint(asint(r1) * asint(float4(asfloat(endLength), asfloat(endLength), asfloat(endLength), asfloat(endLength))));
				r3.x = asuint(asint(asfloat(endLength)) * asint(asfloat(indicesLength)));
				r0 = r0 / r3.xxxx;
				r0 = asuint(asint(r0) * asint(float4(asfloat(endLength), asfloat(endLength), asfloat(endLength), asfloat(endLength))));
				r0 = asuint(asint(r0) * asint(float4(asfloat(axisDim), asfloat(axisDim), asfloat(axisDim), asfloat(axisDim))) + asint(r1));
				r0 = asuint(asint(r0) + asint(r2));
				r0 = r0 / asuint(float4(asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX)));
				r1 = r0 % asuint(float4(asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX)));
				r0 = r0 / asuint(float4(asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX)));
				r2 = r0 % asuint(float4(asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX)));
				r3 = uint4(3u, 3u, 3u, 3u) & r2;
				r2 = r2 >> 2;
				r0 = asuint(asint(float4(asfloat(DimBlockedX), asfloat(DimBlockedX), asfloat(DimBlockedX), asfloat(DimBlockedX))) * asint(r0) + asint(r2));
				r0 = asuint(asint(float4(asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX))) * asint(r0) + asint(r1));
				r1 = asuint(float4(asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX))) & r0.zxyw;
				r0 = r0.xwyz >> asuint(float4(asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX)));
				r2.x = r1.y;
				r2.y = r0.x;
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				o_SV_Target0.x = dot(asfloat(r2), icb[asint(r3.x)]);
				r2.x = r1.z;
				r2.y = r0.z;
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				o_SV_Target0.y = dot(asfloat(r2), icb[asint(r3.y)]);
				r0.x = r1.w;
				r1.y = r0.w;
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				o_SV_Target0.z = dot(asfloat(r1), icb[asint(r3.z)]);
				r0.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r0.xy), asint(r0.w))));
				o_SV_Target0.w = dot(asfloat(r0), icb[asint(r3.w)]);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
