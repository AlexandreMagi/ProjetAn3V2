// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GravityOrb"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 15
		_Strength("Strength", Float) = 1
		_FlowMap("FlowMap", 2D) = "white" {}
		_Speed("Speed", Float) = 1
		_Emissive("Emissive", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		[HDR]_Emissivecolor("Emissive color", Color) = (0,0,0,0)
		_NormalScale("Normal Scale", Range( 0 , 1)) = 0
		_ShadowTransform("Shadow Transform", Range( 0 , 5)) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
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
			float4 screenPos;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform sampler2D _Emissive;
		uniform sampler2D _FlowMap;
		uniform float4 _FlowMap_ST;
		uniform float _Speed;
		uniform float _Strength;
		uniform float4 _Emissivecolor;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform sampler2D _Normal;
		uniform float _NormalScale;
		uniform float _ShadowTransform;
		uniform float _EdgeLength;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float2 uv_FlowMap = i.uv_texcoord * _FlowMap_ST.xy + _FlowMap_ST.zw;
			float2 blendOpSrc89 = i.uv_texcoord;
			float2 blendOpDest89 = (tex2D( _FlowMap, uv_FlowMap )).rg;
			float2 temp_output_89_0 = ( saturate( (( blendOpDest89 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest89 ) * ( 1.0 - blendOpSrc89 ) ) : ( 2.0 * blendOpDest89 * blendOpSrc89 ) ) ));
			float temp_output_67_0 = ( _Time.y * _Speed );
			float temp_output_1_0_g3 = temp_output_67_0;
			float temp_output_72_0 = (0.0 + (( ( temp_output_1_0_g3 - floor( ( temp_output_1_0_g3 + 0.5 ) ) ) * 2 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0));
			float timeA86 = ( -temp_output_72_0 * _Strength );
			float2 lerpResult95 = lerp( i.uv_texcoord , temp_output_89_0 , timeA86);
			float2 temp_cast_0 = (1.0).xx;
			float2 uv_TexCoord85 = i.uv_texcoord * temp_cast_0;
			float2 tilling90 = uv_TexCoord85;
			float2 flowA102 = ( lerpResult95 + tilling90 );
			float temp_output_1_0_g4 = (temp_output_67_0*1.0 + 0.5);
			float timeB84 = ( -(0.0 + (( ( temp_output_1_0_g4 - floor( ( temp_output_1_0_g4 + 0.5 ) ) ) * 2 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) * _Strength );
			float2 lerpResult92 = lerp( i.uv_texcoord , temp_output_89_0 , timeB84);
			float2 flowB101 = ( lerpResult92 + tilling90 );
			float timeBlend112 = saturate( abs( ( 1.0 - ( temp_output_72_0 / 0.5 ) ) ) );
			float4 lerpResult121 = lerp( ( tex2D( _Emissive, flowA102 ) * _Emissivecolor ) , ( tex2D( _Emissive, flowB101 ) * _Emissivecolor ) , timeBlend112);
			float4 emissive124 = lerpResult121;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float3 lerpResult122 = lerp( UnpackScaleNormal( tex2D( _Normal, flowA102 ), _NormalScale ) , UnpackScaleNormal( tex2D( _Normal, flowB101 ), _NormalScale ) , timeBlend112);
			float3 normal123 = lerpResult122;
			float4 screenColor60 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + float4( normal123 , 0.0 ) ).xy/( ase_grabScreenPosNorm + float4( normal123 , 0.0 ) ).w);
			o.Emission = ( emissive124 + screenColor60 ).rgb;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult54 = dot( (WorldNormalVector( i , normal123 )) , ase_worldlightDir );
			o.Alpha = ( 1.0 - saturate( (dotResult54*_ShadowTransform + _ShadowTransform) ) );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
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
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
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
				vertexDataFunc( v );
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
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
Version=17500
531;256;1228;702;717.9968;-150.5183;1.7199;True;False
Node;AmplifyShaderEditor.CommentaryNode;68;-374.4809,2497.611;Inherit;False;1956.211;707.3321;Comment;20;112;106;100;98;94;86;84;80;79;78;77;76;73;72;71;70;69;67;66;65;Time;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-307.5082,2659.645;Inherit;False;Property;_Speed;Speed;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;65;-324.4809,2547.611;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-135.4951,2581.561;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;69;6.237839,2783.539;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;71;58.01811,2583.824;Inherit;False;Sawtooth Wave;-1;;3;289adb816c3ac6d489f255fc3caf5016;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;70;222.1304,2800.769;Inherit;False;Sawtooth Wave;-1;;4;289adb816c3ac6d489f255fc3caf5016;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;72;269.8105,2587.805;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;73;433.9223,2804.75;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;748.6225,2722.037;Inherit;False;Property;_Strength;Strength;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;75;-2552.795,2444.885;Inherit;False;694.0061;215.4954;Comment;3;90;85;82;Tilling;1,1,1,1;0;0
Node;AmplifyShaderEditor.NegateNode;77;490.5381,2601.869;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;76;654.6518,2818.813;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;74;-3508.135,2685.025;Inherit;False;1674.772;500.5223;Comment;13;102;101;97;96;95;93;92;91;89;88;87;83;81;FlowUVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-2502.796,2494.885;Inherit;False;Constant;_Tilling;Tilling;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;885.9135,2621.517;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;932.7097,2816.249;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;81;-3468.748,2971.75;Inherit;True;Property;_FlowMap;FlowMap;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;86;1089.986,2612.338;Inherit;False;timeA;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-3289.769,2735.025;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;1172.583,2777.959;Inherit;False;timeB;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;83;-3153.486,2977.114;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;85;-2305.545,2501.381;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;89;-2889.888,2857.171;Inherit;False;Overlay;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-2610.886,3078.754;Inherit;False;84;timeB;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-2776.591,3003.853;Inherit;False;86;timeA;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-2082.789,2525.804;Inherit;False;tilling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;94;685.0444,3007.573;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;98;832.0993,3013.202;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;95;-2580.165,2742.296;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;92;-2409.123,2997.035;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-2474.662,2910.357;Inherit;False;90;tilling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-2269.861,2799.657;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-2184.486,2982.554;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;100;1012.099,3017.202;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-2050.11,2985.071;Inherit;False;flowB;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;106;1181.099,3022.202;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-2126.938,2776.707;Inherit;False;flowA;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;107;-1777.563,2487.631;Inherit;False;1353.714;675.8963;Comment;9;123;122;120;119;117;114;110;108;44;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-1652.166,2798.809;Inherit;False;102;flowA;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;108;-1727.563,2537.63;Inherit;True;Property;_Normal;Normal;9;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-1664.785,2916.041;Inherit;False;101;flowB;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;112;1369.099,3025.202;Inherit;False;timeBlend;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1664.447,3045.631;Float;False;Property;_NormalScale;Normal Scale;11;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;119;-1281.371,2619.306;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;120;-1281.661,2882.33;Inherit;True;Property;_TextureSample4;Texture Sample 4;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;117;-1003.338,2984.102;Inherit;False;112;timeBlend;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;122;-856.9235,2772.173;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;99;114.5459,1495.972;Inherit;False;1356.305;901.317;Comment;11;124;121;118;116;115;113;111;109;105;104;103;Emissive;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;103;166.2984,1576.588;Inherit;True;Property;_Emissive;Emissive;8;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;227.3257,1924.382;Inherit;False;101;flowB;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;239.9434,1807.15;Inherit;False;102;flowA;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;123;-626.4234,2708.769;Inherit;False;normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;113;579.5073,1846.121;Inherit;False;Property;_Emissivecolor;Emissive color;10;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;111;519.7314,2041.486;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;109;539.6187,1621.599;Inherit;True;Property;_def;def;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;131;-1077.633,924.5687;Inherit;False;123;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;904.3205,2207.5;Inherit;False;112;timeBlend;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;905.7316,1697.942;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;905.7316,1840.98;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;51;-710.7217,1175.419;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;49;-716.9214,1004.019;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;121;1057.168,1768.87;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GrabScreenPosition;53;-734.7308,574.8372;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;56;-610.6858,1356.254;Float;False;Property;_ShadowTransform;Shadow Transform;12;0;Create;True;0;0;False;0;0.5;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;54;-481.7218,1073.619;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-325.8103,708.1867;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;1252.622,1780.568;Inherit;False;emissive;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;57;-260.9226,1213.219;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;60;-172.4217,662.3193;Float;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;False;0;Object;-1;False;True;1;0;FLOAT4;0,0,0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;127;157.221,522.6685;Inherit;False;124;emissive;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;61;-50.62278,1133.419;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;48;-850.2346,104.6758;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;126;344.8344,558.0248;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleNode;59;-362.4576,273.9073;Inherit;False;0.8;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;62;-170.2576,325.407;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;55;-582.2349,222.6756;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;44.3937,407.2958;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;52;-773.9347,262.2758;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;63;163.8695,986.546;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;795.1804,363.5719;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;GravityOrb;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;True;0;False;Opaque;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;67;0;65;0
WireConnection;67;1;66;0
WireConnection;69;0;67;0
WireConnection;71;1;67;0
WireConnection;70;1;69;0
WireConnection;72;0;71;0
WireConnection;73;0;70;0
WireConnection;77;0;72;0
WireConnection;76;0;73;0
WireConnection;79;0;77;0
WireConnection;79;1;78;0
WireConnection;80;0;76;0
WireConnection;80;1;78;0
WireConnection;86;0;79;0
WireConnection;84;0;80;0
WireConnection;83;0;81;0
WireConnection;85;0;82;0
WireConnection;89;0;87;0
WireConnection;89;1;83;0
WireConnection;90;0;85;0
WireConnection;94;0;72;0
WireConnection;98;0;94;0
WireConnection;95;0;87;0
WireConnection;95;1;89;0
WireConnection;95;2;91;0
WireConnection;92;0;87;0
WireConnection;92;1;89;0
WireConnection;92;2;88;0
WireConnection;96;0;95;0
WireConnection;96;1;93;0
WireConnection;97;0;92;0
WireConnection;97;1;93;0
WireConnection;100;0;98;0
WireConnection;101;0;97;0
WireConnection;106;0;100;0
WireConnection;102;0;96;0
WireConnection;112;0;106;0
WireConnection;119;0;108;0
WireConnection;119;1;110;0
WireConnection;119;5;44;0
WireConnection;120;0;108;0
WireConnection;120;1;114;0
WireConnection;120;5;44;0
WireConnection;122;0;119;0
WireConnection;122;1;120;0
WireConnection;122;2;117;0
WireConnection;123;0;122;0
WireConnection;111;0;103;0
WireConnection;111;1;104;0
WireConnection;109;0;103;0
WireConnection;109;1;105;0
WireConnection;116;0;109;0
WireConnection;116;1;113;0
WireConnection;118;0;111;0
WireConnection;118;1;113;0
WireConnection;49;0;131;0
WireConnection;121;0;116;0
WireConnection;121;1;118;0
WireConnection;121;2;115;0
WireConnection;54;0;49;0
WireConnection;54;1;51;0
WireConnection;58;0;53;0
WireConnection;58;1;131;0
WireConnection;124;0;121;0
WireConnection;57;0;54;0
WireConnection;57;1;56;0
WireConnection;57;2;56;0
WireConnection;60;0;58;0
WireConnection;61;0;57;0
WireConnection;48;0;131;0
WireConnection;126;0;127;0
WireConnection;126;1;60;0
WireConnection;59;0;55;0
WireConnection;62;0;59;0
WireConnection;55;0;48;0
WireConnection;55;1;52;0
WireConnection;64;0;62;0
WireConnection;64;1;60;0
WireConnection;63;0;61;0
WireConnection;0;2;126;0
WireConnection;0;9;63;0
ASEEND*/
//CHKSM=2A943156B92096ED01318B035AB720C5FEFBB1CF