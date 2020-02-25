// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "RevealLight"
{
	Properties
	{
		_RevealLightTexture("Reveal Light Texture", 2D) = "white" {}
		_ColortoBeFiltered("Color to Be Filtered", Color) = (0.6981132,0.25356,0.25356,1)
		[HDR]_Reveallightcolor("Reveal light color", Color) = (1,0.07075471,0.07075471,0)
		_DifferenceThreshold("Difference Threshold", Range( 0 , 0.05)) = 0.05
		_Contrast("Contrast", Float) = 0
		_Softparticlesdistance("Soft particles distance", Float) = 1
		_Softparticlespower("Soft particles power", Range( 0 , 1)) = 1
		_RevealLightEnabled("RevealLightEnabled", Range( 0 , 1)) = 1
		[Toggle(_ISNEARONLY_ON)] _IsNearOnly("IsNearOnly", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature_local _ISNEARONLY_ON
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 screenPos;
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

		uniform float4 _Reveallightcolor;
		uniform sampler2D _RevealLightTexture;
		uniform float4 _RevealLightTexture_ST;
		uniform float4 _ColortoBeFiltered;
		uniform float _DifferenceThreshold;
		uniform float _Contrast;
		uniform float _RevealLightEnabled;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Softparticlesdistance;
		uniform float _Softparticlespower;

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
			float2 uv0_RevealLightTexture = i.uv_texcoord * _RevealLightTexture_ST.xy + _RevealLightTexture_ST.zw;
			float mainTexture449 = tex2D( _RevealLightTexture, uv0_RevealLightTexture ).a;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 normalizeResult451 = normalize( (( ase_lightColor * _WorldSpaceLightPos0.w )).rgb );
			float3 normalizeResult450 = normalize( (_ColortoBeFiltered).rgb );
			float dotResult456 = dot( normalizeResult451 , normalizeResult450 );
			float4 handpaint477 = ( ( ( ( _Reveallightcolor * mainTexture449 ) * ( ( ( ase_lightAtten * ase_lightColor ) * _WorldSpaceLightPos0.w ) *  ( dotResult456 - _DifferenceThreshold > 1.0 ? 0.0 : dotResult456 - _DifferenceThreshold <= 1.0 && dotResult456 + _DifferenceThreshold >= 1.0 ? 1.0 : 0.0 )  ) ) * _Contrast ) * _RevealLightEnabled );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth461 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth461 = abs( ( screenDepth461 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Softparticlesdistance ) );
			float clampResult473 = clamp( ( ( 1.0 - distanceDepth461 ) * _Softparticlespower ) , 0.0 , 1.0 );
			#ifdef _ISNEARONLY_ON
				float staticSwitch478 = clampResult473;
			#else
				float staticSwitch478 = 1.0;
			#endif
			float softParticles480 = staticSwitch478;
			float4 alpha485 = ( ( i.vertexColor.a * handpaint477 ) * softParticles480 );
			c.rgb = handpaint477.rgb;
			c.a = alpha485.r;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
548;219;1271;883;2842.131;-1198.908;1.172097;True;False
Node;AmplifyShaderEditor.CommentaryNode;438;-4100.452,1212.745;Inherit;False;2733.683;1073.523;Comment;25;477;474;472;471;469;465;464;463;462;459;458;457;456;455;454;453;452;451;450;446;445;443;442;440;439;RevealLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.LightColorNode;440;-4052.989,1744.04;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.WorldSpaceLightPos;439;-3927.438,1546.679;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;437;-4130.849,626.5197;Inherit;False;1250.163;397;Comment;4;449;447;444;441;Main texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;442;-3677.533,1765.797;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;443;-3901.928,2017.95;Float;False;Property;_ColortoBeFiltered;Color to Be Filtered;1;0;Create;True;0;0;False;0;0.6981132,0.25356,0.25356,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;441;-4080.849,676.5197;Inherit;True;Property;_RevealLightTexture;Reveal Light Texture;0;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;444;-3811.847,864.5197;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;445;-3617.858,1948.67;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;446;-3513.168,1781.163;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;450;-3376.075,1954.255;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;447;-3452.812,714.4095;Inherit;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;452;-3693.492,1401.406;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.NormalizeNode;451;-3274.471,1799.808;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;453;-3692.188,1310.79;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;449;-3104.684,728.1525;Inherit;False;mainTexture;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;457;-3248.92,2080.087;Float;False;Property;_DifferenceThreshold;Difference Threshold;3;0;Create;True;0;0;False;0;0.05;0.05;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;458;-3434.561,1396.208;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;456;-3102.494,1886.83;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;459;-3131.221,1987.065;Float;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;448;-2781.177,674.5223;Inherit;False;1684.377;326.2766;Comment;9;480;478;475;473;470;468;467;461;460;Edge detection;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCIf;462;-2951.5,1886.105;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;454;-3005.896,1288.029;Inherit;False;Property;_Reveallightcolor;Reveal light color;2;1;[HDR];Create;True;0;0;False;0;1,0.07075471,0.07075471,0;1,0.07075471,0.07075471,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;465;-3212.306,1645.608;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;455;-3018.035,1492.83;Inherit;False;449;mainTexture;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;460;-2740.049,749.5244;Inherit;False;Property;_Softparticlesdistance;Soft particles distance;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;461;-2473.103,743.955;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;469;-2684.282,1723.504;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;463;-2725.178,1462.67;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;472;-2418.288,1675.58;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;464;-2181.975,1868.999;Inherit;False;Property;_Contrast;Contrast;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;468;-2277.501,905.7028;Inherit;False;Property;_Softparticlespower;Soft particles power;6;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;467;-2189.792,784.2325;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;492;-2021.537,1719.242;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;470;-1959.349,793.1758;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;471;-1959.049,1913.75;Inherit;False;Property;_RevealLightEnabled;RevealLightEnabled;7;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;475;-1776.757,719.3783;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;474;-1710.71,1758.021;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;473;-1794.949,802.6487;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;478;-1586.857,772.0249;Inherit;False;Property;_IsNearOnly;IsNearOnly;8;0;Create;True;0;0;False;0;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;476;-4078.925,87.76959;Inherit;False;1083.131;395.5159;Comment;6;485;484;483;482;481;479;Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;477;-1566.82,1688.649;Inherit;False;handpaint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;481;-4027.41,137.7696;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;480;-1330.825,825.9489;Inherit;False;softParticles;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;479;-4028.925,360.2855;Inherit;False;477;handpaint;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;482;-3762.51,231.6202;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;483;-3811.496,405.5933;Inherit;False;480;softParticles;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;484;-3449.496,245.5933;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;485;-3277.792,259.1287;Inherit;False;alpha;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;105;-2726.277,-68.20197;Inherit;False;1048.839;533.1951;Comment;3;0;115;486;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;-2461.775,242.7249;Inherit;False;477;handpaint;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;486;-2337.116,141.3877;Inherit;False;485;alpha;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1908.566,-15.95074;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;RevealLight;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;442;0;440;0
WireConnection;442;1;439;2
WireConnection;444;2;441;0
WireConnection;445;0;443;0
WireConnection;446;0;442;0
WireConnection;450;0;445;0
WireConnection;447;0;441;0
WireConnection;447;1;444;0
WireConnection;451;0;446;0
WireConnection;449;0;447;4
WireConnection;458;0;453;0
WireConnection;458;1;452;0
WireConnection;456;0;451;0
WireConnection;456;1;450;0
WireConnection;462;0;456;0
WireConnection;462;1;459;0
WireConnection;462;3;459;0
WireConnection;462;5;457;0
WireConnection;465;0;458;0
WireConnection;465;1;439;2
WireConnection;461;0;460;0
WireConnection;469;0;465;0
WireConnection;469;1;462;0
WireConnection;463;0;454;0
WireConnection;463;1;455;0
WireConnection;472;0;463;0
WireConnection;472;1;469;0
WireConnection;467;0;461;0
WireConnection;492;0;472;0
WireConnection;492;1;464;0
WireConnection;470;0;467;0
WireConnection;470;1;468;0
WireConnection;474;0;492;0
WireConnection;474;1;471;0
WireConnection;473;0;470;0
WireConnection;478;1;475;0
WireConnection;478;0;473;0
WireConnection;477;0;474;0
WireConnection;480;0;478;0
WireConnection;482;0;481;4
WireConnection;482;1;479;0
WireConnection;484;0;482;0
WireConnection;484;1;483;0
WireConnection;485;0;484;0
WireConnection;0;9;486;0
WireConnection;0;13;115;0
ASEEND*/
//CHKSM=48B631242B819870BEF6C4437CBFCC9C4ECBFA85