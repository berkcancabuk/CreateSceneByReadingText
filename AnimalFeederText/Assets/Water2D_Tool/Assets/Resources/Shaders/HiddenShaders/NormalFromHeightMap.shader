Shader "Hidden/NormalFromHeightMap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "grey" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
                float rightTexel = tex2Dlod(_MainTex, float4(i.uv.x + _MainTex_TexelSize.x, i.uv.y, 0, 0)).r * 20.0 - 10.0;
                float leftTexel = tex2Dlod(_MainTex, float4(i.uv.x - _MainTex_TexelSize.x, i.uv.y, 0, 0)).r * 20.0 - 10.0;
                float topTexel = tex2Dlod(_MainTex, float4(i.uv.x, i.uv.y - _MainTex_TexelSize.y, 0, 0)).r * 20.0 - 10.0;
                float bottomTexel = tex2Dlod(_MainTex, float4(i.uv.x, i.uv.y + _MainTex_TexelSize.y, 0, 0)).r * 20.0 - 10.0;
			
                float3 dx = normalize(float3(1.0, leftTexel - rightTexel, 0.0));
                float3 dy = normalize(float3(0.0, bottomTexel - topTexel, 1.0));
		  
                float3 normal = normalize(float3(1, 1, 1));
                normal.xz = cross(dx, dy).xz;            
                normal = normalize(normal);

                return float4((normal.x * 0.5) + 0.5, (normal.z * 0.5) + 0.5, (normal.y * 0.5) + 0.5, 1.0);
            }
			ENDCG
		}
	}
}
