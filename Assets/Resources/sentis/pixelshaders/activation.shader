Shader "Hidden/Sentis/Activation"
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
			#pragma multi_compile __ Abs
			#pragma multi_compile __ Acos
			#pragma multi_compile __ Acosh
			#pragma multi_compile __ Asin
			#pragma multi_compile __ Asinh
			#pragma multi_compile __ Atan
			#pragma multi_compile __ Atanh
			#pragma multi_compile __ Ceil
			#pragma multi_compile __ Celu
			#pragma multi_compile __ Clip
			#pragma multi_compile __ Cos
			#pragma multi_compile __ Cosh
			#pragma multi_compile __ Elu
			#pragma multi_compile __ Erf
			#pragma multi_compile __ Exp
			#pragma multi_compile __ Floor
			#pragma multi_compile __ Gelu
			#pragma multi_compile __ GeluFast
			#pragma multi_compile __ HardSigmoid
			#pragma multi_compile __ HardSwish
			#pragma multi_compile __ LeakyRelu
			#pragma multi_compile __ Log
			#pragma multi_compile __ Neg
			#pragma multi_compile __ Pow
			#pragma multi_compile __ Reciprocal
			#pragma multi_compile __ Relu
			#pragma multi_compile __ Relu6
			#pragma multi_compile __ Round
			#pragma multi_compile __ Selu
			#pragma multi_compile __ Shrink
			#pragma multi_compile __ Sigmoid
			#pragma multi_compile __ Sign
			#pragma multi_compile __ Sin
			#pragma multi_compile __ Sinh
			#pragma multi_compile __ Softplus
			#pragma multi_compile __ Softsign
			#pragma multi_compile __ Sqrt
			#pragma multi_compile __ Square
			#pragma multi_compile __ Swish
			#pragma multi_compile __ Tan
			#pragma multi_compile __ Tanh
			#pragma multi_compile __ ThresholdedRelu

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
			#if defined(Relu)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(asfloat(r1) + abs(asfloat(r1)));
				r1 = asuint(float4(0.5, 0.5, 0.5, 0.5) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Selu)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float Alpha;
			float Beta;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				bool4 b3;
				bool4 b4;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r1));
				r3 = asuint(exp2(asfloat(r3)));
				r3 = asuint(float4(Alpha, Alpha, Alpha, Alpha) * asfloat(r3) + -float4(Alpha, Alpha, Alpha, Alpha));
				r3 = asuint(float4(Beta, Beta, Beta, Beta) * asfloat(r3));
				b4 = (float4(0.0, 0.0, 0.0, 0.0) >= asfloat(r1));
				r1 = asuint(float4(Beta, Beta, Beta, Beta) * asfloat(r1));
				r1 = b4 ? r3 : r1;
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Abs)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(abs(asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Neg)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(-asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Ceil)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(ceil(asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Floor)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(floor(asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Round)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(round(asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Reciprocal)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(float4(1.0, 1.0, 1.0, 1.0) / asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Swish)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(float4(-1.44269502, -1.44269502, -1.44269502, -1.44269502) * asfloat(r1));
				r3 = asuint(exp2(asfloat(r3)));
				r3 = asuint(float4(1.0, 1.0, 1.0, 1.0) + asfloat(r3));
				r1 = asuint(asfloat(r1) / asfloat(r3));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Tanh)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(max(float4(-16.0, -16.0, -16.0, -16.0), asfloat(r1)));
				r1 = asuint(min(float4(16.0, 16.0, 16.0, 16.0), asfloat(r1)));
				r1 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r1));
				r3 = asuint(exp2(asfloat(r1)));
				r1 = asuint(exp2(-asfloat(r1)));
				r4 = asuint(asfloat(r3) + asfloat(r1));
				r1 = asuint(asfloat(r3) + -asfloat(r1));
				r3 = asuint(float4(1.0, 1.0, 1.0, 1.0) / asfloat(r4));
				r1 = asuint(asfloat(r3) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Softplus)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r1));
				r1 = asuint(exp2(asfloat(r1)));
				r1 = asuint(float4(1.0, 1.0, 1.0, 1.0) + asfloat(r1));
				r1 = asuint(log2(asfloat(r1)));
				r1 = asuint(float4(0.693147182, 0.693147182, 0.693147182, 0.693147182) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Sigmoid)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(float4(-1.44269502, -1.44269502, -1.44269502, -1.44269502) * asfloat(r1));
				r1 = asuint(exp2(asfloat(r1)));
				r1 = asuint(float4(1.0, 1.0, 1.0, 1.0) + asfloat(r1));
				r1 = asuint(float4(1.0, 1.0, 1.0, 1.0) / asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(HardSigmoid)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float Alpha;
			float Beta;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(saturate(float4(Alpha, Alpha, Alpha, Alpha) * asfloat(r1) + float4(Beta, Beta, Beta, Beta)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Relu6)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r1)));
				r1 = asuint(min(float4(6.0, 6.0, 6.0, 6.0), asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Elu)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float Alpha;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				bool4 b3;
				bool4 b4;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r1));
				r3 = asuint(exp2(asfloat(r3)));
				r3 = asuint(float4(-1.0, -1.0, -1.0, -1.0) + asfloat(r3));
				r3 = asuint(float4(Alpha, Alpha, Alpha, Alpha) * asfloat(r3));
				b4 = (float4(0.0, 0.0, 0.0, 0.0) >= asfloat(r1));
				r1 = b4 ? r3 : r1;
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(LeakyRelu)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float Alpha;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(float4(Alpha, Alpha, Alpha, Alpha) * asfloat(r1));
				r1 = asuint(max(asfloat(r3), asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Exp)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r1));
				r1 = asuint(exp2(asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Log)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(log2(asfloat(r1)));
				r1 = asuint(float4(0.693147182, 0.693147182, 0.693147182, 0.693147182) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Sqrt)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(sqrt(asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Acos)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b0;
				bool4 b1;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(abs(asfloat(r1)) * float4(-0.0187292993, -0.0187292993, -0.0187292993, -0.0187292993) + float4(0.0742610022, 0.0742610022, 0.0742610022, 0.0742610022));
				r3 = asuint(asfloat(r3) * abs(asfloat(r1)) + float4(-0.212114394, -0.212114394, -0.212114394, -0.212114394));
				r3 = asuint(asfloat(r3) * abs(asfloat(r1)) + float4(1.57072878, 1.57072878, 1.57072878, 1.57072878));
				r4 = asuint(float4(1.0, 1.0, 1.0, 1.0) + -abs(asfloat(r1)));
				b1 = (asfloat(r1) < -asfloat(r1));
				r4 = asuint(sqrt(asfloat(r4)));
				r5 = asuint(asfloat(r4) * asfloat(r3));
				r5 = asuint(asfloat(r5) * float4(-2.0, -2.0, -2.0, -2.0) + float4(3.14159274, 3.14159274, 3.14159274, 3.14159274));
				r1 = b1 ? r5 : uint4(0u, 0u, 0u, 0u);
				r1 = asuint(asfloat(r3) * asfloat(r4) + asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Acosh)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(asfloat(r1) * asfloat(r1) + float4(-1.0, -1.0, -1.0, -1.0));
				r3 = asuint(sqrt(asfloat(r3)));
				r1 = asuint(asfloat(r3) + asfloat(r1));
				r1 = asuint(log2(asfloat(r1)));
				r1 = asuint(float4(0.693147182, 0.693147182, 0.693147182, 0.693147182) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Asin)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b0;
				bool4 b1;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(abs(asfloat(r1)) * float4(-0.0187292993, -0.0187292993, -0.0187292993, -0.0187292993) + float4(0.0742610022, 0.0742610022, 0.0742610022, 0.0742610022));
				r3 = asuint(asfloat(r3) * abs(asfloat(r1)) + float4(-0.212114394, -0.212114394, -0.212114394, -0.212114394));
				r3 = asuint(asfloat(r3) * abs(asfloat(r1)) + float4(1.57072878, 1.57072878, 1.57072878, 1.57072878));
				r4 = asuint(float4(1.0, 1.0, 1.0, 1.0) + -abs(asfloat(r1)));
				b1 = (asfloat(r1) < -asfloat(r1));
				r4 = asuint(sqrt(asfloat(r4)));
				r5 = asuint(asfloat(r4) * asfloat(r3));
				r5 = asuint(asfloat(r5) * float4(-2.0, -2.0, -2.0, -2.0) + float4(3.14159274, 3.14159274, 3.14159274, 3.14159274));
				r1 = b1 ? r5 : uint4(0u, 0u, 0u, 0u);
				r1 = asuint(asfloat(r3) * asfloat(r4) + asfloat(r1));
				r1 = asuint(float4(1.57079637, 1.57079637, 1.57079637, 1.57079637) + -asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Asinh)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(asfloat(r1) * asfloat(r1) + float4(1.0, 1.0, 1.0, 1.0));
				r3 = asuint(sqrt(asfloat(r3)));
				r1 = asuint(asfloat(r3) + asfloat(r1));
				r1 = asuint(log2(asfloat(r1)));
				r1 = asuint(float4(0.693147182, 0.693147182, 0.693147182, 0.693147182) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Atan)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				bool4 b0;
				bool4 b1;
				bool4 b3;
				bool4 b6;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(max(float4(1.0, 1.0, 1.0, 1.0), abs(asfloat(r1))));
				r3 = asuint(float4(1.0, 1.0, 1.0, 1.0) / asfloat(r3));
				r4 = asuint(min(float4(1.0, 1.0, 1.0, 1.0), abs(asfloat(r1))));
				r3 = asuint(asfloat(r4) * asfloat(r3));
				r4 = asuint(asfloat(r3) * asfloat(r3));
				r5 = asuint(asfloat(r4) * float4(0.0208350997, 0.0208350997, 0.0208350997, 0.0208350997) + float4(-0.0851330012, -0.0851330012, -0.0851330012, -0.0851330012));
				r5 = asuint(asfloat(r4) * asfloat(r5) + float4(0.180141002, 0.180141002, 0.180141002, 0.180141002));
				r5 = asuint(asfloat(r4) * asfloat(r5) + float4(-0.330299497, -0.330299497, -0.330299497, -0.330299497));
				r4 = asuint(asfloat(r4) * asfloat(r5) + float4(0.999866009, 0.999866009, 0.999866009, 0.999866009));
				r5 = asuint(asfloat(r4) * asfloat(r3));
				r5 = asuint(asfloat(r5) * float4(-2.0, -2.0, -2.0, -2.0) + float4(1.57079637, 1.57079637, 1.57079637, 1.57079637));
				b6 = (float4(1.0, 1.0, 1.0, 1.0) < abs(asfloat(r1)));
				r1 = asuint(min(float4(1.0, 1.0, 1.0, 1.0), asfloat(r1)));
				b1 = (asfloat(r1) < -asfloat(r1));
				r5 = b6 ? r5 : uint4(0u, 0u, 0u, 0u);
				r3 = asuint(asfloat(r3) * asfloat(r4) + asfloat(r5));
				r1 = asuint(b1 ? -asfloat(r3) : asfloat(r3));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Atanh)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(float4(1.0, 1.0, 1.0, 1.0) + asfloat(r1));
				r1 = asuint(float4(1.0, 1.0, 1.0, 1.0) + -asfloat(r1));
				r1 = asuint(asfloat(r3) / asfloat(r1));
				r1 = asuint(log2(asfloat(r1)));
				r1 = asuint(float4(0.346573591, 0.346573591, 0.346573591, 0.346573591) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Cos)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(cos(asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Cosh)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r1));
				r1 = asuint(float4(-1.44269502, -1.44269502, -1.44269502, -1.44269502) * asfloat(r1));
				r1 = asuint(exp2(asfloat(r1)));
				r3 = asuint(exp2(asfloat(r3)));
				r1 = asuint(asfloat(r3) + asfloat(r1));
				r1 = asuint(float4(0.5, 0.5, 0.5, 0.5) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Sin)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(sin(asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Sinh)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r1));
				r1 = asuint(float4(-1.44269502, -1.44269502, -1.44269502, -1.44269502) * asfloat(r1));
				r1 = asuint(exp2(asfloat(r1)));
				r3 = asuint(exp2(asfloat(r3)));
				r1 = asuint(asfloat(r3) + -asfloat(r1));
				r1 = asuint(float4(0.5, 0.5, 0.5, 0.5) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Tan)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				float4 sin0;
				float4 cos0;
				sincos(asfloat(r1), sin0, cos0);
				r1 = asuint(sin0);
				r3 = asuint(cos0);
				r1 = asuint(asfloat(r1) / asfloat(r3));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx1 = asint(r3.x);
				r0.y = asuint(idx1 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Pow)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float Alpha;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(log2(asfloat(r1)));
				r1 = asuint(float4(Alpha, Alpha, Alpha, Alpha) * asfloat(r1));
				r1 = asuint(exp2(asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Clip)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float Alpha;
			float Beta;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(max(float4(Alpha, Alpha, Alpha, Alpha), asfloat(r1)));
				r1 = asuint(min(float4(Beta, Beta, Beta, Beta), asfloat(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Erf)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b0;
				bool4 b1;
				bool4 b3;
				bool4 b4;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(abs(asfloat(r1)) * float4(0.327591091, 0.327591091, 0.327591091, 0.327591091) + float4(1.0, 1.0, 1.0, 1.0));
				r3 = asuint(float4(1.0, 1.0, 1.0, 1.0) / asfloat(r3));
				r4 = asuint(asfloat(r3) * asfloat(r3));
				r5 = asuint(float4(-0.284496725, -0.284496725, -0.284496725, -0.284496725) * asfloat(r4));
				r4 = asuint(asfloat(r4) * asfloat(r3));
				r5 = asuint(asfloat(r3) * float4(0.254829586, 0.254829586, 0.254829586, 0.254829586) + asfloat(r5));
				r5 = asuint(asfloat(r4) * float4(1.42141378, 1.42141378, 1.42141378, 1.42141378) + asfloat(r5));
				r4 = asuint(asfloat(r4) * asfloat(r3));
				r3 = asuint(asfloat(r4) * asfloat(r3));
				r4 = asuint(asfloat(r4) * float4(-1.45315206, -1.45315206, -1.45315206, -1.45315206) + asfloat(r5));
				r3 = asuint(asfloat(r3) * float4(1.06140542, 1.06140542, 1.06140542, 1.06140542) + asfloat(r4));
				r4 = asuint(-abs(asfloat(r1)) * abs(asfloat(r1)));
				r4 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r4));
				r4 = asuint(exp2(asfloat(r4)));
				r3 = asuint(float4(1.0, 1.0, 1.0, 1.0) - asfloat(r3) * asfloat(r4));
				b4 = (float4(0.0, 0.0, 0.0, 0.0) < asfloat(r1));
				b1 = (asfloat(r1) < float4(0.0, 0.0, 0.0, 0.0));
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				r4.x = b4.x ? 0xffffffffu : 0u;
				r4.y = b4.y ? 0xffffffffu : 0u;
				r4.z = b4.z ? 0xffffffffu : 0u;
				r4.w = b4.w ? 0xffffffffu : 0u;
				r1 = asuint(asint(r1) + -asint(r4));
				r1 = asuint(float4(asint(r1)));
				r1 = asuint(asfloat(r1) * asfloat(r3));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Sign)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b1;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				b3 = (float4(0.0, 0.0, 0.0, 0.0) < asfloat(r1));
				b1 = (asfloat(r1) < float4(0.0, 0.0, 0.0, 0.0));
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				r3.x = b3.x ? 0xffffffffu : 0u;
				r3.y = b3.y ? 0xffffffffu : 0u;
				r3.z = b3.z ? 0xffffffffu : 0u;
				r3.w = b3.w ? 0xffffffffu : 0u;
				r1 = asuint(asint(r1) + -asint(r3));
				r1 = asuint(float4(asint(r1)));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Square)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(asfloat(r1) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Celu)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float Alpha;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(asfloat(r1) / float4(Alpha, Alpha, Alpha, Alpha));
				r1 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r1)));
				r3 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r3));
				r3 = asuint(exp2(asfloat(r3)));
				r3 = asuint(float4(-1.0, -1.0, -1.0, -1.0) + asfloat(r3));
				r3 = asuint(float4(Alpha, Alpha, Alpha, Alpha) * asfloat(r3));
				r3 = asuint(min(float4(0.0, 0.0, 0.0, 0.0), asfloat(r3)));
				r1 = asuint(asfloat(r3) + asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(HardSwish)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(saturate(asfloat(r1) * float4(0.166666672, 0.166666672, 0.166666672, 0.166666672) + float4(0.5, 0.5, 0.5, 0.5)));
				r1 = asuint(asfloat(r3) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Softsign)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(float4(1.0, 1.0, 1.0, 1.0) + abs(asfloat(r1)));
				r1 = asuint(asfloat(r1) / asfloat(r3));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(ThresholdedRelu)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float Alpha;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b1;
				bool4 b2;
				bool4 b3;
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
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r1 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r1) + asint(r2.xxxx));
				b1 = (r1 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b1 = b1 && b3;
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				b1 = (int4(0, 0, 0, 0) == asint(r1));
				r2.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r2.y = r0.x >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				b2 = (float4(Alpha, Alpha, Alpha, Alpha) >= asfloat(r0));
				b1 = b2 || b1;
				o_SV_Target0 = b1 ? float4(0.0, 0.0, 0.0, 0.0) : asfloat(r0);
			}
			#elif defined(Gelu)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				uint4 r6;
				bool4 b0;
				bool4 b3;
				bool4 b4;
				bool4 b5;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(float4(0.707106769, 0.707106769, 0.707106769, 0.707106769) * asfloat(r1));
				r4 = asuint(abs(asfloat(r3)) * float4(0.327591091, 0.327591091, 0.327591091, 0.327591091) + float4(1.0, 1.0, 1.0, 1.0));
				r3 = asuint(-abs(asfloat(r3)) * abs(asfloat(r3)));
				r3 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r3));
				r3 = asuint(exp2(asfloat(r3)));
				r4 = asuint(float4(1.0, 1.0, 1.0, 1.0) / asfloat(r4));
				r5 = asuint(asfloat(r4) * asfloat(r4));
				r6 = asuint(float4(-0.284496725, -0.284496725, -0.284496725, -0.284496725) * asfloat(r5));
				r5 = asuint(asfloat(r5) * asfloat(r4));
				r6 = asuint(asfloat(r4) * float4(0.254829586, 0.254829586, 0.254829586, 0.254829586) + asfloat(r6));
				r6 = asuint(asfloat(r5) * float4(1.42141378, 1.42141378, 1.42141378, 1.42141378) + asfloat(r6));
				r5 = asuint(asfloat(r5) * asfloat(r4));
				r4 = asuint(asfloat(r5) * asfloat(r4));
				r5 = asuint(asfloat(r5) * float4(-1.45315206, -1.45315206, -1.45315206, -1.45315206) + asfloat(r6));
				r4 = asuint(asfloat(r4) * float4(1.06140542, 1.06140542, 1.06140542, 1.06140542) + asfloat(r5));
				r3 = asuint(float4(1.0, 1.0, 1.0, 1.0) - asfloat(r4) * asfloat(r3));
				b4 = (float4(0.0, 0.0, 0.0, 0.0) < asfloat(r1));
				b5 = (asfloat(r1) < float4(0.0, 0.0, 0.0, 0.0));
				r5.x = b5.x ? 0xffffffffu : 0u;
				r5.y = b5.y ? 0xffffffffu : 0u;
				r5.z = b5.z ? 0xffffffffu : 0u;
				r5.w = b5.w ? 0xffffffffu : 0u;
				r4.x = b4.x ? 0xffffffffu : 0u;
				r4.y = b4.y ? 0xffffffffu : 0u;
				r4.z = b4.z ? 0xffffffffu : 0u;
				r4.w = b4.w ? 0xffffffffu : 0u;
				r4 = asuint(asint(r5) + -asint(r4));
				r4 = asuint(float4(asint(r4)));
				r3 = asuint(asfloat(r4) * asfloat(r3) + float4(1.0, 1.0, 1.0, 1.0));
				r1 = asuint(asfloat(r3) * asfloat(r1));
				r1 = asuint(float4(0.5, 0.5, 0.5, 0.5) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(GeluFast)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b0;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3 = asuint(asfloat(r1) * asfloat(r1));
				r3 = asuint(asfloat(r3) * asfloat(r1));
				r3 = asuint(asfloat(r3) * float4(0.0447149985, 0.0447149985, 0.0447149985, 0.0447149985) + asfloat(r1));
				r1 = asuint(float4(0.5, 0.5, 0.5, 0.5) * asfloat(r1));
				r3 = asuint(float4(0.797884583, 0.797884583, 0.797884583, 0.797884583) * asfloat(r3));
				r3 = asuint(max(float4(-16.0, -16.0, -16.0, -16.0), asfloat(r3)));
				r3 = asuint(min(float4(16.0, 16.0, 16.0, 16.0), asfloat(r3)));
				r3 = asuint(float4(1.44269502, 1.44269502, 1.44269502, 1.44269502) * asfloat(r3));
				r4 = asuint(exp2(asfloat(r3)));
				r3 = asuint(exp2(-asfloat(r3)));
				r5 = asuint(asfloat(r4) + asfloat(r3));
				r3 = asuint(asfloat(r4) + -asfloat(r3));
				r4 = asuint(float4(1.0, 1.0, 1.0, 1.0) / asfloat(r5));
				r3 = asuint(asfloat(r3) * asfloat(r4) + float4(1.0, 1.0, 1.0, 1.0));
				r1 = asuint(asfloat(r3) * asfloat(r1));
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#elif defined(Shrink)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int LengthO;
			int StrideAxisO;
			int DimAxisO;
			int DimBlockedO;
			float Alpha;
			float Beta;
			int WidthShiftX;
			int WidthMaskX;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				bool4 b3;
				bool4 b4;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(StrideAxisO));
				r2.x = r0.x % asuint(asfloat(StrideAxisO));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				b3 = (asfloat(r1) < -float4(Beta, Beta, Beta, Beta));
				r4 = asuint(float4(Alpha, Alpha, Alpha, Alpha) + asfloat(r1));
				r3 = b3 ? r4 : uint4(0u, 0u, 0u, 0u);
				b4 = (float4(Beta, Beta, Beta, Beta) < asfloat(r1));
				r1 = asuint(-float4(Alpha, Alpha, Alpha, Alpha) + asfloat(r1));
				r1 = b4 ? r1 : r3;
				r0.x = r0.x / asuint(asfloat(DimBlockedO));
				r3.x = r0.x % asuint(asfloat(DimBlockedO));
				int idx0 = asint(r3.x);
				r0.y = asuint(idx0 << 2);
				r3 = asuint(int4(0, 1, 2, 3) + asint(r0.yyyy));
				r0 = asuint(asint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))) * asint(r0.xxxx) + asint(r3));
				b3 = (r3 < asuint(float4(asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO), asfloat(DimAxisO))));
				r0 = asuint(asint(float4(asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO), asfloat(StrideAxisO))) * asint(r0) + asint(r2.xxxx));
				b0 = (r0 < asuint(float4(asfloat(LengthO), asfloat(LengthO), asfloat(LengthO), asfloat(LengthO))));
				b0 = b0 && b3;
				r0.x = b0.x ? 0xffffffffu : 0u;
				r0.y = b0.y ? 0xffffffffu : 0u;
				r0.z = b0.z ? 0xffffffffu : 0u;
				r0.w = b0.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r0 & r1);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
