Shader "Hidden/Sentis/TextureConversion/TextureToTensor"
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
			int WidthShiftO;
			int StrideWidthO;
			int StrideHeightO;
			int WidthO;
			int HeightO;
			int Channels;
			int CoordOrigin;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.xy = r0.xx / asuint(float2(asfloat(StrideWidthO), asfloat(StrideHeightO)));
				r0.y = r0.y % asuint(asfloat(HeightO));
				r1.x = r0.x % asuint(asfloat(WidthO));
				r0.x = asuint(-1 + asint(asfloat(HeightO)));
				r0.x = asuint(asint(r0.x) + -asint(r0.y));
				r1.y = (asuint(asfloat(CoordOrigin)) != 0) ? r0.y : r0.x;
				r1.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				b1 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(1u, 2u, 3u, 4u));
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r1 & r0);
			}
			#elif defined(BGRA) && defined(EXACT)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int StrideWidthO;
			int StrideHeightO;
			int WidthO;
			int HeightO;
			int Channels;
			int CoordOrigin;
			Texture2D<float4> Xptr : register(t0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.xy = r0.xx / asuint(float2(asfloat(StrideWidthO), asfloat(StrideHeightO)));
				r0.y = r0.y % asuint(asfloat(HeightO));
				r1.x = r0.x % asuint(asfloat(WidthO));
				r0.x = asuint(-1 + asint(asfloat(HeightO)));
				r0.x = asuint(asint(r0.x) + -asint(r0.y));
				r1.y = (asuint(asfloat(CoordOrigin)) != 0) ? r0.y : r0.x;
				r1.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				b1 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(1u, 2u, 3u, 4u));
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r1 & r0.zyxw);
			}
			#elif defined(LINEAR) && defined(RGBA)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int StrideWidthO;
			int StrideHeightO;
			int WidthO;
			int HeightO;
			int Channels;
			int CoordOrigin;
			Texture2D<float4> Xptr : register(t0);
			SamplerState samplerXptr : register(s0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.xy = r0.xx / asuint(float2(asfloat(StrideWidthO), asfloat(StrideHeightO)));
				r0.y = r0.y % asuint(asfloat(HeightO));
				r0.x = r0.x % asuint(asfloat(WidthO));
				r1.x = asuint(float(r0.x));
				r0.x = asuint(-1 + asint(asfloat(HeightO)));
				r0.x = asuint(asint(r0.x) + -asint(r0.y));
				r0.x = (asuint(asfloat(CoordOrigin)) != 0) ? r0.y : r0.x;
				r1.y = asuint(float(r0.x));
				r0.xy = asuint(float2(0.5, 0.5) + asfloat(r1.xy));
				r1.x = asuint(float(asuint(asfloat(WidthO))));
				r1.y = asuint(float(asuint(asfloat(HeightO))));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r1.xy));
				r0 = asuint(Xptr.Sample(samplerXptr, asfloat(r0.xy)));
				b1 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(1u, 2u, 3u, 4u));
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r1 & r0);
			}
			#elif defined(BGRA) && defined(LINEAR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int StrideWidthO;
			int StrideHeightO;
			int WidthO;
			int HeightO;
			int Channels;
			int CoordOrigin;
			Texture2D<float4> Xptr : register(t0);
			SamplerState samplerXptr : register(s0);

			void frag(float4 i_SV_Position0 : SV_Position, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.xy = r0.xx / asuint(float2(asfloat(StrideWidthO), asfloat(StrideHeightO)));
				r0.y = r0.y % asuint(asfloat(HeightO));
				r0.x = r0.x % asuint(asfloat(WidthO));
				r1.x = asuint(float(r0.x));
				r0.x = asuint(-1 + asint(asfloat(HeightO)));
				r0.x = asuint(asint(r0.x) + -asint(r0.y));
				r0.x = (asuint(asfloat(CoordOrigin)) != 0) ? r0.y : r0.x;
				r1.y = asuint(float(r0.x));
				r0.xy = asuint(float2(0.5, 0.5) + asfloat(r1.xy));
				r1.x = asuint(float(asuint(asfloat(WidthO))));
				r1.y = asuint(float(asuint(asfloat(HeightO))));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r1.xy));
				r0 = asuint(Xptr.Sample(samplerXptr, asfloat(r0.xy)));
				b1 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(1u, 2u, 3u, 4u));
				r1.x = b1.x ? 0xffffffffu : 0u;
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				o_SV_Target0 = asfloat(r1 & r0.zyxw);
			}
			#elif defined(EXACT)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int StrideWidthO;
			int StrideHeightO;
			int WidthO;
			int HeightO;
			int Channels;
			int CoordOrigin;
			int ChannelSwizzleR;
			int ChannelSwizzleG;
			int ChannelSwizzleB;
			int ChannelSwizzleA;
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
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.xy = r0.xx / asuint(float2(asfloat(StrideWidthO), asfloat(StrideHeightO)));
				r1.x = r0.x % asuint(asfloat(WidthO));
				r0.x = r0.y % asuint(asfloat(HeightO));
				r0.y = asuint(-1 + asint(asfloat(HeightO)));
				r0.y = asuint(asint(r0.y) + -asint(r0.x));
				r1.y = (asuint(asfloat(CoordOrigin)) != 0) ? r0.x : r0.y;
				r1.zw = uint2(0u, 0u);
				r0 = asuint(Xptr.Load(int3(asint(r1.xy), asint(r1.w))));
				b1 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(1u, 2u, 3u, 4u));
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				[branch] if (b1.x)
				{
					r1.x = asuint(asfloat(ChannelSwizzleR));
					o_SV_Target0.x = dot(asfloat(r0), icb[asint(r1.x)]);
				}
				else
				{
					o_SV_Target0.x = 0.0;
				}
				[branch] if (r1.y != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleG));
					o_SV_Target0.y = dot(asfloat(r0), icb[asint(r1.x)]);
				}
				else
				{
					o_SV_Target0.y = 0.0;
				}
				[branch] if (r1.z != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleB));
					o_SV_Target0.z = dot(asfloat(r0), icb[asint(r1.x)]);
				}
				else
				{
					o_SV_Target0.z = 0.0;
				}
				[branch] if (r1.w != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleA));
					o_SV_Target0.w = dot(asfloat(r0), icb[asint(r1.x)]);
				}
				else
				{
					o_SV_Target0.w = 0.0;
				}
			}
			#elif defined(LINEAR)
			float4 _GlobalsPad0[2];
			int WidthShiftO;
			int StrideWidthO;
			int StrideHeightO;
			int WidthO;
			int HeightO;
			int Channels;
			int CoordOrigin;
			int ChannelSwizzleR;
			int ChannelSwizzleG;
			int ChannelSwizzleB;
			int ChannelSwizzleA;
			Texture2D<float4> Xptr : register(t0);
			SamplerState samplerXptr : register(s0);
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
				bool4 b1;
				r0.xy = asuint(float2(-0.5, -0.5) + i_SV_Position0.xy);
				r0.xy = uint2(asfloat(r0.xy));
				r0.y = asuint(asint(r0.y) << asint(asfloat(WidthShiftO)));
				r0.x = asuint(asint(r0.y) + asint(r0.x));
				r0.xy = r0.xx / asuint(float2(asfloat(StrideWidthO), asfloat(StrideHeightO)));
				r0.x = r0.x % asuint(asfloat(WidthO));
				r0.y = r0.y % asuint(asfloat(HeightO));
				r0.z = asuint(-1 + asint(asfloat(HeightO)));
				r0.z = asuint(asint(r0.z) + -asint(r0.y));
				r0.y = (asuint(asfloat(CoordOrigin)) != 0) ? r0.y : r0.z;
				r1.xy = asuint(float2(r0.xy));
				r0.xy = asuint(float2(0.5, 0.5) + asfloat(r1.xy));
				r1.x = asuint(float(asuint(asfloat(WidthO))));
				r1.y = asuint(float(asuint(asfloat(HeightO))));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r1.xy));
				r0 = asuint(Xptr.Sample(samplerXptr, asfloat(r0.xy)));
				b1 = (asuint(float4(asfloat(Channels), asfloat(Channels), asfloat(Channels), asfloat(Channels))) >= uint4(1u, 2u, 3u, 4u));
				r1.y = b1.y ? 0xffffffffu : 0u;
				r1.z = b1.z ? 0xffffffffu : 0u;
				r1.w = b1.w ? 0xffffffffu : 0u;
				[branch] if (b1.x)
				{
					r1.x = asuint(asfloat(ChannelSwizzleR));
					o_SV_Target0.x = dot(asfloat(r0), icb[asint(r1.x)]);
				}
				else
				{
					o_SV_Target0.x = 0.0;
				}
				[branch] if (r1.y != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleG));
					o_SV_Target0.y = dot(asfloat(r0), icb[asint(r1.x)]);
				}
				else
				{
					o_SV_Target0.y = 0.0;
				}
				[branch] if (r1.z != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleB));
					o_SV_Target0.z = dot(asfloat(r0), icb[asint(r1.x)]);
				}
				else
				{
					o_SV_Target0.z = 0.0;
				}
				[branch] if (r1.w != 0)
				{
					r1.x = asuint(asfloat(ChannelSwizzleA));
					o_SV_Target0.w = dot(asfloat(r0), icb[asint(r1.x)]);
				}
				else
				{
					o_SV_Target0.w = 0.0;
				}
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
