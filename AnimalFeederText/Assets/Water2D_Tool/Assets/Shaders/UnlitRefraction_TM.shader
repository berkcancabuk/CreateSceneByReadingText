Shader"Water2D/UnlitRefraction_TM" {
	Properties {
		_BumpMap ("Normals ", 2D) = "bump" {}
		_BumpWaves("Bump waves", Float) = 1.3
		_Distortion("Distortion", Float) = 0.17
		_BumpTiling ("Bump Tiling", Vector) = (0.04 ,0.04, 0.04, 0.08)
		_BumpDirection ("Bump Direction & Speed", Vector) = (1.0 ,30.0, 20.0, -20.0)
		_BaseColor ("Base Color", COLOR)  = ( .17, .25, .2, 0.5)
	}

	Subshader
	{
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual
	    ZWrite Off
		Cull Off
		ColorMask RGB
	
		GrabPass { "_RefractionTex_H" }
	
		Pass {
			CGPROGRAM
			#pragma target 3.0	
			#pragma vertex vert
			#pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 bumpCoords : TEXCOORD2;
                float4 grabPassPos : TEXCOORD4;
            };

            sampler2D _BumpMap;
            sampler2D _RefractionTex_H;
            float4 _BaseColor;

            float _BumpWaves;
            float _Distortion;
            float4 _BumpTiling;
            float4 _BumpDirection;
	
            v2f vert(appdata_full v)
            {
                v2f o;
		
                half3 worldSpaceVertex = mul(unity_ObjectToWorld, (v.vertex)).xyz;
                half2 tileableUv = worldSpaceVertex.xz;
	
                o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;
                o.pos = UnityObjectToClipPos(v.vertex);

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
                half4 rtRefractions = tex2Dproj(_RefractionTex_H, UNITY_PROJ_COORD(grabWithOffset));
		
                half4 baseColor = _BaseColor;

                baseColor = lerp(lerp(rtRefractions, baseColor, baseColor.a), half4(1.0, 1.0, 1.0, 1.0), half(0.0));
                baseColor.a = 1;

                return baseColor;
            }
		
			ENDCG
		}
	}
	Fallback "Transparent/Diffuse"
}
