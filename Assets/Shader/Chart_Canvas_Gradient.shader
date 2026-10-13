Shader "Chart/Canvas/Gradient"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Angle ("Angle", Float) = 0
		_Combine ("Combine", Color) = (1,1,1,0)
		_ColorFrom ("From", Color) = (1,1,1,1)
		_ColorTo ("To", Color) = (1,1,1,1)
		_ChartTiling ("Tiling", Float) = 1
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}
	SubShader
	{
		Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass
		{
			Name "Default"
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
			float4 _GlobalsPad0[2];
			float _Angle;
			float _ChartTiling;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _Combine;
			float4 _ColorFrom;
			float4 _ColorTo;
			float4 _MainTex_ST;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float2 i_TEXCOORD0 : TEXCOORD0, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD1 : TEXCOORD1, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o_TEXCOORD1 = i_POSITION0.xyzw;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				r0.x = asuint(i_TEXCOORD0.x / _ChartTiling);
				r0.x = asuint(-0.5 + asfloat(r0.x));
				r0.y = asuint(-0.5 + i_TEXCOORD0.y);
				r0.z = asuint(0.0174532793 * _Angle);
				float sin0;
				float cos0;
				sincos(asfloat(r0.z), sin0, cos0);
				r1.x = asuint(sin0);
				r2.x = asuint(cos0);
				r0.y = asuint(asfloat(r2.x) * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) * asfloat(r1.x) + asfloat(r0.y));
				r0.x = asuint(0.5 + asfloat(r0.x));
				r1 = asuint(_ColorTo.xyzw + -_ColorFrom.xyzw);
				r0 = asuint(asfloat(r0.xxxx) * asfloat(r1) + _ColorFrom.xyzw);
				r1.xyz = asuint(_Combine.xyz + -asfloat(r0.xyz));
				o_COLOR0.xyz = _Combine.www * asfloat(r1.xyz) + asfloat(r0.xyz);
				o_COLOR0.w = asfloat(r0.w);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[7];
			float4 _ClipRect;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				b0.xy = (i_TEXCOORD1.xy >= _ClipRect.xy);
				b0.zw = (_ClipRect.zw >= i_TEXCOORD1.xy);
				r0 = b0 ? uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u) : uint4(0u, 0u, 0u, 0u);
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zw));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1 = asuint(i_COLOR0.xyzw * asfloat(r1));
				r0.y = asuint(asfloat(r1.w) * asfloat(r0.x) + -0.00100000005);
				r0.x = asuint(asfloat(r1.w) * asfloat(r0.x));
				o_SV_Target0.xyz = asfloat(r1.xyz);
				o_SV_Target0.w = asfloat(r0.x);
				b0.x = (asfloat(r0.y) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(UNITY_UI_ALPHACLIP)
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1.x = asuint(asfloat(r0.w) * i_COLOR0.w + -0.00100000005);
				r0 = asuint(i_COLOR0.xyzw * asfloat(r0));
				o_SV_Target0 = asfloat(r0);
				b0.x = (asfloat(r1.x) < 0.0);
				if (b0.x) discard;
			}
			#elif defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[7];
			float4 _ClipRect;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				b0.xy = (i_TEXCOORD1.xy >= _ClipRect.xy);
				b0.zw = (_ClipRect.zw >= i_TEXCOORD1.xy);
				r0 = b0 ? uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u) : uint4(0u, 0u, 0u, 0u);
				r0.xy = asuint(asfloat(r0.xy) * asfloat(r0.zw));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1 = asuint(i_COLOR0.xyzw * asfloat(r1));
				o_SV_Target0.w = asfloat(r1.w) * asfloat(r0.x);
				o_SV_Target0.xyz = asfloat(r1.xyz);
			}
			#elif 1
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				o_SV_Target0 = i_COLOR0.xyzw * asfloat(r0);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
