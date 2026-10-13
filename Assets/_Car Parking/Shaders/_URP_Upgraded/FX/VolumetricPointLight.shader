Shader "Shader Graphs/VolumetricPointLight"
{
	Properties
	{
		[HDR] [MainColor] _Color ("Color", Color) = (1,1,1,0)
		_Pow ("Pow", Float) = 1
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
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ FOG_EXP

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[76];
			float4x4 unity_MatrixVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
			}

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_INTERP0 : INTERP0, out float3 o_INTERP1 : INTERP1)
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
				o_INTERP0 = asfloat(r0.xyz);
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 + asfloat(r1);
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP1 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[4];
			float4 _ScaledScreenParams;
			float2 _GlobalMipBias;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float4 _GlobalsPad3[14];
			float3 _WorldSpaceCameraPos;
			float _GlobalsPad4;
			float4 _ProjectionParams;
			float4 _GlobalsPad5[1];
			float4 _ZBufferParams;
			float4 unity_OrthoParams;
			float4 _GlobalsPad6[39];
			float4x4 unity_MatrixV;
			float4x4 unity_MatrixInvV;
			float4 _GlobalsPad7[8];
			float4x4 unity_MatrixInvVP;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerMaterial : register(b2)
			{
				float4 _Color : packoffset(c0);
			}
			Texture2D<float4> _CameraDepthTexture : register(t0);
			SamplerState sampler_CameraDepthTexture : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_INTERP0 : INTERP0, float3 i_INTERP1 : INTERP1, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				bool4 b0;
				r0.x = asuint(dot(i_INTERP1.xyz, i_INTERP1.xyz));
				r0.x = asuint(sqrt(asfloat(r0.x)));
				r0.x = asuint(1.0 / asfloat(r0.x));
				r0.xyz = asuint(i_INTERP1.xyz * asfloat(r0.xxx));
				r1.x = asuint(dot(asfloat(r0.xyz), unity_ObjectToWorld._m00_m10_m20));
				r1.y = asuint(dot(asfloat(r0.xyz), unity_ObjectToWorld._m01_m11_m21));
				r1.z = asuint(dot(asfloat(r0.xyz), unity_ObjectToWorld._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				r2.x = asuint(dot(asfloat(r0.xyz), unity_MatrixInvV._m00_m10_m20));
				r2.y = asuint(dot(asfloat(r0.xyz), unity_MatrixInvV._m01_m11_m21));
				r2.z = asuint(dot(asfloat(r0.xyz), unity_MatrixInvV._m02_m12_m22));
				b0.xy = (float2(0.0, 1.0) == unity_OrthoParams.ww);
				r1.xzw = asuint(_WorldSpaceCameraPos.xyz + -i_INTERP0.xyz);
				r0.z = asuint(dot(asfloat(r1.xzw), asfloat(r1.xzw)));
				r0.z = asuint(rsqrt(asfloat(r0.z)));
				r1.xzw = asuint(asfloat(r1.xzw) * asfloat(r0.zzz));
				r3.x = asuint(unity_MatrixV._m20);
				r3.y = asuint(unity_MatrixV._m21);
				r3.z = asuint(unity_MatrixV._m22);
				r1.xzw = b0.xxx ? r1.xzw : r3.xyz;
				r3.xyz = asuint(unity_MatrixV._m01_m11_m21 * asfloat(r1.zzz));
				r3.xyz = asuint(unity_MatrixV._m00_m10_m20 * asfloat(r1.xxx) + asfloat(r3.xyz));
				r1.xzw = asuint(unity_MatrixV._m02_m12_m22 * asfloat(r1.www) + asfloat(r3.xyz));
				r0.x = asuint(unity_MatrixV._m21 * i_INTERP0.y);
				r0.x = asuint(unity_MatrixV._m20 * i_INTERP0.x + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m22 * i_INTERP0.z + asfloat(r0.x));
				r0.x = asuint(unity_MatrixV._m23 + asfloat(r0.x));
				b0.z = (_ProjectionParams.x < 0.0);
				r2.w = asuint(_ScaledScreenParams.y + -i_SV_POSITION0.y);
				r3.y = b0.z ? r2.w : asuint(i_SV_POSITION0.y);
				r3.x = asuint(i_SV_POSITION0.x);
				r3.xy = asuint(asfloat(r3.xy) / _ScaledScreenParams.xy);
				r4.xyz = asuint(_Color.xyz * _Color.www);
				r0.z = asuint(asfloat(r1.y) * asfloat(r0.w) + 0.300000012);
				r0.z = asuint(saturate(3.0 * asfloat(r0.z)));
				r0.w = asuint(saturate(dot(asfloat(r1.xzw), asfloat(r2.xyz))));
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				r0.w = asuint(asfloat(r0.w) * asfloat(r0.w));
				[branch] if (b0.y)
				{
					r3.z = asuint(1.0 + -asfloat(r3.y));
					r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r3.xz), _GlobalMipBias.x));
					r1.yz = asuint(asfloat(r3.xz) * float2(2.0, 2.0) + float2(-1.0, -1.0));
					r2 = asuint(unity_MatrixInvVP._m01_m11_m21_m31 * -asfloat(r1.zzzz));
					r2 = asuint(unity_MatrixInvVP._m00_m10_m20_m30 * asfloat(r1.yyyy) + asfloat(r2));
					r1 = asuint(unity_MatrixInvVP._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r2));
					r1 = asuint(unity_MatrixInvVP._m03_m13_m23_m33 + asfloat(r1));
					r1.xyz = asuint(asfloat(r1.xyz) / asfloat(r1.www));
					r0.y = asuint(unity_MatrixV._m21 * asfloat(r1.y));
					r0.y = asuint(unity_MatrixV._m20 * asfloat(r1.x) + asfloat(r0.y));
					r0.y = asuint(unity_MatrixV._m22 * asfloat(r1.z) + asfloat(r0.y));
					r0.y = asuint(unity_MatrixV._m23 + asfloat(r0.y));
					r0.y = asuint(abs(asfloat(r0.y)));
				}
				else
				{
					r3.w = asuint(1.0 + -asfloat(r3.y));
					r1 = asuint(_CameraDepthTexture.SampleBias(sampler_CameraDepthTexture, asfloat(r3.xw), _GlobalMipBias.x));
					r1.x = asuint(_ZBufferParams.z * asfloat(r1.x) + _ZBufferParams.w);
					r0.y = asuint(1.0 / asfloat(r1.x));
				}
				r0.y = asuint(saturate(asfloat(r0.y) + asfloat(r0.x)));
				r0.y = asuint(asfloat(r0.w) * asfloat(r0.y));
				r0.x = asuint(-_ProjectionParams.y + -asfloat(r0.x));
				r0.x = asuint(saturate(asfloat(r0.x) + asfloat(r0.x)));
				r0.x = asuint(asfloat(r0.y) * asfloat(r0.x));
				r0.x = asuint(asfloat(r0.z) * asfloat(r0.x));
				o_SV_Target0.xyz = asfloat(r4.xyz) * asfloat(r0.xxx);
				o_SV_Target0.w = 1.0;
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

			void vert(float3 i_POSITION0 : POSITION0, float3 i_NORMAL0 : NORMAL0, float4 i_TANGENT0 : TANGENT0, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_INTERP0 : INTERP0)
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
				r0.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r0.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r0.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r0.w = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.w = asuint(max(1.17549435e-38, asfloat(r0.w)));
				r0.w = asuint(rsqrt(asfloat(r0.w)));
				o_INTERP0 = asfloat(r0.xyz) * asfloat(r0.www);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_INTERP0 : INTERP0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				r0.x = asuint(dot(i_INTERP0.xyz, i_INTERP0.xyz));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				o_SV_Target0.xyz = i_INTERP0.xyz * asfloat(r0.xxx);
				o_SV_Target0.w = 0.0;
			}
			#endif
			ENDHLSL
		}
	}
	Fallback "Hidden/Shader Graph/FallbackError"
	CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}
