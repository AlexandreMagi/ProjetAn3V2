// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CellShadingShader"
{
	Properties
	{
		_Diffuse("Diffuse", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Specular("Specular", 2D) = "white" {}
		_LightRamp("Light Ramp", 2D) = "white" {}
		_Reveallighttexture("Reveal light texture", 2D) = "white" {}
		[HDR]_Tint("Tint", Color) = (0.5,0.5,0.5,0)
		_Scaleandoffsetshadows("Scale and offset shadows", Range( 0 , 1)) = 0.5
		_Rimoffset("Rim offset", Float) = 1
		_Rimpower("Rim power", Range( 0 , 1)) = 0
		[HDR]_Rimtint("Rim tint", Color) = (0.5613652,0.8207547,0.7956936,0)
		[HDR]_Speccolor("Spec color", Color) = (0,0,0,0)
		_Specvalue("Spec value", Float) = 0
		_Specintensity("Spec intensity", Range( 0 , 1)) = 0
		_Speclighttransition("Spec light transition", Range( 0 , 1)) = 0
		_ColortoBeFiltered("Color to Be Filtered", Color) = (0.6981132,0.25356,0.25356,1)
		[HDR]_Reveallightcolor("Reveal light color", Color) = (1,0.07075471,0.07075471,0)
		_DifferenceThreshold("Difference Threshold", Range( 0 , 0.05)) = 0.05
		_Contrast("Contrast", Float) = 0
		_Albedocontrast("Albedo contrast", Float) = 0
		_RevealLightEnabled("RevealLightEnabled", Range( 0 , 1)) = 1
		_SpecEnabled("SpecEnabled", Range( 0 , 1)) = 1
		_RimLightEnabled("RimLightEnabled", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _Albedocontrast;
		uniform float4 _Tint;
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform float4 _Reveallightcolor;
		uniform sampler2D _Reveallighttexture;
		uniform float4 _Reveallighttexture_ST;
		uniform float _Contrast;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 _ColortoBeFiltered;
		uniform float _DifferenceThreshold;
		uniform float _RevealLightEnabled;
		uniform float _RimLightEnabled;
		uniform float _Rimoffset;
		uniform float _Rimpower;
		uniform float4 _Rimtint;
		uniform sampler2D _LightRamp;
		uniform float _Scaleandoffsetshadows;
		uniform float _Specvalue;
		uniform sampler2D _Specular;
		uniform float4 _Specular_ST;
		uniform float4 _Speccolor;
		uniform float _Speclighttransition;
		uniform float _Specintensity;
		uniform float _SpecEnabled;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 normalMap28 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult5 = dot( normalize( (WorldNormalVector( i , normalMap28 )) ) , ase_worldViewDir );
			float lightView10 = dotResult5;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult3 = dot( normalize( (WorldNormalVector( i , normalMap28 )) ) , ase_worldlightDir );
			float lightDir11 = dotResult3;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 rimLight60 = ( _RimLightEnabled * ( saturate( ( pow( ( 1.0 - saturate( ( _Rimoffset + lightView10 ) ) ) , _Rimpower ) * ( lightDir11 * ase_lightAtten ) ) ) * ( ase_lightColor * _Rimtint ) ) );
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 diffuse37 = ( _Tint * tex2D( _Diffuse, uv_Diffuse ) );
			float2 temp_cast_1 = ((lightDir11*_Scaleandoffsetshadows + _Scaleandoffsetshadows)).xx;
			float4 shadow14 = ( diffuse37 * tex2D( _LightRamp, temp_cast_1 ) );
			UnityGI gi45 = gi;
			float3 diffNorm45 = WorldNormalVector( i , normalMap28 );
			gi45 = UnityGI_Base( data, 1, diffNorm45 );
			float3 indirectDiffuse45 = gi45.indirect.diffuse + diffNorm45 * 0.0001;
			float4 lighting44 = ( shadow14 * ( ase_lightColor * float4( ( indirectDiffuse45 + ase_lightAtten ) , 0.0 ) ) );
			float dotResult83 = dot( ( ase_worldViewDir + _WorldSpaceLightPos0.xyz ) , normalize( (WorldNormalVector( i , normalMap28 )) ) );
			float smoothstepResult86 = smoothstep( 2.0 , 2.1 , pow( dotResult83 , _Specvalue ));
			float2 uv_Specular = i.uv_texcoord * _Specular_ST.xy + _Specular_ST.zw;
			float4 lerpResult101 = lerp( _Speccolor , ase_lightColor , _Speclighttransition);
			float4 spec93 = ( ( ase_lightAtten * ( ( smoothstepResult86 * ( tex2D( _Specular, uv_Specular ) * lerpResult101 ) ) * _Specintensity ) ) * _SpecEnabled );
			float4 temp_output_164_0 = ( ase_lightAtten * ase_lightColor );
			float2 uv_Reveallighttexture = i.uv_texcoord * _Reveallighttexture_ST.xy + _Reveallighttexture_ST.zw;
			float3 normalizeResult160 = normalize( (( ( ase_lightColor * _WorldSpaceLightPos0.w ) * lightDir11 )).rgb );
			float3 normalizeResult161 = normalize( (_ColortoBeFiltered).rgb );
			float dotResult165 = dot( normalizeResult160 , normalizeResult161 );
			float4 handpaint373 = ( ( CalculateContrast(_Albedocontrast,( diffuse37 * saturate( temp_output_164_0 ) )) + ( ( _Reveallightcolor * tex2D( _Reveallighttexture, uv_Reveallighttexture ).r ) * CalculateContrast(_Contrast,( ( temp_output_164_0 * _WorldSpaceLightPos0.w ) *  ( dotResult165 - _DifferenceThreshold > 1.0 ? 0.0 : dotResult165 - _DifferenceThreshold <= 1.0 && dotResult165 + _DifferenceThreshold >= 1.0 ? 1.0 : 0.0 )  )) ) ) * _RevealLightEnabled );
			c.rgb = ( ( ( rimLight60 + lighting44 ) + spec93 ) + handpaint373 ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 diffuse37 = ( _Tint * tex2D( _Diffuse, uv_Diffuse ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 temp_output_164_0 = ( 1 * ase_lightColor );
			float2 uv_Reveallighttexture = i.uv_texcoord * _Reveallighttexture_ST.xy + _Reveallighttexture_ST.zw;
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 normalMap28 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult3 = dot( normalize( (WorldNormalVector( i , normalMap28 )) ) , ase_worldlightDir );
			float lightDir11 = dotResult3;
			float3 normalizeResult160 = normalize( (( ( ase_lightColor * _WorldSpaceLightPos0.w ) * lightDir11 )).rgb );
			float3 normalizeResult161 = normalize( (_ColortoBeFiltered).rgb );
			float dotResult165 = dot( normalizeResult160 , normalizeResult161 );
			float4 handpaint373 = ( ( CalculateContrast(_Albedocontrast,( diffuse37 * saturate( temp_output_164_0 ) )) + ( ( _Reveallightcolor * tex2D( _Reveallighttexture, uv_Reveallighttexture ).r ) * CalculateContrast(_Contrast,( ( temp_output_164_0 * _WorldSpaceLightPos0.w ) *  ( dotResult165 - _DifferenceThreshold > 1.0 ? 0.0 : dotResult165 - _DifferenceThreshold <= 1.0 && dotResult165 + _DifferenceThreshold >= 1.0 ? 1.0 : 0.0 )  )) ) ) * _RevealLightEnabled );
			float4 temp_output_115_0 = handpaint373;
			o.Emission = temp_output_115_0.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
0;434;1364;565;9517.594;1188.684;7.598;True;False
Node;AmplifyShaderEditor.CommentaryNode;29;-1555.516,1628.246;Inherit;False;616.2219;280;Comment;2;28;27;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;27;-1505.515,1678.246;Inherit;True;Property;_Normal;Normal;1;0;Create;True;0;0;False;0;-1;None;94c473c9137744d7eabd829f1150da8d;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-1163.291,1713.696;Inherit;False;normalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;8;-2686.427,106.3975;Inherit;False;955.9893;445.9081;Comment;5;10;5;6;4;33;LightView;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;9;-2689.28,-541.9921;Inherit;False;967.5994;445.123;Comment;5;11;3;1;2;32;LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-2655.99,147.7881;Inherit;False;28;normalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;-2646.89,-496.6;Inherit;False;28;normalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-2367.107,156.3975;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;6;-2346.618,348.8701;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;1;-2361.476,-491.992;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;2;-2362.681,-309.2638;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;3;-2065.68,-390.2638;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;149;-5518.278,752.9568;Inherit;False;2417.002;1270.554;Comment;34;343;342;173;171;175;198;271;200;197;165;163;164;162;159;161;160;158;156;157;238;155;154;239;153;152;373;404;407;412;419;421;422;427;431;RevealLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;5;-2102.619,207.8701;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;153;-5441.361,1489.559;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-1914.909,-380.8533;Inherit;False;lightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1926.635,234.4577;Inherit;False;lightView;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;152;-5405.496,1264.572;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;76;-3025.552,760.2269;Inherit;False;1953.216;681.7428;Comment;19;53;55;54;56;57;59;58;64;61;69;65;75;62;60;68;66;67;423;430;Rim light;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-2892.248,810.2269;Inherit;False;Property;_Rimoffset;Rim offset;7;0;Create;True;0;0;False;0;1;0.62;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;38;-1626.52,-545.9164;Inherit;False;883.0335;473.1851;Comment;4;34;35;36;37;Diffuse;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-2975.552,953.5958;Inherit;False;10;lightView;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;239;-5288.269,1646.16;Inherit;False;11;lightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;97;-5508.969,-545.1815;Inherit;False;2753.833;1170.912;Comment;25;93;91;89;92;90;96;86;95;87;84;88;85;83;80;81;82;79;77;99;101;102;104;100;432;429;Spec;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;154;-5179.378,1494.401;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-2710.348,908.9268;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;34;-1576.52,-302.7311;Inherit;True;Property;_Diffuse;Diffuse;0;0;Create;True;0;0;False;0;-1;None;0a0e8af2869fa45f78e25460db578482;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;35;-1532.897,-495.916;Inherit;False;Property;_Tint;Tint;5;1;[HDR];Create;True;0;0;False;0;0.5,0.5,0.5,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;26;-3008.287,1593.846;Inherit;False;1360.203;357.2934;Comment;7;14;39;13;40;22;23;12;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-5349.999,-200.8658;Inherit;False;28;normalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;155;-5290.3,1763.469;Float;False;Property;_ColortoBeFiltered;Color to Be Filtered;14;0;Create;True;0;0;False;0;0.6981132,0.25356,0.25356,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;77;-5443.185,-495.1814;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightPos;79;-5458.969,-330.5507;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-5044.177,1519.358;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-2958.287,1769.568;Inherit;False;Property;_Scaleandoffsetshadows;Scale and offset shadows;6;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-2860.785,1643.888;Inherit;False;11;lightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;157;-4901.539,1526.682;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1185.405,-369.632;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;81;-5146.09,-231.9197;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-5167.698,-437.1821;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;156;-5006.23,1694.189;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;56;-2512.865,909.4227;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-967.4862,-362.8092;Inherit;False;diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;159;-5271.887,1082.177;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ScaleAndOffsetNode;22;-2649.287,1709.568;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;68;-2791.781,1279.628;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;-2782.381,1166.756;Inherit;False;11;lightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;57;-2345.926,908.4907;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;52;-1629.306,61.86906;Inherit;False;1320.589;541.0319;Comment;9;46;47;45;48;42;41;50;43;44;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-2432.583,1030.125;Inherit;False;Property;_Rimpower;Rim power;8;0;Create;True;0;0;False;0;0;0.16;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;160;-4662.843,1545.327;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-4526.103,518.9;Inherit;False;Property;_Speclighttransition;Spec light transition;13;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-4836.847,-175.5037;Inherit;False;Property;_Specvalue;Spec value;11;0;Create;True;0;0;False;0;0;1.18;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;99;-4694.973,259.3847;Inherit;False;Property;_Speccolor;Spec color;10;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;158;-5270.583,991.5604;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;100;-4682.122,456.9261;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.DotProductOpNode;83;-4930.025,-331.8546;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;161;-4764.447,1699.774;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;165;-4490.866,1632.349;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-2433.476,1731.74;Inherit;True;Property;_LightRamp;Light Ramp;3;0;Create;True;0;0;False;0;-1;None;ad447e0d503c98044ae3e05c389e9735;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;40;-2265.153,1649.142;Inherit;False;37;diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-4594.675,-214.0458;Inherit;False;Constant;_min;min;9;0;Create;True;0;0;False;0;2;7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-2564.504,1178.685;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-4576.746,1835.024;Float;False;Property;_DifferenceThreshold;Difference Threshold;16;0;Create;True;0;0;False;0;0.05;0.05;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-1579.307,379.9013;Inherit;False;28;normalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;95;-4537.886,10.41679;Inherit;True;Property;_Specular;Specular;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;-5012.956,1076.979;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;58;-2148.782,933.2247;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;84;-4651.841,-339.9525;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-4572.616,-112.1585;Inherit;False;Constant;_max;max;10;0;Create;True;0;0;False;0;2.1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;101;-4356.205,311.6241;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-4481.92,1751.42;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCIf;271;-4313.124,1629.841;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;412;-4853.151,1360.212;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;45;-1313.841,381.4023;Inherit;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-4075.943,16.85063;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2008.282,1785.43;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-1961.34,951.6218;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;86;-4416.107,-227.7017;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;64;-1949.166,1229.969;Inherit;False;Property;_Rimtint;Rim tint;9;1;[HDR];Create;True;0;0;False;0;0.5613652,0.8207547,0.7956936,0;0,1,0.909091,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;61;-1932.427,1084.487;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightAttenuation;47;-1295.307,491.9012;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;404;-4019.792,1543.259;Inherit;False;Property;_Contrast;Contrast;17;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-976.3067,380.9013;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;-4723.023,849.0089;Inherit;False;37;diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;75;-1727.093,966.7806;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;197;-4623.878,1205.972;Inherit;True;Property;_Reveallighttexture;Reveal light texture;4;0;Create;True;0;0;False;0;-1;2aed7015bc03a174fb153ccfe739a36f;2aed7015bc03a174fb153ccfe739a36f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;419;-4073.66,1457.531;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-4004.886,-216.7677;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;-1838.181,1828.311;Inherit;False;shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;42;-1101.144,217.2121;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;171;-4778.717,1019.016;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;200;-4534.238,1018.239;Inherit;False;Property;_Reveallightcolor;Reveal light color;15;1;[HDR];Create;True;0;0;False;0;1,0.07075471,0.07075471,0;1,0.07075471,0.07075471,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;90;-3692.957,-51.63457;Inherit;False;Property;_Specintensity;Spec intensity;12;0;Create;True;0;0;False;0;0;0.18;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1664.77,1185.366;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-828.9119,266.6102;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;430;-1688.492,849.4459;Inherit;False;Property;_RimLightEnabled;RimLightEnabled;21;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-1047.344,111.8691;Inherit;False;14;shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-3409.571,-217.6718;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;-4504.212,905.4919;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;407;-3873.634,1432.044;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;198;-4288.756,1263.491;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;92;-3499.662,-357.9193;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;421;-4226.12,1166.618;Inherit;False;Property;_Albedocontrast;Albedo contrast;18;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1487.017,944.1747;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-694.6523,160.1816;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-3246.662,-306.9196;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;432;-3380.846,-14.82561;Inherit;False;Property;_SpecEnabled;SpecEnabled;20;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;342;-3691.942,1286.3;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;422;-4004.288,1014.058;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;423;-1344.854,867.0137;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;431;-3611.116,1441.856;Inherit;False;Property;_RevealLightEnabled;RevealLightEnabled;19;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;60;-1245.958,1195.968;Inherit;False;rimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;429;-3115.52,-200.8544;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;105;-962.8774,823.6959;Inherit;False;1048.839;533.1951;Comment;8;98;94;73;74;15;0;114;115;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;343;-3492.676,1117.789;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-532.718,171.3251;Inherit;False;lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-2973.659,-88.91978;Inherit;False;spec;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;427;-3416.35,1282.484;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;-905.2711,879.0651;Inherit;False;60;rimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-919.7242,1059.941;Inherit;False;44;lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;373;-3285.086,1187.563;Inherit;False;handpaint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;-729.7849,1172.935;Inherit;False;93;spec;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-711.91,963.9327;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;-585.0919,1271.468;Inherit;False;373;handpaint;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;98;-554.2408,1045.347;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;114;-394.9169,1083.743;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-145.1671,875.9473;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;CellShadingShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;27;0
WireConnection;4;0;33;0
WireConnection;1;0;32;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;5;0;4;0
WireConnection;5;1;6;0
WireConnection;11;0;3;0
WireConnection;10;0;5;0
WireConnection;154;0;153;0
WireConnection;154;1;152;2
WireConnection;54;0;55;0
WireConnection;54;1;53;0
WireConnection;238;0;154;0
WireConnection;238;1;239;0
WireConnection;157;0;238;0
WireConnection;36;0;35;0
WireConnection;36;1;34;0
WireConnection;81;0;82;0
WireConnection;80;0;77;0
WireConnection;80;1;79;1
WireConnection;156;0;155;0
WireConnection;56;0;54;0
WireConnection;37;0;36;0
WireConnection;22;0;12;0
WireConnection;22;1;23;0
WireConnection;22;2;23;0
WireConnection;57;0;56;0
WireConnection;160;0;157;0
WireConnection;83;0;80;0
WireConnection;83;1;81;0
WireConnection;161;0;156;0
WireConnection;165;0;160;0
WireConnection;165;1;161;0
WireConnection;13;1;22;0
WireConnection;67;0;66;0
WireConnection;67;1;68;0
WireConnection;164;0;158;0
WireConnection;164;1;159;0
WireConnection;58;0;57;0
WireConnection;58;1;59;0
WireConnection;84;0;83;0
WireConnection;84;1;85;0
WireConnection;101;0;99;0
WireConnection;101;1;100;0
WireConnection;101;2;102;0
WireConnection;271;0;165;0
WireConnection;271;1;162;0
WireConnection;271;3;162;0
WireConnection;271;5;163;0
WireConnection;412;0;164;0
WireConnection;412;1;152;2
WireConnection;45;0;46;0
WireConnection;104;0;95;0
WireConnection;104;1;101;0
WireConnection;39;0;40;0
WireConnection;39;1;13;0
WireConnection;69;0;58;0
WireConnection;69;1;67;0
WireConnection;86;0;84;0
WireConnection;86;1;87;0
WireConnection;86;2;88;0
WireConnection;48;0;45;0
WireConnection;48;1;47;0
WireConnection;75;0;69;0
WireConnection;419;0;412;0
WireConnection;419;1;271;0
WireConnection;96;0;86;0
WireConnection;96;1;104;0
WireConnection;14;0;39;0
WireConnection;171;0;164;0
WireConnection;65;0;61;0
WireConnection;65;1;64;0
WireConnection;50;0;42;0
WireConnection;50;1;48;0
WireConnection;89;0;96;0
WireConnection;89;1;90;0
WireConnection;173;0;175;0
WireConnection;173;1;171;0
WireConnection;407;1;419;0
WireConnection;407;0;404;0
WireConnection;198;0;200;0
WireConnection;198;1;197;1
WireConnection;62;0;75;0
WireConnection;62;1;65;0
WireConnection;43;0;41;0
WireConnection;43;1;50;0
WireConnection;91;0;92;0
WireConnection;91;1;89;0
WireConnection;342;0;198;0
WireConnection;342;1;407;0
WireConnection;422;1;173;0
WireConnection;422;0;421;0
WireConnection;423;0;430;0
WireConnection;423;1;62;0
WireConnection;60;0;423;0
WireConnection;429;0;91;0
WireConnection;429;1;432;0
WireConnection;343;0;422;0
WireConnection;343;1;342;0
WireConnection;44;0;43;0
WireConnection;93;0;429;0
WireConnection;427;0;343;0
WireConnection;427;1;431;0
WireConnection;373;0;427;0
WireConnection;73;0;15;0
WireConnection;73;1;74;0
WireConnection;98;0;73;0
WireConnection;98;1;94;0
WireConnection;114;0;98;0
WireConnection;114;1;115;0
WireConnection;0;2;115;0
WireConnection;0;13;114;0
ASEEND*/
//CHKSM=B6B83D8052C8A9CFE7F1A97D9F50FFB0A9A30F11