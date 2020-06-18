// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FlagShader"
{
	Properties
	{
		_Diffuse("Diffuse", 2D) = "white" {}
		[HDR]_Basecolor("Base color", Color) = (0,0,0,0)
		_Emissionpower("Emission power", Float) = 0
		_Diffusetextureoffset("Diffuse texture offset", Vector) = (0,0,0,0)
		_Diffusetexturetilling("Diffuse texture tilling", Vector) = (0,0,0,0)
		_Degres("Degres", Float) = 0
		_Flowgradient("Flow gradient", 2D) = "white" {}
		_Flowspeed("Flow speed", Vector) = (0,0,0,0)
		_OffsetMultiplier("OffsetMultiplier", Vector) = (0,0,0,0)
		_Noisetexture("Noise texture", 2D) = "white" {}
		_Lerppower("Lerp power", Range( 0 , 1)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float2 _OffsetMultiplier;
		uniform sampler2D _Flowgradient;
		uniform float2 _Flowspeed;
		uniform sampler2D _Noisetexture;
		uniform float _Lerppower;
		uniform sampler2D _Diffuse;
		uniform float2 _Diffusetexturetilling;
		uniform float2 _Diffusetextureoffset;
		uniform float _Degres;
		uniform float4 _Basecolor;
		uniform float _Emissionpower;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float temp_output_4_0 = ( _Time.y * _Flowspeed.y );
			float2 temp_cast_1 = (temp_output_4_0).xx;
			float2 uv_TexCoord2 = v.texcoord.xy + temp_cast_1;
			float2 temp_cast_2 = (( 1.0 - temp_output_4_0 )).xx;
			float2 uv_TexCoord53 = v.texcoord.xy + temp_cast_2;
			float4 lerpResult50 = lerp( tex2Dlod( _Flowgradient, float4( uv_TexCoord2, 0, 0.0) ) , tex2Dlod( _Noisetexture, float4( uv_TexCoord53, 0, 0.0) ) , _Lerppower);
			float4 vertexOffset13 = ( ( float4( _OffsetMultiplier, 0.0 , 0.0 ) * lerpResult50 ) * ( 1.0 - v.texcoord.xy.y ) );
			v.vertex.xyz += vertexOffset13.rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord30 = i.uv_texcoord * _Diffusetexturetilling + _Diffusetextureoffset;
			float cos31 = cos( radians( _Degres ) );
			float sin31 = sin( radians( _Degres ) );
			float2 rotator31 = mul( uv_TexCoord30 - float2( 0.5,0.5 ) , float2x2( cos31 , -sin31 , sin31 , cos31 )) + float2( 0.5,0.5 );
			float4 tex2DNode16 = tex2D( _Diffuse, rotator31 );
			float4 albedo21 = ( tex2DNode16 * ( ( _Basecolor * ( 1.0 - tex2DNode16.a ) ) * tex2DNode16.a ) );
			o.Albedo = albedo21.rgb;
			o.Emission = ( albedo21 * _Emissionpower ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
1920;0;1920;1019;4274.582;1469.03;3.348417;True;False
Node;AmplifyShaderEditor.CommentaryNode;42;-2796.931,-570.5579;Inherit;False;2156.087;586.0292;Comment;14;21;66;62;63;19;16;31;36;30;34;35;32;37;68;Diffuse texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;12;-2810.559,319.5539;Inherit;False;2565.678;518.5087;;16;13;11;9;38;10;40;1;2;4;3;6;48;50;51;52;53;Vertex offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;37;-2746.931,-497.257;Inherit;False;Property;_Diffusetexturetilling;Diffuse texture tilling;4;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;32;-2722.55,-334.0575;Inherit;False;Property;_Diffusetextureoffset;Diffuse texture offset;3;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;35;-2443.55,-58.0575;Inherit;False;Property;_Degres;Degres;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-2469.55,-446.0575;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;3;-2760.559,539.5583;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;34;-2272.55,-55.0575;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;6;-2743.759,679.1583;Inherit;False;Property;_Flowspeed;Flow speed;7;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;36;-2326.55,-223.0575;Inherit;False;Constant;_Vector1;Vector 1;6;0;Create;True;0;0;False;0;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-2467.559,554.5582;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;31;-2084.55,-257.0575;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;52;-2446.589,715.5876;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;16;-1853.32,-267.0325;Inherit;True;Property;_Diffuse;Diffuse;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;53;-2253.589,610.5876;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;63;-1552.278,-200.0726;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-2227.56,385.5584;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;19;-1843.226,-520.5579;Inherit;False;Property;_Basecolor;Base color;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1375.278,-232.0726;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;48;-1980.858,632.1444;Inherit;True;Property;_Noisetexture;Noise texture;9;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1984.552,429.5539;Inherit;True;Property;_Flowgradient;Flow gradient;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;-1582.858,666.1444;Inherit;False;Property;_Lerppower;Lerp power;10;0;Create;True;0;0;False;0;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;40;-1625.949,352.9146;Inherit;False;Property;_OffsetMultiplier;OffsetMultiplier;8;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexCoordVertexDataNode;10;-1185.629,647.4885;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;50;-1372.858,518.1444;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-1190.771,-192.3517;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;9;-957.6287,650.4885;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1061.7,502.1697;Inherit;False;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-1063.278,-363.0726;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-869.342,-367.13;Inherit;False;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-793.629,577.4883;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;15;-54.71039,-98;Inherit;False;957.263;652.625;Comment;6;0;14;46;22;45;47;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;165.2703,141.7312;Inherit;False;21;albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;47;176.2,245.0682;Inherit;False;Property;_Emissionpower;Emission power;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-616.0344,576.1387;Inherit;False;vertexOffset;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;341.0071,27.45013;Inherit;False;21;albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;392.2,168.068;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;270.6133,396.1409;Inherit;False;13;vertexOffset;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;571.2056,72.78796;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;FlagShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;0;37;0
WireConnection;30;1;32;0
WireConnection;34;0;35;0
WireConnection;4;0;3;0
WireConnection;4;1;6;2
WireConnection;31;0;30;0
WireConnection;31;1;36;0
WireConnection;31;2;34;0
WireConnection;52;0;4;0
WireConnection;16;1;31;0
WireConnection;53;1;52;0
WireConnection;63;0;16;4
WireConnection;2;1;4;0
WireConnection;62;0;19;0
WireConnection;62;1;63;0
WireConnection;48;1;53;0
WireConnection;1;1;2;0
WireConnection;50;0;1;0
WireConnection;50;1;48;0
WireConnection;50;2;51;0
WireConnection;68;0;62;0
WireConnection;68;1;16;4
WireConnection;9;0;10;2
WireConnection;38;0;40;0
WireConnection;38;1;50;0
WireConnection;66;0;16;0
WireConnection;66;1;68;0
WireConnection;21;0;66;0
WireConnection;11;0;38;0
WireConnection;11;1;9;0
WireConnection;13;0;11;0
WireConnection;46;0;45;0
WireConnection;46;1;47;0
WireConnection;0;0;22;0
WireConnection;0;2;46;0
WireConnection;0;11;14;0
ASEEND*/
//CHKSM=CE7EED15ACE4BAB6D8F9248FCEF90609C63A164D