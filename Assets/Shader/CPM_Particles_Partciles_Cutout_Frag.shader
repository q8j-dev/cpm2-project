Shader "CPM/Particles/Partciles_Cutout_Frag"
{
	Properties
	{
		_Albedo ("Albedo", 2D) = "white" {}
		[HideInInspector] _texcoord ("", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType" = "TransparentCutout" }
		LOD 100
		Pass
		{
			Name "Unlit"
			Cull Off
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[2];
			float4 _LightColor0;
			cbuffer UnityPerCamera : register(b1)
			{
				float3 _WorldSpaceCameraPos : packoffset(c4);
			}
			cbuffer UnityLighting : register(b2)
			{
				float4 _WorldSpaceLightPos0 : packoffset(c0);
			}
			cbuffer UnityPerDraw : register(b3)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
				float4x4 unity_WorldToObject : packoffset(c4);
				float4 unity_WorldTransformParams : packoffset(c9);
			}
			cbuffer UnityPerFrame : register(b4)
			{
				float4 glstate_lightmodel_ambient : packoffset(c0);
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, float4 i_COLOR0 : COLOR0, float4 i_TEXCOORD0 : TEXCOORD0, float4 i_TANGENT0 : TANGENT0, float3 i_NORMAL0 : NORMAL0, out float4 o_SV_POSITION0 : SV_POSITION, out float4 o_TEXCOORD1 : TEXCOORD1, out float4 o_TEXCOORD2 : TEXCOORD2, out float4 o_COLOR0 : COLOR0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				uint4 r3;
				uint4 r4;
				uint4 r5;
				bool4 b0;
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * i_POSITION0.yyyy);
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * i_POSITION0.xxxx + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * i_POSITION0.zzzz + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				o_TEXCOORD1.xy = i_TEXCOORD0.xy;
				o_TEXCOORD1.zw = float2(0.0, 0.0);
				b0.x = (unity_WorldTransformParams.w >= 0.0);
				r0.x = b0.x ? 0x3f800000u : 0xbf800000u;
				r0.x = asuint(i_TANGENT0.w * asfloat(r0.x));
				r0.yzw = asuint(unity_ObjectToWorld._m11_m21_m01 * i_TANGENT0.yyy);
				r0.yzw = asuint(unity_ObjectToWorld._m10_m20_m00 * i_TANGENT0.xxx + asfloat(r0.yzw));
				r0.yzw = asuint(unity_ObjectToWorld._m12_m22_m02 * i_TANGENT0.zzz + asfloat(r0.yzw));
				r1.x = asuint(dot(asfloat(r0.yzw), asfloat(r0.yzw)));
				r1.x = asuint(rsqrt(asfloat(r1.x)));
				r0.yzw = asuint(asfloat(r1.xxx) * asfloat(r0.zyw));
				r1.x = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m00_m10_m20));
				r1.y = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m01_m11_m21));
				r1.z = asuint(dot(i_NORMAL0.xyz, unity_WorldToObject._m02_m12_m22));
				r1.w = asuint(dot(asfloat(r1.xyz), asfloat(r1.xyz)));
				r1.w = asuint(rsqrt(asfloat(r1.w)));
				r1.xyz = asuint(asfloat(r1.xyz) * asfloat(r1.www));
				r2.xyz = asuint(asfloat(r1.zxy) * asfloat(r0.zyw));
				r2.xyz = asuint(asfloat(r1.yzx) * asfloat(r0.ywz) + -asfloat(r2.xyz));
				r2.xyz = asuint(asfloat(r2.yxz) * asfloat(r0.xxx));
				r3.y = r2.x;
				r3.x = r0.z;
				r4.xyz = asuint(unity_ObjectToWorld._m01_m11_m21 * i_POSITION0.yyy);
				r4.xyz = asuint(unity_ObjectToWorld._m00_m10_m20 * i_POSITION0.xxx + asfloat(r4.xyz));
				r4.xyz = asuint(unity_ObjectToWorld._m02_m12_m22 * i_POSITION0.zzz + asfloat(r4.xyz));
				r4.xyz = asuint(unity_ObjectToWorld._m03_m13_m23 + asfloat(r4.xyz));
				r5.xyz = asuint(_WorldSpaceCameraPos.xyz + -asfloat(r4.xyz));
				r4.xyz = asuint(_WorldSpaceLightPos0.xyz - asfloat(r4.xyz) * _WorldSpaceLightPos0.www);
				r3.z = r1.y;
				r3.xyz = asuint(asfloat(r5.yyy) * asfloat(r3.xyz));
				r0.z = r2.z;
				r2.x = r0.w;
				r2.z = r1.x;
				r2.xyz = asuint(asfloat(r2.xyz) * asfloat(r5.xxx) + asfloat(r3.xyz));
				r0.w = r1.z;
				r0.xyz = asuint(asfloat(r0.yzw) * asfloat(r5.zzz) + asfloat(r2.xyz));
				r0.x = asuint(dot(asfloat(r0.xyz), asfloat(r0.xyz)));
				r0.x = asuint(rsqrt(asfloat(r0.x)));
				r0.x = asuint(asfloat(r0.z) * asfloat(r0.x));
				b0.x = (0.0 < asfloat(r0.x));
				r0.xyz = asuint(b0.xxx ? asfloat(r1.xyz) : -asfloat(r1.xyz));
				r0.x = asuint(dot(asfloat(r4.xyz), asfloat(r0.xyz)));
				r0.x = asuint(asfloat(r0.x) * 0.5 + 0.5);
				r0.yzw = asuint(glstate_lightmodel_ambient.xyz + glstate_lightmodel_ambient.xyz);
				o_TEXCOORD2.xyz = _LightColor0.xyz * asfloat(r0.xxx) + asfloat(r0.yzw);
				o_TEXCOORD2.w = 0.0;
				o_COLOR0 = i_COLOR0.xyzw;
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[4];
			float4 _Albedo_ST;
			Texture2D<float4> _Albedo : register(t0);
			SamplerState sampler_Albedo : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float4 i_TEXCOORD1 : TEXCOORD1, float4 i_TEXCOORD2 : TEXCOORD2, float4 i_COLOR0 : COLOR0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				r0.xy = asuint(i_TEXCOORD1.xy * _Albedo_ST.xy + _Albedo_ST.zw);
				r0 = asuint(_Albedo.Sample(sampler_Albedo, asfloat(r0.xy)));
				r0.w = asuint(-0.5 + asfloat(r0.w));
				b0.w = (asfloat(r0.w) < 0.0);
				if (b0.w) discard;
				r1.xyz = asuint(i_TEXCOORD2.xyz);
				r1.xyz = asuint(i_COLOR0.xyz * asfloat(r1.xyz));
				o_SV_Target0.xyz = asfloat(r1.xyz) * asfloat(r0.xyz);
				o_SV_Target0.w = 0.0;
			}
			#endif
			ENDHLSL
		}
	}
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
