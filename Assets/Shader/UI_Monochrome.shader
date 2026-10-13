Shader "UI/Monochrome"
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
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}
	SubShader
	{
		Tags { "CanUseSpriteAtlas" = "true" "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass
		{
			Name "Default"
			Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
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
			float4 _GlobalsPad0[4];
			float4 _ClipRect;
			float4 _MainTex_ST;
			float _UIMaskSoftnessX;
			float _UIMaskSoftnessY;
			int _UIVertexColorAlwaysGammaSpace;
			cbuffer UnityPerCamera : register(b1)
			{
				float4 _ScreenParams : packoffset(c6);
			}
			cbuffer UnityPerDraw : register(b2)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b3)
			{
				float4x4 glstate_matrix_projection : packoffset(c5);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD1 : TEXCOORD1, out float4 o_TEXCOORD2 : TEXCOORD2)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b2;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1));
				o_SV_POSITION0 = asfloat(r0);
				r0.xyz = asuint(i_COLOR0.xyz * float3(0.265884995, 0.265884995, 0.265884995) + float3(0.736584008, 0.736584008, 0.736584008));
				r0.xyz = asuint(i_COLOR0.xyz * asfloat(r0.xyz) + float3(-0.00980184041, -0.00980184041, -0.00980184041));
				r0.xyz = asuint(i_COLOR0.xyz * asfloat(r0.xyz) + float3(0.00319697009, 0.00319697009, 0.00319697009));
				r1.xyz = asuint(i_COLOR0.xyz * float3(0.0849710032, 0.0849710032, 0.0849710032) + float3(-0.000163029006, -0.000163029006, -0.000163029006));
				b2.xyz = (i_COLOR0.xyz < float3(0.0725490004, 0.0725490004, 0.0725490004));
				r0.xyz = b2.xyz ? r1.xyz : r0.xyz;
				o_COLOR0.xyz = (asuint(float3(asfloat(_UIVertexColorAlwaysGammaSpace), asfloat(_UIVertexColorAlwaysGammaSpace), asfloat(_UIVertexColorAlwaysGammaSpace))) != 0) ? asfloat(r0.xyz) : i_COLOR0.xyz;
				o_COLOR0.w = i_COLOR0.w;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o_TEXCOORD1 = i_POSITION0.xyzw;
				r0.xy = asuint(glstate_matrix_projection._m01_m11 * _ScreenParams.yy);
				r0.xy = asuint(glstate_matrix_projection._m00_m10 * _ScreenParams.xx + asfloat(r0.xy));
				r0.xy = asuint(asfloat(r0.ww) / abs(asfloat(r0.xy)));
				r0.xy = asuint(float2(_UIMaskSoftnessX, _UIMaskSoftnessY) * float2(0.25, 0.25) + abs(asfloat(r0.xy)));
				o_TEXCOORD2.zw = float2(0.25, 0.25) / asfloat(r0.xy);
				r0 = asuint(max(float4(-2.0e+10, -2.0e+10, -2.0e+10, -2.0e+10), _ClipRect.xyzw));
				r0 = asuint(min(float4(2.0e+10, 2.0e+10, 2.0e+10, 2.0e+10), asfloat(r0)));
				r0.xy = asuint(i_POSITION0.xy * float2(2.0, 2.0) + -asfloat(r0.xy));
				o_TEXCOORD2.xy = asfloat(r0.xy) + -asfloat(r0.zw);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(UNITY_UI_ALPHACLIP) && defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[2];
			float4 _Color;
			float4 _TextureSampleAdd;
			float4 _ClipRect;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.xy = asuint(_ClipRect.zw + -_ClipRect.xy);
				r0.xy = asuint(-abs(i_TEXCOORD2.xy) + asfloat(r0.xy));
				r0.xy = asuint(saturate(i_TEXCOORD2.zw * asfloat(r0.xy)));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y));
				r0.y = asuint(255.0 * i_COLOR0.w);
				r0.y = asuint(round(asfloat(r0.y)));
				r0.y = asuint(0.00392156886 * asfloat(r0.y));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1 = asuint(_TextureSampleAdd.xyzw + asfloat(r1));
				r0.zw = asuint(_Color.xw * asfloat(r1.xw));
				r1.xyz = asuint(_Color.xyz * asfloat(r1.xyz) + -asfloat(r0.zzz));
				r1.xyz = asuint(i_COLOR0.xxx * asfloat(r1.xyz) + asfloat(r0.zzz));
				r0.y = asuint(asfloat(r0.w) * asfloat(r0.y));
				r0.z = asuint(asfloat(r0.y) * asfloat(r0.x) + -0.00100000005);
				r0.x = asuint(asfloat(r0.y) * asfloat(r0.x));
				b0.y = (asfloat(r0.z) < 0.0);
				if (b0.y) discard;
				o_SV_Target0.xyz = asfloat(r1.xyz) * asfloat(r0.xxx);
				o_SV_Target0.w = asfloat(r0.x);
			}
			#elif defined(UNITY_UI_ALPHACLIP)
			float4 _GlobalsPad0[2];
			float4 _Color;
			float4 _TextureSampleAdd;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.x = asuint(255.0 * i_COLOR0.w);
				r0.x = asuint(round(asfloat(r0.x)));
				r0.x = asuint(0.00392156886 * asfloat(r0.x));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r1 = asuint(_TextureSampleAdd.xyzw + asfloat(r1));
				r0.yz = asuint(_Color.xw * asfloat(r1.xw));
				r1.xyz = asuint(_Color.xyz * asfloat(r1.xyz) + -asfloat(r0.yyy));
				r1.xyz = asuint(i_COLOR0.xxx * asfloat(r1.xyz) + asfloat(r0.yyy));
				r0.y = asuint(asfloat(r0.z) * asfloat(r0.x) + -0.00100000005);
				r0.x = asuint(asfloat(r0.z) * asfloat(r0.x));
				b0.y = (asfloat(r0.y) < 0.0);
				if (b0.y) discard;
				o_SV_Target0.xyz = asfloat(r1.xyz) * asfloat(r0.xxx);
				o_SV_Target0.w = asfloat(r0.x);
			}
			#elif defined(UNITY_UI_CLIP_RECT)
			float4 _GlobalsPad0[2];
			float4 _Color;
			float4 _TextureSampleAdd;
			float4 _ClipRect;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0 = asuint(_TextureSampleAdd.xyzw + asfloat(r0));
				r1.xy = asuint(_Color.xw * asfloat(r0.xw));
				r0.xyz = asuint(_Color.xyz * asfloat(r0.xyz) + -asfloat(r1.xxx));
				r0.xyz = asuint(i_COLOR0.xxx * asfloat(r0.xyz) + asfloat(r1.xxx));
				r0.w = asuint(255.0 * i_COLOR0.w);
				r0.w = asuint(round(asfloat(r0.w)));
				r0.w = asuint(0.00392156886 * asfloat(r0.w));
				r0.w = asuint(asfloat(r1.y) * asfloat(r0.w));
				r1.xy = asuint(_ClipRect.zw + -_ClipRect.xy);
				r1.xy = asuint(-abs(i_TEXCOORD2.xy) + asfloat(r1.xy));
				r1.xy = asuint(saturate(i_TEXCOORD2.zw * asfloat(r1.xy)));
				r1.x = asuint(asfloat(r1.x) * asfloat(r1.y));
				r0.w = asuint(asfloat(r1.x) * asfloat(r0.w));
				o_SV_Target0.xyz = asfloat(r0.xyz) * asfloat(r0.www);
				o_SV_Target0.w = asfloat(r0.w);
			}
			#elif 1
			float4 _GlobalsPad0[2];
			float4 _Color;
			float4 _TextureSampleAdd;
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0 = asuint(_TextureSampleAdd.xyzw + asfloat(r0));
				r1.xy = asuint(_Color.xw * asfloat(r0.xw));
				r0.xyz = asuint(_Color.xyz * asfloat(r0.xyz) + -asfloat(r1.xxx));
				r0.xyz = asuint(i_COLOR0.xxx * asfloat(r0.xyz) + asfloat(r1.xxx));
				r0.w = asuint(255.0 * i_COLOR0.w);
				r0.w = asuint(round(asfloat(r0.w)));
				r0.w = asuint(0.00392156886 * asfloat(r0.w));
				r0.w = asuint(asfloat(r1.y) * asfloat(r0.w));
				o_SV_Target0.xyz = asfloat(r0.xyz) * asfloat(r0.www);
				o_SV_Target0.w = asfloat(r0.w);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
}
