Shader "UI/Particles/Hidden"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "QUEUE" = "Geometry" "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
			Cull Off
			ZTest LEqual
			ZWrite Off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert()
			{
				v2f o;
				o.pos = float4(0.0, 0.0, 0.0, 0.0);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				discard;
				return float4(0.0, 0.0, 0.0, 0.0);
			}
			ENDHLSL
		}
	}
}
