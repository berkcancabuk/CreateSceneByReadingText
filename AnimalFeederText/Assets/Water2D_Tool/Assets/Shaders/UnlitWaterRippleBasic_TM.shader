Shader "Water2D/UnlitWaterRippleBasic_TM" {
    Properties {
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "grey" {}
        _DeepWaterColor ("Deep Water Color", Color) = (0.73, 0.92, 0.99, 1)
        _CubeMapLevel ("Cube Map Reflection", Range (0.0, 1.0)) = 0.2
        [KeywordEnum(PerPixel, PerVertex)] 
		_Normals("Normals", Float)  = 0
        _NormalStrength ("Normal Strength", Float) = 15
        _Cube ("Cubemap", CUBE) = "black" {}

        [HideInInspector] _WaveHeightScale ("Wave Height Scale", Float) = 1.0
        [HideInInspector] _FaceCulling ("", Float) = 2
        [HideInInspector] _HeightOffset (" ", Float) = 0.0
        [HideInInspector] _ApplyOffset (" ", Float) = 0.0
        [HideInInspector] _BottomPos (" ", Float) = -2
	}

    Subshader {
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent"}
        Cull [_FaceCulling]
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM

		    #pragma vertex vert 
		    #pragma fragment frag
            #pragma target 3.0
            #pragma shader_feature _NORMALS_PERPIXEL _NORMALS_PERVERTEX
 
            #include "UnityCG.cginc"
            #include "Water2DInclude.cginc"
  
            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 viewInterpolator : TEXCOORD1;
                #if _NORMALS_PERVERTEX
                    float3 vertNormal : TEXCOORD3;
                #endif
            };
              
            float4 _MainTex_ST;
       
            v2f vert(appdata_full v)
            {
                v2f o;

                float offset = GetAverage(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0));
                
                float oneOrZero = step(_BottomPos + 0.001, v.vertex.y) * _ApplyOffset;
                v.vertex.y += (offset * _WaveHeightScale * oneOrZero) + (_HeightOffset * oneOrZero);

                #if _NORMALS_PERVERTEX
                    o.vertNormal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0), _NormalStrength);
                #endif

                half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
                o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
    
                float3 normal = float3(0, 1, 0);
                
                #ifdef _NORMALS_PERPIXEL
                    normal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(i.uv, 0, 0), _NormalStrength);
                #endif 
    
                #ifdef _NORMALS_PERVERTEX 
                    normal = i.vertNormal;      
                #endif

                half3 viewVector = normalize(i.viewInterpolator.xyz);
                float4x4 modelMatrixInverse = unity_WorldToObject;
                float3 normalDir = normalize(mul(float4(normal, 0.0), modelMatrixInverse).xyz);
                float3 reflectedDir = reflect(viewVector, normalize(normalDir));           

                float4 baseColor = lerp(_DeepWaterColor, texCUBE(_Cube, reflectedDir), _CubeMapLevel);
                baseColor.a = _DeepWaterColor.a;
                       
                return baseColor;
            }
		    ENDCG
        }
    }
}
