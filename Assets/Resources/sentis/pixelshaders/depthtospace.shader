Shader "Hidden/Sentis/DepthToSpace"
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
			#pragma multi_compile __ COLUMNROWDEPTH
			#pragma multi_compile __ DEPTHCOLUMNROW

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
			#if defined(COLUMNROWDEPTH)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxisX;
			int DimAxisX;
			int DimBlockedX;
			int O_width;
			int O_height;
			int O_channels;
			int X_width;
			int X_height;
			int X_channels;
			int BlockSize;
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
				uint4 r4;
				uint4 r5;
				uint4 r6;
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
				r0 = r0 / asuint(float4(asfloat(O_width), asfloat(O_width), asfloat(O_width), asfloat(O_width)));
				r1 = r0 % asuint(float4(asfloat(O_width), asfloat(O_width), asfloat(O_width), asfloat(O_width)));
				r0 = r0 / asuint(float4(asfloat(O_height), asfloat(O_height), asfloat(O_height), asfloat(O_height)));
				r2 = r0 % asuint(float4(asfloat(O_height), asfloat(O_height), asfloat(O_height), asfloat(O_height)));
				r1 = r1 / asuint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize)));
				r3 = r1 % asuint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize)));
				r2 = r2 / asuint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize)));
				r4 = r2 % asuint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize)));
				r0 = r0 / asuint(float4(asfloat(O_channels), asfloat(O_channels), asfloat(O_channels), asfloat(O_channels)));
				r5 = r0 % asuint(float4(asfloat(O_channels), asfloat(O_channels), asfloat(O_channels), asfloat(O_channels)));
				r4 = asuint(asint(r4) * asint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize))));
				r6.x = asuint(asint(asfloat(BlockSize)) * asint(asfloat(BlockSize)));
				r4 = asuint(asint(r6.xxxx) * asint(r5) + asint(r4));
				r3 = asuint(asint(r4) + asint(r3));
				r0 = asuint(asint(float4(asfloat(X_channels), asfloat(X_channels), asfloat(X_channels), asfloat(X_channels))) * asint(r0) + asint(r3));
				r0 = asuint(asint(float4(asfloat(X_height), asfloat(X_height), asfloat(X_height), asfloat(X_height))) * asint(r0) + asint(r2));
				r0 = asuint(asint(float4(asfloat(X_width), asfloat(X_width), asfloat(X_width), asfloat(X_width))) * asint(r0) + asint(r1));
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
			#elif defined(DEPTHCOLUMNROW)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			int WidthShiftX;
			int WidthMaskX;
			int StrideAxisX;
			int DimAxisX;
			int DimBlockedX;
			int O_width;
			int O_height;
			int O_channels;
			int X_width;
			int X_height;
			int X_channels;
			int BlockSize;
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
				uint4 r4;
				uint4 r5;
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
				r0 = r0 / asuint(float4(asfloat(O_width), asfloat(O_width), asfloat(O_width), asfloat(O_width)));
				r1 = r0 % asuint(float4(asfloat(O_width), asfloat(O_width), asfloat(O_width), asfloat(O_width)));
				r1 = r1 / asuint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize)));
				r2 = r1 % asuint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize)));
				r0 = r0 / asuint(float4(asfloat(O_height), asfloat(O_height), asfloat(O_height), asfloat(O_height)));
				r3 = r0 % asuint(float4(asfloat(O_height), asfloat(O_height), asfloat(O_height), asfloat(O_height)));
				r2 = asuint(asint(r2) * asint(float4(asfloat(O_channels), asfloat(O_channels), asfloat(O_channels), asfloat(O_channels))));
				r3 = r3 / asuint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize)));
				r4 = r3 % asuint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize)));
				r0 = r0 / asuint(float4(asfloat(O_channels), asfloat(O_channels), asfloat(O_channels), asfloat(O_channels)));
				r5 = r0 % asuint(float4(asfloat(O_channels), asfloat(O_channels), asfloat(O_channels), asfloat(O_channels)));
				r4 = asuint(asint(r4) * asint(float4(asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize), asfloat(BlockSize))));
				r2 = asuint(asint(r4) * asint(float4(asfloat(O_channels), asfloat(O_channels), asfloat(O_channels), asfloat(O_channels))) + asint(r2));
				r2 = asuint(asint(r2) + asint(r5));
				r0 = asuint(asint(float4(asfloat(X_channels), asfloat(X_channels), asfloat(X_channels), asfloat(X_channels))) * asint(r0) + asint(r2));
				r0 = asuint(asint(float4(asfloat(X_height), asfloat(X_height), asfloat(X_height), asfloat(X_height))) * asint(r0) + asint(r3));
				r0 = asuint(asint(float4(asfloat(X_width), asfloat(X_width), asfloat(X_width), asfloat(X_width))) * asint(r0) + asint(r1));
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
			#endif
			#endif
			ENDHLSL
		}
	}
}
