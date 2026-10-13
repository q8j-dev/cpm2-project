Shader "Hidden/Sentis/LayerNormalizationTail"
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
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int WidthShiftA;
			int WidthMaskA;
			int WidthShiftB;
			int WidthMaskB;
			int WidthShiftK;
			int WidthMaskK;
			int reduceLength;
			float epsilon;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);
			Texture2D<float4> Aptr : register(t2);
			Texture2D<float4> Bptr : register(t3);
			Texture2D<float4> Kptr : register(t4);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
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
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(reduceLength));
				r2.x = r0.x % asuint(asfloat(reduceLength));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r3.zw = uint2(0u, 0u);
				r3.x = asuint(asfloat(WidthMaskA)) & r0.x;
				r3.y = r0.x >> asuint(asfloat(WidthShiftA));
				r3 = asuint(Aptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r1 = asuint(-asfloat(r3) + asfloat(r1));
				r4.zw = uint2(0u, 0u);
				r4.x = asuint(asfloat(WidthMaskS)) & r2.x;
				r4.y = r2.x >> asuint(asfloat(WidthShiftS));
				r4 = asuint(Sptr.Load(int3(asint(r4.xy), asint(r4.w))));
				r1 = asuint(asfloat(r4.xxxx) * asfloat(r1));
				r4.x = asuint(asfloat(WidthMaskK)) & r0.x;
				r4.y = r0.x >> asuint(asfloat(WidthShiftK));
				r4.zw = uint2(0u, 0u);
				r0 = asuint(Kptr.Load(int3(asint(r4.xy), asint(r4.w))));
				r0 = asuint(asfloat(r0) - asfloat(r3) * asfloat(r3));
				r0 = asuint(max(float4(0.0, 0.0, 0.0, 0.0), asfloat(r0)));
				r0 = asuint(float4(epsilon, epsilon, epsilon, epsilon) + asfloat(r0));
				r0 = asuint(sqrt(asfloat(r0)));
				r0 = asuint(asfloat(r1) / asfloat(r0));
				r1.x = asuint(asfloat(WidthMaskB)) & r2.x;
				r1.y = r2.x >> asuint(asfloat(WidthShiftB));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Bptr.Load(int3(asint(r1.xy), asint(r1.w))));
				o_SV_Target0 = asfloat(r1.xxxx) + asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
}
