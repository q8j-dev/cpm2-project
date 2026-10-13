Shader "Hidden/Sentis/Upsample"
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
			#pragma multi_compile __ UPSAMPLE1D
			#pragma multi_compile __ UPSAMPLE2D
			#pragma multi_compile __ UPSAMPLE3D

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
			#if defined(LINEAR) && defined(UPSAMPLE1D)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			float4 Scale;
			float4 Bias;
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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r2.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.y = asuint(float(r1.x));
				r0.y = asuint(asfloat(r0.y) * Scale.x + Bias.x);
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r2.x));
				r0.z = asuint(floor(asfloat(r0.y)));
				r0.y = asuint(asfloat(r0.y) + -asfloat(r0.z));
				r0.z = asuint(int(asfloat(r0.z)));
				r0.w = asuint(1 + asint(r0.z));
				r0.zw = asuint(max(int2(0, 0), asint(r0.zw)));
				r1.x = asuint(-1 + asint(asfloat(X_width)));
				r0.zw = asuint(min(asint(r1.xx), asint(r0.zw)));
				r0.z = asuint(asint(asfloat(X_width)) * asint(r0.x) + asint(r0.z));
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
				o_SV_Target0 = asfloat(r2) * asfloat(r0.xxxx) + asfloat(r1);
			}
			#elif defined(NEAREST_FLOOR) && defined(UPSAMPLE1D)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			float4 Scale;
			float4 Bias;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.y = asuint(float(r1.x));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r1.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r1.x));
				r0.y = asuint(asfloat(r0.y) * Scale.x + Bias.x);
				r0.y = asuint(floor(asfloat(r0.y)));
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
			#elif defined(NEAREST_CEIL) && defined(UPSAMPLE1D)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int O_width;
			float _GlobalsPad2;
			float _GlobalsPad3;
			int O_channelsDiv4;
			int X_width;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int X_channelsDiv4;
			float4 Scale;
			float4 Bias;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.y = asuint(float(r1.x));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r1.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r1.x));
				r0.y = asuint(asfloat(r0.y) * Scale.x + Bias.x);
				r0.y = asuint(ceil(asfloat(r0.y)));
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
			#elif defined(LINEAR) && defined(UPSAMPLE2D)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			float4 Scale;
			float4 Bias;
			Texture2D<float4> Xptr : register(t0);

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
				r0.y = asuint(float(r1.x));
				r0.y = asuint(asfloat(r0.y) * Scale.y + Bias.y);
				r0.z = asuint(float(r2.x));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r1.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r1.x));
				r0.z = asuint(asfloat(r0.z) * Scale.x + Bias.x);
				r0.w = asuint(floor(asfloat(r0.z)));
				r0.z = asuint(asfloat(r0.z) + -asfloat(r0.w));
				r0.w = asuint(int(asfloat(r0.w)));
				r1.x = asuint(floor(asfloat(r0.y)));
				r0.y = asuint(-asfloat(r1.x) + asfloat(r0.y));
				r1.x = asuint(int(asfloat(r1.x)));
				r1.y = asuint(1.0 + -asfloat(r0.y));
				r1.z = asuint(1 + asint(r0.w));
				r0.w = asuint(max(0, asint(r0.w)));
				r1.zw = asuint(max(int2(0, 0), asint(r1.zx)));
				r2.xy = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r1.x = asuint(1 + asint(r1.x));
				r1.x = asuint(max(0, asint(r1.x)));
				r1.xzw = asuint(min(asint(r2.xyx), asint(r1.xzw)));
				r0.w = asuint(min(asint(r2.y), asint(r0.w)));
				r2.x = asuint(asint(asfloat(X_width)) * asint(r1.z) + asint(r1.w));
				r1.z = asuint(asint(asfloat(X_width)) * asint(r1.z) + asint(r1.x));
				r1.x = asuint(asint(asfloat(X_width)) * asint(r0.w) + asint(r1.x));
				r0.w = asuint(asint(asfloat(X_width)) * asint(r0.w) + asint(r1.w));
				r1.w = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r2.x = asuint(asint(r1.w) * asint(r0.x) + asint(r2.x));
				r3.x = asuint(asfloat(WidthMaskX)) & r2.x;
				r3.y = r2.x >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r2 = asuint(asfloat(r2) * asfloat(r1.yyyy));
				r2 = asuint(asfloat(r2) * asfloat(r0.zzzz));
				r0.w = asuint(asint(r1.w) * asint(r0.x) + asint(r0.w));
				r3.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r3.y = r0.w >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r3 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r3 = asuint(asfloat(r3) * asfloat(r1.yyyy));
				r0.w = asuint(1.0 + -asfloat(r0.z));
				r2 = asuint(asfloat(r3) * asfloat(r0.wwww) + asfloat(r2));
				r1.x = asuint(asint(r1.w) * asint(r0.x) + asint(r1.x));
				r0.x = asuint(asint(r1.w) * asint(r0.x) + asint(r1.z));
				r3.x = asuint(asfloat(WidthMaskX)) & r1.x;
				r3.y = r1.x >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				r1 = asuint(asfloat(r1) * asfloat(r0.yyyy));
				r1 = asuint(asfloat(r1) * asfloat(r0.wwww) + asfloat(r2));
				r2.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r2.y = r0.x >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r2) * asfloat(r0.zzzz) + asfloat(r1);
			}
			#elif defined(NEAREST_FLOOR) && defined(UPSAMPLE2D)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			float4 Scale;
			float4 Bias;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.y = asuint(float(r1.x));
				r0.x = r0.x / asuint(asfloat(O_height));
				r1.x = r0.x % asuint(asfloat(O_height));
				r0.y = asuint(asfloat(r0.y) * Scale.y + Bias.y);
				r0.y = asuint(floor(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.y = asuint(max(0, asint(r0.y)));
				r0.zw = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r0.y = asuint(min(asint(r0.y), asint(r0.z)));
				r0.z = asuint(float(r1.x));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r1.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r1.x));
				r0.z = asuint(asfloat(r0.z) * Scale.x + Bias.x);
				r0.z = asuint(floor(asfloat(r0.z)));
				r0.z = asuint(int(asfloat(r0.z)));
				r0.z = asuint(max(0, asint(r0.z)));
				r0.z = asuint(min(asint(r0.z), asint(r0.w)));
				r0.y = asuint(asint(asfloat(X_width)) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.z) * asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(NEAREST_CEIL) && defined(UPSAMPLE2D)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int O_width;
			int O_height;
			float _GlobalsPad2;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			float _GlobalsPad3;
			int X_channelsDiv4;
			float4 Scale;
			float4 Bias;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.y = asuint(float(r1.x));
				r0.x = r0.x / asuint(asfloat(O_height));
				r1.x = r0.x % asuint(asfloat(O_height));
				r0.y = asuint(asfloat(r0.y) * Scale.y + Bias.y);
				r0.y = asuint(ceil(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.y = asuint(max(0, asint(r0.y)));
				r0.zw = asuint(int2(-1, -1) + asint(float2(asfloat(X_width), asfloat(X_height))));
				r0.y = asuint(min(asint(r0.y), asint(r0.z)));
				r0.z = asuint(float(r1.x));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r1.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r1.x));
				r0.z = asuint(asfloat(r0.z) * Scale.x + Bias.x);
				r0.z = asuint(ceil(asfloat(r0.z)));
				r0.z = asuint(int(asfloat(r0.z)));
				r0.z = asuint(max(0, asint(r0.z)));
				r0.z = asuint(min(asint(r0.z), asint(r0.w)));
				r0.y = asuint(asint(asfloat(X_width)) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.x = asuint(asint(r0.z) * asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(LINEAR) && defined(UPSAMPLE3D)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			float4 Scale;
			float4 Bias;
			Texture2D<float4> Xptr : register(t0);

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
				r0.y = asuint(float(r1.x));
				r0.y = asuint(asfloat(r0.y) * Scale.z + Bias.z);
				r0.z = asuint(float(r2.x));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r1.x = r0.x % asuint(asfloat(O_depth));
				r0.z = asuint(asfloat(r0.z) * Scale.y + Bias.y);
				r0.w = asuint(floor(asfloat(r0.z)));
				r0.z = asuint(asfloat(r0.z) + -asfloat(r0.w));
				r0.w = asuint(int(asfloat(r0.w)));
				r1.y = asuint(floor(asfloat(r0.y)));
				r0.y = asuint(-asfloat(r1.y) + asfloat(r0.y));
				r1.y = asuint(int(asfloat(r1.y)));
				r1.z = asuint(1.0 + -asfloat(r0.y));
				r2.zw = uint2(0u, 0u);
				r1.w = asuint(1 + asint(r0.w));
				r0.w = asuint(max(0, asint(r0.w)));
				r1.w = asuint(max(0, asint(r1.w)));
				r3.xyz = asuint(int3(-1, -1, -1) + asint(float3(asfloat(X_width), asfloat(X_height), asfloat(X_depth))));
				r1.w = asuint(min(asint(r3.y), asint(r1.w)));
				r1.x = asuint(float(r1.x));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r4.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r4.x));
				r1.x = asuint(asfloat(r1.x) * Scale.x + Bias.x);
				r3.w = asuint(floor(asfloat(r1.x)));
				r1.x = asuint(-asfloat(r3.w) + asfloat(r1.x));
				r3.w = asuint(int(asfloat(r3.w)));
				r4.x = asuint(1 + asint(r3.w));
				r3.w = asuint(max(0, asint(r3.w)));
				r3.w = asuint(min(asint(r3.w), asint(r3.z)));
				r4.x = asuint(max(0, asint(r4.x)));
				r3.z = asuint(min(asint(r4.x), asint(r3.z)));
				r4.x = asuint(asint(asfloat(X_height)) * asint(r3.z) + asint(r1.w));
				r1.w = asuint(asint(asfloat(X_height)) * asint(r3.w) + asint(r1.w));
				r4.y = asuint(max(0, asint(r1.y)));
				r1.y = asuint(1 + asint(r1.y));
				r1.y = asuint(max(0, asint(r1.y)));
				r1.y = asuint(min(asint(r1.y), asint(r3.x)));
				r3.x = asuint(min(asint(r4.y), asint(r3.x)));
				r0.w = asuint(min(asint(r3.y), asint(r0.w)));
				r3.y = asuint(asint(asfloat(X_width)) * asint(r4.x) + asint(r3.x));
				r4.x = asuint(asint(asfloat(X_width)) * asint(r4.x) + asint(r1.y));
				r4.y = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r4.y = asuint(asint(r4.y) * asint(asfloat(X_depth)));
				r3.y = asuint(asint(r4.y) * asint(r0.x) + asint(r3.y));
				r2.x = asuint(asfloat(WidthMaskX)) & r3.y;
				r2.y = r3.y >> asuint(asfloat(WidthShiftX));
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				r2 = asuint(asfloat(r2) * asfloat(r1.zzzz));
				r2 = asuint(asfloat(r2) * asfloat(r0.zzzz));
				r3.y = asuint(asint(asfloat(X_height)) * asint(r3.z) + asint(r0.w));
				r0.w = asuint(asint(asfloat(X_height)) * asint(r3.w) + asint(r0.w));
				r3.z = asuint(asint(asfloat(X_width)) * asint(r3.y) + asint(r3.x));
				r3.y = asuint(asint(asfloat(X_width)) * asint(r3.y) + asint(r1.y));
				r3.y = asuint(asint(r4.y) * asint(r0.x) + asint(r3.y));
				r3.z = asuint(asint(r4.y) * asint(r0.x) + asint(r3.z));
				r5.x = asuint(asfloat(WidthMaskX)) & r3.z;
				r5.y = r3.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r1.zzzz));
				r3.z = asuint(1.0 + -asfloat(r0.z));
				r2 = asuint(asfloat(r5) * asfloat(r3.zzzz) + asfloat(r2));
				r5.x = asuint(asfloat(WidthMaskX)) & r3.y;
				r5.y = r3.y >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r2 = asuint(asfloat(r5) * asfloat(r3.zzzz) + asfloat(r2));
				r3.y = asuint(asint(r4.y) * asint(r0.x) + asint(r4.x));
				r5.x = asuint(asfloat(WidthMaskX)) & r3.y;
				r5.y = r3.y >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r2 = asuint(asfloat(r5) * asfloat(r0.zzzz) + asfloat(r2));
				r2 = asuint(asfloat(r2) * asfloat(r1.xxxx));
				r1.x = asuint(1.0 + -asfloat(r1.x));
				r3.y = asuint(asint(asfloat(X_width)) * asint(r1.w) + asint(r3.x));
				r1.w = asuint(asint(asfloat(X_width)) * asint(r1.w) + asint(r1.y));
				r1.y = asuint(asint(asfloat(X_width)) * asint(r0.w) + asint(r1.y));
				r0.w = asuint(asint(asfloat(X_width)) * asint(r0.w) + asint(r3.x));
				r0.w = asuint(asint(r4.y) * asint(r0.x) + asint(r0.w));
				r1.y = asuint(asint(r4.y) * asint(r0.x) + asint(r1.y));
				r1.w = asuint(asint(r4.y) * asint(r0.x) + asint(r1.w));
				r0.x = asuint(asint(r4.y) * asint(r0.x) + asint(r3.y));
				r4.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r4.y = r0.x >> asuint(asfloat(WidthShiftX));
				r4.zw = uint2(0u, 0u);
				r4 = asuint(Xptr.Load(int3(asint(r4.xy), asint(r4.w))));
				r4 = asuint(asfloat(r4) * asfloat(r1.zzzz));
				r4 = asuint(asfloat(r4) * asfloat(r0.zzzz));
				r5.x = asuint(asfloat(WidthMaskX)) & r0.w;
				r5.y = r0.w >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r1.zzzz));
				r4 = asuint(asfloat(r5) * asfloat(r3.zzzz) + asfloat(r4));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r5.y = r1.y >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r5 = asuint(asfloat(r5) * asfloat(r0.yyyy));
				r3 = asuint(asfloat(r5) * asfloat(r3.zzzz) + asfloat(r4));
				r4.x = asuint(asfloat(WidthMaskX)) & r1.w;
				r4.y = r1.w >> asuint(asfloat(WidthShiftX));
				r4.zw = uint2(0u, 0u);
				r4 = asuint(Xptr.Load(int3(asint(r4.xy), asint(r4.w))));
				r4 = asuint(asfloat(r4) * asfloat(r0.yyyy));
				r0 = asuint(asfloat(r4) * asfloat(r0.zzzz) + asfloat(r3));
				o_SV_Target0 = asfloat(r0) * asfloat(r1.xxxx) + asfloat(r2);
			}
			#elif defined(NEAREST_FLOOR) && defined(UPSAMPLE3D)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			float4 Scale;
			float4 Bias;
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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.y = asuint(float(r1.x));
				r0.y = asuint(asfloat(r0.y) * Scale.z + Bias.z);
				r0.y = asuint(floor(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.z = asuint(float(r2.x));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r1.x = r0.x % asuint(asfloat(O_depth));
				r0.z = asuint(asfloat(r0.z) * Scale.y + Bias.y);
				r0.z = asuint(floor(asfloat(r0.z)));
				r0.z = asuint(int(asfloat(r0.z)));
				r0.yz = asuint(max(int2(0, 0), asint(r0.yz)));
				r1.yzw = asuint(int3(-1, -1, -1) + asint(float3(asfloat(X_width), asfloat(X_height), asfloat(X_depth))));
				r0.w = asuint(float(r1.x));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r1.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r1.x));
				r0.w = asuint(asfloat(r0.w) * Scale.x + Bias.x);
				r0.w = asuint(floor(asfloat(r0.w)));
				r0.w = asuint(int(asfloat(r0.w)));
				r0.w = asuint(max(0, asint(r0.w)));
				r0.yzw = asuint(min(asint(r1.yzw), asint(r0.yzw)));
				r0.z = asuint(asint(asfloat(X_height)) * asint(r0.w) + asint(r0.z));
				r0.y = asuint(asint(asfloat(X_width)) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.z = asuint(asint(r0.z) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r0.z) * asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				o_SV_Target0 = Xptr.Load(int3(asint(r1.xy), asint(r1.w)));
			}
			#elif defined(NEAREST_CEIL) && defined(UPSAMPLE3D)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			float _GlobalsPad1;
			int WidthShiftX;
			int WidthMaskX;
			int O_width;
			int O_height;
			int O_depth;
			int O_channelsDiv4;
			int X_width;
			int X_height;
			int X_depth;
			int X_channelsDiv4;
			float4 Scale;
			float4 Bias;
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
				r0.x = r0.x / asuint(asfloat(O_width));
				r1.x = r0.x % asuint(asfloat(O_width));
				r0.x = r0.x / asuint(asfloat(O_height));
				r2.x = r0.x % asuint(asfloat(O_height));
				r0.y = asuint(float(r1.x));
				r0.y = asuint(asfloat(r0.y) * Scale.z + Bias.z);
				r0.y = asuint(ceil(asfloat(r0.y)));
				r0.y = asuint(int(asfloat(r0.y)));
				r0.z = asuint(float(r2.x));
				r0.x = r0.x / asuint(asfloat(O_depth));
				r1.x = r0.x % asuint(asfloat(O_depth));
				r0.z = asuint(asfloat(r0.z) * Scale.y + Bias.y);
				r0.z = asuint(ceil(asfloat(r0.z)));
				r0.z = asuint(int(asfloat(r0.z)));
				r0.yz = asuint(max(int2(0, 0), asint(r0.yz)));
				r1.yzw = asuint(int3(-1, -1, -1) + asint(float3(asfloat(X_width), asfloat(X_height), asfloat(X_depth))));
				r0.w = asuint(float(r1.x));
				r0.x = r0.x / asuint(asfloat(O_channelsDiv4));
				r1.x = r0.x % asuint(asfloat(O_channelsDiv4));
				r0.x = asuint(asint(asfloat(X_channelsDiv4)) * asint(r0.x) + asint(r1.x));
				r0.w = asuint(asfloat(r0.w) * Scale.x + Bias.x);
				r0.w = asuint(ceil(asfloat(r0.w)));
				r0.w = asuint(int(asfloat(r0.w)));
				r0.w = asuint(max(0, asint(r0.w)));
				r0.yzw = asuint(min(asint(r1.yzw), asint(r0.yzw)));
				r0.z = asuint(asint(asfloat(X_height)) * asint(r0.w) + asint(r0.z));
				r0.y = asuint(asint(asfloat(X_width)) * asint(r0.z) + asint(r0.y));
				r0.z = asuint(asint(asfloat(X_height)) * asint(asfloat(X_width)));
				r0.z = asuint(asint(r0.z) * asint(asfloat(X_depth)));
				r0.x = asuint(asint(r0.z) * asint(r0.x) + asint(r0.y));
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
