Shader "GUI/CPM Text Shader"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_ColorMask ("Color Mask", Float) = 15
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
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _Color;
			float4 _MainTex_ST;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b1;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				r0.xyz = asuint(i_COLOR0.xyz * float3(0.265884995, 0.265884995, 0.265884995) + float3(0.736584008, 0.736584008, 0.736584008));
				r0.xyz = asuint(i_COLOR0.xyz * asfloat(r0.xyz) + float3(-0.00980184041, -0.00980184041, -0.00980184041));
				r0.xyz = asuint(i_COLOR0.xyz * asfloat(r0.xyz) + float3(0.00319697009, 0.00319697009, 0.00319697009));
				b1.xyz = (i_COLOR0.xyz < float3(0.0725490004, 0.0725490004, 0.0725490004));
				r2.xyz = asuint(i_COLOR0.xyz * float3(0.0849710032, 0.0849710032, 0.0849710032) + float3(-0.000163029006, -0.000163029006, -0.000163029006));
				r0.xyz = b1.xyz ? r2.xyz : r0.xyz;
				r0.w = asuint(i_COLOR0.w);
				o_COLOR0 = _Color.xyzw * asfloat(r0);
				o_TEXCOORD0 = i_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o_TEXCOORD1 = i_POSITION0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(255.0 * i_COLOR0.w);
				r0.x = asuint(round(asfloat(r0.x)));
				r0.w = asuint(0.00392156886 * asfloat(r0.x));
				r1 = asuint(_MainTex.Sample(sampler_MainTex, i_TEXCOORD0.xy));
				r0.xyz = asuint(i_COLOR0.xyz);
				o_SV_Target0 = asfloat(r1.wwww) * asfloat(r0);
			}
			#endif
			ENDHLSL
		}
	}
}
