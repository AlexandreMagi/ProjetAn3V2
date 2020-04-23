// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ElectricityShader"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (1,0,0,0)
		_Scale("Scale", Float) = 5
		_Thickness("Thickness", Float) = 0.5
		_SpeedandDirection("Speed and Direction", Vector) = (0,0.5,0,0)
		_Noisespeed("Noise speed", Vector) = (0.1,-0.5,0,0)
		_ElectricityAmount("ElectricityAmount", Float) = 1
		[Toggle(_USEFADE_ON)] _UseFade("UseFade", Float) = 0
		_FadeAmount("FadeAmount", Float) = 1
		_Gradient("Gradient", 2D) = "white" {}
		_Gradientspeedanddirection("Gradient speed and direction", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USEFADE_ON
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Gradient;
		uniform float2 _Gradientspeedanddirection;
		uniform float4 _Color;
		uniform float _Scale;
		uniform float2 _SpeedandDirection;
		uniform float2 _Noisespeed;
		uniform float _ElectricityAmount;
		uniform float _Thickness;
		uniform float _FadeAmount;


		float2 voronoihash41( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi41( float2 v, float time, inout float2 id, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mr = 0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash41( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = g - f + o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F1;
		}


		float2 voronoihash42( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi42( float2 v, float time, inout float2 id, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mr = 0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash42( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = g - f + o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F1;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 color84 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float2 uv_TexCoord85 = i.uv_texcoord + ( _Time.y * _Gradientspeedanddirection );
			float4 gradient92 = ( color84 * tex2D( _Gradient, uv_TexCoord85 ) );
			float scale19 = _Scale;
			float time15 = _Time.y;
			float time41 = time15;
			float2 uv_TexCoord2 = i.uv_texcoord + ( time15 * _SpeedandDirection );
			float2 coords41 = uv_TexCoord2 * scale19;
			float2 id41 = 0;
			float voroi41 = voronoi41( coords41, time41,id41, 0 );
			float sampleElectricity26 = voroi41;
			float time42 = time15;
			float2 uv_TexCoord14 = i.uv_texcoord + ( time15 * _Noisespeed );
			float2 coords42 = uv_TexCoord14 * scale19;
			float2 id42 = 0;
			float voroi42 = voronoi42( coords42, time42,id42, 0 );
			float sampleNoise24 = voroi42;
			float mainSample31 = ( sampleElectricity26 + sampleNoise24 );
			float2 temp_cast_0 = ((-10.0 + (pow( mainSample31 , _ElectricityAmount ) - 0.0) * (10.0 - -10.0) / (1.0 - 0.0))).xx;
			float2 appendResult10_g1 = (float2(1.0 , _Thickness));
			float2 temp_output_11_0_g1 = ( abs( (temp_cast_0*2.0 + -1.0) ) - appendResult10_g1 );
			float2 break16_g1 = ( 1.0 - ( temp_output_11_0_g1 / fwidth( temp_output_11_0_g1 ) ) );
			float amountAndThickness69 = saturate( min( break16_g1.x , break16_g1.y ) );
			o.Emission = ( gradient92 + ( _Color * amountAndThickness69 ) ).rgb;
			float2 CenteredUV15_g4 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float2 break17_g4 = CenteredUV15_g4;
			float2 appendResult23_g4 = (float2(( length( CenteredUV15_g4 ) * 1.0 * 2.0 ) , ( atan2( break17_g4.x , break17_g4.y ) * 1.0 * ( 1.0 / 6.28318548202515 ) )));
			float clampResult63 = clamp( ( 1.0 - pow( appendResult23_g4.x , _FadeAmount ) ) , 0.0 , 1.0 );
			float mask67 = clampResult63;
			#ifdef _USEFADE_ON
				float staticSwitch77 = ( amountAndThickness69 * mask67 );
			#else
				float staticSwitch77 = amountAndThickness69;
			#endif
			float alpha71 = staticSwitch77;
			o.Alpha = alpha71;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
0;460;1101;539;599.7616;1096.104;1.6;True;False
Node;AmplifyShaderEditor.CommentaryNode;16;-3747.272,-763.8356;Inherit;False;487.4592;174.5454;Comment;2;3;15;Time;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;3;-3697.272,-700.2902;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;25;-3188.748,-737.2762;Inherit;False;1292.442;394.9478;Comment;7;26;20;2;5;17;4;41;Sample electricity;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;22;-3731.996,-506.0562;Inherit;False;456.5225;185.1911;Comment;2;6;19;Scale;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;23;-3199.48,-1216.248;Inherit;False;1259.018;382.4685;Comment;6;11;21;18;14;13;24;Sample noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-3483.813,-713.8356;Inherit;False;time;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;4;-3138.748,-592.7982;Inherit;False;Property;_SpeedandDirection;Speed and Direction;3;0;Create;True;0;0;False;0;0,0.5;0,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;6;-3681.996,-436.8651;Inherit;False;Property;_Scale;Scale;1;0;Create;True;0;0;False;0;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;11;-3149.48,-1070.442;Inherit;False;Property;_Noisespeed;Noise speed;4;0;Create;True;0;0;False;0;0.1,-0.5;0,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;17;-3109.999,-687.2761;Inherit;False;15;time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-3097.924,-1166.248;Inherit;False;15;time;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-2872.288,-1081.256;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-3499.474,-456.0562;Inherit;False;scale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-2861.555,-603.6122;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-2684.525,-1110.66;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;20;-2599.771,-400.4499;Inherit;False;19;scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;21;-2584.594,-955.0294;Inherit;False;19;scale;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-2673.795,-633.0162;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VoronoiNode;41;-2378.019,-604.5944;Inherit;False;0;0;1;0;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.VoronoiNode;42;-2416.62,-1040.863;Inherit;False;0;0;1;0;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.CommentaryNode;30;-1827.172,-1184.797;Inherit;False;786.9774;270.4161;Comment;4;29;28;27;31;Sample blend;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-2104.955,-541.2651;Inherit;False;sampleElectricity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-2159.65,-1023.51;Inherit;False;sampleNoise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;29;-1777.172,-1038.175;Inherit;False;24;sampleNoise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;28;-1770.271,-1134.797;Inherit;False;26;sampleElectricity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;64;-3105.856,-1684.928;Inherit;False;1651.861;293.551;Comment;7;63;62;52;60;56;61;67;Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-1453.863,-1115.367;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-1276.936,-1115.073;Inherit;False;mainSample;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;68;-3226.247,-256.7511;Inherit;False;1324.282;523.3473;Comment;8;69;8;7;9;10;35;34;33;Amount and thickness;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;52;-3055.856,-1634.928;Inherit;True;Polar Coordinates;-1;;4;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-2632.639,-1476.377;Inherit;False;Property;_FadeAmount;FadeAmount;7;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-3150.592,-206.7511;Inherit;False;31;mainSample;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;56;-2632.77,-1621.307;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;34;-3176.247,-86.34043;Inherit;False;Property;_ElectricityAmount;ElectricityAmount;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;35;-2921.274,-190.2445;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;60;-2280.1,-1556.103;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1.13;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;91;-1333.81,-2056.387;Inherit;False;1801.194;544.6252;Comment;8;89;90;87;84;82;78;85;92;Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;89;-1283.81,-1661.193;Inherit;False;Property;_Gradientspeedanddirection;Gradient speed and direction;9;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TFHCRemapNode;7;-2723.444,-180.6487;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-10;False;4;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;87;-1170.606,-1807.494;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2600.156,64.99441;Inherit;False;Constant;_Float4;Float 4;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;61;-2079.686,-1564.805;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-2599.568,168.1268;Inherit;False;Property;_Thickness;Thickness;2;0;Create;True;0;0;False;0;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;63;-1859.902,-1583.157;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;8;-2387.914,12.38855;Inherit;False;Rectangle;-1;;1;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-962.3449,-1745.911;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-2164.685,27.5188;Inherit;False;amountAndThickness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;67;-1695.445,-1559.048;Inherit;False;mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;72;-1776.993,-50.01846;Inherit;False;911.5632;386.9283;Comment;5;71;77;65;70;66;Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;85;-807.9426,-1790.972;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;84;-266.5436,-2006.387;Inherit;False;Constant;_Gradientcolor;Gradient color;10;1;[HDR];Create;True;0;0;False;0;1,1,1,0;1,0,0.7533741,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;78;-554.6393,-1803.935;Inherit;True;Property;_Gradient;Gradient;8;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;66;-1712.811,224.5537;Inherit;False;67;mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1726.993,5.380613;Inherit;False;69;amountAndThickness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1500.468,153.7479;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;75;-279.2628,-1058.491;Inherit;False;1357.521;604.7279;Comment;7;0;73;79;93;47;74;44;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-2.997181,-1812.102;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;77;-1314.006,49.00396;Inherit;False;Property;_UseFade;UseFade;6;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;44;-229.2627,-1008.491;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;170.9197,-1803.871;Inherit;False;gradient;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-113.1615,-783.9802;Inherit;False;69;amountAndThickness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;113.4034,-891.4852;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;87.06787,-1004.7;Inherit;False;92;gradient;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;71;-1089.192,59.90262;Inherit;False;alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;79;326.7242,-922.7067;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;164.4672,-705.068;Inherit;False;71;alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;792.7764,-931.6965;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;ElectricityShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;3;0
WireConnection;13;0;18;0
WireConnection;13;1;11;0
WireConnection;19;0;6;0
WireConnection;5;0;17;0
WireConnection;5;1;4;0
WireConnection;14;1;13;0
WireConnection;2;1;5;0
WireConnection;41;0;2;0
WireConnection;41;1;17;0
WireConnection;41;2;20;0
WireConnection;42;0;14;0
WireConnection;42;1;18;0
WireConnection;42;2;21;0
WireConnection;26;0;41;0
WireConnection;24;0;42;0
WireConnection;27;0;28;0
WireConnection;27;1;29;0
WireConnection;31;0;27;0
WireConnection;56;0;52;0
WireConnection;35;0;33;0
WireConnection;35;1;34;0
WireConnection;60;0;56;0
WireConnection;60;1;62;0
WireConnection;7;0;35;0
WireConnection;61;0;60;0
WireConnection;63;0;61;0
WireConnection;8;1;7;0
WireConnection;8;2;9;0
WireConnection;8;3;10;0
WireConnection;90;0;87;0
WireConnection;90;1;89;0
WireConnection;69;0;8;0
WireConnection;67;0;63;0
WireConnection;85;1;90;0
WireConnection;78;1;85;0
WireConnection;65;0;70;0
WireConnection;65;1;66;0
WireConnection;82;0;84;0
WireConnection;82;1;78;0
WireConnection;77;1;70;0
WireConnection;77;0;65;0
WireConnection;92;0;82;0
WireConnection;47;0;44;0
WireConnection;47;1;74;0
WireConnection;71;0;77;0
WireConnection;79;0;93;0
WireConnection;79;1;47;0
WireConnection;0;2;79;0
WireConnection;0;9;73;0
ASEEND*/
//CHKSM=3417D7ADFB13404C5A295086BEF67C5198273BDE