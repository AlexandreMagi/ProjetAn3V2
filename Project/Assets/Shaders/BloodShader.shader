// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BloodShader"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_Maintexture("Main texture", 2D) = "white" {}
		_Power("Power", Float) = 0
		_normal_blood("normal_blood", 2D) = "bump" {}
		_NoiseAmount("NoiseAmount", Range( 0 , 1)) = 0
		_Noisescale("Noise scale", Float) = 10
		_Noisespeed("Noise speed", Vector) = (0,0.3,0,0)
		_Emissive("Emissive", Color) = (1,1,1,0)
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 uv2_tex4coord2;
		};

		uniform sampler2D _normal_blood;
		uniform float4 _normal_blood_ST;
		uniform float4 _Emissive;
		uniform float4 _Color;
		uniform sampler2D _Maintexture;
		uniform float2 _Noisespeed;
		uniform float _Noisescale;
		uniform float _NoiseAmount;
		uniform float _Power;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_normal_blood = i.uv_texcoord * _normal_blood_ST.xy + _normal_blood_ST.zw;
			float dotResult33 = dot( float4( normalize( (WorldNormalVector( i , UnpackNormal( tex2D( _normal_blood, uv_normal_blood ) ) )) ) , 0.0 ) , _Emissive );
			float2 uv_TexCoord24 = i.uv_texcoord + ( _Time.y * _Noisespeed );
			float simplePerlin2D23 = snoise( uv_TexCoord24*_Noisescale );
			simplePerlin2D23 = simplePerlin2D23*0.5 + 0.5;
			float2 temp_cast_1 = (simplePerlin2D23).xx;
			float2 lerpResult20 = lerp( i.uv_texcoord , temp_cast_1 , _NoiseAmount);
			float4 tex2DNode2 = tex2D( _Maintexture, lerpResult20 );
			float4 temp_output_4_0 = ( i.vertexColor * ( _Color * tex2DNode2 ) );
			o.Emission = ( dotResult33 * temp_output_4_0 ).rgb;
			clip( ( 1.0 - i.uv2_tex4coord2.x ) - pow( ( 1.0 - tex2DNode2.r ) , _Power ));
			o.Alpha = temp_output_4_0.r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
0;73;1312;396;3092.484;792.3412;3.619245;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;25;-1871.847,95.36924;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;29;-1829.184,205.3742;Inherit;False;Property;_Noisespeed;Noise speed;6;0;Create;True;0;0;False;0;0,0.3;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-1631.584,132.5741;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1324.903,202.5201;Inherit;False;Property;_Noisescale;Noise scale;5;0;Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-1437.269,37.76787;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-1161.655,73.40037;Inherit;False;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-1179.579,321.6123;Inherit;False;Property;_NoiseAmount;NoiseAmount;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-1155.393,-118.783;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;20;-890.5959,55.65466;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-642.851,13.65102;Inherit;True;Property;_Maintexture;Main texture;1;0;Create;True;0;0;False;0;-1;f75fbd312ae03644286adea617c7a663;f75fbd312ae03644286adea617c7a663;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-486.2235,-564.7796;Inherit;True;Property;_normal_blood;normal_blood;3;0;Create;True;0;0;False;0;-1;a29b4c7543d5835448cfe3251cd0f427;a29b4c7543d5835448cfe3251cd0f427;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-649.0896,-195.2534;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-310.4078,-65.95216;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;5;-385.985,-291.6425;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;31;-124.3838,-496.9539;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;40;-84.68554,-327.7885;Inherit;False;Property;_Emissive;Emissive;7;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;6;-322.0805,127.9605;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-327.4792,240.2531;Inherit;False;Property;_Power;Power;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;12;77.48473,261.2535;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;7;-156.8811,154.954;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;33;213.9128,-395.2264;Inherit;False;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-165.9288,-117.4326;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;13;301.485,263.2536;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;406.1,-206.8221;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClipNode;14;484.5142,156.9268;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;797.9096,-256.8445;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;BloodShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;30;0;25;0
WireConnection;30;1;29;0
WireConnection;24;1;30;0
WireConnection;23;0;24;0
WireConnection;23;1;28;0
WireConnection;20;0;21;0
WireConnection;20;1;23;0
WireConnection;20;2;26;0
WireConnection;2;1;20;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;31;0;16;0
WireConnection;6;0;2;1
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;33;0;31;0
WireConnection;33;1;40;0
WireConnection;4;0;5;0
WireConnection;4;1;3;0
WireConnection;13;0;12;1
WireConnection;37;0;33;0
WireConnection;37;1;4;0
WireConnection;14;0;4;0
WireConnection;14;1;13;0
WireConnection;14;2;7;0
WireConnection;0;2;37;0
WireConnection;0;9;14;0
ASEEND*/
//CHKSM=558EEA0E21351705303B66E2FB95EFA847672EDB