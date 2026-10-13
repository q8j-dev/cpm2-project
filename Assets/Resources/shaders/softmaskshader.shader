Shader "UI Extensions/SoftMaskShader"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
		_AlphaMask ("AlphaMask - Must be Wrapped", 2D) = "white" {}
		_CutOff ("CutOff", Float) = 0
		[MaterialToggle] _HardBlend ("HardBlend", Float) = 0
		_FlipAlphaMask ("Flip Alpha Mask", Float) = 0
		_NoOuterClip ("Outer Clip", Float) = 0
	}
	SubShader
	{
		Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ColorMask [_ColorMask]
			Cull Off
			ZTest [unity_GUIZTestMode]
			ZWrite Off
			Stencil { Ref [_Stencil] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask] Comp [_StencilComp] Pass [_StencilOp] Fail Keep ZFail Keep }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _Color;
			float4 _GlobalsPad1[2];
			float4 _AlphaMask_ST;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1)
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
				o_COLOR0 = _Color.xyzw * i_COLOR0.xyzw;
				o_TEXCOORD1 = i_POSITION0.xy * _AlphaMask_ST.xy + _AlphaMask_ST.zw;
				o_TEXCOORD0 = i_TEXCOORD0.xy;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[3];
			float4 _TextureSampleAdd;
			uint _UseAlphaClip;
			int _FlipAlphaMask;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[1];
			float _CutOff;
			uint _HardBlend;
			uint _NoOuterClip;
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _AlphaMask : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_AlphaMask : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b1;
				bool4 b2;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0 = asuint(_TextureSampleAdd.xyzw + asfloat(r0));
				r0 = asuint(i_COLOR0.wxyz * asfloat(r0.wxyz));
				b1.xy = (i_TEXCOORD1.xy >= float2(0.0, 0.0));
				b1.zw = (float2(1.0, 1.0) >= i_TEXCOORD1.xy);
				r1 = b1 ? uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u) : uint4(0u, 0u, 0u, 0u);
				b2.x = (0 != asint(asfloat(_NoOuterClip)));
				r2.x = b2.x ? 0xffffffffu : 0u;
				b2.x = (0 == asint(r2.x));
				r1.xy = asuint(asfloat(r1.xy) * asfloat(r1.zw));
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y));
				b1.x = (0.0 != asfloat(r1.x));
				r1.x = b1.x ? 0xffffffffu : 0u;
				b1.x = (0 == asint(r1.x));
				b1.x = b2.x && b1.x;
				r1.x = b1.x ? 0xffffffffu : 0u;
				[branch] if (b1.x)
				{
					r0.x = 0u;
				}
				else
				{
					r1 = asuint(_AlphaMask.Sample(sampler_AlphaMask, i_TEXCOORD1.xy));
					b1.x = (_CutOff >= asfloat(r1.w));
					r1.y = (asuint(asfloat(_HardBlend)) != 0) ? 0x3f800000u : r1.w;
					r1.x = b1.x ? 0u : r1.y;
					b1.y = (1 == asint(asfloat(_FlipAlphaMask)));
					r1.z = asuint(1.0 + -asfloat(r1.x));
					r1.x = b1.y ? r1.z : r1.x;
					r0.x = asuint(asfloat(r1.x) * asfloat(r0.x));
				}
				b1.x = (0 != asint(asfloat(_UseAlphaClip)));
				r1.y = asuint(-0.00100000005 + asfloat(r0.x));
				b1.y = (asfloat(r1.y) < 0.0);
				b1.x = b1.y && b1.x;
				if (b1.x) discard;
				o_SV_Target0 = asfloat(r0.yzwx);
			}
			#endif
			ENDHLSL
		}
	}
}
