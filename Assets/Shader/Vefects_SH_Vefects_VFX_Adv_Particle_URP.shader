Shader "Vefects/SH_Vefects_VFX_Adv_Particle_URP"
{
	Properties
	{
		[HideInInspector] _EmissionColor ("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff ("Alpha Cutoff ", Range(0, 1)) = 0.5
		_Noise_01_Texture ("Noise_01_Texture", 2D) = "white" {}
		_ColorGradientMask ("Color Gradient Mask", 2D) = "white" {}
		_Mask_Texture ("Mask_Texture", 2D) = "white" {}
		_ColorBottom ("Color Bottom", Color) = (1,1,1,0)
		_ColorTop ("Color Top", Color) = (1,1,1,0)
		_NoiseDistortion_Texture ("NoiseDistortion_Texture", 2D) = "white" {}
		_Noise_01_Scale ("Noise_01_Scale", Vector) = (1,1,0,0)
		_GradientColor_Scale ("GradientColor_Scale", Vector) = (1,1,0,0)
		_NoiseDistortion_Scale ("NoiseDistortion_Scale", Vector) = (1,1,0,0)
		_Noise_01_Speed ("Noise_01_Speed", Vector) = (0,0,0,0)
		_Mask_Scale ("Mask_Scale", Vector) = (1,1,0,0)
		_Mask_Offset ("Mask_Offset", Vector) = (0,0,0,0)
		_NoiseDistortion_Speed ("NoiseDistortion_Speed", Vector) = (0,0,0,0)
		_GradientColor_Speed ("GradientColor_Speed", Vector) = (0,0,0,0)
		_UseColor ("UseColor", Range(0, 1)) = 0
		_Mask_Multiply ("Mask_Multiply", Float) = 1
		_Noises_Multiply ("Noises_Multiply", Float) = 1
		_Mask_Power ("Mask_Power", Float) = 1
		_Noises_Power ("Noises_Power", Float) = 1
		_DistortionAmount ("Distortion Amount", Float) = 0
		_DistortionMaskIntensity ("Distortion Mask Intensity", Float) = 1
		_WindSpeed ("Wind Speed", Float) = 1
		_Erosion_Intensity ("Erosion_Intensity", Float) = 0
		_Erosion_Sub ("Erosion_Sub", Float) = 0
		[Space(33)] [Header(Emission)] [Space(13)] _EmissiveIntensity ("Emissive Intensity", Float) = 1
		[Space(33)] [Header(Depth Fade)] [Space(13)] _DepthFade ("Depth Fade", Float) = 0
		[Space(33)] [Header(Cam Offset)] [Space(13)] _CameraOffset ("Camera Offset", Float) = 0
		[Space(33)] [Header(AR)] [Space(13)] _Cull ("Cull", Float) = 2
		_Src ("Src", Float) = 1
		_Dst ("Dst", Float) = 1
		_ZWrite ("ZWrite", Float) = 0
		_ZTest ("ZTest", Float) = 2
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
		[ToggleOff] [HideInInspector] _ReceiveShadows ("Receive Shadows", Float) = 1
	}
	SubShader
	{
		Tags { "QUEUE" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "UniversalMaterialType" = "Unlit" }
		Pass
		{
			Name "Forward"
			Tags { "LIGHTMODE" = "UniversalForwardOnly" }
			Blend [_Src] [_Dst], One OneMinusSrcAlpha
			Cull [_Cull]
			ZTest [_ZTest]
			ZWrite [_ZWrite]
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile __ DIRLIGHTMAP_COMBINED
			#pragma multi_compile __ LIGHTMAP_ON

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[20];
			float3 _WorldSpaceCameraPos;
			float _GlobalsPad1;
			float4 _GlobalsPad2[55];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _CameraOffset : packoffset(c8.w);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD3 : TEXCOORD3, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0, out float4 o_TEXCOORD4 : TEXCOORD4, out float4 o_COLOR0 : COLOR0)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(unity_ObjectToWorld._m01_m11 * i_POSITION0.yy);
				r0.xy = asuint(unity_ObjectToWorld._m00_m10 * i_POSITION0.xx + asfloat(r0.xy));
				r0.xy = asuint(unity_ObjectToWorld._m02_m12 * i_POSITION0.zz + asfloat(r0.xy));
				r0.xy = asuint(unity_ObjectToWorld._m03_m13 + asfloat(r0.xy));
				r0.xy = asuint(-_WorldSpaceCameraPos.xy + asfloat(r0.xy));
				r0.zw = asuint(float2(_CameraOffset, _CameraOffset) + i_TEXCOORD3.xy);
				r0.xy = asuint(asfloat(r0.zw) * asfloat(r0.xy));
				r0.xy = asuint(float2(0.00999999978, 0.0) * asfloat(r0.xy));
				r0.z = 0u;
				r0.xyz = asuint(i_POSITION0.xyz + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0));
				o_SV_POSITION0 = asfloat(r0);
				o_TEXCOORD0 = asfloat(r0);
				o_TEXCOORD4 = i_TEXCOORD0.xyzw;
				o_COLOR0 = i_COLOR0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[13];
			float4 _TimeParameters;
			float4 _GlobalsPad4[1];
			float4 _ProjectionParams;
			float4 _GlobalsPad5[1];
			float4 _ZBufferParams;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _ColorBottom : packoffset(c0);
				float4 _ColorTop : packoffset(c1);
				float2 _NoiseDistortion_Scale : packoffset(c2);
				float2 _GradientColor_Scale : packoffset(c2.z);
				float2 _Mask_Offset : packoffset(c3);
				float2 _Mask_Scale : packoffset(c3.z);
				float2 _NoiseDistortion_Speed : packoffset(c4);
				float2 _GradientColor_Speed : packoffset(c4.z);
				float2 _Noise_01_Speed : packoffset(c5);
				float2 _Noise_01_Scale : packoffset(c5.z);
				float _Mask_Multiply : packoffset(c6);
				float _Mask_Power : packoffset(c6.y);
				float _Noises_Power : packoffset(c6.z);
				float _Noises_Multiply : packoffset(c6.w);
				float _Erosion_Intensity : packoffset(c7);
				float _Erosion_Sub : packoffset(c7.y);
				float _UseColor : packoffset(c7.w);
				float _DistortionMaskIntensity : packoffset(c8);
				float _DistortionAmount : packoffset(c8.y);
				float _WindSpeed : packoffset(c8.z);
				float _DepthFade : packoffset(c10);
				float _EmissiveIntensity : packoffset(c10.y);
			}
			Texture2D<float4> _NoiseDistortion_Texture : register(t0);
			Texture2D<float4> _Mask_Texture : register(t1);
			Texture2D<float4> _ColorGradientMask : register(t2);
			Texture2D<float4> _Noise_01_Texture : register(t3);
			Texture2D<float4> _CameraDepthTexture : register(t4);
			SamplerState sampler_CameraDepthTexture : register(s0);
			SamplerState sampler_Mask_Texture : register(s1);
			SamplerState sampler_NoiseDistortion_Texture : register(s2);
			SamplerState sampler_ColorGradientMask : register(s3);
			SamplerState sampler_Noise_01_Texture : register(s4);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TEXCOORD4 : TEXCOORD4, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				r0.xy = asuint(_Noise_01_Scale.xy * i_TEXCOORD4.xy);
				r0.z = asuint(_WindSpeed * _TimeParameters.x);
				r0.xy = asuint(asfloat(r0.zz) * _Noise_01_Speed.xy + asfloat(r0.xy));
				r1 = asuint(float4(_NoiseDistortion_Scale.x, _NoiseDistortion_Scale.y, _GradientColor_Scale.x, _GradientColor_Scale.y) * i_TEXCOORD4.xyxy);
				r1 = asuint(asfloat(r0.zzzz) * float4(_NoiseDistortion_Speed.x, _NoiseDistortion_Speed.y, _GradientColor_Speed.x, _GradientColor_Speed.y) + asfloat(r1));
				r0.z = asuint(_NoiseDistortion_Texture.Sample(sampler_NoiseDistortion_Texture, asfloat(r1.xy)).x);
				r0.w = asuint(_ColorGradientMask.Sample(sampler_ColorGradientMask, asfloat(r1.zw)).x);
				r0.z = asuint(_DistortionAmount * asfloat(r0.z));
				r0.xy = asuint(asfloat(r0.zz) * float2(0.100000001, 0.100000001) + asfloat(r0.xy));
				r0.z = asuint(0.100000001 * asfloat(r0.z));
				r0.x = asuint(_Noise_01_Texture.Sample(sampler_Noise_01_Texture, asfloat(r0.xy)).x);
				r0.x = asuint(log2(asfloat(r0.x)));
				r0.x = asuint(_Noises_Power * asfloat(r0.x));
				r0.x = asuint(exp2(asfloat(r0.x)));
				r1.xy = asuint(i_TEXCOORD4.xy * _Mask_Scale.xy + _Mask_Offset.xy);
				r0.yz = asuint(asfloat(r0.zz) * float2(_DistortionMaskIntensity, _DistortionMaskIntensity) + asfloat(r1.xy));
				r1.xyz = asuint(_Mask_Texture.Sample(sampler_Mask_Texture, asfloat(r0.yz)).xyz);
				r0.y = asuint(log2(asfloat(r1.x)));
				r1.xyz = asuint(float3(-1.0, -1.0, -1.0) + asfloat(r1.xyz));
				r1.xyz = asuint(float3(_UseColor, _UseColor, _UseColor) * asfloat(r1.xyz) + float3(1.0, 1.0, 1.0));
				r0.y = asuint(_Mask_Power * asfloat(r0.y));
				r0.y = asuint(exp2(asfloat(r0.y)));
				r0.xy = asuint(saturate(float2(_Noises_Multiply, _Mask_Multiply) * asfloat(r0.xy)));
				r0.x = asuint(asfloat(r0.x) + -asfloat(r0.y));
				r0.x = asuint(saturate(_Erosion_Intensity * asfloat(r0.x) + asfloat(r0.y)));
				r0.z = asuint(_Erosion_Sub + i_TEXCOORD4.z);
				r0.y = asuint(asfloat(r0.y) + -asfloat(r0.z));
				r0.x = asuint(saturate(asfloat(r0.y) * asfloat(r0.x)));
				r2.xyz = asuint(_ColorTop.xyz + -_ColorBottom.xyz);
				r0.yzw = asuint(asfloat(r0.www) * asfloat(r2.xyz) + _ColorBottom.xyz);
				r0.yzw = asuint(i_COLOR0.xyz * asfloat(r0.yzw));
				r0.yzw = asuint(asfloat(r1.xyz) * asfloat(r0.yzw));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r0.xxx));
				r1.x = asuint(_ProjectionParams.x * i_TEXCOORD0.y);
				r1.w = asuint(0.5 * asfloat(r1.x));
				r1.xz = asuint(float2(0.5, 0.5) * i_TEXCOORD0.xw);
				r1.xy = asuint(asfloat(r1.xw) + asfloat(r1.zz));
				r1.z = asuint(i_TEXCOORD0.z);
				r1.xyz = asuint(asfloat(r1.xyz) / i_TEXCOORD0.www);
				r1.x = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r1.xy), _GlobalMipBias.x).x);
				r1.y = asuint(_ZBufferParams.z * asfloat(r1.z) + _ZBufferParams.w);
				r1.y = asuint(1.0 / asfloat(r1.y));
				r1.x = asuint(_ZBufferParams.z * asfloat(r1.x) + _ZBufferParams.w);
				r1.x = asuint(1.0 / asfloat(r1.x));
				r1.x = asuint(asfloat(r1.x) + -asfloat(r1.y));
				r1.x = asuint(saturate(asfloat(r1.x) / _DepthFade));
				r0.x = asuint(asfloat(r1.x) * asfloat(r0.x));
				r0.x = asuint(saturate(i_COLOR0.w * asfloat(r0.x)));
				r0.yzw = asuint(asfloat(r0.yzw) * asfloat(r0.xxx));
				o_SV_Target0.w = asfloat(r0.x);
				r0.xyz = asuint(float3(_EmissiveIntensity, _EmissiveIntensity, _EmissiveIntensity) * asfloat(r0.yzw));
				o_SV_Target0.xyz = i_TEXCOORD4.www * asfloat(r0.xyz);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "DepthOnly"
			Tags { "LIGHTMODE" = "DepthOnly" }
			ColorMask 0
			Cull [_Cull]
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[20];
			float3 _WorldSpaceCameraPos;
			float _GlobalsPad1;
			float4 _GlobalsPad2[55];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _CameraOffset : packoffset(c8.w);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD3 : TEXCOORD3, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0, out float4 o_COLOR0 : COLOR0, out float4 o_TEXCOORD3 : TEXCOORD3)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(unity_ObjectToWorld._m01_m11 * i_POSITION0.yy);
				r0.xy = asuint(unity_ObjectToWorld._m00_m10 * i_POSITION0.xx + asfloat(r0.xy));
				r0.xy = asuint(unity_ObjectToWorld._m02_m12 * i_POSITION0.zz + asfloat(r0.xy));
				r0.xy = asuint(unity_ObjectToWorld._m03_m13 + asfloat(r0.xy));
				r0.xy = asuint(-_WorldSpaceCameraPos.xy + asfloat(r0.xy));
				r0.zw = asuint(float2(_CameraOffset, _CameraOffset) + i_TEXCOORD3.xy);
				r0.xy = asuint(asfloat(r0.zw) * asfloat(r0.xy));
				r0.xy = asuint(float2(0.00999999978, 0.0) * asfloat(r0.xy));
				r0.z = 0u;
				r0.xyz = asuint(i_POSITION0.xyz + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0));
				o_SV_POSITION0 = asfloat(r0);
				o_TEXCOORD0 = asfloat(r0);
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD3 = i_TEXCOORD0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD3 : TEXCOORD3, out float4 o_SV_TARGET0 : SV_TARGET0)
			{
				o_SV_TARGET0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "DepthNormals"
			Tags { "LIGHTMODE" = "DepthNormalsOnly" }
			Cull [_Cull]
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[20];
			float3 _WorldSpaceCameraPos;
			float _GlobalsPad1;
			float4 _GlobalsPad2[55];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _CameraOffset : packoffset(c8.w);
			}

			void vert(float4 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TEXCOORD3 : TEXCOORD3, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD0 : TEXCOORD0, out float3 o_TEXCOORD1 : TEXCOORD1, out float4 o_COLOR0 : COLOR0, out float4 o_TEXCOORD2 : TEXCOORD2)
			{
				uint4 r0;
				uint4 r1;
				r0.xy = asuint(unity_ObjectToWorld._m01_m11 * i_POSITION0.yy);
				r0.xy = asuint(unity_ObjectToWorld._m00_m10 * i_POSITION0.xx + asfloat(r0.xy));
				r0.xy = asuint(unity_ObjectToWorld._m02_m12 * i_POSITION0.zz + asfloat(r0.xy));
				r0.xy = asuint(unity_ObjectToWorld._m03_m13 + asfloat(r0.xy));
				r0.xy = asuint(-_WorldSpaceCameraPos.xy + asfloat(r0.xy));
				r0.zw = asuint(float2(_CameraOffset, _CameraOffset) + i_TEXCOORD3.xy);
				r0.xy = asuint(asfloat(r0.zw) * asfloat(r0.xy));
				r0.xy = asuint(float2(0.00999999978, 0.0) * asfloat(r0.xy));
				r0.z = 0u;
				r0.xyz = asuint(i_POSITION0.xyz + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0));
				o_SV_POSITION0 = asfloat(r0);
				o_TEXCOORD0 = asfloat(r0);
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_TEXCOORD1 = asfloat(r0.xyz) * asfloat(r0.www);
				o_COLOR0 = i_COLOR0.xyzw;
				o_TEXCOORD2 = i_TEXCOORD0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD0 : TEXCOORD0, float3 i_TEXCOORD1 : TEXCOORD1, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD2 : TEXCOORD2, out float4 o_SV_TARGET0 : SV_TARGET0)
			{
				uint4 r0;
				r0.x = asuint(dot(i_TEXCOORD1.xyz, i_TEXCOORD1.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				o_SV_TARGET0.xyz = i_TEXCOORD1.xyz * asfloat(r0.xxx);
				o_SV_TARGET0.w = 0.0;
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
}
