Shader "Hidden/Sentis/GridSample"
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
			#pragma multi_compile __ ALIGN_CORNERS
			#pragma multi_compile __ BORDER
			#pragma multi_compile __ GRIDSAMPLE1D
			#pragma multi_compile __ GRIDSAMPLE2D
			#pragma multi_compile __ GRIDSAMPLE3D
			#pragma multi_compile __ LINEAR
			#pragma multi_compile __ NEAREST
			#pragma multi_compile __ REFLECTION
			#pragma multi_compile __ ZEROS

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
			#if defined(ALIGN_CORNERS) && defined(GRIDSAMPLE1D) && defined(LINEAR) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r0.y = asuint(1.0 + asfloat(r1.x));
				r0.y = asuint(0.5 * asfloat(r0.y));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.z = asuint(-1.0 + asfloat(r0.z));
				r0.w = asuint(asfloat(r0.y) * asfloat(r0.z));
				r0.w = asuint(floor(asfloat(r0.w)));
				r1.x = asuint(int(asfloat(r0.w)));
				r1.y = asuint(1 + asint(r1.x));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z) + -asfloat(r0.w));
				b0.z = (asint(r1.x) < 0);
				b0.w = (r1.x >= asuint(asfloat(X_width)));
				b0.z = b0.z || b0.w;
				[branch] if (!b0.z)
				{
					r0.z = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r1.x));
					r2.x = asuint(asfloat(WidthMaskX)) & r0.z;
					r2.y = r0.z >> asuint(asfloat(WidthShiftX));
					r2.zw = uint2(0u, 0u);
					r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				}
				else
				{
					r2 = uint4(0u, 0u, 0u, 0u);
				}
				b0.z = (asint(r1.y) < 0);
				b0.w = (r1.y >= asuint(asfloat(X_width)));
				b0.z = b0.z || b0.w;
				[branch] if (!b0.z)
				{
					r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r1.y));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				}
				else
				{
					r1 = uint4(0u, 0u, 0u, 0u);
				}
				r0.x = asuint(1.0 + -asfloat(r0.y));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#elif defined(ALIGN_CORNERS) && defined(BORDER) && defined(GRIDSAMPLE1D) && defined(LINEAR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(max(-1.0, asfloat(r1.x)));
				r0.y = asuint(min(1.0, asfloat(r0.y)));
				r0.y = asuint(1.0 + asfloat(r0.y));
				r0.y = asuint(0.5 * asfloat(r0.y));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.z = asuint(-1.0 + asfloat(r0.z));
				r0.w = asuint(asfloat(r0.y) * asfloat(r0.z));
				r0.w = asuint(floor(asfloat(r0.w)));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z) + -asfloat(r0.w));
				r0.z = asuint(int(asfloat(r0.w)));
				r0.w = asuint(1 + asint(r0.z));
				r0.z = asuint(max(0, asint(r0.z)));
				r0.z = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.z));
				r1.x = asuint(-1 + asint(asfloat(X_width)));
				r0.w = asuint(min(asint(r1.x), asint(r0.w)));
				r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.w));
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
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE1D) && defined(LINEAR) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(-1.0 + asfloat(r1.x));
				r0.z = asuint(4.0 * asfloat(r0.y));
				b0.z = (asfloat(r0.z) >= -asfloat(r0.z));
				r0.zw = b0.zz ? uint2(0x40800000u, 0x3e800000u) : uint2(0xc0800000u, 0xbe800000u);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y) + 4.0);
				r0.z = asuint(4.0 * asfloat(r0.y));
				b0.z = (asfloat(r0.z) >= -asfloat(r0.z));
				r0.zw = b0.zz ? uint2(0x40800000u, 0x3e800000u) : uint2(0xc0800000u, 0xbe800000u);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y) + -2.0);
				r0.y = asuint(0.5 * abs(asfloat(r0.y)));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.z = asuint(-1.0 + asfloat(r0.z));
				r0.w = asuint(asfloat(r0.y) * asfloat(r0.z));
				r0.w = asuint(floor(asfloat(r0.w)));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z) + -asfloat(r0.w));
				r0.z = asuint(int(asfloat(r0.w)));
				r0.w = asuint(1 + asint(r0.z));
				r0.z = asuint(max(0, asint(r0.z)));
				r0.z = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.z));
				r1.x = asuint(-1 + asint(asfloat(X_width)));
				r0.w = asuint(min(asint(r1.x), asint(r0.w)));
				r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.w));
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
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE1D) && defined(NEAREST) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.y = r0.x / asuint(asfloat(O_channelsDiv4));
				r0.z = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.z;
				r1.y = r0.z >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.z = asuint(1.0 + asfloat(r1.x));
				r0.z = asuint(0.5 * asfloat(r0.z));
				r0.w = asuint(float(asint(asfloat(X_width))));
				r0.w = asuint(-1.0 + asfloat(r0.w));
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.w));
				r0.z = asuint(round(asfloat(r0.z)));
				r0.z = asuint(int(asfloat(r0.z)));
				b0.w = (asint(r0.z) < 0);
				b1.x = (r0.z >= asuint(asfloat(X_width)));
				b0.w = b1.x || b0.w;
				[branch] if (!b0.w)
				{
					r0.x = r0.x % asuint(asfloat(O_channelsDiv4));
					r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.y) + asint(r0.x));
					r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.z));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
				}
				else
				{
					o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			#elif defined(ALIGN_CORNERS) && defined(BORDER) && defined(GRIDSAMPLE1D) && defined(NEAREST)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(max(-1.0, asfloat(r1.x)));
				r0.y = asuint(min(1.0, asfloat(r0.y)));
				r0.y = asuint(1.0 + asfloat(r0.y));
				r0.y = asuint(0.5 * asfloat(r0.y));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.z = asuint(-1.0 + asfloat(r0.z));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z));
				r0.y = asuint(round(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.y = asuint(max(0, asint(r0.y)));
				r0.z = asuint(-1 + asint(asfloat(X_width)));
				r0.y = asuint(min(asint(r0.y), asint(r0.z)));
				r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE1D) && defined(NEAREST) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(-1.0 + asfloat(r1.x));
				r0.z = asuint(4.0 * asfloat(r0.y));
				b0.z = (asfloat(r0.z) >= -asfloat(r0.z));
				r0.zw = b0.zz ? uint2(0x40800000u, 0x3e800000u) : uint2(0xc0800000u, 0xbe800000u);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y) + 4.0);
				r0.z = asuint(4.0 * asfloat(r0.y));
				b0.z = (asfloat(r0.z) >= -asfloat(r0.z));
				r0.zw = b0.zz ? uint2(0x40800000u, 0x3e800000u) : uint2(0xc0800000u, 0xbe800000u);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y) + -2.0);
				r0.y = asuint(0.5 * abs(asfloat(r0.y)));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.z = asuint(-1.0 + asfloat(r0.z));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z));
				r0.y = asuint(round(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.y = asuint(max(0, asint(r0.y)));
				r0.z = asuint(-1 + asint(asfloat(X_width)));
				r0.y = asuint(min(asint(r0.y), asint(r0.z)));
				r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE2D) && defined(LINEAR) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				bool4 b4;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.x = asuint(asint(r0.x) * asint(r0.y));
				r0.yz = asuint(float2(1.0, 1.0) + asfloat(r1.xy));
				r0.yz = asuint(float2(0.5, 0.5) * asfloat(r0.yz));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.xy = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r1.zw = asuint(asfloat(r1.xy) * asfloat(r0.yz));
				r1.zw = asuint(floor(asfloat(r1.zw)));
				r2.xy = asuint(int2(asfloat(r1.zw)));
				r2.zw = asuint(int2(1, 1) + asint(r2.yx));
				r0.yz = asuint(asfloat(r0.yz) * asfloat(r1.xy) + -asfloat(r1.zw));
				b1.xy = (asint(r2.xy) < int2(0, 0));
				b1.zw = (r2.xy >= asuint(float2(asfloat(X_width), asfloat(X_height))));
				b0.w = b1.x || b1.z;
				b1.x = b0.w || b1.y;
				b1.x = b1.x || b1.w;
				r0.w = b0.w ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				[branch] if (!b1.x)
				{
					r1.x = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r0.x));
					r1.x = asuint(asint(r1.x) + asint(r2.x));
					r3.x = asuint(asfloat(WidthMaskX)) & r1.x;
					r3.y = r1.x >> asuint(asfloat(WidthShiftX));
					r3.zw = uint2(0u, 0u);
					r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				}
				else
				{
					r3 = uint4(0u, 0u, 0u, 0u);
				}
				b1.xz = (asint(r2.zw) < int2(0, 0));
				r1.x = b1.x ? 0xffffffffu : 0u;
				r0.w = r1.x | r0.w;
				b4.xy = (r2.zw >= asuint(float2(asfloat(X_height), asfloat(X_width))));
				r4.x = b4.x ? 0xffffffffu : 0u;
				r0.w = r4.x | r0.w;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r4.y = b4.y ? 0xffffffffu : 0u;
				[branch] if (r0.w == 0)
				{
					r0.w = asuint(asint(r2.z) * asint(asfloat(X_width)) + asint(r0.x));
					r0.w = asuint(asint(r0.w) + asint(r2.x));
					r5.x = asuint(asfloat(WidthMaskX)) & r0.w;
					r5.y = r0.w >> asuint(asfloat(WidthShiftX));
					r5.zw = uint2(0u, 0u);
					r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				}
				else
				{
					r5 = uint4(0u, 0u, 0u, 0u);
				}
				r0.w = r4.y | r1.z;
				r1.y = r0.w | r1.y;
				r1.y = r1.y | r1.w;
				[branch] if (r1.y == 0)
				{
					r1.y = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r0.x));
					r1.y = asuint(asint(r1.y) + asint(r2.w));
					r6.x = asuint(asfloat(WidthMaskX)) & r1.y;
					r6.y = r1.y >> asuint(asfloat(WidthShiftX));
					r6.zw = uint2(0u, 0u);
					r6 = asuint(Xptr.Load(int3(asint(r6.xy), asint(r6.w))));
				}
				else
				{
					r6 = uint4(0u, 0u, 0u, 0u);
				}
				r0.w = r0.w | r1.x;
				r0.w = r0.w | r4.x;
				[branch] if (r0.w == 0)
				{
					r0.x = asuint(asint(r2.z) * asint(asfloat(X_width)) + asint(r0.x));
					r0.x = asuint(asint(r0.x) + asint(r2.w));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				}
				else
				{
					r1 = uint4(0u, 0u, 0u, 0u);
				}
				r0.xw = asuint(float2(1.0, 1.0) + -asfloat(r0.yz));
				r2 = asuint(asfloat(r3) * asfloat(r0.xxxx));
				r3 = asuint(asfloat(r5) * asfloat(r0.xxxx));
				r3 = asuint(asfloat(r3) * asfloat(r0.zzzz));
				r2 = asuint(asfloat(r2) * asfloat(r0.wwww) + asfloat(r3));
				r3 = asuint(asfloat(r6) * asfloat(r0.yyyy));
				r2 = asuint(asfloat(r3) * asfloat(r0.wwww) + asfloat(r2));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r1) * asfloat(r0.zzzz) + asfloat(r2);
			}
			#elif defined(ALIGN_CORNERS) && defined(BORDER) && defined(GRIDSAMPLE2D) && defined(LINEAR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yz = asuint(max(float2(-1.0, -1.0), asfloat(r1.xy)));
				r0.yz = asuint(min(float2(1.0, 1.0), asfloat(r0.yz)));
				r0.yz = asuint(float2(1.0, 1.0) + asfloat(r0.yz));
				r0.yz = asuint(float2(0.5, 0.5) * asfloat(r0.yz));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.xy = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r1.zw = asuint(asfloat(r1.xy) * asfloat(r0.yz));
				r1.zw = asuint(floor(asfloat(r1.zw)));
				r0.yz = asuint(asfloat(r0.yz) * asfloat(r1.xy) + -asfloat(r1.zw));
				r1.xy = asuint(int2(asfloat(r1.zw)));
				r1.zw = asuint(max(int2(0, 0), asint(r1.xy)));
				r1.xy = asuint(int2(1, 1) + asint(r1.xy));
				r2.xy = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r1.xy = asuint(min(asint(r2.xy), asint(r1.xy)));
				r0.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.x) * asint(r0.w));
				r0.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r1.w) * asint(asfloat(X_width)) + asint(r0.x));
				r1.y = asuint(asint(r0.w) + asint(r1.z));
				r1.z = asuint(asint(r0.x) + asint(r1.z));
				r0.xw = asuint(asint(r0.xw) + asint(r1.xx));
				r2.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r2.y = r1.y >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r1.xy = asuint(float2(1.0, 1.0) + -asfloat(r0.yz));
				r2 = asuint(asfloat(r2) * asfloat(r1.xxxx));
				r2 = asuint(asfloat(r2) * asfloat(r0.zzzz));
				r3.x = asuint(asfloat(WidthMaskX)) & r1.z;
				r3.y = r1.z >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r1.xxxx));
				r2 = asuint(asfloat(r3) * asfloat(r1.yyyy) + asfloat(r2));
				r3.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r3.y = r0.x >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r0.yyyy));
				r1 = asuint(asfloat(r3) * asfloat(r1.yyyy) + asfloat(r2));
				r2.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r2.y = r0.w >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r2) * asfloat(r0.zzzz) + asfloat(r1);
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE2D) && defined(LINEAR) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yz = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r1 = asuint(float4(4.0, 4.0, 4.0, 4.0) * asfloat(r0.yzyz));
				b1 = (asfloat(r1) >= -asfloat(r1.zwzw));
				r1 = b1 ? uint4(0x40800000u, 0x40800000u, 0x3e800000u, 0x3e800000u) : uint4(0xc0800000u, 0xc0800000u, 0xbe800000u, 0xbe800000u);
				r0.yz = asuint(asfloat(r1.zw) * asfloat(r0.yz));
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz) + float2(4.0, 4.0));
				r1 = asuint(float4(4.0, 4.0, 4.0, 4.0) * asfloat(r0.yzyz));
				b1 = (asfloat(r1) >= -asfloat(r1.zwzw));
				r1 = b1 ? uint4(0x40800000u, 0x40800000u, 0x3e800000u, 0x3e800000u) : uint4(0xc0800000u, 0xc0800000u, 0xbe800000u, 0xbe800000u);
				r0.yz = asuint(asfloat(r1.zw) * asfloat(r0.yz));
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz) + float2(-2.0, -2.0));
				r0.yz = asuint(float2(0.5, 0.5) * abs(asfloat(r0.yz)));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.xy = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r1.zw = asuint(asfloat(r1.xy) * asfloat(r0.yz));
				r1.zw = asuint(floor(asfloat(r1.zw)));
				r0.yz = asuint(asfloat(r0.yz) * asfloat(r1.xy) + -asfloat(r1.zw));
				r1.xy = asuint(int2(asfloat(r1.zw)));
				r1.zw = asuint(max(int2(0, 0), asint(r1.xy)));
				r1.xy = asuint(int2(1, 1) + asint(r1.xy));
				r2.xy = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r1.xy = asuint(min(asint(r2.xy), asint(r1.xy)));
				r0.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.x) * asint(r0.w));
				r0.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r1.w) * asint(asfloat(X_width)) + asint(r0.x));
				r1.y = asuint(asint(r0.w) + asint(r1.z));
				r1.z = asuint(asint(r0.x) + asint(r1.z));
				r0.xw = asuint(asint(r0.xw) + asint(r1.xx));
				r2.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r2.y = r1.y >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r1.xy = asuint(float2(1.0, 1.0) + -asfloat(r0.yz));
				r2 = asuint(asfloat(r2) * asfloat(r1.xxxx));
				r2 = asuint(asfloat(r2) * asfloat(r0.zzzz));
				r3.x = asuint(asfloat(WidthMaskX)) & r1.z;
				r3.y = r1.z >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r1.xxxx));
				r2 = asuint(asfloat(r3) * asfloat(r1.yyyy) + asfloat(r2));
				r3.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r3.y = r0.x >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r0.yyyy));
				r1 = asuint(asfloat(r3) * asfloat(r1.yyyy) + asfloat(r2));
				r2.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r2.y = r0.w >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r2) * asfloat(r0.zzzz) + asfloat(r1);
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE2D) && defined(NEAREST) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.y = r0.x / asuint(asfloat(O_channelsDiv4));
				r0.z = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.z = asuint(asint(r0.z) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.z;
				r1.y = r0.z >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.zw = asuint(float2(1.0, 1.0) + asfloat(r1.xy));
				r0.zw = asuint(float2(0.5, 0.5) * asfloat(r0.zw));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.xy = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r0.zw = asuint(asfloat(r1.xy) * asfloat(r0.zw));
				r0.zw = asuint(round(asfloat(r0.zw)));
				r0.zw = asuint(int2(asfloat(r0.zw)));
				b1.xy = (asint(r0.zw) < int2(0, 0));
				b1.zw = (r0.zw >= asuint(float2(asfloat(X_width), asfloat(X_height))));
				b1.x = b1.x || b1.z;
				b1.x = b1.x || b1.y;
				b1.x = b1.x || b1.w;
				[branch] if (!b1.x)
				{
					r0.x = r0.x % asuint(asfloat(O_channelsDiv4));
					r1.x = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
					r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.y) + asint(r0.x));
					r0.y = asuint(asint(r0.w) * asint(asfloat(X_width)));
					r0.x = asuint(asint(r1.x) * asint(r0.x) + asint(r0.y));
					r0.x = asuint(asint(r0.x) + asint(r0.z));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
				}
				else
				{
					o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			#elif defined(ALIGN_CORNERS) && defined(BORDER) && defined(GRIDSAMPLE2D) && defined(NEAREST)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yz = asuint(max(float2(-1.0, -1.0), asfloat(r1.xy)));
				r0.yz = asuint(min(float2(1.0, 1.0), asfloat(r0.yz)));
				r0.yz = asuint(float2(1.0, 1.0) + asfloat(r0.yz));
				r0.yz = asuint(float2(0.5, 0.5) * asfloat(r0.yz));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.xy = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz));
				r0.yz = asuint(round(asfloat(r0.yz)));
				r0.yz = asuint(int2(asfloat(r0.yz)));
				r0.yz = asuint(max(int2(0, 0), asint(r0.yz)));
				r1.xy = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r0.yz = asuint(min(asint(r1.xy), asint(r0.yz)));
				r0.z = asuint(asint(r0.z) * asint(asfloat(X_width)));
				r0.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.w) * asint(r0.x) + asint(r0.z));
				r0.x = asuint(asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE2D) && defined(NEAREST) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yz = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r1 = asuint(float4(4.0, 4.0, 4.0, 4.0) * asfloat(r0.yzyz));
				b1 = (asfloat(r1) >= -asfloat(r1.zwzw));
				r1 = b1 ? uint4(0x40800000u, 0x40800000u, 0x3e800000u, 0x3e800000u) : uint4(0xc0800000u, 0xc0800000u, 0xbe800000u, 0xbe800000u);
				r0.yz = asuint(asfloat(r1.zw) * asfloat(r0.yz));
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz) + float2(4.0, 4.0));
				r1 = asuint(float4(4.0, 4.0, 4.0, 4.0) * asfloat(r0.yzyz));
				b1 = (asfloat(r1) >= -asfloat(r1.zwzw));
				r1 = b1 ? uint4(0x40800000u, 0x40800000u, 0x3e800000u, 0x3e800000u) : uint4(0xc0800000u, 0xc0800000u, 0xbe800000u, 0xbe800000u);
				r0.yz = asuint(asfloat(r1.zw) * asfloat(r0.yz));
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz) + float2(-2.0, -2.0));
				r0.yz = asuint(float2(0.5, 0.5) * abs(asfloat(r0.yz)));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.xy = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz));
				r0.yz = asuint(round(asfloat(r0.yz)));
				r0.yz = asuint(int2(asfloat(r0.yz)));
				r0.yz = asuint(max(int2(0, 0), asint(r0.yz)));
				r1.xy = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r0.yz = asuint(min(asint(r1.xy), asint(r0.yz)));
				r0.z = asuint(asint(r0.z) * asint(asfloat(X_width)));
				r0.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.w) * asint(r0.x) + asint(r0.z));
				r0.x = asuint(asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE3D) && defined(LINEAR) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				uint4 r12;
				bool4 b1;
				bool4 b2;
				bool4 b6;
				bool4 b8;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.y = asuint(asint(r0.y) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.x = asuint(asint(r0.x) * asint(r0.y));
				r0.yzw = asuint(float3(1.0, 1.0, 1.0) + asfloat(r1.xyz));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.yzw));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r1.xyz = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r2.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.yzw));
				r2.xyz = asuint(floor(asfloat(r2.xyz)));
				r3.xyz = asuint(int3(asfloat(r2.xyz)));
				r4.xyz = asuint(int3(1, 1, 1) + asint(r3.zyx));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r1.xyz) + -asfloat(r2.xyz));
				b1.xyz = (asint(r3.xyz) < int3(0, 0, 0));
				b2.xy = (r3.xy >= asuint(float2(asfloat(X_width), asfloat(X_height))));
				b1.x = b2.x || b1.x;
				b1.w = b1.x || b1.y;
				b1.w = b1.w || b2.y;
				b2.x = b1.w || b1.z;
				b2.z = (r3.z >= asuint(asfloat(X_depth)));
				b2.x = b2.x || b2.z;
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				r2.y = b2.y ? 0xffffffffu : 0u;
				r2.z = b2.z ? 0xffffffffu : 0u;
				[branch] if (!b2.x)
				{
					r2.x = asuint(asint(r3.z) * asint(asfloat(X_width)));
					r2.x = asuint(asint(r2.x) * asint(asfloat(X_height)) + asint(r0.x));
					r2.x = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r2.x));
					r2.x = asuint(asint(r2.x) + asint(r3.x));
					r5.x = asuint(asfloat(WidthMaskX)) & r2.x;
					r5.y = r2.x >> asuint(asfloat(WidthShiftX));
					r5.zw = uint2(0u, 0u);
					r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				}
				else
				{
					r5 = uint4(0u, 0u, 0u, 0u);
				}
				b6.xyz = (asint(r4.xyz) < int3(0, 0, 0));
				r6.x = b6.x ? 0xffffffffu : 0u;
				r6.y = b6.y ? 0xffffffffu : 0u;
				r1.xw = r6.yx | r1.xw;
				b2.x = (r4.x >= asuint(asfloat(X_depth)));
				r2.x = b2.x ? 0xffffffffu : 0u;
				r1.w = r2.x | r1.w;
				r6.z = b6.z ? 0xffffffffu : 0u;
				[branch] if (r1.w == 0)
				{
					r1.w = asuint(asint(r4.x) * asint(asfloat(X_width)));
					r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
					r1.w = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r1.w));
					r1.w = asuint(asint(r1.w) + asint(r3.x));
					r7.x = asuint(asfloat(WidthMaskX)) & r1.w;
					r7.y = r1.w >> asuint(asfloat(WidthShiftX));
					r7.zw = uint2(0u, 0u);
					r7 = asuint(Xptr.Load(int3(asint(r7.xy), asint(r7.w))));
				}
				else
				{
					r7 = uint4(0u, 0u, 0u, 0u);
				}
				b8.xy = (r4.yz >= asuint(float2(asfloat(X_height), asfloat(X_width))));
				r8.x = b8.x ? 0xffffffffu : 0u;
				r1.x = r8.x | r1.x;
				r1.w = r1.x | r1.z;
				r1.w = r1.w | r2.z;
				r8.y = b8.y ? 0xffffffffu : 0u;
				[branch] if (r1.w == 0)
				{
					r1.w = asuint(asint(r3.z) * asint(asfloat(X_width)));
					r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
					r1.w = asuint(asint(r4.y) * asint(asfloat(X_width)) + asint(r1.w));
					r1.w = asuint(asint(r1.w) + asint(r3.x));
					r9.x = asuint(asfloat(WidthMaskX)) & r1.w;
					r9.y = r1.w >> asuint(asfloat(WidthShiftX));
					r9.zw = uint2(0u, 0u);
					r9 = asuint(Xptr.Load(int3(asint(r9.xy), asint(r9.w))));
				}
				else
				{
					r9 = uint4(0u, 0u, 0u, 0u);
				}
				r1.x = r1.x | r6.x;
				r1.x = r1.x | r2.x;
				[branch] if (r1.x == 0)
				{
					r1.x = asuint(asint(r4.x) * asint(asfloat(X_width)));
					r1.x = asuint(asint(r1.x) * asint(asfloat(X_height)) + asint(r0.x));
					r1.x = asuint(asint(r4.y) * asint(asfloat(X_width)) + asint(r1.x));
					r1.x = asuint(asint(r1.x) + asint(r3.x));
					r10.x = asuint(asfloat(WidthMaskX)) & r1.x;
					r10.y = r1.x >> asuint(asfloat(WidthShiftX));
					r10.zw = uint2(0u, 0u);
					r10 = asuint(Xptr.Load(int3(asint(r10.xy), asint(r10.w))));
				}
				else
				{
					r10 = uint4(0u, 0u, 0u, 0u);
				}
				r1.x = r8.y | r6.z;
				r1.y = r1.x | r1.y;
				r1.y = r1.y | r2.y;
				r1.w = r1.y | r1.z;
				r1.w = r1.w | r2.z;
				[branch] if (r1.w == 0)
				{
					r1.w = asuint(asint(r3.z) * asint(asfloat(X_width)));
					r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
					r1.w = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r1.w));
					r1.w = asuint(asint(r1.w) + asint(r4.z));
					r11.x = asuint(asfloat(WidthMaskX)) & r1.w;
					r11.y = r1.w >> asuint(asfloat(WidthShiftX));
					r11.zw = uint2(0u, 0u);
					r11 = asuint(Xptr.Load(int3(asint(r11.xy), asint(r11.w))));
				}
				else
				{
					r11 = uint4(0u, 0u, 0u, 0u);
				}
				r1.xy = r1.xy | r6.yx;
				r1.y = r1.y | r2.x;
				[branch] if (r1.y == 0)
				{
					r1.y = asuint(asint(r4.x) * asint(asfloat(X_width)));
					r1.y = asuint(asint(r1.y) * asint(asfloat(X_height)) + asint(r0.x));
					r1.y = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r1.y));
					r1.y = asuint(asint(r1.y) + asint(r4.z));
					r12.x = asuint(asfloat(WidthMaskX)) & r1.y;
					r12.y = r1.y >> asuint(asfloat(WidthShiftX));
					r12.zw = uint2(0u, 0u);
					r12 = asuint(Xptr.Load(int3(asint(r12.xy), asint(r12.w))));
				}
				else
				{
					r12 = uint4(0u, 0u, 0u, 0u);
				}
				r1.x = r1.x | r8.x;
				r1.y = r1.x | r1.z;
				r1.y = r1.y | r2.z;
				[branch] if (r1.y == 0)
				{
					r1.y = asuint(asint(r3.z) * asint(asfloat(X_width)));
					r1.y = asuint(asint(r1.y) * asint(asfloat(X_height)) + asint(r0.x));
					r1.y = asuint(asint(r4.y) * asint(asfloat(X_width)) + asint(r1.y));
					r1.y = asuint(asint(r1.y) + asint(r4.z));
					r3.x = asuint(asfloat(WidthMaskX)) & r1.y;
					r3.y = r1.y >> asuint(asfloat(WidthShiftX));
					r3.zw = uint2(0u, 0u);
					r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				}
				else
				{
					r3 = uint4(0u, 0u, 0u, 0u);
				}
				r1.x = r1.x | r6.x;
				r1.x = r1.x | r2.x;
				[branch] if (r1.x == 0)
				{
					r1.x = asuint(asint(r4.x) * asint(asfloat(X_width)));
					r0.x = asuint(asint(r1.x) * asint(asfloat(X_height)) + asint(r0.x));
					r0.x = asuint(asint(r4.y) * asint(asfloat(X_width)) + asint(r0.x));
					r0.x = asuint(asint(r0.x) + asint(r4.z));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				}
				else
				{
					r1 = uint4(0u, 0u, 0u, 0u);
				}
				r2.xyz = asuint(float3(1.0, 1.0, 1.0) + -asfloat(r0.yzw));
				r4 = asuint(asfloat(r5) * asfloat(r2.xxxx));
				r5 = asuint(asfloat(r9) * asfloat(r2.xxxx));
				r5 = asuint(asfloat(r5) * asfloat(r0.zzzz));
				r4 = asuint(asfloat(r4) * asfloat(r2.yyyy) + asfloat(r5));
				r5 = asuint(asfloat(r11) * asfloat(r0.yyyy));
				r4 = asuint(asfloat(r5) * asfloat(r2.yyyy) + asfloat(r4));
				r3 = asuint(asfloat(r3) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r3) * asfloat(r0.zzzz) + asfloat(r4));
				r4 = asuint(asfloat(r7) * asfloat(r2.xxxx));
				r5 = asuint(asfloat(r10) * asfloat(r2.xxxx));
				r5 = asuint(asfloat(r5) * asfloat(r0.zzzz));
				r4 = asuint(asfloat(r4) * asfloat(r2.yyyy) + asfloat(r5));
				r5 = asuint(asfloat(r12) * asfloat(r0.yyyy));
				r4 = asuint(asfloat(r5) * asfloat(r2.yyyy) + asfloat(r4));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				r1 = asuint(asfloat(r1) * asfloat(r0.zzzz) + asfloat(r4));
				r0 = asuint(asfloat(r1) * asfloat(r0.wwww));
				o_SV_Target0 = asfloat(r3) * asfloat(r2.zzzz) + asfloat(r0);
			}
			#elif defined(ALIGN_CORNERS) && defined(BORDER) && defined(GRIDSAMPLE3D) && defined(LINEAR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yzw = asuint(max(float3(-1.0, -1.0, -1.0), asfloat(r1.xyz)));
				r0.yzw = asuint(min(float3(1.0, 1.0, 1.0), asfloat(r0.yzw)));
				r0.yzw = asuint(float3(1.0, 1.0, 1.0) + asfloat(r0.yzw));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.yzw));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r1.xyz = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r2.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.yzw));
				r2.xyz = asuint(floor(asfloat(r2.xyz)));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r1.xyz) + -asfloat(r2.xyz));
				r1.xyz = asuint(int3(asfloat(r2.xyz)));
				r1.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r1.w = asuint(asint(r1.w) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r0.x) * asint(r1.w));
				r2.xyz = asuint(int3(1, 1, 1) + asint(r1.xyz));
				r1.xyz = asuint(max(int3(0, 0, 0), asint(r1.xyz)));
				r3.xy = asuint(float2(asfloat(X_width), asfloat(X_height)));
				r3.z = asuint(asfloat(X_depth));
				r3.xyz = asuint(int3(-1, -1, -1) + asint(r3.xyz));
				r2.xyz = asuint(min(asint(r3.xyz), asint(r2.xyz)));
				r1.w = asuint(asint(r2.z) * asint(asfloat(X_width)));
				r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
				r2.z = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r1.w));
				r1.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r1.w));
				r2.w = asuint(asint(r2.z) + asint(r1.x));
				r2.z = asuint(asint(r2.z) + asint(r2.x));
				r3.x = asuint(asfloat(WidthMaskX)) & r2.w;
				r3.y = r2.w >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r4.xyz = asuint(float3(1.0, 1.0, 1.0) + -asfloat(r0.yzw));
				r3 = asuint(asfloat(r4.xxxx) * asfloat(r3));
				r3 = asuint(asfloat(r3) * asfloat(r0.zzzz));
				r2.w = asuint(asint(r1.w) + asint(r1.x));
				r1.w = asuint(asint(r1.w) + asint(r2.x));
				r5.x = asuint(asfloat(WidthMaskX)) & r2.w;
				r5.y = r2.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r4.xxxx));
				r3 = asuint(asfloat(r5) * asfloat(r4.yyyy) + asfloat(r3));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.w;
				r5.y = r1.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r5) * asfloat(r4.yyyy) + asfloat(r3));
				r5.x = asuint(asfloat(WidthMaskX)) & r2.z;
				r5.y = r2.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r5) * asfloat(r0.zzzz) + asfloat(r3));
				r3 = asuint(asfloat(r3) * asfloat(r0.wwww));
				r0.w = asuint(asint(r1.z) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.w) * asint(asfloat(X_height)) + asint(r0.x));
				r0.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r0.x));
				r1.y = asuint(asint(r0.w) + asint(r1.x));
				r0.w = asuint(asint(r0.w) + asint(r2.x));
				r1.z = asuint(asint(r0.x) + asint(r2.x));
				r0.x = asuint(asint(r0.x) + asint(r1.x));
				r2.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r2.y = r1.y >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r4.xxxx));
				r5.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r5.y = r0.x >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r4.xxxx));
				r5 = asuint(asfloat(r5) * asfloat(r0.zzzz));
				r2 = asuint(asfloat(r2) * asfloat(r4.yyyy) + asfloat(r5));
				r5.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r5.y = r0.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r2 = asuint(asfloat(r5) * asfloat(r4.yyyy) + asfloat(r2));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.z;
				r5.y = r1.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				r0 = asuint(asfloat(r1) * asfloat(r0.zzzz) + asfloat(r2));
				o_SV_Target0 = asfloat(r0) * asfloat(r4.zzzz) + asfloat(r3);
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE3D) && defined(LINEAR) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yzw = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r1.xyz = asuint(float3(4.0, 4.0, 4.0) * asfloat(r0.yzw));
				b1.xyz = (asfloat(r1.xyz) >= -asfloat(r1.xyz));
				r2.xyz = b1.xyz ? uint3(0x3e800000u, 0x3e800000u, 0x3e800000u) : uint3(0xbe800000u, 0xbe800000u, 0xbe800000u);
				r1.xyz = b1.xyz ? uint3(0x40800000u, 0x40800000u, 0x40800000u) : uint3(0xc0800000u, 0xc0800000u, 0xc0800000u);
				r0.yzw = asuint(asfloat(r2.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(frac(asfloat(r0.yzw)));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw) + float3(4.0, 4.0, 4.0));
				r1.xyz = asuint(float3(4.0, 4.0, 4.0) * asfloat(r0.yzw));
				b1.xyz = (asfloat(r1.xyz) >= -asfloat(r1.xyz));
				r2.xyz = b1.xyz ? uint3(0x3e800000u, 0x3e800000u, 0x3e800000u) : uint3(0xbe800000u, 0xbe800000u, 0xbe800000u);
				r1.xyz = b1.xyz ? uint3(0x40800000u, 0x40800000u, 0x40800000u) : uint3(0xc0800000u, 0xc0800000u, 0xc0800000u);
				r0.yzw = asuint(asfloat(r2.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(frac(asfloat(r0.yzw)));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw) + float3(-2.0, -2.0, -2.0));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * abs(asfloat(r0.yzw)));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r1.xyz = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r2.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.yzw));
				r2.xyz = asuint(floor(asfloat(r2.xyz)));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r1.xyz) + -asfloat(r2.xyz));
				r1.xyz = asuint(int3(asfloat(r2.xyz)));
				r2.xyz = asuint(float3(1.0, 1.0, 1.0) + -asfloat(r0.yzw));
				r3.xyz = asuint(int3(1, 1, 1) + asint(r1.xyz));
				r1.xyz = asuint(max(int3(0, 0, 0), asint(r1.xyz)));
				r4.xy = asuint(float2(asfloat(X_width), asfloat(X_height)));
				r4.z = asuint(asfloat(X_depth));
				r4.xyz = asuint(int3(-1, -1, -1) + asint(r4.xyz));
				r3.xyz = asuint(min(asint(r4.xyz), asint(r3.xyz)));
				r1.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r1.w = asuint(asint(r1.w) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r0.x) * asint(r1.w));
				r1.w = asuint(asint(r3.z) * asint(asfloat(X_width)));
				r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
				r2.w = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r1.w));
				r1.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r1.w));
				r3.z = asuint(asint(r2.w) + asint(r1.x));
				r2.w = asuint(asint(r2.w) + asint(r3.x));
				r4.x = asuint(asfloat(WidthMaskX)) & r3.z;
				r4.y = r3.z >> asuint(asfloat(WidthShiftX));
				r4.zw = uint2(0u, 0u);
				r4 = asuint(Xptr.Load(int3(asint(r4.xy), asint(r4.w))));
				r4 = asuint(asfloat(r4) * asfloat(r2.xxxx));
				r4 = asuint(asfloat(r4) * asfloat(r0.zzzz));
				r3.z = asuint(asint(r1.w) + asint(r1.x));
				r1.w = asuint(asint(r1.w) + asint(r3.x));
				r5.x = asuint(asfloat(WidthMaskX)) & r3.z;
				r5.y = r3.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r2.xxxx));
				r4 = asuint(asfloat(r5) * asfloat(r2.yyyy) + asfloat(r4));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.w;
				r5.y = r1.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r4 = asuint(asfloat(r5) * asfloat(r2.yyyy) + asfloat(r4));
				r5.x = asuint(asfloat(WidthMaskX)) & r2.w;
				r5.y = r2.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r4 = asuint(asfloat(r5) * asfloat(r0.zzzz) + asfloat(r4));
				r4 = asuint(asfloat(r4) * asfloat(r0.wwww));
				r0.w = asuint(asint(r1.z) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.w) * asint(asfloat(X_height)) + asint(r0.x));
				r0.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r0.x));
				r1.y = asuint(asint(r0.w) + asint(r1.x));
				r0.w = asuint(asint(r0.w) + asint(r3.x));
				r1.z = asuint(asint(r0.x) + asint(r3.x));
				r0.x = asuint(asint(r0.x) + asint(r1.x));
				r3.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r3.y = r1.y >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r2.xxxx));
				r5.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r5.y = r0.x >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r2.xxxx));
				r5 = asuint(asfloat(r5) * asfloat(r0.zzzz));
				r3 = asuint(asfloat(r3) * asfloat(r2.yyyy) + asfloat(r5));
				r5.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r5.y = r0.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r5) * asfloat(r2.yyyy) + asfloat(r3));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.z;
				r5.y = r1.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				r0 = asuint(asfloat(r1) * asfloat(r0.zzzz) + asfloat(r3));
				o_SV_Target0 = asfloat(r0) * asfloat(r2.zzzz) + asfloat(r4);
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE3D) && defined(NEAREST) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b2;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.y = r0.x / asuint(asfloat(O_channelsDiv4));
				r0.z = asuint(asint(r0.y) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.z = asuint(asint(r0.z) * asint(asfloat(O_height)) + asint(r2.x));
				r0.z = asuint(asint(r0.z) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.z;
				r1.y = r0.z >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1.xyz = asuint(float3(1.0, 1.0, 1.0) + asfloat(r1.xyz));
				r1.xyz = asuint(float3(0.5, 0.5, 0.5) * asfloat(r1.xyz));
				r2.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r2.z = asuint(float(asint(asfloat(X_depth))));
				r2.xyz = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r2.xyz));
				r1.xyz = asuint(asfloat(r2.xyz) * asfloat(r1.xyz));
				r1.xyz = asuint(round(asfloat(r1.xyz)));
				r1.xyz = asuint(int3(asfloat(r1.xyz)));
				b2.xyz = (asint(r1.xyz) < int3(0, 0, 0));
				b0.zw = (r1.xy >= asuint(float2(asfloat(X_width), asfloat(X_height))));
				b0.z = b2.x || b0.z;
				b0.z = b0.z || b2.y;
				b0.z = b0.z || b0.w;
				b0.z = b0.z || b2.z;
				b0.w = (r1.z >= asuint(asfloat(X_depth)));
				b0.z = b0.z || b0.w;
				[branch] if (!b0.z)
				{
					r0.x = r0.x % asuint(asfloat(O_channelsDiv4));
					r0.z = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
					r0.z = asuint(asint(r0.z) * asint(asfloat(X_depth)));
					r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.y) + asint(r0.x));
					r0.y = asuint(asint(r1.z) * asint(asfloat(X_width)));
					r0.y = asuint(asint(r0.y) * asint(asfloat(X_height)));
					r0.x = asuint(asint(r0.z) * asint(r0.x) + asint(r0.y));
					r0.x = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
					r0.x = asuint(asint(r0.x) + asint(r1.x));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
				}
				else
				{
					o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			#elif defined(ALIGN_CORNERS) && defined(BORDER) && defined(GRIDSAMPLE3D) && defined(NEAREST)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yzw = asuint(max(float3(-1.0, -1.0, -1.0), asfloat(r1.xyz)));
				r0.yzw = asuint(min(float3(1.0, 1.0, 1.0), asfloat(r0.yzw)));
				r0.yzw = asuint(float3(1.0, 1.0, 1.0) + asfloat(r0.yzw));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.yzw));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r1.xyz = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(round(asfloat(r0.yzw)));
				r0.yzw = asuint(int3(asfloat(r0.yzw)));
				r0.yzw = asuint(max(int3(0, 0, 0), asint(r0.yzw)));
				r1.xy = asuint(float2(asfloat(X_width), asfloat(X_height)));
				r1.z = asuint(asfloat(X_depth));
				r1.xyz = asuint(int3(-1, -1, -1) + asint(r1.xyz));
				r0.yzw = asuint(min(asint(r1.xyz), asint(r0.yzw)));
				r0.w = asuint(asint(r0.w) * asint(asfloat(X_width)));
				r0.w = asuint(asint(r0.w) * asint(asfloat(X_height)));
				r1.x = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r1.x = asuint(asint(r1.x) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r1.x) * asint(r0.x) + asint(r0.w));
				r0.x = asuint(asint(r0.z) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(ALIGN_CORNERS) && defined(GRIDSAMPLE3D) && defined(NEAREST) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yzw = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r1.xyz = asuint(float3(4.0, 4.0, 4.0) * asfloat(r0.yzw));
				b1.xyz = (asfloat(r1.xyz) >= -asfloat(r1.xyz));
				r2.xyz = b1.xyz ? uint3(0x3e800000u, 0x3e800000u, 0x3e800000u) : uint3(0xbe800000u, 0xbe800000u, 0xbe800000u);
				r1.xyz = b1.xyz ? uint3(0x40800000u, 0x40800000u, 0x40800000u) : uint3(0xc0800000u, 0xc0800000u, 0xc0800000u);
				r0.yzw = asuint(asfloat(r2.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(frac(asfloat(r0.yzw)));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw) + float3(4.0, 4.0, 4.0));
				r1.xyz = asuint(float3(4.0, 4.0, 4.0) * asfloat(r0.yzw));
				b1.xyz = (asfloat(r1.xyz) >= -asfloat(r1.xyz));
				r2.xyz = b1.xyz ? uint3(0x3e800000u, 0x3e800000u, 0x3e800000u) : uint3(0xbe800000u, 0xbe800000u, 0xbe800000u);
				r1.xyz = b1.xyz ? uint3(0x40800000u, 0x40800000u, 0x40800000u) : uint3(0xc0800000u, 0xc0800000u, 0xc0800000u);
				r0.yzw = asuint(asfloat(r2.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(frac(asfloat(r0.yzw)));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw) + float3(-2.0, -2.0, -2.0));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * abs(asfloat(r0.yzw)));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r1.xyz = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(round(asfloat(r0.yzw)));
				r0.yzw = asuint(int3(asfloat(r0.yzw)));
				r0.yzw = asuint(max(int3(0, 0, 0), asint(r0.yzw)));
				r1.xy = asuint(float2(asfloat(X_width), asfloat(X_height)));
				r1.z = asuint(asfloat(X_depth));
				r1.xyz = asuint(int3(-1, -1, -1) + asint(r1.xyz));
				r0.yzw = asuint(min(asint(r1.xyz), asint(r0.yzw)));
				r0.w = asuint(asint(r0.w) * asint(asfloat(X_width)));
				r0.w = asuint(asint(r0.w) * asint(asfloat(X_height)));
				r1.x = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r1.x = asuint(asint(r1.x) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r1.x) * asint(r0.x) + asint(r0.w));
				r0.x = asuint(asint(r0.z) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(GRIDSAMPLE1D) && defined(LINEAR) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r0.y = asuint(1.0 + asfloat(r1.x));
				r0.y = asuint(0.5 * asfloat(r0.y));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z) + -0.5);
				r0.z = asuint(floor(asfloat(r0.y)));
				r0.w = asuint(int(asfloat(r0.z)));
				r1.x = asuint(1 + asint(r0.w));
				r0.y = asuint(asfloat(r0.y) + -asfloat(r0.z));
				b0.z = (asint(r0.w) < 0);
				b1.y = (r0.w >= asuint(asfloat(X_width)));
				b0.z = b1.y || b0.z;
				r1.y = b1.y ? 0xffffffffu : 0u;
				[branch] if (!b0.z)
				{
					r0.z = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.w));
					r2.x = asuint(asfloat(WidthMaskX)) & r0.z;
					r2.y = r0.z >> asuint(asfloat(WidthShiftX));
					r2.zw = uint2(0u, 0u);
					r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				}
				else
				{
					r2 = uint4(0u, 0u, 0u, 0u);
				}
				b0.z = (asint(r1.x) < 0);
				b0.w = (r1.x >= asuint(asfloat(X_width)));
				b0.z = b0.z || b0.w;
				[branch] if (!b0.z)
				{
					r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r1.x));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				}
				else
				{
					r1 = uint4(0u, 0u, 0u, 0u);
				}
				r0.x = asuint(1.0 + -asfloat(r0.y));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#elif defined(BORDER) && defined(GRIDSAMPLE1D) && defined(LINEAR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(max(-1.0, asfloat(r1.x)));
				r0.y = asuint(min(1.0, asfloat(r0.y)));
				r0.y = asuint(1.0 + asfloat(r0.y));
				r0.y = asuint(0.5 * asfloat(r0.y));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z) + -0.5);
				r0.z = asuint(floor(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.y) + -asfloat(r0.z));
				r0.z = asuint(int(asfloat(r0.z)));
				r0.w = asuint(1 + asint(r0.z));
				r0.z = asuint(max(0, asint(r0.z)));
				r0.z = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.z));
				r1.x = asuint(-1 + asint(asfloat(X_width)));
				r0.w = asuint(min(asint(r1.x), asint(r0.w)));
				r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.w));
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
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#elif defined(GRIDSAMPLE1D) && defined(LINEAR) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(-1.0 + asfloat(r1.x));
				r0.z = asuint(4.0 * asfloat(r0.y));
				b0.z = (asfloat(r0.z) >= -asfloat(r0.z));
				r0.zw = b0.zz ? uint2(0x40800000u, 0x3e800000u) : uint2(0xc0800000u, 0xbe800000u);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y) + 4.0);
				r0.z = asuint(4.0 * asfloat(r0.y));
				b0.z = (asfloat(r0.z) >= -asfloat(r0.z));
				r0.zw = b0.zz ? uint2(0x40800000u, 0x3e800000u) : uint2(0xc0800000u, 0xbe800000u);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y) + -2.0);
				r0.y = asuint(0.5 * abs(asfloat(r0.y)));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z) + -0.5);
				r0.z = asuint(floor(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.y) + -asfloat(r0.z));
				r0.z = asuint(int(asfloat(r0.z)));
				r0.w = asuint(1 + asint(r0.z));
				r0.z = asuint(max(0, asint(r0.z)));
				r0.z = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.z));
				r1.x = asuint(-1 + asint(asfloat(X_width)));
				r0.w = asuint(min(asint(r1.x), asint(r0.w)));
				r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.w));
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
				o_SV_Target0 = asfloat(r0.xxxx) * asfloat(r2) + asfloat(r1);
			}
			#elif defined(GRIDSAMPLE1D) && defined(NEAREST) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.y = r0.x / asuint(asfloat(O_channelsDiv4));
				r0.z = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.z;
				r1.y = r0.z >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.z = asuint(1.0 + asfloat(r1.x));
				r0.z = asuint(0.5 * asfloat(r0.z));
				r0.w = asuint(float(asint(asfloat(X_width))));
				r0.z = asuint(asfloat(r0.z) * asfloat(r0.w) + -0.5);
				r0.z = asuint(round(asfloat(r0.z)));
				r0.z = asuint(int(asfloat(r0.z)));
				b0.w = (asint(r0.z) < 0);
				b1.x = (r0.z >= asuint(asfloat(X_width)));
				b0.w = b1.x || b0.w;
				[branch] if (!b0.w)
				{
					r0.x = r0.x % asuint(asfloat(O_channelsDiv4));
					r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.y) + asint(r0.x));
					r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.z));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
				}
				else
				{
					o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			#elif defined(BORDER) && defined(GRIDSAMPLE1D) && defined(NEAREST)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(max(-1.0, asfloat(r1.x)));
				r0.y = asuint(min(1.0, asfloat(r0.y)));
				r0.y = asuint(1.0 + asfloat(r0.y));
				r0.y = asuint(0.5 * asfloat(r0.y));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z) + -0.5);
				r0.y = asuint(round(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.y = asuint(max(0, asint(r0.y)));
				r0.z = asuint(-1 + asint(asfloat(X_width)));
				r0.y = asuint(min(asint(r0.y), asint(r0.z)));
				r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(GRIDSAMPLE1D) && defined(NEAREST) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_width)) + asint(r1.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(-1.0 + asfloat(r1.x));
				r0.z = asuint(4.0 * asfloat(r0.y));
				b0.z = (asfloat(r0.z) >= -asfloat(r0.z));
				r0.zw = b0.zz ? uint2(0x40800000u, 0x3e800000u) : uint2(0xc0800000u, 0xbe800000u);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y) + 4.0);
				r0.z = asuint(4.0 * asfloat(r0.y));
				b0.z = (asfloat(r0.z) >= -asfloat(r0.z));
				r0.zw = b0.zz ? uint2(0x40800000u, 0x3e800000u) : uint2(0xc0800000u, 0xbe800000u);
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.y = asuint(frac(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.y) + -2.0);
				r0.y = asuint(0.5 * abs(asfloat(r0.y)));
				r0.z = asuint(float(asint(asfloat(X_width))));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.z) + -0.5);
				r0.y = asuint(round(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.y = asuint(max(0, asint(r0.y)));
				r0.z = asuint(-1 + asint(asfloat(X_width)));
				r0.y = asuint(min(asint(r0.y), asint(r0.z)));
				r0.x = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(GRIDSAMPLE2D) && defined(LINEAR) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				bool4 b2;
				bool4 b4;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.x = asuint(asint(r0.x) * asint(r0.y));
				r0.yz = asuint(float2(1.0, 1.0) + asfloat(r1.xy));
				r0.yz = asuint(float2(0.5, 0.5) * asfloat(r0.yz));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r0.yz = asuint(asfloat(r0.yz) * asfloat(r1.xy) + float2(-0.5, -0.5));
				r1.xy = asuint(floor(asfloat(r0.yz)));
				r1.zw = asuint(int2(asfloat(r1.xy)));
				r2.xy = asuint(int2(1, 1) + asint(r1.wz));
				r0.yz = asuint(-asfloat(r1.xy) + asfloat(r0.yz));
				b1.xy = (asint(r1.zw) < int2(0, 0));
				b2.zw = (r1.zw >= asuint(float2(asfloat(X_width), asfloat(X_height))));
				b0.w = b2.z || b1.x;
				b1.x = b0.w || b1.y;
				b1.x = b1.x || b2.w;
				r0.w = b0.w ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r2.z = b2.z ? 0xffffffffu : 0u;
				r2.w = b2.w ? 0xffffffffu : 0u;
				[branch] if (!b1.x)
				{
					r1.x = asuint(asint(r1.w) * asint(asfloat(X_width)) + asint(r0.x));
					r1.x = asuint(asint(r1.x) + asint(r1.z));
					r3.x = asuint(asfloat(WidthMaskX)) & r1.x;
					r3.y = r1.x >> asuint(asfloat(WidthShiftX));
					r3.zw = uint2(0u, 0u);
					r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				}
				else
				{
					r3 = uint4(0u, 0u, 0u, 0u);
				}
				b4.xy = (asint(r2.xy) < int2(0, 0));
				r4.x = b4.x ? 0xffffffffu : 0u;
				r0.w = r4.x | r0.w;
				b4.zw = (r2.xy >= asuint(float2(asfloat(X_height), asfloat(X_width))));
				r4.z = b4.z ? 0xffffffffu : 0u;
				r0.w = r4.z | r0.w;
				r4.y = b4.y ? 0xffffffffu : 0u;
				r4.w = b4.w ? 0xffffffffu : 0u;
				[branch] if (r0.w == 0)
				{
					r0.w = asuint(asint(r2.x) * asint(asfloat(X_width)) + asint(r0.x));
					r0.w = asuint(asint(r0.w) + asint(r1.z));
					r5.x = asuint(asfloat(WidthMaskX)) & r0.w;
					r5.y = r0.w >> asuint(asfloat(WidthShiftX));
					r5.zw = uint2(0u, 0u);
					r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				}
				else
				{
					r5 = uint4(0u, 0u, 0u, 0u);
				}
				r0.w = r4.y | r4.w;
				r1.x = r0.w | r1.y;
				r1.x = r1.x | r2.w;
				[branch] if (r1.x == 0)
				{
					r1.x = asuint(asint(r1.w) * asint(asfloat(X_width)) + asint(r0.x));
					r1.x = asuint(asint(r1.x) + asint(r2.y));
					r6.x = asuint(asfloat(WidthMaskX)) & r1.x;
					r6.y = r1.x >> asuint(asfloat(WidthShiftX));
					r6.zw = uint2(0u, 0u);
					r1 = asuint(Xptr.Load(int3(asint(r6.xy), asint(r6.w))));
				}
				else
				{
					r1 = uint4(0u, 0u, 0u, 0u);
				}
				r0.w = r0.w | r4.x;
				r0.w = r0.w | r4.z;
				[branch] if (r0.w == 0)
				{
					r0.x = asuint(asint(r2.x) * asint(asfloat(X_width)) + asint(r0.x));
					r0.x = asuint(asint(r0.x) + asint(r2.y));
					r2.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r2.y = r0.x >> asuint(asfloat(WidthShiftX));
					r2.zw = uint2(0u, 0u);
					r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				}
				else
				{
					r2 = uint4(0u, 0u, 0u, 0u);
				}
				r0.xw = asuint(float2(1.0, 1.0) + -asfloat(r0.yz));
				r3 = asuint(asfloat(r3) * asfloat(r0.xxxx));
				r4 = asuint(asfloat(r5) * asfloat(r0.xxxx));
				r4 = asuint(asfloat(r4) * asfloat(r0.zzzz));
				r3 = asuint(asfloat(r3) * asfloat(r0.wwww) + asfloat(r4));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				r1 = asuint(asfloat(r1) * asfloat(r0.wwww) + asfloat(r3));
				r2 = asuint(asfloat(r2) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r2) * asfloat(r0.zzzz) + asfloat(r1);
			}
			#elif defined(BORDER) && defined(GRIDSAMPLE2D) && defined(LINEAR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yz = asuint(max(float2(-1.0, -1.0), asfloat(r1.xy)));
				r0.yz = asuint(min(float2(1.0, 1.0), asfloat(r0.yz)));
				r0.yz = asuint(float2(1.0, 1.0) + asfloat(r0.yz));
				r0.yz = asuint(float2(0.5, 0.5) * asfloat(r0.yz));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r0.yz = asuint(asfloat(r0.yz) * asfloat(r1.xy) + float2(-0.5, -0.5));
				r1.xy = asuint(floor(asfloat(r0.yz)));
				r0.yz = asuint(-asfloat(r1.xy) + asfloat(r0.yz));
				r1.xy = asuint(int2(asfloat(r1.xy)));
				r1.zw = asuint(max(int2(0, 0), asint(r1.xy)));
				r1.xy = asuint(int2(1, 1) + asint(r1.xy));
				r2.xy = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r1.xy = asuint(min(asint(r2.xy), asint(r1.xy)));
				r0.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.x) * asint(r0.w));
				r0.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r1.w) * asint(asfloat(X_width)) + asint(r0.x));
				r1.y = asuint(asint(r0.w) + asint(r1.z));
				r1.z = asuint(asint(r0.x) + asint(r1.z));
				r0.xw = asuint(asint(r0.xw) + asint(r1.xx));
				r2.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r2.y = r1.y >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r1.xy = asuint(float2(1.0, 1.0) + -asfloat(r0.yz));
				r2 = asuint(asfloat(r2) * asfloat(r1.xxxx));
				r2 = asuint(asfloat(r2) * asfloat(r0.zzzz));
				r3.x = asuint(asfloat(WidthMaskX)) & r1.z;
				r3.y = r1.z >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r1.xxxx));
				r2 = asuint(asfloat(r3) * asfloat(r1.yyyy) + asfloat(r2));
				r3.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r3.y = r0.x >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r0.yyyy));
				r1 = asuint(asfloat(r3) * asfloat(r1.yyyy) + asfloat(r2));
				r2.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r2.y = r0.w >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r2) * asfloat(r0.zzzz) + asfloat(r1);
			}
			#elif defined(GRIDSAMPLE2D) && defined(LINEAR) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yz = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r1 = asuint(float4(4.0, 4.0, 4.0, 4.0) * asfloat(r0.yzyz));
				b1 = (asfloat(r1) >= -asfloat(r1.zwzw));
				r1 = b1 ? uint4(0x40800000u, 0x40800000u, 0x3e800000u, 0x3e800000u) : uint4(0xc0800000u, 0xc0800000u, 0xbe800000u, 0xbe800000u);
				r0.yz = asuint(asfloat(r1.zw) * asfloat(r0.yz));
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz) + float2(4.0, 4.0));
				r1 = asuint(float4(4.0, 4.0, 4.0, 4.0) * asfloat(r0.yzyz));
				b1 = (asfloat(r1) >= -asfloat(r1.zwzw));
				r1 = b1 ? uint4(0x40800000u, 0x40800000u, 0x3e800000u, 0x3e800000u) : uint4(0xc0800000u, 0xc0800000u, 0xbe800000u, 0xbe800000u);
				r0.yz = asuint(asfloat(r1.zw) * asfloat(r0.yz));
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz) + float2(-2.0, -2.0));
				r0.yz = asuint(float2(0.5, 0.5) * abs(asfloat(r0.yz)));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r0.yz = asuint(asfloat(r0.yz) * asfloat(r1.xy) + float2(-0.5, -0.5));
				r1.xy = asuint(floor(asfloat(r0.yz)));
				r0.yz = asuint(-asfloat(r1.xy) + asfloat(r0.yz));
				r1.xy = asuint(int2(asfloat(r1.xy)));
				r1.zw = asuint(max(int2(0, 0), asint(r1.xy)));
				r1.xy = asuint(int2(1, 1) + asint(r1.xy));
				r2.xy = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r1.xy = asuint(min(asint(r2.xy), asint(r1.xy)));
				r0.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.x) * asint(r0.w));
				r0.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r1.w) * asint(asfloat(X_width)) + asint(r0.x));
				r1.y = asuint(asint(r0.w) + asint(r1.z));
				r1.z = asuint(asint(r0.x) + asint(r1.z));
				r0.xw = asuint(asint(r0.xw) + asint(r1.xx));
				r2.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r2.y = r1.y >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r1.xy = asuint(float2(1.0, 1.0) + -asfloat(r0.yz));
				r2 = asuint(asfloat(r2) * asfloat(r1.xxxx));
				r2 = asuint(asfloat(r2) * asfloat(r0.zzzz));
				r3.x = asuint(asfloat(WidthMaskX)) & r1.z;
				r3.y = r1.z >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r1.xxxx));
				r2 = asuint(asfloat(r3) * asfloat(r1.yyyy) + asfloat(r2));
				r3.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r3.y = r0.x >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r0.yyyy));
				r1 = asuint(asfloat(r3) * asfloat(r1.yyyy) + asfloat(r2));
				r2.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r2.y = r0.w >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r2) * asfloat(r0.zzzz) + asfloat(r1);
			}
			#elif defined(GRIDSAMPLE2D) && defined(NEAREST) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.y = r0.x / asuint(asfloat(O_channelsDiv4));
				r0.z = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.z = asuint(asint(r0.z) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.z;
				r1.y = r0.z >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.zw = asuint(float2(1.0, 1.0) + asfloat(r1.xy));
				r0.zw = asuint(float2(0.5, 0.5) * asfloat(r0.zw));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r0.zw = asuint(asfloat(r0.zw) * asfloat(r1.xy) + float2(-0.5, -0.5));
				r0.zw = asuint(round(asfloat(r0.zw)));
				r0.zw = asuint(int2(asfloat(r0.zw)));
				b1.xy = (asint(r0.zw) < int2(0, 0));
				b1.zw = (r0.zw >= asuint(float2(asfloat(X_width), asfloat(X_height))));
				b1.x = b1.x || b1.z;
				b1.x = b1.x || b1.y;
				b1.x = b1.x || b1.w;
				[branch] if (!b1.x)
				{
					r0.x = r0.x % asuint(asfloat(O_channelsDiv4));
					r1.x = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
					r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.y) + asint(r0.x));
					r0.y = asuint(asint(r0.w) * asint(asfloat(X_width)));
					r0.x = asuint(asint(r1.x) * asint(r0.x) + asint(r0.y));
					r0.x = asuint(asint(r0.x) + asint(r0.z));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
				}
				else
				{
					o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			#elif defined(BORDER) && defined(GRIDSAMPLE2D) && defined(NEAREST)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yz = asuint(max(float2(-1.0, -1.0), asfloat(r1.xy)));
				r0.yz = asuint(min(float2(1.0, 1.0), asfloat(r0.yz)));
				r0.yz = asuint(float2(1.0, 1.0) + asfloat(r0.yz));
				r0.yz = asuint(float2(0.5, 0.5) * asfloat(r0.yz));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r0.yz = asuint(asfloat(r0.yz) * asfloat(r1.xy) + float2(-0.5, -0.5));
				r0.yz = asuint(round(asfloat(r0.yz)));
				r0.yz = asuint(int2(asfloat(r0.yz)));
				r0.yz = asuint(max(int2(0, 0), asint(r0.yz)));
				r1.xy = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r0.yz = asuint(min(asint(r1.xy), asint(r0.yz)));
				r0.z = asuint(asint(r0.z) * asint(asfloat(X_width)));
				r0.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.w) * asint(r0.x) + asint(r0.z));
				r0.x = asuint(asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(GRIDSAMPLE2D) && defined(NEAREST) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r3.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_height)) + asint(r2.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yz = asuint(float2(-1.0, -1.0) + asfloat(r1.xy));
				r1 = asuint(float4(4.0, 4.0, 4.0, 4.0) * asfloat(r0.yzyz));
				b1 = (asfloat(r1) >= -asfloat(r1.zwzw));
				r1 = b1 ? uint4(0x40800000u, 0x40800000u, 0x3e800000u, 0x3e800000u) : uint4(0xc0800000u, 0xc0800000u, 0xbe800000u, 0xbe800000u);
				r0.yz = asuint(asfloat(r1.zw) * asfloat(r0.yz));
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz) + float2(4.0, 4.0));
				r1 = asuint(float4(4.0, 4.0, 4.0, 4.0) * asfloat(r0.yzyz));
				b1 = (asfloat(r1) >= -asfloat(r1.zwzw));
				r1 = b1 ? uint4(0x40800000u, 0x40800000u, 0x3e800000u, 0x3e800000u) : uint4(0xc0800000u, 0xc0800000u, 0xbe800000u, 0xbe800000u);
				r0.yz = asuint(asfloat(r1.zw) * asfloat(r0.yz));
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r0.yz = asuint(asfloat(r1.xy) * asfloat(r0.yz) + float2(-2.0, -2.0));
				r0.yz = asuint(float2(0.5, 0.5) * abs(asfloat(r0.yz)));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r0.yz = asuint(asfloat(r0.yz) * asfloat(r1.xy) + float2(-0.5, -0.5));
				r0.yz = asuint(round(asfloat(r0.yz)));
				r0.yz = asuint(int2(asfloat(r0.yz)));
				r0.yz = asuint(max(int2(0, 0), asint(r0.yz)));
				r1.xy = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r0.yz = asuint(min(asint(r1.xy), asint(r0.yz)));
				r0.z = asuint(asint(r0.z) * asint(asfloat(X_width)));
				r0.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.w) * asint(r0.x) + asint(r0.z));
				r0.x = asuint(asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(GRIDSAMPLE3D) && defined(LINEAR) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				bool4 b2;
				bool4 b3;
				bool4 b4;
				bool4 b7;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.y = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.y = asuint(asint(r0.y) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.x = asuint(asint(r0.x) * asint(r0.y));
				r0.yzw = asuint(float3(1.0, 1.0, 1.0) + asfloat(r1.xyz));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.yzw));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r1.xyz) + float3(-0.5, -0.5, -0.5));
				r1.xyz = asuint(floor(asfloat(r0.yzw)));
				r2.xyz = asuint(int3(asfloat(r1.xyz)));
				r3.xyz = asuint(int3(1, 1, 1) + asint(r2.zyx));
				r0.yzw = asuint(-asfloat(r1.xyz) + asfloat(r0.yzw));
				b1.xyz = (asint(r2.xyz) < int3(0, 0, 0));
				b4.xy = (r2.xy >= asuint(float2(asfloat(X_width), asfloat(X_height))));
				b1.x = b4.x || b1.x;
				b1.w = b1.x || b1.y;
				b1.w = b1.w || b4.y;
				b2.w = b1.w || b1.z;
				b3.w = (r2.z >= asuint(asfloat(X_depth)));
				b2.w = b3.w || b2.w;
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				r3.w = b3.w ? 0xffffffffu : 0u;
				r4.y = b4.y ? 0xffffffffu : 0u;
				[branch] if (!b2.w)
				{
					r2.w = asuint(asint(r2.z) * asint(asfloat(X_width)));
					r2.w = asuint(asint(r2.w) * asint(asfloat(X_height)) + asint(r0.x));
					r2.w = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r2.w));
					r2.w = asuint(asint(r2.w) + asint(r2.x));
					r5.x = asuint(asfloat(WidthMaskX)) & r2.w;
					r5.y = r2.w >> asuint(asfloat(WidthShiftX));
					r5.zw = uint2(0u, 0u);
					r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				}
				else
				{
					r5 = uint4(0u, 0u, 0u, 0u);
				}
				b4.xzw = (asint(r3.xyz) < int3(0, 0, 0));
				r4.x = b4.x ? 0xffffffffu : 0u;
				r4.z = b4.z ? 0xffffffffu : 0u;
				r1.xw = r4.zx | r1.xw;
				b2.w = (r3.x >= asuint(asfloat(X_depth)));
				r2.w = b2.w ? 0xffffffffu : 0u;
				r1.w = r2.w | r1.w;
				r4.w = b4.w ? 0xffffffffu : 0u;
				[branch] if (r1.w == 0)
				{
					r1.w = asuint(asint(r3.x) * asint(asfloat(X_width)));
					r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
					r1.w = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r1.w));
					r1.w = asuint(asint(r1.w) + asint(r2.x));
					r6.x = asuint(asfloat(WidthMaskX)) & r1.w;
					r6.y = r1.w >> asuint(asfloat(WidthShiftX));
					r6.zw = uint2(0u, 0u);
					r6 = asuint(Xptr.Load(int3(asint(r6.xy), asint(r6.w))));
				}
				else
				{
					r6 = uint4(0u, 0u, 0u, 0u);
				}
				b7.xy = (r3.yz >= asuint(float2(asfloat(X_height), asfloat(X_width))));
				r7.x = b7.x ? 0xffffffffu : 0u;
				r1.x = r7.x | r1.x;
				r1.w = r1.x | r1.z;
				r1.w = r1.w | r3.w;
				r7.y = b7.y ? 0xffffffffu : 0u;
				[branch] if (r1.w == 0)
				{
					r1.w = asuint(asint(r2.z) * asint(asfloat(X_width)));
					r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
					r1.w = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r1.w));
					r1.w = asuint(asint(r1.w) + asint(r2.x));
					r8.x = asuint(asfloat(WidthMaskX)) & r1.w;
					r8.y = r1.w >> asuint(asfloat(WidthShiftX));
					r8.zw = uint2(0u, 0u);
					r8 = asuint(Xptr.Load(int3(asint(r8.xy), asint(r8.w))));
				}
				else
				{
					r8 = uint4(0u, 0u, 0u, 0u);
				}
				r1.x = r1.x | r4.x;
				r1.x = r1.x | r2.w;
				[branch] if (r1.x == 0)
				{
					r1.x = asuint(asint(r3.x) * asint(asfloat(X_width)));
					r1.x = asuint(asint(r1.x) * asint(asfloat(X_height)) + asint(r0.x));
					r1.x = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r1.x));
					r1.x = asuint(asint(r1.x) + asint(r2.x));
					r9.x = asuint(asfloat(WidthMaskX)) & r1.x;
					r9.y = r1.x >> asuint(asfloat(WidthShiftX));
					r9.zw = uint2(0u, 0u);
					r9 = asuint(Xptr.Load(int3(asint(r9.xy), asint(r9.w))));
				}
				else
				{
					r9 = uint4(0u, 0u, 0u, 0u);
				}
				r1.x = r7.y | r4.w;
				r1.y = r1.x | r1.y;
				r1.xy = r1.xy | r4.zy;
				r1.w = r1.y | r1.z;
				r1.w = r1.w | r3.w;
				[branch] if (r1.w == 0)
				{
					r1.w = asuint(asint(r2.z) * asint(asfloat(X_width)));
					r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
					r1.w = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r1.w));
					r1.w = asuint(asint(r1.w) + asint(r3.z));
					r10.x = asuint(asfloat(WidthMaskX)) & r1.w;
					r10.y = r1.w >> asuint(asfloat(WidthShiftX));
					r10.zw = uint2(0u, 0u);
					r10 = asuint(Xptr.Load(int3(asint(r10.xy), asint(r10.w))));
				}
				else
				{
					r10 = uint4(0u, 0u, 0u, 0u);
				}
				r1.y = r1.y | r4.x;
				r1.y = r1.y | r2.w;
				[branch] if (r1.y == 0)
				{
					r1.y = asuint(asint(r3.x) * asint(asfloat(X_width)));
					r1.y = asuint(asint(r1.y) * asint(asfloat(X_height)) + asint(r0.x));
					r1.y = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r1.y));
					r1.y = asuint(asint(r1.y) + asint(r3.z));
					r11.x = asuint(asfloat(WidthMaskX)) & r1.y;
					r11.y = r1.y >> asuint(asfloat(WidthShiftX));
					r11.zw = uint2(0u, 0u);
					r11 = asuint(Xptr.Load(int3(asint(r11.xy), asint(r11.w))));
				}
				else
				{
					r11 = uint4(0u, 0u, 0u, 0u);
				}
				r1.x = r1.x | r7.x;
				r1.y = r1.x | r1.z;
				r1.y = r1.y | r3.w;
				[branch] if (r1.y == 0)
				{
					r1.y = asuint(asint(r2.z) * asint(asfloat(X_width)));
					r1.y = asuint(asint(r1.y) * asint(asfloat(X_height)) + asint(r0.x));
					r1.y = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r1.y));
					r1.y = asuint(asint(r1.y) + asint(r3.z));
					r7.x = asuint(asfloat(WidthMaskX)) & r1.y;
					r7.y = r1.y >> asuint(asfloat(WidthShiftX));
					r7.zw = uint2(0u, 0u);
					r7 = asuint(Xptr.Load(int3(asint(r7.xy), asint(r7.w))));
				}
				else
				{
					r7 = uint4(0u, 0u, 0u, 0u);
				}
				r1.x = r1.x | r4.x;
				r1.x = r1.x | r2.w;
				[branch] if (r1.x == 0)
				{
					r1.x = asuint(asint(r3.x) * asint(asfloat(X_width)));
					r0.x = asuint(asint(r1.x) * asint(asfloat(X_height)) + asint(r0.x));
					r0.x = asuint(asint(r3.y) * asint(asfloat(X_width)) + asint(r0.x));
					r0.x = asuint(asint(r0.x) + asint(r3.z));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					r1 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				}
				else
				{
					r1 = uint4(0u, 0u, 0u, 0u);
				}
				r2.xyz = asuint(float3(1.0, 1.0, 1.0) + -asfloat(r0.yzw));
				r3 = asuint(asfloat(r5) * asfloat(r2.xxxx));
				r4 = asuint(asfloat(r8) * asfloat(r2.xxxx));
				r4 = asuint(asfloat(r4) * asfloat(r0.zzzz));
				r3 = asuint(asfloat(r3) * asfloat(r2.yyyy) + asfloat(r4));
				r4 = asuint(asfloat(r10) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r4) * asfloat(r2.yyyy) + asfloat(r3));
				r4 = asuint(asfloat(r7) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r4) * asfloat(r0.zzzz) + asfloat(r3));
				r4 = asuint(asfloat(r6) * asfloat(r2.xxxx));
				r5 = asuint(asfloat(r9) * asfloat(r2.xxxx));
				r5 = asuint(asfloat(r5) * asfloat(r0.zzzz));
				r4 = asuint(asfloat(r4) * asfloat(r2.yyyy) + asfloat(r5));
				r5 = asuint(asfloat(r11) * asfloat(r0.yyyy));
				r4 = asuint(asfloat(r5) * asfloat(r2.yyyy) + asfloat(r4));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				r1 = asuint(asfloat(r1) * asfloat(r0.zzzz) + asfloat(r4));
				r0 = asuint(asfloat(r1) * asfloat(r0.wwww));
				o_SV_Target0 = asfloat(r3) * asfloat(r2.zzzz) + asfloat(r0);
			}
			#elif defined(BORDER) && defined(GRIDSAMPLE3D) && defined(LINEAR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yzw = asuint(max(float3(-1.0, -1.0, -1.0), asfloat(r1.xyz)));
				r0.yzw = asuint(min(float3(1.0, 1.0, 1.0), asfloat(r0.yzw)));
				r0.yzw = asuint(float3(1.0, 1.0, 1.0) + asfloat(r0.yzw));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.yzw));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r1.xyz) + float3(-0.5, -0.5, -0.5));
				r1.xyz = asuint(floor(asfloat(r0.yzw)));
				r0.yzw = asuint(-asfloat(r1.xyz) + asfloat(r0.yzw));
				r1.xyz = asuint(int3(asfloat(r1.xyz)));
				r1.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r1.w = asuint(asint(r1.w) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r0.x) * asint(r1.w));
				r2.xyz = asuint(int3(1, 1, 1) + asint(r1.xyz));
				r1.xyz = asuint(max(int3(0, 0, 0), asint(r1.xyz)));
				r3.xy = asuint(float2(asfloat(X_width), asfloat(X_height)));
				r3.z = asuint(asfloat(X_depth));
				r3.xyz = asuint(int3(-1, -1, -1) + asint(r3.xyz));
				r2.xyz = asuint(min(asint(r3.xyz), asint(r2.xyz)));
				r1.w = asuint(asint(r2.z) * asint(asfloat(X_width)));
				r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
				r2.z = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r1.w));
				r1.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r1.w));
				r2.w = asuint(asint(r2.z) + asint(r1.x));
				r2.z = asuint(asint(r2.z) + asint(r2.x));
				r3.x = asuint(asfloat(WidthMaskX)) & r2.w;
				r3.y = r2.w >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r4.xyz = asuint(float3(1.0, 1.0, 1.0) + -asfloat(r0.yzw));
				r3 = asuint(asfloat(r4.xxxx) * asfloat(r3));
				r3 = asuint(asfloat(r3) * asfloat(r0.zzzz));
				r2.w = asuint(asint(r1.w) + asint(r1.x));
				r1.w = asuint(asint(r1.w) + asint(r2.x));
				r5.x = asuint(asfloat(WidthMaskX)) & r2.w;
				r5.y = r2.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r4.xxxx));
				r3 = asuint(asfloat(r5) * asfloat(r4.yyyy) + asfloat(r3));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.w;
				r5.y = r1.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r5) * asfloat(r4.yyyy) + asfloat(r3));
				r5.x = asuint(asfloat(WidthMaskX)) & r2.z;
				r5.y = r2.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r5) * asfloat(r0.zzzz) + asfloat(r3));
				r3 = asuint(asfloat(r3) * asfloat(r0.wwww));
				r0.w = asuint(asint(r1.z) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.w) * asint(asfloat(X_height)) + asint(r0.x));
				r0.w = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
				r1.y = asuint(asint(r0.w) + asint(r1.x));
				r0.w = asuint(asint(r0.w) + asint(r2.x));
				r1.z = asuint(asint(r0.x) + asint(r2.x));
				r0.x = asuint(asint(r0.x) + asint(r1.x));
				r2.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r2.y = r1.y >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r4.xxxx));
				r2 = asuint(asfloat(r2) * asfloat(r0.zzzz));
				r5.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r5.y = r0.x >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r4.xxxx));
				r2 = asuint(asfloat(r5) * asfloat(r4.yyyy) + asfloat(r2));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.z;
				r5.y = r1.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				r1 = asuint(asfloat(r1) * asfloat(r4.yyyy) + asfloat(r2));
				r2.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r2.y = r0.w >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r0.yyyy));
				r0 = asuint(asfloat(r2) * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_Target0 = asfloat(r0) * asfloat(r4.zzzz) + asfloat(r3);
			}
			#elif defined(GRIDSAMPLE3D) && defined(LINEAR) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yzw = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r1.xyz = asuint(float3(4.0, 4.0, 4.0) * asfloat(r0.yzw));
				b1.xyz = (asfloat(r1.xyz) >= -asfloat(r1.xyz));
				r2.xyz = b1.xyz ? uint3(0x3e800000u, 0x3e800000u, 0x3e800000u) : uint3(0xbe800000u, 0xbe800000u, 0xbe800000u);
				r1.xyz = b1.xyz ? uint3(0x40800000u, 0x40800000u, 0x40800000u) : uint3(0xc0800000u, 0xc0800000u, 0xc0800000u);
				r0.yzw = asuint(asfloat(r2.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(frac(asfloat(r0.yzw)));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw) + float3(4.0, 4.0, 4.0));
				r1.xyz = asuint(float3(4.0, 4.0, 4.0) * asfloat(r0.yzw));
				b1.xyz = (asfloat(r1.xyz) >= -asfloat(r1.xyz));
				r2.xyz = b1.xyz ? uint3(0x3e800000u, 0x3e800000u, 0x3e800000u) : uint3(0xbe800000u, 0xbe800000u, 0xbe800000u);
				r1.xyz = b1.xyz ? uint3(0x40800000u, 0x40800000u, 0x40800000u) : uint3(0xc0800000u, 0xc0800000u, 0xc0800000u);
				r0.yzw = asuint(asfloat(r2.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(frac(asfloat(r0.yzw)));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw) + float3(-2.0, -2.0, -2.0));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * abs(asfloat(r0.yzw)));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r1.xyz) + float3(-0.5, -0.5, -0.5));
				r1.xyz = asuint(floor(asfloat(r0.yzw)));
				r0.yzw = asuint(-asfloat(r1.xyz) + asfloat(r0.yzw));
				r1.xyz = asuint(int3(asfloat(r1.xyz)));
				r2.xyz = asuint(int3(1, 1, 1) + asint(r1.xyz));
				r1.xyz = asuint(max(int3(0, 0, 0), asint(r1.xyz)));
				r3.xy = asuint(float2(asfloat(X_width), asfloat(X_height)));
				r3.z = asuint(asfloat(X_depth));
				r3.xyz = asuint(int3(-1, -1, -1) + asint(r3.xyz));
				r2.xyz = asuint(min(asint(r3.xyz), asint(r2.xyz)));
				r1.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r1.w = asuint(asint(r1.w) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r0.x) * asint(r1.w));
				r1.w = asuint(asint(r2.z) * asint(asfloat(X_width)));
				r1.w = asuint(asint(r1.w) * asint(asfloat(X_height)) + asint(r0.x));
				r2.z = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r1.w));
				r1.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r1.w));
				r2.w = asuint(asint(r2.z) + asint(r1.x));
				r2.z = asuint(asint(r2.z) + asint(r2.x));
				r3.x = asuint(asfloat(WidthMaskX)) & r2.w;
				r3.y = r2.w >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r4.xyz = asuint(float3(1.0, 1.0, 1.0) + -asfloat(r0.yzw));
				r3 = asuint(asfloat(r4.xxxx) * asfloat(r3));
				r3 = asuint(asfloat(r3) * asfloat(r0.zzzz));
				r2.w = asuint(asint(r1.w) + asint(r1.x));
				r1.w = asuint(asint(r1.w) + asint(r2.x));
				r5.x = asuint(asfloat(WidthMaskX)) & r2.w;
				r5.y = r2.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r4.xxxx));
				r3 = asuint(asfloat(r5) * asfloat(r4.yyyy) + asfloat(r3));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.w;
				r5.y = r1.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r5) * asfloat(r4.yyyy) + asfloat(r3));
				r5.x = asuint(asfloat(WidthMaskX)) & r2.z;
				r5.y = r2.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r5) * asfloat(r0.zzzz) + asfloat(r3));
				r3 = asuint(asfloat(r3) * asfloat(r0.wwww));
				r0.w = asuint(asint(r1.z) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.w) * asint(asfloat(X_height)) + asint(r0.x));
				r0.w = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r2.y) * asint(asfloat(X_width)) + asint(r0.x));
				r1.y = asuint(asint(r0.w) + asint(r1.x));
				r0.w = asuint(asint(r0.w) + asint(r2.x));
				r1.z = asuint(asint(r0.x) + asint(r2.x));
				r0.x = asuint(asint(r0.x) + asint(r1.x));
				r2.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r2.y = r1.y >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r4.xxxx));
				r5.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r5.y = r0.x >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r4.xxxx));
				r5 = asuint(asfloat(r5) * asfloat(r0.zzzz));
				r2 = asuint(asfloat(r2) * asfloat(r4.yyyy) + asfloat(r5));
				r5.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r5.y = r0.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r2 = asuint(asfloat(r5) * asfloat(r4.yyyy) + asfloat(r2));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.z;
				r5.y = r1.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				r0 = asuint(asfloat(r1) * asfloat(r0.zzzz) + asfloat(r2));
				o_SV_Target0 = asfloat(r0) * asfloat(r4.zzzz) + asfloat(r3);
			}
			#elif defined(GRIDSAMPLE3D) && defined(NEAREST) && defined(ZEROS)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				bool4 b0;
				bool4 b2;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.y = r0.x / asuint(asfloat(O_channelsDiv4));
				r0.z = asuint(asint(r0.y) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.z = asuint(asint(r0.z) * asint(asfloat(O_height)) + asint(r2.x));
				r0.z = asuint(asint(r0.z) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.z;
				r1.y = r0.z >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r1.xyz = asuint(float3(1.0, 1.0, 1.0) + asfloat(r1.xyz));
				r1.xyz = asuint(float3(0.5, 0.5, 0.5) * asfloat(r1.xyz));
				r2.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r2.z = asuint(float(asint(asfloat(X_depth))));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r2.xyz) + float3(-0.5, -0.5, -0.5));
				r1.xyz = asuint(round(asfloat(r1.xyz)));
				r1.xyz = asuint(int3(asfloat(r1.xyz)));
				b2.xyz = (asint(r1.xyz) < int3(0, 0, 0));
				b0.zw = (r1.xy >= asuint(float2(asfloat(X_width), asfloat(X_height))));
				b0.z = b2.x || b0.z;
				b0.z = b0.z || b2.y;
				b0.z = b0.z || b0.w;
				b0.z = b0.z || b2.z;
				b0.w = (r1.z >= asuint(asfloat(X_depth)));
				b0.z = b0.z || b0.w;
				[branch] if (!b0.z)
				{
					r0.x = r0.x % asuint(asfloat(O_channelsDiv4));
					r0.z = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
					r0.z = asuint(asint(r0.z) * asint(asfloat(X_depth)));
					r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.y) + asint(r0.x));
					r0.y = asuint(asint(r1.z) * asint(asfloat(X_width)));
					r0.y = asuint(asint(r0.y) * asint(asfloat(X_height)));
					r0.x = asuint(asint(r0.z) * asint(r0.x) + asint(r0.y));
					r0.x = asuint(asint(r1.y) * asint(asfloat(X_width)) + asint(r0.x));
					r0.x = asuint(asint(r0.x) + asint(r1.x));
					r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
					r1.y = r0.x >> asuint(asfloat(WidthShiftX));
					r1.zw = uint2(0u, 0u);
					o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
				}
				else
				{
					o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
				}
			}
			#elif defined(BORDER) && defined(GRIDSAMPLE3D) && defined(NEAREST)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yzw = asuint(max(float3(-1.0, -1.0, -1.0), asfloat(r1.xyz)));
				r0.yzw = asuint(min(float3(1.0, 1.0, 1.0), asfloat(r0.yzw)));
				r0.yzw = asuint(float3(1.0, 1.0, 1.0) + asfloat(r0.yzw));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * asfloat(r0.yzw));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r1.xyz) + float3(-0.5, -0.5, -0.5));
				r0.yzw = asuint(round(asfloat(r0.yzw)));
				r0.yzw = asuint(int3(asfloat(r0.yzw)));
				r0.yzw = asuint(max(int3(0, 0, 0), asint(r0.yzw)));
				r1.xy = asuint(float2(asfloat(X_width), asfloat(X_height)));
				r1.z = asuint(asfloat(X_depth));
				r1.xyz = asuint(int3(-1, -1, -1) + asint(r1.xyz));
				r0.yzw = asuint(min(asint(r1.xyz), asint(r0.yzw)));
				r0.w = asuint(asint(r0.w) * asint(asfloat(X_width)));
				r0.w = asuint(asint(r0.w) * asint(asfloat(X_height)));
				r1.x = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r1.x = asuint(asint(r1.x) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r1.x) * asint(r0.x) + asint(r0.w));
				r0.x = asuint(asint(r0.z) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(GRIDSAMPLE3D) && defined(NEAREST) && defined(REFLECTION)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int WidthShiftS;
			int WidthMaskS;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			Texture2D<float4> Xptr : register(t0);
			Texture2D<float4> Sptr : register(t1);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r3.x = r0.x % asuint(asfloat(O_depth));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(asint(r0.x) * asint(asfloat(O_depth)) + asint(r3.x));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_height)) + asint(r2.x));
				r0.y = asuint(asint(r0.y) * asint(asfloat(O_width)) + asint(r1.x));
				r1.x = asuint(asfloat(WidthMaskS)) & r0.y;
				r1.y = r0.y >> asuint(asfloat(WidthShiftS));
				r1.zw = uint2(0u, 0u);
				r1 = asuint(Sptr.Load(int3(asint(r1.xy), asint(r1.w))));
				r0.yzw = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r1.xyz = asuint(float3(4.0, 4.0, 4.0) * asfloat(r0.yzw));
				b1.xyz = (asfloat(r1.xyz) >= -asfloat(r1.xyz));
				r2.xyz = b1.xyz ? uint3(0x3e800000u, 0x3e800000u, 0x3e800000u) : uint3(0xbe800000u, 0xbe800000u, 0xbe800000u);
				r1.xyz = b1.xyz ? uint3(0x40800000u, 0x40800000u, 0x40800000u) : uint3(0xc0800000u, 0xc0800000u, 0xc0800000u);
				r0.yzw = asuint(asfloat(r2.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(frac(asfloat(r0.yzw)));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw) + float3(4.0, 4.0, 4.0));
				r1.xyz = asuint(float3(4.0, 4.0, 4.0) * asfloat(r0.yzw));
				b1.xyz = (asfloat(r1.xyz) >= -asfloat(r1.xyz));
				r2.xyz = b1.xyz ? uint3(0x3e800000u, 0x3e800000u, 0x3e800000u) : uint3(0xbe800000u, 0xbe800000u, 0xbe800000u);
				r1.xyz = b1.xyz ? uint3(0x40800000u, 0x40800000u, 0x40800000u) : uint3(0xc0800000u, 0xc0800000u, 0xc0800000u);
				r0.yzw = asuint(asfloat(r2.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(frac(asfloat(r0.yzw)));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw) + float3(-2.0, -2.0, -2.0));
				r0.yzw = asuint(float3(0.5, 0.5, 0.5) * abs(asfloat(r0.yzw)));
				r1.xy = asuint(float2(asint(float2(asfloat(X_width), asfloat(X_height)))));
				r1.z = asuint(float(asint(asfloat(X_depth))));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r1.xyz) + float3(-0.5, -0.5, -0.5));
				r0.yzw = asuint(round(asfloat(r0.yzw)));
				r0.yzw = asuint(int3(asfloat(r0.yzw)));
				r0.yzw = asuint(max(int3(0, 0, 0), asint(r0.yzw)));
				r1.xy = asuint(float2(asfloat(X_width), asfloat(X_height)));
				r1.z = asuint(asfloat(X_depth));
				r1.xyz = asuint(int3(-1, -1, -1) + asint(r1.xyz));
				r0.yzw = asuint(min(asint(r1.xyz), asint(r0.yzw)));
				r0.w = asuint(asint(r0.w) * asint(asfloat(X_width)));
				r0.w = asuint(asint(r0.w) * asint(asfloat(X_height)));
				r1.x = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r1.x = asuint(asint(r1.x) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r1.x) * asint(r0.x) + asint(r0.w));
				r0.x = asuint(asint(r0.z) * asint(asfloat(X_width)) + asint(r0.x));
				r0.x = asuint(asint(r0.x) + asint(r0.y));
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
