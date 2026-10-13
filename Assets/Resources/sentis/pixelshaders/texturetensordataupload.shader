Shader "Hidden/Sentis/TextureTensorDataUpload"
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
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			int WidthShiftX;
			int WidthMaskX;
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
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r1.x = r0.x % asuint(asfloat(StrideAxisO));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r2.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r2.x);
				r0.y = asuint(idx0 << 2);
				r2 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r2));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r1.xxxx));
				r1 = uint4(3u, 3u, 3u, 3u) & r0;
				r0 = r0 >> 2;
				r2 = asuint(float4(asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX))) & r0.zxyw;
				r0 = r0.xwyz >> asuint(float4(asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX)));
				r3.x = r2.y;
				r3.y = r0.x;
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				o_SV_Target0.x = dot(asfloat(r3), icb[asint(r1.x)]);
				r3.x = r2.z;
				r3.y = r0.z;
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				o_SV_Target0.y = dot(asfloat(r3), icb[asint(r1.y)]);
				r0.x = r2.w;
				r2.y = r0.w;
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				o_SV_Target0.z = dot(asfloat(r2), icb[asint(r1.z)]);
				r0.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r0.xy), asint(r0.w))));
				o_SV_Target0.w = dot(asfloat(r0), icb[asint(r1.w)]);
			}
			#elif defined(TensorInt)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);
			static const float4 icb[4] =
			{
				float4(1.0, 0.0, 0.0, 0.0),
				float4(0.0, 1.0, 0.0, 0.0),
				float4(0.0, 0.0, 1.0, 0.0),
				float4(0.0, 0.0, 0.0, 1.0)
			};

			void frag(float4 i_SV_Position0 : SV_Position, out int4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
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
				r1 = uint4(3u, 3u, 3u, 3u) & r0;
				r0 = r0 >> 2;
				r2 = asuint(float4(asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX), asfloat(WidthMaskX))) & r0.zxyw;
				r0 = r0.xwyz >> asuint(float4(asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX), asfloat(WidthShiftX)));
				r3.x = r2.y;
				r3.y = r0.x;
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3.x = asuint(dot(asfloat(r3), icb[asint(r1.x)]));
				r4.x = r2.z;
				r4.y = r0.z;
				r4.zw = uint2(0u, 0u);
				r4 = asuint(Xptr.Load(int3(asint(r4.xy), asint(r4.w))));
				r3.y = asuint(dot(asfloat(r4), icb[asint(r1.y)]));
				r0.x = r2.w;
				r2.y = r0.w;
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r3.z = asuint(dot(asfloat(r2), icb[asint(r1.z)]));
				r0.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r0.xy), asint(r0.w))));
				r3.w = asuint(dot(asfloat(r0), icb[asint(r1.w)]));
				r0 = r3 >> 1;
				r1 = uint4(0xbfffffffu, 0xbfffffffu, 0xbfffffffu, 0xbfffffffu) & r3;
				r0 = uint4(0x40000000u, 0x40000000u, 0x40000000u, 0x40000000u) & r0;
				o_SV_Target0 = asint(asfloat(asint(r1) + asint(r0)));
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
