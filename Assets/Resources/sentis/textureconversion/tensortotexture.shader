Shader "Hidden/Sentis/TextureConversion/TensorToTexture"
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
			#pragma multi_compile __ BGRA
			#pragma multi_compile __ EXACT
			#pragma multi_compile __ LINEAR
			#pragma multi_compile __ RGBA

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
			#if defined(EXACT) && defined(RGBA)
			float4 _GlobalsPad0[2];
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			float _GlobalsPad3;
			int HeightO;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int Stride1X;
			int Stride0X;
			int Channels;
			int CoordOrigin;
			float4 _GlobalsPad6[1];
			float4 ChannelScale;
			float4 ChannelBias;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0.x = asuint(-1 + asint(asfloat(HeightO)));
				r0.yz = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.yz = uint2(asfloat(r0.yz));
				r0.x = asuint(asint(r0.x) + -asint(r0.z));
				r0.x = (asuint(asfloat(CoordOrigin)) != 0) ? r0.z : r0.x;
				r0.x = asuint(asint(r0.x) * asint(asfloat(Stride1X)));
				r0.x = asuint(asint(asfloat(Stride0X)) * asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				b1 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(1u, 2u, 3u, 4u));
				r0 = b1 ? r0 : uint4(0u, 0u, 0u, 0u);
				o_SV_Target0 = ChannelScale.xyzw * asfloat(r0) + ChannelBias.xyzw;
			}
			#elif defined(BGRA) && defined(EXACT)
			float4 _GlobalsPad0[2];
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			float _GlobalsPad3;
			int HeightO;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int Stride1X;
			int Stride0X;
			int Channels;
			int CoordOrigin;
			float4 _GlobalsPad6[1];
			float4 ChannelScale;
			float4 ChannelBias;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0.x = asuint(-1 + asint(asfloat(HeightO)));
				r0.yz = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.yz = uint2(asfloat(r0.yz));
				r0.x = asuint(asint(r0.x) + -asint(r0.z));
				r0.x = (asuint(asfloat(CoordOrigin)) != 0) ? r0.z : r0.x;
				r0.x = asuint(asint(r0.x) * asint(asfloat(Stride1X)));
				r0.x = asuint(asint(asfloat(Stride0X)) * asint(r0.y) + asint(r0.x));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				b1 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(3u, 2u, 1u, 4u));
				r0 = b1 ? r0.zyxw : uint4(0u, 0u, 0u, 0u);
				o_SV_Target0 = ChannelScale.xyzw * asfloat(r0) + ChannelBias.xyzw;
			}
			#elif defined(LINEAR) && defined(RGBA)
			float4 _GlobalsPad0[2];
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			int WidthO;
			int HeightO;
			int WidthX;
			int HeightX;
			int Stride1X;
			int Stride0X;
			int Channels;
			int CoordOrigin;
			float4 _GlobalsPad3[1];
			float4 ChannelScale;
			float4 ChannelBias;
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
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r1.xyz = asuint(int3(-1, -1, -1) + asint(float3(asfloat(HeightO), asfloat(WidthX), asfloat(HeightX))));
				r0.z = asuint(asint(r1.x) + -asint(r0.y));
				r0.y = (asuint(asfloat(CoordOrigin)) != 0) ? r0.y : r0.z;
				r2.xy = asuint(float2(r0.yx));
				r0.xy = asuint(float2(0.5, 0.5) + asfloat(r2.xy));
				r2 = asuint(float4(asuint(float4(asfloat(WidthO), asfloat(HeightO), asfloat(WidthX), asfloat(HeightX)))));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r2.yx));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r2.wz) + float2(-0.5, -0.5));
				r0.zw = asuint(floor(asfloat(r0.xy)));
				r0.zw = uint2(asfloat(r0.zw));
				r1.xw = asuint(int2(1, 1) + asint(r0.zw));
				r1.xw = min(r1.xw, r1.zy);
				r1.yz = min(r0.wz, r1.yz);
				r0.zw = asuint(float2(r0.zw));
				r0.xy = asuint(asfloat(r0.xy) + -asfloat(r0.zw));
				r0.zw = asuint(asint(r1.xw) * asint(float2(asfloat(Stride1X), asfloat(Stride0X))));
				r1.x = asuint(asint(r0.w) + asint(r0.z));
				r0.zw = asuint(asint(float2(asfloat(Stride0X), asfloat(Stride1X))) * asint(r1.yz) + asint(r0.zw));
				r1.yz = asuint(asint(r1.yz) * asint(float2(asfloat(Stride0X), asfloat(Stride1X))));
				r1.y = asuint(asint(r1.y) + asint(r1.z));
				r2.x = asuint(asfloat(WidthMaskX)) & r1.x;
				r2.y = r1.x >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				b3 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(1u, 2u, 3u, 4u));
				r2 = b3 ? r2 : uint4(0u, 0u, 0u, 0u);
				r2 = asuint(ChannelScale.xyzw * asfloat(r2) + ChannelBias.xyzw);
				r2 = asuint(asfloat(r2) * asfloat(r0.xxxx));
				r4.xy = r0.zw >> asuint(float2(asfloat(WidthShiftX), asfloat(WidthShiftX)));
				r5.xy = asuint(float2(asfloat(WidthMaskX), asfloat(WidthMaskX))) & r0.zw;
				r5.w = 0u;
				r4.zw = r5.yw;
				r6 = asuint(Xptr.Load(int3(asint(r4.zy), asint(r4.w))));
				r5.z = r4.x;
				r4 = asuint(Xptr.Load(int3(asint(r5.xz), asint(r5.w))));
				r4 = b3 ? r4 : uint4(0u, 0u, 0u, 0u);
				r4 = asuint(ChannelScale.xyzw * asfloat(r4) + ChannelBias.xyzw);
				r4 = asuint(asfloat(r4) * asfloat(r0.xxxx));
				r5 = b3 ? r6 : uint4(0u, 0u, 0u, 0u);
				r5 = asuint(ChannelScale.xyzw * asfloat(r5) + ChannelBias.xyzw);
				r0.xz = asuint(float2(1.0, 1.0) + -asfloat(r0.xy));
				r2 = asuint(asfloat(r0.xxxx) * asfloat(r5) + asfloat(r2));
				r2 = asuint(asfloat(r2) * asfloat(r0.yyyy));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r5.y = r1.y >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r1 = b3 ? r1 : uint4(0u, 0u, 0u, 0u);
				r1 = asuint(ChannelScale.xyzw * asfloat(r1) + ChannelBias.xyzw);
				r1 = asuint(asfloat(r0.xxxx) * asfloat(r1) + asfloat(r4));
				o_SV_Target0 = asfloat(r0.zzzz) * asfloat(r1) + asfloat(r2);
			}
			#elif defined(BGRA) && defined(LINEAR)
			float4 _GlobalsPad0[2];
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			int WidthO;
			int HeightO;
			int WidthX;
			int HeightX;
			int Stride1X;
			int Stride0X;
			int Channels;
			int CoordOrigin;
			float4 _GlobalsPad3[1];
			float4 ChannelScale;
			float4 ChannelBias;
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
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r1.xyz = asuint(int3(-1, -1, -1) + asint(float3(asfloat(HeightO), asfloat(WidthX), asfloat(HeightX))));
				r0.z = asuint(asint(r1.x) + -asint(r0.y));
				r0.y = (asuint(asfloat(CoordOrigin)) != 0) ? r0.y : r0.z;
				r2.xy = asuint(float2(r0.yx));
				r0.xy = asuint(float2(0.5, 0.5) + asfloat(r2.xy));
				r2 = asuint(float4(asuint(float4(asfloat(WidthO), asfloat(HeightO), asfloat(WidthX), asfloat(HeightX)))));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r2.yx));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r2.wz) + float2(-0.5, -0.5));
				r0.zw = asuint(floor(asfloat(r0.xy)));
				r0.zw = uint2(asfloat(r0.zw));
				r1.xw = asuint(int2(1, 1) + asint(r0.zw));
				r1.xw = min(r1.xw, r1.zy);
				r1.yz = min(r0.wz, r1.yz);
				r0.zw = asuint(float2(r0.zw));
				r0.xy = asuint(asfloat(r0.xy) + -asfloat(r0.zw));
				r0.zw = asuint(asint(r1.xw) * asint(float2(asfloat(Stride1X), asfloat(Stride0X))));
				r1.x = asuint(asint(r0.w) + asint(r0.z));
				r0.zw = asuint(asint(float2(asfloat(Stride0X), asfloat(Stride1X))) * asint(r1.yz) + asint(r0.zw));
				r1.yz = asuint(asint(r1.yz) * asint(float2(asfloat(Stride0X), asfloat(Stride1X))));
				r1.y = asuint(asint(r1.y) + asint(r1.z));
				r2.x = asuint(asfloat(WidthMaskX)) & r1.x;
				r2.y = r1.x >> asuint(asfloat(WidthShiftX));
				r2.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r2.xy), asint(r2.w))));
				b3 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(3u, 2u, 1u, 4u));
				r2 = b3 ? r2.zyxw : uint4(0u, 0u, 0u, 0u);
				r2 = asuint(ChannelScale.xyzw * asfloat(r2) + ChannelBias.xyzw);
				r2 = asuint(asfloat(r2) * asfloat(r0.xxxx));
				r4.xy = r0.zw >> asuint(float2(asfloat(WidthShiftX), asfloat(WidthShiftX)));
				r5.xy = asuint(float2(asfloat(WidthMaskX), asfloat(WidthMaskX))) & r0.zw;
				r5.w = 0u;
				r4.zw = r5.yw;
				r6 = asuint(Xptr.Load(int3(asint(r4.zy), asint(r4.w))));
				r5.z = r4.x;
				r4 = asuint(Xptr.Load(int3(asint(r5.xz), asint(r5.w))));
				r4 = b3 ? r4.zyxw : uint4(0u, 0u, 0u, 0u);
				r4 = asuint(ChannelScale.xyzw * asfloat(r4) + ChannelBias.xyzw);
				r4 = asuint(asfloat(r4) * asfloat(r0.xxxx));
				r5 = b3 ? r6.zyxw : uint4(0u, 0u, 0u, 0u);
				r5 = asuint(ChannelScale.xyzw * asfloat(r5) + ChannelBias.xyzw);
				r0.xz = asuint(float2(1.0, 1.0) + -asfloat(r0.xy));
				r2 = asuint(asfloat(r0.xxxx) * asfloat(r5) + asfloat(r2));
				r2 = asuint(asfloat(r2) * asfloat(r0.yyyy));
				r5.x = asuint(asfloat(WidthMaskX)) & r1.y;
				r5.y = r1.y >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r1 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				r1 = b3 ? r1.zyxw : uint4(0u, 0u, 0u, 0u);
				r1 = asuint(ChannelScale.xyzw * asfloat(r1) + ChannelBias.xyzw);
				r1 = asuint(asfloat(r0.xxxx) * asfloat(r1) + asfloat(r4));
				o_SV_Target0 = asfloat(r0.zzzz) * asfloat(r1) + asfloat(r2);
			}
			#elif defined(EXACT)
			float4 _GlobalsPad0[2];
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			float _GlobalsPad3;
			int HeightO;
			float _GlobalsPad4;
			float _GlobalsPad5;
			int Stride1X;
			int Stride0X;
			int Channels;
			int CoordOrigin;
			int ChannelSwizzleR;
			int ChannelSwizzleG;
			int ChannelSwizzleB;
			int ChannelSwizzleA;
			float4 ChannelScale;
			float4 ChannelBias;
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
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.z = asuint(-1 + asint(asfloat(HeightO)));
				r0.z = asuint(asint(r0.z) + -asint(r0.y));
				r0.y = (asuint(asfloat(CoordOrigin)) != 0) ? r0.y : r0.z;
				r0.y = asuint(asint(r0.y) * asint(asfloat(Stride1X)));
				r0.x = asuint(asint(asfloat(Stride0X)) * asint(r0.x) + asint(r0.y));
				r1.x = asuint(asfloat(WidthMaskX)) & r0.x;
				r1.y = r0.x >> asuint(asfloat(WidthShiftX));
				r1.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				b1 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= asuint(float4(asfloat(ChannelSwizzleR), asfloat(ChannelSwizzleG), asfloat(ChannelSwizzleB), asfloat(ChannelSwizzleA))));
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				[branch] if (b1.x)
				{
					r1.x = asuint(asfloat(ChannelSwizzleR));
					r2.x = asuint(dot(asfloat(r0), icb[asint(r1.x)]));
				}
				else
				{
					r2.x = 0u;
				}
				[branch] if (r1.y != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleG));
					r2.y = asuint(dot(asfloat(r0), icb[asint(r1.x)]));
				}
				else
				{
					r2.y = 0u;
				}
				[branch] if (r1.z != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleB));
					r2.z = asuint(dot(asfloat(r0), icb[asint(r1.x)]));
				}
				else
				{
					r2.z = 0u;
				}
				[branch] if (r1.w != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleA));
					r2.w = asuint(dot(asfloat(r0), icb[asint(r1.x)]));
				}
				else
				{
					r2.w = 0u;
				}
				o_SV_Target0 = ChannelScale.xyzw * asfloat(r2) + ChannelBias.xyzw;
			}
			#elif defined(LINEAR)
			float4 _GlobalsPad0[2];
			float _GlobalsPad1;
			float _GlobalsPad2;
			int WidthShiftX;
			int WidthMaskX;
			int WidthO;
			int HeightO;
			int WidthX;
			int HeightX;
			int Stride1X;
			int Stride0X;
			int Channels;
			int CoordOrigin;
			int ChannelSwizzleR;
			int ChannelSwizzleG;
			int ChannelSwizzleB;
			int ChannelSwizzleA;
			float4 ChannelScale;
			float4 ChannelBias;
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
				uint4 r7;
				bool4 b3;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r1.xyz = asuint(int3(-1, -1, -1) + asint(float3(asfloat(HeightO), asfloat(WidthX), asfloat(HeightX))));
				r0.z = asuint(asint(r1.x) + -asint(r0.y));
				r0.y = (asuint(asfloat(CoordOrigin)) != 0) ? r0.y : r0.z;
				r2.xy = asuint(float2(r0.yx));
				r0.xy = asuint(float2(0.5, 0.5) + asfloat(r2.xy));
				r2 = asuint(float4(asuint(float4(asfloat(WidthO), asfloat(HeightO), asfloat(WidthX), asfloat(HeightX)))));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r2.yx));
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r2.wz) + float2(-0.5, -0.5));
				r0.zw = asuint(floor(asfloat(r0.xy)));
				r0.zw = uint2(asfloat(r0.zw));
				r1.xw = asuint(float2(r0.zw));
				r0.xy = asuint(-asfloat(r1.xw) + asfloat(r0.xy));
				r1.xw = min(r0.wz, r1.yz);
				r0.zw = asuint(int2(1, 1) + asint(r0.zw));
				r0.zw = min(r0.zw, r1.zy);
				r1.yz = asuint(float2(1.0, 1.0) + -asfloat(r0.xy));
				r2.xy = asuint(asint(r1.xw) * asint(float2(asfloat(Stride0X), asfloat(Stride1X))));
				r2.x = asuint(asint(r2.x) + asint(r2.y));
				r3.x = asuint(asfloat(WidthMaskX)) & r2.x;
				r3.y = r2.x >> asuint(asfloat(WidthShiftX));
				r3.zw = uint2(0u, 0u);
				r2 = asuint(Xptr.Load(int3(asint(r3.xy), asint(r3.w))));
				b3 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= asuint(float4(asfloat(ChannelSwizzleR), asfloat(ChannelSwizzleG), asfloat(ChannelSwizzleB), asfloat(ChannelSwizzleA))));
				r3.x = b3.x ? 0xffffffffu : 0u;
				r3.y = b3.y ? 0xffffffffu : 0u;
				r3.z = b3.z ? 0xffffffffu : 0u;
				r3.w = b3.w ? 0xffffffffu : 0u;
				[branch] if (b3.x)
				{
					r4.x = asuint(asfloat(ChannelSwizzleR));
					r4.x = asuint(dot(asfloat(r2), icb[asint(r4.x)]));
				}
				else
				{
					r4.x = 0u;
				}
				[branch] if (r3.y != 0)
				{
					r5.x = asuint(asfloat(ChannelSwizzleG));
					r4.y = asuint(dot(asfloat(r2), icb[asint(r5.x)]));
				}
				else
				{
					r4.y = 0u;
				}
				[branch] if (r3.z != 0)
				{
					r5.x = asuint(asfloat(ChannelSwizzleB));
					r4.z = asuint(dot(asfloat(r2), icb[asint(r5.x)]));
				}
				else
				{
					r4.z = 0u;
				}
				[branch] if (r3.w != 0)
				{
					r5.x = asuint(asfloat(ChannelSwizzleA));
					r4.w = asuint(dot(asfloat(r2), icb[asint(r5.x)]));
				}
				else
				{
					r4.w = 0u;
				}
				r2 = asuint(ChannelScale.xyzw * asfloat(r4) + ChannelBias.xyzw);
				r0.zw = asuint(asint(r0.zw) * asint(float2(asfloat(Stride1X), asfloat(Stride0X))));
				r1.xw = asuint(asint(r1.xw) * asint(float2(asfloat(Stride0X), asfloat(Stride1X))) + asint(r0.zw));
				r4.xy = asuint(float2(asfloat(WidthMaskX), asfloat(WidthMaskX))) & r1.xw;
				r5.xy = r1.xw >> asuint(float2(asfloat(WidthShiftX), asfloat(WidthShiftX)));
				r4.z = r5.x;
				r4.w = 0u;
				r6 = asuint(Xptr.Load(int3(asint(r4.xz), asint(r4.w))));
				[branch] if (r3.x != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleR));
					r7.x = asuint(dot(asfloat(r6), icb[asint(r1.x)]));
				}
				else
				{
					r7.x = 0u;
				}
				[branch] if (r3.y != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleG));
					r7.y = asuint(dot(asfloat(r6), icb[asint(r1.x)]));
				}
				else
				{
					r7.y = 0u;
				}
				[branch] if (r3.z != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleB));
					r7.z = asuint(dot(asfloat(r6), icb[asint(r1.x)]));
				}
				else
				{
					r7.z = 0u;
				}
				[branch] if (r3.w != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleA));
					r7.w = asuint(dot(asfloat(r6), icb[asint(r1.x)]));
				}
				else
				{
					r7.w = 0u;
				}
				r6 = asuint(ChannelScale.xyzw * asfloat(r7) + ChannelBias.xyzw);
				r6 = asuint(asfloat(r6) * asfloat(r0.xxxx));
				r2 = asuint(asfloat(r1.yyyy) * asfloat(r2) + asfloat(r6));
				r5.zw = r4.yw;
				r4 = asuint(Xptr.Load(int3(asint(r5.zy), asint(r5.w))));
				[branch] if (r3.x != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleR));
					r5.x = asuint(dot(asfloat(r4), icb[asint(r1.x)]));
				}
				else
				{
					r5.x = 0u;
				}
				[branch] if (r3.y != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleG));
					r5.y = asuint(dot(asfloat(r4), icb[asint(r1.x)]));
				}
				else
				{
					r5.y = 0u;
				}
				[branch] if (r3.z != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleB));
					r5.z = asuint(dot(asfloat(r4), icb[asint(r1.x)]));
				}
				else
				{
					r5.z = 0u;
				}
				[branch] if (r3.w != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleA));
					r5.w = asuint(dot(asfloat(r4), icb[asint(r1.x)]));
				}
				else
				{
					r5.w = 0u;
				}
				r4 = asuint(ChannelScale.xyzw * asfloat(r5) + ChannelBias.xyzw);
				r0.z = asuint(asint(r0.w) + asint(r0.z));
				r5.x = asuint(asfloat(WidthMaskX)) & r0.z;
				r5.y = r0.z >> asuint(asfloat(WidthShiftX));
				r5.zw = uint2(0u, 0u);
				r5 = asuint(Xptr.Load(int3(asint(r5.xy), asint(r5.w))));
				[branch] if (r3.x != 0)
				{
					r0.z = asuint(asfloat(ChannelSwizzleR));
					r6.x = asuint(dot(asfloat(r5), icb[asint(r0.z)]));
				}
				else
				{
					r6.x = 0u;
				}
				[branch] if (r3.y != 0)
				{
					r0.z = asuint(asfloat(ChannelSwizzleG));
					r6.y = asuint(dot(asfloat(r5), icb[asint(r0.z)]));
				}
				else
				{
					r6.y = 0u;
				}
				[branch] if (r3.z != 0)
				{
					r0.z = asuint(asfloat(ChannelSwizzleB));
					r6.z = asuint(dot(asfloat(r5), icb[asint(r0.z)]));
				}
				else
				{
					r6.z = 0u;
				}
				[branch] if (r3.w != 0)
				{
					r0.z = asuint(asfloat(ChannelSwizzleA));
					r6.w = asuint(dot(asfloat(r5), icb[asint(r0.z)]));
				}
				else
				{
					r6.w = 0u;
				}
				r3 = asuint(ChannelScale.xyzw * asfloat(r6) + ChannelBias.xyzw);
				r3 = asuint(asfloat(r3) * asfloat(r0.xxxx));
				r3 = asuint(asfloat(r1.yyyy) * asfloat(r4) + asfloat(r3));
				r0 = asuint(asfloat(r3) * asfloat(r0.yyyy));
				o_SV_Target0 = asfloat(r1.zzzz) * asfloat(r2) + asfloat(r0);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
