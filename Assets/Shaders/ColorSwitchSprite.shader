Shader "Sprites/ColorSwitch"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Range("Range", Range(1, 20)) = 10
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			}; 
			
			fixed4 _Color;
			half _Range;
			fixed4 _ColorMatrix[20];
			 
			v2f vert(appdata_t IN) {
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : COLOR {
				fixed4 col = tex2D(_MainTex, IN.texcoord);
				half index = (half)round(col.r * _Range);
				fixed4 paletteColor = _ColorMatrix[index];
				paletteColor.a = col.a;

				fixed4 outColor = lerp(paletteColor, col, step(col.a, 0.99));

				return outColor;
			}
		ENDCG
		}
	}
}