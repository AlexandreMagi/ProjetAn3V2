// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Custom FX Shader/GravityOrbZone"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		[HDR]_Color("Color", Color) = (0,0,0,0)
		_Softparticlesdistance("Soft particles distance", Float) = 1
		_Softparticlespower("Soft particles power", Range( 0 , 1)) = 1
		_Directionandspeed("Direction and speed", Vector) = (0,-0.5,0,0)
		[Toggle(_ISUSINGPREVISUMODE_ON)] _isUsingPrevisuMode("isUsingPrevisuMode", Float) = 0
		_PrevisuColor("PrevisuColor", Color) = (1,1,1,1)
		_Previsupower("Previsu power", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ISUSINGPREVISUMODE_ON
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 screenPos;
		};

		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float2 _Directionandspeed;
		uniform float4 _PrevisuColor;
		uniform float _Previsupower;
		uniform float _Smoothness;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Softparticlesdistance;
		uniform float _Softparticlespower;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (0.0).xx;
			#ifdef _ISUSINGPREVISUMODE_ON
				float2 staticSwitch68 = temp_cast_0;
			#else
				float2 staticSwitch68 = ( _Time.y * _Directionandspeed );
			#endif
			float2 uv_TexCoord29 = i.uv_texcoord + staticSwitch68;
			float4 tex2DNode4 = tex2D( _Albedo, uv_TexCoord29 );
			float4 mainTextureRGBA34 = tex2DNode4;
			float2 temp_cast_1 = (0.0).xx;
			#ifdef _ISUSINGPREVISUMODE_ON
				float2 staticSwitch71 = temp_cast_1;
			#else
				float2 staticSwitch71 = ( _Time.y * float2( 0,0.5 ) );
			#endif
			float2 uv_TexCoord46 = i.uv_texcoord + staticSwitch71;
			float4 tex2DNode47 = tex2D( _Albedo, uv_TexCoord46 );
			float4 secondTextureRGBA52 = tex2DNode47;
			float4 fullTextureRGBA56 = ( mainTextureRGBA34 * secondTextureRGBA52 );
			float4 temp_cast_2 = (0.0).xxxx;
			#ifdef _ISUSINGPREVISUMODE_ON
				float4 staticSwitch64 = ( _PrevisuColor * _Previsupower );
			#else
				float4 staticSwitch64 = temp_cast_2;
			#endif
			float4 previsu65 = staticSwitch64;
			o.Emission = ( ( _Color * fullTextureRGBA56 ) + previsu65 ).rgb;
			o.Smoothness = _Smoothness;
			float mainTextureAlpha5 = tex2DNode4.a;
			float secondTextureAlpha48 = tex2DNode47.a;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth15 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth15 = abs( ( screenDepth15 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Softparticlesdistance ) );
			float clampResult20 = clamp( ( ( 1.0 - distanceDepth15 ) * _Softparticlespower ) , 0.0 , 1.0 );
			float softParticles22 = clampResult20;
			float alpha12 = ( ( i.vertexColor.a * ( mainTextureAlpha5 * secondTextureAlpha48 ) ) * softParticles22 );
			o.Alpha = ( 0.1 + alpha12 );
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
				half4 color : COLOR0;
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
				o.screenPos = ComputeScreenPos( o.pos );
				o.color = v.color;
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
				surfIN.screenPos = IN.screenPos;
				surfIN.vertexColor = IN.color;
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
-1920;171;1920;1019;1136.882;93.32593;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;1;-2396.533,-337.464;Inherit;False;1678.909;595.5169;Comment;9;68;34;5;4;29;32;36;31;70;Main texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;42;-2388.419,348.4362;Inherit;False;1668.901;575.8102;Comment;9;71;52;48;47;46;45;44;43;72;Second texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;31;-2270.224,-119.8737;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;36;-2269.356,20.2128;Inherit;False;Property;_Directionandspeed;Direction and speed;4;0;Create;True;0;0;False;0;0,-0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;44;-2324.59,717.2982;Inherit;False;Constant;_Vector0;Vector 0;6;0;Create;True;0;0;False;0;0,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;43;-2353.458,577.2117;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-2108.962,589.4069;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1995.211,21.29385;Inherit;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;13;-965.2625,-839.5933;Inherit;False;1684.377;326.2766;Comment;7;22;20;18;17;16;15;14;Edge detection;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-2025.728,-107.6784;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-2054.204,717.1182;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;71;-1945.015,584.8207;Inherit;False;Property;_isUsingPrevisuMode2;isUsingPrevisuMode;5;0;Create;False;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Reference;64;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-924.1345,-764.5911;Inherit;False;Property;_Softparticlesdistance;Soft particles distance;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;68;-1869.477,-97.5593;Inherit;False;Property;_isUsingPrevisuMode;isUsingPrevisuMode;5;0;Create;False;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Reference;64;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-1646.743,551.0609;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-1572.092,-182.5806;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;15;-657.1885,-770.1605;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-461.5864,-608.4127;Inherit;False;Property;_Softparticlespower;Soft particles power;3;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;47;-1310.286,481.067;Inherit;True;Property;_Albedo;Albedo;0;0;Fetch;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-1306.154,-198.0169;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;16;-350.0391,-744.134;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;6;-2269.612,-969.7056;Inherit;False;1226.693;462.8935;Comment;8;12;11;10;9;7;51;50;8;Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;5;-959.7119,-102.6479;Inherit;False;mainTextureAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-959.1568,585.8101;Inherit;False;secondTextureAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-143.4344,-720.9398;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-2221.309,-605.1744;Inherit;False;48;secondTextureAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;57;-1698.979,1123.042;Inherit;False;722.0964;258.8485;Comment;4;53;54;55;56;TextureBlend;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-952.0808,-203.7207;Inherit;False;mainTextureRGBA;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-970.2379,481.1111;Inherit;False;secondTextureRGBA;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;8;-2248.447,-689.2076;Inherit;False;5;mainTextureAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;20;157.8747,-690.3184;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;59;902.5725,-347.7512;Inherit;False;1058;451;Comment;6;65;64;63;62;61;60;Previsu state;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-1648.979,1173.042;Inherit;False;34;mainTextureRGBA;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;485.0896,-688.1666;Inherit;False;softParticles;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;60;997.7786,-248.2139;Inherit;False;Property;_PrevisuColor;PrevisuColor;6;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;61;952.5726,-12.75121;Inherit;False;Property;_Previsupower;Previsu power;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-2000.23,-710.5063;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;7;-2147.154,-919.7056;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;54;-1648.099,1265.89;Inherit;False;52;secondTextureRGBA;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;63;1295.573,-298.7512;Inherit;False;Constant;_Float2;Float 2;16;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1882.255,-825.855;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;1278.573,-147.7512;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;9;-1853.767,-670.733;Inherit;False;22;softParticles;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-1372.885,1186.751;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-1209.883,1196.201;Inherit;False;fullTextureRGBA;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1653.741,-814.4819;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;64;1483.573,-191.7512;Inherit;False;Property;_isUsingPrevisuMode;isUsingPrevisuMode;5;0;Create;True;0;0;False;0;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;25;-331.6069,-50;Inherit;False;994.8459;832.2835;Comment;10;23;0;66;27;67;58;24;26;73;83;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1283.543,-806.3028;Inherit;False;alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;65;1777.092,-171.2837;Inherit;False;previsu;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;-287.8953,202.9431;Inherit;False;56;fullTextureRGBA;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;26;-243.6645,-2.352051;Inherit;False;Property;_Color;Color;1;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;23;-309.5683,659.074;Inherit;False;12;alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-277.6845,529.3582;Inherit;False;Constant;_Alpha;Alpha;6;0;Create;True;0;0;False;0;0.1;0;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;49.66702,124.5725;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;21.54214,265.1887;Inherit;False;65;previsu;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;73;68.1055,390.4236;Inherit;False;Property;_Smoothness;Smoothness;8;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;220.442,197.5884;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;94.11841,569.6741;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;399.797,271.2046;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_Custom FX Shader/GravityOrbZone;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;45;0;43;0
WireConnection;45;1;44;0
WireConnection;32;0;31;0
WireConnection;32;1;36;0
WireConnection;71;1;45;0
WireConnection;71;0;72;0
WireConnection;68;1;32;0
WireConnection;68;0;70;0
WireConnection;46;1;71;0
WireConnection;29;1;68;0
WireConnection;15;0;14;0
WireConnection;47;1;46;0
WireConnection;4;1;29;0
WireConnection;16;0;15;0
WireConnection;5;0;4;4
WireConnection;48;0;47;4
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;34;0;4;0
WireConnection;52;0;47;0
WireConnection;20;0;18;0
WireConnection;22;0;20;0
WireConnection;51;0;8;0
WireConnection;51;1;50;0
WireConnection;10;0;7;4
WireConnection;10;1;51;0
WireConnection;62;0;60;0
WireConnection;62;1;61;0
WireConnection;55;0;53;0
WireConnection;55;1;54;0
WireConnection;56;0;55;0
WireConnection;11;0;10;0
WireConnection;11;1;9;0
WireConnection;64;1;63;0
WireConnection;64;0;62;0
WireConnection;12;0;11;0
WireConnection;65;0;64;0
WireConnection;27;0;26;0
WireConnection;27;1;24;0
WireConnection;66;0;27;0
WireConnection;66;1;67;0
WireConnection;83;0;58;0
WireConnection;83;1;23;0
WireConnection;0;2;66;0
WireConnection;0;4;73;0
WireConnection;0;9;83;0
ASEEND*/
//CHKSM=445A1CD6CFF87335094AD322C34C3172F856CC5C