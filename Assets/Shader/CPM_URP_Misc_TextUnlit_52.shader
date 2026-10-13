Shader "CPM/URP/Misc/TextUnlit"
{
	Properties
	{
		[NoScaleOffset] _MainTex ("MainTex", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		[HideInInspector] _QueueOffset ("_QueueOffset", Float) = 0
		[HideInInspector] _QueueControl ("_QueueControl", Float) = -1
		[HideInInspector] [NoScaleOffset] unity_Lightmaps ("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_LightmapsInd ("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector] [NoScaleOffset] unity_ShadowMasks ("unity_ShadowMasks", 2DArray) = "" {}
	}
	SubShader
	{
		Tags { "DisableBatching" = "False" "QUEUE" = "AlphaTest" "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "ShaderGraphShader" = "true" "ShaderGraphTargetId" = "UniversalUnlitSubTarget" "UniversalMaterialType" = "Unlit" }
		Pass
		{
			Name "Universal Forward"
			Cull Back
			ZTest LEqual
			ZWrite On
			AlphaToMask 1
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ DIRLIGHTMAP_COMBINED
			#pragma multi_compile __ FOG_EXP
			#pragma multi_compile __ LIGHTMAP_ON

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1, out float3 o_INTERP2 : INTERP2, out float3 o_INTERP3 : INTERP3)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_INTERP2 = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				o_INTERP1 = i_COLOR0.xyzw;
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP3 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			float _AlphaToMaskAvailable;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _Color : packoffset(c1);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, float3 i_INTERP3 : INTERP3, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0 = asuint(_MainTex.SampleBias(sampler_MainTex, i_INTERP0.xy, _GlobalMipBias.x));
				r1 = asuint(_Color.xyzw * i_INTERP1.xyzw);
				r0.x = asuint(asfloat(r1.w) * asfloat(r0.w));
				r0.y = asuint(asfloat(r1.w) * asfloat(r0.w) + -0.5);
				o_SV_Target0.xyz = asfloat(r1.xyz);
				r0.z = asuint(ddx(asfloat(r0.x)));
				r0.w = asuint(ddy(asfloat(r0.x)));
				r0.z = asuint(abs(asfloat(r0.z)) + abs(asfloat(r0.w)));
				r0.z = asuint(max(9.99999975e-05, asfloat(r0.z)));
				r0.y = asuint(asfloat(r0.y) / asfloat(r0.z));
				r0.y = asuint(saturate(0.5 + asfloat(r0.y)));
				b0.z = (asfloat(r0.x) >= 0.5);
				r0.x = b0.z ? r0.x : 0u;
				b0.z = (0.0 != _AlphaToMaskAvailable);
				r0.x = b0.z ? r0.y : r0.x;
				o_SV_Target0.w = b0.z ? asfloat(r0.y) : 1.0;
				r0.x = asuint(-9.99999975e-05 + asfloat(r0.x));
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "DepthOnly"
			Tags { "LIGHTMODE" = "DepthOnly" }
			ColorMask 8
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1)
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
				o_INTERP0 = i_TEXCOORD0.xyzw;
				o_INTERP1 = i_COLOR0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _Color : packoffset(c1);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, out float4 o_SV_TARGET0 : SV_TARGET0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.x = asuint(_Color.w * i_INTERP1.w);
				r1 = asuint(_MainTex.SampleBias(sampler_MainTex, i_INTERP0.xy, _GlobalMipBias.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r1.w) + -0.5);
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				o_SV_TARGET0 = i_SV_POSITION0.zzzz;
			}
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

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1, out float3 o_INTERP2 : INTERP2)
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
				o_INTERP0 = i_TEXCOORD0.xyzw;
				o_INTERP1 = i_COLOR0.xyzw;
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP2 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _Color : packoffset(c1);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.x = asuint(_Color.w * i_INTERP1.w);
				r1 = asuint(_MainTex.SampleBias(sampler_MainTex, i_INTERP0.xy, _GlobalMipBias.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r1.w) + -0.5);
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				r0.x = asuint(dot(i_INTERP2.xyz, i_INTERP2.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				o_SV_Target0.xyz = i_INTERP2.xyz * asfloat(r0.xxx);
				o_SV_Target0.w = 0.0;
			}
			#endif
			ENDHLSL
		}
		Pass
		{
			Name "ShadowCaster"
			Tags { "LIGHTMODE" = "SHADOWCASTER" }
			ColorMask 0
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			float4 _GlobalsPad1[47];
			float4 _ShadowBias;
			float4 _GlobalsPad2[2];
			float3 _LightDirection;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_COLOR0 : COLOR0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_INTERP0 : INTERP0, out float4 o_INTERP1 : INTERP1, out float3 o_INTERP2 : INTERP2)
			{
				uint4 r0;
				uint4 r1;
				r0.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz));
				r0.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r0.xyz));
				r0.xyz = asuint(_LightDirection.xyz * _ShadowBias.xxx + asfloat(r0.xyz));
				r1.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r1.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r1.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www));
				r0.w = asuint(saturate(dot(_LightDirection.xyz, asfloat(r1.xyz))));
				r0.w = asuint(1.0 + -asfloat(r0.w));
				r0.w = asuint(_ShadowBias.y * asfloat(r0.w));
				r0.xyz = asuint(asfloat(r1.xyz) * asfloat(r0.www) + asfloat(r0.xyz));
				o_INTERP2 = asfloat(r1.xyz);
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				r0 = asuint(unity_MatrixVP._m03_m13_m23_m33 + asfloat(r0));
				o_SV_POSITION0.z = min(1.0, asfloat(r0.z));
				o_SV_POSITION0.xyw = asfloat(r0.xyw);
				o_INTERP0 = i_TEXCOORD0.xyzw;
				o_INTERP1 = i_COLOR0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			cbuffer UnityPerMaterial : register(b1)
			{
				float4 _Color : packoffset(c1);
			}
			Texture2D<float4> _MainTex : register(t0);
			SamplerState sampler_MainTex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_INTERP0 : INTERP0, float4 i_INTERP1 : INTERP1, float3 i_INTERP2 : INTERP2, out float4 o_SV_TARGET0 : SV_TARGET0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.x = asuint(_Color.w * i_INTERP1.w);
				r1 = asuint(_MainTex.SampleBias(sampler_MainTex, i_INTERP0.xy, _GlobalMipBias.x));
				r0.x = asuint(asfloat(r0.x) * asfloat(r1.w) + -0.5);
				b0.x = (asfloat(r0.x) < 0.0);
				if (b0.x) discard;
				o_SV_TARGET0 = float4(0.0, 0.0, 0.0, 0.0);
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}
