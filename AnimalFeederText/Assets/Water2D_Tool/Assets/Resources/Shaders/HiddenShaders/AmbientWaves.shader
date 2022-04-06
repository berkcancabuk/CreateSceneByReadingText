Shader "Hidden/AmbientWaves"
{
	Properties
	{
		[HideInInspector] _MainTex ("Texture", 2D) = "grey" {}
        [HideInInspector] _FadeDistance ("Fade Distance", Range(0, 1)) = 0.5
        [HideInInspector] _FadeEnd ("Fade End", Range(0, 1)) = 1.0
        [HideInInspector] _FadeDirection ("Fade Direction", Float) = 1
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float _WaterWidth;
            float _WaterLength;

            float _Amplitude1;
            float _WaveLength1;
            float _PhaseOffset1;

            float _Amplitude2;
            float _WaveLength2;
            float _PhaseOffset2;

            float _Amplitude3;
            float _WaveLength3;
            float _PhaseOffset3;

            float _FadeDistance;
            float _FadeEnd;
            float _AmplitudeFade;
            float _FadeDirection;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed col = tex2D(_MainTex, i.uv).r;
                float offsetColor = 0;    
                float phase = _Time * 60.0;

                float fade1 = (i.uv.y - (1.0 - _FadeEnd)) * _FadeDistance;
                fade1 = clamp(fade1, 0.0, 1.0);

                float fade2 = (1.0 - i.uv.y - _FadeEnd) * _FadeDistance;
                fade2 = clamp(fade2, 0.0, 1.0);
               
                float scale = fade1 * _FadeDirection + fade2 * step((_FadeDirection + 1), 1);
                scale = scale * _AmplitudeFade - step(_AmplitudeFade - 1, -0.5);
                
                offsetColor += _Amplitude1 * scale * sin(i.uv.x * _WaterWidth * _WaveLength1 + phase * _PhaseOffset1);
                offsetColor += _Amplitude2 * scale * sin(i.uv.x * _WaterWidth * _WaveLength2 + phase * _PhaseOffset2);
                offsetColor += _Amplitude3 * scale * sin(i.uv.x * _WaterWidth * _WaveLength3 + phase * _PhaseOffset3);

                col += offsetColor;
                col = clamp(0, 1, col);

				return col;
			}
			ENDCG
		}
	}
}
