Shader "Bakery/Skybox"
{
	Properties
	{
		_Tint ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_Exposure ("Exposure", Float) = 1
		_MatrixRight ("Right", Vector) = (1,0,0,0)
		_MatrixUp ("Up", Vector) = (0,1,0,0)
		_MatrixForward ("Forward", Vector) = (0,0,1,0)
		[NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "white" {}
		_NoTexture ("No texture", Float) = 0
		_Hemispherical ("Hemispherical", Float) = 0
	}
	SubShader
	{
		Tags { "PreviewType" = "Skybox" "QUEUE" = "Background" "RenderType" = "Background" }
		Pass
		{
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[4];
			float _GlobalsPad1;
			float3 _MatrixRight;
			float3 _MatrixUp;
			float _GlobalsPad2;
			float3 _MatrixForward;
			float _GlobalsPad3;
			float4x4 reflection2World;
			cbuffer UnityPerDraw : register(b1)
			{
				float4x4 unity_ObjectToWorld : packoffset(c0);
			}
			cbuffer UnityPerFrame : register(b2)
			{
				float4x4 unity_MatrixVP : packoffset(c17);
			}

			void vert(float4 i_POSITION0 : POSITION0, out float4 o_SV_POSITION0 : SV_POSITION, out float3 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				uint4 r1;
				r0.x = asuint(dot(_MatrixUp.xyz, i_POSITION0.xyz));
				r0 = asuint(unity_ObjectToWorld._m01_m11_m21_m31 * asfloat(r0.xxxx));
				r1.x = asuint(dot(_MatrixRight.xyz, i_POSITION0.xyz));
				r0 = asuint(unity_ObjectToWorld._m00_m10_m20_m30 * asfloat(r1.xxxx) + asfloat(r0));
				r1.x = asuint(dot(_MatrixForward.xyz, i_POSITION0.xyz));
				r0 = asuint(unity_ObjectToWorld._m02_m12_m22_m32 * asfloat(r1.xxxx) + asfloat(r0));
				r0 = asuint(unity_ObjectToWorld._m03_m13_m23_m33 + asfloat(r0));
				r1 = asuint(unity_MatrixVP._m01_m11_m21_m31 * asfloat(r0.yyyy));
				r1 = asuint(unity_MatrixVP._m00_m10_m20_m30 * asfloat(r0.xxxx) + asfloat(r1));
				r1 = asuint(unity_MatrixVP._m02_m12_m22_m32 * asfloat(r0.zzzz) + asfloat(r1));
				o_SV_POSITION0 = unity_MatrixVP._m03_m13_m23_m33 * asfloat(r0.wwww) + asfloat(r1);
				r0.xyz = asuint(reflection2World._m01_m11_m21 * i_POSITION0.yyy);
				r0.xyz = asuint(reflection2World._m00_m10_m20 * i_POSITION0.xxx + asfloat(r0.xyz));
				o_TEXCOORD0 = reflection2World._m02_m12_m22 * i_POSITION0.zzz + asfloat(r0.xyz);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[2];
			float4 _Tex_HDR;
			float4 _Tint;
			float _Exposure;
			float _GlobalsPad1;
			float _GlobalsPad2;
			float _GlobalsPad3;
			float4 _GlobalsPad4[6];
			float _NoTexture;
			float _Hemispherical;
			TextureCube<float4> _Tex : register(t0);
			SamplerState sampler_Tex : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float3 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				bool4 b0;
				b0.x = (_NoTexture < 0.5);
				r1 = asuint(_Tex.Sample(sampler_Tex, i_TEXCOORD0.xyz));
				r0 = b0.xxxx ? r1 : uint4(0x3f800000u, 0x3f800000u, 0x3f800000u, 0x3f800000u);
				r0.w = asuint(-1.0 + asfloat(r0.w));
				r0.w = asuint(_Tex_HDR.w * asfloat(r0.w) + 1.0);
				r0.w = asuint(log2(asfloat(r0.w)));
				r0.w = asuint(_Tex_HDR.y * asfloat(r0.w));
				r0.w = asuint(exp2(asfloat(r0.w)));
				r0.w = asuint(_Tex_HDR.x * asfloat(r0.w));
				r0.xyz = asuint(asfloat(r0.www) * asfloat(r0.xyz));
				r0.xyz = asuint(_Tint.xyz * asfloat(r0.xyz));
				r0.xyz = asuint(float3(_Exposure, _Exposure, _Exposure) * asfloat(r0.xyz));
				b0.w = (i_TEXCOORD0.y < 0.0);
				r0.w = b0.w ? 0u : 0x3f800000u;
				r1.xyz = asuint(asfloat(r0.xyz) * asfloat(r0.www));
				b0.w = (0.0 < _Hemispherical);
				o_SV_Target0.xyz = b0.www ? asfloat(r1.xyz) : asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#endif
			ENDHLSL
		}
	}
}
