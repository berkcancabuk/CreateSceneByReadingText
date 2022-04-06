Shader "Hidden/BSplineResampling"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "grey" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
			
			float4 frag (v2f_img i) : SV_Target
			{
                float2 res = float2(_MainTex_TexelSize.z, _MainTex_TexelSize.w);

                float2 UVs = i.uv * res - 0.5;
                float2 index = floor(UVs);
                float2 fraction = frac(UVs);
                float2 one_frac = 1.0 - fraction;
                float2 one_frac2 = one_frac * one_frac;
                float2 fraction2 = fraction * fraction;

                float2 w0 = 1.0/6.0 * one_frac2 * one_frac;
                float2 w1 = 2.0/3.0 - 0.5 * fraction2 * (2.0-fraction);
                float2 w2 = 2.0/3.0 - 0.5 * one_frac2 * (2.0-one_frac);
                float2 w3 = 1.0/6.0 * fraction2 * fraction;
                float2 g0 = w0 + w1;
                float2 g1 = w2 + w3;

                float2 h0 = ((w1 / g0) - 0.5 + index) / res;
                float2 h1 = ((w3 / g1) + 1.5 + index) / res;

                float3 tex00 = tex2Dlod(_MainTex, float4(h0.x, h0.y, 0, 0)).rgb; 
                float3 tex10 = tex2Dlod(_MainTex, float4(h1.x, h0.y, 0, 0)).rgb; 
                float3 tex01 = tex2Dlod(_MainTex, float4(h0.x, h1.y, 0, 0)).rgb;
                float3 tex11 = tex2Dlod(_MainTex, float4(h1.x, h1.y, 0, 0)).rgb; 

                tex00 = lerp(tex01, tex00, g0.y);
                tex10 = lerp(tex11, tex10, g0.y);
                float3 finalCol = lerp(tex10, tex00, g0.x);
				
                float4 col = float4(finalCol, 1);
				return col;
			}
			ENDCG
		}
	}
}
