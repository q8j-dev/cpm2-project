Shader "Shader Graphs/BillBoardFlare"
{
	Properties
	{
		[NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
		_FlarePower ("FlarePower", Range(0, 1)) = 1
		[HDR] _Color ("Color", Color) = (1,1,1,0)
		_FadeDistance ("FadeDistance", Float) = 1
		[Toggle(_ANGLEFADE)] _ANGLEFADE ("AngleFade", Float) = 0
		_Angle_Spread_Min ("Angle Spread Min", Range(0, 1)) = 0
		_Angle_Spread_Max ("Angle Spread Max", Range(0, 1)) = 1
		_Depth_Offset ("Depth Offset", Float) = 0
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	SubShader
	{
		Tags { "DisableBatching" = "False" "QUEUE" = "Transparent" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Transparent" "ShaderGraphShader" = "true" "ShaderGraphTargetId" = "UniversalUnlitSubTarget" "UniversalMaterialType" = "Unlit" }
		Pass
		{
			Name "Universal Forward"
			Blend SrcAlpha One, One One
			Cull Back
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ FOG_EXP
			#pragma multi_compile __ _ANGLEFADE
			#pragma multi_compile __ _QUALITY_POTATO

			#if defined(SHADER_STAGE_VERTEX)
			#if (defined(FOG_EXP) && defined(_ANGLEFADE)) || defined(_ANGLEFADE)
			float4 _GlobalsPad0[64];
			float4x4 unity_MatrixV;
			float4x4 unity_MatrixInvV;
			float4 _GlobalsPad1[4];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _FadeDistance : packoffset(c2);
				float _Angle_Spread_Max : packoffset(c2.y);
				float _Angle_Spread_Min : packoffset(c2.z);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1, out float3 o_INTERP2 : INTERP2)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(dot(unity_ObjectToWorld._m00_m10_m20, unity_ObjectToWorld._m00_m10_m20));
				r0.w = asuint(dot(unity_ObjectToWorld._m01_m11_m21, unity_ObjectToWorld._m01_m11_m21));
				r0.xy = asuint(sqrt(asfloat(r0.xw)));
				r0.w = asuint(dot(unity_ObjectToWorld._m02_m12_m22, unity_ObjectToWorld._m02_m12_m22));
				r0.z = asuint(sqrt(asfloat(r0.w)));
				r0.xyz = asuint(i_POSITION0.xyz * asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_MatrixInvV._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_WorldToObject._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_WorldToObject._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_INTERP1.xyz = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				r0.xyz = asuint(unity_WorldToObject._m01_m11_m21 * unity_MatrixInvV._m12_m12_m12);
				r0.xyz = asuint(unity_WorldToObject._m00_m10_m20 * unity_MatrixInvV._m02_m02_m02 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * unity_MatrixInvV._m22_m22_m22 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 * unity_MatrixInvV._m32_m32_m32 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r1.xyz = asuint(unity_WorldToObject._m01_m11_m21 * -asfloat(r0.yyy));
				r0.xyw = asuint(asfloat(r1.xyz) - unity_WorldToObject._m00_m10_m20 * asfloat(r0.xxx));
				r0.xyz = asuint(asfloat(r0.xyw) - unity_WorldToObject._m02_m12_m22 * asfloat(r0.zzz));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				r0.x = asuint(dot(asfloat(r0.xyz), i_NORMAL0.xyz));
				r0.x = asuint(-_Angle_Spread_Min + asfloat(r0.x));
				r0.y = asuint(_Angle_Spread_Max + -_Angle_Spread_Min);
				r0.x = asuint(saturate(asfloat(r0.x) / asfloat(r0.y)));
				r0.y = asuint(unity_ObjectToWorld._m13 * unity_MatrixV._m21);
				r0.y = asuint(unity_MatrixV._m20 * unity_ObjectToWorld._m03 + asfloat(r0.y));
				r0.y = asuint(unity_MatrixV._m22 * unity_ObjectToWorld._m23 + asfloat(r0.y));
				r0.y = asuint(unity_MatrixV._m23 + asfloat(r0.y));
				r0.y = asuint(saturate(-asfloat(r0.y) / _FadeDistance));
				o_INTERP1.w = asfloat(r0.x) * asfloat(r0.y);
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP2 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#elif defined(FOG_EXP) || 1
			float4 _GlobalsPad0[64];
			float4x4 unity_MatrixV;
			float4x4 unity_MatrixInvV;
			float4 _GlobalsPad1[4];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _FadeDistance : packoffset(c2);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1, out float3 o_INTERP2 : INTERP2)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(dot(unity_ObjectToWorld._m00_m10_m20, unity_ObjectToWorld._m00_m10_m20));
				r0.w = asuint(dot(unity_ObjectToWorld._m01_m11_m21, unity_ObjectToWorld._m01_m11_m21));
				r0.xy = asuint(sqrt(asfloat(r0.xw)));
				r0.w = asuint(dot(unity_ObjectToWorld._m02_m12_m22, unity_ObjectToWorld._m02_m12_m22));
				r0.z = asuint(sqrt(asfloat(r0.w)));
				r0.xyz = asuint(i_POSITION0.xyz * asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_MatrixInvV._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_WorldToObject._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_WorldToObject._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_INTERP1.xyz = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				r0.x = asuint(unity_ObjectToWorld._m13 * unity_MatrixV._m21);
				r0.x = asuint(unity_MatrixV._m20 * unity_ObjectToWorld._m03 + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m22 * unity_ObjectToWorld._m23 + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 + asfloat(r0.x));
				o_INTERP1.w = saturate(-asfloat(r0.x) / _FadeDistance);
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP2 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if (defined(FOG_EXP) && defined(_ANGLEFADE) && defined(_QUALITY_POTATO)) || (defined(_ANGLEFADE) && defined(_QUALITY_POTATO)) || (defined(FOG_EXP) && defined(_QUALITY_POTATO)) || defined(_QUALITY_POTATO)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_Target0 : SV_Target0)
			{
				if (0xffffffffu != 0) discard;
				o_SV_Target0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#elif (defined(FOG_EXP) && defined(_ANGLEFADE)) || defined(_ANGLEFADE) || defined(FOG_EXP) || 1
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _AlphaToMaskAvailable;
			float _GlobalsPad1;
			float4 _GlobalsPad2[17];
			float4 _ZBufferParams;
			float4 unity_OrthoParams;
			float4 _GlobalsPad3[39];
			float4x4 unity_MatrixV;
			float4 _GlobalsPad4[8];
			float4x4 unity_MatrixVP;
			float4x4 unity_MatrixInvVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float4 _Color : packoffset(c1);
				float _Depth_Offset : packoffset(c2.w);
				float _FlarePower : packoffset(c3);
			}
			Texture2D<float4> _CameraDepthTexture : register(t0);
			Texture2D<float4> _MainTex : register(t1);
			SamplerState sampler_CameraDepthTexture : register(s0);
			SamplerState sampler_MainTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0 = asuint(float4(_FlarePower, _FlarePower, _FlarePower, _FlarePower) * _Color.xyzw);
				r1 = asuint(_MainTex.SampleBias(sampler_MainTex, i_INTERP0.xy, _GlobalMipBias.x));
				r0 = asuint(asfloat(r1) * asfloat(r0));
				o_SV_Target0.xyz = asfloat(r0.xyz) * asfloat(r0.www);
				r0.xyz = asuint(unity_ObjectToWorld._m13_m13_m13 * unity_MatrixVP._m01_m11_m31);
				r0.xyz = asuint(unity_MatrixVP._m00_m10_m30 * unity_ObjectToWorld._m03_m03_m03 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixVP._m02_m12_m32 * unity_ObjectToWorld._m23_m23_m23 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixVP._m03_m13_m33 + asfloat(r0.xyz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				r0.xy = asuint(asfloat(r0.xy) * float2(0.5, -0.5) + float2(0.5, 0.5));
				b0.z = (1.0 == unity_OrthoParams.w);
				[branch] if (b0.z)
				{
					r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r0.xy), _GlobalMipBias.x));
					r1.yz = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0));
					r2 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r1.zzzz));
					r2 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r1.yyyy) + asfloat(r2));
					r1 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r2));
					r1 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r1));
					r1.xyz = asuint(asfloat(r1.xyz) / asfloat(r1.www));
					r0.z = asuint(unity_MatrixV._m21 * asfloat(r1.y));
					r0.z = asuint(unity_MatrixV._m20 * asfloat(r1.x) + asfloat(r0.z));
					r0.z = asuint(unity_MatrixV._m22 * asfloat(r1.z) + asfloat(r0.z));
					r0.z = asuint(unity_MatrixV._m23 + asfloat(r0.z));
					r0.z = asuint(abs(asfloat(r0.z)));
				}
				else
				{
					r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r0.xy), _GlobalMipBias.x));
					r0.x = asuint(_ZBufferParams.z * asfloat(r1.x) + _ZBufferParams.w);
					r0.z = asuint(1.0 / asfloat(r0.x));
				}
				r0.x = asuint(unity_ObjectToWorld._m13 * unity_MatrixV._m21);
				r0.x = asuint(unity_MatrixV._m20 * unity_ObjectToWorld._m03 + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m22 * unity_ObjectToWorld._m23 + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 + asfloat(r0.x));
				r0.x = asuint(_Depth_Offset + -asfloat(r0.x));
				r0.x = asuint(asfloat(r0.z) + -asfloat(r0.x));
				r0.x = asuint(saturate(ceil(asfloat(r0.x))));
				r0.x = asuint(i_INTERP1.w * asfloat(r0.x));
				r0.y = asuint(asfloat(r0.x) * asfloat(r0.w));
				b0.z = (asfloat(r0.y) >= 0.000199999995);
				r0.z = b0.z ? r0.y : 0u;
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.w) + -0.000199999995);
				r0.w = asuint(ddx(asfloat(r0.y)));
				r0.y = asuint(ddy(asfloat(r0.y)));
				r0.y = asuint(abs(asfloat(r0.w)) + abs(asfloat(r0.y)));
				r0.y = asuint(max(9.99999975e-05, asfloat(r0.y)));
				r0.x = asuint(asfloat(r0.x) / asfloat(r0.y));
				r0.x = asuint(saturate(0.5 + asfloat(r0.x)));
				b0.y = (0.0 != _AlphaToMaskAvailable);
				r0.x = b0.y ? r0.x : r0.z;
				r0.y = asuint(-9.99999975e-05 + asfloat(r0.x));
				b0.y = (asfloat(r0.y) < 0.0);
				if (b0.y) discard;
				o_SV_Target0.w = asfloat(r0.x);
			}
			#endif
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "DepthNormalsOnly"
			Tags { "LIGHTMODE" = "DepthNormalsOnly" }
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ _ANGLEFADE
			#pragma multi_compile __ _QUALITY_POTATO

			#if defined(SHADER_STAGE_VERTEX)
			#if defined(_ANGLEFADE)
			float4 _GlobalsPad0[64];
			float4x4 unity_MatrixV;
			float4x4 unity_MatrixInvV;
			float4 _GlobalsPad1[4];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _FadeDistance : packoffset(c2);
				float _Angle_Spread_Max : packoffset(c2.y);
				float _Angle_Spread_Min : packoffset(c2.z);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(dot(unity_ObjectToWorld._m00_m10_m20, unity_ObjectToWorld._m00_m10_m20));
				r0.w = asuint(dot(unity_ObjectToWorld._m01_m11_m21, unity_ObjectToWorld._m01_m11_m21));
				r0.xy = asuint(sqrt(asfloat(r0.xw)));
				r0.w = asuint(dot(unity_ObjectToWorld._m02_m12_m22, unity_ObjectToWorld._m02_m12_m22));
				r0.z = asuint(sqrt(asfloat(r0.w)));
				r0.xyz = asuint(i_POSITION0.xyz * asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_MatrixInvV._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_WorldToObject._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_WorldToObject._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				r0.xyz = asuint(unity_WorldToObject._m01_m11_m21 * unity_MatrixInvV._m12_m12_m12);
				r0.xyz = asuint(unity_WorldToObject._m00_m10_m20 * unity_MatrixInvV._m02_m02_m02 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * unity_MatrixInvV._m22_m22_m22 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 * unity_MatrixInvV._m32_m32_m32 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r1.xyz = asuint(unity_WorldToObject._m01_m11_m21 * -asfloat(r0.yyy));
				r0.xyw = asuint(asfloat(r1.xyz) - unity_WorldToObject._m00_m10_m20 * asfloat(r0.xxx));
				r0.xyz = asuint(asfloat(r0.xyw) - unity_WorldToObject._m02_m12_m22 * asfloat(r0.zzz));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r0.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				r0.x = asuint(dot(asfloat(r0.xyz), i_NORMAL0.xyz));
				r0.x = asuint(-_Angle_Spread_Min + asfloat(r0.x));
				r0.y = asuint(_Angle_Spread_Max + -_Angle_Spread_Min);
				r0.x = asuint(saturate(asfloat(r0.x) / asfloat(r0.y)));
				r0.y = asuint(unity_ObjectToWorld._m13 * unity_MatrixV._m21);
				r0.y = asuint(unity_MatrixV._m20 * unity_ObjectToWorld._m03 + asfloat(r0.y));
				r0.y = asuint(unity_MatrixV._m22 * unity_ObjectToWorld._m23 + asfloat(r0.y));
				r0.y = asuint(unity_MatrixV._m23 + asfloat(r0.y));
				r0.y = asuint(saturate(-asfloat(r0.y) / _FadeDistance));
				o_INTERP1.w = asfloat(r0.x) * asfloat(r0.y);
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP1.xyz = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#elif 1
			float4 _GlobalsPad0[64];
			float4x4 unity_MatrixV;
			float4x4 unity_MatrixInvV;
			float4 _GlobalsPad1[4];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float _FadeDistance : packoffset(c2);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(dot(unity_ObjectToWorld._m00_m10_m20, unity_ObjectToWorld._m00_m10_m20));
				r0.w = asuint(dot(unity_ObjectToWorld._m01_m11_m21, unity_ObjectToWorld._m01_m11_m21));
				r0.xy = asuint(sqrt(asfloat(r0.xw)));
				r0.w = asuint(dot(unity_ObjectToWorld._m02_m12_m22, unity_ObjectToWorld._m02_m12_m22));
				r0.z = asuint(sqrt(asfloat(r0.w)));
				r0.xyz = asuint(i_POSITION0.xyz * asfloat(r0.xyz));
				r1.xyz = asuint(unity_MatrixInvV._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_MatrixInvV._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_MatrixInvV._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_WorldToObject._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_WorldToObject._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_WorldToObject._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_WorldToObject._m03_m13_m23 + asfloat(r0.xyz));
				r1.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * asfloat(r0.yyy));
				r0.xyw = asuint(unity_ObjectToWorld._m00_m10_m20 * asfloat(r0.xxx) + asfloat(r1.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * asfloat(r0.zzz) + asfloat(r0.xyw));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP1.xyz = asfloat(r0.xyz) * asfloat(r0.www);
				r0.x = asuint(unity_ObjectToWorld._m13 * unity_MatrixV._m21);
				r0.x = asuint(unity_MatrixV._m20 * unity_ObjectToWorld._m03 + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m22 * unity_ObjectToWorld._m23 + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 + asfloat(r0.x));
				o_INTERP1.w = saturate(-asfloat(r0.x) / _FadeDistance);
			}
			#endif
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			#if (defined(_ANGLEFADE) && defined(_QUALITY_POTATO)) || defined(_QUALITY_POTATO)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				if (0xffffffffu != 0) discard;
				r0.x = asuint(dot(i_INTERP1.xyz, i_INTERP1.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				o_SV_Target0.xyz = i_INTERP1.xyz * asfloat(r0.xxx);
				o_SV_Target0.w = 0.0;
			}
			#elif defined(_ANGLEFADE) || 1
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[17];
			float4 _ZBufferParams;
			float4 unity_OrthoParams;
			float4 _GlobalsPad4[39];
			float4x4 unity_MatrixV;
			float4 _GlobalsPad5[8];
			float4x4 unity_MatrixVP;
			float4x4 unity_MatrixInvVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float4 _Color : packoffset(c1);
				float _Depth_Offset : packoffset(c2.w);
				float _FlarePower : packoffset(c3);
			}
			Texture2D<float4> _CameraDepthTexture : register(t0);
			Texture2D<float4> _MainTex : register(t1);
			SamplerState sampler_CameraDepthTexture : register(s0);
			SamplerState sampler_MainTex : register(s1);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				r0.xyz = asuint(unity_ObjectToWorld._m13_m13_m13 * unity_MatrixVP._m01_m11_m31);
				r0.xyz = asuint(unity_MatrixVP._m00_m10_m30 * unity_ObjectToWorld._m03_m03_m03 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixVP._m02_m12_m32 * unity_ObjectToWorld._m23_m23_m23 + asfloat(r0.xyz));
				r0.xyz = asuint(unity_MatrixVP._m03_m13_m33 + asfloat(r0.xyz));
				r0.xy = asuint(asfloat(r0.xy) / asfloat(r0.zz));
				r0.xy = asuint(asfloat(r0.xy) * float2(0.5, -0.5) + float2(0.5, 0.5));
				b0.z = (1.0 == unity_OrthoParams.w);
				[branch] if (b0.z)
				{
					r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r0.xy), _GlobalMipBias.x));
					r0.zw = asuint(asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0));
					r2 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r0.wwww));
					r2 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r0.zzzz) + asfloat(r2));
					r1 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r2));
					r1 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r1));
					r1.xyz = asuint(asfloat(r1.xyz) / asfloat(r1.www));
					r0.z = asuint(unity_MatrixV._m21 * asfloat(r1.y));
					r0.z = asuint(unity_MatrixV._m20 * asfloat(r1.x) + asfloat(r0.z));
					r0.z = asuint(unity_MatrixV._m22 * asfloat(r1.z) + asfloat(r0.z));
					r0.z = asuint(unity_MatrixV._m23 + asfloat(r0.z));
					r0.z = asuint(abs(asfloat(r0.z)));
				}
				else
				{
					r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r0.xy), _GlobalMipBias.x));
					r0.x = asuint(_ZBufferParams.z * asfloat(r1.x) + _ZBufferParams.w);
					r0.z = asuint(1.0 / asfloat(r0.x));
				}
				r0.x = asuint(unity_ObjectToWorld._m13 * unity_MatrixV._m21);
				r0.x = asuint(unity_MatrixV._m20 * unity_ObjectToWorld._m03 + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m22 * unity_ObjectToWorld._m23 + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 + asfloat(r0.x));
				r0.x = asuint(_Depth_Offset + -asfloat(r0.x));
				r0.x = asuint(asfloat(r0.z) + -asfloat(r0.x));
				r0.x = asuint(saturate(ceil(asfloat(r0.x))));
				r0.x = asuint(i_INTERP1.w * asfloat(r0.x));
				r0.y = asuint(_FlarePower * _Color.w);
				r1 = asuint(_MainTex.SampleBias(sampler_MainTex, i_INTERP0.xy, _GlobalMipBias.x));
				r0.y = asuint(asfloat(r1.w) * asfloat(r0.y));
				r0.x = asuint(asfloat(r0.x) * asfloat(r0.y) + -0.000199999995);
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				r0.x = asuint(dot(i_INTERP1.xyz, i_INTERP1.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				o_SV_Target0.xyz = i_INTERP1.xyz * asfloat(r0.xxx);
				o_SV_Target0.w = 0.0;
			}
			#endif
			#endif
			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}
