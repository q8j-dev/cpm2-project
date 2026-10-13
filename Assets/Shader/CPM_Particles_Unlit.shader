Shader "CPM/Particles/Unlit"
{
	Properties
	{
		_BaseMap ("Base Map", 2D) = "white" {}
		_BaseColor ("Base Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "PreviewType" = "Plane" "QUEUE" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" }
		Pass
		{
			Name "ParticleUnlit"
			Tags { "LIGHTMODE" = "UniversalForward" }
			Blend SrcAlpha One, SrcAlpha One
			ColorMask 14
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ FOG_EXP

			#if defined(SHADER_STAGE_VERTEX)
			#if defined(FOG_EXP)
			float4 _GlobalsPad0[21];
			float4 _ProjectionParams;
			float4 _GlobalsPad1[36];
			float4 unity_FogParams;
			float4 _GlobalsPad2[17];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float4 _BaseMap_ST : packoffset(c0);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0));
				o_SV_POSITION0 = asfloat(r0);
				r0.x = asuint(asfloat(r0.z) / _ProjectionParams.y);
				r0.x = asuint(1.0 + -asfloat(r0.x));
				r0.x = asuint(_ProjectionParams.z * asfloat(r0.x));
				r0.x = asuint(max(0.0, asfloat(r0.x)));
				o_TEXCOORD1 = unity_FogParams.x * asfloat(r0.x);
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
			}
			#elif 1
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float4 _BaseMap_ST : packoffset(c0);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_COLOR0 : COLOR0, out float2 o_TEXCOORD0 : TEXCOORD0, out float o_TEXCOORD1 : TEXCOORD1)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0);
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD0 = i_TEXCOORD0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				o_TEXCOORD1 = 0.0;
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if defined(FOG_EXP)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[121];
			float4 _GlobalAmbientEquator;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _BaseColor : packoffset(c1);
			}
			Texture2D<float4> _BaseMap : register(t0);
			SamplerState sampler_BaseMap : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_BaseMap.SampleBias(sampler_BaseMap, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0 = asuint(i_COLOR0.xyzw * asfloat(r0));
				r0.xyz = asuint(_BaseColor.xyz * asfloat(r0.xyz));
				o_SV_Target0.w = _BaseColor.w * asfloat(r0.w);
				r1.xyz = asuint(max(float3(0.0199999996, 0.0199999996, 0.0199999996), _GlobalAmbientEquator.xyz));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.xyz));
				r0.w = asuint(exp2(-i_TEXCOORD1));
				r0.w = asuint(min(1.0, asfloat(r0.w)));
				o_SV_Target0.xyz = asfloat(r0.www) * asfloat(r0.xyz);
			}
			#elif 1
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[121];
			float4 _GlobalAmbientEquator;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _BaseColor : packoffset(c1);
			}
			Texture2D<float4> _BaseMap : register(t0);
			SamplerState sampler_BaseMap : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_COLOR0 : COLOR0, float2 i_TEXCOORD0 : TEXCOORD0, float i_TEXCOORD1 : TEXCOORD1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				r0 = asuint(_BaseMap.SampleBias(sampler_BaseMap, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r0 = asuint(i_COLOR0.xyzw * asfloat(r0));
				r0.xyz = asuint(_BaseColor.xyz * asfloat(r0.xyz));
				o_SV_Target0.w = _BaseColor.w * asfloat(r0.w);
				r1.xyz = asuint(max(float3(0.0199999996, 0.0199999996, 0.0199999996), _GlobalAmbientEquator.xyz));
				o_SV_Target0.xyz = asfloat(r1.xyz) * asfloat(r0.xyz);
			}
			#endif
			#endif
			ENDHLSL
		}
	}
	Fallback "Universal Render Pipeline/Particles/Unlit"
}
