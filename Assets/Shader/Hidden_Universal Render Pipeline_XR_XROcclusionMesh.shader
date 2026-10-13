Shader "Hidden/Universal Render Pipeline/XR/XROcclusionMesh"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" }
		Pass
		{
			ColorMask 0
			Cull Off
			ZTest Always
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			float4 vert(appdata v) : SV_POSITION
			{
				return float4(v.vertex.xy * float2(2.0, -2.0) + float2(-1.0, 1.0), 1.0, 1.0);
			}

			float4 frag() : SV_Target
			{
				return float4(0.0, 0.0, 0.0, 0.0);
			}
			ENDHLSL
		}
	}
}
