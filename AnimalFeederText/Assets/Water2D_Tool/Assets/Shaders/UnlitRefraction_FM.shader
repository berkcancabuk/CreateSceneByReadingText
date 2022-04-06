Shader "Water2D/UnlitRefraction_FM" {
    Properties {
	     [HideInInspector] _MainTex ("Main Texture", 2D) = "white" {}
	    _BumpMap ("Normals", 2D) = "bump" {}
	    _BumpWaves("Bump Waves", Float) = 0.3
	    _Distortion("Distortion", Float) = 0.15
	    _BumpTiling ("Bump Tiling", Vector) = (0.04 ,0.04, 0.04, 0.08)
	    _BumpDirection ("Bump Direction & Speed", Vector) = (1.0 ,30.0, 20.0, -20.0)
	    _BaseColor ("Base Color", COLOR)  = (.17, .25, .2, 0.5)

        _WaterLineColor ("Water Line Color", Color) = (1, 1, 1, 1)
        [NoScaleOffset] _WaterLineTex ("Water Line Texture", 2D) = "white" {}
        _PixelsPerUnit("Pixels Per Unit", Float) = 100

        [HideInInspector] _WaterHeight (" ", Float) = 10
        [HideInInspector] _WaterWidth (" ", Float) = 10
	}

	Subshader
	{
		Tags {"RenderType"="Transparent" "Queue"="Transparent+10"}
		ColorMask RGB
        Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual
		GrabPass {"_RefractionTex_V"}
	
		Pass {	
			CGPROGRAM
			#pragma target 3.0	
			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"
            #include "Water2DInclude.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                fixed2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed2 texcoord : TEXCOORD0;
                float4 bumpCoords : TEXCOORD2;
                float4 grabPassPos : TEXCOORD3;
            };

            sampler2D _RefractionTex_V;
            float4 _BaseColor;
            fixed4 _MainTex_ST;
	
            v2f vert(appdata_full v)
            {
                v2f o;
		
                half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
                half2 tileableUv = worldSpaceVertex.xy;

                o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                #if UNITY_UV_STARTS_AT_TOP
				    float scale = -1.0;
                #else
                    float scale = 1.0f;
                #endif

                o.grabPassPos.xy = (float2(o.pos.x, o.pos.y * scale) + o.pos.w) * 0.5;
                o.grabPassPos.zw = o.pos.zw;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 bump = (UnpackNormal(tex2D(_BumpMap, i.bumpCoords.xy)) + UnpackNormal(tex2D(_BumpMap, i.bumpCoords.zw))) * 0.5;
                half3 worldN = half3(0, 1, 0) + bump.xxy * _BumpWaves * half3(1, 0, 1);
                half3 worldNormal = normalize(worldN);

                half4 distortOffset = half4(worldNormal.xz * _Distortion * 10.0, 0, 0);
                half4 grabWithOffset = i.grabPassPos + distortOffset;
                half4 rtRefractions = tex2Dproj(_RefractionTex_V, UNITY_PROJ_COORD(grabWithOffset));
		
                half4 baseColor = _BaseColor;
                baseColor = lerp(rtRefractions, baseColor, baseColor.a);
                baseColor = AddWaterLine(i.texcoord, baseColor);
                baseColor.a = 1;

                return baseColor;
            }
		
			ENDCG
		}
	}
	Fallback "Transparent/Diffuse"
}
