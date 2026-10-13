Shader "HighlightPlus/ClearStencil"
{
	Properties
	{
	}
	SubShader
	{
		Pass
		{
			ColorMask 0
			Cull Off
			ZTest Always
			ZWrite Off
			Stencil { Ref 2 ReadMask 255 WriteMask 255 Comp Always Pass Zero Fail Keep ZFail Keep }
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
				o.pos = float4(v.vertex.xy, 0.0, 0.5);
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
