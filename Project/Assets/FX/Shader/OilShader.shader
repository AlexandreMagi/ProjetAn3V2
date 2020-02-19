// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "OilShader"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_Maintexture("Main texture", 2D) = "white" {}
		_ColortoBeFiltered("Color to Be Filtered", Color) = (0.6981132,0.25356,0.25356,1)
		_Power("Power", Float) = 0
		_Normal("Normal", 2D) = "bump" {}
		_DifferenceThreshold("Difference Threshold", Range( 0 , 0.05)) = 0.05
		_Contrast("Contrast", Float) = 0
		_RevealLightEnabled("RevealLightEnabled", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha noshadow exclude_path:deferred 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float4 uv2_tex4coord2;
			float3 worldNormal;
			INTERNAL_DATA
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

		uniform sampler2D _Maintexture;
		uniform float4 _Maintexture_ST;
		uniform float _Contrast;
		uniform float4 _ColortoBeFiltered;
		uniform float _DifferenceThreshold;
		uniform float _RevealLightEnabled;
		uniform float4 _Color;
		uniform float _Power;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;

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
			float3 normalizeResult26 = normalize( (( ase_lightColor * _WorldSpaceLightPos0.w )).rgb );
			float3 normalizeResult24 = normalize( (_ColortoBeFiltered).rgb );
			float dotResult27 = dot( normalizeResult26 , normalizeResult24 );
			float4 handpaint42 = ( ( ( mainTexture47.a * _Contrast ) * ( ( ( ase_lightAtten * ase_lightColor ) * _WorldSpaceLightPos0.w ) *  ( dotResult27 - _DifferenceThreshold > 1.0 ? 0.0 : dotResult27 - _DifferenceThreshold <= 1.0 && dotResult27 + _DifferenceThreshold >= 1.0 ? 1.0 : 0.0 )  ) ) * _RevealLightEnabled );
			float4 albedo54 = ( i.vertexColor * ( _Color * mainTexture47 ) );
			clip( ( 1.0 - i.uv2_tex4coord2.x ) - pow( ( 1.0 - mainTexture47.a ) , _Power ));
			float4 alpha56 = albedo54;
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 normal53 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			c.rgb = ( float4( (WorldNormalVector( i , normal53 )) , 0.0 ) + ( albedo54 + handpaint42 ) ).rgb;
			c.a = ( handpaint42 * alpha56 ).r;
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
1920;0;1920;1019;332.6131;847.4784;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;43;-1388.508,-598.9751;Inherit;False;1250.163;397;Comment;4;47;46;45;44;Main texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;17;-2037.948,-1729.962;Inherit;False;2314.31;1039.325;Comment;25;42;41;39;40;38;37;36;35;33;32;29;50;27;28;67;25;24;31;26;22;23;21;20;19;18;RevealLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;44;-1338.507,-548.9751;Inherit;True;Property;_Maintexture;Main texture;1;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;18;-1864.934,-1396.028;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;19;-1990.485,-1198.667;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-1069.505,-360.9753;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-1615.03,-1176.91;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;21;-1839.424,-924.7576;Float;False;Property;_ColortoBeFiltered;Color to Be Filtered;2;0;Create;True;0;0;False;0;0.6981132,0.25356,0.25356,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;46;-710.4697,-511.0856;Inherit;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;23;-1555.354,-994.0374;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;22;-1450.665,-1161.544;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;67;-1624.094,-1683.982;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;26;-1211.967,-1142.899;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;24;-1313.571,-988.4525;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;-388.3411,-462.3425;Inherit;False;mainTexture;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;25;-1630.989,-1541.301;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.DotProductOpNode;27;-1039.991,-1055.877;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;59;-1915.892,-166.3318;Inherit;False;1786.528;809.8964;Comment;14;49;1;51;6;8;3;5;12;13;4;7;14;54;56;Vertex stream;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-992.5359,-1529.877;Inherit;False;47;mainTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1372.057,-1546.499;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1186.416,-862.6205;Float;False;Property;_DifferenceThreshold;Difference Threshold;5;0;Create;True;0;0;False;0;0.05;0.05;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1068.717,-955.6426;Float;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-675.8593,-1313.36;Inherit;False;Property;_Contrast;Contrast;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCIf;35;-888.9971,-1056.602;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;50;-786.8634,-1496.961;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1149.802,-1297.099;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1;-1725.543,-19.94276;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;49;-1865.892,213.0156;Inherit;False;47;mainTexture;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-507.8399,-1364.232;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;5;-1462.439,-116.3318;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-621.7792,-1219.203;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-1386.861,109.3584;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;51;-1677.102,298.0916;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.CommentaryNode;52;-2042.894,-583.5406;Inherit;False;620;291;Comment;2;16;53;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-342.326,-1260.643;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;12;-998.9691,436.5648;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;39;-439.4882,-1085.067;Inherit;False;Property;_RevealLightEnabled;RevealLightEnabled;7;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1403.932,415.5644;Inherit;False;Property;_Power;Power;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1242.382,57.87801;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;6;-1398.534,303.2713;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-111.8478,-1229.096;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;16;-1992.894,-533.5406;Inherit;True;Property;_Normal;Normal;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;13;-774.9684,438.5649;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;7;-1233.334,330.2649;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;54;-982.0477,200.048;Inherit;False;albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-1666.341,-489.6215;Inherit;False;normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;58;-97.94054,-613.8153;Inherit;False;1474.184;624;Comment;9;74;0;66;68;55;63;57;75;77;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;57.5493,-1279.338;Inherit;False;handpaint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClipNode;14;-591.9388,332.2377;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;63;-35.63366,-361.6935;Inherit;False;42;handpaint;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-33.16711,-549.3242;Inherit;False;54;albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;309.3869,-549.4784;Inherit;False;53;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-353.3649,359.5806;Inherit;False;alpha;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;74;496.2654,-480.3812;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;57;-31.68322,-268.0138;Inherit;False;56;alpha;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;188.4915,-482.4047;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;190.879,-271.3094;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;77;695.3869,-325.4784;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;902.2339,-436.0492;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;OilShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;45;2;44;0
WireConnection;20;0;19;0
WireConnection;20;1;18;2
WireConnection;46;0;44;0
WireConnection;46;1;45;0
WireConnection;23;0;21;0
WireConnection;22;0;20;0
WireConnection;26;0;22;0
WireConnection;24;0;23;0
WireConnection;47;0;46;0
WireConnection;27;0;26;0
WireConnection;27;1;24;0
WireConnection;28;0;67;0
WireConnection;28;1;25;0
WireConnection;35;0;27;0
WireConnection;35;1;32;0
WireConnection;35;3;32;0
WireConnection;35;5;29;0
WireConnection;50;0;31;0
WireConnection;36;0;28;0
WireConnection;36;1;18;2
WireConnection;37;0;50;3
WireConnection;37;1;33;0
WireConnection;38;0;36;0
WireConnection;38;1;35;0
WireConnection;3;0;1;0
WireConnection;3;1;49;0
WireConnection;51;0;49;0
WireConnection;40;0;37;0
WireConnection;40;1;38;0
WireConnection;4;0;5;0
WireConnection;4;1;3;0
WireConnection;6;0;51;3
WireConnection;41;0;40;0
WireConnection;41;1;39;0
WireConnection;13;0;12;1
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;54;0;4;0
WireConnection;53;0;16;0
WireConnection;42;0;41;0
WireConnection;14;0;54;0
WireConnection;14;1;13;0
WireConnection;14;2;7;0
WireConnection;56;0;14;0
WireConnection;74;0;75;0
WireConnection;66;0;55;0
WireConnection;66;1;63;0
WireConnection;68;0;63;0
WireConnection;68;1;57;0
WireConnection;77;0;74;0
WireConnection;77;1;66;0
WireConnection;0;9;68;0
WireConnection;0;13;77;0
ASEEND*/
//CHKSM=9A53E7BA43399F5FC14569B34197F59140793B2D