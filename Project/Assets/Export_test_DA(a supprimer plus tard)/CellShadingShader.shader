// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "CellShadingShader"
{
	Properties
	{
		_Diffuse("Diffuse", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_LightRamp("Light Ramp", 2D) = "white" {}
		_ColortoBeFiltered("Color to Be Filtered", Color) = (0.6981132,0.25356,0.25356,1)
		_Specular("Specular", 2D) = "white" {}
		_DifferenceThreshold("Difference Threshold", Range( 0 , 0.05)) = 0.05
		[HDR]_Tint("Tint", Color) = (0.5,0.5,0.5,0)
		_Scaleandoffsetshadows("Scale and offset shadows", Range( 0 , 1)) = 0.5
		_Rimoffset("Rim offset", Float) = 1
		_Rimpower("Rim power", Range( 0 , 1)) = 0
		[HDR]_Rimtint("Rim tint", Color) = (0.5613652,0.8207547,0.7956936,0)
		[HDR]_Speccolor("Spec color", Color) = (0,0,0,0)
		_Specvalue("Spec value", Float) = 0
		_Specintensity("Spec intensity", Range( 0 , 1)) = 0
		_Speclighttransition("Spec light transition", Range( 0 , 1)) = 0
		_Painttexture("Paint texture", 2D) = "white" {}
		[HDR]_Paintcolor("Paint color", Color) = (1,0.07075471,0.07075471,0)
		_Contrast("Contrast", Float) = 0
		_Albedocontrast("Albedo contrast", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
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
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
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

		uniform float _Rimoffset;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _Rimpower;
		uniform float4 _Rimtint;
		uniform float4 _Tint;
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform sampler2D _LightRamp;
		uniform float _Scaleandoffsetshadows;
		uniform float _Specvalue;
		uniform sampler2D _Specular;
		uniform float4 _Specular_ST;
		uniform float4 _Speccolor;
		uniform float _Speclighttransition;
		uniform float _Specintensity;
		uniform float _Albedocontrast;
		uniform float4 _Paintcolor;
		uniform sampler2D _Painttexture;
		uniform float4 _Painttexture_ST;
		uniform float _Contrast;
		uniform float4 _ColortoBeFiltered;
		uniform float _DifferenceThreshold;


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
			float4 rimLight60 = ( saturate( ( pow( ( 1.0 - saturate( ( _Rimoffset + lightView10 ) ) ) , _Rimpower ) * ( lightDir11 * ase_lightAtten ) ) ) * ( ase_lightColor * _Rimtint ) );
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 diffuse37 = ( _Tint * tex2D( _Diffuse, uv_Diffuse ) );
			float2 temp_cast_0 = ((lightDir11*_Scaleandoffsetshadows + _Scaleandoffsetshadows)).xx;
			float4 shadow14 = ( diffuse37 * tex2D( _LightRamp, temp_cast_0 ) );
			UnityGI gi45 = gi;
			float3 diffNorm45 = WorldNormalVector( i , normalMap28 );
			gi45 = UnityGI_Base( data, 1, diffNorm45 );
			float3 indirectDiffuse45 = gi45.indirect.diffuse + diffNorm45 * 0.0001;
			float4 lighting44 = ( shadow14 * ( ase_lightColor * float4( ( indirectDiffuse45 + ase_lightAtten ) , 0.0 ) ) );
			float dotResult83 = dot( ( ase_worldViewDir + _WorldSpaceLightPos0.xyz ) , normalize( (WorldNormalVector( i , normalMap28 )) ) );
			float smoothstepResult86 = smoothstep( 2.0 , 2.1 , pow( dotResult83 , _Specvalue ));
			float2 uv_Specular = i.uv_texcoord * _Specular_ST.xy + _Specular_ST.zw;
			float4 lerpResult101 = lerp( _Speccolor , ase_lightColor , _Speclighttransition);
			float4 spec93 = ( ase_lightAtten * ( ( smoothstepResult86 * ( tex2D( _Specular, uv_Specular ) * lerpResult101 ) ) * _Specintensity ) );
			float4 temp_output_164_0 = ( ase_lightAtten * ase_lightColor );
			float2 uv_Painttexture = i.uv_texcoord * _Painttexture_ST.xy + _Painttexture_ST.zw;
			float3 normalizeResult160 = normalize( (( ( ase_lightColor * _WorldSpaceLightPos0.w ) * lightDir11 )).rgb );
			float3 normalizeResult161 = normalize( (_ColortoBeFiltered).rgb );
			float dotResult165 = dot( normalizeResult160 , normalizeResult161 );
			float4 handpaint373 = ( CalculateContrast(_Albedocontrast,( diffuse37 * saturate( temp_output_164_0 ) )) + ( ( _Paintcolor * tex2D( _Painttexture, uv_Painttexture ).r ) * CalculateContrast(_Contrast,( ( temp_output_164_0 * _WorldSpaceLightPos0.w ) *  ( dotResult165 - _DifferenceThreshold > 1.0 ? 0.0 : dotResult165 - _DifferenceThreshold <= 1.0 && dotResult165 + _DifferenceThreshold >= 1.0 ? 1.0 : 0.0 )  )) ) );
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
0;73;1312;396;8219.272;900.3502;8.80171;True;False
Node;AmplifyShaderEditor.CommentaryNode;29;-1346.357,-723.4073;Inherit;False;616.2219;280;Comment;2;28;27;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;27;-1296.356,-673.4071;Inherit;True;Property;_Normal;Normal;1;0;Create;True;0;0;False;0;-1;None;94c473c9137744d7eabd829f1150da8d;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-954.132,-637.9576;Inherit;False;normalMap;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;9;-1647.621,-266.611;Inherit;False;967.5994;445.123;Comment;5;11;3;1;2;32;LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;8;-1644.768,280.0073;Inherit;False;955.9893;445.9081;Comment;5;10;5;6;4;33;LightView;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;32;-1605.231,-221.219;Inherit;False;28;normalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-1614.331,321.3979;Inherit;False;28;normalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;1;-1319.817,-216.611;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;2;-1321.022,-33.88268;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;4;-1325.448,330.0073;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;6;-1304.959,522.4801;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;3;-1024.021,-114.8827;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;149;-5518.278,752.9568;Inherit;False;2417.002;1270.554;Comment;32;343;342;173;171;175;198;271;200;197;165;163;164;162;159;161;160;158;156;157;238;155;154;239;153;152;373;404;407;412;419;421;422;RevealLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;5;-1060.96,381.48;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;152;-5405.496,1264.572;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;153;-5441.361,1489.559;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;76;-2893.847,838.0521;Inherit;False;1953.216;681.7428;Comment;17;53;55;54;56;57;59;58;64;61;69;65;75;62;60;68;66;67;Rim light;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-884.9761,408.0676;Inherit;False;lightView;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-873.2495,-105.4723;Inherit;False;lightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;38;-524.9953,256.2808;Inherit;False;883.0335;473.1851;Comment;4;34;35;36;37;Diffuse;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;239;-5288.269,1646.16;Inherit;False;11;lightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;26;-595.7087,-279.9429;Inherit;False;1360.203;357.2934;Comment;7;14;39;13;40;22;23;12;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;97;-4461.179,-496.6599;Inherit;False;2753.833;1170.912;Comment;23;93;91;89;92;90;96;86;95;87;84;88;85;83;80;81;82;79;77;99;101;102;104;100;Spec;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-2843.847,1031.421;Inherit;False;10;lightView;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-2760.543,888.0521;Inherit;False;Property;_Rimoffset;Rim offset;8;0;Create;True;0;0;False;0;1;0.62;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;154;-5179.378,1494.401;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;34;-474.9955,499.4661;Inherit;True;Property;_Diffuse;Diffuse;0;0;Create;True;0;0;False;0;-1;None;0a0e8af2869fa45f78e25460db578482;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;35;-431.3724,306.281;Inherit;False;Property;_Tint;Tint;6;1;[HDR];Create;True;0;0;False;0;0.5,0.5,0.5,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldSpaceLightPos;79;-4411.179,-282.0295;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-2578.643,986.7521;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-5044.177,1519.358;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-545.7085,-104.2213;Inherit;False;Property;_Scaleandoffsetshadows;Scale and offset shadows;7;0;Create;True;0;0;False;0;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-448.2063,-229.9007;Inherit;False;11;lightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;155;-5290.3,1763.469;Float;False;Property;_ColortoBeFiltered;Color to Be Filtered;3;0;Create;True;0;0;False;0;0.6981132,0.25356,0.25356,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;77;-4395.395,-446.66;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;82;-4302.209,-152.3444;Inherit;False;28;normalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-83.88008,432.5651;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-4119.908,-388.6609;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;134.0384,439.3879;Inherit;False;diffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;22;-236.7086,-164.2211;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;156;-5006.23,1694.189;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;157;-4901.539,1526.682;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;56;-2381.16,987.248;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;52;-869.0151,893.999;Inherit;False;1320.589;541.0319;Comment;9;46;47;45;48;42;41;50;43;44;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;81;-4098.3,-183.3984;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;66;-2650.676,1244.581;Inherit;False;11;lightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;68;-2660.076,1357.453;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-20.89709,-142.0489;Inherit;True;Property;_LightRamp;Light Ramp;2;0;Create;True;0;0;False;0;-1;None;ad447e0d503c98044ae3e05c389e9735;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;102;-3478.311,567.4213;Inherit;False;Property;_Speclighttransition;Spec light transition;14;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-2300.878,1107.95;Inherit;False;Property;_Rimpower;Rim power;9;0;Create;True;0;0;False;0;0;0.16;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;57;-2214.221,986.316;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-819.0151,1212.031;Inherit;False;28;normalMap;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;147.4257,-224.6473;Inherit;False;37;diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;100;-3634.331,505.4473;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.DotProductOpNode;83;-3882.235,-283.3334;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;160;-4662.843,1545.327;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;158;-5270.583,991.5604;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;99;-3647.182,307.906;Inherit;False;Property;_Speccolor;Spec color;11;1;[HDR];Create;True;0;0;False;0;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;159;-5271.887,1082.177;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;85;-3789.057,-126.9823;Inherit;False;Property;_Specvalue;Spec value;12;0;Create;True;0;0;False;0;0;1.18;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;161;-4764.447,1699.774;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-4576.746,1835.024;Float;False;Property;_DifferenceThreshold;Difference Threshold;5;0;Create;True;0;0;False;0;0.05;0.05;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;47;-535.0151,1324.031;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-4481.92,1751.42;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-3546.883,-165.5244;Inherit;False;Constant;_min;min;9;0;Create;True;0;0;False;0;2;7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;58;-2017.078,1011.05;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-2432.799,1256.51;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;-5012.956,1076.979;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;95;-3490.095,58.93813;Inherit;True;Property;_Specular;Specular;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;101;-3308.413,360.1453;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;404.2965,-88.35849;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;45;-553.5491,1213.532;Inherit;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;88;-3524.825,-63.63707;Inherit;False;Constant;_max;max;10;0;Create;True;0;0;False;0;2.1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;84;-3604.05,-291.4313;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;165;-4490.866,1632.349;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;64;-1817.461,1307.794;Inherit;False;Property;_Rimtint;Rim tint;10;1;[HDR];Create;True;0;0;False;0;0.5613652,0.8207547,0.7956936,0;0,1,0.909091,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;86;-3368.316,-179.1804;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-216.0151,1213.031;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCIf;271;-4313.124,1629.841;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;412;-4853.151,1360.212;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-3028.152,65.37197;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;61;-1800.722,1162.312;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-1829.635,1029.447;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;14;574.3979,-45.47767;Inherit;False;shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;42;-340.8528,1049.342;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;197;-4623.878,1205.972;Inherit;True;Property;_Painttexture;Paint texture;15;0;Create;True;0;0;False;0;-1;2aed7015bc03a174fb153ccfe739a36f;2aed7015bc03a174fb153ccfe739a36f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;171;-4778.717,1019.016;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;419;-4073.66,1457.531;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;75;-1595.388,1044.606;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;404;-4019.792,1543.259;Inherit;False;Property;_Contrast;Contrast;17;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;65;-1533.065,1263.191;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-2645.166,-3.11319;Inherit;False;Property;_Specintensity;Spec intensity;13;0;Create;True;0;0;False;0;0;0.18;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;-4723.023,849.0089;Inherit;False;37;diffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-287.0527,943.999;Inherit;False;14;shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-68.6203,1098.74;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-2957.095,-168.2463;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;200;-4534.238,1018.239;Inherit;False;Property;_Paintcolor;Paint color;16;1;[HDR];Create;True;0;0;False;0;1,0.07075471,0.07075471,0;1,0.07075471,0.07075471,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;65.63934,992.3115;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;173;-4504.212,905.4919;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;-2361.78,-169.1504;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;407;-3873.634,1432.044;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1355.312,1022;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;92;-2451.871,-309.3981;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;198;-4288.756,1263.491;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;421;-4226.12,1166.618;Inherit;False;Property;_Albedocontrast;Albedo contrast;18;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;342;-3691.942,1286.3;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-2198.871,-258.3984;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;60;-1164.631,1011.196;Inherit;False;rimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;227.5734,1003.455;Inherit;False;lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;105;918.9303,286.1003;Inherit;False;1048.839;533.1951;Comment;8;98;94;73;74;15;0;114;115;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;422;-4004.288,1014.058;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-1970.869,-269.3984;Inherit;False;spec;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;976.5367,341.4695;Inherit;False;60;rimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;343;-3492.676,1117.789;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;962.0836,522.3456;Inherit;False;44;lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;1169.898,426.3372;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;1152.023,635.3395;Inherit;False;93;spec;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;373;-3312.786,1108.563;Inherit;False;handpaint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;1296.716,733.8727;Inherit;False;373;handpaint;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;98;1327.567,507.7513;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;114;1486.891,546.1471;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1736.641,338.3517;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;CellShadingShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;27;0
WireConnection;1;0;32;0
WireConnection;4;0;33;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;5;0;4;0
WireConnection;5;1;6;0
WireConnection;10;0;5;0
WireConnection;11;0;3;0
WireConnection;154;0;153;0
WireConnection;154;1;152;2
WireConnection;54;0;55;0
WireConnection;54;1;53;0
WireConnection;238;0;154;0
WireConnection;238;1;239;0
WireConnection;36;0;35;0
WireConnection;36;1;34;0
WireConnection;80;0;77;0
WireConnection;80;1;79;1
WireConnection;37;0;36;0
WireConnection;22;0;12;0
WireConnection;22;1;23;0
WireConnection;22;2;23;0
WireConnection;156;0;155;0
WireConnection;157;0;238;0
WireConnection;56;0;54;0
WireConnection;81;0;82;0
WireConnection;13;1;22;0
WireConnection;57;0;56;0
WireConnection;83;0;80;0
WireConnection;83;1;81;0
WireConnection;160;0;157;0
WireConnection;161;0;156;0
WireConnection;58;0;57;0
WireConnection;58;1;59;0
WireConnection;67;0;66;0
WireConnection;67;1;68;0
WireConnection;164;0;158;0
WireConnection;164;1;159;0
WireConnection;101;0;99;0
WireConnection;101;1;100;0
WireConnection;101;2;102;0
WireConnection;39;0;40;0
WireConnection;39;1;13;0
WireConnection;45;0;46;0
WireConnection;84;0;83;0
WireConnection;84;1;85;0
WireConnection;165;0;160;0
WireConnection;165;1;161;0
WireConnection;86;0;84;0
WireConnection;86;1;87;0
WireConnection;86;2;88;0
WireConnection;48;0;45;0
WireConnection;48;1;47;0
WireConnection;271;0;165;0
WireConnection;271;1;162;0
WireConnection;271;3;162;0
WireConnection;271;5;163;0
WireConnection;412;0;164;0
WireConnection;412;1;152;2
WireConnection;104;0;95;0
WireConnection;104;1;101;0
WireConnection;69;0;58;0
WireConnection;69;1;67;0
WireConnection;14;0;39;0
WireConnection;171;0;164;0
WireConnection;419;0;412;0
WireConnection;419;1;271;0
WireConnection;75;0;69;0
WireConnection;65;0;61;0
WireConnection;65;1;64;0
WireConnection;50;0;42;0
WireConnection;50;1;48;0
WireConnection;96;0;86;0
WireConnection;96;1;104;0
WireConnection;43;0;41;0
WireConnection;43;1;50;0
WireConnection;173;0;175;0
WireConnection;173;1;171;0
WireConnection;89;0;96;0
WireConnection;89;1;90;0
WireConnection;407;1;419;0
WireConnection;407;0;404;0
WireConnection;62;0;75;0
WireConnection;62;1;65;0
WireConnection;198;0;200;0
WireConnection;198;1;197;1
WireConnection;342;0;198;0
WireConnection;342;1;407;0
WireConnection;91;0;92;0
WireConnection;91;1;89;0
WireConnection;60;0;62;0
WireConnection;44;0;43;0
WireConnection;422;1;173;0
WireConnection;422;0;421;0
WireConnection;93;0;91;0
WireConnection;343;0;422;0
WireConnection;343;1;342;0
WireConnection;73;0;15;0
WireConnection;73;1;74;0
WireConnection;373;0;343;0
WireConnection;98;0;73;0
WireConnection;98;1;94;0
WireConnection;114;0;98;0
WireConnection;114;1;115;0
WireConnection;0;13;114;0
ASEEND*/
//CHKSM=4892724539C740C7641AB50AC9EE1FC03D4C6E5D