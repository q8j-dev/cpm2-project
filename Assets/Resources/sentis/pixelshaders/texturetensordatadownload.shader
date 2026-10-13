Shader "Hidden/Sentis/TextureTensorDataDownload"
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
			#pragma multi_compile __ TensorFloat
			#pragma multi_compile __ TensorInt

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
			#if defined(TensorFloat)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxisX;
			int DimAxisX;
			int DimBlockedX;
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
				int idx0 = asint(r0.x);
				r0.x = asuint(idx0 << 2);
				r0 = asuint(int4(0, 1, 2, 3) + asint(r0.xxxx));
				r0 = r0 / asuint(float4(asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX)));
				r1 = r0 % asuint(float4(asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX)));
				r0 = r0 / asuint(float4(asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX)));
				r2 = r0 % asuint(float4(asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX), asfloat(DimAxisX)));
				r3 = r2 >> 2;
				r0 = asuint(asint(float4(asfloat(DimBlockedX), asfloat(DimBlockedX), asfloat(DimBlockedX), asfloat(DimBlockedX))) * asint(r0) + asint(r3));
				r2 = uint4(3u, 3u, 3u, 3u) & r2;
				r0 = asuint(asint(float4(asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX), asfloat(StrideAxisX))) * asint(r0) + asint(r1));
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
			#elif defined(TensorInt)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxisX;
			int DimAxisX;
			int DimBlockedX;
			Texture2D<int4> Xptr : register(t0);

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
				bool4 b2;
				bool4 b4;
				bool4 b8;
				bool4 b9;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				int idx0 = asint(r0.x);
				r0.x = asuint(idx0 << 2);
				r0 = asuint(int4(0, 1, 2, 3) + asint(r0.xxxx));
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
				r3.w = r3.x | r3.y;
				r5.x = r1.y;
				r5.y = r0.x;
				r5.zw = uint2(0u, 0u);
				r5 = Xptr.Load(int3(asint(r5.xy), asint(r5.w)));
				b8 = (r2.yxxx < uint4(1u, 1u, 2u, 3u));
				b2 = (r2.yyzz < uint4(2u, 3u, 1u, 2u));
				r9.y = b8.z ? r6.x : 0u;
				r6.xy = b2.xw ? r6.yz : uint2(0u, 0u);
				r9.z = b8.z ? 0u : r7.x;
				r2.xw = b2.xw ? uint2(0u, 0u) : r7.yz;
				r8.w = b8.w ? 0xffffffffu : 0u;
				b9.w = (0 == asint(r8.w));
				r8.y = b8.y ? 0xffffffffu : 0u;
				r9.x = r8.y;
				r9.w = b9.w ? 0xffffffffu : 0u;
				r5 = r9 & r5;
				r5.xy = r5.xz | r5.yw;
				r3.x = r5.x | r5.y;
				r0.x = r1.z;
				r1.y = r0.z;
				r1.zw = uint2(0u, 0u);
				r1 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
				r2.y = b2.y ? 0xffffffffu : 0u;
				b8.w = (0 == asint(r2.y));
				r4.y = r6.y;
				r8.y = r6.x;
				r2.z = b2.z ? 0xffffffffu : 0u;
				r4.xz = r2.zw;
				r8.z = r2.x;
				r4.w = b4.w ? 0xffffffffu : 0u;
				r1 = r1 & r4;
				r1.xy = r1.xz | r1.yw;
				r3.z = r1.x | r1.y;
				r0.zw = uint2(0u, 0u);
				r0 = Xptr.Load(int3(asint(r0.xy), asint(r0.w)));
				r8.x = b8.x ? 0xffffffffu : 0u;
				r8.w = b8.w ? 0xffffffffu : 0u;
				r0 = r0 & r8;
				r0.xy = r0.xz | r0.yw;
				r3.y = r0.x | r0.y;
				r0 = asuint(max(int4(int(0xc0000000), int(0xc0000000), int(0xc0000000), int(0xc0000000)), asint(r3)));
				r0 = asuint(min(int4(int(0x3fffffff), int(0x3fffffff), int(0x3fffffff), int(0x3fffffff)), asint(r0)));
				r1 = asuint(asint(r0) << 1);
				r0 = uint4(0xbfffffffu, 0xbfffffffu, 0xbfffffffu, 0xbfffffffu) & r0;
				r1 = uint4(0xfffffffeu, 0xfffffffeu, 0xfffffffeu, 0xfffffffeu) ^ r1;
				r1 = uint4(0x40000000u, 0x40000000u, 0x40000000u, 0x40000000u) & r1;
				o_SV_Target0 = asfloat(asint(r1) + asint(r0));
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
