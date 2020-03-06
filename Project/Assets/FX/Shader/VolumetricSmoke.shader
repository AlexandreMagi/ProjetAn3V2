// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Custom FX Shader/VolumetricSmoke"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Offset("Offset", Range( 0 , 1)) = 0
		_Contrast("Contrast", Range( 0 , 1)) = 0
		_Brigthness("Brigthness", Float) = 1
		_Softparticlesdistance("Soft particles distance", Float) = 1
		_Softparticlespower("Soft particles power", Range( 0 , 1)) = 1
		_Distancefalloff("Distance falloff", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
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
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float4 vertexColor : COLOR;
			float4 screenPos;
			float eyeDepth;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Transmission;
		};

		uniform float _Contrast;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Offset;
		uniform float _Brigthness;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Softparticlesdistance;
		uniform float _Softparticlespower;
		uniform float _Distancefalloff;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			half3 transmission = max(0 , -dot(s.Normal, gi.light.dir)) * gi.light.color * s.Transmission;
			half4 d = half4(s.Albedo * transmission , 0);

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + d;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			o.Normal = float3(0,0,1);
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 UV6 = uv0_MainTex;
			float MainTexture9 = tex2D( _MainTex, UV6 ).a;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 LightDirection13 = mul( ase_worldlightDir, ase_worldToTangent );
			float Offset32 = _Offset;
			float lerpResult30 = lerp( MainTexture9 , ( 1.0 - ( ( MainTexture9 - tex2D( _MainTex, ( float3( UV6 ,  0.0 ) + ( LightDirection13 * Offset32 ) ).xy ).a ) + 0.5 ) ) , 0.5);
			float Sample_0125 = saturate( lerpResult30 );
			float temp_output_52_0 = ( Offset32 * 0.5 );
			float lerpResult47 = lerp( MainTexture9 , ( 1.0 - ( ( MainTexture9 - tex2D( _MainTex, ( float3( UV6 ,  0.0 ) + ( LightDirection13 * temp_output_52_0 ) ).xy ).a ) + 0.5 ) ) , 0.5);
			float Sample_0249 = saturate( lerpResult47 );
			float lerpResult109 = lerp( Sample_0125 , Sample_0249 , 0.5);
			float temp_output_69_0 = ( temp_output_52_0 * 0.5 );
			float lerpResult68 = lerp( MainTexture9 , ( 1.0 - ( ( MainTexture9 - tex2D( _MainTex, ( float3( UV6 ,  0.0 ) + ( LightDirection13 * temp_output_69_0 ) ).xy ).a ) + 0.5 ) ) , 0.5);
			float Sample_0364 = saturate( lerpResult68 );
			float lerpResult110 = lerp( lerpResult109 , Sample_0364 , 0.5);
			float temp_output_85_0 = ( temp_output_69_0 * 0.5 );
			float lerpResult84 = lerp( MainTexture9 , ( 1.0 - ( ( MainTexture9 - tex2D( _MainTex, ( float3( UV6 ,  0.0 ) + ( LightDirection13 * temp_output_85_0 ) ).xy ).a ) + 0.5 ) ) , 0.5);
			float Sample_0480 = saturate( lerpResult84 );
			float lerpResult111 = lerp( lerpResult110 , Sample_0480 , 0.5);
			float lerpResult100 = lerp( MainTexture9 , ( 1.0 - ( ( MainTexture9 - tex2D( _MainTex, ( float3( UV6 ,  0.0 ) + ( LightDirection13 * ( temp_output_85_0 * 0.5 ) ) ).xy ).a ) + 0.5 ) ) , 0.5);
			float Sample_0596 = saturate( lerpResult100 );
			float lerpResult112 = lerp( lerpResult111 , Sample_0596 , 0.5);
			float AllSamples113 = lerpResult112;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 Combined124 = saturate( ( float4( ( ( ( 1.0 - ( _Contrast + AllSamples113 ) ) * _Brigthness ) + ( ase_lightColor.rgb * ( ase_lightColor.a * 0.1 ) ) ) , 0.0 ) * i.vertexColor ) );
			o.Albedo = Combined124.rgb;
			float3 temp_cast_12 = (0.1).xxx;
			o.Transmission = temp_cast_12;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth143 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth143 = abs( ( screenDepth143 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Softparticlesdistance ) );
			float clampResult147 = clamp( ( distanceDepth143 * _Softparticlespower ) , 0.0 , 1.0 );
			float SoftParticles150 = clampResult147;
			float cameraDepthFade169 = (( i.eyeDepth -_ProjectionParams.y - 0.0 ) / 1.0);
			float clampResult170 = clamp( ( cameraDepthFade169 * _Distancefalloff ) , 0.0 , 1.0 );
			float DistanceFalloff163 = clampResult170;
			float Alpha129 = ( ( ( i.vertexColor.a * MainTexture9 ) * SoftParticles150 ) * DistanceFalloff163 );
			o.Alpha = Alpha129;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustom alpha:fade keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

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
				float3 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
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
				vertexDataFunc( v, customInputData );
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
				o.customPack1.z = customInputData.eyeDepth;
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
				surfIN.eyeDepth = IN.customPack1.z;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandardCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardCustom, o )
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
563;284;1437;594;2050.862;1606.764;2.129277;True;False
Node;AmplifyShaderEditor.CommentaryNode;14;-1893.088,-278.7723;Inherit;False;737.5463;380.47;Comment;4;10;11;12;13;LightDirection;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;33;-2566.851,-97.27917;Inherit;False;601.335;192.2287;Comment;2;17;32;Offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;7;-2565.308,-814.533;Inherit;False;1275.295;413.9059;Comment;5;5;8;6;2;9;Main texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldToTangentMatrix;12;-1836.371,-9.302032;Inherit;False;0;1;FLOAT3x3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;10;-1843.088,-228.7722;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;17;-2519.851,-24.27917;Inherit;False;Property;_Offset;Offset;1;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-2189.516,-21.05045;Inherit;False;Offset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;35;-2669.982,977.3318;Inherit;False;2444.05;551.2091;Comment;15;52;42;45;46;44;48;37;39;40;38;49;41;43;36;47;Sample_02;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;5;-2501.393,-625.395;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;e1718afb08607094fae574e0bda1a59c;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-1557.684,-127.2084;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-2250.933,-554.2006;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;36;-2539.79,1351.297;Inherit;False;32;Offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1379.542,-123.9506;Inherit;False;LightDirection;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;31;-2610.267,311.9779;Inherit;False;2260.85;494.9999;Comment;14;15;19;16;18;20;21;23;22;24;27;28;25;30;34;Sample_01;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-2488.402,702.5973;Inherit;False;32;Offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;-2524.317,596.0627;Inherit;False;13;LightDirection;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;6;-2019.063,-551.8672;Inherit;False;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-2575.705,1244.763;Inherit;False;13;LightDirection;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-2321.745,1381.688;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;54;-2683.627,1682.33;Inherit;False;2444.05;551.2091;Comment;14;69;68;66;65;64;63;62;61;60;59;58;57;56;55;Sample_03;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-2114.145,1308.694;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-2237.629,643.3395;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;70;-2689.751,2402.872;Inherit;False;2444.05;551.2091;Comment;14;85;84;82;81;80;79;78;77;76;75;74;73;72;71;Sample_04;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;39;-2131.292,1207.919;Inherit;False;6;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-2254.777,542.5653;Inherit;False;6;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-2589.35,1949.762;Inherit;False;13;LightDirection;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;8;-1796.984,-735.9972;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-2335.39,2086.689;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;41;-1903.593,1279.593;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;40;-1947.496,1056.362;Inherit;True;Property;_MainTex;MainTex;0;0;Fetch;True;0;0;False;0;None;e1718afb08607094fae574e0bda1a59c;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-2127.79,2013.694;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-2341.514,2807.228;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-2027.078,614.2388;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;76;-2595.474,2670.303;Inherit;False;13;LightDirection;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-1489.038,-679.3181;Inherit;False;MainTexture;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;-2144.937,1912.918;Inherit;False;6;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;20;-2070.98,391.0083;Inherit;True;Property;_MainTex;MainTex;0;0;Fetch;True;0;0;False;0;None;e1718afb08607094fae574e0bda1a59c;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.CommentaryNode;86;-2685.702,3146.354;Inherit;False;2444.05;551.2091;Comment;14;101;100;98;97;96;95;94;93;92;91;90;89;88;87;Sample_05;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-2337.465,3550.71;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;23;-1703.099,361.9779;Inherit;False;9;MainTexture;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-1917.238,1984.592;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;21;-1797.041,535.1227;Inherit;True;Property;_TextureSample1;Texture Sample 1;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;43;-1579.614,1053.806;Inherit;False;9;MainTexture;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;-2151.061,2633.459;Inherit;False;6;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-2133.914,2734.234;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;62;-1961.141,1761.361;Inherit;True;Property;_MainTex;MainTex;0;0;Fetch;True;0;0;False;0;None;e1718afb08607094fae574e0bda1a59c;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;42;-1673.556,1200.477;Inherit;True;Property;_TextureSample2;Texture Sample 2;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;92;-2591.425,3413.785;Inherit;False;13;LightDirection;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;-1593.259,1758.805;Inherit;False;9;MainTexture;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;78;-1967.265,2481.902;Inherit;True;Property;_MainTex;MainTex;0;0;Fetch;True;0;0;False;0;None;e1718afb08607094fae574e0bda1a59c;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;81;-1923.362,2705.133;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;55;-1687.201,1905.476;Inherit;True;Property;_TextureSample3;Texture Sample 3;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;44;-1326.109,1187.864;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;22;-1449.594,522.5095;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-2147.012,3376.941;Inherit;False;6;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-2129.865,3477.716;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;58;-1339.754,1892.863;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-1599.383,2479.346;Inherit;False;9;MainTexture;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-1154.563,1189.5;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;71;-1693.325,2626.017;Inherit;True;Property;_TextureSample4;Texture Sample 4;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;94;-1963.214,3225.384;Inherit;True;Property;_MainTex;MainTex;0;0;Fetch;True;0;0;False;0;None;e1718afb08607094fae574e0bda1a59c;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1278.049,524.1464;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-1919.311,3448.615;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;46;-1011.39,1198.09;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;74;-1345.878,2613.404;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;27;-1134.876,532.7362;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;-1168.208,1894.499;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;-1595.332,3222.828;Inherit;False;9;MainTexture;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;87;-1689.274,3369.499;Inherit;True;Property;_TextureSample5;Texture Sample 5;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;47;-812.9468,1107.341;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;90;-1341.827,3356.886;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-1174.332,2615.04;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;57;-1025.035,1903.089;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;30;-936.4323,441.9873;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;28;-738.5461,530.7266;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;73;-1031.16,2623.63;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;48;-615.0607,1196.081;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;88;-1170.281,3358.522;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;68;-826.592,1812.34;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-573.4181,524.3649;Inherit;False;Sample_01;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;84;-832.7166,2532.881;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;59;-628.706,1901.08;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;89;-1027.108,3367.112;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-449.9325,1189.719;Inherit;False;Sample_02;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;114;-252.6929,224.3905;Inherit;False;1183.124;665.7948;Comment;10;103;109;110;111;112;107;106;105;104;113;All Samples;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;-200.8864,274.3905;Inherit;False;25;Sample_01;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;64;-463.5777,1894.718;Inherit;False;Sample_03;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;100;-828.666,3276.363;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;-202.6929,397.8023;Inherit;False;49;Sample_02;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;75;-634.8304,2621.621;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;-42.63087,527.8823;Inherit;False;64;Sample_03;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;91;-630.7799,3365.103;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-469.7021,2615.259;Inherit;False;Sample_04;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;109;56.58096,300.8105;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;110;219.581,412.8104;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;96;-465.6512,3358.741;Inherit;False;Sample_05;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;106;116.5553,650.1045;Inherit;False;80;Sample_04;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;268.688,774.1847;Inherit;False;96;Sample_05;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;111;364.5809,511.81;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;141;-955.1324,-952.6198;Inherit;False;1508.377;337.2766;Comment;6;150;143;142;144;146;147;Edge detection;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;112;511.5807,613.8099;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;140;-964.4158,-540.5218;Inherit;False;1670.863;655.6391;Comment;15;116;118;134;117;136;135;119;121;120;137;138;132;131;123;124;Colors;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;706.4307,628.5784;Inherit;False;AllSamples;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-914.0049,-877.6175;Inherit;False;Property;_Softparticlesdistance;Soft particles distance;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-451.4575,-721.4393;Inherit;False;Property;_Softparticlespower;Soft particles power;5;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;116;-914.4158,-490.5219;Inherit;False;Property;_Contrast;Contrast;2;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;164;-900.3539,-1604.923;Inherit;False;918.2714;368.0299;Comment;5;163;162;169;161;170;Distance Falloff;1,1,1,1;0;0
Node;AmplifyShaderEditor.DepthFade;143;-647.0599,-883.187;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;-847.6275,-360.9443;Inherit;False;113;AllSamples;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-850.3539,-1345.893;Inherit;False;Property;_Distancefalloff;Distance falloff;6;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;169;-818.7958,-1514.82;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;-133.3055,-833.9661;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;117;-574.0844,-453.1259;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;136;-575.6995,-0.8829694;Inherit;False;Constant;_Float0;Float 0;9;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;134;-615.7589,-158.306;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;-550.0166,-1457.568;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;147;31.09518,-824.4933;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;121;-390.7474,-266.0534;Inherit;False;Property;_Brigthness;Brigthness;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-386.8734,-85.08954;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;130;-2275.246,-1827.137;Inherit;False;1129.736;481.6372;Comment;8;129;158;127;154;128;126;166;165;Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;119;-397.2474,-432.4535;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-199.6472,-327.1533;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;170;-413.7958,-1448.82;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;150;247.2206,-828.1931;Inherit;False;SoftParticles;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;137;-236.6999,-157.8828;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;-2225.245,-1559.673;Inherit;False;9;MainTexture;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;126;-2224.39,-1777.137;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;163;-262.0825,-1459.329;Inherit;False;DistanceFalloff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;132;-40.52429,-93.38612;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-2012.044,-1655.875;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;138;-47.36277,-236.2798;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;-2063.899,-1473.407;Inherit;False;150;SoftParticles;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;143.4549,-190.4796;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;-1737.25,-1667.925;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;166;-1708.422,-1462.874;Inherit;False;163;DistanceFalloff;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;123;322.2323,-185.2175;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;165;-1580.422,-1649.874;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;482.447,-230.6248;Inherit;False;Combined;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;102;115.0082,1097.905;Inherit;False;508.5089;521;Comment;4;26;0;125;139;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;129;-1410.77,-1664.373;Inherit;False;Alpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;26;165.0082,1150.624;Inherit;False;124;Combined;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;177.9474,1428.572;Inherit;False;129;Alpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;139;193.9283,1307.714;Inherit;False;Constant;_Transmission;Transmission;9;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;368.5172,1147.905;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_Custom FX Shader/VolumetricSmoke;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;17;0
WireConnection;11;0;10;0
WireConnection;11;1;12;0
WireConnection;2;2;5;0
WireConnection;13;0;11;0
WireConnection;6;0;2;0
WireConnection;52;0;36;0
WireConnection;38;0;37;0
WireConnection;38;1;52;0
WireConnection;16;0;15;0
WireConnection;16;1;34;0
WireConnection;8;0;5;0
WireConnection;8;1;6;0
WireConnection;69;0;52;0
WireConnection;41;0;39;0
WireConnection;41;1;38;0
WireConnection;63;0;60;0
WireConnection;63;1;69;0
WireConnection;85;0;69;0
WireConnection;18;0;19;0
WireConnection;18;1;16;0
WireConnection;9;0;8;4
WireConnection;101;0;85;0
WireConnection;65;0;61;0
WireConnection;65;1;63;0
WireConnection;21;0;20;0
WireConnection;21;1;18;0
WireConnection;79;0;76;0
WireConnection;79;1;85;0
WireConnection;42;0;40;0
WireConnection;42;1;41;0
WireConnection;81;0;77;0
WireConnection;81;1;79;0
WireConnection;55;0;62;0
WireConnection;55;1;65;0
WireConnection;44;0;43;0
WireConnection;44;1;42;4
WireConnection;22;0;23;0
WireConnection;22;1;21;4
WireConnection;95;0;92;0
WireConnection;95;1;101;0
WireConnection;58;0;66;0
WireConnection;58;1;55;4
WireConnection;45;0;44;0
WireConnection;71;0;78;0
WireConnection;71;1;81;0
WireConnection;24;0;22;0
WireConnection;97;0;93;0
WireConnection;97;1;95;0
WireConnection;46;0;45;0
WireConnection;74;0;82;0
WireConnection;74;1;71;4
WireConnection;27;0;24;0
WireConnection;56;0;58;0
WireConnection;87;0;94;0
WireConnection;87;1;97;0
WireConnection;47;0;43;0
WireConnection;47;1;46;0
WireConnection;90;0;98;0
WireConnection;90;1;87;4
WireConnection;72;0;74;0
WireConnection;57;0;56;0
WireConnection;30;0;23;0
WireConnection;30;1;27;0
WireConnection;28;0;30;0
WireConnection;73;0;72;0
WireConnection;48;0;47;0
WireConnection;88;0;90;0
WireConnection;68;0;66;0
WireConnection;68;1;57;0
WireConnection;25;0;28;0
WireConnection;84;0;82;0
WireConnection;84;1;73;0
WireConnection;59;0;68;0
WireConnection;89;0;88;0
WireConnection;49;0;48;0
WireConnection;64;0;59;0
WireConnection;100;0;98;0
WireConnection;100;1;89;0
WireConnection;75;0;84;0
WireConnection;91;0;100;0
WireConnection;80;0;75;0
WireConnection;109;0;103;0
WireConnection;109;1;104;0
WireConnection;110;0;109;0
WireConnection;110;1;105;0
WireConnection;96;0;91;0
WireConnection;111;0;110;0
WireConnection;111;1;106;0
WireConnection;112;0;111;0
WireConnection;112;1;107;0
WireConnection;113;0;112;0
WireConnection;143;0;142;0
WireConnection;146;0;143;0
WireConnection;146;1;144;0
WireConnection;117;0;116;0
WireConnection;117;1;118;0
WireConnection;162;0;169;0
WireConnection;162;1;161;0
WireConnection;147;0;146;0
WireConnection;135;0;134;2
WireConnection;135;1;136;0
WireConnection;119;0;117;0
WireConnection;120;0;119;0
WireConnection;120;1;121;0
WireConnection;170;0;162;0
WireConnection;150;0;147;0
WireConnection;137;0;134;1
WireConnection;137;1;135;0
WireConnection;163;0;170;0
WireConnection;127;0;126;4
WireConnection;127;1;128;0
WireConnection;138;0;120;0
WireConnection;138;1;137;0
WireConnection;131;0;138;0
WireConnection;131;1;132;0
WireConnection;158;0;127;0
WireConnection;158;1;154;0
WireConnection;123;0;131;0
WireConnection;165;0;158;0
WireConnection;165;1;166;0
WireConnection;124;0;123;0
WireConnection;129;0;165;0
WireConnection;0;0;26;0
WireConnection;0;6;139;0
WireConnection;0;9;125;0
ASEEND*/
//CHKSM=B7329727C88CF3416459E84DC5F32E5B6CE94320