// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BloodShader"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_Maintexture("Main texture", 2D) = "white" {}
		_Power("Power", Float) = 0
		_normal_oil("normal_oil", 2D) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 uv2_tex4coord2;
		};

		uniform sampler2D _normal_oil;
		uniform float4 _normal_oil_ST;
		uniform float4 _Color;
		uniform sampler2D _Maintexture;
		uniform float4 _Maintexture_ST;
		uniform float _Power;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_normal_oil = i.uv_texcoord * _normal_oil_ST.xy + _normal_oil_ST.zw;
			o.Normal = tex2D( _normal_oil, uv_normal_oil ).rgb;
			float2 uv_Maintexture = i.uv_texcoord * _Maintexture_ST.xy + _Maintexture_ST.zw;
			float4 tex2DNode2 = tex2D( _Maintexture, uv_Maintexture );
			float4 temp_output_4_0 = ( i.vertexColor * ( _Color * tex2DNode2 ) );
			o.Emission = temp_output_4_0.rgb;
			clip( ( 1.0 - i.uv2_tex4coord2.x ) - pow( ( 1.0 - tex2DNode2.a ) , _Power ));
			o.Alpha = temp_output_4_0.r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
1920;0;1920;1019;566.779;734.3677;1;True;False
Node;AmplifyShaderEditor.SamplerNode;2;-427.9856,-25.77385;Inherit;True;Property;_Maintexture;Main texture;1;0;Create;True;0;0;False;0;-1;None;6d1e26679ac916d4cbfea89fe49ecb3b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-434.2242,-234.6782;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;12;77.48473,261.2535;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-95.54235,-105.377;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-112.6137,200.8282;Inherit;False;Property;_Power;Power;2;0;Create;True;0;0;False;0;0;0.16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;6;-107.215,88.53565;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;5;-171.1195,-331.0673;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;13;301.485,263.2536;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;48.93664,-156.8575;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;7;57.98434,115.5292;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClipNode;14;484.5142,156.9268;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;16;242.8817,-368.9384;Inherit;True;Property;_normal_oil;normal_oil;3;0;Create;True;0;0;False;0;-1;None;91a8747647c51f0489d7181c21159cee;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;797.9096,-256.8445;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;BloodShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;6;0;2;4
WireConnection;13;0;12;1
WireConnection;4;0;5;0
WireConnection;4;1;3;0
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;14;0;4;0
WireConnection;14;1;13;0
WireConnection;14;2;7;0
WireConnection;0;1;16;0
WireConnection;0;2;4;0
WireConnection;0;9;14;0
ASEEND*/
//CHKSM=DFAFC34195B11619371ECCD516668EB3BF193832