#ifndef WATER2DINCLUDE_INCLUDED
#define WATER2DINCLUDE_INCLUDED

#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _FoamTex;
sampler2D _FoamGradient;
sampler2D _ReflectionTex;
sampler2D _WaterLineTex;
sampler2D _BumpMap;

sampler2D _RefractionTexUnlit_TM;
sampler2D _RefractionTexUnlit_FM;
sampler2D _RefractionTexLit_TM;
sampler2D _RefractionTexLit_FM;

samplerCUBE _Cube;
sampler2D_float _CameraDepthTexture;

float4 _WaterLineTex_TexelSize;
float4 _MainTex_TexelSize;
float4 _ShallowWaterColor;
float4 _DeepWaterColor;
float4 _BumpTiling;
float4 _BumpDirection;
float4 _SpecularColor;
float4 _WorldLightDir;
float4 _FoamColor;
float4 _WaterLineColor;

float _WaterDepth;
float _FogDepth;
float _FogFalloff;
float _FogDensityCap;

float _WaveHeightScale;
float _NormalStrength;
float _CubeMapLevel;
float _Distortion;
float _EdgeBlend;
float _OneOrZero;
float _FoamStrength;
float _BottomPos;
float _HeightOffset;
float _FresnelPower;
float _FresnelBias;
float _Shininess;
float _UnderWaterOpacity;
float _WaterHeight;
float _WaterWidth;
float _Opacity;
float _BumpWaves;
float _PixelsPerUnit;
float _Smoothness;
float _ApplyOffset;

float GetAverage(sampler2D mainTex, float4 texelSize, float4 uv)
{
    float2 dx, dy;
    float average;

    dx = float2(texelSize.x, 0.0);
    dy = float2(0.0, texelSize.y);

    average = (tex2Dlod(mainTex, float4(uv - dx, 0, 0)).r - 0.5) * 2.0;
    average += (tex2Dlod(mainTex, float4(uv - dy, 0, 0)).r - 0.5) * 2.0;
    average += (tex2Dlod(mainTex, float4(uv + dx, 0, 0)).r - 0.5) * 2.0;
    average += (tex2Dlod(mainTex, float4(uv + dy, 0, 0)).r - 0.5) * 2.0;
        
    average *= 0.25;

    return average;
}

float3 NormalFromHeightmap(sampler2D mainTex, float4 texelSize, float4 uv, float normalStrength)
{
    float rightTexel = tex2Dlod(mainTex, float4(uv.x + texelSize.x, uv.y, 0, 0)).r * normalStrength - normalStrength * 0.5;
    float leftTexel = tex2Dlod(mainTex, float4(uv.x - texelSize.x, uv.y, 0, 0)).r * normalStrength - normalStrength * 0.5;
    float topTexel = tex2Dlod(mainTex, float4(uv.x, uv.y - texelSize.y, 0, 0)).r * normalStrength - normalStrength * 0.5;
    float bottomTexel = tex2Dlod(mainTex, float4(uv.x, uv.y + texelSize.y, 0, 0)).r * normalStrength - normalStrength * 0.5;
			
    float3 dx = normalize(float3(1, leftTexel - rightTexel, 0));
    float3 dy = normalize(float3(0, bottomTexel - topTexel, 1));
		  
    float3 normal = normalize(float3(1, 1, 1));   
    normal.xz = cross(dx, dy).xz;  
    normal = normalize(normal);

    return normal;
}

float GetSpecularLevel(half3 viewVector, half3 worldNormal, float4 worldLightDir, float shininess)
{
    half3 reflectVector = normalize(reflect(viewVector, worldNormal));
    half3 h = normalize((worldLightDir.xyz) + viewVector.xyz);
    float nh = max(0, dot(worldNormal, -h));
    float spec = max(0.0, pow(nh, shininess));

    return spec;
}

