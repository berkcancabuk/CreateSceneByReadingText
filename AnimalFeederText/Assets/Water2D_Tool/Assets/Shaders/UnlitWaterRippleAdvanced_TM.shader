Shader "Water2D/UnlitWaterRippleAdvanced_TM" {
	Properties {
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "grey" {}
        [HideInInspector] _ReflectionTex ("", 2D) = "white" {}
 
        _DeepWaterColor ("Deep Water Color", Color) = (0.588, 0.745, 1.0, 1)
        _ShallowWaterColor ("Shallow Water Color", Color) = (0.874, 0.965, 1.0, 1)
        _UnderWaterOpacity ("Under Water Opacity", Range (0.0, 1.0)) = 0.4
        _CubeMapLevel ("Cube Map Reflection", Range (0.0, 1.0)) = 0.29    
        _EdgeBlend ("Edge Blend", Float) = 4.0
        _WaterDepth ("Water Depth", Float) = 0.15

        _Smoothness ("Reflection Intensity", Range(0.0, 1.0)) = 0.8
        _FresnelPower  ("Fresnel Power", Float) = 0.2
        _FresnelBias  ("Fresnel Bias", Float) = 0.2
        
        [Toggle(_SPECULAR_ON)] _SPECULAR ("Specular Reflection", Float) = 0
	    _SpecularColor ("Specular Color", COLOR)  = ( .72, .72, .72, 1)
	    _WorldLightDir ("Specular Light Direction", Vector) = (0.0, 0.1, -0.5, 0.0)
	    _Shininess ("Shininess", Range (2.0, 500.0)) = 200.0
		_Distortion("Distortion", Range (0.0, 1.5)) = 0.168
		
        [KeywordEnum(PerPixel, PerVertex)] _Normals ("Normals", Float)  = 0
        _NormalStrength ("Normal Strength", Float) = 15
		
        _BumpTiling ("Bump Tiling", Vector) = (0.04 ,0.04, 0.04, 0.08)
		_BumpDirection ("Bump Speed (Map1 x, y, Map2 z, w)", Vector) = (1.0 ,30.0, 20.0, -20.0)
        [NoScaleOffset] _Cube ("Cubemap", CUBE) = "black" {}
		[NoScaleOffset] _BumpMap ("Bump Normals", 2D) = "bump" {}
        
        [Toggle(_FOAM_ON)] _FOAM ("Enable Foam", Float) = 0
        _FoamColor("FoamColor", Color) = (1, 1, 1, 1)
	    [NoScaleOffset] _FoamTex ("Foam Texture", 2D) = "white" {}
	    [NoScaleOffset] _FoamGradient ("Foam Gradient ", 2D) = "white" {}
	    _FoamStrength ("Foam Strength", float) = 0.4

        [HideInInspector] _WaveHeightScale ("Wave Height Scale", Float) = 1.0
        [HideInInspector] _FaceCulling ("FaceCulling", Float) = 2
        [HideInInspector] _OneOrZero ("One Or Zero", Float) = 0
        [HideInInspector] _HeightOffset (" ", Float) = 0.0
        [HideInInspector] _ApplyOffset (" ", Float) = 0.0
        [HideInInspector] _BottomPos (" ", Float) = -2
	}

    Subshader {
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        ZTest LEqual
        Cull [_FaceCulling]
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        
        GrabPass {"_RefractionTexUnlit_TM"}
        
        Pass {
            CGPROGRAM
		    #pragma vertex vert 
		    #pragma fragment frag
            #pragma target 3.0
            //#pragma multi_compile WATER_HEIGHTMAP WATER_NORMAL WATER_COLOR
            #pragma multi_compile_fog 
            #pragma shader_feature _NORMALS_PERPIXEL _NORMALS_PERVERTEX
            #pragma shader_feature _ _FOAM_ON
            #pragma shader_feature _ _SPECULAR_ON

            #include "UnityCG.cginc"
            #include "Water2DInclude.cginc"

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float4 grabPassPos : TEXCOORD2;
                float3 viewInterpolator : TEXCOORD3;
                float2 bumpuv0 : TEXCOORD4;
                float2 bumpuv1 : TEXCOORD5;
                UNITY_FOG_COORDS(6)
                #if _FOAM_ON
                    float2 foamuv : TEXCOORD7;
                #endif
                #if _NORMALS_PERVERTEX
                    float3 vertNormal : TEXCOORD8;
                #endif
            };
     
            float4 _MainTex_ST;       

            v2f vert(appdata_full v)
            {
                v2f o;

                float offset = GetAverage(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0));
                float oneOrZero = step(_BottomPos + 0.001, v.vertex.y) * _ApplyOffset;
                v.vertex.y += (offset * _WaveHeightScale * oneOrZero) + (_HeightOffset * oneOrZero);

                half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
                o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;

                #if _NORMALS_PERVERTEX
                    o.vertNormal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0), _NormalStrength);
                #endif

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                
                o.screenPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.screenPos.z);
                o.grabPassPos = ComputeGrabScreenPos(o.pos);

                float4 temp;
                float4 wpos = mul(unity_ObjectToWorld, v.vertex);
                temp.xyzw = wpos.xzxz * _BumpTiling.xyzw + _Time.xxxx * 0.1 * _BumpDirection.xyzw;
                o.bumpuv0 = temp.xy;
                o.bumpuv1 = temp.wz;
                
                #if _FOAM_ON
                    o.foamuv = 7.0f * wpos.xz + 0.05 * float2(_SinTime.w, _SinTime.w);
                #endif
                
                UNITY_TRANSFER_FOG(o, o.pos);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 reflectionScreenPos = i.screenPos;
                reflectionScreenPos = float4(reflectionScreenPos.xyz, reflectionScreenPos.w + 0.00000000001);
                float4 refractionScreenPos = i.grabPassPos;
			             
                float3 normal = float3(0, 1, 0);
                
                #if _NORMALS_PERPIXEL
                    normal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(i.uv, 0, 0), _NormalStrength);
                #endif 
    
                #if _NORMALS_PERVERTEX 
                    normal = i.vertNormal;      
                #endif

                float3 worldNormal = UnityObjectToWorldNormal(normal);
    	        
                half3 bump1 = UnpackNormal(tex2D(_BumpMap, i.bumpuv0)).rgb;
                half3 bump2 = UnpackNormal(tex2D(_BumpMap, i.bumpuv1)).rgb;
                half3 bump = (bump1 + bump2) * 0.5;
                
                half2 distortOffset = half2(bump.xy * _Distortion);
                distortOffset = lerp(float2(0, 0), distortOffset, dot(normal, float3(0, 1, 0)));
            	
                half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
                float uvOffsetLevel = 1;

			    half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture , UNITY_PROJ_COORD(reflectionScreenPos));
                depth = LinearEyeDepth(depth);
			        
                edgeBlendFactors = saturate(_EdgeBlend * (depth - i.screenPos.w));
                uvOffsetLevel = saturate(_WaterDepth * (depth - i.screenPos.w));

                reflectionScreenPos.xy += lerp(float2(0, 0), (normal.xz * _WaveHeightScale) / (i.screenPos.w + 0.00000000001) + distortOffset.xy, uvOffsetLevel);
                refractionScreenPos.xy += lerp(float2(0, 0), (normal.xz * _WaveHeightScale) / i.grabPassPos.w + distortOffset.xy, uvOffsetLevel);
                    
                float currentPixelDepth = i.screenPos.z;
                float offsetPixelDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture , UNITY_PROJ_COORD(reflectionScreenPos)));

                float oneOrZero = step(offsetPixelDepth, currentPixelDepth);
                refractionScreenPos.xy = refractionScreenPos.xy * (step((oneOrZero + 1), 1)) + i.grabPassPos.xy * oneOrZero;

                float4 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(reflectionScreenPos));
                float4 rtRefractions = tex2Dproj(_RefractionTexUnlit_TM, UNITY_PROJ_COORD(refractionScreenPos));
                  
                half3 viewVector = normalize(i.viewInterpolator.xyz);
                float4 cubeMapReflection = GetCubeMapColor(viewVector, normal, _Cube);
     
                rtReflections = lerp(rtReflections, cubeMapReflection, _CubeMapLevel);
                rtReflections = rtReflections * (step((_OneOrZero + 1), 1)) + cubeMapReflection * _OneOrZero;
                
                half refl2Refr = Fresnel(viewVector, UnityObjectToWorldNormal(float3(0,1,0)), _FresnelBias, _FresnelPower);
                refl2Refr = refl2Refr * (step((_OneOrZero + 1), 1)) + _UnderWaterOpacity * _OneOrZero;
                
                half depthWithOffset = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(reflectionScreenPos)));
                depthWithOffset = depthWithOffset * (step((oneOrZero + 1), 1)) + depth * oneOrZero;
                float colorDepth = saturate(_WaterDepth / abs(depthWithOffset - i.screenPos.z));
                float4 colorTint = lerp(_DeepWaterColor, _ShallowWaterColor, colorDepth);
                rtReflections = lerp(colorTint, rtReflections, _Smoothness);
                float4 baseColor = lerp(rtRefractions, rtReflections, refl2Refr) * colorTint;

                float spec = 0;
                #if _SPECULAR_ON
                    spec = GetSpecularLevel(viewVector, worldNormal, _WorldLightDir, _Shininess);
                #endif
                baseColor = baseColor + spec * _SpecularColor;
                
                #if _FOAM_ON
                    float objectZ = i.screenPos.z;
                    float3 foam = Foam(_FoamTex, _FoamGradient, i.foamuv, objectZ, depth, _FoamStrength, bump);
                    baseColor.rgb += foam;
                #endif

                baseColor.a = edgeBlendFactors.x;
                
                UNITY_APPLY_FOG(i.fogCoord, baseColor);

                //#ifdef WATER_COLOR
                //    baseColor = baseColor;                            
                //#endif
                
                //#ifdef WATER_HEIGHTMAP
                //    float4 heightMapColor = tex2D(_MainTex, i.uv);
                //    heightMapColor = float4 (heightMapColor.r, heightMapColor.r, heightMapColor.r, 1);
                //    baseColor = heightMapColor;
                //#endif
                                
                //#ifdef WATER_NORMAL
                //    float3 newNorm = float3(normal.x, normal.z, normal.y);
                //    baseColor.rgb = newNorm * 0.5 + 0.5;
                //#endif
                 
                return baseColor;
            }
		    ENDCG
        }
    }
    CustomEditor "Water2DShaderInspector_TM"
}
