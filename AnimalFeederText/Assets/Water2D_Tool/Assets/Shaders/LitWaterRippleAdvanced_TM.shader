Shader"Water2D/LitWaterRippleAdvanced_TM" {
	Properties {
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "grey" {}
        [HideInInspector] _ReflectionTex ("", 2D) = "white" {}

        _DeepWaterColor ("Deep Water Color", COLOR) = (1, 1, 1, 1)
        _ShallowWaterColor ("Shalow Water Color", Color) = (0.73, 0.92, 0.99, 1)
        _UnderWaterOpacity ("Under Water Opacity", Range (0.0, 1.0)) = 0.4
		[KeywordEnum(PerPixel, PerVertex)] _Normals("Normals", Float)  = 0
        _NormalStrength ("Normal Strength", Float) = 15
        _CubeMapLevel ("Sky Box Reflection", Range (0.0, 1.0)) = 0.2
        _EdgeBlend ("Edge Blend", Float) = 4.0
        _WaterDepth ("Water Depth", Float) = 0.6
        _Smoothness ("Reflection Intensity", Range (0.0, 1.0)) = 0.8

        _FresnelPower  ("Fresnel Power", Float) = 0.2
        _FresnelBias  ("Fresnel Bias", Float) = 0.2
        
        [Toggle(_SPECULAR_ON)] _SPECULAR ("Specular Reflection", Float) = 0
	    _SpecularColor ("Specular Color", COLOR)  = ( .72, .72, .72, 1)
	    _WorldLightDir ("Specular Light Direction", Vector) = (0.0, 0.1, -0.5, 0.0)
	    _Shininess ("Shininess", Range (2.0, 500.0)) = 200.0
		
        _Distortion("Distortion", Range (0.0, 1.5)) = 0.168
		_BumpTiling ("Bump Tiling", Vector) = (0.04 ,0.04, 0.04, 0.08)
		_BumpDirection ("Bump Speed (Map1 x, y, Map2 z, w)", Vector) = (1.0 ,30.0, 20.0, -20.0)
		[NoScaleOffset] _BumpMap ("Bump Normals", 2D) = "bump" {}
        [NoScaleOffset] _Cube ("Cube Map", CUBE) = "black" {}

        [Toggle(_FOAM_ON)] _FOAM ("Enable Foam", Float) = 0
        _FoamColor("FoamColor", Color) = (1, 1, 1, 1)
	    [NoScaleOffset] _FoamTex ("Foam Texture", 2D) = "white" {}
	    [NoScaleOffset] _FoamGradient ("Foam Gradient ", 2D) = "white" {}
	    _FoamStrength ("Foam Strength", float) = 0.4

        [HideInInspector] _WaveHeightScale ("Wave Height Scale", Float) = 1.0
        [HideInInspector] _FaceCulling ("", Float) = 2
        [HideInInspector] _OneOrZero ("One Or Zero", Float) = 0
        [HideInInspector] _HeightOffset (" ", Float) = 0
        [HideInInspector] _ApplyOffset (" ", Float) = 0.0
        [HideInInspector] _BottomPos (" ", Float) = -2

	}

	SubShader {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" "IgnoreProjector" = "True" }
        ZTest LEqual
        Cull[_FaceCulling]
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On

        GrabPass {"_RefractionTexLit_TM"}
		
		CGPROGRAM
        #pragma surface surf BlinnPhong vertex:vert noshadow keepalpha
		#pragma target 4.0
        //#pragma multi_compile WATER_HEIGHTMAP WATER_NORMAL WATER_COLOR
        #pragma shader_feature _NORMALS_PERPIXEL _NORMALS_PERVERTEX
        #pragma shader_feature _ _FOAM_ON
        #pragma shader_feature _ _SPECULAR_ON

        #include "UnityCG.cginc"
        #include "Water2DInclude.cginc"

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float4 screenPos : TEXCOORD1;
            float4 grabPassPos : TEXCOORD2;
            float3 viewInterpolator : TEXCOORD3;
            float2 bumpuv0 : TEXCOORD4;
            float2 bumpuv1 : TEXCOORD5;
            #if _FOAM_ON
                float2 foamuv : TEXCOORD6;
            #endif
            #if _NORMALS_PERVERTEX
                float3 vertNormal : TEXCOORD7;
            #endif
            INTERNAL_DATA
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float offset = GetAverage(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0));
            float oneOrZero = step(_BottomPos + 0.001, v.vertex.y) * _ApplyOffset;
            v.vertex.y += (offset * _WaveHeightScale * oneOrZero) + (_HeightOffset * oneOrZero);

            #if _NORMALS_PERVERTEX
                o.vertNormal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0), _NormalStrength);
            #endif

            float4 pos = UnityObjectToClipPos(v.vertex);

            o.screenPos = ComputeScreenPos(pos);
            COMPUTE_EYEDEPTH(o.screenPos.w);
            o.grabPassPos = ComputeGrabScreenPos(pos);
    
            half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
            o.viewInterpolator.xyz = worldSpaceVertex - _WorldSpaceCameraPos;

            float4 temp;
            float4 wpos = mul(unity_ObjectToWorld, v.vertex);
            temp.xyzw = wpos.xzxz * _BumpTiling.xyzw + _Time.xxxx * 0.1 * _BumpDirection.xyzw;
            o.bumpuv0 = temp.xy;
            o.bumpuv1 = temp.wz;

            #if _FOAM_ON
                o.foamuv = 7.0f * wpos.xz + 0.05 * float2(_SinTime.w, _SinTime.w);
            #endif
        }

        void surf(Input IN, inout SurfaceOutput o)
        {		
            float4 reflectionScreenPos = IN.screenPos;
            reflectionScreenPos = float4(reflectionScreenPos.xyz, reflectionScreenPos.w + 0.00000000001);
            float4 refractionScreenPos = IN.grabPassPos;

            float3 normal = float3(0, 1, 0);
            
            #if _NORMALS_PERPIXEL
                normal = NormalFromHeightmap(_MainTex, _MainTex_TexelSize, float4(IN.uv_MainTex, 0, 0), _NormalStrength);
            #endif       
            
            #if _NORMALS_PERVERTEX 
                normal = IN.vertNormal;      
            #endif
    
            float3 worldNormal = UnityObjectToWorldNormal(normal);

            half3 bump1 = UnpackNormal(tex2D(_BumpMap, IN.bumpuv0)).rgb;
            half3 bump2 = UnpackNormal(tex2D(_BumpMap, IN.bumpuv1)).rgb;
            half3 bump = (bump1 + bump2) * 0.5;
            
            half2 distortOffset = half2(bump.xy * _Distortion);
            distortOffset = lerp(float2(0, 0), distortOffset, dot(normal, float3(0, 1, 0)));
            	
            half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
            float uvOffsetLevel = 1;

			float depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture , UNITY_PROJ_COORD(reflectionScreenPos));
            depth = LinearEyeDepth(depth);
			        
            edgeBlendFactors = saturate(_EdgeBlend * (depth - IN.screenPos.w));
            uvOffsetLevel = saturate(_WaterDepth * (depth - IN.screenPos.w));
                    
            reflectionScreenPos.xy += lerp(float2(0, 0), (normal.xz * _WaveHeightScale) / (IN.screenPos.w + 0.00000000001) + distortOffset.xy, uvOffsetLevel);
            refractionScreenPos.xy += lerp(float2(0, 0), (normal.xz * _WaveHeightScale) / IN.grabPassPos.w + distortOffset.xy, uvOffsetLevel);
                    
            float currentPixelDepth = IN.screenPos.w;
            float offsetPixelDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture , UNITY_PROJ_COORD(reflectionScreenPos)));
                    
            float oneOrZero = step(offsetPixelDepth, currentPixelDepth);
            refractionScreenPos.xy = refractionScreenPos.xy * (step((oneOrZero + 1), 1)) + IN.grabPassPos.xy * oneOrZero;
        
            float4 rtReflections = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(reflectionScreenPos));
            float4 rtRefractions = tex2Dproj(_RefractionTexLit_TM, UNITY_PROJ_COORD(refractionScreenPos));
                 
            half3 viewVector = normalize(IN.viewInterpolator.xyz);
            float4 cubeMapReflection = GetCubeMapColor(viewVector, normal, _Cube);
            
            rtReflections = lerp(rtReflections, cubeMapReflection, _CubeMapLevel);
            rtReflections = rtReflections * (step((_OneOrZero + 1), 1)) + cubeMapReflection * _OneOrZero;

            half refl2Refr = Fresnel(viewVector, UnityObjectToWorldNormal(float3(0, 1, 0)), _FresnelBias, _FresnelPower);
            refl2Refr = refl2Refr * (step((_OneOrZero + 1), 1)) + _UnderWaterOpacity * _OneOrZero;


            half depthWithOffset = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(reflectionScreenPos)));
            depthWithOffset = depthWithOffset * (step((oneOrZero + 1), 1)) + depth * oneOrZero;
            float colorDepth = saturate(_WaterDepth / abs(depthWithOffset - IN.screenPos.w));
            float4 colorTint = lerp(_DeepWaterColor, _ShallowWaterColor, colorDepth);
            rtReflections = lerp(colorTint, rtReflections, _Smoothness);
            half4 baseColor = lerp(rtRefractions, rtReflections, refl2Refr) * colorTint;
  
            float spec = 0;
            #if _SPECULAR_ON
                spec  = GetSpecularLevel(viewVector, worldNormal, _WorldLightDir, _Shininess);
            #endif
            baseColor = baseColor + spec * _SpecularColor;
            
            #if _FOAM_ON
                float objectZ = IN.screenPos.w;
                float3 foam = Foam(_FoamTex, _FoamGradient, IN.foamuv, objectZ, depth, _FoamStrength, bump);
                baseColor.rgb += foam * _FoamColor;
            #endif
            
            //#ifdef WATER_COLOR
            //    baseColor = baseColor;                            
            //#endif
                
            //#ifdef WATER_HEIGHTMAP
            //    float4 heightMapColor = tex2D(_MainTex, IN.uv_MainTex);
            //    heightMapColor = float4 (heightMapColor.r, heightMapColor.r, heightMapColor.r, 1);
            //    baseColor = heightMapColor;
            //#endif
                                
            //#ifdef WATER_NORMAL
            //    float3 newNorm = float3(normal.x, normal.z, normal.y);
            //    baseColor.rgb = newNorm * 0.5 + 0.5;
            //#endif

            o.Albedo = baseColor.rgb;
            o.Alpha = edgeBlendFactors.x;
        }
		ENDCG
	}
    CustomEditor"Water2DShaderInspector_TM"
    FallBack "Diffuse"
}
