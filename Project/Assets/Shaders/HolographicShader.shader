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
		_Holographicpower("Holographic power", Range( 0 , 2)) = 2
		[Toggle(_USEFRESNEL_ON)] _useFresnel("useFresnel", Float) = 0
		_Scale("Scale", Float) = 1
		_Power("Power", Float) = 1
		[HDR]_FresnelColor("FresnelColor", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _USEFRESNEL_ON
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float4 _FresnelColor;
		uniform float _Scale;
		uniform float _Power;
		uniform float4 _Maincolor;
		uniform sampler2D _CollectibleTexture;
		uniform float4 _CollectibleTexture_ST;
		uniform sampler2D _Holographicmask;
		uniform float2 _Holospeedanddir;
		uniform float _Holographicpower;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_cast_0 = (0.0).xxxx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV58 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode58 = ( 0.0 + _Scale * pow( 1.0 - fresnelNdotV58, _Power ) );
			#ifdef _USEFRESNEL_ON
				float4 staticSwitch59 = ( _FresnelColor * fresnelNode58 );
			#else
				float4 staticSwitch59 = temp_cast_0;
			#endif
			float4 fresnel61 = staticSwitch59;
			float4 mainColor10 = _Maincolor;
			float2 uv_CollectibleTexture = i.uv_texcoord * _CollectibleTexture_ST.xy + _CollectibleTexture_ST.zw;
			float4 tex2DNode1 = tex2D( _CollectibleTexture, uv_CollectibleTexture );
			float4 collectibleTexture3 = tex2DNode1;
			o.Emission = ( fresnel61 + ( mainColor10 * collectibleTexture3 ) ).rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 panner55 = ( _Time.y * _Holospeedanddir + ase_screenPosNorm.xy);
			float4 holographicEffect20 = ( tex2D( _Holographicmask, panner55 ) * _Holographicpower );
			float collectibleAlpha6 = tex2DNode1.a;
			o.Alpha = ( holographicEffect20 * collectibleAlpha6 ).r;
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
Version=17700
1920;6;1920;1013;931.1968;651.6067;1.3;True;False
Node;AmplifyShaderEditor.CommentaryNode;66;-681.184,-808.5767;Inherit;False;1359.017;581.642;Comment;8;61;58;62;63;59;60;65;64;Fresnel;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;15;-2090.857,45.49573;Inherit;False;1287.039;606.1716;Comment;8;20;25;28;36;55;54;56;57;Holographic effect;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;25;-2008.654,503.5539;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;54;-2013.595,125.028;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;28;-2032.73,348.4496;Inherit;False;Property;_Holospeedanddir;Holo speed and dir;2;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;62;-631.184,-458.8953;Inherit;False;Property;_Scale;Scale;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-620.184,-367.8953;Inherit;False;Property;_Power;Power;7;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;58;-422.9538,-480.9346;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;13;-1328.702,-312.035;Inherit;False;517.459;262;Comment;2;8;10;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;64;-372.5244,-758.5767;Inherit;False;Property;_FresnelColor;FresnelColor;8;1;[HDR];Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;55;-1773.146,298.7556;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;4;-2069.831,-325.1599;Inherit;False;666.4862;280.9927;Comment;3;1;3;6;Collectible texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;60;10.97744,-654.5773;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-72.2244,-541.4767;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-2019.831,-274.1671;Inherit;True;Property;_CollectibleTexture;CollectibleTexture;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-1278.702,-262.035;Inherit;False;Property;_Maincolor;Main color;1;1;[HDR];Create;True;0;0;False;0;0,0.8904901,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;57;-1494.674,502.6486;Inherit;False;Property;_Holographicpower;Holographic power;4;0;Create;True;0;0;False;0;2;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;36;-1561.463,291.1971;Inherit;True;Property;_Holographicmask;Holographic mask;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;3;-1645.345,-275.1598;Inherit;False;collectibleTexture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;59;178.1165,-504.1087;Inherit;False;Property;_useFresnel;useFresnel;5;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;12;-541.845,39.09825;Inherit;False;1323.575;584.4327;Comment;9;21;0;7;9;5;11;22;68;70;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1035.243,-208.2655;Inherit;False;mainColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-1203.7,312.2386;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-1041.153,319.8273;Inherit;False;holographicEffect;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-365.701,115.0983;Inherit;False;10;mainColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;6;-1666.146,-166.2076;Inherit;False;collectibleAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;5;-377.845,199.9922;Inherit;False;3;collectibleTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;453.833,-514.8839;Inherit;False;fresnel;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-54.20454,157.3769;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;21;-298.9749,378.1586;Inherit;False;20;holographicEffect;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;7;-253.0778,495.9171;Inherit;False;6;collectibleAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;27.97876,76.2265;Inherit;False;61;fresnel;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;59.0624,369.446;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;70;210.2031,121.8933;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;439.6305,133.0311;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;HolographicShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;58;2;62;0
WireConnection;58;3;63;0
WireConnection;55;0;54;0
WireConnection;55;2;28;0
WireConnection;55;1;25;0
WireConnection;65;0;64;0
WireConnection;65;1;58;0
WireConnection;36;1;55;0
WireConnection;3;0;1;0
WireConnection;59;1;60;0
WireConnection;59;0;65;0
WireConnection;10;0;8;0
WireConnection;56;0;36;0
WireConnection;56;1;57;0
WireConnection;20;0;56;0
WireConnection;6;0;1;4
WireConnection;61;0;59;0
WireConnection;9;0;11;0
WireConnection;9;1;5;0
WireConnection;22;0;21;0
WireConnection;22;1;7;0
WireConnection;70;0;68;0
WireConnection;70;1;9;0
WireConnection;0;2;70;0
WireConnection;0;9;22;0
ASEEND*/
//CHKSM=6F983F98392479ACC0C9FD664D07F6D872997420