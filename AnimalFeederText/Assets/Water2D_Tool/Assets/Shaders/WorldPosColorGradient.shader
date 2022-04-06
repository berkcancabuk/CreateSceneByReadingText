// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Water2D/WorldPosColorGradient"
{
	Properties
	{
		_TopColor("Top Color", Color) = (0,0.6397059,0.05735293,0)
		_BottomColor("Bottom Color", Color) = (0.5912272,0.6544118,0,0)
		_TopPoint("Top Point", Float) = 1
		_BottomPoint("Bottom Point", Float) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _TopColor;
		uniform float4 _BottomColor;
		uniform float _TopPoint;
		uniform float _BottomPoint;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float4 lerpResult10 = lerp( _TopColor , _BottomColor , saturate( ( ( _TopPoint - ase_worldPos.y ) / ( _TopPoint - _BottomPoint ) ) ));
			o.Albedo = lerpResult10.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
7;29;1906;1004;2822.998;545.175;1.160293;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-1890.035,200.61;Float;False;Property;_BottomPoint;Bottom Point;3;0;Create;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;5;-1881.234,-69.11407;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;3;-1867.516,-285.4967;Float;False;Property;_TopPoint;Top Point;2;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;7;-1445.903,47.2142;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;6;-1453.696,-132.2941;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;8;-1204.203,-35.97263;Float;True;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-501.6213,-563.3088;Float;False;Property;_TopColor;Top Color;0;0;Create;0,0.6397059,0.05735293,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-503.9213,-348.9231;Float;False;Property;_BottomColor;Bottom Color;1;0;Create;0.5912272,0.6544118,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;9;-894.3784,-62.36882;Float;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;322.0849,-13.3505;Float;False;Property;_Metallic;Metallic;5;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;351.5558,158.6918;Float;False;Property;_Smoothness;Smoothness;4;0;Create;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;31.31603,-174.1957;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;877.5589,-130.0517;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Water2D/WorldPosColorGradient;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;3;0
WireConnection;7;1;4;0
WireConnection;6;0;3;0
WireConnection;6;1;5;2
WireConnection;8;0;6;0
WireConnection;8;1;7;0
WireConnection;9;0;8;0
WireConnection;10;0;1;0
WireConnection;10;1;2;0
WireConnection;10;2;9;0
WireConnection;0;0;10;0
WireConnection;0;3;11;0
WireConnection;0;4;12;0
ASEEND*/
//CHKSM=0D4783A59C349B23E254E1D90554DA412B4F20F3