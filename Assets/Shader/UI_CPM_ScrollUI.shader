Shader "UI/CPM/ScrollUI"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_ScrollDir ("Scroll Direction (X,Y)", Vector) = (0.1,0,0,0)
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
	}
	SubShader
	{
		Tags { "Canvas" = "Overlay" "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
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
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ UNITY_UI_CLIP_RECT

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD1 : TEXCOORD1, out float4 o_COLOR0 : COLOR0)
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
				o_TEXCOORD0 = i_TEXCOORD0.xy;
				o_TEXCOORD1 = i_POSITION0.xy;
				o_COLOR0 = i_COLOR0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[9];
			float4 _Time;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _ScrollDir : packoffset(c1);
				float4 _ClipRect : packoffset(c2);
				float _UIMaskSoftnessX : packoffset(c3);
				float _UIMaskSoftnessY : packoffset(c3.y);
				float _MaskSoftnessX : packoffset(c3.z);
				float _MaskSoftnessY : packoffset(c3.w);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _MaskTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_MaskTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b1;
				b0.xy = (i_TEXCOORD1.xy >= _ClipRect.xy);
				r0.xy = b0.xy ? uint2(0x3f800000u, 0x3f800000u) : uint2(0u, 0u);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				b0.yz = (_ClipRect.zw >= i_TEXCOORD1.xy);
				r0.yz = b0.yz ? uint2(0x3f800000u, 0x3f800000u) : uint2(0u, 0u);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r0.yw = asuint(-_ClipRect.xy + i_TEXCOORD1.xy);
				r1.xy = asuint(max(float2(_UIMaskSoftnessX, _UIMaskSoftnessY), float2(_MaskSoftnessX, _MaskSoftnessY)));
				r1.zw = asuint(max(float2(9.99999975e-05, 9.99999975e-05), asfloat(r1.xy)));
				r1.x = asuint(asfloat(r1.x) + asfloat(r1.y));
				b1.x = (0.0 >= asfloat(r1.x));
				r1.x = b1.x ? 0x3f800000u : 0u;
				r1.yz = asuint(float2(1.0, 1.0) / asfloat(r1.zw));
				r0.yw = asuint(saturate(asfloat(r1.yz) * asfloat(r0.yw)));
				r2.xy = asuint(_ClipRect.zw + -i_TEXCOORD1.xy);
				r1.yz = asuint(saturate(asfloat(r2.xy) * asfloat(r1.yz)));
				r0.yw = asuint(asfloat(r1.yz) * asfloat(r0.yw));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z) + -asfloat(r0.y));
				r0.x = asuint(asfloat(r1.x) * asfloat(r0.x) + asfloat(r0.y));
				r0.yz = asuint(_ScrollDir.xy * _Time.yy + i_TEXCOORD0.xy);
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r1 = asuint(_MainTex.SampleBias(sampler_MainTex, asfloat(r0.yz), _GlobalMipBias.x));
				r1 = asuint(i_COLOR0.xyzw * asfloat(r1));
				r2 = asuint(_MaskTex.SampleBias(sampler_MaskTex, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r1 = asuint(asfloat(r2.wwww) * asfloat(r1));
				r0.y = asuint(asfloat(r1.w) * asfloat(r0.x) + -0.00100000005);
				r0.x = asuint(asfloat(r1.w) * asfloat(r0.x));
				o_SV_Target0.xyz = asfloat(r1.xyz);
				o_SV_Target0.w = asfloat(r0.x);
				b0.x = (asfloat(r0.y) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(UNITY_UI_ALPHACLIP)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[9];
			float4 _Time;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _ScrollDir : packoffset(c1);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _MaskTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_MaskTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.xy = asuint(_ScrollDir.xy * _Time.yy + i_TEXCOORD0.xy);
				r0.xy = asuint(frac(asfloat(r0.xy)));
				r0 = asuint(_MainTex.SampleBias(sampler_MainTex, asfloat(r0.xy), _GlobalMipBias.x));
				r0 = asuint(i_COLOR0.xyzw * asfloat(r0));
				r1 = asuint(_MaskTex.SampleBias(sampler_MaskTex, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r1.x = asuint(asfloat(r0.w) * asfloat(r1.w) + -0.00100000005);
				r0 = asuint(asfloat(r1.wwww) * asfloat(r0));
				o_SV_Target0 = asfloat(r0);
				b0.x = (asfloat(r1.x) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[9];
			float4 _Time;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _ScrollDir : packoffset(c1);
				float4 _ClipRect : packoffset(c2);
				float _UIMaskSoftnessX : packoffset(c3);
				float _UIMaskSoftnessY : packoffset(c3.y);
				float _MaskSoftnessX : packoffset(c3.z);
				float _MaskSoftnessY : packoffset(c3.w);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _MaskTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_MaskTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b1;
				b0.xy = (i_TEXCOORD1.xy >= _ClipRect.xy);
				r0.xy = b0.xy ? uint2(0x3f800000u, 0x3f800000u) : uint2(0u, 0u);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				b0.yz = (_ClipRect.zw >= i_TEXCOORD1.xy);
				r0.yz = b0.yz ? uint2(0x3f800000u, 0x3f800000u) : uint2(0u, 0u);
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r0.yw = asuint(-_ClipRect.xy + i_TEXCOORD1.xy);
				r1.xy = asuint(max(float2(_UIMaskSoftnessX, _UIMaskSoftnessY), float2(_MaskSoftnessX, _MaskSoftnessY)));
				r1.zw = asuint(max(float2(9.99999975e-05, 9.99999975e-05), asfloat(r1.xy)));
				r1.x = asuint(asfloat(r1.x) + asfloat(r1.y));
				b1.x = (0.0 >= asfloat(r1.x));
				r1.x = b1.x ? 0x3f800000u : 0u;
				r1.yz = asuint(float2(1.0, 1.0) / asfloat(r1.zw));
				r0.yw = asuint(saturate(asfloat(r1.yz) * asfloat(r0.yw)));
				r2.xy = asuint(_ClipRect.zw + -i_TEXCOORD1.xy);
				r1.yz = asuint(saturate(asfloat(r2.xy) * asfloat(r1.yz)));
				r0.yw = asuint(asfloat(r1.yz) * asfloat(r0.yw));
				r0.y = asuint(asfloat(r0.y) * asfloat(r0.w));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.z) + -asfloat(r0.y));
				r0.x = asuint(asfloat(r1.x) * asfloat(r0.x) + asfloat(r0.y));
				r0.yz = asuint(_ScrollDir.xy * _Time.yy + i_TEXCOORD0.xy);
				r0.yz = asuint(frac(asfloat(r0.yz)));
				r1 = asuint(_MainTex.SampleBias(sampler_MainTex, asfloat(r0.yz), _GlobalMipBias.x));
				r1 = asuint(i_COLOR0.xyzw * asfloat(r1));
				r2 = asuint(_MaskTex.SampleBias(sampler_MaskTex, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r1 = asuint(asfloat(r2.wwww) * asfloat(r1));
				o_SV_Target0.w = asfloat(r1.w) * asfloat(r0.x);
				o_SV_Target0.xyz = asfloat(r1.xyz);
			}
			#elif 1
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[9];
			float4 _Time;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _ScrollDir : packoffset(c1);
			}
			Texture2D<float4> _MainTex : register(t0);
			Texture2D<float4> _MaskTex : register(t1);
			SamplerState sampler_MainTex : register(s0);
			SamplerState sampler_MaskTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, float2 i_TEXCOORD1 : TEXCOORD1, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(_ScrollDir.xy * _Time.yy + i_TEXCOORD0.xy);
				r0.xy = asuint(frac(asfloat(r0.xy)));
				r0 = asuint(_MainTex.SampleBias(sampler_MainTex, asfloat(r0.xy), _GlobalMipBias.x));
				r0 = asuint(i_COLOR0.xyzw * asfloat(r0));
				r1 = asuint(_MaskTex.SampleBias(sampler_MaskTex, i_TEXCOORD0.xy, _GlobalMipBias.x));
				o_SV_Target0 = asfloat(r1.wwww) * asfloat(r0);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
