// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Custom Shader/MaskPosition"
{
	Properties
	{
		_HitPosition("Hit Position", Vector) = (0,0,0,0)
		_HitColor1("Hit Color", Color) = (1,1,1,1)
		_HitSize1("Hit Size", Float) = 5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _HitColor1;
		uniform float _HitSize1;
		uniform float3 _HitPosition;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 HitColor14 = _HitColor1;
			float HitSize13 = _HitSize1;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 hit29 = saturate( ( HitColor14 * ( HitSize13 / distance( ase_vertex3Pos , _HitPosition ) ) ) );
			o.Albedo = hit29.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
563;284;1437;594;2873.585;1507.425;3.108895;True;False
Node;AmplifyShaderEditor.CommentaryNode;7;-2224.686,-1182.323;Inherit;False;1356.619;764.8547;Comment;13;29;33;22;17;15;9;19;14;12;13;11;10;8;Impact Effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-2094.78,-1132.323;Float;False;Property;_HitSize1;Hit Size;2;0;Create;True;0;0;False;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-2136.468,-1025.422;Float;False;Property;_HitColor1;Hit Color;1;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1881.581,-1128.423;Float;False;HitSize;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;9;-2174.686,-770.7226;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;10;-2165.079,-589.4228;Float;False;Property;_HitPosition;Hit Position;0;0;Create;True;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DistanceOpNode;12;-1934.481,-687.1226;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-1855.298,-1022.87;Float;False;HitColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;-1840.294,-780.9553;Inherit;False;13;HitSize;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;17;-1609.401,-809.0696;Inherit;False;14;HitColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;19;-1648.291,-697.0535;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1422.301,-790.3698;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;33;-1247.87,-820.6651;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-1068.997,-918.0972;Float;False;hit;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-1330.73,-267.6144;Inherit;False;29;hit;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1084.724,-285.0058;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_Custom Shader/MaskPosition;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;8;0
WireConnection;12;0;9;0
WireConnection;12;1;10;0
WireConnection;14;0;11;0
WireConnection;19;0;15;0
WireConnection;19;1;12;0
WireConnection;22;0;17;0
WireConnection;22;1;19;0
WireConnection;33;0;22;0
WireConnection;29;0;33;0
WireConnection;0;0;31;0
ASEEND*/
//CHKSM=0FCE48DF4BCF589B881FA753AEE70CBFDD8468A2