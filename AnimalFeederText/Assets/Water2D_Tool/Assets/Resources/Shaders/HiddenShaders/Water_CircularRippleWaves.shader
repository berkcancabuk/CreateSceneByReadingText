Shader "Hidden/Water_CircularRippleWaves" {
    Properties
	{
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"
    #include "Water2DHiddenInclude.cginc"

    float4 frag(v2f_img i) : COLOR
	{      
        return GetInteractionsSum(i.uv);
    }

    float GetAverage(float2 uv)
    {
        float2 dx, dy;
        float average;

        dx = float2(_MainTex_TexelSize.x, 0.0);
        dy = float2(0.0, _MainTex_TexelSize.y);

        average = (tex2D(_MainTex, uv - dx).r - 0.5) * 2.0;        
        average += (tex2D(_MainTex, uv - dy).r - 0.5) * 2.0;
        average += (tex2D(_MainTex, uv + dx).r - 0.5) * 2.0;
        average += (tex2D(_MainTex, uv + dy).r - 0.5) * 2.0;
        
        average *= 0.5;

        return average;
    }

    float4 fragWaveMovement(v2f_img i) : COLOR
    {
        float average;
        float value;

        average = GetAverage(i.uv);

        float newValue = average - ((tex2D(_PrevTex, i.uv).r - 0.5) * 2.0);
        float dampedValue = newValue * (1.0 - _Damping);
        
        dampedValue = (dampedValue * 0.5) + 0.5;
        dampedValue = clamp(dampedValue, 0, 1);

        return dampedValue;
    }


	ENDCG

    Subshader {
	    Pass {
		    CGPROGRAM
		    #pragma vertex vert_img
		    #pragma fragment frag
		    ENDCG
	    }

	    Pass {
		    CGPROGRAM
		    #pragma vertex vert_img
		    #pragma fragment fragWaveMovement
		    ENDCG
	    }
    }
    Fallback off
}