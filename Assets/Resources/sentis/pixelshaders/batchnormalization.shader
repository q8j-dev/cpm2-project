Shader "Hidden/Sentis/BatchNormalization"
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
			int WidthShiftB;
			int WidthMaskB;
			int WidthShiftM;
			int WidthMaskM;
			int WidthShiftV;
			int WidthMaskV;
			float epsilon;
			int O_channels;
			int O_width;
			int O_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);
			Texture2D<float4> Bptr : register(t2);
			Texture2D<float4> Mptr : register(t3);
			Texture2D<float4> Vptr : register(t4);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r0.x = r0.x / asuint(asfloat(O_width));
				r0.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r2.x = r0.x >> asuint(asfloat(WidthShiftM));
				r2.y = r2.x;
				r2.xz = asuint(float2(asfloat(WidthMaskM), asfloat(WidthMaskV))) & r0.xx;
				r2.w = 0u;
				r3 = asuint(Mptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r1 = asuint(-asfloat(r3) + asfloat(r1));
				r2.y = r0.x >> asuint(asfloat(WidthShiftV));
				r2 = asuint(Vptr.Load(int3(asint(r2.zy), asint(r2.w))));
				r2 = asuint(float4(epsilon, epsilon, epsilon, epsilon) + asfloat(r2));
				r2 = asuint(sqrt(asfloat(r2)));
				r2 = asuint(asfloat(r1) / asfloat(r2));
				int idx0 = asint(r0.x);
				r0.y = asuint(idx0 << 2);
				r3.xyz = asuint(int3(1, 2, 3) + asint(r0.yyy));
				b0.y = (r0.y < asuint(asfloat(O_channels)));
				r4.x = b0.y ? r2.x : r1.x;
				b0.yzw = (r3.xyz < asuint(float3(asfloat(O_channels), asfloat(O_channels), asfloat(O_channels))));
				r4.yzw = b0.yzw ? r2.yzw : r1.yzw;
				r1.x = r0.x >> asuint(asfloat(WidthShiftS));
				r2.z = r1.x;
				r2.xy = asuint(float2(asfloat(WidthMaskS), asfloat(WidthMaskB))) & r0.xx;
				r1.y = r0.x >> asuint(asfloat(WidthShiftB));
				r2.w = 0u;
				r0 = asuint(Sptr.Load(int3(asint(r2.xz), asint(r2.w))));
				r1.zw = r2.yw;
				r1 = asuint(Bptr.Load(int3(asint(r1.zy), asint(r1.w))));
				o_SV_Target0 = asfloat(r0) * asfloat(r4) + asfloat(r1);
			}
			#endif
			ENDHLSL
		}
	}
}
