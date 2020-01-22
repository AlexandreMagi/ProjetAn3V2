// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RevealLight"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Reveallighttexture("Reveal light texture", 2D) = "white" {}
		_ColortoBeFiltered("Color to Be Filtered", Color) = (0.6981132,0.25356,0.25356,1)
		[HDR]_Reveallightcolor("Reveal light color", Color) = (1,0.07075471,0.07075471,0)
		_DifferenceThreshold("Difference Threshold", Range( 0 , 0.05)) = 0.05
		_RevealLightEnabled("RevealLightEnabled", Range( 0 , 1)) = 1
		_Contrast("Contrast", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		struct Input
		{
			float2 uv_texcoord;
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

		uniform float _Contrast;
		uniform float4 _Reveallightcolor;
		uniform sampler2D _Reveallighttexture;
		uniform float4 _Reveallighttexture_ST;
		uniform float4 _ColortoBeFiltered;
		uniform float _DifferenceThreshold;
		uniform float _RevealLightEnabled;
		uniform float _Cutoff = 0.5;


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
			float2 uv_Reveallighttexture = i.uv_texcoord * _Reveallighttexture_ST.xy + _Reveallighttexture_ST.zw;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 normalizeResult160 = normalize( (( ase_lightColor * _WorldSpaceLightPos0.w )).rgb );
			float3 normalizeResult161 = normalize( (_ColortoBeFiltered).rgb );
			float dotResult165 = dot( normalizeResult160 , normalizeResult161 );
			float4 handpaint373 = ( CalculateContrast(_Contrast,( float4( 0,0,0,0 ) + ( ( _Reveallightcolor * tex2D( _Reveallighttexture, uv_Reveallighttexture ).a ) * ( ( ( ase_lightAtten * ase_lightColor ) * _WorldSpaceLightPos0.w ) *  ( dotResult165 - _DifferenceThreshold > 1.0 ? 0.0 : dotResult165 - _DifferenceThreshold <= 1.0 && dotResult165 + _DifferenceThreshold >= 1.0 ? 1.0 : 0.0 )  ) ) )) * _RevealLightEnabled );
			float4 temp_output_115_0 = handpaint373;
			c.rgb = temp_output_115_0.rgb;
			c.a = 1;
			clip( temp_output_115_0.r - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float2 uv_Reveallighttexture = i.uv_texcoord * _Reveallighttexture_ST.xy + _Reveallighttexture_ST.zw;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 normalizeResult160 = normalize( (( ase_lightColor * _WorldSpaceLightPos0.w )).rgb );
			float3 normalizeResult161 = normalize( (_ColortoBeFiltered).rgb );
			float dotResult165 = dot( normalizeResult160 , normalizeResult161 );
			float4 handpaint373 = ( CalculateContrast(_Contrast,( float4( 0,0,0,0 ) + ( ( _Reveallightcolor * tex2D( _Reveallighttexture, uv_Reveallighttexture ).a ) * ( ( ( 1 * ase_lightColor ) * _WorldSpaceLightPos0.w ) *  ( dotResult165 - _DifferenceThreshold > 1.0 ? 0.0 : dotResult165 - _DifferenceThreshold <= 1.0 && dotResult165 + _DifferenceThreshold >= 1.0 ? 1.0 : 0.0 )  ) ) )) * _RevealLightEnabled );
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
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
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
0;482;1318;517;1675.32;-566.4517;1.6;True;False
Node;AmplifyShaderEditor.CommentaryNode;149;-3688.979,468.5045;Inherit;False;2733.683;1073.523;Comment;27;373;427;434;431;342;435;198;419;197;200;271;412;164;162;165;163;160;161;159;158;156;157;154;155;152;153;442;RevealLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.LightColorNode;153;-3641.517,999.7997;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.WorldSpaceLightPos;152;-3515.967,802.439;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;155;-3490.457,1273.71;Float;False;Property;_ColortoBeFiltered;Color to Be Filtered;2;0;Create;True;0;0;False;0;0.6981132,0.25356,0.25356,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;154;-3266.062,1021.557;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;157;-3101.696,1036.923;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;156;-3206.387,1204.43;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;160;-2863,1055.568;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;161;-2964.604,1210.015;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;159;-3282.021,657.1661;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightAttenuation;158;-3280.717,566.5498;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;163;-2837.449,1335.847;Float;False;Property;_DifferenceThreshold;Difference Threshold;4;0;Create;True;0;0;False;0;0.05;0.05;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;165;-2691.023,1142.59;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-2719.75,1242.825;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;-3023.09,651.9681;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;197;-2625.428,740.2084;Inherit;True;Property;_Reveallighttexture;Reveal light texture;1;0;Create;True;0;0;False;0;-1;2aed7015bc03a174fb153ccfe739a36f;2aed7015bc03a174fb153ccfe739a36f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;200;-2594.425,543.7891;Inherit;False;Property;_Reveallightcolor;Reveal light color;3;1;[HDR];Create;True;0;0;False;0;1,0.07075471,0.07075471,0;1,0.07075471,0.07075471,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;412;-2800.835,901.3683;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCIf;271;-2540.029,1141.865;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;419;-2272.812,979.264;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;198;-2199.52,848.0807;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;342;-1993.359,937.8237;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;442;-1821.534,893.842;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;435;-1789.891,1049.107;Inherit;False;Property;_Contrast;Contrast;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;434;-1661.482,914.5543;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;431;-1628.345,1067.6;Inherit;False;Property;_RevealLightEnabled;RevealLightEnabled;5;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;427;-1370.974,914.5682;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;105;-911.2739,755.3216;Inherit;False;1048.839;533.1951;Comment;2;0;115;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;373;-1180.004,887.0143;Inherit;False;handpaint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;-559.8775,1035.867;Inherit;False;373;handpaint;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-93.56361,807.5729;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;RevealLight;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;154;0;153;0
WireConnection;154;1;152;2
WireConnection;157;0;154;0
WireConnection;156;0;155;0
WireConnection;160;0;157;0
WireConnection;161;0;156;0
WireConnection;165;0;160;0
WireConnection;165;1;161;0
WireConnection;164;0;158;0
WireConnection;164;1;159;0
WireConnection;412;0;164;0
WireConnection;412;1;152;2
WireConnection;271;0;165;0
WireConnection;271;1;162;0
WireConnection;271;3;162;0
WireConnection;271;5;163;0
WireConnection;419;0;412;0
WireConnection;419;1;271;0
WireConnection;198;0;200;0
WireConnection;198;1;197;4
WireConnection;342;0;198;0
WireConnection;342;1;419;0
WireConnection;442;1;342;0
WireConnection;434;1;442;0
WireConnection;434;0;435;0
WireConnection;427;0;434;0
WireConnection;427;1;431;0
WireConnection;373;0;427;0
WireConnection;0;2;115;0
WireConnection;0;10;115;0
WireConnection;0;13;115;0
ASEEND*/
//CHKSM=515F415D1B1CCFE1DAA347311C8691DAC70D37E9