inline half Fresnel(half3 viewVector, half3 worldNormal, half bias, half power)
{
    half facing = clamp(1.0 - max(dot(-viewVector, worldNormal), 0.0), 0.0, 1.0);
    half refl2Refr = saturate(bias + (1.0 - bias) * pow(facing, power));
    return refl2Refr;
}

float3 Foam(sampler2D _FoamTex, sampler2D _FoamGradient, float2 foamuv, float objectZ, float depth, float _FoamStrength, half3 bump)
{
    float intensityFactor = 1 - saturate((depth - objectZ) / _FoamStrength);
    half3 foamGradient = 1 - tex2D(_FoamGradient, float2(intensityFactor - _Time.y * 0.15, 0) + bump.xy * 0.15);
    float2 foamDistortUV = bump.xy * 0.2;
    half3 foamColor = tex2D(_FoamTex, foamuv + foamDistortUV).rgb;
    half foamLightIntensity = saturate((_WorldSpaceLightPos0.y + 0.2) * 4);
    float3 foam = foamGradient * intensityFactor * foamColor * foamLightIntensity;

    return foam;
}

float4 AddWaterLine(float2 uv, float4 baseColor)
{
    float u = _WaterWidth * uv.x * (_PixelsPerUnit * _WaterLineTex_TexelSize.x);
    float v = (_WaterHeight * (uv.y - 1) * (_PixelsPerUnit * _WaterLineTex_TexelSize.y));
    float4 waterLineColor = tex2D(_WaterLineTex, float2(u, v)) * _WaterLineColor;
    
    waterLineColor = lerp(baseColor, waterLineColor, waterLineColor.a);
    float oneOrZero = step(abs(v), 0.95);
    baseColor = baseColor * (step((oneOrZero + 1), 1)) + waterLineColor * oneOrZero;

    return baseColor;
}

float4 GetCubeMapColor(half3 viewVector, float3 normal, samplerCUBE cube)
{
    float4x4 modelMatrixInverse = unity_WorldToObject;
    float3 normalDir = normalize(mul(float4(normal, 0.0), modelMatrixInverse).xyz);
    float3 reflectedDir = reflect(viewVector, normalize(normalDir));
    float4 cubeMapColor = texCUBE(cube, reflectedDir);

    return cubeMapColor;
}

float3 ReflectedDir(float3 viewInterpolator, float3 normal)
{
    half3 viewVector = normalize(viewInterpolator);
    float4x4 modelMatrixInverse = unity_WorldToObject;
    float3 normalDir = normalize(mul(float4(normal, 0.0), modelMatrixInverse).xyz);
    float3 reflectedDir = reflect(viewVector, normalize(normalDir));

    return reflectedDir;
}

float3 BSplineResampling(sampler2D mainTex, float4 texelSize, float4 uv, float normalStrength)
{
    float2 res = float2(texelSize.z, texelSize.w);

    float2 UVs = uv*res-0.5;
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

    float3 tex00 = tex2Dlod(mainTex, float4(h0.x, h0.y, 0, 0)).rgb; 
    float3 tex10 = tex2Dlod(mainTex, float4(h1.x, h0.y, 0, 0)).rgb; 
    float3 tex01 = tex2Dlod(mainTex, float4(h0.x, h1.y, 0, 0)).rgb;
    float3 tex11 = tex2Dlod(mainTex, float4(h1.x, h1.y, 0, 0)).rgb; 

    tex00 = lerp(tex01, tex00, g0.y);
    tex10 = lerp(tex11, tex10, g0.y);
    return lerp(tex10, tex00, g0.x);
}

inline void ComputeScreenAndGrabPassPos (float4 pos, out float4 screenPos, out float4 grabPassPos) 
{
	#if UNITY_UV_STARTS_AT_TOP
		float scale = -1.0;
	#else
		float scale = 1.0f;
	#endif
	
	screenPos = ComputeNonStereoScreenPos(pos); 
	grabPassPos.xy = ( float2( pos.x, pos.y*scale ) + pos.w ) * 0.5;
	grabPassPos.zw = pos.zw;
}
#endif
