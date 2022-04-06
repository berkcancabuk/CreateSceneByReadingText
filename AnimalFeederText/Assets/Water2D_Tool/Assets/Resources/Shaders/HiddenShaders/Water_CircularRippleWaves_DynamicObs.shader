Shader"Hidden/Water_CircularRippleWaves_DynamicObs" {
    Properties
	{
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"
    #include "Water2DHiddenInclude.cginc"

    float4 frag(v2f_img i) : COLOR
	{
        float interactionsSum = GetInteractionsSum(i.uv);   
        float oneOrZero = GetDynamicObstructionColor(i.uv);

        interactionsSum = interactionsSum * oneOrZero - 0.5 * (oneOrZero - 1.0);
        
        return interactionsSum;
    }

    float GetAverageWithDynamicObstruction(float2 uv)
    {
        float2 dx, dy;
        float2 texelUV;
        float average;

        dx = float2(_MainTex_TexelSize.x, 0.0);
        dy = float2(0.0, _MainTex_TexelSize.y);

        texelUV = uv - dx * GetDynamicObstructionColor(uv - dx);
        average = (tex2D(_MainTex, texelUV).r - 0.5) * 2.0;
            
        texelUV = uv - dy * GetDynamicObstructionColor(uv - dy);
        average += (tex2D(_MainTex, texelUV).r - 0.5) * 2.0;
        
        texelUV = uv + dx * GetDynamicObstructionColor(uv + dx);
        average += (tex2D(_MainTex, texelUV).r - 0.5) * 2.0;

        texelUV = uv + dy * GetDynamicObstructionColor(uv + dy);
        average += (tex2D(_MainTex, texelUV).r - 0.5) * 2.0;
        
        average *= 0.5;

        return average;
    }

    float4 fragWaveMovement(v2f_img i) : COLOR
	{  
        float average;
        float oneOrZero;

        average = GetAverageWithDynamicObstruction(i.uv);

        float newValue = average - ((tex2D(_PrevTex, i.uv).r - 0.5) * 2.0);
        float dampedValue = newValue * (1.0 - _Damping);
        dampedValue = (dampedValue * 0.5) + 0.5; 
        
        oneOrZero = GetDynamicObstructionColor(i.uv);
        dampedValue = dampedValue * oneOrZero - 0.5 * (oneOrZero - 1);
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