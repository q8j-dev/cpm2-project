Shader "Hidden/Sentis/Resize1D"
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
			#pragma multi_compile __ LINEAR
			#pragma multi_compile __ NEAREST_CEIL
			#pragma multi_compile __ NEAREST_FLOOR

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
			#if defined(LINEAR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int innerLength;
			int outAxisSize;
			int inputAxisSize;
			float Scale;
			float Bias;
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
				r0.x = r0.x / asuint(asfloat(innerLength));
				r1.x = r0.x % asuint(asfloat(innerLength));
				r0.x = r0.x / asuint(asfloat(outAxisSize));
				r2.x = r0.x % asuint(asfloat(outAxisSize));
				r0.y = asuint(float(r2.x));
				r0.y = asuint(asfloat(r0.y) * Scale + Bias);
				r0.z = asuint(floor(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.y) + -asfloat(r0.z));
				r0.z = asuint(int(asfloat(r0.z)));
				r0.w = asuint(1 + asint(r0.z));
				r0.zw = asuint(max(int2(0, 0), asint(r0.zw)));
				r1.y = asuint(-1 + asint(asfloat(inputAxisSize)));
				r0.zw = asuint(min(asint(r1.yy), asint(r0.zw)));
				r0.z = asuint(asint(r0.x) * asint(asfloat(inputAxisSize)) + asint(r0.z));
				r0.x = asuint(asint(r0.x) * asint(asfloat(inputAxisSize)) + asint(r0.w));
				r0.x = asuint(asint(r0.x) * asint(asfloat(innerLength)) + asint(r1.x));
				r0.z = asuint(asint(r0.z) * asint(asfloat(innerLength)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				r0.x = asuint(1.0 + -asfloat(r0.y));
				r2.x = asuint(asfloat(WidthMaskX)) & r0.z;
				r2.y = r0.z >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				o_SV_Target0 = asfloat(r2) * asfloat(r0.xxxx) + asfloat(r1);
			}
			#elif defined(NEAREST_FLOOR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int innerLength;
			int outAxisSize;
			int inputAxisSize;
			float Scale;
			float Bias;
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
				r0.x = r0.x / asuint(asfloat(innerLength));
				r1.x = r0.x % asuint(asfloat(innerLength));
				r0.x = r0.x / asuint(asfloat(outAxisSize));
				r2.x = r0.x % asuint(asfloat(outAxisSize));
				r0.y = asuint(float(r2.x));
				r0.y = asuint(asfloat(r0.y) * Scale + Bias);
				r0.y = asuint(floor(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.y = asuint(max(0, asint(r0.y)));
				r0.z = asuint(-1 + asint(asfloat(inputAxisSize)));
				r0.y = asuint(min(asint(r0.y), asint(r0.z)));
				r0.x = asuint(asint(r0.x) * asint(asfloat(inputAxisSize)) + asint(r0.y));
				r0.x = asuint(asint(r0.x) * asint(asfloat(innerLength)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(NEAREST_CEIL)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int innerLength;
			int outAxisSize;
			int inputAxisSize;
			float Scale;
			float Bias;
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
				r0.x = r0.x / asuint(asfloat(innerLength));
				r1.x = r0.x % asuint(asfloat(innerLength));
				r0.x = r0.x / asuint(asfloat(outAxisSize));
				r2.x = r0.x % asuint(asfloat(outAxisSize));
				r0.y = asuint(float(r2.x));
				r0.y = asuint(asfloat(r0.y) * Scale + Bias);
				r0.y = asuint(ceil(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.y = asuint(max(0, asint(r0.y)));
				r0.z = asuint(-1 + asint(asfloat(inputAxisSize)));
				r0.y = asuint(min(asint(r0.y), asint(r0.z)));
				r0.x = asuint(asint(r0.x) * asint(asfloat(inputAxisSize)) + asint(r0.y));
				r0.x = asuint(asint(r0.x) * asint(asfloat(innerLength)) + asint(r1.x));
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
