Shader "Hidden/ProBuilder/HideVertices"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "IGNOREPROJECTOR" = "true" "RenderType" = "Geometry" }
		Pass
		{
			Cull Back
			ZTest LEqual
			ZWrite On
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = float4(0.0, 0.0, 0.0, 0.0);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				return float4(0.0, 0.0, 0.0, 0.0);
			}
			ENDHLSL
		}
	}
}
