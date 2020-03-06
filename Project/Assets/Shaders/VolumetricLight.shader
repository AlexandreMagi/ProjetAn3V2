// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Custom Shader/VolumetricLight"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (0,0,0,0)
		_Opacity("Opacity", Float) = 1
		_Power("Power", Float) = 0
		_Distancefalloff("Distance falloff", Range( 0 , 1)) = 1
		_SmoothFalloff("SmoothFalloff", Float) = 0.59
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			float eyeDepth;
		};

		uniform float4 _Color;
		uniform float _SmoothFalloff;
		uniform float _Opacity;
		uniform float _Power;
		uniform float _Distancefalloff;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 mainColor2 = _Color;
			o.Emission = mainColor2.rgb;
			float bottomFalloff37 = ( i.uv_texcoord.y * _SmoothFalloff );
			float3 objToWorld13 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float clampResult17 = clamp( ( distance( _WorldSpaceCameraPos , objToWorld13 ) - 0.5 ) , 0.0 , 1.0 );
			float distanceWithCamera34 = clampResult17;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV6 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode6 = ( 0.0 + 10.0 * pow( 1.0 - fresnelNdotV6, _Power ) );
			float clampResult7 = clamp( fresnelNode6 , 0.0 , 1.0 );
			float lerpResult8 = lerp( _Opacity , 0.0 , clampResult7);
			float edgeFade31 = lerpResult8;
			float cameraDepthFade46 = (( i.eyeDepth -_ProjectionParams.y - 0.0 ) / 1.0);
			float clampResult51 = clamp( ( cameraDepthFade46 * _Distancefalloff ) , 0.0 , 1.0 );
			float distanceFalloff52 = clampResult51;
			o.Alpha = ( bottomFalloff37 * ( distanceWithCamera34 * ( edgeFade31 * distanceFalloff52 ) ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
563;284;1437;594;3077.316;761.1211;3.736595;True;False
Node;AmplifyShaderEditor.CommentaryNode;30;-2835.237,-29.81713;Inherit;False;1118.831;396.355;Comment;6;31;8;9;7;6;24;Edge fade;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;33;-2844.22,-587.9227;Inherit;False;1224.526;458.3693;Comment;6;34;12;14;13;17;15;Distance with camera;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;45;-2831.835,-1502.574;Inherit;False;918.2714;368.0299;Comment;5;52;51;49;47;46;Distance Falloff;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-2785.238,243.4446;Inherit;False;Property;_Power;Power;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TransformPositionNode;13;-2650.22,-346.9229;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CameraDepthFade;46;-2750.277,-1412.471;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;6;-2587.39,170.3919;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;10;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-2781.835,-1243.544;Inherit;False;Property;_Distancefalloff;Distance falloff;5;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;12;-2785.22,-514.9229;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-2481.498,-1355.219;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2286.865,20.1829;Inherit;False;Property;_Opacity;Opacity;1;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;7;-2317.315,176.2392;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;14;-2384.221,-430.923;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;51;-2345.277,-1346.471;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;15;-2198.695,-420.5537;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;8;-2139.865,146.1832;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;36;-1060.83,-1586.487;Inherit;False;661.5032;380.4963;Comment;4;62;37;77;63;bottomFallof;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-1010.205,-1335.678;Inherit;False;Property;_SmoothFalloff;SmoothFalloff;6;0;Create;True;0;0;False;0;0.59;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;17;-2025.695,-349.5536;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-2193.563,-1356.98;Inherit;False;distanceFalloff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;5;-793.2382,-50;Inherit;False;1582.201;646.4595;Comment;11;0;87;4;23;38;85;18;35;11;53;32;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;77;-1004.423,-1528.792;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-1954.675,153.7368;Inherit;False;edgeFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-1858.113,-314.2856;Inherit;False;distanceWithCamera;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;3;-2864.686,-992.2612;Inherit;False;559.9308;262;Comment;2;1;2;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-779.2048,-1386.678;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;-725.2949,378.255;Inherit;False;31;edgeFade;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-737.5782,486.9994;Inherit;False;52;distanceFalloff;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-2814.686,-942.2612;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;35;-577.3524,263.2442;Inherit;False;34;distanceWithCamera;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-607.4602,-1364.92;Inherit;False;bottomFalloff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-512.944,422.0331;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;2;-2528.755,-932.8412;Inherit;False;mainColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;39;-1091.94,-980.0589;Inherit;False;1345.206;318.9902;Comment;6;50;48;44;43;41;40;Edge detection;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-314.3412,281.3773;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-363.6438,161.6351;Inherit;False;37;bottomFalloff;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;87;145.3562,309.6351;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-136.6001,248.6001;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;4;81.22148,41.80614;Inherit;False;2;mainColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;41;-783.8674,-910.6261;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;42.02227,-871.8851;Inherit;False;softParticles;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-437.504,-872.6581;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-1050.812,-905.0566;Inherit;False;Property;_Softparticlesdistance;Soft particles distance;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-755.6546,-760.1313;Inherit;False;Property;_Softparticlespower;Soft particles power;4;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;48;-199.0032,-863.1853;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;38;-103.7884,110.7232;Inherit;False;50;softParticles;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;526.9002,34.17507;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;_Custom Shader/VolumetricLight;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;3;24;0
WireConnection;49;0;46;0
WireConnection;49;1;47;0
WireConnection;7;0;6;0
WireConnection;14;0;12;0
WireConnection;14;1;13;0
WireConnection;51;0;49;0
WireConnection;15;0;14;0
WireConnection;8;0;9;0
WireConnection;8;2;7;0
WireConnection;17;0;15;0
WireConnection;52;0;51;0
WireConnection;31;0;8;0
WireConnection;34;0;17;0
WireConnection;62;0;77;2
WireConnection;62;1;63;0
WireConnection;37;0;62;0
WireConnection;11;0;32;0
WireConnection;11;1;53;0
WireConnection;2;0;1;0
WireConnection;18;0;35;0
WireConnection;18;1;11;0
WireConnection;87;1;38;0
WireConnection;23;0;85;0
WireConnection;23;1;18;0
WireConnection;41;0;40;0
WireConnection;50;0;48;0
WireConnection;44;0;41;0
WireConnection;44;1;43;0
WireConnection;48;0;44;0
WireConnection;0;2;4;0
WireConnection;0;9;23;0
ASEEND*/
//CHKSM=8A88173EEE4A0A016A1368E6078EA45765F917B2