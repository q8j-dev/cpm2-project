Shader "Hidden/Universal Render Pipeline/Stop NaN"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			Name "Stop NaN"
			Cull Off
			ZTest Always
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#if defined(SHADER_STAGE_VERTEX)
			float4 _GlobalsPad0[127];
			float4 _BlitScaleBias;

			void vert(uint i_SV_VertexID0 : SV_VertexID, out float4 o_SV_POSITION0 : SV_POSITION, out float2 o_TEXCOORD0 : TEXCOORD0)
			{
				uint4 r0;
				r0.x = asuint(asint(asfloat(i_SV_VertexID0)) << 1);
				r0.x = 2u & r0.x;
				r0.w = 2u & asuint(asfloat(i_SV_VertexID0));
				r0.xy = asuint(float2(r0.xw));
				o_SV_POSITION0.xy = asfloat(r0.xy) * float2(2.0, 2.0) + float2(-1.0, -1.0);
				r0.z = asuint(1.0 + -asfloat(r0.y));
				o_TEXCOORD0 = asfloat(r0.xz) * _BlitScaleBias.xy + _BlitScaleBias.zw;
				o_SV_POSITION0.zw = float2(1.0, 1.0);
			}
			#endif

			#if defined(SHADER_STAGE_FRAGMENT)
			float4 _GlobalsPad0[5];
			float2 _GlobalMipBias;
			Texture2D<float4> _BlitTexture : register(t0);
			SamplerState sampler_PointClamp : register(s0);

			void frag(float4 i_SV_POSITION0 : SV_POSITION, float2 i_TEXCOORD0 : TEXCOORD0, out float4 o_SV_Target0 : SV_Target0)
			{
				uint4 r0;
				uint4 r1;
				uint4 r2;
				bool4 b0;
				bool4 b1;
				bool4 b2;
				r0 = asuint(_BlitTexture.SampleBias(sampler_PointClamp, i_TEXCOORD0.xy, _GlobalMipBias.x));
				r1.xyz = uint3(0x7fffffffu, 0x7fffffffu, 0x7fffffffu) & r0.xyz;
				b2.xyz = (uint3(0x7f800000u, 0x7f800000u, 0x7f800000u) < r1.xyz);
				b1.xyz = (int3(int(0x7f800000), int(0x7f800000), int(0x7f800000)) == asint(r1.xyz));
				b0.w = b2.x || b2.y;
				b0.w = b0.w || b2.z;
				b1.x = b1.x || b1.y;
				b1.x = b1.x || b1.z;
				b0.w = b1.x || b0.w;
				o_SV_Target0.xyz = b0.www ? float3(0.0, 0.0, 0.0) : asfloat(r0.xyz);
				o_SV_Target0.w = 1.0;
			}
			#endif
			ENDHLSL
		}
	}
}
