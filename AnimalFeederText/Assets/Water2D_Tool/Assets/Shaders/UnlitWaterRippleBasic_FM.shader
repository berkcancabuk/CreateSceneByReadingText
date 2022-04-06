Shader "Water2D/UnlitWaterRippleBasic_FM"
{
	Properties
	{
		[HideInInspector] _MainTex ("Texture", 2D) = "grey" {}
		_BaseColor ("Base Color", COLOR)  = ( .17, .25, .2, 0.5)
        _WaterLineColor ("Water Line Color", COLOR)  = ( 1, 1, 1, 1)
        _WaterLineHeight ("Water Line Height", Float) = 0.1

        [HideInInspector] _WaveHeightScale (" ", Float) = 1.0
        [HideInInspector] _WaterHeight (" ", Float) = 10
        [HideInInspector] _HeightOffset (" ", Float) = 0
        [HideInInspector] _ApplyOffset (" ", Float) = 0.0
        [HideInInspector] _BottomPos (" ", Float) = -2
	}
	SubShader
	{
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
            #include "Water2DInclude.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			float4 _MainTex_ST;
            float4 _BaseColor;
            float _WaterLineHeight;
			
			v2f vert (appdata v)
			{
				v2f o;
				
                float offset = GetAverage(_MainTex, _MainTex_TexelSize, float4(v.uv.xy, 0, 0));
                float oneOrZero = step(_BottomPos + 0.001, v.vertex.y) * _ApplyOffset;
                v.vertex.y += (offset * _WaveHeightScale * oneOrZero) + (_HeightOffset * oneOrZero);

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                fixed4 baseColor = _BaseColor;

                float uvFilter = 1.0 - (1.0 / _WaterHeight) * _WaterLineHeight;
                float oneOrZero = step(uvFilter, i.uv.y);
                baseColor = baseColor * (step((oneOrZero + 1), 1)) + _WaterLineColor * oneOrZero;
               
                return baseColor;
            }
			ENDCG
		}
	}
}
