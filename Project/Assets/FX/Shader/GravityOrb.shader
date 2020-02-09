// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GravityOrb"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 2
		_Edgedistance("Edge distance", Float) = 1
		_Edgepower("Edge power", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float4 screenPos;
		};

		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Edgedistance;
		uniform float _Edgepower;
		uniform float _EdgeLength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth152 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth152 = abs( ( screenDepth152 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Edgedistance ) );
			float edge157 = ( ( 1.0 - distanceDepth152 ) * _Edgepower );
			float3 temp_cast_0 = (edge157).xxx;
			o.Emission = temp_cast_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
318;270;1292;627;807.2465;-349.241;1;True;True
Node;AmplifyShaderEditor.RangedFloatNode;153;-641.5521,100.4608;Inherit;False;Property;_Edgedistance;Edge distance;12;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;152;-439.6073,96.89123;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-125.8528,255.112;Inherit;False;Property;_Edgepower;Edge power;13;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;154;-125.9012,104.9825;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;135;-3630.614,1627.168;Inherit;False;5189.865;1755.778;Comment;5;68;75;74;107;99;FlowMap;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;74.14722,146.112;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;140;-1275.975,438.5085;Inherit;False;1400.09;799.6021;Comment;12;0;150;148;149;141;146;147;145;139;143;151;158;Output;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;134;-3578.023,30.64416;Inherit;False;1920.639;1424.128;Comment;22;138;55;62;52;59;48;53;136;132;63;64;126;61;127;60;58;57;54;56;51;49;131;Deformation;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;99;42.06744,1677.168;Inherit;False;1356.305;901.317;Comment;11;124;121;118;116;115;113;111;109;105;104;103;Emissive;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;107;-1850.041,2668.827;Inherit;False;1353.714;675.8963;Comment;9;123;122;120;119;117;114;110;108;44;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;75;-2625.273,2626.081;Inherit;False;694.0061;215.4954;Comment;3;90;85;82;Tilling;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;157;279.1901,164.1553;Inherit;False;edge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;74;-3580.614,2866.221;Inherit;False;1674.772;500.5223;Comment;13;102;101;97;96;95;93;92;91;89;88;87;83;81;FlowUVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;68;-446.9594,2678.807;Inherit;False;1956.211;707.3321;Comment;20;112;106;100;98;94;86;84;80;79;78;77;76;73;72;71;70;69;67;66;65;Time;1,1,1,1;0;0
Node;AmplifyShaderEditor.NegateNode;76;582.1733,3000.009;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;141;-788.1133,665.9452;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;148;-784.7655,813.5373;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;-1899.172,564.7249;Inherit;False;Screenposition;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;143;-1123.296,495.9373;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;147;-949.9649,786.5438;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;146;-953.4323,885.3171;Inherit;False;Property;_Power1;Power;5;0;Create;True;0;0;False;0;0;0.39;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;145;-915.4188,999.0209;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;126;-2105.555,533.993;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;151;-1257.84,724.0198;Inherit;True;Property;_Mask;Mask;11;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenColorNode;60;-2697.995,646.4894;Float;False;Global;_GrabScreen0;Grab Screen 0;1;0;Create;True;0;0;False;0;Object;-1;False;True;1;0;FLOAT4;0,0,0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;127;-2293.168,498.6367;Inherit;False;124;emissive;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;71;-14.46033,2765.02;Inherit;False;Sawtooth Wave;-1;;3;289adb816c3ac6d489f255fc3caf5016;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-2866.199,699.1551;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;1180.144,1961.764;Inherit;False;emissive;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;121;984.6895,1950.066;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GrabScreenPosition;53;-3185.12,550.8054;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;131;-3528.023,900.5371;Inherit;False;123;normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;115;831.842,2388.696;Inherit;False;112;timeBlend;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;149;-691.4185,1001.021;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;59;-2812.847,249.8758;Inherit;False;0.8;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-2405.996,383.2642;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClipNode;150;-391.9005,857.2036;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-379.9867,2840.841;Inherit;False;Property;_Speed;Speed;8;0;Create;True;0;0;False;0;1;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;65;-396.9594,2728.807;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;-207.9736,2762.757;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;55;-3032.624,198.6441;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-3061.075,1332.222;Float;False;Property;_ShadowTransform;Shadow Transform;16;0;Create;True;0;0;False;0;0.5;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;49;-3167.311,979.9873;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScaleAndOffsetNode;57;-2711.312,1189.187;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;132;-1946.933,355.9656;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;70;149.652,2981.965;Inherit;False;Sawtooth Wave;-1;;4;289adb816c3ac6d489f255fc3caf5016;0;1;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;48;-3300.624,80.64418;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;54;-2932.111,1049.587;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;139;-464.8971,521.485;Inherit;False;138;Screenposition;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;69;-66.24059,2964.735;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;62;-2620.647,301.3754;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;136;-2117.686,947.8417;Inherit;False;Shadowtransform;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;63;-2286.52,962.5143;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;61;-2501.012,1109.387;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;51;-3161.111,1151.387;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;52;-3224.324,238.2443;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;116;833.2532,1879.138;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;123;-698.9019,2889.965;Inherit;False;normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;833.2532,2022.176;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;-2547.141,3091.553;Inherit;False;90;tilling;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;98;759.6208,3194.398;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;92;-2481.602,3178.231;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;95;-2652.644,2923.492;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-2849.07,3185.049;Inherit;False;86;timeA;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;94;612.5659,3188.769;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-2683.365,3259.95;Inherit;False;84;timeB;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-2155.268,2707;Inherit;False;tilling;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BlendOpsNode;89;-2962.366,3038.367;Inherit;False;Overlay;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-3362.248,2916.221;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;86;1017.507,2793.534;Inherit;False;timeA;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;83;-3225.965,3158.31;Inherit;False;True;True;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;84;1100.104,2959.155;Inherit;False;timeB;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;85;-2378.023,2682.577;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;81;-3541.227,3152.946;Inherit;True;Property;_FlowMap;FlowMap;7;0;Create;True;0;0;False;0;-1;None;361b48a39764f0f49a3c85a12da3a557;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;813.4351,2802.713;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-2575.274,2676.081;Inherit;False;Constant;_Tilling;Tilling;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;860.2312,2997.445;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;676.144,2903.233;Inherit;False;Property;_Strength;Strength;6;0;Create;True;0;0;False;0;1;1.33;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-2256.965,3163.75;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NegateNode;77;418.0597,2783.065;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-2342.34,2980.853;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;73;361.4439,2985.946;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;113;507.0288,2027.317;Inherit;False;Property;_Emissivecolor;Emissive color;14;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;111;447.253,2222.682;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;122;-929.402,2953.369;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;109;467.1403,1802.795;Inherit;True;Property;_def;def;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;158;-344.1588,620.5605;Inherit;False;157;edge;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;119;-1353.849,2800.502;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;103;93.81997,1757.784;Inherit;True;Property;_Emissive;Emissive;9;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;120;-1354.14,3063.526;Inherit;True;Property;_TextureSample4;Texture Sample 4;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;100;939.6205,3198.398;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;154.8472,2105.578;Inherit;False;101;flowB;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-1737.264,3097.237;Inherit;False;101;flowB;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;108;-1800.042,2718.826;Inherit;True;Property;_Normal;Normal;10;0;Create;True;0;0;False;0;None;302951faffe230848aa0d3df7bb70faa;True;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;112;1296.62,3206.398;Inherit;False;timeBlend;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1736.926,3226.827;Float;False;Property;_NormalScale;Normal Scale;15;0;Create;True;0;0;False;0;0;0.158;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-1724.645,2980.005;Inherit;False;102;flowA;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TFHCRemapNode;72;197.332,2769.001;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-2122.589,3166.267;Inherit;False;flowB;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-2199.417,2957.903;Inherit;False;flowA;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;106;1108.62,3203.398;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;117;-1075.817,3165.298;Inherit;False;112;timeBlend;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;167.465,1988.346;Inherit;False;102;flowA;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-130.8843,488.5082;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;GravityOrb;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;True;0;False;Opaque;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;2;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;4.46;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;152;0;153;0
WireConnection;154;0;152;0
WireConnection;155;0;154;0
WireConnection;155;1;156;0
WireConnection;157;0;155;0
WireConnection;76;0;73;0
WireConnection;141;0;143;0
WireConnection;141;1;151;0
WireConnection;148;0;147;0
WireConnection;148;1;146;0
WireConnection;138;0;126;0
WireConnection;147;0;151;1
WireConnection;126;0;127;0
WireConnection;126;1;60;0
WireConnection;60;0;58;0
WireConnection;71;1;67;0
WireConnection;58;0;53;0
WireConnection;58;1;131;0
WireConnection;124;0;121;0
WireConnection;121;0;116;0
WireConnection;121;1;118;0
WireConnection;121;2;115;0
WireConnection;149;0;145;1
WireConnection;59;0;55;0
WireConnection;64;0;62;0
WireConnection;64;1;60;0
WireConnection;150;0;141;0
WireConnection;150;1;149;0
WireConnection;150;2;148;0
WireConnection;67;0;65;0
WireConnection;67;1;66;0
WireConnection;55;0;48;0
WireConnection;55;1;52;0
WireConnection;49;0;131;0
WireConnection;57;0;54;0
WireConnection;57;1;56;0
WireConnection;57;2;56;0
WireConnection;132;1;126;0
WireConnection;70;1;69;0
WireConnection;48;0;131;0
WireConnection;54;0;49;0
WireConnection;54;1;51;0
WireConnection;69;0;67;0
WireConnection;62;0;59;0
WireConnection;136;0;63;0
WireConnection;63;0;61;0
WireConnection;61;0;57;0
WireConnection;116;0;109;0
WireConnection;116;1;113;0
WireConnection;123;0;122;0
WireConnection;118;0;111;0
WireConnection;118;1;113;0
WireConnection;98;0;94;0
WireConnection;92;0;87;0
WireConnection;92;1;89;0
WireConnection;92;2;88;0
WireConnection;95;0;87;0
WireConnection;95;1;89;0
WireConnection;95;2;91;0
WireConnection;94;0;72;0
WireConnection;90;0;85;0
WireConnection;89;0;87;0
WireConnection;89;1;83;0
WireConnection;86;0;79;0
WireConnection;83;0;81;0
WireConnection;84;0;80;0
WireConnection;85;0;82;0
WireConnection;79;0;77;0
WireConnection;79;1;78;0
WireConnection;80;0;76;0
WireConnection;80;1;78;0
WireConnection;97;0;92;0
WireConnection;97;1;93;0
WireConnection;77;0;72;0
WireConnection;96;0;95;0
WireConnection;96;1;93;0
WireConnection;73;0;70;0
WireConnection;111;0;103;0
WireConnection;111;1;104;0
WireConnection;122;0;119;0
WireConnection;122;1;120;0
WireConnection;122;2;117;0
WireConnection;109;0;103;0
WireConnection;109;1;105;0
WireConnection;119;0;108;0
WireConnection;119;1;110;0
WireConnection;119;5;44;0
WireConnection;120;0;108;0
WireConnection;120;1;114;0
WireConnection;120;5;44;0
WireConnection;100;0;98;0
WireConnection;112;0;106;0
WireConnection;72;0;71;0
WireConnection;101;0;97;0
WireConnection;102;0;96;0
WireConnection;106;0;100;0
WireConnection;0;2;158;0
ASEEND*/
//CHKSM=0F5B4D424A72AC8786388E1DB26385F474F8E900