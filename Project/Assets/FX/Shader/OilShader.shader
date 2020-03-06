// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "OilShader"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_Maintexture("Main texture", 2D) = "white" {}
		_ColortoBeFiltered1("Color to Be Filtered", Color) = (0.6981132,0.25356,0.25356,1)
		[HDR]_Reveallightcolor1("Reveal light color", Color) = (1,0.07075471,0.07075471,0)
		_Power("Power", Float) = 0
		_DifferenceThreshold("Difference Threshold", Range( 0 , 0.05)) = 0.05
		_Normal("Normal", 2D) = "bump" {}
		_Contrast("Contrast", Float) = 0
		_RevealLightEnabled("RevealLightEnabled", Range( 0 , 1)) = 1
		[Toggle(_ISSELFEMITTING1_ON)] _IsSelfEmitting1("IsSelfEmitting", Float) = 1
		_SelfEmittingValue("SelfEmittingValue", Range( 0 , 0.05)) = 0.05
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature_local _ISSELFEMITTING1_ON
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha noshadow exclude_path:deferred noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float4 vertexColor : COLOR;
			float4 uv2_tex4coord2;
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

		uniform float4 _Reveallightcolor1;
		uniform sampler2D _Maintexture;
		uniform float4 _Maintexture_ST;
		uniform float4 _ColortoBeFiltered1;
		uniform float _DifferenceThreshold;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _SelfEmittingValue;
		uniform float _Contrast;
		uniform float _RevealLightEnabled;
		uniform float4 _Color;
		uniform float _Power;

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
			float2 uv0_Maintexture = i.uv_texcoord * _Maintexture_ST.xy + _Maintexture_ST.zw;
			float4 mainTexture47 = tex2D( _Maintexture, uv0_Maintexture );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 normalizeResult85 = normalize( (( ase_lightColor * _WorldSpaceLightPos0.w )).rgb );
			float3 normalizeResult86 = normalize( (_ColortoBeFiltered1).rgb );
			float dotResult88 = dot( normalizeResult85 , normalizeResult86 );
			float temp_output_92_0 =  ( dotResult88 - _DifferenceThreshold > 1.0 ? 0.0 : dotResult88 - _DifferenceThreshold <= 1.0 && dotResult88 + _DifferenceThreshold >= 1.0 ? 1.0 : 0.0 ) ;
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 normal53 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult116 = dot( normalize( (WorldNormalVector( i , normal53 )) ) , ase_worldlightDir );
			float lightDir117 = dotResult116;
			float4 temp_output_98_0 = ( ( ( ase_lightAtten * ase_lightColor ) * _WorldSpaceLightPos0.w ) * ( temp_output_92_0 * lightDir117 ) );
			float ifLocalVar99 = 0;
			if( temp_output_92_0 == 0.0 )
				ifLocalVar99 = _SelfEmittingValue;
			#ifdef _ISSELFEMITTING1_ON
				float4 staticSwitch102 = ( temp_output_98_0 + ifLocalVar99 );
			#else
				float4 staticSwitch102 = temp_output_98_0;
			#endif
			float4 handpaint111 = ( ( ( ( _Reveallightcolor1 * mainTexture47 ) * saturate( staticSwitch102 ) ) * _Contrast ) * _RevealLightEnabled );
			float4 albedo54 = ( i.vertexColor * ( _Color * mainTexture47 ) );
			clip( ( 1.0 - i.uv2_tex4coord2.x ) - pow( ( 1.0 - mainTexture47.a ) , _Power ));
			float4 alpha56 = albedo54;
			c.rgb = ( float4( (WorldNormalVector( i , normal53 )) , 0.0 ) + ( albedo54 + handpaint111 ) ).rgb;
			c.a = ( handpaint111 * alpha56 ).r;
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
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
740;236;1437;594;2748.256;2733.083;1.68109;True;False
Node;AmplifyShaderEditor.CommentaryNode;52;-2042.894,-583.5406;Inherit;False;620;291;Comment;2;16;53;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;16;-1992.894,-533.5406;Inherit;True;Property;_Normal;Normal;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;78;-2509.989,-1927.948;Inherit;False;3324.852;1069.659;Comment;33;111;110;109;108;107;106;105;104;103;102;101;100;99;98;97;96;95;94;93;92;91;90;89;88;87;86;85;84;83;82;81;80;79;RevealLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.LightColorNode;79;-2462.525,-1396.653;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-1666.341,-489.6215;Inherit;False;normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;80;-2432.428,-1598.047;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;113;-1719.689,-2420.385;Inherit;False;936.6855;440.1697;Comment;5;117;116;115;114;112;LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-2232.266,-1395.062;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-1689.259,-2321.223;Inherit;False;53;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;81;-2311.464,-1122.743;Float;False;Property;_ColortoBeFiltered1;Color to Be Filtered;2;0;Create;True;0;0;False;0;0.6981132,0.25356,0.25356,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;115;-1495.885,-2363.385;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;84;-2027.394,-1192.023;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;83;-1922.704,-1359.53;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;112;-1595.79,-2183.957;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;86;-1785.611,-1186.438;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;85;-1684.007,-1340.885;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;116;-1196.089,-2263.657;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;88;-1512.03,-1253.863;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;89;-2229.724,-1851.903;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-1540.757,-1153.628;Float;False;Constant;_Float1;Float 0;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;43;-1388.508,-598.9751;Inherit;False;1250.163;397;Comment;4;47;46;45;44;Main texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-986.1423,-2258.971;Inherit;False;lightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;90;-2231.028,-1761.287;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;87;-1658.456,-1060.606;Float;False;Property;_DifferenceThreshold;Difference Threshold;5;0;Create;True;0;0;False;0;0.05;0.05;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCIf;92;-1361.036,-1254.588;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-1394.26,-1380.378;Inherit;False;117;lightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;44;-1338.507,-548.9751;Inherit;True;Property;_Maintexture;Main texture;1;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-1972.097,-1766.485;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-1296.117,-1022.665;Inherit;False;Property;_SelfEmittingValue;SelfEmittingValue;10;0;Create;True;0;0;False;0;0.05;0.51;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-1621.842,-1495.085;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-1069.505,-360.9753;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1173.428,-1394.386;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;99;-982.8604,-1197.965;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-1007.184,-1514.712;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;46;-710.4697,-511.0856;Inherit;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;100;-809.5193,-1339.208;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;-388.3411,-462.3425;Inherit;False;mainTexture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;101;-1427.571,-1647.863;Inherit;False;47;mainTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;102;-666.0674,-1400.824;Inherit;False;Property;_IsSelfEmitting1;IsSelfEmitting;9;0;Create;True;0;0;False;0;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;103;-1415.432,-1852.664;Inherit;False;Property;_Reveallightcolor1;Reveal light color;3;1;[HDR];Create;True;0;0;False;0;1,0.07075471,0.07075471,0;1,0.07075471,0.07075471,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;59;-1915.892,-166.3318;Inherit;False;1786.528;809.8964;Comment;14;49;1;51;6;8;3;5;12;13;4;7;14;54;56;Vertex stream;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-1134.714,-1678.023;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;105;-342.4594,-1423.931;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;-1865.892,213.0156;Inherit;False;47;mainTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1;-1725.543,-19.94276;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;107;-77.61938,-1294.877;Inherit;False;Property;_Contrast;Contrast;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;51;-1677.102,298.0916;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.VertexColorNode;5;-1462.439,-116.3318;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-1386.861,109.3584;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-132.9404,-1543.993;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;12;-998.9691,436.5648;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-1403.932,415.5644;Inherit;False;Property;_Power;Power;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1242.382,57.87801;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;108;145.3066,-1250.126;Inherit;False;Property;_RevealLightEnabled;RevealLightEnabled;8;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;82.81763,-1444.634;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;6;-1398.534,303.2713;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;393.6456,-1405.855;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;13;-774.9684,438.5649;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;7;-1233.334,330.2649;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-982.0477,200.048;Inherit;False;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;58;-97.94054,-613.8153;Inherit;False;1474.184;624;Comment;9;74;0;66;68;55;63;57;75;77;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;111;537.5347,-1475.227;Inherit;False;handpaint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClipNode;14;-591.9388,332.2377;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-35.63366,-361.6935;Inherit;False;111;handpaint;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-33.16711,-549.3242;Inherit;False;54;albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;309.3869,-549.4784;Inherit;False;53;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-353.3649,359.5806;Inherit;False;alpha;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;188.4915,-482.4047;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-31.68322,-268.0138;Inherit;False;56;alpha;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;74;496.2654,-480.3812;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;190.879,-271.3094;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;77;695.3869,-325.4784;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;902.2339,-436.0492;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;OilShader;False;False;False;False;True;True;True;True;True;True;False;False;False;False;True;True;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;53;0;16;0
WireConnection;82;0;79;0
WireConnection;82;1;80;2
WireConnection;115;0;114;0
WireConnection;84;0;81;0
WireConnection;83;0;82;0
WireConnection;86;0;84;0
WireConnection;85;0;83;0
WireConnection;116;0;115;0
WireConnection;116;1;112;0
WireConnection;88;0;85;0
WireConnection;88;1;86;0
WireConnection;117;0;116;0
WireConnection;92;0;88;0
WireConnection;92;1;91;0
WireConnection;92;3;91;0
WireConnection;92;5;87;0
WireConnection;94;0;89;0
WireConnection;94;1;90;0
WireConnection;96;0;94;0
WireConnection;96;1;80;2
WireConnection;45;2;44;0
WireConnection;95;0;92;0
WireConnection;95;1;93;0
WireConnection;99;0;92;0
WireConnection;99;3;97;0
WireConnection;98;0;96;0
WireConnection;98;1;95;0
WireConnection;46;0;44;0
WireConnection;46;1;45;0
WireConnection;100;0;98;0
WireConnection;100;1;99;0
WireConnection;47;0;46;0
WireConnection;102;1;98;0
WireConnection;102;0;100;0
WireConnection;104;0;103;0
WireConnection;104;1;101;0
WireConnection;105;0;102;0
WireConnection;51;0;49;0
WireConnection;3;0;1;0
WireConnection;3;1;49;0
WireConnection;106;0;104;0
WireConnection;106;1;105;0
WireConnection;4;0;5;0
WireConnection;4;1;3;0
WireConnection;109;0;106;0
WireConnection;109;1;107;0
WireConnection;6;0;51;3
WireConnection;110;0;109;0
WireConnection;110;1;108;0
WireConnection;13;0;12;1
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;54;0;4;0
WireConnection;111;0;110;0
WireConnection;14;0;54;0
WireConnection;14;1;13;0
WireConnection;14;2;7;0
WireConnection;56;0;14;0
WireConnection;66;0;55;0
WireConnection;66;1;63;0
WireConnection;74;0;75;0
WireConnection;68;0;63;0
WireConnection;68;1;57;0
WireConnection;77;0;74;0
WireConnection;77;1;66;0
WireConnection;0;9;68;0
WireConnection;0;13;77;0
ASEEND*/
//CHKSM=263487D63FD2EA573BA74C74A3E82DA4AAF1EB47