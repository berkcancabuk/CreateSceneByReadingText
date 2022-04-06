#ifndef WATER2DHIDDENINCLUDE_INCLUDED
#define WATER2DHIDDENINCLUDE_INCLUDED

#include "UnityCG.cginc"

sampler2D _PrevTex;
sampler2D _MainTex;
sampler2D _ObstructionTex;

float4 _MainTex_TexelSize;
float4 _WaterRipple1;
float4 _WaterRipple2;
float4 _WaterRipple3;
float4 _WaterRipple4;
float4 _WaterRipple5;
float4 _WaterRipple6;
float4 _WaterRipple7;
float4 _WaterRipple8;
float4 _WaterRipple9;
float4 _WaterRipple10;

float4 _RecObst1;
float4 _RecObst2;
float4 _RecObst3;
float4 _RecObst4;
float4 _RecObst5;
float4 _AxisScale;

float3 _CircleObst1;
float3 _CircleObst2;
float3 _CircleObst3;
float3 _CircleObst4;
float3 _CircleObst5;

float _Damping;

float GetInteraction(float4 parameters, float2 uv)
{
    float2 center = parameters.xy;
    float radius = parameters.z;
    float strength = parameters.w;
       
    center *= _AxisScale.xy;
    uv *= _AxisScale.xy;

    float drop = max(0.0, 1.0 - length(center - uv) / radius);
    drop *= strength;
      
    return drop;
}

float GetInteractionsSum(float2 uv)
{
    float interactions;
    float origColor;
              
    origColor = tex2D(_MainTex, uv).r * 2.0 - 1.0;
        
    interactions = GetInteraction(_WaterRipple1, uv);
    interactions += GetInteraction(_WaterRipple2, uv);
    interactions += GetInteraction(_WaterRipple3, uv);
    interactions += GetInteraction(_WaterRipple4, uv);
    interactions += GetInteraction(_WaterRipple5, uv);
    interactions += GetInteraction(_WaterRipple6, uv);
    interactions += GetInteraction(_WaterRipple7, uv);
    interactions += GetInteraction(_WaterRipple8, uv);
    interactions += GetInteraction(_WaterRipple9, uv);
    interactions += GetInteraction(_WaterRipple10, uv);

    float final = interactions + origColor;
    final = clamp(final, -1, 1);
    final = final * 0.5 + 0.5;
    
    return final;
}

float InsideBox(float2 v, float2 bottomLeft, float2 topRight)
{
    float2 s = step(bottomLeft, v) - step(topRight, v);
    return s.x * s.y;
}

float InsideCircle(float2 uv, float2 cCenter, float cRadius)
{
    uv *= _AxisScale.xy;
    cCenter *= _AxisScale.xy;

    float radiusPowerTwo = cRadius * cRadius;
            
    float value = (uv.x - cCenter.x) * (uv.x - cCenter.x) + (uv.y - cCenter.y) * (uv.y - cCenter.y);
    value = step(value, radiusPowerTwo);

    return value;
}

float GetDynamicObstructionColor(float2 uv)
{
    fixed4 c = float4(0, 0, 0, 1);

    float value;
    float finalValue = 1.0;
            
    value = InsideBox(uv, float2(_RecObst1.x, _RecObst1.y), float2(_RecObst1.z, _RecObst1.w));
    finalValue = clamp(finalValue - value, 0, 1);
    
    value = InsideBox(uv, float2(_RecObst2.x, _RecObst2.y), float2(_RecObst2.z, _RecObst2.w));
    finalValue = clamp(finalValue - value, 0, 1);
    
    value = InsideBox(uv, float2(_RecObst3.x, _RecObst3.y), float2(_RecObst3.z, _RecObst3.w));
    finalValue = clamp(finalValue - value, 0, 1);

    value = InsideBox(uv, float2(_RecObst4.x, _RecObst4.y), float2(_RecObst4.z, _RecObst4.w));
    finalValue = clamp(finalValue - value, 0, 1);

    value = InsideBox(uv, float2(_RecObst5.x, _RecObst5.y), float2(_RecObst5.z, _RecObst5.w));
    finalValue = clamp(finalValue - value, 0, 1);
            

    value = InsideCircle(uv, float2(_CircleObst1.x, _CircleObst1.y), _CircleObst1.z);
    finalValue = clamp(finalValue - value, 0, 1);
        
    value = InsideCircle(uv, float2(_CircleObst2.x, _CircleObst2.y), _CircleObst2.z);
    finalValue = clamp(finalValue - value, 0, 1);
        
    value = InsideCircle(uv, float2(_CircleObst3.x, _CircleObst3.y), _CircleObst3.z);
    finalValue = clamp(finalValue - value, 0, 1);
        
    value = InsideCircle(uv, float2(_CircleObst4.x, _CircleObst4.y), _CircleObst4.z);
    finalValue = clamp(finalValue - value, 0, 1);
        
    value = InsideCircle(uv, float2(_CircleObst5.x, _CircleObst5.y), _CircleObst5.z);
    finalValue = clamp(finalValue - value, 0, 1);
            
    return finalValue;
}

#endif
