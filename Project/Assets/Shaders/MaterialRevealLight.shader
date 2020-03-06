// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_Phosphorescence Custom Shader/MaterialRevealLight"
{
	Properties
	{
		_Normal("Normal", 2D) = "bump" {}
		_Spec("Spec", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf StandardCustomLighting keepalpha 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
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

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Spec;
		uniform float4 _Spec_ST;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			SurfaceOutputStandardSpecular s553 = (SurfaceOutputStandardSpecular ) 0;
			s553.Albedo = float3( 0,0,0 );
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 normal519 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			s553.Normal = WorldNormalVector( i , normal519 );
			s553.Emission = float3( 0,0,0 );
			float2 uv_Spec = i.uv_texcoord * _Spec_ST.xy + _Spec_ST.zw;
			float4 specular561 = tex2D( _Spec, uv_Spec );
			s553.Specular = specular561.rgb;
			s553.Smoothness = 0.0;
			s553.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi553 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g553 = UnityGlossyEnvironmentSetup( s553.Smoothness, data.worldViewDir, s553.Normal, float3(0,0,0));
			gi553 = UnityGlobalIllumination( data, s553.Occlusion, s553.Normal, g553 );
			#endif

			float3 surfResult553 = LightingStandardSpecular ( s553, viewDir, gi553 ).rgb;
			surfResult553 += s553.Emission;

			#ifdef UNITY_PASS_FORWARDADD//553
			surfResult553 -= s553.Emission;
			#endif//553
			c.rgb = surfResult553;
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
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
111;433;1437;594;8044.212;966.2676;6.700753;True;False
Node;AmplifyShaderEditor.CommentaryNode;564;-4114.377,219.9849;Inherit;False;723.0901;280;Comment;2;556;561;Specular;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;523;-3270.217,67.66077;Inherit;False;608.8699;280;Comment;2;518;519;Normal Map;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;556;-4064.376,269.9849;Inherit;True;Property;_Spec;Spec;10;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;518;-3220.217,117.6608;Inherit;True;Property;_Normal;Normal;8;0;Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;519;-2885.347,145.5973;Inherit;False;normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;561;-3615.287,321.3157;Inherit;False;specular;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;105;-2803.912,469.2758;Inherit;False;1048.839;533.1951;Comment;4;0;553;555;562;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;499;-1659.458,533.1132;Inherit;False;936.6855;440.1697;Comment;5;504;503;502;501;522;LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;437;-4130.849,626.5197;Inherit;False;1250.163;397;Comment;4;449;447;444;441;Main texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;555;-2754.273,621.7943;Inherit;False;519;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;438;-4100.452,1212.745;Inherit;False;3530.671;1034.709;Comment;35;548;477;474;492;471;472;464;547;463;454;455;494;544;542;469;495;465;534;458;452;453;462;535;459;456;457;450;451;445;446;442;443;439;440;550;RevealLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;562;-2753.377,701.963;Inherit;False;561;specular;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;458;-3562.561,1374.208;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;450;-3376.075,1954.255;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;452;-3821.492,1379.406;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.NormalizeNode;451;-3274.471,1799.808;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;457;-3248.92,2080.087;Float;False;Property;_DifferenceThreshold;Difference Threshold;3;0;Create;True;0;0;False;0;0.05;0.05;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;441;-4080.849,676.5197;Inherit;True;Property;_RevealLightTexture;Reveal Light Texture;0;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DotProductOpNode;456;-3102.494,1886.83;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;459;-3131.221,1987.065;Float;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;444;-3811.847,864.5197;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;469;-2262.764,1647.369;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;465;-3212.306,1645.608;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCIf;462;-2951.5,1886.105;Inherit;False;6;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;447;-3452.812,714.4095;Inherit;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;542;-2573.324,1942.728;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;453;-3820.188,1288.79;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;449;-3104.684,728.1525;Inherit;False;mainTexture;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;544;-2079.499,1794.229;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;454;-3005.896,1288.029;Inherit;False;Property;_Reveallightcolor;Reveal light color;2;1;[HDR];Create;True;0;0;False;0;1,0.07075471,0.07075471,0;1,0.07075471,0.07075471,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;503;-1135.858,689.8417;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;495;-2886.581,2118.028;Inherit;False;Property;_SelfEmittingValue;SelfEmittingValue;7;0;Create;True;0;0;False;0;0.05;0.51;0;0.05;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;446;-3513.168,1781.163;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;494;-1936.047,1732.613;Inherit;False;Property;_IsSelfEmitting;IsSelfEmitting;6;0;Create;True;0;0;False;0;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;442;-3822.73,1745.631;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CustomStandardSurface;553;-2467.713,572.9057;Inherit;False;Specular;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;464;-1426.841,1851.95;Inherit;False;Property;_Contrast;Contrast;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;472;-1482.162,1602.834;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;534;-2731.892,1804.307;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;550;-2782.171,1706.051;Inherit;False;Constant;_Float2;Float 2;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;548;-2591.601,1748.369;Inherit;False;Property;_isDependingOnLightDir;isDependingOnLightDir;9;0;Create;True;0;0;False;0;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;471;-1203.915,1896.701;Inherit;False;Property;_RevealLightEnabled;RevealLightEnabled;5;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;492;-1266.404,1702.193;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;504;-925.9117,694.5279;Inherit;False;lightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;445;-3617.858,1948.67;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;474;-955.576,1740.972;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;535;-2968.724,1785.315;Inherit;False;504;lightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;477;-811.6868,1671.6;Inherit;False;handpaint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;522;-1629.028,632.2754;Inherit;False;519;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;463;-2725.178,1462.67;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;455;-3018.035,1492.83;Inherit;False;449;mainTexture;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;501;-1535.559,769.5417;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightColorNode;440;-4052.989,1744.04;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.WorldSpaceLightPos;439;-4022.892,1542.646;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;443;-3901.928,2017.95;Float;False;Property;_ColortoBeFiltered;Color to Be Filtered;1;0;Create;True;0;0;False;0;0.6981132,0.25356,0.25356,1;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;547;-1624.681,1700.896;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;502;-1435.654,590.1135;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1986.201,521.527;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;_Phosphorescence Custom Shader/MaterialRevealLight;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;True;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;519;0;518;0
WireConnection;561;0;556;0
WireConnection;458;0;453;0
WireConnection;458;1;452;0
WireConnection;450;0;445;0
WireConnection;451;0;446;0
WireConnection;456;0;451;0
WireConnection;456;1;450;0
WireConnection;444;2;441;0
WireConnection;469;0;465;0
WireConnection;469;1;462;0
WireConnection;465;0;458;0
WireConnection;465;1;439;2
WireConnection;462;0;456;0
WireConnection;462;1;459;0
WireConnection;462;3;459;0
WireConnection;462;5;457;0
WireConnection;447;0;441;0
WireConnection;447;1;444;0
WireConnection;542;0;462;0
WireConnection;542;3;495;0
WireConnection;449;0;447;4
WireConnection;544;0;469;0
WireConnection;544;1;542;0
WireConnection;503;0;502;0
WireConnection;503;1;501;0
WireConnection;446;0;442;0
WireConnection;494;1;469;0
WireConnection;494;0;544;0
WireConnection;442;0;440;0
WireConnection;442;1;439;2
WireConnection;553;1;555;0
WireConnection;553;3;562;0
WireConnection;472;0;463;0
WireConnection;472;1;547;0
WireConnection;534;0;462;0
WireConnection;534;1;535;0
WireConnection;548;1;550;0
WireConnection;548;0;534;0
WireConnection;492;0;472;0
WireConnection;492;1;464;0
WireConnection;504;0;503;0
WireConnection;445;0;443;0
WireConnection;474;0;492;0
WireConnection;474;1;471;0
WireConnection;477;0;474;0
WireConnection;463;0;454;0
WireConnection;463;1;455;0
WireConnection;547;0;494;0
WireConnection;502;0;522;0
WireConnection;0;13;553;0
ASEEND*/
//CHKSM=4354065D900D2D5DA7F694078695A82CF47D8953