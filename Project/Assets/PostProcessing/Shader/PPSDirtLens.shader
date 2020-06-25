// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PPSDirtLens"
{
	Properties
	{
		_Lensdirttexture("Lens dirt texture", 2D) = "white" {}
		_DirtMask("DirtMask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Cull Off
		ZWrite Off
		ZTest Always
		
		Pass
		{
			CGPROGRAM

			

			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"

		
			struct ASEAttributesDefault
			{
				float3 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				
			};

			struct ASEVaryingsDefault
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoordStereo : TEXCOORD1;
			#if STEREO_INSTANCING_ENABLED
				uint stereoTargetEyeIndex : SV_RenderTargetArrayIndex;
			#endif
				
			};

			uniform sampler2D _MainTex;
			uniform half4 _MainTex_TexelSize;
			uniform half4 _MainTex_ST;
			
			uniform sampler2D _DirtMask;
			uniform float4 _DirtMask_ST;
			uniform sampler2D _Lensdirttexture;


			
			float2 TransformTriangleVertexToUV (float2 vertex)
			{
				float2 uv = (vertex + 1.0) * 0.5;
				return uv;
			}

			ASEVaryingsDefault Vert( ASEAttributesDefault v  )
			{
				ASEVaryingsDefault o;
				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.texcoord = TransformTriangleVertexToUV (v.vertex.xy);
#if UNITY_UV_STARTS_AT_TOP
				o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
				o.texcoordStereo = TransformStereoScreenSpaceTex (o.texcoord, 1.0);

				v.texcoord = o.texcoordStereo;
				float4 ase_ppsScreenPosVertexNorm = float4(o.texcoordStereo,0,1);

				

				return o;
			}

			float4 Frag (ASEVaryingsDefault i  ) : SV_Target
			{
				float4 ase_ppsScreenPosFragNorm = float4(i.texcoordStereo,0,1);

				float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode69 = tex2D( _MainTex, uv_MainTex );
				float2 uv_DirtMask = i.texcoord.xy * _DirtMask_ST.xy + _DirtMask_ST.zw;
				float dotResult307 = dot( ( unity_CameraToWorld[0].x / unity_CameraToWorld[0].z ) , ( unity_CameraToWorld[0].y / unity_CameraToWorld[0].z ) );
				float2 temp_cast_0 = (dotResult307).xx;
				float2 uv0246 = i.texcoord.xy * float2( 1,1 ) + temp_cast_0;
				float4 blendOpSrc217 = tex2DNode69;
				float4 blendOpDest217 = ( tex2D( _DirtMask, uv_DirtMask ) * tex2D( _Lensdirttexture, uv0246 ) );
				

				float4 color = ( saturate( ( 1.0 - ( 1.0 - blendOpSrc217 ) * ( 1.0 - blendOpDest217 ) ) ));
				
				return color;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18000
-1920;171;1920;1019;1841.138;-462.2135;1.544176;True;False
Node;AmplifyShaderEditor.CameraToWorldMatrix;311;-1732.906,1283.525;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.VectorFromMatrixNode;312;-1480.906,1278.525;Inherit;False;Row;0;1;0;FLOAT4x4;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;301;-1183.906,1331.525;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;303;-1176.944,1459.48;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;307;-909.3981,1463.795;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;265;-640.7191,1113.227;Inherit;True;Property;_DirtMask;DirtMask;1;0;Create;True;0;0;False;0;86f26fb0047536c46909ce135114df30;86f26fb0047536c46909ce135114df30;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;246;-746.2319,1436.613;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;82;-337.9034,1319.266;Inherit;True;Property;_Lensdirttexture;Lens dirt texture;0;0;Create;True;0;0;False;0;-1;b6beeebe19990a842b9ce6fa1954ad96;b6beeebe19990a842b9ce6fa1954ad96;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;245;-390.4787,1113.538;Inherit;True;Property;_Mask;Mask;1;0;Create;True;0;0;False;0;-1;86f26fb0047536c46909ce135114df30;86f26fb0047536c46909ce135114df30;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;76;-3570.997,946.6905;Inherit;False;0;0;_MainTex;Pass;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;69;-3403.381,948.4154;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;335;137.6823,1195.933;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;333;-705.9907,2131.937;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;330;-956.5364,2138.534;Inherit;False;0;0;_MainTex_TexelSize;Pass;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;291;-1292.448,1745.474;Inherit;False;Constant;_Color0;Color 0;3;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCGrayscale;290;-1282.448,1654.474;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;289;-846.449,1774.474;Inherit;False;Color Mask;-1;;1;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;1;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;217;599.2957,956.3867;Inherit;False;Screen;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;331;-355.7854,1953.086;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;334;277.3272,1353.034;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;75;1024.052,972.0229;Float;False;True;-1;2;ASEMaterialInspector;0;2;PPSDirtLens;32139be9c1eb75640a847f011acf3bcf;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;1;False;False;False;True;2;False;-1;False;False;True;2;False;-1;True;7;False;-1;False;False;False;0;False;False;False;False;False;False;False;False;False;False;True;2;0;;0;0;Standard;0;0;1;True;False;;0
WireConnection;312;0;311;0
WireConnection;301;0;312;1
WireConnection;301;1;312;3
WireConnection;303;0;312;2
WireConnection;303;1;312;3
WireConnection;307;0;301;0
WireConnection;307;1;303;0
WireConnection;246;1;307;0
WireConnection;82;1;246;0
WireConnection;245;0;265;0
WireConnection;69;0;76;0
WireConnection;335;0;245;0
WireConnection;335;1;82;0
WireConnection;333;0;330;0
WireConnection;290;0;69;0
WireConnection;289;1;290;0
WireConnection;289;3;291;0
WireConnection;217;0;69;0
WireConnection;217;1;335;0
WireConnection;334;1;289;0
WireConnection;75;0;217;0
ASEEND*/
//CHKSM=D5EACAC317DF9663FAB05A479CF34D268390358B