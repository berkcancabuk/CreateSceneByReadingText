Shader "Water2D/LitWaterRippleAdvanced_FM" {
	Properties {
        [HideInInspector] _MainTex ("Main Texture", 2D) = "grey" {}
        _DeepWaterColor ("Deep Water Color", Color) = (0.588, 0.745, 1.0, 1)
        _ShallowWaterColor ("Shallow Water Color", Color) = (0.874, 0.965, 1.0, 1)
        _WaterDepth ("Water Depth", Float) = 1.0
        _EdgeBlend ("Edge Blend", Float) = 4.0
        _Opacity ("Opacity", Range (0.0, 1.0)) = 0.5
        [Toggle(_DEPTHFOG_ON)] _DEPTHFOG ("Enable Depth Fog", Float) = 0
        _FogDepth ("Fog Depth", Float) = 3.0
        _FogFalloff ("Fog Depth", Float) = 0.5
        _FogDensityCap ("Fog Density Cap", Range (0.0, 1.0)) = 0.8
		_BumpWaves("Bump Waves", Float) = 0.15
		_Distortion("Distortion", Float) = 0.15
		_BumpTiling ("Bump Tiling", Vector) = (0.04 ,0.04, 0.04, 0.08)
		_BumpDirection ("Bump Direction & Speed", Vector) = (1.0 ,30.0, 20.0, -20.0)
		[NoScaleOffset] _BumpMap ("Bump Normals", 2D) = "bump" {}
        [NoScaleOffset] _WaterLineTex ("Water Line Texture", 2D) = "white" {}
        _WaterLineColor ("Water Line Color", COLOR)  = ( 1, 1, 1, 1)
        _PixelsPerUnit("Pixels Per Unit", Float) = 100

        [HideInInspector] _WaveHeightScale (" ", Float) = 1.0
        [HideInInspector] _WaterHeight (" ", Float) = 10
        [HideInInspector] _WaterWidth (" ", Float) = 10
        [HideInInspector] _HeightOffset (" ", Float) = 0
        [HideInInspector] _ApplyOffset (" ", Float) = 0.0
        [HideInInspector] _BottomPos (" ", Float) = -2
	}
	SubShader {
		Tags {"RenderType"="Transparent" "Queue"="Transparent+10" "IgnoreProjector" = "True" }
        Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual
		ZWrite Off
	
		GrabPass{"_RefractionTexLit_FM"}
		
		CGPROGRAM
        #pragma surface surf Lambert vertex:vert noshadow keepalpha
		#pragma target 4.0
        #pragma shader_feature _ _DEPTHFOG_ON
        #include "UnityCG.cginc"
        #include "Water2DInclude.cginc"

		struct Input {
            float2 uv_MainTex : TEXCOORD0;
            float4 bumpCoords : TEXCOORD2;
            float4 screenPos : TEXCOORD3;
            float4 grabPassPos : TEXCOORD4;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float offset = GetAverage(_MainTex, _MainTex_TexelSize, float4(v.texcoord.xy, 0, 0));
            float oneOrZero = step(_BottomPos + 0.001, v.vertex.y) * _ApplyOffset;
            v.vertex.y += (offset * _WaveHeightScale * oneOrZero) + (_HeightOffset * oneOrZero);

            half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
            half2 tileableUv = worldSpaceVertex.xy;
            o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;

            float4 pos = UnityObjectToClipPos(v.vertex);

            o.screenPos = ComputeScreenPos(pos);
            COMPUTE_EYEDEPTH(o.screenPos.w);
            o.grabPassPos = ComputeGrabScreenPos(pos);
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            half3 bump = (UnpackNormal(tex2D(_BumpMap, IN.bumpCoords.xy)) + UnpackNormal(tex2D(_BumpMap, IN.bumpCoords.zw))) * 0.5;
            half3 worldN = half3(0, 1, 0) + bump.xxy * _BumpWaves * half3(1, 0, 1);
            half3 worldNormal = normalize(worldN);
            
            half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
 
            float4 screenPos = float4(IN.screenPos.xyz, IN.screenPos.w + 0.00000000001);
            float depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(screenPos));
			depth = LinearEyeDepth(depth);
			edgeBlendFactors = saturate(_EdgeBlend * (depth - IN.screenPos.w));
			edgeBlendFactors.y = 1.0 - edgeBlendFactors.y;
            
            half4 distortOffset = half4(worldNormal.xz * _Distortion * 10.0, 0, 0);
            distortOffset = lerp(half4(0, 0, 0, 0), distortOffset, edgeBlendFactors.x);
            half4 grabWithOffset = IN.grabPassPos + distortOffset;     
            half4 rtRefractions = tex2Dproj(_RefractionTexLit_FM, UNITY_PROJ_COORD(grabWithOffset));

            screenPos.xy += distortOffset.xy;
            float depthWithOffset = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(screenPos)));
            float colorDepth = saturate(_WaterDepth / abs(depthWithOffset - IN.screenPos.w));
            float4 colorTint = lerp(_DeepWaterColor, _ShallowWaterColor, colorDepth);
            float4 baseColor = lerp(rtRefractions, colorTint, _Opacity);

            #ifdef _DEPTHFOG_ON
                float fogDepth = saturate(_FogDepth / abs(depthWithOffset - IN.screenPos.w));
                fogDepth = saturate(pow(fogDepth, _FogFalloff));
                float fogDensity = max(fogDepth, 1.0 - _FogDensityCap);
                baseColor = lerp(baseColor, _DeepWaterColor, (1.0 - fogDensity));
            #endif
            
            baseColor = AddWaterLine(IN.uv_MainTex, baseColor);

            o.Albedo = baseColor.rgb;
            o.Alpha = edgeBlendFactors.x;
        }
		ENDCG
	}
    CustomEditor "Water2DShaderInspector_FM"
}
