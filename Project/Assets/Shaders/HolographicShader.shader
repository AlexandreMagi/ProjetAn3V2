// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HolographicShader"
{
	Properties
	{
		_CollectibleTexture("CollectibleTexture", 2D) = "white" {}
		[HDR]_Maincolor("Main color", Color) = (0,0.8904901,1,0)
		_Holospeedanddir("Holo speed and dir", Vector) = (0,0,0,0)
		_Holographicmask("Holographic mask", 2D) = "white" {}
		_Holographicpower("Holographic power", Range( 0 , 10)) = 1
		[Toggle(_USEFRESNEL_ON)] _useFresnel("useFresnel", Float) = 0
		_Scale("Scale", Float) = 1
		_Power("Power", Float) = 1
		[HDR]_FresnelColor("FresnelColor", Color) = (1,1,1,0)
		[Toggle(useFresnel)] _Keyword0("Keyword 0", Float) = 0
		_Edgepower1("Edge power", Range( 0 , 1)) = 0
		_Edgedistance1("Edge distance", Float) = 1
		[HDR]_Edgecolor1("Edge color", Color) = (1,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USEFRESNEL_ON
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			float4 screenPos;
		};

		uniform float4 _Maincolor;
		uniform sampler2D _CollectibleTexture;
		uniform float4 _CollectibleTexture_ST;
		uniform float4 _FresnelColor;
		uniform float _Scale;
		uniform float _Power;
		uniform float4 _Edgecolor1;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Edgedistance1;
		uniform float _Edgepower1;
		uniform sampler2D _Holographicmask;
		uniform float2 _Holospeedanddir;
		uniform float _Holographicpower;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 mainColor10 = _Maincolor;
			float2 uv_CollectibleTexture = i.uv_texcoord * _CollectibleTexture_ST.xy + _CollectibleTexture_ST.zw;
			float4 tex2DNode1 = tex2D( _CollectibleTexture, uv_CollectibleTexture );
			float4 collectibleTexture3 = tex2DNode1;
			float2 temp_cast_0 = (_Time.y).xx;
			float dotResult4_g1 = dot( temp_cast_0 , float2( 12.9898,78.233 ) );
			float lerpResult10_g1 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g1 ) * 43758.55 ) ));
			float temp_output_77_0 = (( lerpResult10_g1 < 0.9 ) ? 1.0 :  0.5 );
			float4 temp_output_83_0 = ( ( mainColor10 * collectibleTexture3 ) * temp_output_77_0 );
			o.Albedo = temp_output_83_0.rgb;
			float4 temp_cast_2 = (0.0).xxxx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV58 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode58 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV58, _Power ) );
			#ifdef _USEFRESNEL_ON
				float4 staticSwitch59 = ( _FresnelColor * fresnelNode58 );
			#else
				float4 staticSwitch59 = temp_cast_2;
			#endif
			float4 fresnel61 = staticSwitch59;
			#ifdef useFresnel
				float4 staticSwitch84 = ( fresnel61 * temp_output_77_0 );
			#else
				float4 staticSwitch84 = temp_output_83_0;
			#endif
			float collectibleAlpha6 = tex2DNode1.a;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth87 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth87 = abs( ( screenDepth87 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Edgedistance1 ) );
			float clampResult91 = clamp( ( ( 1.0 - distanceDepth87 ) * _Edgepower1 ) , 0.0 , 1.0 );
			float4 edge94 = ( collectibleAlpha6 * ( _Edgecolor1 * clampResult91 ) );
			o.Emission = ( staticSwitch84 + edge94 ).rgb;
			float2 panner55 = ( _Time.y * _Holospeedanddir + ase_screenPosNorm.xy);
			float4 holographicEffect20 = ( tex2D( _Holographicmask, panner55 ) * _Holographicpower );
			o.Alpha = ( ( ( holographicEffect20 * collectibleAlpha6 ) * temp_output_77_0 ) + edge94 ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

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
				float4 screenPos : TEXCOORD3;
				float3 worldNormal : TEXCOORD4;
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
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
Version=18000
1920;6;1920;1013;-1492.448;1059.438;1.158347;True;False
Node;AmplifyShaderEditor.CommentaryNode;85;1049.357,-737.2881;Inherit;False;2117.004;541.8289;Comment;11;94;93;91;92;90;89;88;87;86;98;99;Edge detection;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;15;-2090.857,45.49573;Inherit;False;1287.039;606.1716;Comment;8;20;25;28;36;55;54;56;57;Holographic effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;66;-773.2752,-1211.475;Inherit;False;1439.794;615.6105;Comment;8;61;59;60;65;58;64;62;63;Fresnel;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;86;1099.357,-500.8564;Inherit;False;Property;_Edgedistance1;Edge distance;11;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-712.2752,-770.7939;Inherit;False;Property;_Power;Power;7;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-723.2752,-861.794;Inherit;False;Property;_Scale;Scale;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;87;1301.302,-504.426;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;28;-2032.73,348.4496;Inherit;False;Property;_Holospeedanddir;Holo speed and dir;2;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ScreenPosInputsNode;54;-2013.595,125.028;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;25;-2008.654,503.5539;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;64;-464.6155,-1161.475;Inherit;False;Property;_FresnelColor;FresnelColor;8;1;[HDR];Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;58;-515.0449,-883.8333;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;4;-2069.831,-325.1599;Inherit;False;666.4862;280.9927;Comment;3;1;3;6;Collectible texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;13;-1328.702,-312.035;Inherit;False;517.459;262;Comment;2;8;10;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;55;-1773.146,298.7556;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;88;1667.815,-338.239;Inherit;False;Property;_Edgepower1;Edge power;10;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;89;1647.292,-480.031;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-164.3155,-944.3755;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-2019.831,-274.1671;Inherit;True;Property;_CollectibleTexture;CollectibleTexture;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-1278.702,-262.035;Inherit;False;Property;_Maincolor;Main color;1;1;[HDR];Create;True;0;0;False;0;0,0.8904901,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;36;-1561.463,291.1971;Inherit;True;Property;_Holographicmask;Holographic mask;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;60;2.751984,-1046.974;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;12;-411.3306,39.09825;Inherit;False;1830.596;713.8964;Comment;18;0;84;82;78;22;83;9;77;68;21;7;11;5;75;74;95;96;97;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-1494.674,502.6486;Inherit;False;Property;_Holographicpower;Holographic power;4;0;Create;True;0;0;False;0;1;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;1985.967,-450.766;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;92;2039.654,-647.3348;Inherit;False;Property;_Edgecolor1;Edge color;12;1;[HDR];Create;True;0;0;False;0;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;59;147.4911,-925.3054;Inherit;False;Property;_useFresnel;useFresnel;5;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;6;-1666.146,-166.2076;Inherit;False;collectibleAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;91;2150.368,-441.2932;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1203.7,312.2386;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;74;-335.7511,609.6149;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1035.243,-208.2655;Inherit;False;mainColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3;-1645.345,-275.1598;Inherit;False;collectibleTexture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;423.2075,-936.0806;Inherit;False;fresnel;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;5;-377.845,199.9922;Inherit;False;3;collectibleTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-1041.153,319.8273;Inherit;False;holographicEffect;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;75;-115.7563,609.5588;Inherit;False;Random Range;-1;;1;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-365.701,115.0983;Inherit;False;10;mainColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;2360.584,-472.8489;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;2336.884,-662.125;Inherit;False;6;collectibleAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-54.20454,157.3769;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;21;-298.9749,378.1586;Inherit;False;20;holographicEffect;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;7;-253.0778,495.9171;Inherit;False;6;collectibleAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;-35.02124,267.2265;Inherit;False;61;fresnel;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCCompareLower;77;119.6436,537.1588;Inherit;False;4;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;3;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;2580.137,-523.1233;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;2780.638,-440.5626;Inherit;False;edge;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;407.3652,248.2186;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;59.0624,369.446;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;438.165,134.6081;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;404.6882,364.1801;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;577.8774,489.4219;Inherit;False;94;edge;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;84;595.064,202.7209;Inherit;False;Property;_Keyword0;Keyword 0;9;0;Create;True;0;0;False;0;0;0;0;True;useFresnel;Toggle;2;Key0;Key1;Fetch;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;810.5771,377.6218;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;827.4775,212.5218;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1007.131,129.8494;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;HolographicShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;87;0;86;0
WireConnection;58;2;62;0
WireConnection;58;3;63;0
WireConnection;55;0;54;0
WireConnection;55;2;28;0
WireConnection;55;1;25;0
WireConnection;89;0;87;0
WireConnection;65;0;64;0
WireConnection;65;1;58;0
WireConnection;36;1;55;0
WireConnection;90;0;89;0
WireConnection;90;1;88;0
WireConnection;59;1;60;0
WireConnection;59;0;65;0
WireConnection;6;0;1;4
WireConnection;91;0;90;0
WireConnection;56;0;36;0
WireConnection;56;1;57;0
WireConnection;10;0;8;0
WireConnection;3;0;1;0
WireConnection;61;0;59;0
WireConnection;20;0;56;0
WireConnection;75;1;74;0
WireConnection;93;0;92;0
WireConnection;93;1;91;0
WireConnection;9;0;11;0
WireConnection;9;1;5;0
WireConnection;77;0;75;0
WireConnection;98;0;99;0
WireConnection;98;1;93;0
WireConnection;94;0;98;0
WireConnection;78;0;68;0
WireConnection;78;1;77;0
WireConnection;22;0;21;0
WireConnection;22;1;7;0
WireConnection;83;0;9;0
WireConnection;83;1;77;0
WireConnection;82;0;22;0
WireConnection;82;1;77;0
WireConnection;84;1;83;0
WireConnection;84;0;78;0
WireConnection;96;0;82;0
WireConnection;96;1;95;0
WireConnection;97;0;84;0
WireConnection;97;1;95;0
WireConnection;0;0;83;0
WireConnection;0;2;97;0
WireConnection;0;9;96;0
ASEEND*/
//CHKSM=DDF864AE852A406378BE6CC769A64EFBEF4F96EC